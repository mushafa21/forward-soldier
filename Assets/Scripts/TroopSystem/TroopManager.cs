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
    public int maxSelectableTroops = 5; // Maximum number of troops that can be in the shop

    private static TroopManager instance;
    public List<TroopLevel> selectableTroops = new List<TroopLevel>();
    public GameObject troopCardPrefab;
    private List<TroopCardUI> _troopCards;
    public TroopCardUI currentSelectedTroop;
    private GameObject troopCardContainer;
    
    
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        troopCardContainer = GameManager.Instance.troopContainer;
        _troopCards = new List<TroopCardUI>();
        
        // Clear all existing troop card children
        foreach (Transform child in troopCardContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        // Create troop cards based on selectableTroops
        foreach (TroopLevel troopLevel in selectableTroops)
        {
            GameObject troopCardObj = Instantiate(troopCardPrefab, troopCardContainer.transform);
            TroopCardUI troopCardUI = troopCardObj.GetComponent<TroopCardUI>();
            
            if (troopCardUI != null)
            {
                troopCardUI.SetTroop(troopLevel.troopSO, troopLevel.level);
                _troopCards.Add(troopCardUI);
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
            foreach (TroopLevel troopLevel in selectableTroops)
            {
                GameObject troopCardObj = Instantiate(troopCardPrefab, troopCardContainer.transform);
                TroopCardUI troopCardUI = troopCardObj.GetComponent<TroopCardUI>();

                if (troopCardUI != null)
                {
                    troopCardUI.SetTroop(troopLevel.troopSO, troopLevel.level);
                    _troopCards.Add(troopCardUI);
                }
            }
        }
    }
    
    public void RemoveTroopFromShop(TroopCardUI troopCardUI)
    {
        // Find and remove the troop from selectableTroops
        TroopLevel troopToRemove = null;
        foreach (TroopLevel troopLevel in selectableTroops)
        {
            if (troopLevel.troopSO == troopCardUI.troopSO)
            {
                troopToRemove = troopLevel;
                break;
            }
        }
        
        if (troopToRemove != null)
        {
            selectableTroops.Remove(troopToRemove);
            // Refresh the troop cards to reflect the change
            RefreshTroopCards();
        }
    }
}