using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Shop Configuration")]
    public GameObject shopItemPrefab; // Prefab for individual shop items
    public Transform shopItemContainer; // Container to hold shop item UIs
    public int numberOfItemsToShow = 5; // Number of items to show in the shop

    [Header("All Shop Items")]
    public List<ShopItemSO> allShopItems; // List of all possible shop items

    [Header("References")]
    public Button refreshButton; // Button to refresh the shop items

    public UIButton nextlevelButton;

    private List<ShopItemSO> currentShopItems; // Currently displayed shop items
    private List<ShopItemUI> shopItemUIs; // List of active shop item UIs

    void Start()
    {
        currentShopItems = new List<ShopItemSO>();
        shopItemUIs = new List<ShopItemUI>();
        
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshShop);
        }
        
        nextlevelButton.onClick.AddListener(GoToNextLevel);
        
        RefreshShop();
    }

    void GoToNextLevel()
    {
        GameManager.Instance.GoToNextLevel();
    }

    
    



    public void RefreshShop()
    {
        // Clear existing shop item UIs
        foreach (Transform child in shopItemContainer)
        {
            Destroy(child.gameObject);
        }
        shopItemUIs.Clear();
        
        // Select random items from all shop items
        SelectRandomShopItems();
        
        // Create UI for each selected item
        foreach (ShopItemSO item in currentShopItems)
        {
            GameObject itemGO = Instantiate(shopItemPrefab, shopItemContainer);
            ShopItemUI itemUI = itemGO.GetComponent<ShopItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(item, this);
                shopItemUIs.Add(itemUI);
            }
        }
    }

    void SelectRandomShopItems()
    {
        currentShopItems.Clear();
        
        if (allShopItems == null || allShopItems.Count == 0) return;

        // Use a copy of the list to avoid modifying the original
        List<ShopItemSO> availableItems = new List<ShopItemSO>(allShopItems);
        
        // Select random items up to the specified number or available items
        int itemsToSelect = Mathf.Min(numberOfItemsToShow, availableItems.Count);
        
        for (int i = 0; i < itemsToSelect; i++)
        {
            if (availableItems.Count == 0) break;
            
            int randomIndex = Random.Range(0, availableItems.Count);
            currentShopItems.Add(availableItems[randomIndex]);
            availableItems.RemoveAt(randomIndex);
        }
    }

    public void AttemptPurchase(ShopItemUI shopItemUI)
    {
        if (shopItemUI == null || shopItemUI.shopItemData == null) return;

        // Check if player can afford the item
        if (!shopItemUI.CanAfford())
        {
            Debug.Log("Cannot afford this item!");
            return;
        }

        // Deduct the price from player's gold
        GameManager.Instance.currentGold -= shopItemUI.shopItemData.price;
        GameManager.Instance.UpdateGold();

        // Process the purchase based on item type
        ProcessPurchase(shopItemUI.shopItemData);

        // Remove the purchased item from the current shop display
        RemovePurchasedItem(shopItemUI);
    }

    void ProcessPurchase(ShopItemSO itemData)
    {
        switch (itemData.type)
        {
            case ShopItemType.TowerUpgrade:
                HandleTowerUpgrade();
                break;
                
            case ShopItemType.SoulUpgrade:
                HandleSoulUpgrade();
                break;
                
            case ShopItemType.Troop:
                // Check if max troop limit is reached and troop doesn't already exist
                if (TroopManager.Instance.IsMaxTroopsReached() && !TroopAlreadyExists(itemData.troopSO))
                {
                    Debug.Log("Maximum number of troops reached! Cannot purchase new troop.");
                    // We should provide UI feedback to the player but for now just log
                    return;
                }
                HandleTroopPurchase(itemData.troopSO);
                break;
                
            default:
                Debug.LogWarning("Unknown shop item type: " + itemData.type);
                break;
        }
    }
    
    bool TroopAlreadyExists(TroopSO troopSO)
    {
        foreach (TroopLevel troopLevel in TroopManager.Instance.selectableTroops)
        {
            if (troopLevel.troopSO == troopSO)
            {
                return true;
            }
        }
        return false;
    }
    
    void RemovePurchasedItem(ShopItemUI purchasedItemUI)
    {
        // Find the index of the purchased item in currentShopItems
        int indexToRemove = -1;
        for (int i = 0; i < currentShopItems.Count; i++)
        {
            if (currentShopItems[i] == purchasedItemUI.shopItemData)
            {
                indexToRemove = i;
                break;
            }
        }
        
        if (indexToRemove != -1)
        {
            // Remove the item from the current shop display
            currentShopItems.RemoveAt(indexToRemove);
            
            // Remove the corresponding UI element
            if (indexToRemove < shopItemUIs.Count)
            {
                ShopItemUI itemUI = shopItemUIs[indexToRemove];
                if (itemUI != null && itemUI.gameObject != null)
                {
                    Destroy(itemUI.gameObject);
                }
                shopItemUIs.RemoveAt(indexToRemove);
            }
            
            // Add a new random item to replace the purchased one
            // AddNewItemToShop();
        }
    }
    
    void AddNewItemToShop()
    {
        if (allShopItems == null || allShopItems.Count == 0) return;
        
        // Create a list of items that are not currently in the shop
        List<ShopItemSO> availableItems = new List<ShopItemSO>();
        foreach (ShopItemSO item in allShopItems)
        {
            if (!currentShopItems.Contains(item))
            {
                availableItems.Add(item);
            }
        }
        
        // If there are available items, add a random one
        if (availableItems.Count > 0)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            ShopItemSO newItem = availableItems[randomIndex];
            
            // Add to current shop items
            currentShopItems.Add(newItem);
            
            // Create UI for the new item
            GameObject itemGO = Instantiate(shopItemPrefab, shopItemContainer);
            ShopItemUI itemUI = itemGO.GetComponent<ShopItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(newItem, this);
                shopItemUIs.Add(itemUI);
            }
        }
    }

    void HandleTowerUpgrade()
    {
        // Increase tower health by 20% per upgrade (adjust as needed)
        float healthIncreasePercentage = 0.2f; // 20% increase per upgrade
        float baseHealth = 1000f; // Base tower health
        
        // Save the tower level in GameManager first (to get the next level)
        GameManager.Instance.IncreaseTowerLevel();
        int newTowerLevel = GameManager.Instance.GetTowerLevel();
        
        // Get the player tower and increase its health
        if (LevelManager.Instance != null && LevelManager.Instance.playerTower != null)
        {
            // New max health = baseHealth * (1 + healthIncreasePercentage) ^ (newTowerLevel - 1)
            // Level 1: 100% of base (1000) - initial state
            // Level 2: 120% of base (1200) - after first upgrade
            // Level 3: 144% of base (1440) - after second upgrade, etc.
            float newMaxHealth = baseHealth * Mathf.Pow(1f + healthIncreasePercentage, newTowerLevel - 1);
            
            LevelManager.Instance.playerTower.maxHealth = newMaxHealth;
            LevelManager.Instance.playerTower.currentHealth = newMaxHealth;
            
            // Update UI 
            LevelManager.Instance.playerTower.UpdateUI();
        }
        
        Debug.Log("Tower upgraded! New max health: " + (LevelManager.Instance?.playerTower?.maxHealth));
    }

    void HandleSoulUpgrade()
    {
        // Decrease the interval time in SoulManager by 10% per upgrade (adjust as needed)
        float intervalReductionPercentage = 0.1f; // 10% reduction per upgrade
        float baseInterval = 5f; // Base interval time
        
        // Save the soul upgrade level in GameManager first (to get the next level)
        GameManager.Instance.IncreaseSoulUpgradeLevel();
        int newSoulUpgradeLevel = GameManager.Instance.GetSoulUpgradeLevel();
        
        if (SoulSystem.SoulManager.Instance != null)
        {
            // Calculate new interval = baseInterval * (1 - intervalReductionPercentage) ^ (newLevel - 1)
            // Level 1: 100% of base (5s) - initial state
            // Level 2: 90% of base (4.5s) - after first upgrade
            // Level 3: 81% of base (4.05s) - after second upgrade, etc.
            float newInterval = baseInterval * Mathf.Pow(1f - intervalReductionPercentage, newSoulUpgradeLevel - 1);
            
            // Ensure the interval doesn't go too low (minimum 0.5 seconds)
            newInterval = Mathf.Max(newInterval, 0.5f);
            
            SoulSystem.SoulManager.Instance.intervalTime = newInterval;
        }
        
        Debug.Log("Soul generation upgraded! New interval: " + (SoulSystem.SoulManager.Instance?.intervalTime));
    }

    void HandleTroopPurchase(TroopSO troopSO)
    {
        if (troopSO == null) 
        {
            Debug.LogError("Tried to purchase a troop but troopSO is null!");
            return;
        }

        // Check if this troop already exists in TroopManager's selectable troops
        bool troopExists = false;
        int existingIndex = -1;
        
        for (int i = 0; i < TroopManager.Instance.selectableTroops.Count; i++)
        {
            if (TroopManager.Instance.selectableTroops[i].troopSO == troopSO)
            {
                troopExists = true;
                existingIndex = i;
                break;
            }
        }

        if (troopExists)
        {
            // If troop exists, increase its level by 1
            TroopManager.Instance.selectableTroops[existingIndex].level++;
            
            Debug.Log($"Added level to existing troop: {troopSO.name}. New level: {TroopManager.Instance.selectableTroops[existingIndex].level}");
        }
        else
        {
            // If troop doesn't exist, add it to selectable troops at level 1
            TroopLevel newTroopLevel = new TroopLevel();
            newTroopLevel.troopSO = troopSO;
            newTroopLevel.level = 1;
            
            TroopManager.Instance.selectableTroops.Add(newTroopLevel);
            
            Debug.Log($"Added new troop to shop: {troopSO.name} at level 1");
        }
        
        // Refresh the troop cards in TroopManager to reflect the changes
        TroopManager.Instance.RefreshTroopCards();
    }
}