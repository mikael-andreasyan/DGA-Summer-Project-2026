using System;
using System.Collections;
using UnityEngine;

public class BasicCloud : MonoBehaviour
{   
    // Cloud bobbing
    [SerializeField] protected float downSpeed = 5f; // How fast the cloud should bob down
    [SerializeField] protected float upSpeed = 3f; // How fast the cloud should bob up
    [SerializeField] protected float settleSpeed = 2f; // How fast the cloud settle back into place after bobbing up
    [SerializeField] protected float maxDistanceDown = 3f; // How far the cloud moves down while bobbing
    [SerializeField] protected float maxDistanceUp = 3f; // How far the cloud moves up while bobbing
    [SerializeField] protected float boostThreshold = 2f; // Where the player can get a boost off the cloud
    protected bool isCollidingPlayer;

    protected bool isOnTop; // Whether the player is currently resting on top of the cloud


    protected Rigidbody2D rb;
    protected Rigidbody2D playerRB;
    protected Collider2D playerCol;
    protected float startY; // Cloud's starting y position
    protected Collider2D col; // Added to parent Cloud class so we can alter collision in rain/storm cloud
    [SerializeField] protected Collider2D leftBoost; // Left boost hitbox
    [SerializeField] protected Collider2D middleBoost; // Middle boost hitbox
    [SerializeField] protected Collider2D rightBoost; // Right boost hitbox
    
    protected SpriteRenderer sr; // Added to parent Cloud class so we can change sprites in subclesses

    protected bool justLanded; // Flag to set whether cloud should start bobbing
    protected bool hasScored; // Whether the GameManager has already scored points for landing on this cloud

    protected bool isSettling; // Whether the cloud is settling downwards after bobbing 

    // Weakpoint stuff 
    protected float landingTime; // The time the player landed on the cloud
    protected bool canWeakpointBoost; // Whether the player can get a weakpoint boost off this cloud by jumping now
    protected int chosenWeakpoint = -1; // Which weakpoint the cloud has. 0 left, 1 middle, 2 right
    [SerializeField] protected float weakpointLifetime = 1; // How many seconds player has to get weakpoint boost after landing
    protected bool weakpointExpired; // Whether the weakpoint has expired

    protected Vector2 lastPosition;

    private bool playerCurrentlyOnCloud;

    public ParticleSystem particleSystem;


    protected void Awake()
    {
        chosenWeakpoint = UnityEngine.Random.Range(0, 3); // Randomly choose weakpoint; may change later
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        startY = transform.position.y;
        hasScored = false;
        isSettling = false;
        landingTime = -1;
        canWeakpointBoost = false;
        weakpointExpired = false;
    }


    protected virtual void FixedUpdate()
    {
        if (isSettling && rb.position.y <= startY)
        {
            isSettling = false;
            rb.linearVelocityY = 0;
        }
        else if (rb.position.y >= startY + maxDistanceUp)
        {
            isSettling = true;
            rb.linearVelocityY = settleSpeed * -1; // Start settling down once cloud has reached certain height
        }
        else if (rb.position.y <= startY - maxDistanceDown)
        {
            rb.linearVelocityY = upSpeed; // Stop moving up if cloud is too low
        }

        else if (justLanded)
        {
            justLanded = false;
            landingTime = Time.time;
            if (!weakpointExpired)
            {
                StartCoroutine(startWeakpointExpirationTimer());
            }
            rb.linearVelocityY = downSpeed * -1;
            if (!hasScored)
            {
                GameManager.Instance.RegisterCloudBounce();
                hasScored = true;
            }
        }

        // if (isOnTop && playerCol != null)
        // {
        //     float cloudTop = col.bounds.max.y;
        //     float playerBottom = playerCol.bounds.min.y;

        //     if (playerBottom < cloudTop)
        //     {
        //         float correction = cloudTop - playerBottom;
        //         float maxCorrectionPerStep = 0.1f;
        //         float applied = Mathf.Min(correction, maxCorrectionPerStep);
        //         playerRB.position += new Vector2(0f, applied);
        //     }
        // }
    }

