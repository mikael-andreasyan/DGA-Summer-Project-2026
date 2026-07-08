using System;
using UnityEngine;

public class BasicCloud : MonoBehaviour
{
    [SerializeField] protected float speed = 2f; // How fast the cloud should bob up & down
    [SerializeField] protected float maxDistance = 3f; // How far the cloud moves up
    [SerializeField] protected float boostThreshold = 2f; // Where the player can get a boost off the cloud
    protected bool isCollidingPlayer;

    protected Rigidbody2D playerRB;
    protected Rigidbody2D rb;
    protected float startY; // Cloud's starting y position
    protected Collider2D col; // Added to parent Cloud class so we can alter collision in rain/storm cloud
    protected SpriteRenderer sr; // Added to parent Cloud class so we can change sprites in subclesses

    protected bool hasScored; // Whether the GameManager has already scored points for landing on this cloud

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        startY = rb.position.y;
        hasScored = false;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected virtual void FixedUpdate()
    {
        if (rb.position.y >= startY + maxDistance)
        {
            rb.linearVelocityY = speed * -1; // Start moving down once cloud has reached certain height
        }
        else if (rb.position.y <= startY)
        {
            rb.linearVelocityY = 0; // Stop moving down if cloud is too low
        }

        if (rb.position.y <= startY && playerRB != null && isCollidingPlayer 
        && playerRB.position.y > rb.position.y && playerRB.linearVelocity.y <= 0)
        {
            rb.linearVelocityY = speed; // Start moving up if player is on top of it
            if (!hasScored)
            {
                // GameManager.Instance.RegisterCloudBounce();
                // hasScored = true;
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isCollidingPlayer = true;
            if (playerRB == null)
            {
                playerRB = other.gameObject.GetComponent<Rigidbody2D>(); // Get reference to player
            }
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D other)
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
