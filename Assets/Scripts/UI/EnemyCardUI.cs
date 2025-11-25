using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class EnemyCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image enemyPortrait;
    public Image enemyClass;
    public TextMeshProUGUI enemyName;
    public TroopSO enemySO;  // Using TroopSO since there's no EnemySO defined
    public TextMeshProUGUI descriptionText, healthText, attackText, defenseText, speedText, attackSpeedText;
    public GameObject statsObject;

    public bool isShowingStats = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemySO != null)
        {
            InitEnemy();
        }
    }

    public void SetEnemy(TroopSO enemy)
    {
        enemySO = enemy;
        InitEnemy();
    }

    void InitEnemy()
    {
        if (enemySO != null)
        {
            // Using TroopSO properties for enemy card
            enemyPortrait.sprite = enemySO.potrait;  // or portrait, depending on the field name
            enemyClass.sprite = TroopClassSpriteManager.Instance.GetSpriteForClass(enemySO.troopClass); // Assuming same class system
            attackText.text = enemySO.GetDamageAtLevel(1).ToString("F0"); // Using level 1 for enemy stats display
            healthText.text = enemySO.GetHealthAtLevel(1).ToString("F0"); // Using level 1 for enemy stats display
            defenseText.text = enemySO.GetDefenseAtLevel(1).ToString("F0"); // Using level 1 for enemy defense display
            speedText.text = enemySO.speedCategory;
            attackSpeedText.text = enemySO.attackCooldown + "s"; // Add attack speed
            descriptionText.text = enemySO.description;
            enemyName.text = enemySO.name;
        }
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
}