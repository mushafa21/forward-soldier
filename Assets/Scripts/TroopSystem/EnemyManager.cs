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
    public List<TroopSO> spawnableTroops = new List<TroopSO>(); // List of troops the enemy can spawn
    public float spawnDecisionInterval = 5f; // Time in seconds between spawn decisions

    private bool isInitialized = false;
    private bool isBattlePreparationPhase = true; // Initially in battle preparation
    private TroopSO currentTargetTroop = null; // The troop currently targeted for spawning

    private static EnemyManager instance;
    public static EnemyManager Instance
    {
        get
        {
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
        GameManager.Instance.enemyTroops = spawnableTroops;
        InitializeEnemyManager();
    }

    private void InitializeEnemyManager()
    {
        if (isInitialized) return;

        currentSouls = 0f; // Start with 0 souls
        isInitialized = true;

        // Select a random troop as the current target
        SelectRandomTroop();

        // Start the enemy logic coroutines (they will check battle phase internally)
        StartCoroutine(SoulRegenCoroutine());
        StartCoroutine(EnemyActionCoroutine());
    }

    private IEnumerator SoulRegenCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(soulRegenInterval);
            if (isInitialized && !isBattlePreparationPhase)
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
            if (isInitialized && !isBattlePreparationPhase)
            {
                PerformEnemyAction();
            }
        }
    }

    private void PerformEnemyAction()
    {
        if (spawnableTroops.Count == 0 || spawnPaths.Count == 0 || currentTargetTroop == null)
        {
            return; // Can't do anything if there are no troops or paths to spawn
        }

        // 20% chance to skip spawning this decision
        if (Random.Range(0f, 1f) < 0.2f)
        {
            Debug.Log("Enemy skipped spawning this decision cycle");
            return;
        }

        // Check if the current target troop can be spawned (has enough souls)
        if (currentSouls >= currentTargetTroop.soulCost)
        {
            // Select a random path
            LanePath selectedPath = spawnPaths[Random.Range(0, spawnPaths.Count)];

            // Add enemy level bonus to the troop level
            int enemyLevel = GameManager.Instance != null ? GameManager.Instance.GetEnemyLevel() : 0;
            int troopSpawnLevel = 1 + enemyLevel; // Default to level 1, plus enemy level

            // Spawn the troop at the selected path
            SpawnTroop(currentTargetTroop, selectedPath, troopSpawnLevel);

            // After spawning, select a new random troop
            SelectRandomTroop();
        }
        // If not enough souls, do nothing and wait for next decision
    }

    private void SelectRandomTroop()
    {
        if (spawnableTroops.Count > 0)
        {
            currentTargetTroop = spawnableTroops[Random.Range(0, spawnableTroops.Count)];
            Debug.Log($"Enemy selected new target troop: {currentTargetTroop.name} (Cost: {currentTargetTroop.soulCost})");
        }
        else
        {
            currentTargetTroop = null;
            Debug.LogWarning("No spawnable troops available!");
        }
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

    // Method to start the actual battle (called by LevelManager)
    public void StartBattle()
    {
        isBattlePreparationPhase = false;
    }
}