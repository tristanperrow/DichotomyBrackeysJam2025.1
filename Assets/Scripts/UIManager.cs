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

    public void ShowTooltip(string text, Vector2 worldPosition)
    {
        // check if tasks are open (don't show if they are)

        // show text
        _tooltipLabel.text = text;
        _tooltipContainer.style.display = DisplayStyle.Flex;
        UpdateTooltipPosition(worldPosition);
    }

    public void HideTooltip()
    {
        _tooltipContainer.style.display = DisplayStyle.None;
    }

    // moves panel to screen position from world position
    private void UpdateTooltipPosition(Vector2 worldPosition)
    {
        // get screen position for the panel
        var screenPos = RuntimePanelUtils.CameraTransformWorldToPanel(
            _tooltipContainer.panel,
            worldPosition,
            Camera.main
        );

        // TODO: Not working?
        // centers the tooltip at the world position
        /*
        float tw = _tooltipContainer.resolvedStyle.width;
        float th = _tooltipContainer.resolvedStyle.height;

        screenPos.x -= tw / 2;
        screenPos.y -= th / 2;

        Debug.Log(tw + "-" + th);
        */

        // finally set position
        _tooltipContainer.transform.position = screenPos;
    }
}
