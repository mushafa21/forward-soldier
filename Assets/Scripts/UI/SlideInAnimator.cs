using UnityEngine;
using DG.Tweening;

public class SlideInAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public Ease easeType = Ease.OutQuart; // This ease gives a smooth sliding effect
    public Vector2 slideOffset = new Vector2(0f, -100f); // How far to slide from (x, y)

    private Vector2 originalAnchoredPosition;
    private bool isInitialized = false;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = GetComponentInParent<RectTransform>();
        }
    }

    private void Initialize()
    {
        if (isInitialized) return;
        
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) rectTransform = GetComponentInParent<RectTransform>();
        
        if (rectTransform != null)
        {
            originalAnchoredPosition = rectTransform.anchoredPosition;
        }
        else
        {
            Debug.LogError("RectTransform component not found on this object or its parent!", this);
        }

        isInitialized = true;
    }

    // Call this method to show the UI element with slide in animation
    public void ShowSlideIn()
    {
        Initialize();

        if (rectTransform == null) return;

        // Set initial position below the current position
        Vector2 startPosition = originalAnchoredPosition + slideOffset;

        // This ensures the start position is off-screen/below before animating in
        rectTransform.anchoredPosition = startPosition;

        // Animate the position from below to its original position
        rectTransform.DOAnchorPos(originalAnchoredPosition, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true); // Use SetUpdate(true) if you need it to run when Time.timeScale is 0 (e.g., in a pause menu)
    }
    
    public void HideSlideOut()
    {
        Initialize();

        if (rectTransform == null) return;

        // Animate the position from original position to below
        Vector2 endPosition = originalAnchoredPosition + slideOffset;
        
        rectTransform.DOAnchorPos(endPosition, animationDuration)
            .SetEase(Ease.InQuart) // Use a complementary ease for hiding
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // Optional: Disable the GameObject after it's hidden
                gameObject.SetActive(false);
            });
    }
    
    private void OnEnable()
    {
        // ShowSlideIn();
    }
}