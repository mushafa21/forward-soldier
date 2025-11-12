using UnityEngine;

public class LightPillar : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Duration of the scale animation (half cycle - 0 to 10)")]
    public float animationDuration = 1.0f;
    
    [Tooltip("Maximum scale on the Y axis")]
    public float maxYScale = 10.0f;

    private Vector3 initialScale;
    private float animationTimer;
    private bool isAnimating = true;
    public AudioClip summonSound;

    void Start()
    {
        // Store initial scale
        initialScale = transform.localScale;
        
        // Start with Y scale at 0
        transform.localScale = new Vector3(initialScale.x, 0, initialScale.z);
        
        // Reset timer
        animationTimer = 0f;
        if (summonSound != null)
        {
            AudioManager.Instance.PlaySFX(summonSound);
        }
    }

    void Update()
    {
        if (!isAnimating) return;

        // Increment animation timer
        animationTimer += Time.deltaTime;

        // Calculate the progress through the animation (0 to 1, then back to 0)
        float progress = animationTimer / (animationDuration * 2);
        
        float yScale;
        if (progress <= 0.5f)
        {
            // Scale up: 0 to max
            float upProgress = progress * 2; // Normalize to 0-1 for the up phase
            yScale = Mathf.Lerp(0, maxYScale, upProgress);
        }
        else
        {
            // Scale down: max back to 0
            float downProgress = (progress - 0.5f) * 2; // Normalize to 0-1 for the down phase
            yScale = Mathf.Lerp(maxYScale, 0, downProgress);
        }

        // Apply the new scale (keeping X and Z unchanged)
        transform.localScale = new Vector3(initialScale.x, yScale, initialScale.z);

        // Check if we've completed a full cycle (0 to max back to 0)
        if (animationTimer >= animationDuration * 2)
        {
            // Destroy the object after animation completes
            Destroy(transform.parent.gameObject);
        }
    }
}