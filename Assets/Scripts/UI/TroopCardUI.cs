using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class TroopCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")] public Button button;
    public Image troopPotrait;
    public Image troopClass;
    public TextMeshProUGUI troopName;
    public TextMeshProUGUI troopDescription;
    public TextMeshProUGUI troopAttackSpeed;

    public TextMeshProUGUI troopCostText;
    public TroopSO troopSO;
    public GameObject selectedIndicator;
    public TextMeshProUGUI coolDownText;
    public Image coolDownImage;
    public Image disabledImage;
    public TextMeshProUGUI healthText, attactText, defenseText, speedText, levelText;
    public int currentLevel = 1;

    [Header("Remove Button")] public Button removeButton; // Button to remove this troop from the shop

    public bool isSelected = false;
    public bool isInCooldown = false;
    public GameObject statsObject;

    public bool isShowingStats = false;

    private bool _isShopItem = false;
    private bool _isInBattlePreparation = false; // Flag to know if this troop card is in battle preparation
    private bool _isSelectedInBattlePrep = false; // Flag to track if selected in battle preparation
    private BattlePreparationUI _battlePrepUI; // Cached reference to BattlePreparationUI

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find and cache BattlePreparationUI reference
        _battlePrepUI = LevelManager.Instance.battlePreparationUI;
        print("BATTLE PREPARATION UI = " + _battlePrepUI);

        if (!_isShopItem)
        {
            print("Listener Is Added");
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

    private void Update()
    {
        // print("_isInBattlePreparation = " + _isInBattlePreparation);
        // print("_battlePrepUI = " + _battlePrepUI);
        // print("_isSelectedInBattlePrep = " + _isSelectedInBattlePrep);
    }

    public void SetTroop(TroopSO troop, int level)
    {
        troopSO = troop;
        currentLevel = level;
        InitTroop();
        UpdateDisabledImage(); // Update the disabled image when troop is set
    }

    public void SetShopItem(TroopSO troop, int level)
    {
        troopSO = troop;
        currentLevel = level;
        _isShopItem = true;
        levelText.gameObject.SetActive(false);
        InitTroop();
        UpdateDisabledImage(); // Update the disabled image when troop is set
    }

    public void SetInBattlePreparation(bool inBattlePrep)
    {
        _isInBattlePreparation = inBattlePrep;
        UpdateDisabledImage();
    }

    // Method to update the disabled image based on context
    public void UpdateDisabledImage()
    {
        if (_isShopItem)
        {
            disabledImage.gameObject.SetActive(false);
        }
        else if (troopSO != null && disabledImage != null)
        {
            if (!_isInBattlePreparation)
            {
                // During normal game, show disabled image if not enough souls
                float currentSouls = SoulSystem.SoulManager.Instance.GetSouls();
                bool hasEnoughSouls = currentSouls >= troopSO.soulCost;
                disabledImage.gameObject.SetActive(!hasEnoughSouls);
            }
            else
            {
                // During battle preparation, show disabled image if troop is selected
                disabledImage.gameObject.SetActive(_isSelectedInBattlePrep);
            }
        }
    }

    void SelectTroop()
    {
        print("Select Troop Button Called");

        // Handle different behavior based on context
        if (_isInBattlePreparation && !_isSelectedInBattlePrep)
        {
            print("CALL THIS 1");
            // If in battle preparation and not selected, and selection limit not reached
            if (_battlePrepUI != null && !_battlePrepUI.IsSelectionLimitReached())
            {
                print("CALL THIS 2");

                // Select the troop for battle
                _battlePrepUI.SelectTroop(this);
                _isSelectedInBattlePrep = true;

                // Turn on the disabled image
                // if (disabledImage != null)
                // {
                //     disabledImage.gameObject.SetActive(true);
                // }
            }
        }
        else if (_isInBattlePreparation && _isSelectedInBattlePrep)
        {
            print("CALL THIS 3");

            // If in battle preparation and already selected, deselect it
            if (_battlePrepUI != null)
            {
                print("CALL THIS 4");

                _battlePrepUI.DeselectTroop(this);
                _isSelectedInBattlePrep = false;

                // Turn off the disabled image
                // if (disabledImage != null)
                // {
                //     disabledImage.gameObject.SetActive(false);
                // }
            }
        }
        else
        {
            // Normal behavior during actual battle
            print("Select Troop Called");
            TroopManager.Instance.SetCurrentTroop(this);
        }
    }

    void InitTroop()
    {
        troopPotrait.sprite = troopSO.potrait;
        troopClass.sprite = TroopClassSpriteManager.Instance.GetSpriteForClass(troopSO.troopClass);
        troopCostText.text = troopSO.soulCost.ToString();
        attactText.text = troopSO.GetDamageAtLevel(currentLevel).ToString("F0"); // Format to integer
        healthText.text = troopSO.GetHealthAtLevel(currentLevel).ToString("F0"); // Format to integer
        defenseText.text = troopSO.GetDefenseAtLevel(currentLevel).ToString("F0"); // Format to integer
        speedText.text = troopSO.speedCategory;
        levelText.text = "LVL " + currentLevel;
        troopName.text = troopSO.name;
        troopDescription.text = troopSO.description.ToUpper();
        troopAttackSpeed.text = troopSO.attackCooldown + "s";
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
            statsObject.GetComponent<SlideInAnimator>().ShowSlideIn();
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

    // Method to be called when battle starts
    public void OnBattleStart()
    {
        // Reset battle preparation state
        _isInBattlePreparation = false;
        _isSelectedInBattlePrep = false;

        // Update disabled image based on soul availability during battle
        UpdateDisabledImage();
    }
}