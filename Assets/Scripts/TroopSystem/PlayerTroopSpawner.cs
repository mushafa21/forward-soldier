using UnityEngine;
using PathSystem;
using UnityEngine.EventSystems;

namespace TroopSystem
{
    public class PlayerTroopSpawner : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Spawner Settings")]
        public LanePath spawnPath; // Path where troops will spawn and move along

        [Header("Spawn Settings")]
        public GameObject troopPrefab; // Prefab of the troop to spawn
        public float spawnCooldown = 1f; // Cooldown between spawns to prevent spam clicking
        public KeyCode activationKey = KeyCode.Space; // Optional keyboard key to trigger spawn

        [Header("Visual Feedback")]
        public bool changeColorOnHover = true; // Whether to change color when mouse hovers
        public Color normalColor = Color.white; // Normal color
        public Color hoverColor = Color.yellow; // Color when mouse hovers
        public float hoverScaleMultiplier = 1.1f; // Scale multiplier when hovered

        private float lastSpawnTime = 0f;
        private Collider2D spawnerCollider; // Collider to detect clicks
        private SpriteRenderer spriteRenderer; // Reference to sprite renderer for visual feedback
        private Vector3 originalScale; // Store original scale
        private Color originalColor = Color.white; // Store original colo

        void Start()
        {
            // If no path is assigned, try to find one automatically
            if (spawnPath == null)
            {
                spawnPath = FindObjectOfType<LanePath>();
            }

            // Ensure the spawner has a collider for click detection
            spawnerCollider = GetComponent<Collider2D>();
            spawnerCollider.isTrigger = true; // Ensure it's a trigger


            // Get references to visual components for feedback
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            originalScale = transform.localScale;
        }

        void Update()
        {
            // Allow spawning via keyboard key as well
            if (Input.GetKeyDown(activationKey))
            {
                TrySpawnTroop();
            }
        }

        // Public method to manually spawn a troop via UI click
        public void OnPointerClick(PointerEventData eventData)
        {
            TrySpawnTroop();
        }

        // Handle visual feedback when mouse enters the spawner area
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (changeColorOnHover && spriteRenderer != null)
            {
                spriteRenderer.color = hoverColor;
            }
            
            // Scale up slightly to provide visual feedback
            transform.localScale = originalScale * hoverScaleMultiplier;
        }

        // Handle visual feedback when mouse exits the spawner area
        public void OnPointerExit(PointerEventData eventData)
        {
            if (changeColorOnHover && spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            
            // Return to original scale
            transform.localScale = originalScale;
        }

        // Handle 2D physics-based clicks as well for game objects in the scene
        void OnMouseDown()
        {
            // Only trigger if we're not relying on UI system or if the click is valid
            if (spawnerCollider != null)
            {
                TrySpawnTroop();
            }
        }

        // Try to spawn a troop, respecting cooldown
        private void TrySpawnTroop()
        {
            if (Time.time - lastSpawnTime >= spawnCooldown)
            {
                SpawnTroop();
                lastSpawnTime = Time.time;
            }
            else
            {
                // Optional: provide feedback that cooldown is still active
                float remainingCooldown = spawnCooldown - (Time.time - lastSpawnTime);
                Debug.Log($"Spawner is on cooldown. Wait {remainingCooldown:F1} more seconds.");
            }
        }

        // Public method to manually spawn a troop
        public void SpawnTroop()
        {
            if (troopPrefab != null && spawnPath != null)
            {
                // Troop position will be set by the troop's faction-based logic
                GameObject newTroopObject = Instantiate(troopPrefab, Vector3.zero, Quaternion.identity);

                // Get the troop component and assign the path
                Troop newTroop = newTroopObject.GetComponent<Troop>();
                if (newTroop != null)
                {
                    newTroop.faction = TroopFaction.Player; // Set the faction
                    newTroop.SetPath(spawnPath);
                    print("Current Selected Troop = " + TroopManager.Instance.currentSelectedTroop);
                    if (TroopManager.Instance.currentSelectedTroop != null)
                    {
                        newTroop.SetTroopSO(TroopManager.Instance.currentSelectedTroop);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Missing troop prefab or spawn path!");
            }
        }

        // Visualize the spawn area in the editor
        void OnDrawGizmosSelected()
        {
            if (spawnerCollider != null)
            {
                Gizmos.color = Color.green;
                Vector2 size = Vector2.one;
                
                // Get the actual size from the collider if available
                if (spawnerCollider is BoxCollider2D boxCollider)
                {
                    size = boxCollider.size;
                }
                else if (spawnerCollider is CircleCollider2D circleCollider)
                {
                    size = new Vector2(circleCollider.radius * 2, circleCollider.radius * 2);
                }
                
                Gizmos.DrawWireCube(transform.position, size);
            }
        }
    }
}