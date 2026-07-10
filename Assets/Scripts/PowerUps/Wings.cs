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

    void Awake()
    {
        boostTime = boostDistance / boostSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(BoostPlayer(other.transform, other.GetComponent<Rigidbody2D>(), other.GetComponent<PlayerController>()));
        }
    }

    IEnumerator BoostPlayer(Transform player, Rigidbody2D playerRb, PlayerController playerController)
    {
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
        }

        // Take manual control of the player's position for the duration of the boost,
        // so gravity/other physics don't fight the upward lerp.
        RigidbodyType2D originalBodyType = playerRb.bodyType;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.bodyType = RigidbodyType2D.Kinematic;

        while (elapsedTime < boostTime)
        {
            // Read horizontal input each frame and let the player steer while boosting
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            currentX += horizontalInput * wingsMovementSpeed * Time.deltaTime;

            float t = elapsedTime / boostTime;
            float y = Mathf.Lerp(startPosition.y, targetY, t);

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

        // Hand control back to the player's own controller
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Destroy(gameObject); // Destroy the wings after boosting the player
    }
}