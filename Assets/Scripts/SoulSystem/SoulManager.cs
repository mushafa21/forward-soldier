using System;
using UnityEngine;

namespace SoulSystem
{
    public class SoulManager : MonoBehaviour
    {
        [Header("Soul Generation Settings")]
        public float soulsPerInterval = 1f;  // Amount of souls to add each interval
        public float intervalTime = 5f;      // Time in seconds between each soul addition

        private float souls = 0f;
        public float maxSouls = 100f;
        private float timeSinceLastAddition = 0f;


        private static SoulManager instance;

        public static SoulManager Instance
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

        public void ResetSouls()
        {
            souls = 0;
            timeSinceLastAddition = 0f;  // Reset the timer when souls are reset
            UIManager.Instance.UpdateSoulText(souls,maxSouls);
            UIManager.Instance.UpdateIntervalProgress(timeSinceLastAddition, intervalTime);
            UpdateTroopCardUI(); // Update all troop cards when souls are reset

        }

        private void Start()
        {
            ResetSouls();
            UIManager.Instance.UpdateSoulText(souls,maxSouls);
            UIManager.Instance.UpdateIntervalProgress(timeSinceLastAddition, intervalTime);
        }

        void Update()
        {
            // Only run soul generation during actual battle, not during battle preparation
            if (IsBattleStarted())
            {
                timeSinceLastAddition += Time.deltaTime;

                // Update the interval progress bar
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.UpdateIntervalProgress(timeSinceLastAddition, intervalTime);
                }

                if (timeSinceLastAddition >= intervalTime)
                {
                    IncreaseSouls(soulsPerInterval);
                }
            }
        }


        public void IncreaseSouls(float amount)
        {
            if (souls < maxSouls)
            {
                souls = Math.Min(souls + amount, maxSouls);
            }

            timeSinceLastAddition = 0f;  // Reset the timer
            UIManager.Instance.UpdateSoulText(souls,maxSouls);
            UIManager.Instance.UpdateIntervalProgress(timeSinceLastAddition, intervalTime); // Update progress bar after adding souls
            UpdateTroopCardUI(); // Update all troop cards after increasing souls
        }


        public void DecreaseSouls(float amount)
        {
            souls -= amount;
            UIManager.Instance.UpdateSoulText(souls,maxSouls);
            UpdateTroopCardUI(); // Update all troop cards after decreasing souls
        }

        // Public method to get the current soul count
        public float GetSouls()
        {
            return souls;
        }

        // Public method to set the soul count
        public void SetSouls(float amount)
        {
            souls = amount;
            UpdateTroopCardUI(); // Update all troop cards when souls are set directly
        }

        // Public method to get the current interval progress
        public float GetIntervalProgress()
        {
            return timeSinceLastAddition;
        }

        // Public method to get the interval time
        public float GetIntervalTime()
        {
            return intervalTime;
        }

        // Method to check if battle has started
        private bool IsBattleStarted()
        {
            // Check if LevelManager indicates battle has started
            if (LevelManager.Instance != null)
            {
                // We need to check if we can access the battle state
                // Since LevelManager has an _isBattleStarted field, we need to make it accessible
                // For now, we assume battle has started if there's no BattlePreparationUI active
                // or we could check if LevelManager's battle preparation sequence has completed

                // Alternative approach: If BattlePreparationUI exists and is active, battle hasn't started
                if (LevelManager.Instance.currentState == LevelState.preparation)
                {
                    return false; // Battle not started yet
                }
            }
            return true; // Assume battle started if no battle prep UI is active
        }

        // Method to update all troop card UIs
        private void UpdateTroopCardUI()
        {
            // Find all TroopCardUI objects and update their disabled images
            TroopCardUI[] troopCards = FindObjectsOfType<TroopCardUI>();
            foreach (TroopCardUI card in troopCards)
            {
                card.UpdateDisabledImage();
            }
        }
    }
}