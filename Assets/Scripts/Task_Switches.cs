using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Task_Switches : Task
{

    public bool taskOpen = false;

    // settings
    [SerializeField] private Texture2D onButton;
    [SerializeField] private Texture2D offButton;

    // Task Activation
    private bool[] switchState = new bool[40];

    // UI
    private VisualElement switchesContainer;

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        UIManager.Instance.OnTaskClosed.AddListener(CloseTask);
        taskOpen = true;

        switchesContainer = UIManager.Instance._taskContainer.Q<VisualElement>("switches");

        foreach (VisualElement element in switchesContainer.Children())
        {
            element.AddManipulator(new Clickable((evt) =>
            {
                if (switchesContainer.panel.focusController.focusedElement == element)
                {
                    
                }
            }));
        }
    }

    protected override void OnActivatedTask()
    {
        // on activation
    }

    public void CloseTask()
    {
        UIManager.Instance.OnTaskClosed.RemoveListener(CloseTask);
        taskOpen = false;
    }
}
