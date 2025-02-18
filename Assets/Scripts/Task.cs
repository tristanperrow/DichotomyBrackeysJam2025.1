using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class Task : Interactable
{
    [Header("Task Settings")]
    public TaskData data;

    [Header("Task Events")]
    public UnityEvent OnTaskCompleted;
    public UnityEvent OnTaskFailed;

    [Header("Task State")]
    public float timeRemaining;
    public bool isActive = false;

    private void Awake()
    {
        // set the prompt from interactable
        interactionPrompt = data.taskInteractionPrompt;
        interactionPosition = data.taskInteractionPromptPosition;
        // modify task settings
        timeRemaining = data.timeUntilFail;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            isActive = false;
            FailTask();
        }
    }

    public void CompleteTask()
    {
        isActive = false;
        OnTaskCompleted?.Invoke();
    }

    public void FailTask()
    {
        isActive = false;
        OnTaskFailed?.Invoke();
    }

    public override void Interact()
    {
        OpenTask();
        OnInteracted?.Invoke();
    }

    public abstract void OpenTask();
}

[CreateAssetMenu(fileName = "Task", menuName = "Game/ScriptableObjects/Tasks")]
public class TaskData : ScriptableObject
{
    public string taskName = "Default Task";
    [TextArea(5, 10)]
    public string taskDescription = "";
    public string taskInteractionPrompt = "Interact";
    public Vector2 taskInteractionPromptPosition = Vector2.zero;

    [Range(5f, 60f)]
    public float timeUntilFail = 20; // in seconds

    [Header("Task UI")]
    public VisualTreeAsset taskUI;
}