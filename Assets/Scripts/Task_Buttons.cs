using UnityEngine;
using UnityEngine.UIElements;

public class Task_Buttons : Task
{

    public bool taskOpen = false;

    // settings
    [SerializeField] private Texture2D onButton;
    [SerializeField] private Texture2D offButton;

    // Task Activation
    private bool[] buttonState = new bool[40];

    // UI
    private VisualElement buttonsContainer;

    // Audio
    [SerializeField] private AudioSource pushOnSound;
    [SerializeField] private AudioSource pushOffSound;

    private void TryCompleteTask()
    {
        var allOn = true;
        foreach (var state in buttonState)
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

        buttonsContainer = UIManager.Instance._taskContainer.Q<VisualElement>("buttons");

        // start buttons in on or off state
        int i = 0;
        foreach (Button button in buttonsContainer.Children())
        {
            if (buttonState[i])
                button.style.backgroundImage = new StyleBackground(onButton);
            else
                button.style.backgroundImage = new StyleBackground(offButton);

            int j = i;
            button.clicked += () =>
            {
                buttonState[j] = !buttonState[j];
                if (buttonState[j])
                {
                    button.style.backgroundImage = new StyleBackground(onButton);
                    pushOnSound.Play();
                }
                else
                {
                    button.style.backgroundImage = new StyleBackground(offButton);
                    pushOffSound.Play();
                }
                TryCompleteTask();
            };

            i++;
        }
    }

    protected override void OnActivatedTask()
    {
        // on activation
        for (int i = 0; i < buttonState.Length; i++)
        {
            if (Random.Range(0, 2) == 0)
                buttonState[i] = true;
            else
                buttonState[i] = false;
        }
    }

    public void CloseTask()
    {
        UIManager.Instance.OnTaskClosed.RemoveListener(CloseTask);
        taskOpen = false;
    }
}
