using TowerSystem;
using UnityEngine;

namespace TroopSystem
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public float speed = 10f;           // Speed at which the projectile moves
        public float lifetime = 5f;         // How long the projectile exists before being destroyed
        public float damage = 20f;          // Damage dealt to target
        
        private Transform target;          // The target troop to move towards
        private Troop sourceTroop;         // The troop that fired this projectile
        private float currentLifetime;     // Current time remaining before destruction

        public void Initialize(Transform targetTroop, Troop source, float projDamage, float projSpeed)
        {
            target = targetTroop;
            sourceTroop = source;
            damage = projDamage;
            speed = projSpeed;
            currentLifetime = lifetime;
        }

        void Update()
        {
            // Update lifetime
            currentLifetime -= Time.deltaTime;
            
            // Destroy projectile if lifetime expires
            if (currentLifetime <= 0)
            {
                Destroy(gameObject);
                return;
            }

            // If target is destroyed or null, move in the last known direction for a short time before destroying
            if (target == null)
            {
                // Move straight for remaining lifetime (or a short period before destroying)
                transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
                return;
            }

            // Move towards the target
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            // Rotate to face the direction of movement
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            // Check if we've reached the target (within a small distance threshold)
            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                // Apply damage to the target troop
                Troop targetTroop = target.GetComponent<Troop>();
                Tower tower = target.GetComponent<Tower>();
                if (targetTroop != null)
                {
                    // Check that target troop is still alive
                    if (targetTroop.currentHealth > 0)
                    {
                        targetTroop.TakeDamage(damage);
                    }
                } else if (tower != null)
                {
                    if (tower.currentHealth > 0)
                    {
                        tower.TakeDamage(damage);
                    }
                }
                
                // Destroy the projectile after hitting the target
                Destroy(gameObject);
            }
        }
    }
}