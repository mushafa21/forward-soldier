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

        private void Start()
        {
            UIManager.Instance.UpdateSoulText(souls);
        }

        void Update()
        {
            timeSinceLastAddition += Time.deltaTime;

            if (timeSinceLastAddition >= intervalTime)
            {
                IncreaseSouls();
            }
        }


        void IncreaseSouls()
        {
            souls += soulsPerInterval;
            timeSinceLastAddition = 0f;  // Reset the timer
            UIManager.Instance.UpdateSoulText(souls);
        }


        public void DecreaseSouls(float amount)
        {
            souls -= amount;
            UIManager.Instance.UpdateSoulText(souls);
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