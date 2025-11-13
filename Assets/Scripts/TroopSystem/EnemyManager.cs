using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathSystem;
using SoulSystem;
using TroopSystem;

public class EnemyManager : MonoBehaviour
{
    [Header("Soul System")]
    public float soulRegenAmount = 1f; // Amount of souls regenerated per interval
    public float soulRegenInterval = 2f; // Time in seconds between soul regeneration
    private float currentSouls;
    public float maxSouls = 100f; // Maximum souls the enemy can have

    [Header("Spawn Settings")]
    public GameObject troopPrefab; // Prefab of the troop to spawn
    public GameObject spawnEffectPrefab; // Prefab of the troop to spawn

    public List<LanePath> spawnPaths = new List<LanePath>(); // List of paths where the enemy can spawn troops
    public List<TroopLevel> spawnableTroops = new List<TroopLevel>(); // List of troops the enemy can spawn
    public float spawnDecisionInterval = 5f; // Time in seconds between spawn decisions

    private bool isInitialized = false;

    private static EnemyManager instance;
    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EnemyManager");
                    instance = obj.AddComponent<EnemyManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeEnemyManager();
    }

    private void InitializeEnemyManager()
    {
        if (isInitialized) return;

        currentSouls = 0f; // Start with 0 souls
        isInitialized = true;

        // Start the enemy logic coroutines
        StartCoroutine(SoulRegenCoroutine());
        StartCoroutine(EnemyActionCoroutine());
    }

    private IEnumerator SoulRegenCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(soulRegenInterval);
            if (isInitialized)
            {
                currentSouls = Mathf.Min(maxSouls, currentSouls + soulRegenAmount);
                Debug.Log("Enemy souls regenerated: " + currentSouls);
            }
        }
    }

    private IEnumerator EnemyActionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDecisionInterval);
            if (isInitialized)
            {
                PerformEnemyAction();
            }
        }
    }

    private void PerformEnemyAction()
    {
        if (spawnableTroops.Count == 0 || spawnPaths.Count == 0)
        {
            return; // Can't do anything if there are no troops or paths to spawn
        }

        // Try to spawn a troop randomly if possible
        TrySpawnRandomTroop();
    }

    private void TrySpawnRandomTroop()
    {
        if (spawnableTroops.Count == 0)
            return;

        // Find a valid troop to spawn based on available souls
        TroopLevel troopToSpawn = GetAffordableTroop();

        if (troopToSpawn != null && currentSouls >= troopToSpawn.troopSO.soulCost)
        {
            // Select a random path
            LanePath selectedPath = spawnPaths[Random.Range(0, spawnPaths.Count)];
            
            // Add enemy level bonus to the troop level
            int enemyLevel = GameManager.Instance != null ? GameManager.Instance.GetEnemyLevel() : 0;
            int troopSpawnLevel = troopToSpawn.level + enemyLevel;
            
            // Spawn the troop at the selected path
            SpawnTroop(troopToSpawn.troopSO, selectedPath, troopSpawnLevel);
        }
        // If no affordable troop is found, the enemy will wait until it has more souls
    }

    private TroopLevel GetAffordableTroop()
    {
        List<TroopLevel> affordableTroops = new List<TroopLevel>();
        
        foreach (TroopLevel troopLevel in spawnableTroops)
        {
            if (currentSouls >= troopLevel.troopSO.soulCost)
            {
                affordableTroops.Add(troopLevel);
            }
        }
        
        if (affordableTroops.Count > 0)
        {
            // Return a random affordable troop
            return affordableTroops[Random.Range(0, affordableTroops.Count)];
        }
        
        return null; // No affordable troops
    }

    public void SpawnTroop(TroopSO troopSO, LanePath path, int level)
    {
        if (troopSO == null || path == null || troopPrefab == null)
        {
            Debug.LogWarning("Missing troop SO, path, or prefab to spawn enemy troop!");
            return;
        }

        currentSouls -= troopSO.soulCost; // Deduct the cost

        // Instantiate the troop
        GameObject newTroopObject = Instantiate(troopPrefab, Vector3.zero, Quaternion.identity);

        // Get the troop component and assign the path and data
        Troop newTroop = newTroopObject.GetComponent<Troop>();
        if (newTroop != null)
        {
            newTroop.faction = TroopFaction.Enemy; // Set the faction to Enemy
            newTroop.SetPath(path);
            newTroop.SetTroopSO(troopSO);
            newTroop.level = level; // Default to level 1 for enemy troops (could be configurable)
        }

        if (spawnEffectPrefab != null)
        {
            GameObject spawn = Instantiate(spawnEffectPrefab, transform);
            spawn.transform.position = path.GetEndPoint();
        }

        Debug.Log($"Enemy spawned troop: {troopSO.name} at path: {path.name}");
    }

    // Public methods for other scripts to interact with the enemy manager
    public float GetSouls()
    {
        return currentSouls;
    }

    public void IncreaseSouls(float amount)
    {
        currentSouls = Mathf.Min(maxSouls, currentSouls + amount);
    }

    public void DecreaseSouls(float amount)
    {
        currentSouls = Mathf.Max(0, currentSouls - amount);
    }
}