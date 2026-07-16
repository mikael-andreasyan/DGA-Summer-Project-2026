using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject[] chunkPrefabs;

    [SerializeField] private int chunksAhead = 5;

    private readonly List<GameObject> loadedChunks = new();

    private GameObject highestChunk;

    private void Start()
    {
        SpawnFirstChunk();

        while (GetChunksAbovePlayer() < chunksAhead)
        {
            SpawnNextChunk();
        }
    }

    private void Update()
    {
        while (GetChunksAbovePlayer() < chunksAhead)
        {
            SpawnNextChunk();
        }

        UnloadChunks();
    }

    private void SpawnFirstChunk()
    {
        GameObject chunk = Instantiate(
            chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]
        );

        loadedChunks.Add(chunk);
        highestChunk = chunk;
    }

    private void SpawnNextChunk()
    {
        Transform previousTop =
            highestChunk.transform.Find("Top");

        GameObject chunk = Instantiate(
            chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]
        );

        Transform newBottom =
            chunk.transform.Find("Bottom");

        if (previousTop == null || newBottom == null)
        {
            Debug.LogError("Chunk missing Top or Bottom marker.");
            Destroy(chunk);
            return;
        }

        chunk.transform.position +=
            previousTop.position - newBottom.position;

        loadedChunks.Add(chunk);
        highestChunk = chunk;
    }

    private int GetChunksAbovePlayer()
    {
        int count = 0;

        foreach (GameObject chunk in loadedChunks)
        {
            Transform bottom = chunk.transform.Find("Bottom");

            if (bottom != null &&
                bottom.position.y > cameraTransform.position.y)
            {
                count++;
            }
        }

        return count;
    }

    private void UnloadChunks()
    {
        float unloadY =
            cameraTransform.position.y -
            Camera.main.orthographicSize * 4f;

        for (int i = loadedChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = loadedChunks[i];

            if (chunk == highestChunk)
                continue;

            Transform top = chunk.transform.Find("Top");

            if (top != null && top.position.y < unloadY)
            {
                loadedChunks.RemoveAt(i);
                Destroy(chunk);
            }
        }
    }
}