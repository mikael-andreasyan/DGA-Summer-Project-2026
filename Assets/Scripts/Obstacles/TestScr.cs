using UnityEngine;
using System.Collections;

public class TestScr : BasicCloud
{
    /*
    Storm cloud: top-landing behavior is exactly BasicCloud's, untouched (via base.OnCollisionEnter2D).

    A second, fully solid collider (bottomBlocker) covers only the underside of the cloud.
    Because it's NOT "Used By Effector", it's solid from every direction - the player
    physically cannot pass through it, same as any normal wall. Unity's physics engine blocks
    the movement itself; there's no trigger/script needed to "catch" the player mid-phase.

    When a collision contact specifically involves bottomBlocker (identified via
    ContactPoint2D.collider, since both colliders live on this one GameObject), we apply the
    zap/knockback.
    */

    [SerializeField] private Collider2D bottomBlocker; // solid collider covering only the underside
    [SerializeField] private float phaseCooldown = 1f;
    [SerializeField] private GameObject lightning;
    [SerializeField] private float downwardBoostStrength; // relative to player's jump strength (0.5 = half)

    private float phaseCooldownTimer;

    protected override void Start()
    {
        base.Start();
        phaseCooldownTimer = 0f;
    }

    private void Update()
    {
        // NOTE: removed the manual "base.FixedUpdate();" call that was here before - BasicCloud
        // already declares FixedUpdate() as a real Unity method, so it runs automatically every
        // physics step on its own. Calling it again from Update() was running the bobbing logic
        // an inconsistent number of extra times per second, on top of Unity's own call - that's
        // worth knowing about if bobbing ever looked erratic before.
        if (phaseCooldownTimer > 0f)
        {
            phaseCooldownTimer -= Time.deltaTime;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other); // exact same top-landing behavior as BasicCloud, untouched

        if (!other.gameObject.CompareTag("Player")) return;
        if (phaseCooldownTimer > 0f) return;
        if (bottomBlocker == null) return;

        // Only proceed if THIS specific contact involved the bottom collider - not the top
        // landing collider, which also lives on this same GameObject.
        bool hitBottom = false;
        foreach (ContactPoint2D contact in other.contacts)
        {
            if (contact.collider == bottomBlocker)
            {
                hitBottom = true;
                break;
            }
        }
        if (!hitBottom) return;

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