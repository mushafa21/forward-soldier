using UnityEngine;
using System.Collections.Generic;

public class CloudShadowGenerator : MonoBehaviour
{
    [Header("Cloud Settings")]
    public GameObject cloudPrefab; // The cloud shadow prefab to instantiate
    public float spawnInterval = 3f; // Time between cloud spawns
    public float moveSpeed = 1f; // Speed at which clouds move from left to right
    public float leftBoundary = -20f; // Position where clouds get destroyed when they go too far right
    public float rightBoundary = 20f; // Position where clouds are destroyed when they go too far right

    [Header("Y Position Settings")]
    public float minYPosition = 5f; // Minimum Y position for random placement
    public float maxYPosition = 10f; // Maximum Y position for random placement

    [Header("Spawn Area")]
    public float spawnMinX = -20f; // Leftmost spawn position
    public float spawnMaxX = -15f; // Rightmost spawn position (off-screen to the left)

    [Header("Performance Settings")]
    public int maxClouds = 20; // Maximum number of clouds to prevent too many objects
    public bool useObjectPooling = true; // Whether to use object pooling for performance

    private List<GameObject> activeClouds = new List<GameObject>();
    private Queue<GameObject> pooledClouds = new Queue<GameObject>();
    private float lastSpawnTime = 0f;

    void Start()
    {
        if (cloudPrefab == null)
        {
            Debug.LogWarning("CloudShadowGenerator: No cloud prefab assigned! Please assign a prefab in the inspector.");
        }
        SpawnCloud();

    }

    void Update()
    {
        // Spawn clouds at the specified interval
        if (Time.time - lastSpawnTime >= spawnInterval && cloudPrefab != null)
        {
            SpawnCloud();
            lastSpawnTime = Time.time;
        }

        // Move all active clouds
        MoveClouds();
        
        // Clean up clouds that are no longer needed
        CleanupClouds();
    }

    void SpawnCloud()
    {
        // Limit the number of clouds for performance
        if (activeClouds.Count >= maxClouds)
        {
            return;
        }

        GameObject cloud;

        // Use object pooling if enabled
        if (useObjectPooling && pooledClouds.Count > 0)
        {
            cloud = pooledClouds.Dequeue();
            cloud.SetActive(true);
        }
        else
        {
            // Create a new cloud instance
            cloud = Instantiate(cloudPrefab, transform);
        }

        // Set random X and Y position for the new cloud
        float randomX = Random.Range(spawnMinX, spawnMaxX);
        float randomY = Random.Range(minYPosition, maxYPosition);
        cloud.transform.position = new Vector3(randomX, randomY, transform.position.z);

        // Add to active clouds list
        activeClouds.Add(cloud);

    }

    void MoveClouds()
    {
        // Move all active clouds to the right
        for (int i = activeClouds.Count - 1; i >= 0; i--)
        {
            GameObject cloud = activeClouds[i];
            if (cloud != null)
            {
                // Move the cloud horizontally
                cloud.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
        }
    }

    void CleanupClouds()
    {
        // Remove clouds that have moved beyond the right boundary
        for (int i = activeClouds.Count - 1; i >= 0; i--)
        {
            GameObject cloud = activeClouds[i];
            if (cloud != null)
            {
                // Check if the cloud has moved beyond the right boundary
                if (cloud.transform.position.x > rightBoundary)
                {
                    ReturnCloudToPool(cloud);
                    activeClouds.RemoveAt(i);
                }
            }
            else
            {
                // Remove null references from the list
                activeClouds.RemoveAt(i);
            }
        }
    }

    void ReturnCloudToPool(GameObject cloud)
    {
        if (useObjectPooling)
        {
            cloud.SetActive(false);
            pooledClouds.Enqueue(cloud);
        }
        else
        {
            Destroy(cloud);
        }
    }

    // Optional: Method to manually spawn a cloud from other scripts
    public void SpawnCloudInstantly()
    {
        SpawnCloud();
        lastSpawnTime = Time.time; // Reset spawn timer so it doesn't double-spawn
    }

    // Clean up when the generator is destroyed
    void OnDestroy()
    {
        if (pooledClouds != null)
        {
            foreach (GameObject cloud in pooledClouds)
            {
                if (cloud != null)
                    DestroyImmediate(cloud);
            }
            pooledClouds.Clear();
        }

        if (activeClouds != null)
        {
            foreach (GameObject cloud in activeClouds)
            {
                if (cloud != null)
                    DestroyImmediate(cloud);
            }
            activeClouds.Clear();
        }
    }

    // Visualize the spawn area in the editor
    void OnDrawGizmosSelected()
    {
        if (cloudPrefab != null)
        {
            // Draw a wireframe rectangle showing the spawn area
            Gizmos.color = Color.blue;
            Vector3 spawnAreaCenter = new Vector3((spawnMinX + spawnMaxX) / 2f, (minYPosition + maxYPosition) / 2f, transform.position.z);
            Vector3 spawnAreaSize = new Vector3(spawnMaxX - spawnMinX, maxYPosition - minYPosition, 0.1f);
            Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);

            // Draw the movement boundaries
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(leftBoundary, minYPosition, transform.position.z), 
                           new Vector3(leftBoundary, maxYPosition, transform.position.z));
            Gizmos.DrawLine(new Vector3(rightBoundary, minYPosition, transform.position.z), 
                           new Vector3(rightBoundary, maxYPosition, transform.position.z));
        }
    }
}