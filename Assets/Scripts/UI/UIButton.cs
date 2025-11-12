using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/UIButton")]
public class UIButton : Selectable, IPointerClickHandler
{
    [Header("Interaction")]
    [Tooltip("The event that is triggered when the button is clicked.")]
    public UnityEvent onClick = new UnityEvent();

    [Header("Sound Effects")]
    [Tooltip("Assign an AudioClip here to override the default button hover sound.")]
    public AudioClip overrideHoverSound;

    [Tooltip("Assign an AudioClip here to override the default button click sound.")]
    public AudioClip overrideClickSound;
    
    private bool isHovering = false;
    private bool isPressed = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        DoStateTransition(currentSelectionState, true);
    }
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if (state == SelectionState.Highlighted && !isPressed)
        {
            PlayHoverSound();
        }
    }
    
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;
        PlayClickSound();
        onClick.Invoke();
    }

    #region Sound Playback Logic

    private void PlayHoverSound()
    {
        if (!interactable) return;

        if (overrideHoverSound != null)
        {
            AudioManager.Instance?.PlaySFX(overrideHoverSound);
        }
        else
        {
            AudioManager.Instance?.PlayButtonHoverSound();
        }
    }

    private void PlayClickSound()
    {
        if (!interactable) return;

        if (overrideClickSound != null)
        {
            AudioManager.Instance?.PlaySFX(overrideClickSound);
        }
        else
        {
            AudioManager.Instance?.PlayButtonClickSound();
        }
    }

    #endregion
}