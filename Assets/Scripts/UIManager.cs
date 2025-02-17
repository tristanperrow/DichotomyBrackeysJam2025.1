using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // stored elements
    private VisualElement _tooltipContainer;
    private Label _tooltipLabel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            //
        }
        InitGameHUD();
    }

    private void InitGameHUD()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _tooltipContainer = root.Q<VisualElement>("tooltip-container");
        _tooltipLabel = _tooltipContainer.Q<Label>("tooltip-label");

        _tooltipContainer.style.display = DisplayStyle.None;
    }
}
