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

    [Header("Transition Settings")]
    public GameObject transitionCanvas; // Canvas for the transition effect
    public UnityEngine.UI.Image transitionCircle; // Circle image for the wipe effect
    public float transitionDuration = 1f; // Duration of the transition effect
    
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
            // DontDestroyOnLoad(gameObject);
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
        StartCoroutine(CircleWipeTransition(() => StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex])), false));
    }

    public void UpdateStateText()
    {
        stageText.text =  "STAGE: " + currentStage;
    }
    
    public void GoToNextLevel()
    {
        // SceneManager.UnloadScene("ShopScene");
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
            StartCoroutine(CircleWipeTransition(() => StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex])), false));
        }
        else
        {
            // All levels completed - increase enemy level and repeat the last level
            enemyLevel++;
            Debug.Log($"All levels completed! Enemy level increased to {enemyLevel}. Repeating level {levelScenes[currentLevelIndex]} with increased difficulty.");

            // Load the same level again but with increased enemy difficulty
            StartCoroutine(CircleWipeTransition(() => StartCoroutine(LoadLevelWithDelay(levelScenes[currentLevelIndex])), false));

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
        StartCoroutine(CircleWipeTransition(() => SceneManager.LoadScene("MainMenuScene"), true));
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

    public void RestartLevel()
    {
        StartCoroutine(CircleWipeTransition(() => {
            // Reload the current active scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }));
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
    }

    // Coroutine for circle wipe transition effect
    private IEnumerator CircleWipeTransition(System.Action onComplete, bool skipReveal = false)
    {
        if (transitionCanvas != null && transitionCircle != null)
        {
            // Make the transition canvas persist across scenes
            DontDestroyOnLoad(transitionCanvas);

            // Activate the transition canvas
            transitionCanvas.SetActive(true);

            // Start with a small circle at center to not hide anything initially
            transitionCircle.rectTransform.localScale = Vector3.zero;

            // Animate the circle expanding to cover the screen (wipe out current scene)
            float elapsedTime = 0f;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one * 30f; // Expand to cover everything

            while (elapsedTime < transitionDuration * 0.5f) // Use half the duration for the wipe out
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / (transitionDuration * 0.5f);

                // Use a custom easing for smooth expansion
                float easedProgress = EaseOutCubic(progress);

                transitionCircle.rectTransform.localScale = Vector3.Lerp(startScale, endScale, easedProgress);

                yield return null;
            }

            // Ensure the circle fully covers the screen
            transitionCircle.rectTransform.localScale = endScale;
        }

        // Execute the action (load the new scene)
        onComplete?.Invoke();

        // Wait a moment to ensure scene loads
        yield return new WaitForSeconds(0.05f);

        // Only perform the reverse animation if not skipping it
        if (!skipReveal && transitionCanvas != null && transitionCircle != null)
        {
            // Start with the circle covering everything
            transitionCircle.rectTransform.localScale = Vector3.one * 30f;

            // Animate the circle shrinking to reveal the new scene
            float elapsedTime = 0f;
            Vector3 startScale = Vector3.one * 30f;
            Vector3 endScale = Vector3.zero; // Shrink to nothing to reveal everything

            while (elapsedTime < transitionDuration * 0.5f) // Use half the duration for the reveal
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / (transitionDuration * 0.5f);

                // Use a custom easing for smooth shrinking
                float easedProgress = EaseOutCubic(progress);

                transitionCircle.rectTransform.localScale = Vector3.Lerp(startScale, endScale, easedProgress);

                yield return null;
            }

            // Ensure the circle is fully shrunk at the end
            transitionCircle.rectTransform.localScale = endScale;

            // Hide the transition canvas after the transition completes
            transitionCanvas.SetActive(false);
        }
        else if (transitionCanvas != null)
        {
            // If we're skipping the reveal (e.g. for going to main menu), just hide the canvas
            transitionCanvas.SetActive(false);
        }
    }

    // Easing function for smooth animation
    private float EaseOutCubic(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(1f - t, 3);
    }
}
