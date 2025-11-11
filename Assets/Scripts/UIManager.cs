using System;
using MoreMountains.Tools;
using TMPro;
using TowerSystem;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public TextMeshProUGUI soulText;
    public MMProgressBar soulBar;

    
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateSoulText(float amount, float maxAmount)
    {
        soulText.text = amount + "/" + maxAmount;
        soulBar.UpdateBar(amount,0,maxAmount);
    }
}