using UnityEngine;
using PathSystem;

namespace TroopSystem
{
    public class TroopSpawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        public LanePath spawnPath; // Path where troops will spawn and move along
        public TroopFaction troopFaction = TroopFaction.Player; // Faction of the spawned troops
        
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
                // Troop position will be set by the troop's faction-based logic
                GameObject newTroopObject = Instantiate(troopPrefab, Vector3.zero, Quaternion.identity);
                
                // Get the troop component and assign the path
                Troop newTroop = newTroopObject.GetComponent<Troop>();
                if (newTroop != null)
                {
                    newTroop.faction = troopFaction; // Set the faction
                    newTroop.SetPath(spawnPath);
                }
                
                Debug.Log($"Spawned {troopFaction} troop on path");
            }
            else
            {
                Debug.LogWarning("Missing troop prefab or spawn path!");
            }
        }

        // Visualize the spawn point and path in the editor

    }
}