


using System;
using System.Collections;
using System.Collections.Generic;
using SoulSystem;
using TowerSystem;
using UnityEngine;
using UnityEngine.UI;
using TroopSystem;
using Unity.Cinemachine;


public enum LevelState
{
    preparation,
    battle
}

public class LevelManager : MonoBehaviour
{
    public GameObject playerTowerCamera;
    public GameObject enemyTowerCamera;
    public GameObject battleCamera;
    public SlideInAnimator victorySlideIn;
    public SlideInAnimator DefeatSlideIn;

    public GameObject victoryScreen;
    public UIButton continueButton;
    public UIButton backToMenuButton;
    public UIButton resetButton;

    public GameObject deathScreen;
    public Tower playerTower;
    public Tower enemyTower;
    public AudioClip victorySound;
    public AudioClip battleSound;
    public AudioClip loseSound;
    public AudioClip preparationSound;
    public AudioClip hornSound;
    public AudioClip boomSound;

    public List<TroopSO> unlockedTroops = new List<TroopSO>();


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

    [Header("Countdown UI")]
    public GameObject countdownCanvas;  // Canvas for the countdown text
    public TMPro.TextMeshProUGUI countdownText;  // Text for the countdown

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
        AudioManager.Instance.PlayMusic(preparationSound);
        GameManager.Instance.UpdateStateText();
        GameManager.Instance.ApplyEnemyLevelChanges();
        battleCamera.gameObject.SetActive(false);
        resetButton.onClick.AddListener(ResetLevel);

        UIManager.Instance.HideHUD();
  
        // Start battle preparation sequence instead of directly starting battle
        StartCoroutine(BattlePreparationSequence());
        GameManager.Instance.unlockedTroops = unlockedTroops;
    }

    private void ResetLevel()
    {
        GameManager.Instance.RestartLevel();
    }

    private IEnumerator BattlePreparationSequence()
    {
        yield return new WaitForSeconds(0.5f);

        // Turn on battle camera first
        if (battleCamera != null)
        {
            battleCamera.SetActive(true);
        }

        // Pause for 2 seconds
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ShowHUD();


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

        // Hide the battle preparation UI first
        battlePreparationUI.HideBattlePreparation();

        // Start the countdown sequence
        StartCoroutine(CountdownSequence());
    }

    // Coroutine for the countdown sequence
    private IEnumerator CountdownSequence()
    {
        if (countdownCanvas != null)
        {
            countdownCanvas.SetActive(true);  // Show the countdown canvas
        }

        // Show READY
        if (countdownText != null)
        {
            countdownText.text = "READY";
        }
        AudioManager.Instance.PlaySFX(boomSound);
        yield return new WaitForSeconds(1f);

        // Show SET
        if (countdownText != null)
        {
            countdownText.text = "SET";
        }
        AudioManager.Instance.PlaySFX(boomSound);
        yield return new WaitForSeconds(1f);

        // Show FORWARD!
        if (countdownText != null)
        {
            countdownText.text = "FORWARD!";
        }
        AudioManager.Instance.PlaySFX(hornSound);

        yield return new WaitForSeconds(1f);

        // Hide the countdown canvas and start the actual battle
        if (countdownCanvas != null)
        {
            countdownCanvas.SetActive(false);
        }

        // Start the actual battle
        StartBattleProcess();
    }

    // Method that handles the actual battle start process
    private void StartBattleProcess()
    {
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
        AudioManager.Instance.PlayMusic(battleSound);

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
        AudioManager.Instance.PlayMusic(victorySound);

        UIManager.Instance.HideHUD();

        // // Set the Cinemachine blend speed to 0.5 seconds
        // if (cinemachineBrain != null)
        // {
        //     cinemachineBrain.m_DefaultBlend.Duration = 0.5f;
        // }

        enemyTowerCamera.SetActive(true);

        // Wait 1 second before showing the victory screen
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;

        // Show victory screen
        victoryScreen.SetActive(true);
        victorySlideIn.ShowSlideIn();
        GameManager.Instance.IncreaseGold(1000);
    }

    private IEnumerator GameOverSequence()
    {
        // Remove all troops from the scene
        RemoveAllTroops();
        AudioManager.Instance.PlayMusic(loseSound);
        UIManager.Instance.HideHUD();

        // // Set the Cinemachine blend speed to 0.5 seconds
        // if (cinemachineBrain != null)
        // {
        //     cinemachineBrain.m_DefaultBlend.Duration = 0.5f;
        // }

        playerTowerCamera.SetActive(false);
        playerTowerCamera.SetActive(true);

        // Wait 1 second before showing the game over screen
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;

        // Show game over screen
        deathScreen.SetActive(true);
        DefeatSlideIn.ShowSlideIn();

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