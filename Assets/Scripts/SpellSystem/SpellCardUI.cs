using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TroopSystem;

namespace SpellSystem
{
    public class SpellCardUI : MonoBehaviour
    {
        [Header("References")]
        public Button button;
        public Image spellIcon;
        public TextMeshProUGUI spellCostText;
        public TextMeshProUGUI coolDownText;
        public Image coolDownImage;
        public SpellSO spellSO;
        public GameObject selectedIndicator;

        public bool isInCooldown = false;

        void Start()
        {
            button.onClick.AddListener(UseSpell);
            InitSpell();
        }

        public void SetSpell(SpellSO spell)
        {
            spellSO = spell;
            InitSpell();
        }

        void InitSpell()
        {
            if (spellSO != null && spellIcon != null)
            {
                spellIcon.sprite = spellSO.icon;
                spellCostText.text = spellSO.spellCost.ToString();
            }
        }

        void UseSpell()
        {
            if (isInCooldown || spellSO == null) return;

            // Check if player has enough souls/spell points
            if (SoulSystem.SoulManager.Instance.GetSouls() < spellSO.spellCost)
            {
                Debug.Log("Not enough souls to use this spell!");
                return;
            }

            // Use the spell and start cooldown
            SoulSystem.SoulManager.Instance.DecreaseSouls(spellSO.spellCost);
            ExecuteSpellEffect();
            StartCooldown();
        }

        void ExecuteSpellEffect()
        {
            switch (spellSO.spellType)
            {
                case SpellType.Fireball:
                    CastFireball();
                    break;
                case SpellType.Shield:
                    CastShield();
                    break;
                case SpellType.AttackBoost:
                    CastAttackBoost();
                    break;
            }
        }

        void CastFireball()
        {
            // Find the nearest enemy troop to target
            Troop nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                // In a real implementation, you'd spawn the fireball projectile
                // For now, we'll create it at the player's tower position (or another starting point)
                // and target the nearest enemy
                Vector3 startPosition = GetPlayerTowerPosition();
                SpawnFireball(startPosition, nearestEnemy.transform);
            }
            else
            {
                // If no specific enemy found, target the enemy tower
                Vector3 targetPosition = GetEnemyTowerPosition();
                if (targetPosition != Vector3.zero) // If enemy tower exists
                {
                    Vector3 startPosition = GetPlayerTowerPosition();
                    SpawnFireballAtPosition(startPosition, targetPosition);
                }
                else
                {
                    Debug.Log("No enemy found to target with fireball!");
                    // As fallback, apply immediate AOE at a fixed position or just return
                    return;
                }
            }
        }

        Vector3 GetPlayerTowerPosition()
        {
            // Try to get the player tower position as a starting point
            if (LevelManager.Instance != null && LevelManager.Instance.playerTower != null)
            {
                return LevelManager.Instance.playerTower.transform.position;
            }
            // Fallback to a default position
            return Vector3.zero;
        }

        Vector3 GetEnemyTowerPosition()
        {
            // Try to get the enemy tower position as a potential target
            if (LevelManager.Instance != null && LevelManager.Instance.enemyTower != null)
            {
                return LevelManager.Instance.enemyTower.transform.position;
            }
            // Return zero if no enemy tower
            return Vector3.zero;
        }

        void SpawnFireball(Vector3 startPosition, Transform target)
        {
            // Create the fireball at start position
            GameObject fireballObj = new GameObject("Fireball");
            fireballObj.transform.position = startPosition;
            
            // Add sprite/renderer if needed for visualization
            var spriteRenderer = fireballObj.AddComponent<SpriteRenderer>();
            // You would assign the fireball sprite here if available
            // spriteRenderer.sprite = fireballSprite; // If you have one
            
            // Add the FireballProjectile component
            FireballProjectile fireball = fireballObj.AddComponent<FireballProjectile>();
            fireball.speed = 15f; // Default speed, could be from spellSO
            fireball.damage = spellSO.damage;
            fireball.radius = spellSO.radius;
            
            // Target the enemy troop
            fireball.SetTarget(target);
        }

