using MoreMountains.Tools;
using UnityEngine;
using PathSystem;
using TMPro;
using TroopSystem;

namespace TowerSystem
{
    public enum TowerFaction
    {
        Player,
        Enemy
    }

    public class Tower : MonoBehaviour
    {
        [Header("Tower Settings")]
        public TowerFaction faction = TowerFaction.Player;
        public LanePath associatedPath;
        
        [Header("Tower Health")]
        public float maxHealth = 1000f;
        public float currentHealth;
        
        [Header("Visual")]
        public TextMeshProUGUI healthText;
        public MMProgressBar healthBar;
        public Renderer towerRenderer;
        public Color playerColor = Color.blue;
        public Color enemyColor = Color.red;

        void Start()
        {
            currentHealth = maxHealth;
            SetFactionColor();
            UpdateUI();
        }
        

        // Take damage from attacks
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            UpdateUI();
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnTowerDestroyed();
            }
        }

        // Called when the tower is destroyed
        void OnTowerDestroyed()
        {
            Debug.Log($"{faction} tower has been destroyed!");
            // In a real game, you might want to trigger game over conditions
            // or other tower destruction logic here
        }

        // Set the color based on faction
        void SetFactionColor()
        {
            if (towerRenderer != null)
            {
                towerRenderer.material.color = faction == TowerFaction.Player ? playerColor : enemyColor;
            }

            if (faction == TowerFaction.Enemy)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                healthText.gameObject.transform.localScale = new Vector3(-Mathf.Abs(healthText.gameObject.transform.localScale.x), healthText.gameObject.transform.localScale.y, healthText.gameObject.transform.localScale.z);
            }
        }

        // Get the tower's health percentage
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0;
        }

        void UpdateUI()
        {
            if (healthText != null)
            {
                healthText.text = $"{currentHealth} / {maxHealth}";
            }

            if (healthBar != null)
            {
                healthBar.UpdateBar(currentHealth,0,maxHealth);
            }
        }
    }
}