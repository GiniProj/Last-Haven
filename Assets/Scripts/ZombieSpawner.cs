using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [Tooltip("The initial spawn rate of the spawner")]
    [SerializeField] private float initialSpawnRate = 5f;
    [Tooltip("The minimum spawn rate that the spawner can reach")]
    [SerializeField] private float minimumSpawnRate = 0.5f;
    [SerializeField] private float spawnRateReduction = 0.01f;
    [Tooltip("Maximum additional distance beyond camera size for spawning")]
    [SerializeField] private float maxAdditionalSpawnDistance = 5f;

    [Header("Random Spawn Properties")]
    [SerializeField] private bool withRandomSpawnProperties = false;
    [SerializeField] private Vector2 zombieHealthRange = new Vector2(3f, 15f);
    [SerializeField] private Vector2 zombieMoveSpeedRange = new Vector2(0.5f, 2f);

    private float spawnRate;
    private float nextSpawnTime;
    private Camera mainCamera;

    private void Start()
    {
        spawnRate = initialSpawnRate;
        nextSpawnTime = Time.time + spawnRate;
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            enabled = false;
            return;
        }

        if (!mainCamera.orthographic)
        {
            Debug.LogError("Main camera is not orthographic!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnZombie();
            nextSpawnTime = Time.time + spawnRate;

            // Increase difficulty over time
            spawnRate = Mathf.Max(minimumSpawnRate, spawnRate - spawnRateReduction);
        }
    }

    private void SpawnZombie()
    {
        float minSpawnDistance = mainCamera.orthographicSize;
        float maxSpawnDistance = minSpawnDistance + maxAdditionalSpawnDistance;

        // Get random angle and distance
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        // Calculate spawn position relative to the center position
        Vector2 spawnDirection = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
        Vector2 spawnPos = (Vector2)transform.position + (spawnDirection * distance);

        GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        // Randomize zombie properties
        if (withRandomSpawnProperties && zombie != null)
        {
            Zombie zombieComponent = zombie.GetComponent<Zombie>();
            if (zombieComponent != null)
            {
                zombieComponent.ZombieSetHealth = Random.Range(zombieHealthRange.x, zombieHealthRange.y);
                zombieComponent.ZombieSetMoveSpeed = Random.Range(zombieMoveSpeedRange.x, zombieMoveSpeedRange.y);
            }
        }
    }
}