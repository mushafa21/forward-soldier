using UnityEngine;
using System.Collections.Generic;
using TroopSystem;

public class TroopManager : MonoBehaviour
{
    [Header("Troop Management")] public List<Troop> activeTroops = new List<Troop>();

    private static TroopManager instance;
    public List<TroopSO> selectableTroops = new List<TroopSO>();
    public GameObject troopCardPrefab;
    private List<TroopCardUI> _troopCards;
    public TroopCardUI currentSelectedTroop;
    public GameObject troopCardContainer;
    
    
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
                troopCardUI.SetTroop(troopSO);
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
}