using MoreMountains.Tools;
using UnityEngine;
using PathSystem;
using TMPro;
using TroopSystem;
using DG.Tweening;

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

        public float defense = 10f;
        public float currentHealth;

        [Header("Tower Combat")]
        public float attackDamage = 50f;
        public float attackRange = 5f;
        public float attackCooldown = 2f;
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;
        private float lastAttackTime = 0f;
        private TroopSystem.Troop targetTroop = null;
        
        [Header("Visual")]
        public TextMeshProUGUI healthText;
        public MMProgressBar healthBar;
        public Renderer towerRenderer;
        public Color playerColor = Color.blue;
        public Color enemyColor = Color.red;
        public SpriteRenderer mageSprite;
        public Material factionMaterial; // Assign your 'Troop_ColorSwap_Material' here
        public Material flagMaterial; // Assign your 'Troop_ColorSwap_Material' here
        public SpriteRenderer flagSprite1;
        public SpriteRenderer flagSprite2;

        public ParticleSystem hitParticle;
        public ParticleSystem destroyParticle;

        private Vector3 originalScale; // Store the original scale to ensure proper reset after animations

        void Start()
        {
            currentHealth = maxHealth;
            SetFactionColor();
            UpdateUI();

            // Store the original scale for use in animations
            originalScale = transform.localScale;
        }
        

        // Take damage from attacks
        public void TakeDamage(float damage, bool isMagicAttack = false)
        {
            float damageToApply = damage;
            if (!isMagicAttack)
            {
                // Apply defense reduction only for non-magic attacks
                damageToApply = damage - defense;
            }

            currentHealth -= damageToApply;
            UpdateUI();
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnTowerDestroyed();
            }
            hitParticle.Play();

            // Apply squash and stretch effect when taking damage
            ApplySquashAndStretch();
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
                flagSprite1.material = new Material(flagMaterial);
                flagSprite2.material = new Material(flagMaterial);
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

        // Find the closest enemy troop in range
        private TroopSystem.Troop FindClosestEnemyTroop()
        {
            // Get all active troops from the manager
            System.Collections.Generic.List<TroopSystem.Troop> allTroops = TroopManager.Instance.GetAllTroops();

            TroopSystem.Troop closestTroop = null;
            float closestDistance = float.MaxValue;

            foreach (TroopSystem.Troop troop in allTroops)
            {
                // Check if the troop is active, not dead, and of opposite faction
                if (troop != null && troop.currentHealth > 0 && IsOppositeFactionTroop(troop))
                {
                    float distance = Vector3.Distance(transform.position, troop.transform.position);
                    if (distance < attackRange && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTroop = troop;
                    }
                }
            }

            return closestTroop;
        }

        // Check if the target troop is of opposite faction
        bool IsOppositeFactionTroop(TroopSystem.Troop otherTroop)
        {
            if (otherTroop == null) return false;

            switch (faction)
            {
                case TowerFaction.Player:
                    return otherTroop.faction == TroopFaction.Enemy;
                case TowerFaction.Enemy:
                    return otherTroop.faction == TroopFaction.Player;
                default:
                    return false;
            }
        }

        // Spawn projectile towards a target
        private void SpawnProjectile(Transform target)
        {
            if (projectilePrefab != null)
            {
                // Spawn the projectile at the current tower's position
                GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

                // Get the projectile component and initialize it
                TroopSystem.Projectile projectile = projectileObj.GetComponent<TroopSystem.Projectile>();
                if (projectile != null)
                {
                    // Initialize with target, source tower (as the source troop), damage and speed
                    projectile.Initialize(target, null, attackDamage, projectileSpeed);
                }
            }
        }

        void Update()
        {
            // Only attack if it's time to attack again
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                // Find a new target if we don't have one or if the current target is out of range/invalid
                if (targetTroop == null || targetTroop.currentHealth <= 0 || !targetTroop.isActiveAndEnabled ||
                    Vector3.Distance(transform.position, targetTroop.transform.position) > attackRange)
                {
                    targetTroop = FindClosestEnemyTroop();
                }

                // If we have a valid target, attack it
                if (targetTroop != null)
                {
                    // Check if the target is still valid
                    if (targetTroop.currentHealth > 0 && targetTroop.isActiveAndEnabled &&
                        Vector3.Distance(transform.position, targetTroop.transform.position) <= attackRange)
                    {
                        // Attack the target by spawning a projectile
                        SpawnProjectile(targetTroop.transform);
                        lastAttackTime = Time.time;
                    }
                    else
                    {
                        // Target is no longer valid, clear it
                        targetTroop = null;
                    }
                }
            }
        }

        // Apply squash and stretch effect when taking damage
        private void ApplySquashAndStretch()
        {
            // Stop any existing animations on this transform
            transform.DOKill();

            // Calculate a slight squash and stretch effect using the stored original scale
            Vector3 targetSquashScale = new Vector3(originalScale.x * 0.9f, originalScale.y * 1.1f, originalScale.z); // Squash horizontally, stretch vertically
            Vector3 targetStretchScale = new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z); // Stretch horizontally, squash vertically

            // Apply the effect with DOTween (squash first, then return to original)
            // First, do a quick squash
            transform.DOScale(targetSquashScale, 0.05f)
                .OnComplete(() =>
                {
                    // Then stretch and return to original
                    transform.DOScale(targetStretchScale, 0.05f)
                        .OnComplete(() =>
                        {
                            // Return to original scale
                            transform.DOScale(originalScale, 0.05f);
                        });
                });
        }
    }
}