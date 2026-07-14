using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CosmicRay : MonoBehaviour
{
    public SpriteRenderer warning;
    public SpriteRenderer ray;

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
    private float rayHeight;

    private float posWarningY;
    private float posTopY;

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
        if (ourCamera == null)
        {
            return;
        }

        transform.SetParent(ourCamera.transform, true);

        float cameraHalfHeight = ourCamera.orthographicSize;

        float topOffset = cameraHalfHeight + topGap;
        float botOffset = -cameraHalfHeight;
        float warningOffset = cameraHalfHeight - warningTempFix;

        rayHeight = topOffset - botOffset;
        float centerOffset = (topOffset + botOffset) / 2f;

        Vector3 pos = transform.localPosition;
        pos.y = centerOffset;
        transform.localPosition = pos;

        posWarningY = warningOffset - centerOffset;
        posTopY = topOffset - centerOffset;

        killZoneCollider.size = new Vector2(rayWidth, rayHeight);
        killZoneCollider.offset = Vector2.zero;
    }


    private IEnumerator WarningSequence()
    {
        if (warning != null)
        {
            warning.gameObject.SetActive(true);
            Vector3 wPos = warning.transform.localPosition;
            wPos.y = posWarningY;
            warning.transform.localPosition = wPos;
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

       // set null ? do we want it to vanish?
    }


    private IEnumerator RayDropSequence()
    {
        if (ray != null)
        {
            ray.gameObject.SetActive(true);
            Vector3 dPos = ray.transform.localPosition;
            dPos.y = posTopY;
            ray.transform.localPosition = dPos;
            ray.transform.localScale = new Vector3(rayScaleX, 0f, 1f);
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
                Vector3 dPos = ray.transform.localPosition;
                dPos.y = posTopY - (currentHeight / 2);
                ray.transform.localPosition = dPos;
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
        if (other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDeath();
        }
    }

}
