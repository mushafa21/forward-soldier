using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattlePreparationUI : MonoBehaviour
{
    [Header("References")]
    public Transform enemyContainer;  // Container for EnemyCardUI
    public Transform troopContainer;  // Container for available TroopCardUI
    public Transform selectedTroopContainer; // Container for selected TroopCardUI
    public UIButton startButton;  // Button to start the battle
    public Canvas battlePreparationCanvas;  // The canvas for battle preparation

    [Header("Prefabs")]
    public EnemyCardUI enemyCardPrefab;  // Prefab for EnemyCardUI
    public TroopCardUI troopCardPrefab;  // Prefab for TroopCardUI

    [Header("Settings")]
    public int maxSelectableTroops = 5;  // Maximum number of troops that can be selected

    private static BattlePreparationUI instance;
    public static BattlePreparationUI Instance
    {
        get { return instance; }
    }

    private List<EnemyCardUI> enemyCards = new List<EnemyCardUI>();
    private List<TroopCardUI> troopCards = new List<TroopCardUI>();
    private List<TroopCardUI> availableTroops = new List<TroopCardUI>();  // Troops available for selection
    private List<TroopCardUI> selectedTroops = new List<TroopCardUI>();   // Troops selected for battle

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
        if (battlePreparationCanvas != null)
        {
            battlePreparationCanvas.enabled = false; // Initially hide the canvas
        }

        // Clear child objects in containers
        ClearContainer(enemyContainer);
        ClearContainer(troopContainer);

        // Populate enemy cards from GameManager
        PopulateEnemyCards();

        // Populate troop cards from GameManager
        PopulateTroopCards();

        // Setup start button
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartBattle);
        }
    }

    private void ClearContainer(Transform container)
    {
        if (container != null)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void PopulateEnemyCards()
    {
        if (enemyContainer == null || GameManager.Instance == null || enemyCardPrefab == null) return;

        // Get enemy troops from GameManager
        // Note: GameManager.enemyTroops is now List<TroopSO>
        List<TroopSO> enemyTroops = GameManager.Instance.enemyTroops;

        foreach (var troopSO in enemyTroops)
        {
            // Create enemy card from TroopSO using prefab
            if (troopSO != null)
            {
                EnemyCardUI enemyCard = Instantiate(enemyCardPrefab, enemyContainer);
                enemyCard.SetEnemy(troopSO); // Use SetEnemy with TroopSO

                enemyCards.Add(enemyCard);
            }
        }
    }

    private void PopulateTroopCards()
    {
        if (troopContainer == null || GameManager.Instance == null || troopCardPrefab == null) return;

        // Get unlocked troops from GameManager
        List<TroopSO> unlockedTroops = GameManager.Instance.unlockedTroops;

        // Create troop cards for unlocked troops using prefab
        foreach (var troopSO in unlockedTroops)
        {
            TroopCardUI troopCard = Instantiate(troopCardPrefab, troopContainer);
            troopCard.SetTroop(troopSO, 1); // Using level 1 for initial setup
            troopCard.SetInBattlePreparation(true); // Mark as in battle preparation

            // Store in available troops list
            availableTroops.Add(troopCard);
            troopCards.Add(troopCard);
        }
    }

    public void ShowBattlePreparation()
    {
        if (battlePreparationCanvas != null)
        {
            battlePreparationCanvas.enabled = true;
        }
        gameObject.SetActive(true);
    }

    public void HideBattlePreparation()
    {
        if (battlePreparationCanvas != null)
        {
            battlePreparationCanvas.enabled = false;
        }
        gameObject.SetActive(false);
    }

    private void StartBattle()
    {
        // Before hiding the UI, update the TroopManager with selected troops
        UpdateTroopManagerWithSelectedTroops();

        // Hide the battle preparation UI
        HideBattlePreparation();

        // Start the actual battle by calling GameManager or BattleManager
        LevelManager.Instance.StartActualBattle();
    }

    // Update TroopManager with selected troops
    private void UpdateTroopManagerWithSelectedTroops()
    {
        if (TroopManager.Instance != null)
        {
            // Clear current selectable troops
            TroopManager.Instance.selectableTroops.Clear();

            // Add selected troops to TroopManager
            foreach (var troopCard in selectedTroops)
            {
                if (troopCard.troopSO != null)
                {
                    TroopManager.Instance.selectableTroops.Add(troopCard.troopSO);
                }
            }

            // Refresh troop cards in TroopManager
            TroopManager.Instance.RefreshTroopCards();
        }
    }

    // Method called when a troop is selected for battle
    public void SelectTroop(TroopCardUI troopCard)
    {
        if (selectedTroops.Count < maxSelectableTroops && !selectedTroops.Contains(troopCard))
        {
            print("ADD TROOP TO LIST");
            // Add to selected troops
            selectedTroops.Add(troopCard);

            // Disable the card (show disabled image)
            // if (troopCard.disabledImage != null)
            // {
            //     troopCard.disabledImage.gameObject.SetActive(true);
            // }

            // Remove from available troops
            availableTroops.Remove(troopCard);

            // Move the troop card to the selected container
            if (selectedTroopContainer != null)
            {
                troopCard.transform.SetParent(selectedTroopContainer, false);
            }
        }
        else
        {
            print("NOT ADD TROOP TO LIST");
        }
    }

    // Method called when a troop is deselected from battle
    public void DeselectTroop(TroopCardUI troopCard)
    {
        if (selectedTroops.Contains(troopCard))
        {
            // Remove from selected troops
            selectedTroops.Remove(troopCard);

            // Enable the card (hide disabled image)
            // if (troopCard.disabledImage != null)
            // {
            //     troopCard.disabledImage.gameObject.SetActive(false);
            // }

            // Add back to available troops
            availableTroops.Add(troopCard);

            // Move the troop card back to the available container
            if (troopContainer != null)
            {
                troopCard.transform.SetParent(troopContainer, false);
            }
        }
    }

    // Method to check if troop selection limit is reached
    public bool IsSelectionLimitReached()
    {
        return selectedTroops.Count >= maxSelectableTroops;
    }

    // Method to get selected troops
    public List<TroopCardUI> GetSelectedTroops()
    {
        return selectedTroops.ToList();
    }

    // Method to get available troops
    public List<TroopCardUI> GetAvailableTroops()
    {
        return availableTroops.ToList();
    }
}