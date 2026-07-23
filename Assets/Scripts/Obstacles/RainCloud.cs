using NUnit.Framework;
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
    [Header("Sprite Refs")]
    [SerializeField] private Sprite idleCloudSprite;
    [SerializeField] private Sprite rainingCloudSprite;
    [SerializeField] private Sprite fadedVaporSprite;

    [Header("Duration Timers")]
    [SerializeField] private float rainDuration = 0.6f;
    [SerializeField] private float vaporDuration = 2f;

    [Header("Flashing Window")]
    [SerializeField] private float flashWindow = 0.5f;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private float flashAlpha = 0.3f;

    private RainCloudStates state = RainCloudStates.Idle;
    private float timer = 0f; // This will count how long a state has been active 
    private float flashTimer = 0f;
    private bool flashVis = true;

    
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
                timer += Time.deltaTime;
                float remainingTime = rainDuration - timer;
                if (remainingTime <= flashWindow)
                {
                    FlashWarning();
                }
                else if (!flashVis)
                {
                    ResetFlash();
                }
                if (timer >= rainDuration)
                {
                    StartVapor();
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
        ResetFlash();
        sr.sprite = fadedVaporSprite;

        // Turning off player-cloud collision for this state
        isCollidingPlayer = false; 
        playerRB = null;
        if (col != null)
        {
            col.enabled = false;
        } 
    }

    
    private void FlashWarning()
    {
        flashTimer += Time.deltaTime;
        if (flashTimer >= flashInterval)
        {
            flashTimer = 0f;
            flashVis = !flashVis;
            SetAlpha(flashVis ? 1f : flashAlpha);
        }
    }


    private void SetAlpha(float alpha)
    {
        if (sr == null)
        {
            return;
        }
        Color temp = sr.color;
        temp.a = alpha;
        sr.color = temp;
    }


    private void ResetFlash()
    {
        flashTimer = 0f;
        flashVis = true;
        SetAlpha(1f);
    }


    private void Respawn()
    {
        // Vapor -> Idle
        state = RainCloudStates.Idle;
        timer = 0f;
        ResetFlash();
        sr.sprite = idleCloudSprite; 

        if (col != null) // Back to enabled collision
        {
            col.enabled = true;
        }
    }
}

