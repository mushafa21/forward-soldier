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
        
        [Header("Tower Health")]
        public float maxHealth = 1000f;
        public float currentHealth;
        
        [Header("Visual")]
        public TextMeshProUGUI healthText;
        public MMProgressBar healthBar;
        public Renderer towerRenderer;
        public Color playerColor = Color.blue;
        public Color enemyColor = Color.red;
        public SpriteRenderer mageSprite;
        public Material factionMaterial; // Assign your 'Troop_ColorSwap_Material' here
        public ParticleSystem hitParticle;
        public ParticleSystem destroyParticle;
        void Start()
        {
            // Scale maxHealth based on tower level from GameManager
            if (faction == TowerFaction.Player && GameManager.Instance != null)
            {
                int towerLevel = GameManager.Instance.GetTowerLevel();
                float healthIncreasePercentage = 0.2f; // 20% increase per level (same as in ShopUI)
                float baseHealth = 200; // Base tower health
                
                // Calculate new max health using the same formula as in ShopUI
                maxHealth = baseHealth * Mathf.Pow(1f + healthIncreasePercentage, towerLevel - 1);
            }
            
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
            hitParticle.Play();
        }

        // Called when the tower is destroyed
        void OnTowerDestroyed()
        {
            destroyParticle.Play();
            if (faction == TowerFaction.Player)
            {
                LevelManager.Instance.StartGameOverSequence();

            } else if (faction == TowerFaction.Enemy)
            {
                LevelManager.Instance.StartVictorySequence();

            }
            Debug.Log($"{faction} tower has been destroyed!");
            // In a real game, you might want to trigger game over conditions
            // or other tower destruction logic here
        }

        // Set the color based on faction
        void SetFactionColor()
        {
            if (mageSprite == null || factionMaterial == null)
            {
                if (mageSprite == null) Debug.LogWarning($"SpriteRenderer not assigned on {gameObject.name}");
                if (factionMaterial == null) Debug.LogWarning($"FactionMaterial not assigned on {gameObject.name}");
                return;
            }

            // Create a new instance of the material for this troop
            // This is CRUCIAL so that changing one troop's color
            // doesn't change all troops using the same material.
            Material materialInstance = new Material(factionMaterial);
            
            // Set the sprite renderer to use this new material instance
            mageSprite.material = materialInstance;

            // Set the '_TargetColor' property on the shader
            if (faction == TowerFaction.Enemy)
            {
                materialInstance.SetColor("_TargetColor", enemyColor);
                mageSprite.gameObject.transform.localScale = new Vector3(mageSprite.gameObject.transform.localScale.x * -1, mageSprite.gameObject.transform.localScale.y, mageSprite.gameObject.transform.localScale.z);
                towerRenderer.gameObject.transform.localScale = new Vector3(towerRenderer.gameObject.transform.localScale.x * -1, towerRenderer.gameObject.transform.localScale.y, towerRenderer.gameObject.transform.localScale.z);

            }
            else // Player
            {
                // For the player, we set the target color to be the same
                // as the source color (which is set on the material asset).
                // Or, you can set it to a specific playerColor.
                // This ensures Player troops stay blue.
                materialInstance.SetColor("_TargetColor", playerColor);
            }
        }
        
 


        // Get the tower's health percentage
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0;
        }

        public void UpdateUI()
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