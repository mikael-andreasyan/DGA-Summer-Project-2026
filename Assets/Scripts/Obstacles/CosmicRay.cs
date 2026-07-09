using System.Collections;
using UnityEngine;

public class CosmicRay : MonoBehaviour
{
    public SpriteRenderer warning;
    public SpriteRenderer ray;

    // Trying to make it spawn at the top of the camera:
    public Camera ourCamera;
    public float rayWidth = 1f;
    public float topGap = 0.5f;
    public float warningTempFix = 0.5f; // Temporary offset fixes before having sprites/animations

    public float warningDuration = 2.5f;
    public float rayDropDuration = 0.1f;
    public float activeDuration = 2.0f;

    public float warningBlinkSlow = 0.3f;
    public float warningBlinkFast = .03f;

    private BoxCollider2D killZoneCollider;
    private float topY, botY, rayHeight, rayCenterY;
    private float warningTopY; // Same as TempFix, need this for now

    // Currently using the square sprite, but idk what best practice is or sprite size? I added this scaling solution
    // but we can remove it later if it causes any problems or if we can simplify this.
    private float rayNativeWidth = 1f;
    private float rayNativeHeight = 1f;
    private float rayScaleX = 1f;


    void Awake()
    {
        killZoneCollider = GetComponent<BoxCollider2D>();
        killZoneCollider.enabled = false;
        killZoneCollider.isTrigger = true;
        ourCamera = Camera.main; 

        if ( warning != null )
        {
            warning.gameObject.SetActive(false);
        }

        if ( ray != null )
        {
            ray.gameObject.SetActive(false);

            if (ray.sprite != null)
            {
                // Part of the scaling solution w/ world units
                rayNativeWidth = ray.sprite.bounds.size.x;
                rayNativeHeight = ray.sprite.bounds.size.y;
                rayScaleX = rayWidth / rayNativeWidth;
            }
        }
    }


    void OnEnable()
    {
        DesignRay();
        StartCoroutine(RunRaySequence());
    }


    private IEnumerator RunRaySequence()
    {
        // WARNING 
        yield return WarningSequence();

        // RAY (DROP)
        yield return RayDropSequence();

        // RAY (ACTIVE)
        yield return RayActiveSequence();

        // REMOVE
        RemoveRay();

    }


    private void DesignRay()
    {
        float cameraHalfHeight = ourCamera.orthographicSize;
        float cameraY = ourCamera.transform.position.y;

        topY = cameraY + cameraHalfHeight + topGap;
        botY = cameraY - cameraHalfHeight;
        warningTopY = cameraY + cameraHalfHeight - warningTempFix;
        rayHeight = topY - botY;
        rayCenterY = (topY + botY) / 2f;

        Vector3 pos = transform.position;
        pos.y = rayCenterY;
        transform.position = pos;

        killZoneCollider.size = new Vector2(rayWidth, rayHeight);
        killZoneCollider.offset = Vector2.zero;
    }


    private IEnumerator WarningSequence()
    {
        if (warning != null)
        {
            warning.gameObject.SetActive(true);
            warning.transform.position = new Vector3(transform.position.x, warningTopY, transform.position.z);
        }

        float timer = 0f;
        float blinkTimer = 0f;
        bool visible = true;

        while (timer < warningDuration)
        {
            timer += Time.deltaTime;
            blinkTimer += Time.deltaTime;

            // Set up a lerp so any warning sprite will blink. Should it be animated or just a sprite?

            float transitionBlink = timer / warningDuration;
            float currentBlink = Mathf.Lerp(warningBlinkSlow, warningBlinkFast, transitionBlink);

            if (blinkTimer >= currentBlink)
            {
                blinkTimer = 0f;
                visible = !visible;
                if (warning != null)
                {
                    warning.enabled = visible;
                }
            }
            yield return null;
        }
    }


    private IEnumerator RayDropSequence()
    {
        if (ray != null)
        {
            ray.gameObject.SetActive(true);
            ray.transform.position = new Vector3(transform.position.x, topY, transform.position.z);
            ray.transform.localScale = new Vector3(rayWidth, 0f, 1f);
        }

        float currentDropTime = 0f;
        while (currentDropTime < rayDropDuration)
        {
            currentDropTime += Time.deltaTime;
            float unitTransition = Mathf.Clamp01(currentDropTime / rayDropDuration);
            float currentHeight = rayHeight * unitTransition;
            if (ray != null)
            {
                // Scaling solution
                float currentScaleY = currentHeight / rayNativeHeight;
                ray.transform.position = new Vector3(transform.position.x, topY - (currentHeight / 2f), transform.position.z);
                ray.transform.localScale = new Vector3(rayScaleX, currentScaleY, 1f);

            }
            yield return null;
        }
    }


    private IEnumerator RayActiveSequence()
    {
        killZoneCollider.enabled = true;

        float uptime = 0f;
        while (uptime < activeDuration)
        {
            uptime += Time.deltaTime;
            yield return null;
        }
    }


    private void RemoveRay()
    {
        killZoneCollider.enabled = false;
        if (ray != null)
        {
            ray.gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"CosmicRay hit: {other.name}. Placeholder until activate player death");
    }

}
