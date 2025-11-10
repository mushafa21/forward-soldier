


using System;
using TowerSystem;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
 
    private static LevelManager instance;

    public static LevelManager Instance
    {
        get
        {
            return instance;
        }
    }
    
    public Tower playerTower;
    public Tower enemyTower;

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
}