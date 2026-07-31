using UnityEngine;
using System.Collections;

public class Wings : MonoBehaviour
{
    [Header("Wing Boost Settings")]
    [Tooltip("Boost distance that the wings will take the player up")]
    public float boostDistance = 5f;
    [Tooltip("Speed at which the player is boosted upward")]
    public float boostSpeed = 10f;
    private float boostTime;
    [Tooltip("Speed at which the player can move horizontally while boosting")]
    public float wingsMovementSpeed = 5f;

    [Header("Boundaries")]
    [Tooltip("Layer that the left/right boundary wall colliders are on")]
    public LayerMask wallLayerMask;
    [Tooltip("Small gap kept between the player and a wall so they don't overlap it")]
    public float wallSkin = 0.05f;

    [Header("Player Visuals")]
    [Tooltip("Bunny-with-wings sprite drawn by the player's own renderer during the boost")]
    [SerializeField] private Sprite playerWithWings;

    public static bool isBoosting = false;

    private SpriteRenderer wingsRenderer;
    private SpriteRenderer playerRenderer;
    private Animator playerAnimator;

    void Awake()
    {
        boostTime = boostDistance / boostSpeed;
        wingsRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Hides the wings' own sprite and draws the combined bunny-with-wings
    /// artwork on the player instead, so the two don't overlap.
    /// </summary>
    private void EnterWings(Collider2D playerCollider)
    {
        if (wingsRenderer != null)
        {
            wingsRenderer.enabled = false;
        }

        playerRenderer = playerCollider.GetComponent<SpriteRenderer>();
        playerAnimator = playerCollider.GetComponent<Animator>();

        // The Animator keyframes the sprite every frame, so it has to stand
        // down or the swap gets overwritten immediately. Disabling
        // PlayerController isn't enough - the Animator is its own component.
        if (playerAnimator != null)
        {
            playerAnimator.enabled = false;
        }

        if (playerRenderer != null && playerWithWings != null)
        {
            playerRenderer.sprite = playerWithWings;
        }
    }

    /// <summary>
    /// Gives the sprite back to the Animator once the boost is over.
    /// </summary>
    private void ExitWings()
    {
        if (playerAnimator != null)
        {
            playerAnimator.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBoosting)
        {
            ServiceLocator.Get<AudioManager>()?.PlayPowerupPickup();
            StartCoroutine(BoostPlayer(other.transform, other.GetComponent<Rigidbody2D>(), other.GetComponent<PlayerController>(), other));
        }
    }

    IEnumerator BoostPlayer(Transform player, Rigidbody2D playerRb, PlayerController playerController, Collider2D playerCollider)
    {
        isBoosting = true;
        float elapsedTime = 0f;
        Vector2 startPosition = player.position;
        float targetY = startPosition.y + boostDistance;
        float currentX = startPosition.x;

        // Disable the player's own controller for the duration of the boost so its
        // FixedUpdate (movement, jump buffering, ground checks) doesn't fight the
        // position we're setting manually below.
        if (playerController != null)
        {
            playerController.enabled = false;
            playerController.isFlying = true;
        }

        EnterWings(playerCollider);

        // Take manual control of the player's position for the duration of the boost,
        // so gravity/other physics don't fight the upward lerp.
        RigidbodyType2D originalBodyType = playerRb.bodyType;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.bodyType = RigidbodyType2D.Kinematic;

        // Used to keep the player's collider from poking through a wall when clamped
        float halfWidth = playerCollider != null ? playerCollider.bounds.extents.x : 0.5f;

        while (elapsedTime < boostTime)
        {
            // Read horizontal input each frame and let the player steer while boosting
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float t = elapsedTime / boostTime;
            float y = Mathf.Lerp(startPosition.y, targetY, t);

            float proposedX = currentX + horizontalInput * wingsMovementSpeed * Time.deltaTime;
            currentX = ClampToWalls(currentX, proposedX, y, halfWidth);

            Vector2 newPos = new Vector2(currentX, y);
            player.position = newPos;
            transform.position = newPos; // Move the wings along with the player

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.position = new Vector2(currentX, targetY); // Ensure the player reaches the target height

        // Restore normal physics and hand off velocity so momentum carries smoothly
        playerRb.bodyType = originalBodyType;
        float finalHorizontalInput = Input.GetAxisRaw("Horizontal");
        playerRb.linearVelocity = new Vector2(finalHorizontalInput * wingsMovementSpeed, boostSpeed);

        ExitWings();

        // Hand control back to the player's own controller
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.isFlying = false;
        }

        isBoosting = false;
        Destroy(gameObject); // Destroy the wings after boosting the player
    }

    /// <summary>
    /// Raycasts from the player's current X toward the proposed X. If a collider on
    /// wallLayerMask is in the way, the movement is clamped so the player's edge
    /// (accounting for halfWidth + wallSkin) stops right at the wall instead of
    /// passing through it.
    /// </summary>
    private float ClampToWalls(float fromX, float proposedX, float y, float halfWidth)
    {
        float delta = proposedX - fromX;
        if (Mathf.Approximately(delta, 0f))
        {
            return proposedX;
        }

        float direction = Mathf.Sign(delta);
        float castDistance = Mathf.Abs(delta) + halfWidth + wallSkin;

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(fromX, y), Vector2.right * direction, castDistance, wallLayerMask);
        if (hit.collider != null)
        {
            return hit.point.x - direction * (halfWidth + wallSkin);
        }

        return proposedX;
    }
}