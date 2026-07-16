//using System.Collections;
//using Unity.VisualScripting;
//using UnityEngine;

//public class CosmicRay : MonoBehaviour
//{
//    public SpriteRenderer warning;
//    public SpriteRenderer ray;

//    public Camera ourCamera;
//    public float rayWidth = 1f;
//    public float topGap = 0.5f;
//    public float warningTempFix = 0.5f; // Temporary offset fixes before having sprites/animations

//    public float warningDuration = 2.5f;
//    public float rayDropDuration = 0.1f;
//    public float activeDuration = 2.0f;

//    public float warningBlinkSlow = 0.3f;
//    public float warningBlinkFast = .03f;

//    private BoxCollider2D killZoneCollider;
//    private float rayHeight;

//    private float posWarningY;
//    private float posTopY;

//    // Currently using the square sprite, but idk what best practice is or sprite size? I added this scaling solution
//    // but we can remove it later if it causes any problems or if we can simplify this.
//    private float rayNativeWidth = 1f;
//    private float rayNativeHeight = 1f;
//    private float rayScaleX = 1f;


//    void Awake()
//    {
//        killZoneCollider = GetComponent<BoxCollider2D>();
//        killZoneCollider.enabled = false;
//        killZoneCollider.isTrigger = true;
//        ourCamera = Camera.main; 

//        if ( warning != null )
//        {
//            warning.gameObject.SetActive(false);
//        }

//        if ( ray != null )
//        {
//            ray.gameObject.SetActive(false);

//            if (ray.sprite != null)
//            {
//                // Part of the scaling solution w/ world units
//                rayNativeWidth = ray.sprite.bounds.size.x;
//                rayNativeHeight = ray.sprite.bounds.size.y;
//                rayScaleX = rayWidth / rayNativeWidth;
//            }
//        }
//    }


//    void OnEnable()
//    {
//        DesignRay();
//        StartCoroutine(RunRaySequence());
//    }


//    private IEnumerator RunRaySequence()
//    {
//        // WARNING 
//        yield return WarningSequence();

//        // RAY (DROP)
//        yield return RayDropSequence();

//        // RAY (ACTIVE)
//        yield return RayActiveSequence();

//        // REMOVE
//        RemoveRay();

//    }


//    private void DesignRay()
//    {
//        if (ourCamera == null)
//        {
//            return;
//        }

//        transform.SetParent(ourCamera.transform, true);

//        float cameraHalfHeight = ourCamera.orthographicSize;

//        float topOffset = cameraHalfHeight + topGap;
//        float botOffset = -cameraHalfHeight;
//        float warningOffset = cameraHalfHeight - warningTempFix;

//        rayHeight = topOffset - botOffset;
//        float centerOffset = (topOffset + botOffset) / 2f;

//        Vector3 pos = transform.localPosition;
//        pos.y = centerOffset;
//        transform.localPosition = pos;

//        posWarningY = warningOffset - centerOffset;
//        posTopY = topOffset - centerOffset;

//        killZoneCollider.size = new Vector2(rayWidth, rayHeight);
//        killZoneCollider.offset = Vector2.zero;
//    }


//    private IEnumerator WarningSequence()
//    {
//        if (warning != null)
//        {
//            warning.gameObject.SetActive(true);
//            Vector3 wPos = warning.transform.localPosition;
//            wPos.y = posWarningY;
//            warning.transform.localPosition = wPos;
//        }

//        float timer = 0f;
//        float blinkTimer = 0f;
//        bool visible = true;

//        while (timer < warningDuration)
//        {
//            timer += Time.deltaTime;
//            blinkTimer += Time.deltaTime;

//            // Set up a lerp so any warning sprite will blink. Should it be animated or just a sprite?

//            float transitionBlink = timer / warningDuration;
//            float currentBlink = Mathf.Lerp(warningBlinkSlow, warningBlinkFast, transitionBlink);

//            if (blinkTimer >= currentBlink)
//            {
//                blinkTimer = 0f;
//                visible = !visible;
//                if (warning != null)
//                {
//                    warning.enabled = visible;
//                }
//            }
//            yield return null;
//        }

//       // set null ? do we want it to vanish?
//    }


//    private IEnumerator RayDropSequence()
//    {
//        if (ray != null)
//        {
//            ray.gameObject.SetActive(true);
//            Vector3 dPos = ray.transform.localPosition;
//            dPos.y = posTopY;
//            ray.transform.localPosition = dPos;
//            ray.transform.localScale = new Vector3(rayScaleX, 0f, 1f);
//        }

