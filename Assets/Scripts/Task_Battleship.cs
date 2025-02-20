using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Task_Battleship : Task
{

    public bool taskOpen = false;

    // settings
    [SerializeField] private Texture2D onButton;
    [SerializeField] private Texture2D offButton;

    // Task Activation
    

    // UI
    private VisualElement taskGrid;

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        UIManager.Instance.OnTaskClosed.AddListener(CloseTask);
        taskOpen = true;

        taskGrid = UIManager.Instance._taskContainer.Q<VisualElement>("task-grid");

        foreach (Button element in taskGrid.Children())
        {
            
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
