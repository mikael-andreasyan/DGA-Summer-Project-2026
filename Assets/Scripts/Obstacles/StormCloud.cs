using UnityEngine;
using System.Collections;

public class StormCloud : BasicCloud
{
    /*
    cloud that kills u if u try to phase tru the bottom

    */
    [Header("Cloud Behaviors")]
    [SerializeField] private float phaseCooldown = 1f;
    [SerializeField] private float downwardBoostStrength; //the strength of the downward boost to apply to the player, relative to player's jump strength (so 0.5 is half the player's jump strength)

    [Header("Lightning/Spark Obstacles")]
    [SerializeField] private Collider2D zapZone;
    [SerializeField] private GameObject lightning;
    [SerializeField] private SpriteRenderer lightningSprite;
    [SerializeField] private Animator lightningAnimator;

    [Header("Player Effects")]
    [SerializeField] private float zapDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip lightningStrikeSound;

    private float phaseCooldownTimer;

    protected override void Start()
    {
        base.Start();
        phaseCooldownTimer = 0f;

        if (lightningSprite != null)
        {
            lightningSprite.enabled = false;
        }
    }


    private void Update()
    {
        if (phaseCooldownTimer > 0f)
        {
            phaseCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ZapZoneCollide(other);
    }

    private void ZapZoneCollide(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (phaseCooldownTimer > 0f) {
            return;
        }

        // BUG FIX: This SHOULD place a safeguard on when the bottom of the player is on the top of the cloud
        // If it is on top of the cloud, return; no collide.
        if (col != null && other.bounds.min.y >= col.bounds.max.y - 0.05f) // 0.05 can be adjusted. Just a margin
        {
            return;
        }

        PlayerController playerController = other.GetComponent<PlayerController>();
        Rigidbody2D playerRB = other.attachedRigidbody;
        if (playerController == null  || playerRB == null)
        {
            return;
        }

        playerController.ForceJump(-downwardBoostStrength);
        playerController.Stun(zapDuration);
        StrikeLightning(playerRB);

        phaseCooldownTimer = phaseCooldown;
    }


    protected void OnCollisionEnter2D(Collision2D other)
    {
        // Base? 
    }


    /*testing, makes clould turn color when "striking" player
    */
    private IEnumerator FlashColor()
    {
        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(2f);
        sr.color = originalColor;
    }


    private void StrikeLightning(Rigidbody2D playerRb)
    {
        if (lightning != null)
        {
            Instantiate(lightning, playerRb.position, Quaternion.identity);
        }

        if (lightningSprite != null)
        {
            StartCoroutine(handleLightningVisibility());
        }

        if (lightningAnimator != null)
        {
            lightningAnimator.Play("Lightning", 0, 0f);
        }

        ServiceLocator.Get<AudioManager>()?.PlaySFX(lightningStrikeSound);

        //testing
        StartCoroutine(FlashColor());
        //GameManager.Instance.PlayerDeath();
    }

    private IEnumerator handleLightningVisibility()
    {
        lightningSprite.enabled = true;

        yield return null;
        AnimatorStateInfo info = lightningAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        lightningSprite.enabled = false;
    }
}