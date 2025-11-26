using System.Collections.Generic;
using UnityEngine;

public class NewTroopUI : MonoBehaviour
{
    [Header("References")]
    public Transform troopContainer;  // Container for troop cards
    public GameObject emptyUnlocked;  // Empty state GameObject to show when no unlocked troops
    public TroopCardUI troopCardPrefab;  // Prefab for TroopCardUI
    public UIButton nextLevelButton;
    [Header("Visual Data")]
    public List<TroopSO> unlockedTroops = new List<TroopSO>();  // List of unlocked troops for visual purposes

    private List<TroopCardUI> spawnedTroopCards = new List<TroopCardUI>();  // Keep track of spawned troop cards

    private void Start()
    {
        RefreshTroopDisplay();
        nextLevelButton.onClick.AddListener(NextLevel);
    }

    private void NextLevel()
    {
        GameManager.Instance.GoToNextLevel();
    }

    // Method to refresh the troop display
    public void RefreshTroopDisplay()
    {
        // Clear existing troop cards
        ClearTroopCards();

        // Check if there are any unlocked troops
        if (unlockedTroops.Count == 0)
        {
            // Show the empty state
            if (emptyUnlocked != null)
            {
                emptyUnlocked.SetActive(true);
            }

            // Hide the troop container if there's nothing to show
            if (troopContainer != null)
            {
                troopContainer.gameObject.SetActive(false);
            }
        }
        else
        {
            // Hide the empty state
            if (emptyUnlocked != null)
            {
                emptyUnlocked.SetActive(false);
            }

            // Show the troop container
            if (troopContainer != null)
            {
                troopContainer.gameObject.SetActive(true);

                // Spawn troop cards for each unlocked troop
                foreach (TroopSO troopSO in unlockedTroops)
                {
                    if (troopSO != null && troopCardPrefab != null && troopContainer != null)
                    {
                        TroopCardUI troopCard = Instantiate(troopCardPrefab, troopContainer);
                        troopCard.SetShopItem(troopSO, 1); // Use level 1 for display purposes
                        spawnedTroopCards.Add(troopCard);
                    }
                }
            }
        }
    }

    // Method to clear existing troop cards
    private void ClearTroopCards()
    {
        if (spawnedTroopCards != null)
        {
            foreach (TroopCardUI troopCard in spawnedTroopCards)
            {
                if (troopCard != null)
                {
                    Destroy(troopCard.gameObject);
                }
            }
            spawnedTroopCards.Clear();
        }

        // Alternative: Clear all children if any remain
        if (troopContainer != null)
        {
            foreach (Transform child in troopContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}