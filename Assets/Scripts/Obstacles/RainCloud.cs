using System.Xml;
using UnityEngine;

public class RainCloud : BasicCloud
{
    /*
     * Currently, the cloud has the following behavior:
     *      - Idle sprite can be collided with
     *      - The player collides with the cloud, and it begins 'raining'
     *      - While the player is connected to the cloud the rain timer goes down
     *      - It will turn into vapor and you cannot interact with it until it respawns
     * I would like to discuss/flesh out some of the desired behavior. Right now, it cumulates the total time 
     * that a player has been on the cloud. Maybe it should reset after the player has been off it for a while?
     */

    private enum RainCloudStates { Idle, Raining, Vapor } // Idle = full cloud. Raining = player is on the cloud. Vapor = waiting to respawn.
    [SerializeField] private Sprite idleCloudSprite;
    [SerializeField] private Sprite rainingCloudSprite;
    [SerializeField] private Sprite fadedVaporSprite;

    [SerializeField] private float rainDuration = 5f;
    [SerializeField] private float vaporDuration = 3f;

    private RainCloudStates state = RainCloudStates.Idle;
    private float timer = 0f; // This will count how long a state has been active 
    // **Update: timer has a problem b/c the player bounces off at the top of a jump. Switched to cumulative method.

    protected override void Start()
    {
        base.Start();

        if (idleCloudSprite == null && sr != null)
        {
            idleCloudSprite = sr.sprite;
        }
        sr.sprite = idleCloudSprite;
    }
    

    void Update()
    {
        switch (state)
        {
            case RainCloudStates.Idle:
                if (isCollidingPlayer)
                {
                    state = RainCloudStates.Raining;
                    timer = 0f;
                    sr.sprite = rainingCloudSprite; // Idle -> Raining
                }
                break;

            case RainCloudStates.Raining:
                if (isCollidingPlayer)
                {
                    timer += Time.deltaTime;
                    if (timer >= rainDuration)
                    {
                        StartVapor();
                    }
                }
                break;

            case RainCloudStates.Vapor:
                timer += Time.deltaTime;
                if (timer >= vaporDuration)
                {
                    Respawn(); 
                }
                break;

        }
    }


    protected override void FixedUpdate()
    {
        if (state == RainCloudStates.Vapor)
        {
            return;
        }
        base.FixedUpdate();
    }


    private void StartVapor()
    {
        // Raining -> Vapor
        state = RainCloudStates.Vapor;
        timer = 0f;

        isCollidingPlayer = false; // Has to be false, collision is turning off
        playerRB = null;

        if (col != null)
        {
            col.enabled = false;
        }
        sr.sprite = fadedVaporSprite; 
        rb.linearVelocityY = 0f;
}


    private void Respawn()
    {
        // Vapor -> Idle
        state = RainCloudStates.Idle;
        timer = 0f;
        rb.position = new Vector2(rb.position.x, startY); // What do we want to do with the position? Should we make it smooth back to start or stay in place?
        rb.linearVelocityY = 0f;
        sr.sprite = idleCloudSprite; 
        if (col  != null) // Back to enabled collision
        {
            col.enabled = true;
        }
    }
}
