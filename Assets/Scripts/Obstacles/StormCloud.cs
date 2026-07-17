
using UnityEngine;
using System.Collections;

public class StormCloud : BasicCloud
{
    /*
    cloud that kills u if u try to phase tru the bottom
    
    */

    [SerializeField] private float phaseCooldown = 1f;
    [SerializeField] private GameObject lightning;
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (phaseCooldownTimer > 0f) return;

        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
        if (otherRb == null) return;

        
        StrikeLightning(otherRb);
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