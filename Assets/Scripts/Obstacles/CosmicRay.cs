using System.Collections;
using UnityEngine;

public class CosmicRay : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer warning;
    [SerializeField] private SpriteRenderer ray;
    [SerializeField] private SpriteRenderer aoeWarning;
    [SerializeField] private ParticleSystem topParticles;
    [SerializeField] private Animator rayAnimator;
    [SerializeField] private Animator warningAnimator;
    [SerializeField] private Animator aoeWarningAnimator;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 2.5f;
    [SerializeField] private float activeDuration = 2.0f;

    [Header("Sizing")]
    [SerializeField] private float topGap = 0.5f;
    [SerializeField] private float hitboxWidth = 0.8f;

    [Header("Warning Controls")]
    [SerializeField] private float warningTransformTime= 0.3f;
    [SerializeField] private float warningGap = .03f;
    [SerializeField] private float warningScreenOffset = 1f;
    [SerializeField] private float aoeBlinkSlow = 0.3f;
    [SerializeField] private float aoeBlinkFast = 0.01f;

    public Camera ourCamera;
    private BoxCollider2D killZoneCollider;

    private float rayHeight;
    private float posWarningStartY;
    private float posWarningEndY;
    private float posTopY;
    private float posBotY;
    private float topOffsetPivot;
    
    private float rayNativeWidth = 1f;
    private float rayNativeHeight = 1f;
    private float spriteScale = 1f;
    private float rayVisualWidth = 1f;

    private float warningNativeWidth = 1f;
    private float warningNativeHeight = 1f;
    private float warningSpriteScale = 1f;
    private float aoeHeight = 1f;

    private Coroutine aoeCoroutine;

    public AudioClip warningSound;


    void Awake()
    {
        killZoneCollider = GetComponent<BoxCollider2D>();
        killZoneCollider.enabled = false;
        killZoneCollider.isTrigger = true;
        ourCamera = Camera.main;

        if (warning != null)
        {
            warning.gameObject.SetActive(false);
            if (warningAnimator == null)
                warningAnimator = warning.GetComponent<Animator>();

            if (warning.sprite != null)
            {
                warningNativeWidth = warning.sprite.bounds.size.x;
                warningNativeHeight = warning.sprite.bounds.size.y;
            }
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
                rayNativeWidth = ray.sprite.bounds.size.x;
                rayNativeHeight = ray.sprite.bounds.size.y;
                topOffsetPivot = ray.sprite.bounds.max.y;
            }
        }

        if (aoeWarning != null)
        {
            aoeWarning.gameObject.SetActive(false);
            if (aoeWarningAnimator == null)
            {
                aoeWarningAnimator = aoeWarning.GetComponent<Animator>();
            }

            if (aoeWarning.sprite != null)
            {
                aoeHeight = aoeWarning.sprite.bounds.size.y;
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

        rayHeight = topOffset - botOffset;
        float centerOffset = (topOffset + botOffset) / 2f;

        Vector3 pos = transform.localPosition;
        pos.y = centerOffset;
        transform.localPosition = pos;

        posTopY = topOffset - centerOffset;
        posBotY = botOffset - centerOffset;

        spriteScale = rayHeight / rayNativeHeight;
        rayVisualWidth = rayNativeWidth * spriteScale;
        float spriteScaledWidth = rayVisualWidth * hitboxWidth;

        killZoneCollider.size = new Vector2(spriteScaledWidth, rayHeight);
        killZoneCollider.offset = Vector2.zero;

        //Warning Desing

        if (warningNativeWidth > 0f)
        {
            warningSpriteScale = rayVisualWidth / warningNativeWidth;
        }
        float warningHalf = (warningNativeHeight * warningSpriteScale) * 0.5f;
        posWarningEndY = posTopY - warningGap - warningHalf;
        posWarningStartY = posTopY + warningScreenOffset + warningHalf;

        //AOE

        if (aoeWarning != null && aoeHeight > 0f)
        {
            float aoeScale = rayHeight / aoeHeight;
            aoeWarning.transform.localScale = new Vector3(aoeScale, aoeScale, 1f);

            Vector3 aoePos = aoeWarning.transform.localPosition;
            aoePos.y = 0f;
            aoeWarning.transform.localPosition = aoePos;
        }
    }


    private IEnumerator WarningSequence()
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        if (audioManager != null && warningSound != null)
        {
            audioManager.PlaySFX(warningSound);
        }

        aoeCoroutine = StartCoroutine(AOEWarningManager(warningDuration));
        
        if (warning == null) {
            yield return new WaitForSeconds(warningDuration); 
            yield break;
        }

        warning.gameObject.SetActive(true);
        warning.transform.localScale = new Vector3(warningSpriteScale, warningSpriteScale, 1);

        Vector3 startPos = warning.transform.localPosition;
        startPos.y = posWarningStartY;
        warning.transform.localPosition = startPos;

        if (warningAnimator != null)
        {
            warningAnimator.Play("WarnLoop", 0, 0f);
        }

        yield return WarningTransform(warning.transform, posWarningStartY, posWarningEndY, warningTransformTime);

        float looming = warningDuration - (warningTransformTime * 2f);
        if (looming > 0f) {
            yield return new WaitForSeconds(looming);
        }

        yield return WarningTransform(warning.transform, posWarningEndY, posWarningStartY, warningTransformTime);

        warning.gameObject.SetActive(false);
    }


    private IEnumerator RayDropSequence()
    {
        if (ray != null)
        {
            ray.gameObject.SetActive(true);

            Vector3 dPos = ray.transform.localPosition;
            dPos.y = posTopY - (topOffsetPivot * spriteScale);
            ray.transform.localPosition = dPos;

            ray.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
        }

        if (warning != null)
        {
            warning.gameObject.SetActive(false);
        }

        if (aoeWarning != null)
        {
            aoeWarning.gameObject.SetActive(false);
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
            Anchor(); // This function allows the sprite to "exit" through the bottom of the screen 
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

        float destroyDelay = 0f;

        if (topParticles != null)
        {
            topParticles.Stop();
            destroyDelay = topParticles.main.duration + topParticles.main.startLifetime.constantMax;
        }
        
        if (ray != null)
        {
            ray.gameObject.SetActive(false);
        }

        Destroy(gameObject, destroyDelay);
    }


    private void Anchor()
    {
        float bottomLocalOffset = ray.sprite.bounds.min.y; 

        Vector3 p = ray.transform.localPosition;
        p.y = posBotY - (bottomLocalOffset * spriteScale);
        ray.transform.localPosition = p;
    }


    private IEnumerator WarningTransform(Transform target, float fromY, float toY, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float timer = Mathf.Clamp01(elapsed / duration);

            Vector3 pos = target.localPosition;
            pos.y = Mathf.Lerp(fromY, toY, timer);
            target.localPosition = pos;

            yield return null;
        }

        Vector3 finalPos = target.localPosition;
        finalPos.y = toY;
        target.localPosition = finalPos;
    }

    private IEnumerator AOEWarningManager(float totalDuration)
    {
        if (aoeWarning == null)
        {
            yield break;
        }

        aoeWarning.gameObject.SetActive(true);
        aoeWarning.enabled = true;

        float animLength = 0f;

        if (aoeWarningAnimator != null)
        {
            aoeWarningAnimator.Play("WarnAOE", 0, 0f);
            yield return null;
            animLength = aoeWarningAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
        }
        yield return new WaitForSeconds(0.2f);
        float blinkDuration = Mathf.Max(0f, totalDuration - animLength);
        float elapsed = 0f;
        float blinkTimer = 0f;
        bool visible = true;

        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            blinkTimer += Time.deltaTime;

            float timer = Mathf.Clamp01(elapsed / blinkDuration);
            float blinkInterval = Mathf.Lerp(aoeBlinkSlow, aoeBlinkFast, timer);

            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;
                visible = !visible;
                aoeWarning.enabled = visible;
            }

            yield return null;
        }

        aoeWarning.enabled = true;
        aoeWarning.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDeath();
        }
    }

}