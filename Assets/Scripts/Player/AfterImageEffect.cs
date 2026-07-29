using System.Collections;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    [Header("Source")] // this is found automatically but just incase we want anything else to use this
    [SerializeField] private SpriteRenderer source;

    [Header("Style")]
    [SerializeField] private Color ghostColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float spawnInterval = 0.05f;
    [SerializeField] private float ghostLifetime = 0.35f;

    private bool isPlaying;

    private void Awake()
    {
        if (source == null)
        {
            source = GetComponent<SpriteRenderer>();
        }
    }

    public void Play()
    {
        if (source == null || isPlaying)
        {
            return;
        }
        StartCoroutine(SpawnTrail());
    }

    private IEnumerator SpawnTrail()
    {
        isPlaying = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            SpawnGhost();
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        isPlaying = false;
    }

    private void SpawnGhost()
    {
        GameObject ghost = new GameObject("AfterImageGhost");
        ghost.transform.position = source.transform.position;
        ghost.transform.rotation = source.transform.rotation;
        ghost.transform.localScale = source.transform.lossyScale;

        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = source.sprite;
        sr.flipX = source.flipX;
        sr.flipY = source.flipY;
        sr.sortingLayerID = source.sortingLayerID;
        sr.sortingOrder = source.sortingOrder - 1;
        sr.color = ghostColor;

        StartCoroutine(FadeAndDestroy(sr, ghost));
    }

    private IEnumerator FadeAndDestroy(SpriteRenderer sr, GameObject ghost)
    {
        float age = 0f;
        Color start = ghostColor;

        while (age < ghostLifetime)
        {
            age += Time.deltaTime;
            Color c = start;
            c.a = Mathf.Lerp(start.a, 0f, age / ghostLifetime);
            sr.color = c;
            yield return null;
        }

        Destroy(ghost);
    }
}
