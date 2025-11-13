using UnityEngine;
using PathSystem;
using System.Collections.Generic;
using MoreMountains.Tools;
using SoulSystem;

namespace TroopSystem
{
        
    public enum TroopFaction
    {
        Player,
        Enemy
    }

    public enum TroopState
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    public class Troop : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 2f;
        
        [Header("Path Following")]
        public LanePath currentPath;
        public int currentWaypointIndex = 0;
        
        [Header("Troop Properties")]
        public TroopFaction faction = TroopFaction.Player;
        public TroopSO troopStats; // Reference to the ScriptableObject for stats
        public int level = 1; // Level of the troop
        
        [Header("Combat Stats")]
        public float maxHealth = 100f;
        public float currentHealth;
        public float damage = 20f;
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        private float lastAttackTime = 0f;
        private float attackAnimationTime = 0.25f; // Time for attack animation to complete before dealing damage
        
        [Header("State Management")]
        public TroopState currentState = TroopState.Idle;
        
        [Header("Visual References")]
        public MMProgressBar healthBar;
        public GameObject walkParticle; // Reference to the walk particle game object
        public Animator animator; // Reference to the animator (if using animations)
        public SpriteRenderer spriteRenderer; // *** ADD THIS ***
        public ParticleSystem spawnParticle;
        [Header("Audio References")]
        public AudioSource audioSource; // Reference to the audio source component
        
        [Header("Faction Visuals")] // *** ADD THIS SECTION ***
        public Material factionMaterial; // Assign your 'Troop_ColorSwap_Material' here
        public Color playerColor = new Color(0.2f, 0.4f, 1f, 1f); // The default 'target' blue
        public Color enemyColor = new Color(1f, 0.2f, 0.2f, 1f); // The 'target' red
        
        [Header("Timing Settings")]
        public float deathDestroyDelay = 2f; // Time to wait before destroying after death

        [Header("Visual Positioning")]
        public float spawnYOffset = 0.2f; // Y offset to prevent visual stacking
        public float deathZPosition = 10f; // Z position when dead to move behind other objects
        public float zOffsetPerYUnit = 0.1f; // How much Z offset per Y unit (for depth perception)
        
        [Header("Combat Settings")]
        public float towerSizeCompensation = 3.0f; // Additional range to account for tower size when targeting

        [Header("Ghost Settings")]
        public GameObject ghostPrefab; // Prefab to spawn when troop dies
        public float ghostSpawnYOffset = 0.5f; // Y offset to position ghost above the troop

        private bool hasReachedEnd = false;
        private float originalScaleX = 0f; // Store original x scale to properly flip
        private float originalParticleScaleX = 0f; // Store original particle x scale
        private float originalParticlePositionX = 0f; // Store original particle x scale

        private TowerSystem.Tower targetTower = null; // Tower that this troop is targeting
        private Troop targetTroop = null; // Other troop that this troop is targeting
        private float yPositionOffset = 0f; // Y offset for this specific troop

        private bool isWalkingSoundPlaying = false; // Track if walk sound is currently playing

