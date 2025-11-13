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
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ResetSouls()
        {
            souls = 0;
            UIManager.Instance.UpdateSoulText(souls,maxSouls);

        }

        private void Start()
        {
            ResetSouls();
            UIManager.Instance.UpdateSoulText(souls,maxSouls);
        }

        void Update()
        {
            timeSinceLastAddition += Time.deltaTime;

            if (timeSinceLastAddition >= intervalTime)
            {
                IncreaseSouls(soulsPerInterval);
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
        }


        public void DecreaseSouls(float amount)
        {
            souls -= amount;
            UIManager.Instance.UpdateSoulText(souls,maxSouls);
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
        }
    }
}