using System.Collections;
using UnityEngine;

public class CosmicRay : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer warning;
    [SerializeField] private SpriteRenderer ray;
    [SerializeField] private ParticleSystem topParticles;
    [SerializeField] private Animator rayAnimator;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 2.5f;
    [SerializeField] private float activeDuration = 2.0f;

    [Header("Sizing")]
    [SerializeField] private float topGap = 0.5f;
    [SerializeField] private float warningTempFix = 0.5f;
    [SerializeField] private float hitboxWidth = 0.8f;

    [Header("Warning Controls")]
    [SerializeField] private float warningBlinkSlow = 0.3f;
    [SerializeField] private float warningBlinkFast = .03f;

    public Camera ourCamera;
    private BoxCollider2D killZoneCollider;

    private float rayHeight;
    private float posWarningY;
    private float posTopY;
    private float posBotY;
    private float topOffsetPivot;
    
    private float rayNativeWidth = 1f;
    private float rayNativeHeight = 1f;
    private float spriteScale = 1f;

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

        spriteScale = rayHeight / rayNativeHeight;
        float spriteScaledWidth = rayNativeWidth * spriteScale * hitboxWidth;

        killZoneCollider.size = new Vector2(spriteScaledWidth, rayHeight);
        killZoneCollider.offset = Vector2.zero;
    }


    private IEnumerator WarningSequence()
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        if (audioManager != null && warningSound != null)
        {
            audioManager.PlaySFX(warningSound);
        }
        
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

            Vector3 dPos = ray.transform.localPosition;
            dPos.y = posTopY - (topOffsetPivot * spriteScale);
            ray.transform.localPosition = dPos;

            ray.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDeath();
        }
    }

}