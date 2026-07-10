using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform player;

    [Header("Chunk Loading")]
    [SerializeField] private int chunksAhead = 5;

    private readonly List<GameObject> activeChunks = new();

    private float chunkHeight;
    private int highestChunkIndex;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (player == null && GameManager.Instance != null)
        {
            player = GameManager.Instance.GetPlayer();
        }

        DetermineChunkSize();

        // Spawn initial chunks
        for (int i = 0; i <= chunksAhead; i++)
        {
            SpawnChunk(i);
        }

        highestChunkIndex = chunksAhead;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        LoadAhead();
        UnloadBehind();
    }

    private void DetermineChunkSize()
    {
        GameObject tempChunk = Instantiate(chunkPrefab);

        Bounds bounds = GetChunkBounds(tempChunk);
        chunkHeight = bounds.size.y;

        Destroy(tempChunk);

        if (chunkHeight <= 0)
        {
            Debug.LogError("Chunk height could not be determined.");
        }
    }

    private void LoadAhead()
    {
        float playerY = player.position.y;

        while ((highestChunkIndex * chunkHeight) < playerY + (chunksAhead * chunkHeight))
        {
            highestChunkIndex++;
            SpawnChunk(highestChunkIndex);
        }
    }

    private void UnloadBehind()
    {
        float cameraBottom =
            cam.transform.position.y -
            cam.orthographicSize;

        for (int i = activeChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = activeChunks[i];

            if (chunk == null)
            {
                activeChunks.RemoveAt(i);
                continue;
            }

            Bounds bounds = GetChunkBounds(chunk);

            // Entire chunk below camera
            if (bounds.max.y < cameraBottom)
            {
                activeChunks.RemoveAt(i);
                Destroy(chunk);
            }
        }
    }

    private void SpawnChunk(int index)
    {
        Vector3 position = new Vector3(
            0f,
            index * chunkHeight,
            0f);

        GameObject chunk = Instantiate(
            chunkPrefab,
            position,
            Quaternion.identity);

        activeChunks.Add(chunk);
    }

    private Bounds GetChunkBounds(GameObject chunk)
    {
        Renderer[] renderers = chunk.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        Collider2D collider2D = chunk.GetComponentInChildren<Collider2D>();

        if (collider2D != null)
        {
            return collider2D.bounds;
        }

        Collider collider3D = chunk.GetComponentInChildren<Collider>();

        if (collider3D != null)
        {
            return collider3D.bounds;
        }

        return new Bounds(chunk.transform.position, Vector3.one);
    }
}