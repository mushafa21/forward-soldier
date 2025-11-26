using UnityEngine;
using DG.Tweening;

public enum SlideDirection
{
    DownToUp,    // Slides from bottom to top
    UpToDown,    // Slides from top to bottom
    LeftToRight, // Slides from left to right
    RightToLeft  // Slides from right to left
}

public class SlideInAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public Ease easeType = Ease.OutQuart; // This ease gives a smooth sliding effect
    public SlideDirection slideDirection = SlideDirection.DownToUp; // Direction of the slide
    public float slideDistance = 100f; // How far to slide

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

    // Helper method to get the offset vector based on slide direction
    private Vector2 GetSlideOffset()
    {
        switch (slideDirection)
        {
            case SlideDirection.DownToUp:
                return new Vector2(0f, -slideDistance);
            case SlideDirection.UpToDown:
                return new Vector2(0f, slideDistance);
            case SlideDirection.LeftToRight:
                return new Vector2(-slideDistance, 0f);
            case SlideDirection.RightToLeft:
                return new Vector2(slideDistance, 0f);
            default:
                return new Vector2(0f, -slideDistance); // Default to DownToUp
        }
    }

    // Call this method to show the UI element with slide in animation
    public void ShowSlideIn()
    {
        Initialize();

        if (rectTransform == null) return;

        // Get the offset based on the slide direction
        Vector2 offset = GetSlideOffset();

        // Set initial position off-screen based on direction
        Vector2 startPosition = originalAnchoredPosition + offset;

        // This ensures the start position is off-screen before animating in
        rectTransform.anchoredPosition = startPosition;

        // Animate the position from off-screen to its original position
        rectTransform.DOAnchorPos(originalAnchoredPosition, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true); // Use SetUpdate(true) if you need it to run when Time.timeScale is 0 (e.g., in a pause menu)
    }

    public void HideSlideOut()
    {
        Initialize();

        if (rectTransform == null) return;

        // Get the offset based on the slide direction
        Vector2 offset = GetSlideOffset();

        // Animate the position from original position to off-screen
        Vector2 endPosition = originalAnchoredPosition + offset;

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