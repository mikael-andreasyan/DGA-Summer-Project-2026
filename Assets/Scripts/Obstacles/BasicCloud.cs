using System;
using UnityEngine;

public class BasicCloud : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // How fast the cloud should bob up & down
    [SerializeField] private float maxDistance = 3f; // How far the cloud moves up
    [SerializeField] private float boostThreshold = 2f; // Where the player can get a boost off the cloud
    private bool isCollidingPlayer;

    private Rigidbody2D playerRB;
    private Rigidbody2D rb;
    private float startY; // Cloud's starting y position

    private bool hasScored; // Whether the GameManager has already scored points for landing on this cloud

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      startY = rb.position.y;
      hasScored = false;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    void FixedUpdate()
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