        // Awake is called before Start
        void Awake()
        {
            // Capture the original scale values from the prefab immediately
            originalScaleX = transform.localScale.x;
            
            if (walkParticle != null)
            {
                originalParticleScaleX = walkParticle.transform.localScale.x;
                originalParticlePositionX = walkParticle.transform.localPosition.x;
            }

            // *** ADD THIS ***
            // Ensure spriteRenderer is assigned, try to find it if not
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            // Ensure audioSource is assigned, try to find it if not
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    // Add an AudioSource component if none exists
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false; // Don't play automatically
                }
            }
            
            if (spawnParticle != null)
            {
                var main = spawnParticle.main;
                main.simulationSpeed = 4;
                // spawnParticle.playbackSpeed = 4;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set initial state to Idle
            currentState = TroopState.Idle;
            
            // Initialize stats from ScriptableObject if provided
            if (troopStats != null)
            {
                maxHealth = troopStats.GetHealthAtLevel(level);
                damage = troopStats.damage;
                moveSpeed = troopStats.movementSpeed;
                attackRange = troopStats.attackRange;
                attackCooldown = troopStats.attackCooldown;
                if (troopStats.animatorController != null)
                {
                    animator.runtimeAnimatorController = troopStats.animatorController;
                }
            }
            
            currentHealth = maxHealth;
            
            // Initialize visual components
            InitializeVisuals();
            
            UpdateHealth();

            // *** ADD THIS ***
            InitializeFactionColor();
            
            // Register with the troop manager
            TroopManager.Instance.AddTroop(this);
            
            if (currentPath != null && currentPath.waypoints.Count > 0)
            {
                if (faction == TroopFaction.Enemy)
                {
                    // Enemy troops start at the end of the path and move toward the start
                    Vector3 startPos = currentPath.GetEndPoint();
                    transform.position = startPos;
                    currentWaypointIndex = currentPath.GetWaypointCount() - 2; // Second to last waypoint
                }
                else
                {
                    // Player troops start at the beginning of the path and move toward the end
                    Vector3 startPos = currentPath.GetStartPoint();
                    transform.position = startPos;
                    currentWaypointIndex = 1; // Next waypoint to move toward
                }
                
                // Calculate Y position offset to prevent visual stacking
                CalculateYPositionOffset();
                
                // Update scale based on direction
                UpdateScaleBasedOnDirection();
                
                // Switch to walk state after setting up the path
                ChangeState(TroopState.Walk);
            }

          
        }

        // *** ADD THIS NEW METHOD ***
        void InitializeFactionColor()
        {
            if (spriteRenderer == null || factionMaterial == null)
            {
                if (spriteRenderer == null) Debug.LogWarning($"SpriteRenderer not assigned on {gameObject.name}");
                if (factionMaterial == null) Debug.LogWarning($"FactionMaterial not assigned on {gameObject.name}");
                return;
            }

            // Create a new instance of the material for this troop
            // This is CRUCIAL so that changing one troop's color
            // doesn't change all troops using the same material.
            Material materialInstance = new Material(factionMaterial);
            
            // Set the sprite renderer to use this new material instance
            spriteRenderer.material = materialInstance;

            // Set the '_TargetColor' property on the shader
            if (faction == TroopFaction.Enemy)
            {
                materialInstance.SetColor("_TargetColor", enemyColor);
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


        // Update is called once per frame
        void Update()
        {
            switch (currentState)
            {
                case TroopState.Idle:
                    HandleIdleState();
                    break;
                case TroopState.Walk:
                    HandleWalkState();
                    break;
                case TroopState.Attack:
                    HandleAttackState();
                    break;
                case TroopState.Death:
                    HandleDeathState();
                    break;
            }
        }

        void InitializeVisuals()
        {
            // Set up walk particle system
            if (walkParticle != null)
            {
                walkParticle.SetActive(false); // Start inactive
            }
        }

        // Calculate Y position offset to prevent visual stacking with other troops on the same path
        void CalculateYPositionOffset()
        {
            // Apply the offset based on the last spawn state on this path
            switch (currentPath.lastSpawnState)
            {
                case PathSystem.TroopSpawnState.Center:
                    yPositionOffset = 0f; // Center position
                    currentPath.lastSpawnState = PathSystem.TroopSpawnState.Up; // Next troop goes up
                    break;
                case PathSystem.TroopSpawnState.Up:
                    yPositionOffset = spawnYOffset; // Up position
                    currentPath.lastSpawnState = PathSystem.TroopSpawnState.Down; // Next troop goes down
                    break;
                case PathSystem.TroopSpawnState.Down:
                    yPositionOffset = -spawnYOffset; // Down position
                    currentPath.lastSpawnState = PathSystem.TroopSpawnState.Center; // Next troop goes center
                    break;
            }
            
            // Apply the offset to the current position
            Vector3 newPosition = transform.position;
            newPosition.y += yPositionOffset;
            
            // Also adjust Z position based on Y offset for depth perception (troops higher up appear further back)
            // To achieve this: higher Y = higher Z (further back), lower Y = lower Z (closer to front)
            newPosition.z += yPositionOffset * zOffsetPerYUnit;
            
            transform.position = newPosition;
        }

        void HandleIdleState()
        {
            // In idle state, troop stops moving and plays idle animation
            // Movement is suspended in this state
            
            // Make sure walk particles are off
            if (walkParticle != null)
                walkParticle.SetActive(false);
            
            // Make sure walk sound is off when idle
            StopWalkSound();
            
            // If we're just in attack cooldown, don't check for targets yet
            if (Time.time - lastAttackTime < attackCooldown)
            {
                // Wait for cooldown to complete
                // But check if targets are dead during cooldown to clear them
                if (targetTower != null && (targetTower.currentHealth <= 0 || !targetTower.isActiveAndEnabled))
                {
                    targetTower = null;
                }
                
                if (targetTroop != null && (targetTroop.currentHealth <= 0 || !targetTroop.isActiveAndEnabled))
                {
                    targetTroop = null;
                    // Switch back to walk state after killing enemy troop
                    ChangeState(TroopState.Walk);
                    return;
                }
                
                return;
            }

            // Check if our current targets are dead and clear them if so
            if (targetTower != null && (targetTower.currentHealth <= 0 || !targetTower.isActiveAndEnabled))
            {
                targetTower = null; // Clear the destroyed tower target
            }
            
            if (targetTroop != null && (targetTroop.currentHealth <= 0 || !targetTroop.isActiveAndEnabled))
            {
                targetTroop = null; // Clear the destroyed troop target
                // Switch back to walk state to continue moving after killing enemy troop
                ChangeState(TroopState.Walk);
                return;
            }

            // If we still have a valid target that's in range, go back to attack state
            if (targetTower != null)
            {
                float distanceToTower = Vector3.Distance(transform.position, targetTower.transform.position);
                // Add size compensation for larger towers
                float effectiveTowerRange = attackRange + towerSizeCompensation;
                if (targetTower.currentHealth > 0 && targetTower.isActiveAndEnabled && distanceToTower <= effectiveTowerRange)
                {
                    ChangeState(TroopState.Attack);
                    return;
                }
                else
                {
                    // Target is no longer valid (dead, disabled, or out of range), clear it
                    targetTower = null;
                }
            }
            
            if (targetTroop != null)
            {
                float distanceToTroop = Vector3.Distance(transform.position, targetTroop.transform.position);
                if (targetTroop.currentHealth > 0 && targetTroop.isActiveAndEnabled && distanceToTroop <= attackRange)
                {
                    ChangeState(TroopState.Attack);
                    return;
                }
                else
                {
                    // Target is no longer valid (dead, disabled, or out of range), clear it
                    targetTroop = null;
                }
            }

            // If no current targets are valid, check for new targets
            CheckForTargetInRange();
            
            // If we didn't transition to attack state after checking for targets,
            // we should go back to walk state to continue moving
            if (currentState != TroopState.Attack)
            {
                ChangeState(TroopState.Walk);
            }
        }

        void HandleWalkState()
        {
            // In walk state, troop moves along the path and plays walk particle
            MoveAlongPath();
            
            // Enable walk particle
            if (walkParticle != null)
                walkParticle.SetActive(true);
            
            // Ensure walk sound is playing in walk state
            PlayWalkSound();
            
            // Check if a target (tower or troop) is in range and transition to attack state if so
            CheckForTargetInRange();
        }

        void HandleAttackState()
        {
            // In attack state, troop stops moving
            // Movement is suspended in this state
            
            // Make sure walk particles are off
            if (walkParticle != null)
                walkParticle.SetActive(false);
            
            // Check if our target is still valid - if not, transition back to idle to look for new targets
            if (targetTower != null && (targetTower.currentHealth <= 0 || !targetTower.isActiveAndEnabled))
            {
                targetTower = null;
                ChangeState(TroopState.Idle);
                return;
            }
            
            if (targetTroop != null && (targetTroop.currentHealth <= 0 || !targetTroop.isActiveAndEnabled))
            {
                targetTroop = null;
                ChangeState(TroopState.Idle);
                return;
            }
            
            // This state should have already triggered the attack animation via ChangeState
            // The actual attack damage and state transition are handled in DealAttackDamageAndGoToIdle
            // via the Invoke call in ChangeState
        }



        // Check if there is a target (tower or other troop) in range and set it if found
        void CheckForTargetInRange()
        {
            // Only look for new targets if we don't already have one
            if (targetTower == null && targetTroop == null)
            {
                TowerSystem.Tower closestTower;
                if (faction == TroopFaction.Enemy)
                {
                    closestTower = LevelManager.Instance.playerTower;
                }
                else
                {
                    closestTower = LevelManager.Instance.enemyTower;
                }
                if (closestTower != null)
                {
                    float distanceToTower = Vector3.Distance(transform.position, closestTower.transform.position);
                    // Add size compensation for larger towers
                    float effectiveTowerRange = attackRange + towerSizeCompensation;
                    if (distanceToTower <= effectiveTowerRange || hasReachedEnd)
                    {
                        targetTower = closestTower; // Set the target tower
                        ChangeState(TroopState.Attack);
                        return;
                    }
                }
                
                Troop closestTroop = FindClosestEnemyTroop();
                if (closestTroop != null)
                {
                    float distanceToTroop = Vector3.Distance(transform.position, closestTroop.transform.position);
                    if (distanceToTroop <= attackRange)
                    {
                        targetTroop = closestTroop; // Set the target troop
                        ChangeState(TroopState.Attack);
                        return;
                    }
                }
            }
            // If we get here, no targets were found in range
            // This method just checks and sets targets if found, doesn't change state
        }

        

        // Find the closest enemy troop of the opposite faction on the current path
        TroopSystem.Troop FindClosestEnemyTroop()
        {
            // Get all active troops from the manager
            List<TroopSystem.Troop> allTroops = TroopManager.Instance.GetAllTroops();

            TroopSystem.Troop closestTroop = null;
            float closestDistance = float.MaxValue;

            foreach (TroopSystem.Troop troop in allTroops)
            {
                // Check if the troop is active, not dead, on the same path, and of opposite faction
                if (troop != null && troop != this && troop.currentHealth > 0 && troop.currentPath == currentPath && IsOppositeFactionTroop(troop))
                {
                    float distance = Vector3.Distance(transform.position, troop.transform.position);

                    if (distance < closestDistance)
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
                case TroopFaction.Player:
                    return otherTroop.faction == TroopFaction.Enemy;
                case TroopFaction.Enemy:
                    return otherTroop.faction == TroopFaction.Player;
                default:
                    return false;
            }
        }
        
        // Check if the tower is of opposite faction
        bool IsOppositeFactionTower(TowerSystem.Tower tower)
        {
            if (tower == null) return false;
            
            switch (faction)
            {
                case TroopFaction.Player:
                    return tower.faction == TowerSystem.TowerFaction.Enemy;
                case TroopFaction.Enemy:
                    return tower.faction == TowerSystem.TowerFaction.Player;
                default:
                    return false;
            }
        }

        void HandleDeathState()
        {
            // In death state, troop stops moving and plays death animation
            // Movement is suspended in this state
            
            // Make sure walk particles are off
            if (walkParticle != null)
                walkParticle.SetActive(false);
            
      
            // Move to back layer by setting Z position
            Vector3 pos = transform.position;
            pos.z = deathZPosition;
            transform.position = pos;
            
            // Destroy the game object after delay
            Destroy(gameObject, deathDestroyDelay);
        }

        void SpawnGhost()
        {
            // Spawn ghost prefab if available
            if (ghostPrefab != null)
            {
                // Position the ghost slightly above the troop's position
                Vector3 ghostPosition = transform.position;
                ghostPosition.y += ghostSpawnYOffset;
                
                // Spawn the ghost
                GameObject spawnedGhost = Instantiate(ghostPrefab, ghostPosition, Quaternion.identity);
                
                // Set the ghost's parent to the troop so it gets destroyed when the troop is destroyed
                spawnedGhost.transform.SetParent(transform);
            }

        }

        // Move the troop along the assigned path (only active in Walk state)
        void MoveAlongPath()
        {
            if (currentPath == null || currentPath.waypoints.Count == 0) return;

            if (faction == TroopFaction.Enemy)
            {
                // Enemy troops move from end to start (reverse direction)
                if (currentWaypointIndex >= 0)
                {
                    Vector3 targetWaypoint = currentPath.GetWaypoint(currentWaypointIndex);
                    
                    // Move towards the target waypoint while preserving the Y offset
                    Vector3 targetPosition = targetWaypoint;
                    targetPosition.y += yPositionOffset; // Add the Y offset to maintain visual stacking prevention
                    targetPosition.z += yPositionOffset * zOffsetPerYUnit; // Also preserve Z offset for depth

                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                    
                    // Check if we've reached the waypoint (using the offset-adjusted position for distance check)
                    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                    {
                        currentWaypointIndex--;
                        
                        // If we've reached the first waypoint, we've reached the end (which is start of path for player)
                        if (currentWaypointIndex < 0)
                        {
                            hasReachedEnd = true;
                            OnReachedPathEnd();
                        }
                        else
                        {
                            // Update scale based on the direction of the next movement
                            UpdateScaleBasedOnDirection();
                        }
                    }
                    
                }
            }
            else
            {
                // Player troops move from start to end (normal direction)
                if (currentWaypointIndex < currentPath.GetWaypointCount())
                {
                    Vector3 targetWaypoint = currentPath.GetWaypoint(currentWaypointIndex);
                    
                    // Move towards the target waypoint while preserving the Y offset
                    Vector3 targetPosition = targetWaypoint;
                    targetPosition.y += yPositionOffset; // Add the Y offset to maintain visual stacking prevention
                    targetPosition.z += yPositionOffset * zOffsetPerYUnit; // Also preserve Z offset for depth

                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                    
                    // Check if we've reached the waypoint (using the offset-adjusted position for distance check)
                    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                    {
                        currentWaypointIndex++;
                        
                        // If we've reached the last waypoint, we've reached the end
                        if (currentWaypointIndex >= currentPath.GetWaypointCount())
                        {
                            hasReachedEnd = true;
                            OnReachedPathEnd();
                        }
                        else
                        {
                            // Update scale based on the direction of the next movement
                            UpdateScaleBasedOnDirection();
                        }
                    }
                }
            }
        }

        // Update the scale based on the direction of movement (x-axis)
        void UpdateScaleBasedOnDirection()
        {
            if (currentPath == null || currentPath.waypoints.Count == 0) return;

            // Vector3 direction = Vector3.zero;
            //
            // if (faction == TroopFaction.Enemy)
            // {
            //     // For enemy troops, we're going from higher index to lower index (end to start)
            //     if (currentWaypointIndex >= 0 && currentWaypointIndex < currentPath.GetWaypointCount() - 1)
            //     {
            //         // Calculate direction from current index to previous index (moving backward)
            //         direction = currentPath.GetWaypoint(currentWaypointIndex) - currentPath.GetWaypoint(currentWaypointIndex + 1);
            //     }
            //     else if (currentWaypointIndex == currentPath.GetWaypointCount() - 1 && currentPath.GetWaypointCount() > 1)
            //     {
            //         // At start of enemy path (but at the end of the waypoint array), 
            //         // use direction from end of array to second to last
            //         direction = currentPath.GetWaypoint(currentPath.GetWaypointCount() - 1) - 
            //                     currentPath.GetWaypoint(currentPath.GetWaypointCount() - 2);
            //     }
            // }
            // else
            // {
            //     // For player troops, we're going from lower index to higher index (start to end)
            //     if (currentWaypointIndex > 0 && currentWaypointIndex < currentPath.GetWaypointCount())
            //     {
            //         // Calculate direction from current index to next index
            //         direction = currentPath.GetWaypoint(currentWaypointIndex) - currentPath.GetWaypoint(currentWaypointIndex - 1);
            //     }
            //     else if (currentWaypointIndex == 0 && currentPath.GetWaypointCount() > 1)
            //     {
            //         // At start of normal path, use direction from first to second
            //         direction = currentPath.GetWaypoint(1) - currentPath.GetWaypoint(0);
            //     }
            // }

            Vector3 towerPosition;
            if (faction == TroopFaction.Player)
            {
                towerPosition = LevelManager.Instance.enemyTower.transform.position;
            }
            else
            {
                towerPosition = LevelManager.Instance.playerTower.transform.position;
            }
            // Flip based on x component of direction
            if (towerPosition.x < transform.position.x) // Moving left
            {
                // Flip the x scale to face left
                transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                if (walkParticle != null)
                {
                    // walkParticle.transform.localPosition = new Vector3(-Mathf.Abs(originalParticlePositionX),walkParticle.transform.localPosition.y, walkParticle.transform.localPosition.z);
                    // walkParticle.transform.localScale = new Vector3(-Mathf.Abs(originalParticleScaleX), walkParticle.transform.localScale.y, walkParticle.transform.localScale.z);
                }
            }
            else if (towerPosition.x > transform.position.x) // Moving right
            {
                // Keep normal x scale to face right
                transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                if (walkParticle != null)
                {
                    // walkParticle.transform.localPosition = new Vector3(Mathf.Abs(originalParticlePositionX),walkParticle.transform.localPosition.y, walkParticle.transform.localPosition.z);
                    // walkParticle.transform.localScale = new Vector3(Mathf.Abs(originalParticleScaleX), walkParticle.transform.localScale.y, walkParticle.transform.localScale.z);
                }
            }
        }

        // Called when the troop reaches the end of its path
        void OnReachedPathEnd()
        {
            if (faction == TroopFaction.Player)
            {
                Debug.Log("Player troop reached the end of the path!");
            }
            else
            {
                Debug.Log("Enemy troop reached the start of the path!");
            }
            
            // Look for towers at the end of the path immediately
            // CheckForTargetInRange();
            
            // If a tower was found and we're in attack state, stay in attack
            // if (currentState != TroopState.Attack)
            // {
            //     // If no tower is found in range, stay at idle to continue checking
            //     // This allows troops to remain at the end of the path and keep checking for towers
            //     ChangeState(TroopState.Idle);
            // }
        }

        // Take damage from attacks
        public void TakeDamage(float damageAmount)
        {
            if (currentHealth <= 0) return; // Already dead

            currentHealth -= damageAmount;
            
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                ChangeState(TroopState.Death);
            }

            UpdateHealth();
        }

        private void UpdateHealth()
        {
            healthBar.UpdateBar(currentHealth,0,maxHealth);
        }

        // Play walk sound in a loop while walking
        private void PlayWalkSound()
        {
            if (audioSource != null && troopStats != null && troopStats.walkSound != null)
            {
                // If we're not already playing walk sound, start it
                if (!isWalkingSoundPlaying)
                {
                    audioSource.clip = troopStats.walkSound;
                    audioSource.loop = true;
                    audioSource.Play();
                    isWalkingSoundPlaying = true;
                }
            }
        }

        // Stop walk sound when not walking
        private void StopWalkSound()
        {
            if (audioSource != null && isWalkingSoundPlaying)
            {
                audioSource.loop = false;
                audioSource.Stop();
                isWalkingSoundPlaying = false;
            }
        }

        // Play attack sound once
        private void PlayAttackSound()
        {
            AudioManager.Instance.PlaySFX(troopStats.hitSound);
            if (audioSource != null && troopStats != null && troopStats.hitSound != null)
            {
                // Stop any looping sounds (like walk sound) before playing attack sound
                audioSource.loop = false;
                
                // Set the attack sound clip and play it once
                audioSource.PlayOneShot(troopStats.hitSound);
            }
        }
        
        private void PlayDeathSound()
        {

            if (audioSource != null && troopStats != null && troopStats.hitSound != null)
            {
                // Stop any looping sounds (like walk sound) before playing attack sound
                audioSource.loop = false;
                if (faction == TroopFaction.Player)
                {
                    AudioManager.Instance.PlaySFX(troopStats.allyDeathSound);
                    
                }
                else
                {
                    AudioManager.Instance.PlaySFX(troopStats.enemyDeathSound);
                    
                }
                
            }
        }

        // Method to change the troop's state
        public void ChangeState(TroopState newState)
        {
            TroopState previousState = currentState;
            currentState = newState;

            switch (currentState)
            {
                case TroopState.Idle :
                    if(animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Idle");
                    animator.SetBool("IsWalking",false);
                    StopWalkSound(); // Stop walk sound when entering idle state
                    break;
                case TroopState.Walk:
                    if(animator != null && animator.runtimeAnimatorController != null) animator.SetBool("IsWalking",true);
                    PlayWalkSound(); // Start walk sound when entering walk state
                    break;
                case TroopState.Attack:
                    // When entering attack state, first play attack animation
                    StopWalkSound(); // Stop walk sound when entering attack state
                    PlayAttackSound(); // Play attack sound when entering attack state

                    if(animator != null && animator.runtimeAnimatorController != null)
                    {
                        animator.SetBool("IsWalking",false);

                        animator.SetTrigger("Attack");
                    }
          
                    // After animation plays, deal damage and go to idle state
                    Invoke("DealAttackDamageAndGoToIdle", attackAnimationTime);
                    break;
                case TroopState.Death:
                    // Unregister from troop manager when dying
                    TroopManager.Instance?.RemoveTroop(this);
                    PlayDeathSound();
                    SpawnGhost();
                    if(animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Death");
                    animator.SetBool("IsWalking",false);
                    StopWalkSound(); // Stop walk sound when entering death state
                    if (faction == TroopFaction.Player)
                    {
                        EnemyManager.Instance.IncreaseSouls(troopStats.soulGainedWhenDefeated);
                    } else if (faction == TroopFaction.Enemy)
                    {
                        SoulManager.Instance.IncreaseSouls(troopStats.soulGainedWhenDefeated);
                    }
                    break;
            }
        }
        
        // Deal damage to the tower and go to idle state after attack animation completes
        void DealAttackDamageAndGoToIdle()
        {
            if (currentState == TroopState.Attack) // Make sure we're still in attack state
            {
                // Check if troop SO has a projectile prefab to use
                if (troopStats != null && troopStats.projectilePrefab != null)
                {
                    // Use projectile attack instead of direct attack
                    if (targetTower != null)
                    {
                        // Check if the tower is still alive and in range
                        if (targetTower.currentHealth > 0 && targetTower.isActiveAndEnabled)
                        {
                            float distanceToTower = Vector3.Distance(transform.position, targetTower.transform.position);
                            // Add size compensation for larger towers
                            float effectiveTowerRange = attackRange + towerSizeCompensation;
                            if (distanceToTower <= effectiveTowerRange || hasReachedEnd)
                            {
                                // Spawn projectile towards the tower
                                SpawnProjectile(targetTower.transform);
                            }
                        }
                    }
                    else if (targetTroop != null)
                    {
                        // Check if the target troop is still alive and in range
                        if (targetTroop.currentHealth > 0 && targetTroop.isActiveAndEnabled)
                        {
                            float distanceToTroop = Vector3.Distance(transform.position, targetTroop.transform.position);
                            if (distanceToTroop <= attackRange)
                            {
                                // Spawn projectile towards the target troop
                                SpawnProjectile(targetTroop.transform);
                            }
                        }
                    }
                }
                else
                {
                    // Use direct attack (original behavior) if no projectile prefab is set
                    if (targetTower != null)
                    {
                        // Check if the tower is still alive and in range
                        if (targetTower.currentHealth > 0 && targetTower.isActiveAndEnabled)
                        {
                            float distanceToTower = Vector3.Distance(transform.position, targetTower.transform.position);
                            // Add size compensation for larger towers
                            float effectiveTowerRange = attackRange + towerSizeCompensation;
                            if (distanceToTower <= effectiveTowerRange || hasReachedEnd)
                            {
                                targetTower.TakeDamage(damage);
                            }
                        }
                    }
                    else if (targetTroop != null)
                    {
                        // Check if the target troop is still alive and in range
                        if (targetTroop.currentHealth > 0 && targetTroop.isActiveAndEnabled)
                        {
                            float distanceToTroop = Vector3.Distance(transform.position, targetTroop.transform.position);
                            if (distanceToTroop <= attackRange)
                            {
                                targetTroop.TakeDamage(damage);
                            }
                        }
                    }
                }
                
                // Update last attack time
                lastAttackTime = Time.time;
                
                // Go to idle state for the remainder of cooldown
                ChangeState(TroopState.Idle);
            }
        }
        
        private void SpawnProjectile(Transform target)
        {
            if (troopStats != null && troopStats.projectilePrefab != null)
            {
                // Spawn the projectile at the current troop's position
                GameObject projectileObj = Instantiate(troopStats.projectilePrefab, transform.position, Quaternion.identity);
                
                // Get the projectile component and initialize it
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                if (projectile != null)
                {
                    // Use the projectile speed from the troop SO
                    float projectileSpeed = troopStats.projectileSpeed;
                    // Initialize with target, source troop, damage and speed
                    projectile.Initialize(target, this, damage, projectileSpeed);
                }
            }
        }

        // Method to assign a path to this troop
        public void SetPath(LanePath path)
        {
            if (path != null)
            {
                currentPath = path;
                hasReachedEnd = false;
                
                // Start at the appropriate end of the new path based on
                if (path.waypoints.Count > 0)
                {
                    if (faction == TroopFaction.Enemy)
                    {
                        transform.position = path.GetEndPoint();
                        currentWaypointIndex = path.GetWaypointCount() - 2; // Second to last waypoint
                    }
                    else
                    {
                        transform.position = path.GetStartPoint();
                        currentWaypointIndex = 1; // Next waypoint
                    }
                }
                
                // Update scale based on direction
                UpdateScaleBasedOnDirection();
                
                // Automatically transition to walk state when a path is set
                ChangeState(TroopState.Walk);
            }
        }
        
        public void SetTroopSO(TroopSO troopSO)
        {
            troopStats = troopSO;
            maxHealth = troopStats.GetHealthAtLevel(level);
            damage = troopStats.damage;
            moveSpeed = troopStats.movementSpeed;
            attackRange = troopStats.attackRange;
            attackCooldown = troopStats.attackCooldown;
            if (troopStats.animatorController != null)
            {
                animator.runtimeAnimatorController = troopStats.animatorController;
            }
        }

        // Clean up when the object is destroyed
        void OnDestroy()
        {
            TroopManager.Instance?.RemoveTroop(this);
        }
    }
}