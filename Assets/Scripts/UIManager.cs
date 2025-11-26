using System;
using MoreMountains.Tools;
using TMPro;
using TowerSystem;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI soulText;
    public MMProgressBar soulBar;
    public Slider intervalSlider; // Slider for soul generation interval
    public GameObject hud;
    public GameObject troopListContainer;
    public GameObject soulContainer;
    public GameObject levelStageContainer;

    private static UIManager instance;

    public static UIManager Instance
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

    public void UpdateSoulText(float amount, float maxAmount)
    {
        soulText.text = amount + "/" + maxAmount;
        soulBar.UpdateBar(amount, 0, maxAmount);
    }

    // Method to update the interval progress slider
    public void UpdateIntervalProgress(float current, float max)
    {
        if (intervalSlider != null)
        {
            // Calculate the normalized value (0 to 1) for the slider
            float normalizedValue = max > 0 ? Mathf.Clamp01(current / max) : 0;
            intervalSlider.value = normalizedValue;
        }
    }

    public void HideHUD()
    {
        hud.SetActive(false);
    }

    public void ShowHUD()
    {
        hud.SetActive(true);
        troopListContainer.GetComponent<SlideInAnimator>().ShowSlideIn();
        soulContainer.GetComponent<SlideInAnimator>().ShowSlideIn();
        levelStageContainer.GetComponent<SlideInAnimator>().ShowSlideIn();
        
    }
}