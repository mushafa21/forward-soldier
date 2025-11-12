using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class TroopCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Button button;
    public Image troopPotrait;
    public TextMeshProUGUI troopCostText;
    public TroopSO troopSO;
    public GameObject selectedIndicator;
    public TextMeshProUGUI coolDownText;
    public Image coolDownImage;
    public TextMeshProUGUI nameText,healthText,attactText,speedText,levelText;
    public int currentLevel = 1;
    
    [Header("Remove Button")]
    public Button removeButton; // Button to remove this troop from the shop
    
    public bool isSelected = false;
    public bool isInCooldown = false;
    public GameObject statsObject;

    public bool isShowingStats = false;

    private bool _isShopItem = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!_isShopItem)
        {
            button.onClick.AddListener(SelectTroop);
        }
        
        if (removeButton != null)
        {
            removeButton.onClick.AddListener(RemoveTroop);
        }

        if (troopSO != null)
        {
            InitTroop();

        }
        
    }

    public void SetTroop(TroopSO troop, int level)
    {
        troopSO = troop;
        currentLevel = level;
    }
    
    public void SetShopItem(TroopSO troop, int level)
    {
        troopSO = troop;
        currentLevel = level;
        _isShopItem = true;
        levelText.gameObject.SetActive(false);
        InitTroop();

    }

    

    void SelectTroop()
    {
        print("Select Troop Called");
        TroopManager.Instance.SetCurrentTroop(this);
    }

    void InitTroop()
    {
        troopPotrait.sprite = troopSO.potrait;
        troopCostText.text = troopSO.soulCost.ToString();
        attactText.text = troopSO.GetDamageAtLevel(currentLevel).ToString("F0"); // Format to integer
        healthText.text = troopSO.GetHealthAtLevel(currentLevel).ToString("F0"); // Format to integer
        speedText.text = troopSO.speedCategory;
        nameText.text = troopSO.name;
        levelText.text = "LVL " + currentLevel;

    }

    public void StartCooldown()
    {
        if (troopSO == null) return;
        
        isInCooldown = true;
        coolDownImage.fillAmount = 1f; // Start with full fill
        coolDownImage.gameObject.SetActive(true);
        
        // Start the cooldown coroutine
        StartCoroutine(CooldownRoutine(troopSO.spawnCooldown));
    }
    
    private System.Collections.IEnumerator CooldownRoutine(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = 1f - (elapsed / duration); // Decreases from 1 to 0
            coolDownImage.fillAmount = progress;
            
            // Update cooldown text
            int secondsRemaining = Mathf.CeilToInt(duration - elapsed);
            coolDownText.text = secondsRemaining.ToString();
            coolDownText.gameObject.SetActive(true);
            
            yield return null;
        }
        
        // Cooldown finished
        isInCooldown = false;
        coolDownImage.gameObject.SetActive(false);
        coolDownText.gameObject.SetActive(false);
    }
    
    public void Select()
    {
        isSelected = true;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(true);
    }
    
    public void Deselect()
    {
        isSelected = false;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isShowingStats)
        {
            isShowingStats = true;
            statsObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isShowingStats)
        {
            isShowingStats = false;
            statsObject.GetComponent<SlideInAnimator>().HideSlideOut();
        }
    }
    
    void RemoveTroop()
    {
        // Remove this troop from the TroopManager's selectable troops
        TroopManager.Instance.RemoveTroopFromShop(this);
    }
}
