using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform player;

    [Header("Rock Prefabs")]
    public GameObject[] rockPrefabs;   // ← drag prefabs here

    [Header("Spawn Settings")]
    public float spawnDistance = 120f;
    public float spawnInterval = 25f;
    public float spawnWidth = 40f;
    public float spawnHeight = 20f;

    private float nextSpawnZ;

    void Start()
    {
        nextSpawnZ = player.position.z + spawnDistance;
    }

    void Update()
    {
        if (player.position.z + spawnDistance > nextSpawnZ)
        {
            SpawnObstacle();
            nextSpawnZ += spawnInterval;
        }
    }

    void SpawnObstacle()
    {
        if (rockPrefabs.Length == 0) return;

        // pick random rock
        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

        Vector3 spawnPos = new Vector3(
            Random.Range(-spawnWidth, spawnWidth),
            Random.Range(-spawnHeight, spawnHeight),
            nextSpawnZ
        );

        Instantiate(prefab, spawnPos, Random.rotation);
    }
}