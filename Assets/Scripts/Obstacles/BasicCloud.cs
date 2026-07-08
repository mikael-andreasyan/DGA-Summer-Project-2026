using System;
using UnityEngine;

public class BasicCloud : MonoBehaviour
{
    [SerializeField] protected float downSpeed = 5f; // How fast the cloud should bob down
    [SerializeField] protected float upSpeed = 3f; // How fast the cloud should bob up
    [SerializeField] protected float settleSpeed = 2f; // How fast the cloud settle back into place after bobbing up
    [SerializeField] protected float maxDistanceDown = 3f; // How far the cloud moves down while bobbing
    [SerializeField] protected float maxDistanceUp = 3f; // How far the cloud moves up while bobbing
    [SerializeField] protected float boostThreshold = 2f; // Where the player can get a boost off the cloud
    protected bool isCollidingPlayer;

    protected Rigidbody2D playerRB;
    protected Rigidbody2D rb;
    protected float startY; // Cloud's starting y position

    protected bool hasScored; // Whether the GameManager has already scored points for landing on this cloud

    protected bool isSettling; // Whether the cloud is settling downwards after bobbing 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      startY = rb.position.y;
      hasScored = false;
      isSettling = false;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    void FixedUpdate()
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

        if (rb.position.y <= startY && playerRB != null && isCollidingPlayer 
        && playerRB.position.y > rb.position.y && playerRB.linearVelocity.y <= 0)
        {
            rb.linearVelocityY = downSpeed * -1; // Start moving down if player is on top of it
            if (!hasScored)
            {
                 GameManager.Instance.RegisterCloudBounce();
                 hasScored = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isCollidingPlayer = true;
            if (playerRB == null)
            {
                playerRB = other.gameObject.GetComponent<Rigidbody2D>(); // Get reference to player
            }
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isCollidingPlayer = false;
        }
    }

    public bool isBoostAvailable() // Whether the player gets a boost from jumping off the cloud
    {
        // If the cloud is moving up, and it's above the boost threshold distance, then
        // the player can get a boost
        return (rb.linearVelocity.y >= 0 && rb.position.y >= startY + boostThreshold);
    }
}
