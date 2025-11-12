using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public GameObject descriptionObject;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemPriceText;
    public TroopCardUI troopCardUI;
    public UIButton purchaseButton;
    
    [Header("Data")]
    public ShopItemSO shopItemData;
    
    private ShopUI shopUI;
    public bool isShowingDescription = false;

    
    void Start()
    {
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
    }
    
    public void Initialize(ShopItemSO itemData, ShopUI shopParent)
    {
        shopItemData = itemData;
        shopUI = shopParent;
        
        UpdateDisplay();
    }
    
    void UpdateDisplay()
    {
        if (shopItemData == null) return;


        if (shopItemData.type == ShopItemType.Troop)
        {
            troopCardUI.gameObject.SetActive(true);
            itemIcon.gameObject.SetActive(false);
            troopCardUI.SetShopItem(shopItemData.troopSO,1);
        }
        // Set the name and price
        if (itemNameText != null)
        {
            itemNameText.text = GetItemName(shopItemData.type);
        }
        
        if (itemPriceText != null)
        {
            itemPriceText.text = shopItemData.price.ToString("N0");
        }
        
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = GetItemDescription(shopItemData.type);
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = shopItemData.icon;
        }
    }
    
    string GetItemName(ShopItemType type)
    {
        switch (type)
        {
            case ShopItemType.TowerUpgrade:
                return "Tower Upgrade";
            case ShopItemType.SoulUpgrade:
                return "Soul Generation";
            case ShopItemType.Troop:
                return shopItemData.troopSO != null ? shopItemData.troopSO.name : "Troop";
            default:
                return "Unknown Item";
        }
    }
    
    string GetItemDescription(ShopItemType type)
    {
        switch (type)
        {
            case ShopItemType.TowerUpgrade:
                return "Increase your tower's health, Level " + GameManager.Instance.GetTowerLevel() + " -> " + (GameManager.Instance.GetTowerLevel() + 1);
            case ShopItemType.SoulUpgrade:
                return "Increase soul generation speed, Level " + GameManager.Instance.GetSoulUpgradeLevel() + " -> " + (GameManager.Instance.GetSoulUpgradeLevel() + 1);
            case ShopItemType.Troop:
                return shopItemData.troopSO != null ? shopItemData.troopSO.name + " Troop" : "Troop Unit";
            default:
                return "Unknown item";
        }
    }
    
    
    void OnPurchaseClicked()
    {
        if (shopUI != null)
        {
            shopUI.AttemptPurchase(this);
        }
    }
    
    public bool CanAfford()
    {
        if (shopItemData == null) return false;
        return GameManager.Instance.currentGold >= shopItemData.price;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopItemData.type == ShopItemType.Troop) return;
        
        {
            isShowingDescription = true;
            descriptionObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (shopItemData.type == ShopItemType.Troop) return;

        if (isShowingDescription)
        {
            isShowingDescription = false;
            descriptionObject.GetComponent<SlideInAnimator>().HideSlideOut();
        }
    }
}