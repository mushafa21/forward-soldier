using UnityEngine;

namespace TroopSystem
{
    public class Ghost : MonoBehaviour
    {
        [Header("Ghost Settings")]
        public float floatSpeed = 1.0f;      // Speed at which the ghost moves upward
        public float floatHeight = 2.0f;     // Maximum height the ghost will float up to
        public float destroyDelay = 3.0f;    // Time before the ghost is destroyed

        private Vector3 startPosition;       // Starting position of the ghost
        private float spawnTime;             // Time when the ghost was spawned

        void Start()
        {
            startPosition = transform.position;
            spawnTime = Time.time;
            
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