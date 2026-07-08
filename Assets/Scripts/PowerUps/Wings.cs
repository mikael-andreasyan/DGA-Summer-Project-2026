using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Wings : MonoBehaviour
{
    [Header("Wing Boost Settings")]
    [Tooltip("Boost distance that the wings will take the player up")]
    public float boostDistance = 5f;
    [Tooltip("Speed at which the player is boosted upward")]
    public float boostSpeed = 10f;
    private float boostTime;

    void Awake()
    {
        boostTime = boostDistance / boostSpeed;
    }

    void Update()
    {
       
        
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(BoostPlayer(other.transform, other.GetComponent<Rigidbody2D>()));
        }
    }

    IEnumerator BoostPlayer(Transform player, Rigidbody2D playerRb)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = player.position;
        Vector3 targetPosition = startPosition + Vector3.up * boostDistance;

        while (elapsedTime < boostTime)
        {
            playerRb.linearVelocity = new Vector2(
                0, 
                playerRb.linearVelocity.y); // Freeze the player's x position during the boost

            player.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / boostTime));
            elapsedTime += Time.deltaTime;
            transform.position = player.position; // Move the wings along with the player

            yield return null;
        }

        player.position = targetPosition; // Ensure the player reaches the target position
        playerRb.linearVelocity = new Vector2(0, boostSpeed); // Reset the player's velocity after the boost
        Destroy(gameObject); // Destroy the wings after boosting the player
    }



}
