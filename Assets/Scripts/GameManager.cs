using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public int currentGold = 0;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI stageText;
    public GameObject playerHud;
    public GameObject troopContainer;
    
    // Tower upgrade level
    public int towerLevel = 1;

    public int currentStage = 1;
    // Soul upgrade level
    public int soulUpgradeLevel = 1;

    [Header("Level Management")]
    public int currentLevelIndex = 0; // Index of the current level in the scenes list
    public List<string> levelScenes; // List of level scene names
    
    [Header("Enemy Level")]
    public int enemyLevel = 0; // Level of enemy difficulty, increases after completing all levels

    public List<TroopSO> enemyTroops = new List<TroopSO>();
    public List<TroopSO> unlockedTroops = new List<TroopSO>();

    public static GameManager Instance
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goldText.text = currentGold.ToString("N0");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseGold(int amount)
    {
        currentGold += amount;
        goldText.text = currentGold.ToString("N0");
    }

    public void UpdateGold()
    {
        goldText.text = currentGold.ToString("N0");
    }
    
    public void IncreaseTowerLevel()
    {
        towerLevel++;
    }
    
    public void IncreaseSoulUpgradeLevel()
    {
        soulUpgradeLevel++;
    }
    
    public int GetTowerLevel()
    {
        return towerLevel;
    }
    
    public int GetSoulUpgradeLevel()
    {
        return soulUpgradeLevel;
    }

    public void GoToFirstLevel()
    {
        currentLevelIndex = 0;
        currentGold = 0;
        towerLevel = 1;
        soulUpgradeLevel = 1;
        currentStage = 1;
        StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex]));

    }

    public void UpdateStateText()
    {
        stageText.text =  "STAGE: " + currentStage;
    }
    
    public void GoToNextLevel()
    {
        SceneManager.UnloadScene("ShopScene");
        currentStage++;
        if (levelScenes == null || levelScenes.Count == 0)
        {
            Debug.LogWarning("No level scenes defined in GameManager!");
            return;
        }
        
        // Check if there's a next level
        if (currentLevelIndex < levelScenes.Count - 1)
        {
            currentLevelIndex++;
            StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex]));
        }
        else
        {
            // All levels completed - increase enemy level and repeat the last level
            enemyLevel++;
            Debug.Log($"All levels completed! Enemy level increased to {enemyLevel}. Repeating level {levelScenes[currentLevelIndex]} with increased difficulty.");
            
            // Load the same level again but with increased enemy difficulty
            StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex]));
            
        }
    }
    
    public void GoToLevel(int levelIndex)
    {
        if (levelScenes == null || levelScenes.Count == 0)
        {
            Debug.LogWarning("No level scenes defined in GameManager!");
            return;
        }
        
        if (levelIndex >= 0 && levelIndex < levelScenes.Count)
        {
            currentLevelIndex = levelIndex;
            StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex]));
        }
        else
        {
            Debug.LogWarning($"Level index {levelIndex} is out of range. Available levels: 0 to {levelScenes.Count - 1}");
        }
    }
    
    public void ApplyEnemyLevelChanges()
    {
        // Apply enemy level changes to affect the next level
        // This method can be called from other parts of the game as needed
        if (LevelManager.Instance != null && LevelManager.Instance.enemyTower != null)
        {
            // Increase enemy tower max health by 50 per enemy level
            float healthIncreasePerLevel = 50f;
            LevelManager.Instance.enemyTower.maxHealth += enemyLevel * healthIncreasePerLevel;
            LevelManager.Instance.enemyTower.currentHealth = LevelManager.Instance.enemyTower.maxHealth;
            LevelManager.Instance.enemyTower.UpdateUI();
        }
        
        // For troop level increases, we'll handle that in EnemyManager when it spawns troops
        Debug.Log($"Enemy level changes applied: Enemy tower health increased by {enemyLevel * 50} (+50 per level). Enemy troops will spawn at higher levels.");
    }
    
    public int GetEnemyLevel()
    {
        return enemyLevel;
    }
    
    private IEnumerator LoadLevelWithDelay(string sceneName)
    {
        // Optional: Add a loading screen or delay
        yield return null; // Wait one frame
        
        SceneManager.LoadScene(sceneName);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    
    public string GetCurrentLevelName()
    {
        if (levelScenes != null && currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Count)
        {
            return levelScenes[currentLevelIndex];
        }
        return "";
    }
    
    public int GetTotalLevels()
    {
        return levelScenes != null ? levelScenes.Count : 0;
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
    }
}
