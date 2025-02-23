using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Task_Engine : Task
{

    public static Task_Engine Instance;

    public float engineTemp = 120f;
    public float freezePoint = 32f;
    public float boilPoint = 212f;

    public bool cooling = true;
    public bool taskOpen = false;

    public Gradient tempGradient;
    public Light2D globalLight;

    public float tempMult = 1.0f;

    // GAME OBJECTS


    // UI
    private Label tl_label;
    private Label tc_label;
    private Label tr_label;
    private Label bl_label;
    private Label bc_label;
    private Label br_label;

    private Button coolButton;
    private Button heatButton;

    // audio
    [SerializeField] private AudioSource buttonSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // no two main engine tasks
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActivateTask();
    }

    public void Update()
    {
        if (!isActive) return;
        // make sure this task never ends via time
        timeRemaining = int.MaxValue;

        var df = (cooling) ? -1 : 1;
        var dt = Time.deltaTime;

        engineTemp += df * dt * 2 * tempMult;

        UpdateTaskLabels();
        UpdateGlobalLight();

        if (engineTemp < freezePoint || engineTemp > boilPoint)
        {
            // fail the task
            StartCoroutine(SceneTransitionManager.Instance.LoadScene(2));
        }
    }

    public void UpdateTaskLabels()
    {
        string str = $"{engineTemp:000.000}";
        int[] digits = new int[6];
        for (int i = 0; i <= 6; i++)
        {
            if (i < 3)
            {
                digits[i] = int.Parse(str[i].ToString());
            }
            else if (i > 3)
            {
                digits[i-1] = int.Parse(str[i].ToString());
            }
        }
        // updating UI
        try
        {
            tl_label.text = "" + digits[0];
            tc_label.text = "" + digits[1];
            tr_label.text = "" + digits[2];
            bl_label.text = "" + digits[3];
            bc_label.text = "" + digits[4];
            br_label.text = "" + digits[5];
        }
        catch
        {
            //Debug.Log("Some error occured trying to update numbers...");
        }

        // updating world sprite

    }

    public void UpdateGlobalLight()
    {
        if (globalLight == null) return;

        float normalizedTemp = Mathf.InverseLerp(freezePoint, boilPoint, engineTemp);

        Color lightColor = tempGradient.Evaluate(normalizedTemp);

        globalLight.color = lightColor;
    }

    private void Cool()
    {
        cooling = true;
        buttonSound.Play();
    }

    private void Heat()
    {
        cooling = false;
        buttonSound.Play();
    }

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        UIManager.Instance.OnTaskClosed.AddListener(CloseTask);
        taskOpen = true;

        // UI
        var numbers = UIManager.Instance._taskContainer.Q<VisualElement>("numbers");
        tl_label = numbers.Q<Label>("top-left");
        tc_label = numbers.Q<Label>("top-center");
        tr_label = numbers.Q<Label>("top-right");
        bl_label = numbers.Q<Label>("bot-left");
        bc_label = numbers.Q<Label>("bot-center");
        br_label = numbers.Q<Label>("bot-right");

        var buttons = UIManager.Instance._taskContainer.Q<VisualElement>("buttons");
        coolButton = buttons.Q<Button>("cool-button");
        heatButton = buttons.Q<Button>("heat-button");

        coolButton.clicked += Cool;
        heatButton.clicked += Heat;
    }

    protected override void OnActivatedTask()
    {
        // do any task related logic here.
    }

    public void CloseTask()
    {
        UIManager.Instance.OnTaskClosed.RemoveListener(CloseTask);
        taskOpen = false;

        // remove ui vars / events
        tl_label = null;
        tc_label = null;
        tr_label = null;
        bl_label = null;
        bc_label = null;
        br_label = null;

        coolButton.clicked -= Cool;
        heatButton.clicked -= Heat;

        coolButton = null;
        heatButton = null;

        // update world sprite?


    }
}
