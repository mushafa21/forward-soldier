


using System;
using System.Collections;
using System.Collections.Generic;
using SoulSystem;
using TowerSystem;
using UnityEngine;
using UnityEngine.UI;
using TroopSystem;

public class LevelManager : MonoBehaviour
{
    
    
    public GameObject victoryScreen;
    public UIButton continueButton;
    public UIButton backToMenuButton;

    public GameObject deathScreen;
    public Tower playerTower;
    public Tower enemyTower;
    // public AudioClip victorySound;
    public AudioClip battleSound;
    // public AudioClip loseSound;
    
 
    private static LevelManager instance;

    public static LevelManager Instance
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
        Time.timeScale = 1;
        GameManager.Instance.playerHud.SetActive(true);
        GameManager.Instance.UpdateGold();
        AudioManager.Instance.PlayMusic(battleSound);
        GameManager.Instance.UpdateStateText();
        GameManager.Instance.ApplyEnemyLevelChanges();
    }

    private void OnEnable()
    {
        continueButton.onClick.AddListener(GoToShop);
        backToMenuButton.onClick.AddListener(BackToMenu);
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.RemoveAllListeners();

    }

    private void GoToShop()
    {
        victoryScreen.SetActive(false);
        GameManager.Instance.OpenShop();
    }

    private void BackToMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }

    public void ShowVictoryScreen()
    {
        Time.timeScale = 0;
        victoryScreen.SetActive(true);
        GameManager.Instance.IncreaseGold(1000);
        
    }
    
    public void ShowGameOverScreen()
    {
        Time.timeScale = 0;
        deathScreen.SetActive(true);

    }
    
    public void StartVictorySequence()
    {
        SoulManager.Instance.ResetSouls();
        StartCoroutine(VictorySequence());
    }
    
    public void StartGameOverSequence()
    {
        SoulManager.Instance.ResetSouls();
        StartCoroutine(GameOverSequence());
    }
    
    private IEnumerator VictorySequence()
    {
        // Remove all troops from the scene
        RemoveAllTroops();
        
        // Wait 1 second before showing the victory screen
        yield return new WaitForSeconds(1f);
        
        // Show victory screen
        Time.timeScale = 0;
        victoryScreen.SetActive(true);
        GameManager.Instance.IncreaseGold(1000);
    }
    
    private IEnumerator GameOverSequence()
    {
        // Remove all troops from the scene
        RemoveAllTroops();
        
        // Wait 1 second before showing the game over screen
        yield return new WaitForSeconds(1f);
        
        // Show game over screen
        Time.timeScale = 0;
        deathScreen.SetActive(true);
    }
    
    private void RemoveAllTroops()
    {
        // Get all active troops from the TroopManager
        List<Troop> allTroops = TroopManager.Instance.GetAllTroops();
        
        // Create a copy of the list to avoid modification during iteration
        List<Troop> troopsToDestroy = new List<Troop>(allTroops);
        
        // Destroy each troop
        foreach (Troop troop in troopsToDestroy)
        {
            if (troop != null)
            {
                // Use DestroyImmediate if in play mode, otherwise Destroy
                Destroy(troop.gameObject);
            }
        }
        
        // Clear the active troops list in TroopManager
        TroopManager.Instance.activeTroops.Clear();
    }
}