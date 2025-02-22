using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // stored elements
    private VisualElement _tooltipContainer;
    private Label _tooltipLabel;

    public VisualElement _taskContainer;

    public VisualElement _recCircle;
    public Label _timeLabel;

    // state
    public bool inTask = false;

    public UnityEvent OnTaskClosed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // if a second instance has spawned?
            return;
        }

        InitGameHUD();
        inTask = false;

        // reset scene transition manager sorting order
        SceneTransitionManager.Instance.GetComponent<UIDocument>().sortingOrder = 0;
    }

    private void InitGameHUD()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        // tooltip
        _tooltipContainer = root.Q<VisualElement>("tooltip-container");
        _tooltipLabel = _tooltipContainer.Q<Label>("tooltip-label");

        _tooltipContainer.style.display = DisplayStyle.None;

        // tasks
        _taskContainer = root.Q<VisualElement>("task-container");
        _taskContainer.AddManipulator(new Clickable((evt) =>
        {
            // check if the element is focused
            if (_taskContainer.panel.focusController.focusedElement == _taskContainer)
            {
                HideTask();
            }
        }));    // clicking outside the task UI will close a task
        _taskContainer.focusable = true;

        _taskContainer.style.display = DisplayStyle.None;

        // security cam
        _recCircle = root.Q<VisualElement>("circle");
        _timeLabel = root.Q<Label>("time");
    }

    public void ShowTooltip(string text, Vector2 worldPosition)
    {
        // check if tasks are open (don't show if they are)
        if (inTask)
        {
            HideTooltip();
            return;
        }
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

    // shows the task
    public void ShowTask(VisualTreeAsset taskUI)
    {
        // make sure not already in a task
        if (inTask) return;
        // clone the task into _taskContainer
        taskUI.CloneTree(_taskContainer);
        // enable the _taskContainer ui
        _taskContainer.style.display = DisplayStyle.Flex;
        // set state of UI to in task
        inTask = true;
    }

    // removes the task from task container
    public void HideTask()
    {
        // don't run if not in a task
        if (!inTask) return;
        // fire closed event
        OnTaskClosed.Invoke();
        // delete all children of _taskContainer
        _taskContainer.Clear();
        // disable the _taskContainer ui
        _taskContainer.style.display = DisplayStyle.None;
        inTask = false;
    }

    // update timer HUD
    public void UpdateSecurityCamHud(string timeOfDay, bool circleShowing)
    {
        if (timeOfDay == null || _recCircle == null || _timeLabel == null ) return;

        if (circleShowing)
        {
            _recCircle.style.visibility = Visibility.Visible;
        }
        else
        {
            _recCircle.style.visibility = Visibility.Hidden;
        }

        _timeLabel.text = timeOfDay;
    }
}
