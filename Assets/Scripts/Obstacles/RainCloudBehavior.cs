using UnityEngine;
using UnityEngine.Rendering;

public class RainCloudBehavior : CloudBehavior
{
    public float timeToDissipate = 8f;
    public float reformTime = 5f;
    public float intermediate = 1f;

    private float playerOnTime = 0f;
    private bool isDissipated = false;
    private float formTimer = 0f;

    protected override void Update()
    {
        if (isDissipated)
        {
            formTimer -= Time.deltaTime;
            if (formTimer <= 0f)
            {
                Reform();
            }
            return;
        }
        base.Update();
        
        if (isPlayerOn)
        {
            playerOnTime += Time.deltaTime;
            if (playerOnTime >= timeToDissipate)
            {
                Dissipate();
            }
        }
    }


    protected override void FixedUpdate()
    {
        if (isDissipated)
        {
            return;
        }
        base.FixedUpdate();
    }

    private void Dissipate()
    {
        isDissipated = true;
        isPlayerOn = false;

        col.enabled = false;
        sr.sprite = null;

        formTimer = reformTime;
        
    }

    private void Reform()
    {
        isDissipated = false;

        rb.position = startPosition;
        driftClock = 0f;
        playerOnTime = 0f;
        isJumpAvailable = true;
        timer = collideDuration;
        sr.sprite = collideSprite;
        col.enabled = true;


    }

}