    // protected virtual void OnCollisionEnter2D(Collision2D other) {
    //     if(other.gameObject.CompareTag("Player"))
    //     {
    //         isCollidingPlayer = true;
    //         if (playerRB == null)
    //         {
    //             playerRB = other.gameObject.GetComponent<Rigidbody2D>(); // Get reference to player
    //             playerCol = other.gameObject.GetComponent<Collider2D>();
    //         }

    //         bool onTopThisFrame = false;

    //         foreach (ContactPoint2D contact in other.contacts)
    //         {
    //             if (playerCol.bounds.min.y >= col.bounds.max.y - 0.5f &&
    //             playerRB.linearVelocity.y <= 0f)
    //             {
    //                 onTopThisFrame = true;
    //                 break;
    //             }
    //         }

    //         if (onTopThisFrame && !isOnTop)
    //         {
    //             justLanded = true;
    //         }

    //         isOnTop = onTopThisFrame;
    //     }
    // }
    // protected virtual void OnCollisionExit2D(Collision2D other)
    // {
    //     if(other.gameObject.CompareTag("Player"))
    //     {
    //         isCollidingPlayer = false;
    //         isOnTop = false; 
    //     }
    // }

    // // In case player is in contact with the cloud the whole time   
    // protected virtual void OnCollisionStay2D(Collision2D other)
    // {
    //     if (!other.gameObject.CompareTag("Player")) return;


    //     bool onTopThisFrame = false;
    //     foreach (ContactPoint2D contact in other.contacts)
    //     {
    //         // Debug.Log($"enabled={contact.enabled}, normal.y={contact.normal.y}");
    //         if (playerCol.bounds.min.y >= col.bounds.max.y - 0.5f && 
    //         playerRB.linearVelocity.y <= 0f)
    //         {
    //             onTopThisFrame = true;
    //             break;
    //         }
    //     }

    //     if (onTopThisFrame && !isOnTop)
    //     {
    //         justLanded = true; // transitioned from passing-through/no-contact to actually resting on top
    //     }

    //     isOnTop = onTopThisFrame;
    // }   

    public bool isBoostAvailable() // Whether the player gets a boost from jumping off the cloud
    {
        // If the cloud is moving up, and it's above the boost threshold distance, then
        // the player can get a boost
        return (rb.linearVelocity.y > 0); // just check whether cloud is moving up for now
         // && rb.position.y >= startY + boostThreshold
    }

    public bool isWeakpointAvailable() // Whether the player gets a special weakpoint boost
    {
       return canWeakpointBoost; 
    } 

    // Weakpoint methods called by children

    public bool isActiveWeakpoint(int num) // 0 for left, 1 for middle, 2 for right
    {
        // Debug.Log(num);
        // Debug.Log(chosenWeakpoint);
        return num == chosenWeakpoint;
    }

    public void registerChildCollision(int child) // Called by child colliders for weakpoint boost
    // 0 for left, 1 for middle, 2 for right
    {
        if (child == chosenWeakpoint && !weakpointExpired)
        {
            canWeakpointBoost = true;
            // Debug.Log("Player can weakpoint boost now");
        }
    }

    public void registerChildCollisionExit(int child) // Called by child colliders for weakpoint boost
    // 0 for left, 1 for middle, 2 for right
    {
        if (child == chosenWeakpoint && !weakpointExpired)
        {
            canWeakpointBoost = false;
            // Debug.Log("Player can no longer weakpoint boost");
        }
    }
    
    IEnumerator startWeakpointExpirationTimer()
    {
        yield return new WaitForSeconds(weakpointLifetime);
        weakpointExpired = true;
        canWeakpointBoost = false;
        foreach (Transform child in transform)
        {
            if (child.gameObject!=particleSystem.gameObject)
            {
               Destroy(child.gameObject); 
            }
        }
        // Debug.Log("Weakpoint expired");
    }


    public void PlayerLanded()
    {
        if (playerCurrentlyOnCloud)
            return;

        isCollidingPlayer = true;
        playerCurrentlyOnCloud = true;
        justLanded = true;
        Debug.Log("Playing particles");
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        particleSystem.Play();
    }

    public void PlayerLeft()
    {
        isCollidingPlayer = false;
        playerCurrentlyOnCloud = false;
    }

}
