using UnityEngine;
using System.Collections.Generic;
using TroopSystem;


[System.Serializable]
public class TroopLevel
{
    public int level;
    public TroopSO troopSO;
}

public class TroopManager : MonoBehaviour
{
    [Header("Troop Management")] public List<Troop> activeTroops = new List<Troop>();

    [Header("Shop Settings")]
    public int maxSelectableTroops = 5; // Maximum number of troops that can be used

    private static TroopManager instance;
    public List<TroopSO> selectableTroops = new List<TroopSO>();
    public GameObject troopCardPrefab;
    private List<TroopCardUI> _troopCards;
    public TroopCardUI currentSelectedTroop;
    private GameObject troopCardContainer;

    private bool _battleStarted = false; // Track if battle has started


    public static TroopManager Instance
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

    private void Start()
    {
        // Initially disable the TroopManager during battle preparation
        this.enabled = false;

        troopCardContainer = GameManager.Instance.troopContainer;
        _troopCards = new List<TroopCardUI>();

        // Clear all existing troop card children
        foreach (Transform child in troopCardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Create troop cards based on selectableTroops
        foreach (TroopSO troopSO in selectableTroops)
        {
            GameObject troopCardObj = Instantiate(troopCardPrefab, troopCardContainer.transform);
            TroopCardUI troopCardUI = troopCardObj.GetComponent<TroopCardUI>();

            if (troopCardUI != null)
            {
                troopCardUI.SetTroop(troopSO, 1); // Use level 1 since level feature is removed
                // Mark as not in battle preparation initially
                troopCardUI.SetInBattlePreparation(false);
                _troopCards.Add(troopCardUI);
            }
        }
    }

    // Method called when battle starts
    public void StartBattle()
    {
        _battleStarted = true;
        this.enabled = true; // Enable TroopManager for battle

        // Update all troop cards that battle has started
        foreach (var troopCard in _troopCards)
        {
            if (troopCard != null)
            {
                troopCard.OnBattleStart();
            }
        }
    }

    // Add a troop to the manager
    public void AddTroop(Troop troop)
    {
        if (troop != null && !activeTroops.Contains(troop))
        {
            activeTroops.Add(troop);
        }
    }

    // Remove a troop from the manager
    public void RemoveTroop(Troop troop)
    {
        if (troop != null)
        {
            activeTroops.Remove(troop);
        }
    }

    // Get all active troops
    public List<Troop> GetAllTroops()
    {
        return activeTroops;
    }

    // Get all troops of a specific faction
    public List<Troop> GetTroopsByFaction(TroopFaction faction)
    {
        List<Troop> factionTroops = new List<Troop>();

        foreach (Troop troop in activeTroops)
        {
            if (troop != null && troop.faction == faction && troop.isActiveAndEnabled)
            {
                factionTroops.Add(troop);
            }
        }

        return factionTroops;
    }

    // Get troops by path
    public List<Troop> GetTroopsByPath(PathSystem.LanePath path)
    {
        List<Troop> pathTroops = new List<Troop>();

        foreach (Troop troop in activeTroops)
        {
            if (troop != null && troop.currentPath == path && troop.isActiveAndEnabled)
            {
                pathTroops.Add(troop);
            }
        }

        return pathTroops;
    }

    public void SetCurrentTroop(TroopCardUI troopCard)
    {
        currentSelectedTroop = troopCard;
        
        // Deselect all troop cards
        foreach (TroopCardUI card in _troopCards)
        {
            if (card != null)
            {
                card.Deselect();
            }
        }
        
        // Select the current troop card
        if (troopCard != null)
        {
            troopCard.Select();
        }
    }

    private void Update()
    {
        // Handle keyboard shortcuts for troop selection (1-9 keys)
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) // KeyCode.Alpha1 to KeyCode.Alpha9
            {
                SelectTroopByIndex(i);
                break;
            }
        }
    }

    private void SelectTroopByIndex(int index)
    {
        // Check if the index is within the bounds of the _troopCards list
        if (index >= 0 && index < _troopCards.Count && _troopCards[index] != null)
        {
            SetCurrentTroop(_troopCards[index]);
        }
        // If index is out of bounds or the troop card is null, do nothing
    }
    
    public bool IsMaxTroopsReached()
    {
        return selectableTroops.Count >= maxSelectableTroops;
    }
    
    public void RefreshTroopCards()
    {
        if (_troopCards != null)
        {
            // Clear all existing troop card children
            foreach (Transform child in troopCardContainer.transform)
            {
                Destroy(child.gameObject);
            }
            
            _troopCards.Clear();

            // Create troop cards based on selectableTroops
            foreach (TroopSO troopSO in selectableTroops)
            {
                GameObject troopCardObj = Instantiate(troopCardPrefab, troopCardContainer.transform);
                TroopCardUI troopCardUI = troopCardObj.GetComponent<TroopCardUI>();

                if (troopCardUI != null)
                {
                    troopCardUI.SetTroop(troopSO, 1); // Use level 1 since level feature is removed
                    _troopCards.Add(troopCardUI);
                }
            }
        }
    }
    
    public void RemoveTroopFromShop(TroopCardUI troopCardUI)
    {
        // Find and remove the troop from selectableTroops
        if (selectableTroops.Contains(troopCardUI.troopSO))
        {
            selectableTroops.Remove(troopCardUI.troopSO);
            // Refresh the troop cards to reflect the change
            RefreshTroopCards();
        }
    }
}