using System.Collections.Generic;
using UnityEngine;
using PathSystem;
using TroopSystem;

namespace TowerSystem
{
    public enum TowerType
    {
        Player,
        Enemy
    }

    public class Tower : MonoBehaviour
    {
        [Header("Tower Configuration")]
        public TowerType towerType = TowerType.Player;
        public LanePath associatedPath; // Path this tower is associated with
        public TroopSO troopStats; // Stats to use for attacks
        
        [Header("Attack Settings")]
        public float attackRange = 3f; // Override from troopStats if provided
        public float attackCooldown = 1f; // Override from troopStats if provided
        public float damage = 20f; // Override from troopStats if provided
        
        [Header("Targeting")]
        public LayerMask targetLayerMask = -1; // Layers to consider as targets

        private float lastAttackTime = 0f;
        private Troop currentTarget = null;
        private TroopManager troopManager;
        
        [Header("Tower Health")]
        public float maxHealth = 1000f;
        public float currentHealth;

        void Start()
        {
            currentHealth = maxHealth;
            
            // Initialize the troop manager reference
            troopManager = TroopManager.Instance;
            
            // Use stats from TroopSO if available
            if (troopStats != null)
            {
                attackRange = troopStats.attackRange;
                attackCooldown = troopStats.attackCooldown;
                damage = troopStats.damage;
            }
        }

        void Update()
        {
            HandleTargetingAndAttack();
        }

        // Handle targeting and attack logic
        void HandleTargetingAndAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return; // Still on cooldown

            // Find a target to attack
            Troop target = FindTarget();
            
            if (target != null)
            {
                // Attack the target
                AttackTarget(target);
                lastAttackTime = Time.time;
            }
        }

        // Find a target within attack range
        Troop FindTarget()
        {
            // Get all troops in the scene
            List<Troop> allTroops = troopManager.GetAllTroops();
            
            Troop closestTarget = null;
            float closestDistance = float.MaxValue;
            
            foreach (Troop troop in allTroops)
            {
                // Skip dead troops
                if (troop == null || !troop.GetComponent<Collider>() || troop.currentPath != associatedPath || !troop.gameObject.activeInHierarchy)
                    continue;

                // Check if the troop is from the opposite faction
                if (IsOppositeFaction(troop))
                {
                    // Check if troop is within attack range
                    float distance = Vector3.Distance(transform.position, troop.transform.position);
                    
                    if (distance <= attackRange && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = troop;
                    }
                }
            }
            
            return closestTarget;
        }

        // Attack the target
        void AttackTarget(Troop target)
        {
            if (target != null)
            {
                // Apply damage to the target
                target.TakeDamage(damage);
                
                Debug.Log($"Tower dealt {damage} damage to {target.name}");
            }
        }

        // Check if the troop is from the opposite faction
        bool IsOppositeFaction(Troop troop)
        {
            if (troop == null) return false;
            
            switch (towerType)
            {
                case TowerType.Player:
                    return troop.faction == TroopFaction.Enemy;
                case TowerType.Enemy:
                    return troop.faction == TroopFaction.Player;
                default:
                    return false;
            }
        }

        // Take damage from attacks
        public void TakeDamage(float damageAmount)
        {
            currentHealth -= damageAmount;
            
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnTowerDestroyed();
            }
        }

        // Called when the tower is destroyed
        void OnTowerDestroyed()
        {
            Debug.Log($"{towerType} tower has been destroyed!");
            // You can implement tower destruction logic here
            // For example, notify the game manager, play effects, etc.
        }

        // Visualize the attack range in the editor
        private void OnDrawGizmosSelected()
        {
            if (attackRange > 0)
            {
                // Draw attack range gizmo
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
        }

        // Get the tower's current health as a percentage
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0;
        }
    }
}