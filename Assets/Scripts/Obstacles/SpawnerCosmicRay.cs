using System.Collections;
using System.Threading;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerCosmicRay : MonoBehaviour
{
    public GameObject cosmicRayPrefab;
    public Transform player;

    [Header("Timing")]
    public float minTimer = 8f;
    public float maxTimer = 12f;

    [Header("Distance From Player, Spawning")]
    public float maxPlayerDist = 5f;
    public float minPlayerDist = 1f;

    [Header("Game Area Bounds")]
    public Transform leftWall;
    public Transform rightWall;
    public float margin = 0.15f;


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
        spawnPoint = Bound(spawnPoint);

        Instantiate(cosmicRayPrefab, new Vector3(spawnPoint, 0f, 0f), Quaternion.identity);
    }


    private float Bound(float firstPoint)
    {
        if (leftWall == null || rightWall == null)
        {
            return firstPoint;
        }

        float minX = leftWall.position.x + margin;
        float maxX = rightWall.position.x - margin;

        return Mathf.Clamp(firstPoint, minX, maxX);

    }
}
