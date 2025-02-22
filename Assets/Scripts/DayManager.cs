using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TaskDayPair
{
    public Task task;
    public int unlockDay;
}

[System.Serializable]
public class GameObjDayPair
{
    public List<GameObject> gameObjs;
    public int unlockDay;
}

public class DayManager : MonoBehaviour
{

    public static DayManager Instance;

    public float endTime = 36000;   // in seconds ( * 120) [36000 is standard]
    public float trainingEndTime = 14400; // in seconds * 120 [14400 = 4 hours]

    [Header("Task Settings")]
    public int tasksPerHour = 3;
    public List<TaskDayPair> taskDayPairs = new List<TaskDayPair>();
    public List<GameObjDayPair> gameObjDayPairs = new List<GameObjDayPair>();

    [Header("Events")]
    public UnityEvent dayOverEvent;

    // time the day started
    public float _dayTime;
    // time the day will end
    private float _endTime = 36000; // in seconds ( * 120)
    // if the day has ended
    private bool _hasDayEnded = false;

    // task properties
    private List<Task> tasks = new List<Task>();
    private float _timeUntilNextTask = 0f;
    private List<Task> _activeTasks = new List<Task>();

    // security cam ui
    private float _circleShowTime = 1f;
    private float _circleHideTime = 0.5f;
    private float _lastStateTime = 1f;
    private bool _circleState;

    // ambiance sound
    [SerializeField] private AudioSource longACSound;
    [SerializeField] private AudioSource longFanSound;
    [SerializeField] private AudioSource shortGarageSound;
    [SerializeField] private AudioSource shortPumpSound;
    [SerializeField] private AudioSource shortCreakingSound;

    private float _nextAmbTime = 30f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // destroy duplicate instances
            Debug.LogWarning("There are multiple DayManager classes.");
            Destroy(gameObject);
        }

        // enable gameObjects based on day
        var curDay = PlayerSave.Instance.day;
        foreach (var pair in gameObjDayPairs)
        {
            if (pair.unlockDay <= curDay)
            {
                foreach (var gameObj in pair.gameObjs)
                {
                    gameObj.SetActive(true);
                }
            }
        }

        // add tasks to tasks list based on day
        tasks.Clear();
        foreach (var pair in taskDayPairs)
        {
            if (pair.unlockDay <= curDay)
            {
                tasks.Add(pair.task);
            }
        }
    }

    private void Start()
    {
        foreach (var task in tasks)
        {
            task.OnTaskCompleted.AddListener(TaskCompleted);
            task.OnTaskFailed.AddListener(TaskFailed);
        }

        _dayTime = 0;
        _endTime = endTime;
        _hasDayEnded = false;

        // start ambiance sounds
        longACSound.Play();
        longFanSound.Play();

        // initial ambiance timer
        _nextAmbTime = Random.Range(10f, 30f);
    }

    private void Update()
    {
        _lastStateTime -= Time.deltaTime;
        _dayTime += Time.deltaTime * 120;
        // first two days are shorter
        if (_dayTime > _endTime || (_dayTime > trainingEndTime && PlayerSave.Instance.day < 2))
        {
            if (!_hasDayEnded)
            {
                _hasDayEnded = true;
                dayOverEvent.Invoke();

                PlayerSave.Instance.day += 1;
                // go back to main menu
                StartCoroutine(SceneTransitionManager.Instance.LoadScene(3));
            }
        }
        else
        {
            if (_timeUntilNextTask <= 0)
            {
                if (TrySpawnTask())
                    _timeUntilNextTask = 3600 / tasksPerHour;
            } 
            else
            {
                _timeUntilNextTask -= Time.deltaTime * 120;
            }
        }

        // security cam code
        if (_lastStateTime < 0f && _circleState)
        {
            _lastStateTime = _circleHideTime;
            _circleState = false;
        }
        else if (_lastStateTime < 0f && !_circleState)
        {
            _lastStateTime = _circleShowTime;
            _circleState = true;
        }

        /* TODO: Increment in 15 minute periods
        var mins = Mathf.FloorToInt((_dayTime % 3600) / 60);
        if (mins > _timeIncrementRate + _lastIncrement || _timeIncrementRate < mins - 30)
        {
            _lastIncrement = Mathf.FloorToInt(mins / 15) * 15;
            UIManager.Instance.UpdateSecurityCamHud(GetDayTimeString(), _circleState);
        }
        */

        UIManager.Instance.UpdateSecurityCamHud(GetDayTimeString(), _circleState);

        // ambiance
        if (!_hasDayEnded)
        {
            _nextAmbTime -= Time.deltaTime;
            if (_nextAmbTime <= 0f)
            {
                PlayRandomAmbianceSound();
                _nextAmbTime = Random.Range(10, 30f);
            }
        }
    }

    #region - Tasks -

    public bool TrySpawnTask()
    {
        List<Task> availableTasks = new List<Task>();
        // check for available (non-active) tasks
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
                availableTasks.Add(task);
            }
        }

        if (availableTasks.Count > 0)
        {
            // get random task from available tasks
            int taskIndex = Random.Range(0, availableTasks.Count);
            Task task = availableTasks[taskIndex];
            // add task to active tasks, then return true
            _activeTasks.Add(task);
            task.ActivateTask();
            return true;
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

        Task_Engine.Instance.tempMult *= 1.5f;
    }

    #endregion

    public string GetDayTimeString()
    {
        int hours = Mathf.FloorToInt(_dayTime / 3600);
        int minutes = Mathf.FloorToInt(Mathf.FloorToInt((_dayTime % 3600) / 60) / 15) * 15;
        return $"{hours:00}:{minutes:00}";
    }

    public void PlayRandomAmbianceSound()
    {
        int soundIndex = Random.Range(0, 3);
        switch (soundIndex)
        {
            case 0:
                shortGarageSound.Play();
                break;
            case 1:
                shortPumpSound.Play();
                break;
            case 2:
                shortCreakingSound.Play();
                break;
        }

    }
}
