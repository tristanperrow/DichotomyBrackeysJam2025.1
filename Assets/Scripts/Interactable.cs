using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    public string interactionPrompt = "Interact [E]";
    public UnityEvent OnInteracted;

    public virtual void Interact()
    {
        OnInteracted?.Invoke();
    }

    public virtual void ShowPrompt()
    {
        // Show tooltip
    }

    public virtual void HidePrompt()
    {
        // Hide tooltip
    }
}