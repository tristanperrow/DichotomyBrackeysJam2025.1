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
    private bool[] switchState = new bool[15];

    // UI
    private VisualElement switchesContainer;

    // audio
    [SerializeField] private AudioSource flickOnSound;
    [SerializeField] private AudioSource flickOffSound;

    private void TryCompleteTask()
    {
        var allOn = true;
        foreach (var state in switchState)
        {
            if (state == false)
            {
                allOn = false;
                break;
            }
        }

        if (allOn)
        {
            CompleteTask();
            //UIManager.Instance.HideTask();
            //CloseTask();
        }
    }

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        UIManager.Instance.OnTaskClosed.AddListener(CloseTask);
        taskOpen = true;

        switchesContainer = UIManager.Instance._taskContainer.Q<VisualElement>("switches");

        int i = 0;
        foreach (VisualElement element in switchesContainer.Children())
        {
            if (switchState[i])
                element.style.backgroundImage = new StyleBackground(onButton);
            else
                element.style.backgroundImage = new StyleBackground(offButton);

            int j = i;
            element.AddManipulator(new Clickable((evt) =>
            {
                if (switchesContainer.panel.focusController.focusedElement == element)
                {
                    switchState[j] = !switchState[j];
                    if (switchState[j])
                    {
                        element.style.backgroundImage = new StyleBackground(onButton);
                        flickOnSound.Play();
                    }
                    else
                    {
                        element.style.backgroundImage = new StyleBackground(offButton);
                        flickOffSound.Play();
                    }

                    TryCompleteTask();
                };
            }));

            i++;
        }
    }

    protected override void OnActivatedTask()
    {
        // on activation
        for (int i = 0; i < switchState.Length; i++)
        {
            if (Random.Range(0, 2) == 0)
                switchState[i] = true;
            else
                switchState[i] = false;
        }
    }

    public void CloseTask()
    {
        UIManager.Instance.OnTaskClosed.RemoveListener(CloseTask);
        taskOpen = false;
    }
}
