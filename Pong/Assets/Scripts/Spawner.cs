using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public GameObject prefab; 
    
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = 0f;
    public float maxY = 4f;

    // Prevent multiple spawns in first level
    private bool hasSpawned = false; 

    void Start()
    {
        hasSpawned = false; 
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SpawnPrefabs();
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (BallController.Instance != null && BallController.Instance.gameStarted && !hasSpawned)
            {
                SpawnPrefabs();
                hasSpawned = true;
            }
        }
    }

    void SpawnPrefabs()
    {
        int spawnNumber = SceneManager.GetActiveScene().buildIndex + 1;

        for (int i = 0; i < spawnNumber; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }
}