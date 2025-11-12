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
    public GameObject playerHud;
    
    // Tower upgrade level
    public int towerLevel = 1;
    
    // Soul upgrade level
    public int soulUpgradeLevel = 1;

    [Header("Level Management")]
    public int currentLevelIndex = 0; // Index of the current level in the scenes list
    public List<string> levelScenes; // List of level scene names

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
        playerHud.SetActive(false);

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
        StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex]));

    }
    
    public void GoToNextLevel()
    {
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
            Debug.Log("No more levels! Reached the end of the level list.");
            // Optionally, you could implement a "game completed" state here
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
    
    private IEnumerator LoadLevelWithDelay(string sceneName)
    {
        // Optional: Add a loading screen or delay
        yield return null; // Wait one frame
        
        SceneManager.LoadScene(sceneName);
        playerHud.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
        playerHud.SetActive(false);
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
