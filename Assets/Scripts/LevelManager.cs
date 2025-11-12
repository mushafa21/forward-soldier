


using System;
using TowerSystem;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    
    
    public GameObject victoryScreen;
    public UIButton continueButton;
    public GameObject deathScreen;
    public Tower playerTower;
    public Tower enemyTower;
    
 
    private static LevelManager instance;

    public static LevelManager Instance
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

    private void OnEnable()
    {
        continueButton.onClick.AddListener(GoToShop);
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
    }

    private void GoToShop()
    {
        victoryScreen.SetActive(false);
        GameManager.Instance.OpenShop();
    }

    public void ShowVictoryScreen()
    {
        Time.timeScale = 0;
        victoryScreen.SetActive(true);
        GameManager.Instance.IncreaseGold(1000);
        
    }
}