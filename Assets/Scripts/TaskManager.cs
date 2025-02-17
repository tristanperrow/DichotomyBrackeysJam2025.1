using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskManager : MonoBehaviour
{
    public List<Task> ActiveTasks { get; private set; }
    public UnityEvent<Task> OnTaskFailed;

    private void Start()
    {
        OnTaskFailed.AddListener(TaskFailed);
    }

    private void TaskFailed(Task task)
    {
        Debug.Log("You have failed:" + task.data);
    } 
}
