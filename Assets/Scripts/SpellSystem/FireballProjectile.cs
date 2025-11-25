using UnityEngine;

namespace SpellSystem
{
    public class FireballProjectile : MonoBehaviour
    {
        [Header("Fireball Settings")]
        public float speed = 10f;
        public float damage = 50f;
        public float radius = 2f;          // AOE radius
        public GameObject explosionEffect; // Optional explosion effect prefab
        
        private Transform target;
        private Vector3 targetPosition;
        private bool hasTarget = false;

        public void SetTarget(Transform targetTransform)
        {
            target = targetTransform;
            hasTarget = true;
        }
        
        public void SetTargetPosition(Vector3 pos)
        {
            targetPosition = pos;
            hasTarget = true;
        }

        void Update()
        {
            if (!hasTarget) return;

            Vector3 moveDirection;
            
            if (target != null)
            {
                // Move towards a moving target
                moveDirection = (target.position - transform.position).normalized;
            }
            else
            {
                // Move towards a fixed position
                moveDirection = (targetPosition - transform.position).normalized;
            }

            transform.position += moveDirection * speed * Time.deltaTime;
            
            // Rotate to face direction of movement
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Check if we've reached the target position
            if (Vector3.Distance(transform.position, 
                target != null ? target.position : targetPosition) < 0.5f)
            {
                Explode();
            }
        }

        void Explode()
        {
            // Apply AOE damage at the explosion position
            ApplyAOEDamage(transform.position, radius, damage);
            
            // Spawn explosion effect if available
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }
            
            // Destroy the fireball
            Destroy(gameObject);
        }

        void ApplyAOEDamage(Vector3 center, float radius, float damage)
        {
            // Find all enemy troops in the AOE radius
            var allTroops = TroopManager.Instance.GetAllTroops();
            foreach (var troop in allTroops)
            {
                if (troop != null && troop.currentHealth > 0 && troop.faction == TroopSystem.TroopFaction.Enemy)
                {
                    float distance = Vector3.Distance(center, troop.transform.position);
                    if (distance <= radius)
                    {
                        troop.TakeDamage(damage, true); // Magic attack ignores defense
                    }
                }
            }
        }
    }
}