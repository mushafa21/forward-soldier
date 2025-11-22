using System;
using UnityEngine;

namespace TroopSystem
{
    public class Ghost : MonoBehaviour
    {
        [Header("Ghost Settings")]
        public float floatSpeed = 1.0f;      // Speed at which the ghost moves upward
        public float floatHeight = 2.0f;     // Maximum height the ghost will float up to
        public float destroyDelay = 3.0f;    // Time before the ghost is destroyed
        public Material enemyGhostMaterial;
        private Vector3 startPosition;       // Starting position of the ghost
        private float spawnTime;          
        private SpriteRenderer _spriteRenderer; // Time when the ghost was spawned

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            startPosition = transform.position;
            spawnTime = Time.time;
            
        }

        public void SetFaction(TroopFaction faction)
        {
            if (faction == TroopFaction.Enemy)
            {
                _spriteRenderer.material = new Material(enemyGhostMaterial);
            }
        }

        void Update()
        {
            // // Move the ghost upward over time
            float elapsed = Time.time - spawnTime;
            float progress = elapsed * floatSpeed / floatHeight; // Calculate progress
            
            // Update position to float upward
            Vector3 newPosition = startPosition;
            newPosition.y += Mathf.Min(elapsed * floatSpeed, floatHeight); // Move up but cap at max height
            transform.position = newPosition;
            
            // Destroy the ghost after the specified delay
            if (elapsed >= destroyDelay)
            {
                Destroy(gameObject);
            }
        }
    }
}