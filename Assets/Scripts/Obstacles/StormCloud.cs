using UnityEngine;
using System.Collections;

public class StormCloud : BasicCloud
{
    /*
    cloud that kills u if u try to phase tru the bottom

    */

    [SerializeField] private Collider2D zapZone;
    [SerializeField] private float phaseCooldown = 1f;
    [SerializeField] private GameObject lightning;
    [SerializeField] private float downwardBoostStrength; //the strength of the downward boost to apply to the player, relative to player's jump strength (so 0.5 is half the player's jump strength)

    private float phaseCooldownTimer;

    protected override void Start()
    {
        base.Start();
        phaseCooldownTimer = 0f;
    }


    private void Update()
    {
        if (phaseCooldownTimer > 0f)
        {
            phaseCooldownTimer -= Time.deltaTime;
        }
    }


    /*
    if the player collides w/ the hitbox area below cloud, thriggers lightning
    */
    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other); // this fixed the landing behavior

        if (!other.gameObject.CompareTag("Player")) return;
        if (phaseCooldownTimer > 0f) return;
        if (zapZone == null) return;

        bool hitZone = false;
        foreach (ContactPoint2D contact in other.contacts)
        {
            if (contact.collider == zapZone)
            {
                hitZone = true;
                break;
            }
        }
        if (!hitZone) return;

        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController == null || other.rigidbody == null) return;

        playerController.ForceJump(-downwardBoostStrength);
        StrikeLightning(other.rigidbody);
        phaseCooldownTimer = phaseCooldown;
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

        //testing
        StartCoroutine(FlashColor());
        //GameManager.Instance.PlayerDeath();
    }
}