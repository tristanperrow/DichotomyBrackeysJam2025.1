using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Task_Captcha : Task
{

    public bool taskOpen = false;

    // settings

    // Task Activation


    // UI
    private VisualElement interiorContainer;

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        UIManager.Instance.OnTaskClosed.AddListener(CloseTask);
        taskOpen = true;


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
