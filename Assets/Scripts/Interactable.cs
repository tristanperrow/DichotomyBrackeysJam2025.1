using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    public string interactionPrompt = "Interact";
    public Vector2 interactionPosition = Vector2.zero;

    public UnityEvent OnInteracted;

    public virtual void Interact()
    {
        OnInteracted?.Invoke();
    }

    public virtual void ShowPrompt()
    {
        // show tooltip here?
    }

    public virtual void HidePrompt()
    {
        // hide tooltip here?
    }
}