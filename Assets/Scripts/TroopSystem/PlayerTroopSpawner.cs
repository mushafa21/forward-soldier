using UnityEngine;
using PathSystem;
using SoulSystem;
using UnityEngine.EventSystems;

namespace TroopSystem
{
    public class PlayerTroopSpawner : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Spawner Settings")]
        public LanePath spawnPath; // Path where troops will spawn and move along

        [Header("Spawn Settings")]
        public GameObject troopPrefab; // Prefab of the troop to spawn
        public KeyCode activationKey = KeyCode.Space; // Optional keyboard key to trigger spawn

        [Header("Visual Feedback")]
        public bool changeColorOnHover = true; // Whether to change color when mouse hovers
        public Color normalColor = Color.white; // Normal color
        public Color hoverColor = Color.yellow; // Color when mouse hovers
        public float hoverScaleMultiplier = 1.1f; // Scale multiplier when hovered
        public GameObject spawnEffect;

        
        private Collider2D spawnerCollider; // Collider to detect clicks
        private SpriteRenderer spriteRenderer; // Reference to sprite renderer for visual feedback
        private Vector3 originalScale; // Store original scale
        private Color originalColor = Color.white; // Store original colo
        

        void Start()
        {
            
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
            // Check if there's a selected troop
            if (TroopManager.Instance.currentSelectedTroop == null)
            {
                Debug.Log("No troop selected for spawning.");
                return;
            }
            
            // Check if the selected troop is on cooldown
            if (TroopManager.Instance.currentSelectedTroop.isInCooldown)
            {
                Debug.Log("Selected troop is on cooldown.");
                return;
            }
            
            // Check if player has enough souls to spawn the selected troop
            if (TroopManager.Instance.currentSelectedTroop.troopSO.soulCost <= SoulManager.Instance.GetSouls())
            {
                SoulManager.Instance.DecreaseSouls(TroopManager.Instance.currentSelectedTroop.troopSO.soulCost);
                SpawnTroop();
            }
            else
            {
                Debug.Log("Not enough souls to spawn selected troop.");
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
                        newTroop.SetTroopSO(TroopManager.Instance.currentSelectedTroop.troopSO);
                        newTroop.level = TroopManager.Instance.currentSelectedTroop.currentLevel; // Set the troop level
                        TroopManager.Instance.currentSelectedTroop.StartCooldown();
                    }
                }

                if (spawnEffect != null)
                {
                    GameObject  spawnGO = Instantiate(spawnEffect);
                    spawnGO.transform.position = transform.position;
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