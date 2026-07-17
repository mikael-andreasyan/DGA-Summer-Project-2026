using UnityEngine;

public class JumpBoostPickup : MonoBehaviour
{
    public float boostMultiplier = 1.5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ForceJump(boostMultiplier);
            Destroy(gameObject);
        }
    }
}