using UnityEngine;
using UnityEngine.UIElements;

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