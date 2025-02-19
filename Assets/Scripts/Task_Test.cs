using UnityEngine;
using UnityEngine.UIElements;

public class Task_Test : Task
{

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        Debug.Log("Task has been opened!");
    }

    protected override void OnActivatedTask()
    {
        // do any task related logic here.
        Debug.Log("Task has been activated!");
    }
}
