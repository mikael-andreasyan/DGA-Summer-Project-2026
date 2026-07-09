using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class RainCloud : BasicCloud
{
    /*
     * Currently, the cloud has the following behavior:
     *      - Idle sprite can be collided with
     *      - The player collides with the cloud, and it begins 'raining'
     *      - While the player is connected to the cloud the rain timer goes down
     *      - It will turn into vapor and you cannot interact with it until it respawns
     *      - There is a rain timer before it goes back to idle.
     * I would like to discuss/flesh out some of the desired behavior. Right now, it cumulates the total time 
     * that a player has been on the cloud. Do we want it to reset after the player has been off it for a while?
     */

    private enum RainCloudStates { Idle, Raining, Vapor } // Idle = full cloud. Raining = player is on the cloud. Vapor = waiting to respawn.
    [SerializeField] private Sprite idleCloudSprite;
    [SerializeField] private Sprite rainingCloudSprite;
    [SerializeField] private Sprite fadedVaporSprite;

    [SerializeField] private float rainDuration = 3f;
    [SerializeField] private float vaporDuration = 4f;
    [SerializeField] private float resetTimer = 3f;

    private RainCloudStates state = RainCloudStates.Idle;
    private float timer = 0f; // This will count how long a state has been active 
    private float offTimer = 0f; // This one counts how long the player has been off, in order to reset the rain.
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
                    offTimer = 0f;
                    sr.sprite = rainingCloudSprite; // Idle -> Raining
                }
                break;

            case RainCloudStates.Raining:
                if (isCollidingPlayer)
                {
                    offTimer = 0f;
                    timer += Time.deltaTime;
                    if (timer >= rainDuration)
                    {
                        StartVapor();
                    }
                }
                else
                {
                    offTimer += Time.deltaTime;
                    if (offTimer >= resetTimer)
                    {
                        Respawn(); // Might want to change this to a new function when we have animations and sprites
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

    private void StartVapor()
    {
        // Raining -> Vapor
        state = RainCloudStates.Vapor;
        timer = 0f;
        sr.sprite = fadedVaporSprite;

        // Turning off player-cloud collision for this state
        isCollidingPlayer = false; 
        playerRB = null;
        if (col != null)
        {
            col.enabled = false;
        } 
    }


    private void Respawn()
    {
        // Vapor -> Idle
        state = RainCloudStates.Idle;
        timer = 0f;
        offTimer = 0f;
        sr.sprite = idleCloudSprite; 

        if (col != null) // Back to enabled collision
        {
            col.enabled = true;
        }
    }
}

