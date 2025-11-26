
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager instance;

    [Header("Transition Settings")]
    public GameObject transitionCanvas; // Canvas for the transition effect
    public UnityEngine.UI.Image transitionCircle; // Circle image for the wipe effect
    public float transitionDuration = 1f; // Duration of the transition effect

    // Flag to track if we're in a transition state
    private bool inTransition = false;

    public static TransitionManager Instance
    {
        get
        {
            return instance;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the transition canvas when a new scene loads
        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(false);

            // Reset the circle scale to zero to ensure a clean state
            if (transitionCircle != null)
            {
                transitionCircle.rectTransform.localScale = Vector3.zero;
            }
        }

        // End the transition state
        inTransition = false;
    }

    // Method to signal start of transition
    public void StartTransition()
    {
        inTransition = true;
    }

    // Method to signal end of transition
    public void EndTransition()
    {
        inTransition = false;

        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(false);

            // Reset the circle scale to zero to ensure a clean state
            if (transitionCircle != null)
            {
                transitionCircle.rectTransform.localScale = Vector3.zero;
            }
        }
    }

    // Method to complete the exit animation after scene load
    public IEnumerator CompleteExitAnimation(System.Action onComplete)
    {
        if (transitionCanvas != null && transitionCircle != null)
        {
            // Make sure the transition canvas is active for the exit animation in the new scene
            transitionCanvas.SetActive(true);

            // Start with the circle covering everything (from the previous scene's end state)
            transitionCircle.rectTransform.localScale = Vector3.one * 30f;

            // Animate the circle shrinking to reveal the new scene
            float elapsedTime = 0f;
            Vector3 startScale = Vector3.one * 30f;
            Vector3 endScale = Vector3.zero; // Shrink to nothing to reveal everything

            while (elapsedTime < transitionDuration * 0.5f) // Use half the duration for the reveal
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / (transitionDuration * 0.5f);

                // Use a custom easing for smooth shrinking
                float easedProgress = EaseOutCubic(progress);

                transitionCircle.rectTransform.localScale = Vector3.Lerp(startScale, endScale, easedProgress);

                yield return null;
            }

            // Ensure the circle is fully shrunk at the end
            transitionCircle.rectTransform.localScale = endScale;

            // Hide the transition canvas after the transition completes
            transitionCanvas.SetActive(false);
        }

        // Small delay to ensure canvas is hidden before calling completion
        yield return new WaitForSeconds(0.05f);

        // Call the completion callback
        onComplete?.Invoke();

        // End the transition state after a brief delay
        yield return new WaitForSeconds(0.05f);
        EndTransition();
    }

    // Easing function for smooth animation
    private float EaseOutCubic(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(1f - t, 3);
    }

}