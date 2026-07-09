using System.Collections;
using UnityEngine;

public class CosmicRay : MonoBehaviour
{

    BoxCollider2D rayCollider;
    SpriteRenderer raySprite;

    public bool isRayOn;
    public float timeToStart;
    public float rayTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rayCollider = GetComponent<BoxCollider2D>();
        raySprite = GetComponent<SpriteRenderer>();
        isRayOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRayOn == false)
        {
            rayCollider.enabled = false;
            raySprite.enabled = false;
            StartCoroutine(StartRay());
        }
        else if (isRayOn == true)
        {
            rayCollider.enabled = true;
            raySprite.enabled = true;
        }
        
    }


    IEnumerator StartRay()
    {
        yield return new WaitForSeconds(timeToStart);
        isRayOn = true;
        yield return new WaitForSeconds(rayTime);
        isRayOn = false;
    }
}
