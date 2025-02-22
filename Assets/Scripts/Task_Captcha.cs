using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Task_Captcha : Task
{

    public bool taskOpen = false;

    // settings

    // Task Activation
    private string captchaText;

    // UI
    private VisualElement interiorContainer;
    private Label captchaLabel;
    private TextField captchaInput;
    private Button submitButton;

    // Audio
    [SerializeField] private AudioSource successSound;
    [SerializeField] private AudioSource failureSound;

    private void OnSubmitButtonClicked()
    {
        if (captchaInput.value == captchaText)
        {
            // task success
            successSound.Play();
            CompleteTask();
            CloseTask();
            UIManager.Instance.HideTask();
        }
        else
        {
            // failed captcha, regenerate new captcha
            failureSound.Play();
            GenerateCaptcha();
        }
    }

    private void GenerateCaptcha()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        captchaText = "";

        for (int i = 0; i < 6; i++)
        {
            captchaText += chars[Random.Range(0, chars.Length)];
        }

        if (captchaLabel != null)
        {
            captchaLabel.text = captchaText;
        } 
    }

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        UIManager.Instance.OnTaskClosed.AddListener(CloseTask);
        taskOpen = true;

        interiorContainer = UIManager.Instance._taskContainer.Q<VisualElement>("interior-container");

        captchaLabel = interiorContainer.Q<Label>("captcha-label");
        captchaInput = interiorContainer.Q<TextField>("captcha-text-input");
        submitButton = interiorContainer.Q<Button>("submit-button");

        submitButton.clicked += OnSubmitButtonClicked;

        // initialize ui
        captchaLabel.text = captchaText;
    }

    protected override void OnActivatedTask()
    {
        // on activation
        GenerateCaptcha();
    }

    public void CloseTask()
    {
        UIManager.Instance.OnTaskClosed.RemoveListener(CloseTask);
        taskOpen = false;

        // remove ui / events
        submitButton.clicked -= OnSubmitButtonClicked;

        interiorContainer = null;
        captchaLabel = null;
        captchaInput = null;
        submitButton = null;
    }
}
