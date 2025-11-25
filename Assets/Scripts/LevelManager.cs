


using System;
using System.Collections;
using System.Collections.Generic;
using SoulSystem;
using TowerSystem;
using UnityEngine;
using UnityEngine.UI;
using TroopSystem;


public enum LevelState
{
    preparation,
    battle
}

public class LevelManager : MonoBehaviour
{
    public GameObject introCamera;
    public GameObject battleCamera;

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
    public LevelState currentState = LevelState.preparation;

    public static LevelManager Instance
    {
        get
        {
            return instance;
        }
    }

    [Header("Battle Preparation")]
    public BattlePreparationUI battlePreparationUI;  // Reference to BattlePreparationUI

    private bool isBattleStarted = false;  // Flag to track if battle has started

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

        // Start battle preparation sequence instead of directly starting battle
        StartCoroutine(BattlePreparationSequence());
    }

    private IEnumerator BattlePreparationSequence()
    {
        // Turn on battle camera first
        if (battleCamera != null)
        {
            battleCamera.SetActive(true);
        }

        // Pause for 2 seconds
        yield return new WaitForSeconds(2f);

        // Show the BattlePreparationUI canvas
        if (battlePreparationUI != null)
        {
            battlePreparationUI.ShowBattlePreparation();
        }
        else
        {
            Debug.LogWarning("BattlePreparationUI reference is not set in LevelManager!");
        }
    }

    // This method is called when the start button in BattlePreparationUI is clicked
    public void StartActualBattle()
    {
        if (isBattleStarted) return;  // Prevent multiple starts

        currentState = LevelState.battle;
        isBattleStarted = true;

        // Start EnemyManager (spawning enemies and soul regeneration)
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.StartBattle(); // Use the new StartBattle method to exit battle prep phase
        }

        // Start TroopManager for deploying troops
        if (TroopManager.Instance != null)
        {
            TroopManager.Instance.StartBattle(); // Use the new StartBattle method instead of just enabling
        }
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