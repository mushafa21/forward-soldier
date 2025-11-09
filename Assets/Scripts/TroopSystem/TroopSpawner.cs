using UnityEngine;
using PathSystem;

namespace TroopSystem
{
    public class TroopSpawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        public LanePath spawnPath; // Path where troops will spawn and move along
        public bool spawnReversedTroops = false; // If true, troops will move in reverse direction (for enemy troops)
        
        [Header("Spawn Settings")]
        public float spawnInterval = 2f; // Time between spawns
        public GameObject troopPrefab; // Prefab of the troop to spawn
        
        private float lastSpawnTime = 0f;

        void Start()
        {
            // If no path is assigned, try to find one automatically
            if (spawnPath == null)
            {
                spawnPath = FindObjectOfType<LanePath>();
            }
        }

        void Update()
        {
            // Check if it's time to spawn a new troop
            if (Time.time - lastSpawnTime >= spawnInterval)
            {
                SpawnTroop();
                lastSpawnTime = Time.time;
            }
        }

        // Public method to manually spawn a troop
        public void SpawnTroop()
        {
            if (troopPrefab != null && spawnPath != null)
            {
                // Spawn at the start of the path
                Vector3 spawnPosition = spawnReversedTroops ? spawnPath.GetEndPoint() : spawnPath.GetStartPoint();
                
                GameObject newTroopObject = Instantiate(troopPrefab, spawnPosition, Quaternion.identity);
                
                // Get the troop component and assign the path
                Troop newTroop = newTroopObject.GetComponent<Troop>();
                if (newTroop != null)
                {
                    newTroop.SetPath(spawnPath, spawnReversedTroops);
                }
                
                Debug.Log($"Spawned {(spawnReversedTroops ? "enemy" : "friendly")} troop on path");
            }
            else
            {
                Debug.LogWarning("Missing troop prefab or spawn path!");
            }
        }

        // Visualize the spawn point and path in the editor
        private void OnDrawGizmosSelected()
        {
            if (spawnPath != null)
            {
                // Draw a line to the start or end of the path depending on direction
                Vector3 spawnPoint = spawnReversedTroops ? spawnPath.GetEndPoint() : spawnPath.GetStartPoint();
                Gizmos.color = spawnReversedTroops ? Color.red : Color.blue;
                Gizmos.DrawLine(transform.position, spawnPoint);
                Gizmos.DrawWireSphere(spawnPoint, 0.5f);
            }
            else
            {
                // Draw spawn point
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}