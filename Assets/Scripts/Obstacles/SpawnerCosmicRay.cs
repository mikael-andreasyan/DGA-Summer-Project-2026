using System.Collections;
using System.Threading;
using UnityEngine;

public class SpawnerCosmicRay : MonoBehaviour
{
    public GameObject cosmicRayPrefab;
    public Transform player;

    public float minTimer = 5f;
    public float maxTimer = 10f;

    public float maxPlayerDist = 5f;
    public float minPlayerDist = 1f;

    void Start()
    {
        StartCoroutine(RayLoop());
    }

    private IEnumerator RayLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimer, maxTimer));
            SpawnRay();
        }
    }

    private void SpawnRay()
    {
        if (cosmicRayPrefab == null) { return; }

        float dist = Random.Range(minPlayerDist, maxPlayerDist);
        float lr = Random.value;
        if (lr < 0.5)
        {
            dist *= -1f;
        }


        float spawnPoint = player.position.x + dist;

        Instantiate(cosmicRayPrefab, new Vector3(spawnPoint, 0f, 0f), Quaternion.identity);
    }
}
