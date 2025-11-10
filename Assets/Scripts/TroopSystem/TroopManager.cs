using UnityEngine;
using System.Collections.Generic;
using TroopSystem;

public class TroopManager : MonoBehaviour
{
    [Header("Troop Management")] public List<Troop> activeTroops = new List<Troop>();

    private static TroopManager instance;
    public TroopSO currentSelectedTroop;
    
    
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

    public void SetCurrentTroop(TroopSO troopSO)
    {
        currentSelectedTroop = troopSO;
    }
}