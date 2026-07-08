using UnityEngine;

/// <summary>
/// Double Jump Boots powerup.
/// When the player touches this in the air, it arms a single boosted jump
/// (stronger than their normal jump) that can be used any time before they
/// touch the ground again. If they land without using it, the boost expires.
/// </summary>
public class DoubleBoots : MonoBehaviour
{
    [Header("Double Boots Settings")]
    [Tooltip("The jump multiplier, decides how big the boost is (Setting this to 2f is 2x jump velocity)")]
    public float boostMultiplier = 2f;

    private bool canBoost = false;
    private PlayerController pc;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canBoost) return; // already armed, ignore further triggers

        if (!other.CompareTag("Player")) return;

        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null) return;

        // Only grant the boost if the player is actually airborne,
        // matching "when the player touches them in the air".
        if (!playerController.isGrounded)
        {
            pc = playerController;
            canBoost = true;

            // Stop this trigger from firing again while the boots
            // are following the player around.
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            SetVisible(false); // hide the pickup while it's "held"
            print("boost ready!");
        }
    }

    private void Update()
    {
        if (!canBoost) return;

        // Follow the player while the boost is armed.
        transform.position = pc.transform.position;

        // If the player lands without using the boost, it expires.
        if (pc.isGrounded)
        {
            canBoost = false;
            Destroy(gameObject);
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            print("boosted!");
            pc.ForceJump(boostMultiplier); // apply the boosted jump directly
            canBoost = false;
            Destroy(gameObject);
        }
    }

    private void SetVisible(bool visible)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = visible;
    }
}