//        float currentDropTime = 0f;
//        while (currentDropTime < rayDropDuration)
//        {
//            currentDropTime += Time.deltaTime;
//            float unitTransition = Mathf.Clamp01(currentDropTime / rayDropDuration);
//            float currentHeight = rayHeight * unitTransition;
//            if (ray != null)
//            {
//                // Scaling solution
//                float currentScaleY = currentHeight / rayNativeHeight;
//                Vector3 dPos = ray.transform.localPosition;
//                dPos.y = posTopY - (currentHeight / 2);
//                ray.transform.localPosition = dPos;
//                ray.transform.localScale = new Vector3(rayScaleX, currentScaleY, 1f);

//            }
//            yield return null;
//        }
//    }


//    private IEnumerator RayActiveSequence()
//    {
//        killZoneCollider.enabled = true;

//        float uptime = 0f;
//        while (uptime < activeDuration)
//        {
//            uptime += Time.deltaTime;
//            yield return null;
//        }
//    }


//    private void RemoveRay()
//    {
//        killZoneCollider.enabled = false;
//        if (ray != null)
//        {
//            ray.gameObject.SetActive(false);
//        }
//        Destroy(gameObject);
//    }


//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") && GameManager.Instance != null)
//        {
//            GameManager.Instance.PlayerDeath();
//        }
//    }

//}
using System.Collections;
using UnityEngine;

public class CosmicRay : MonoBehaviour
{
    public SpriteRenderer warning;
    public SpriteRenderer ray;
    public ParticleSystem topParticles;

    public Camera ourCamera;
    public float rayWidth = 1f;
    public float topGap = 0.5f;
    public float warningTempFix = 0.5f; // Temporary offset fixes before having sprites/animations

    public float warningDuration = 2.5f;
    public float activeDuration = 2.0f;

    public float warningBlinkSlow = 0.3f;
    public float warningBlinkFast = .03f;

    private BoxCollider2D killZoneCollider;
    private float rayHeight;
    public Animator rayAnimator;

    private float posWarningY;
    private float posTopY;

    private float posBotY;
    private float topOffsetPivot = 0f;
    private float fullScaleY;

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

        if (warning != null)
        {
            warning.gameObject.SetActive(false);
        }

        if (ray != null)
        {
            ray.gameObject.SetActive(false);

            if (rayAnimator == null)
            {
                rayAnimator = ray.GetComponent<Animator>();
            }

            if (ray.sprite != null)
            {
                // Part of the scaling solution w/ world units
                rayNativeWidth = ray.sprite.bounds.size.x;
                rayNativeHeight = ray.sprite.bounds.size.y;
                rayScaleX = rayWidth / rayNativeWidth;

                topOffsetPivot = ray.sprite.bounds.max.y;
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

        // RAY (END)
        yield return RayEndingSequence();

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
        posBotY = botOffset - centerOffset;

        fullScaleY = rayHeight / rayNativeHeight;

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
            dPos.y = posTopY - (topOffsetPivot * fullScaleY);
            ray.transform.localPosition = dPos;

            ray.transform.localScale = new Vector3(rayScaleX, fullScaleY, 1f);
        }
        if (warning != null)
        {
            warning.gameObject.SetActive(false);
        }
        if (topParticles != null)
        {
            Vector3 pos = topParticles.transform.localPosition;
            pos.y = posTopY;
            topParticles.transform.localPosition = pos;
            topParticles.Play();
        }
        yield return PlayAnimation("RayFall");
    }


    private IEnumerator RayActiveSequence()
    {
        killZoneCollider.enabled = true;

        if (rayAnimator != null)
        {
            rayAnimator.Play("RayLoop", 0, 0f);
        }
        yield return new WaitForSeconds(activeDuration);
    }

    private IEnumerator RayEndingSequence()
    {
        killZoneCollider.enabled = false;

        if (rayAnimator == null) yield break;

        rayAnimator.Play("RayEnd", 0, 0f);
        yield return null;

        float len = rayAnimator.GetCurrentAnimatorStateInfo(0).length;
        float elapsed = 0f;

        while (elapsed < len)
        {
            Anchor(); 
            elapsed += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator PlayAnimation(string stateName)
    {
        if (rayAnimator == null)
        {
            yield break;
        }

        rayAnimator.Play(stateName, 0, 0f);
        yield return null;

        float length = rayAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
    }


    private void RemoveRay()
    {
        killZoneCollider.enabled = false;
        topParticles.Stop();
        if (ray != null)
        {
            ray.gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }

    // Trying to use the Unity animator and can't get the animation for the ray to play T-T... using this for now to switch from fall from top to despawn toward bottom
    private void Anchor()
    {
        float bottomLocalOffset = ray.sprite.bounds.min.y; 

        Vector3 p = ray.transform.localPosition;
        p.y = posBotY - (bottomLocalOffset * fullScaleY);
        ray.transform.localPosition = p;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDeath();
        }
    }

}