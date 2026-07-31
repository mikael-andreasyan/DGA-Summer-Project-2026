using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Numerics;

public class BarrelLauncher : MonoBehaviour
{
    [SerializeField] public float angularSpeed;
    [SerializeField] public float angleHalfBound;
    [SerializeField] public float verticalDisplacement;

    [SerializeField] public float launchSpeed;
    [SerializeField] public float launchDistance;
    [SerializeField] public LayerMask wallLayer;

    [Header("Player Visuals")]
    [SerializeField] private Sprite playerInBarrel;

    private UnityEngine.Vector2 a;
    private UnityEngine.Vector2 b;
    private float t;
    private bool timeIncreasing;
    private UnityEngine.Vector2 directionalVector;
    private UnityEngine.Vector2 boostVector;

    private Transform player;
    private PlayerController playerController;
    private Rigidbody2D playerRB;
    private SpriteRenderer playerRenderer;
    private Animator playerAnimator;
    private SpriteRenderer barrelRenderer;

    private static bool isBoosting;
    private bool isWaiting;

   void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !Wings.isBoosting && !isBoosting)
        {
            playerController = other.GetComponent<PlayerController>();
            playerRB = other.GetComponent<Rigidbody2D>();

            playerController.enabled = false;
            playerRB.linearVelocity = new UnityEngine.Vector2(0,0);

            player = other.gameObject.transform;
            player.position = transform.position;

            enterBarrel(other);

            isWaiting = true;
        }
    }


    void enterBarrel(Collider2D playerCollider)
    {
        if (barrelRenderer != null)
        {
            barrelRenderer.enabled = false;
        }

        playerRenderer = playerCollider.GetComponent<SpriteRenderer>();
        playerAnimator = playerCollider.GetComponent<Animator>();

        playerAnimator.enabled = false;

        if (playerRenderer != null && playerInBarrel != null)
        {
            playerRenderer.flipX = false; 
            playerRenderer.sprite = playerInBarrel;
        }
    }

    void exitBarrel()
    {
        playerAnimator.enabled = true;

        if (player != null)
        {
            player.rotation = UnityEngine.Quaternion.identity;
        }
    }

    void Start()
    {
        barrelRenderer = GetComponent<SpriteRenderer>();
        establishAngles();
        t = 0f;
        timeIncreasing = true;
        isWaiting = false;
        isBoosting = false;
    }

    void Update()
    {
        if (isWaiting)
        {
            Debug.Log("Barrel time: " + t);
            handlePreLaunch();
        }
    }

    void establishAngles()
{
    UnityEngine.Vector2 objectPos = (UnityEngine.Vector2) transform.position;
    UnityEngine.Vector2 rightTranslation = new UnityEngine.Vector2(angleHalfBound, verticalDisplacement);
    UnityEngine.Vector2 leftTranslation = new UnityEngine.Vector2(-angleHalfBound, verticalDisplacement);

    a = rightTranslation;   
    b = leftTranslation;
}

    void FlowTime()
    {
        if (timeIncreasing)
        {
            increaseTime();
            if (t >= 1f)
            {
                t = 1f;
                timeIncreasing = false;
            }
        }
        else
        {
            decreaseTime();
            if (t <= 0f)
            {
                t = 0f;
                timeIncreasing = true;
            }
        }
    }

    void increaseTime()
    {
        
        t += Time.deltaTime / angularSpeed;
    }

    void decreaseTime()
    {
        
        t -= Time.deltaTime / angularSpeed;
    }

    void reverseTime()
    {
        if (timeIncreasing)
        {
            timeIncreasing = false;
        }
        else
        {
            timeIncreasing = true;
        }
    }


    IEnumerator Boost()
    {
    isBoosting = true;

    UnityEngine.Vector2 initialPos = transform.position;
    UnityEngine.Vector2 finalPos = (UnityEngine.Vector2)transform.position + boostVector;

    float elapsedTime = 0f;
    float maxTime = launchDistance / launchSpeed;

    UnityEngine.Vector2 currentPos = initialPos;

    while (elapsedTime < maxTime)
    {
        float lerpT = elapsedTime / maxTime;

        UnityEngine.Vector2 nextPos = UnityEngine.Vector2.Lerp(initialPos, finalPos, lerpT);

        // Check the path from where we are now to where we're about to move
        RaycastHit2D hit = Physics2D.Linecast(currentPos, nextPos, wallLayer);

        if (hit.collider != null)
        {
            // Stop at the point of impact instead of moving into the wall
            transform.position = hit.point;
            player.position = hit.point;
            break;
        }

        transform.position = nextPos;
        player.position = nextPos;
        currentPos = nextPos;

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    if (elapsedTime >= maxTime)
    {
        // Only snap to the exact final position if we weren't cut short by a wall
        player.position = finalPos;
    }

    exitBarrel();

    playerController.enabled = true;
    playerRB.linearVelocity = boostVector;

    isBoosting = false;

    Destroy(gameObject);
    }

    void handlePreLaunch()
    {
        
        FlowTime();

        directionalVector = UnityEngine.Vector2.Lerp(a, b, t);
        // boostVector = directionalVector.normalized * launchDistance;
        boostVector = directionalVector.normalized * launchSpeed;
        transform.up = directionalVector.normalized;
        transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y, 10);

        player.rotation = transform.rotation;

        if (Input.GetButtonDown("Jump"))
        {
            isWaiting = false;
            // StartCoroutine(Boost());
            exitBarrel();
            playerRB.AddForce(boostVector, ForceMode2D.Impulse);
            playerController.enabled = true;
            playerController.Launch();
            exitBarrel();
            Destroy(gameObject);
        }
    }


}
