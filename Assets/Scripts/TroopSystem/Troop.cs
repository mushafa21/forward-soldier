using UnityEngine;
using PathSystem;

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
        public bool isReversed = false; // If true, follow path in reverse order
        
        [Header("Troop Properties")]
        public TroopFaction faction = TroopFaction.Player;
        public TroopSO troopStats; // Reference to the ScriptableObject for stats
        
        [Header("Combat Stats")]
        public float maxHealth = 100f;
        public float currentHealth;
        public float damage = 20f;
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        private float lastAttackTime = 0f;
        
        [Header("State Management")]
        public TroopState currentState = TroopState.Idle;
        
        [Header("Visual References")]
        public GameObject walkParticle; // Reference to the walk particle game object
        public Animator animator; // Reference to the animator (if using animations)
        
        [Header("Timing Settings")]
        public float deathDestroyDelay = 2f; // Time to wait before destroying after death

        private bool hasReachedEnd = false;
        private float originalScaleX = 0f; // Store original x scale to properly flip
        private float originalParticleScaleX = 0f; // Store original particle x scale

        // Awake is called before Start
        void Awake()
        {
            // Capture the original scale values from the prefab immediately
            originalScaleX = transform.localScale.x;
            
            if (walkParticle != null)
            {
                originalParticleScaleX = walkParticle.transform.localScale.x;
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
                maxHealth = troopStats.health;
                damage = troopStats.damage;
                moveSpeed = troopStats.movementSpeed;
                attackRange = troopStats.attackRange;
                attackCooldown = troopStats.attackCooldown;
            }
            
            currentHealth = maxHealth;
            
            // Initialize visual components
            InitializeVisuals();
            
            // Register with the troop manager
            TroopManager.Instance.AddTroop(this);
            
            if (currentPath != null && currentPath.waypoints.Count > 0)
            {
                if (isReversed)
                {
                    // Start at the end of the path for reversed movement
                    transform.position = currentPath.GetEndPoint();
                    currentWaypointIndex = currentPath.GetWaypointCount() - 2; // Second to last waypoint
                }
                else
                {
                    // Start at the beginning of the path for normal movement
                    transform.position = currentPath.GetStartPoint();
                    currentWaypointIndex = 1; // Next waypoint to move toward
                }
                
                // Update scale based on direction
                UpdateScaleBasedOnDirection();
                
                // Switch to walk state after setting up the path
                ChangeState(TroopState.Walk);
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

        void HandleIdleState()
        {
            // In idle state, troop stops moving and plays idle animation
            // Movement is suspended in this state
            
            // Make sure walk particles are off
            if (walkParticle != null)
                walkParticle.SetActive(false);
            
           
        }

        void HandleWalkState()
        {
            // In walk state, troop moves along the path and plays walk particle
            MoveAlongPath();
            
            // Enable walk particle
            if (walkParticle != null)
                walkParticle.SetActive(true);
    
        }

        void HandleAttackState()
        {
            // In attack state, troop stops moving and plays attack animation
            // Movement is suspended in this state
            
            // Make sure walk particles are off
            if (walkParticle != null)
                walkParticle.SetActive(false);
            
           
        }

        void HandleDeathState()
        {
            // In death state, troop stops moving and plays death animation
            // Movement is suspended in this state
            
            // Make sure walk particles are off
            if (walkParticle != null)
                walkParticle.SetActive(false);
            
      
            
            // Destroy the game object after delay
            Destroy(gameObject, deathDestroyDelay);
        }

        // Move the troop along the assigned path (only active in Walk state)
        void MoveAlongPath()
        {
            if (currentPath == null || currentPath.waypoints.Count == 0) return;

            if (isReversed)
            {
                // Move in reverse direction (from end to start)
                if (currentWaypointIndex >= 0)
                {
                    Vector3 targetWaypoint = currentPath.GetWaypoint(currentWaypointIndex);
                    
                    // Move towards the target waypoint
                    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, moveSpeed * Time.deltaTime);
                    
                    // Check if we've reached the waypoint
                    if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
                    {
                        currentWaypointIndex--;
                        
                        // If we've reached the first waypoint, we've reached the end
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
                // Normal movement (from start to end)
                if (currentWaypointIndex < currentPath.GetWaypointCount())
                {
                    Vector3 targetWaypoint = currentPath.GetWaypoint(currentWaypointIndex);
                    
                    // Move towards the target waypoint
                    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, moveSpeed * Time.deltaTime);
                    
                    // Check if we've reached the waypoint
                    if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
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

            Vector3 direction = Vector3.zero;

            if (isReversed)
            {
                // For reversed movement, we're going from higher index to lower index (end to start)
                if (currentWaypointIndex >= 0 && currentWaypointIndex < currentPath.GetWaypointCount() - 1)
                {
                    // Calculate direction from current index to previous index (moving backward)
                    direction = currentPath.GetWaypoint(currentWaypointIndex) - currentPath.GetWaypoint(currentWaypointIndex + 1);
                }
                else if (currentWaypointIndex == currentPath.GetWaypointCount() - 1 && currentPath.GetWaypointCount() > 1)
                {
                    // At start of reversed path (but at the end of the waypoint array), 
                    // use direction from end of array to second to last
                    direction = currentPath.GetWaypoint(currentPath.GetWaypointCount() - 1) - 
                                currentPath.GetWaypoint(currentPath.GetWaypointCount() - 2);
                }
            }
            else
            {
                // For normal movement, we're going from lower index to higher index (start to end)
                if (currentWaypointIndex > 0 && currentWaypointIndex < currentPath.GetWaypointCount())
                {
                    // Calculate direction from current index to next index
                    direction = currentPath.GetWaypoint(currentWaypointIndex) - currentPath.GetWaypoint(currentWaypointIndex - 1);
                }
                else if (currentWaypointIndex == 0 && currentPath.GetWaypointCount() > 1)
                {
                    // At start of normal path, use direction from first to second
                    direction = currentPath.GetWaypoint(1) - currentPath.GetWaypoint(0);
                }
            }

            // Flip based on x component of direction
            if (direction.x < 0) // Moving left
            {
                // Flip the x scale to face left
                transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                if (walkParticle != null)
                {
                    walkParticle.transform.localScale = new Vector3(-Mathf.Abs(originalParticleScaleX), walkParticle.transform.localScale.y, walkParticle.transform.localScale.z);
                }
            }
            else if (direction.x > 0) // Moving right
            {
                // Keep normal x scale to face right
                transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
                if (walkParticle != null)
                {
                    walkParticle.transform.localScale = new Vector3(Mathf.Abs(originalParticleScaleX), walkParticle.transform.localScale.y, walkParticle.transform.localScale.z);
                }
            }
        }

        // Called when the troop reaches the end of its path
        void OnReachedPathEnd()
        {
            ChangeState(TroopState.Attack);
            Debug.Log($"Troop reached the {(isReversed ? "start" : "end")} of the path!");
            // In the game context, this might trigger damage to the enemy castle or player castle
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
        }

        // Method to change the troop's state
        public void ChangeState(TroopState newState)
        {
            currentState = newState;
            switch (currentState)
            {
                case TroopState.Idle : 
                    if(animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Idle"); 
                    break;
                case TroopState.Walk: 
                    if(animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Walk"); 
                    break;
                case TroopState.Attack: 
                    if(animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Attack"); 
                    break;
                case TroopState.Death: 
                    // Unregister from troop manager when dying
                    TroopManager.Instance?.RemoveTroop(this);
                    if(animator != null && animator.runtimeAnimatorController != null) animator.SetTrigger("Death"); 
                    break;
            }
        }

        // Method to assign a path to this troop
        public void SetPath(LanePath path, bool reverse = false)
        {
            if (path != null)
            {
                currentPath = path;
                isReversed = reverse;
                currentWaypointIndex = reverse ? path.GetWaypointCount() - 1 : 0;
                hasReachedEnd = false;
                
                // Start at the appropriate end of the new path
                if (path.waypoints.Count > 0)
                {
                    if (reverse)
                    {
                        transform.position = path.GetEndPoint();
                        currentWaypointIndex = path.GetWaypointCount() - 2; // Second to last waypoint when going in reverse
                    }
                    else
                    {
                        transform.position = path.GetStartPoint();
                        currentWaypointIndex = 1; // Next waypoint when going normally
                    }
                }
                
                // Update scale based on direction
                UpdateScaleBasedOnDirection();
                
                // Automatically transition to walk state when a path is set
                ChangeState(TroopState.Walk);
            }
        }

        // Clean up when the object is destroyed
        void OnDestroy()
        {
            TroopManager.Instance?.RemoveTroop(this);
        }
    }
}