using System;
using TMPro;
using TowerSystem;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public TextMeshProUGUI soulText;

    
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

    public void UpdateSoulText(float amount)
    {
        soulText.text = amount.ToString();
    }
}