        void SpawnFireballAtPosition(Vector3 startPosition, Vector3 targetPosition)
        {
            // Create the fireball at start position
            GameObject fireballObj = new GameObject("Fireball");
            fireballObj.transform.position = startPosition;
            
            // Add sprite/renderer if needed for visualization
            var spriteRenderer = fireballObj.AddComponent<SpriteRenderer>();
            // You would assign the fireball sprite here if available
            
            // Add the FireballProjectile component
            FireballProjectile fireball = fireballObj.AddComponent<FireballProjectile>();
            fireball.speed = 15f; // Default speed, could be from spellSO
            fireball.damage = spellSO.damage;
            fireball.radius = spellSO.radius;
            
            // Target the position
            fireball.SetTargetPosition(targetPosition);
        }

        void CastShield()
        {
            // Apply shield effect to all player troops
            List<Troop> playerTroops = TroopManager.Instance.GetAllTroops();
            foreach (Troop troop in playerTroops)
            {
                if (troop != null && troop.faction == TroopFaction.Player)
                {
                    // This would require a shield component on troops
                    // For now, we'll implement a temporary damage reduction using a coroutine
                    StartCoroutine(ApplyShieldToTroop(troop, spellSO.duration));
                }
            }
        }

        void CastAttackBoost()
        {
            // Increase attack of all player troops for a duration
            List<Troop> playerTroops = TroopManager.Instance.GetAllTroops();
            foreach (Troop troop in playerTroops)
            {
                if (troop != null && troop.faction == TroopFaction.Player)
                {
                    StartCoroutine(ApplyAttackBoostToTroop(troop, spellSO.duration));
                }
            }
        }

        Troop FindNearestEnemy()
        {
            // Since this is a UI element, we can't easily determine the "nearest" enemy in the game world
            // Instead, we'll find the first active enemy troop available
            // In a real implementation, you might want to target specific lanes or use other targeting methods
            List<Troop> allTroops = TroopManager.Instance.GetAllTroops();
            
            // We'll just return the first enemy troop we find that's alive
            foreach (Troop troop in allTroops)
            {
                if (troop != null && troop.currentHealth > 0 && troop.faction == TroopFaction.Enemy)
                {
                    return troop;
                }
            }
            
            return null;
        }

        void ApplyAOEDamage(Vector3 center, float radius, float damage)
        {
            // Find all enemy troops in the AOE radius
            List<Troop> allTroops = TroopManager.Instance.GetAllTroops();
            foreach (Troop troop in allTroops)
            {
                if (troop != null && troop.currentHealth > 0 && troop.faction == TroopFaction.Enemy)
                {
                    float distance = Vector3.Distance(center, troop.transform.position);
                    if (distance <= radius)
                    {
                        troop.TakeDamage(damage, true); // Magic attack ignores defense
                    }
                }
            }
        }

        IEnumerator ApplyShieldToTroop(Troop troop, float duration)
        {
            // This would normally involve adding temporary shield properties to the troop
            // For now, we'll implement temporary invulnerability by ignoring damage temporarily
            // This would require changes to TakeDamage method, which we'll simulate by tracking state
            yield return new WaitForSeconds(duration);
        }

        IEnumerator ApplyAttackBoostToTroop(Troop troop, float duration)
        {
            if (troop.troopStats != null)
            {
                // Temporarily increase damage
                float originalDamage = troop.damage;
                float boostedDamage = originalDamage * (1 + spellSO.bonusPercent / 100f);
                troop.damage = boostedDamage;

                yield return new WaitForSeconds(duration);

                // Restore original damage after duration
                troop.damage = originalDamage;
            }
        }

        public void StartCooldown()
        {
            if (spellSO == null) return;

            isInCooldown = true;
            coolDownImage.fillAmount = 1f; // Start with full fill
            coolDownImage.gameObject.SetActive(true);

            // Start the cooldown coroutine
            StartCoroutine(CooldownRoutine(spellSO.cooldown));
        }

        private IEnumerator CooldownRoutine(float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = 1f - (elapsed / duration); // Decreases from 1 to 0
                coolDownImage.fillAmount = progress;

                // Update cooldown text
                int secondsRemaining = Mathf.CeilToInt(duration - elapsed);
                coolDownText.text = secondsRemaining.ToString();
                coolDownText.gameObject.SetActive(true);

                yield return null;
            }

            // Cooldown finished
            isInCooldown = false;
            coolDownImage.gameObject.SetActive(false);
            coolDownText.gameObject.SetActive(false);
        }

        void Update()
        {
            // Check for space key to use the spell
            if (Input.GetKeyDown(KeyCode.Space) && !isInCooldown && spellSO != null)
            {
                UseSpell();
            }
        }
    }
}