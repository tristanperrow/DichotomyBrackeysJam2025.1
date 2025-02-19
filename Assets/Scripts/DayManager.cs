using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DayManager : MonoBehaviour
{

    static DayManager Instance;

    public float endTime = 36000;   // in seconds ( * 120)

    [Header("Task Settings")]
    public int tasksPerHour = 3;
    public List<Task> tasks = new List<Task>();

    [Header("Events")]
    public UnityEvent dayOverEvent;

    // time the day started
    private float _dayTime;
    // time the day will end
    private float _endTime = 36000; // in seconds ( * 120)
    // if the day has ended
    private bool _hasDayEnded = false;


    // task properties
    private float _timeUntilNextTask = 0f;
    private List<Task> _activeTasks = new List<Task>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogWarning("There are multiple DayManager classes.");
        }

        foreach (var task in tasks)
        {
            task.OnTaskCompleted.AddListener(TaskCompleted);
            task.OnTaskFailed.AddListener(TaskFailed);
        }
    }

    private void Start()
    {
        _dayTime = 0;
        _endTime = endTime;
        _hasDayEnded = false;
    }

    private void Update()
    {
        _dayTime += Time.deltaTime * 120;
        if (_dayTime > _endTime)
        {
            if (!_hasDayEnded)
            {
                _hasDayEnded = true;
                dayOverEvent.Invoke();
            }
        }
        else
        {
            if (_timeUntilNextTask <= 0)
            {
                if (TrySpawnTask())
                    _timeUntilNextTask = tasksPerHour * 3600;
            } 
            else
            {
                _timeUntilNextTask -= Time.deltaTime * 120;
            }
        }
    }

    #region - Tasks -

    public bool TrySpawnTask()
    {
        // if all available tasks are currently active, return false
        if (_activeTasks.Count == tasks.Count)
            return false;
        foreach (Task task in tasks)
        {
            bool inActiveTasks = false;
            foreach (Task aTask in _activeTasks)
            {
                if (aTask.ID == task.ID)
                    inActiveTasks = true;
            }
            if (inActiveTasks == false)
            {
                _activeTasks.Add(task);
                task.ActivateTask();
                return true;
            }
        }

        return false;
    }

    public void TaskCompleted(Task task)
    {
        // remove task from active tasks
        _activeTasks.Remove(task);
        // do other task completed related stuff
        Debug.Log("Task Completed: " + task.data.taskName);
    }

    public void TaskFailed(Task task)
    {
        // remove task from active tasks
        _activeTasks.Remove(task);
        // do other task failed related stuff
        Debug.Log("Task Failed: " + task.data.taskName);
    }

    #endregion

    public string GetDayTimeString()
    {
        int hours = Mathf.FloorToInt(_dayTime / 3600);
        int minutes = Mathf.FloorToInt((_dayTime % 3600) / 60);
        return $"{hours:00}:{minutes:00}";
    }
}
