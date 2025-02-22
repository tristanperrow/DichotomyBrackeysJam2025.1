using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Task_Battleship : Task
{

    public bool taskOpen = false;

    // settings
    [SerializeField] private Vector2 offset = new Vector2(-25, -30);

    [SerializeField] private Texture2D missTex;
    [SerializeField] private Texture2D hitTex;
    [SerializeField] private Texture2D battleshipTex;

    // Task Activation
    private bool[] battleshipPositions = new bool[25];
    private bool[] prevAttempts = new bool[25];

    private bool isBattleshipVertical;
    private List<int> battleshipIndices = new List<int>();

    // UI
    private VisualElement taskGrid;
    private VisualElement crosshair;
    private VisualElement battleship;

    // audio
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource missSound;
    [SerializeField] private AudioSource completeSound;

    // crosshair code
    private void MouseEnterCell(MouseEnterEvent e)
    {
        if (crosshair == null) return;

        VisualElement button = e.target as VisualElement;

        crosshair.style.left = button.worldBound.x + offset.x;
        crosshair.style.top = button.worldBound.y + offset.y;
    }

    private void CellClicked(Button button, int index)
    {
        if (!taskOpen) return;
        if (index < 0 || index >= 25) return;
        if (prevAttempts[index]) return;

        // note that cell has now been clicked
        prevAttempts[index] = true;

        // check if position is a battleship
        if (battleshipPositions[index])
        {
            // hit ship
            button.style.backgroundImage = new StyleBackground(hitTex);
            hitSound.Play();

            // check if all ship positions were hit
            bool allHit = true;
            foreach (int ind in battleshipIndices)
            {
                if (!prevAttempts[ind])
                {
                    allHit = false;
                    break;
                }
            }

            // if all were hit, complete the task
            if (allHit)
            {
                completeSound.Play();
                CompleteTask();
                ShowBattleship();
            }
        }
        else
        {
            // missed ship
            button.style.backgroundImage = new StyleBackground(missTex);
            missSound.Play();
        }
    }

    private void ShowBattleship()
    {
        if (battleship == null) return;

        Button cell1 = taskGrid.Children().ElementAt(battleshipIndices[0]) as Button;
        Rect cellBounds = cell1.worldBound;

        if (isBattleshipVertical)
        {
            battleship.style.left = cellBounds.x - 110;
            battleship.style.top = cellBounds.y;

            battleship.style.rotate = new Rotate(-90);
        }
        else
        {
            battleship.style.left = cellBounds.x - 25;
            battleship.style.top = cellBounds.y - 72;

            battleship.style.rotate = new Rotate(0);
        }

        battleship.style.display = DisplayStyle.Flex;
    }

    public override void OpenTask()
    {
        UIManager.Instance.ShowTask(data.taskUI);
        UIManager.Instance.OnTaskClosed.AddListener(CloseTask);
        taskOpen = true;

        taskGrid = UIManager.Instance._taskContainer.Q<VisualElement>("task-grid");

        int i = 0;
        foreach (Button element in taskGrid.Children())
        {
            element.RegisterCallback<MouseEnterEvent>(MouseEnterCell);

            int j = i;
            // register click callback
            element.clicked += () =>
            {
                CellClicked(element, j);
            };

            // increment element index
            i++;
        }

        crosshair = UIManager.Instance._taskContainer.Q<VisualElement>("crosshair");

        battleship = UIManager.Instance._taskContainer.Q<VisualElement>("battleship");
    }

    protected override void OnActivatedTask()
    {
        // on activation
        battleshipPositions = new bool[25];
        prevAttempts = new bool[25];
        battleshipIndices.Clear();

        // place battleship
        isBattleshipVertical = UnityEngine.Random.Range(0, 2) == 0;

        if (isBattleshipVertical)
        {
            int sc = UnityEngine.Random.Range(0, 5); // starting column
            int sr = UnityEngine.Random.Range(0, 3); // starting row

            for (int i = 0; i < 3; i++)
            {
                int ind = (sr + i) * 5 + sc;
                battleshipPositions[ind] = true;
                battleshipIndices.Add(ind);
            }
        }
        else
        {
            int sc = UnityEngine.Random.Range(0, 3);
            int sr = UnityEngine.Random.Range(0, 5);

            for (int i = 0; i < 3; i++)
            {
                int ind = sr * 5 + (sc + i);
                battleshipPositions[ind] = true;
                battleshipIndices.Add(ind);
            }
        }
    }

    public void CloseTask()
    {
        UIManager.Instance.OnTaskClosed.RemoveListener(CloseTask);
        taskOpen = false;

        battleship.style.display = DisplayStyle.None;

        // remove ui / events
        crosshair = null;

        foreach (Button element in taskGrid.Children())
        {
            element.UnregisterCallback<MouseEnterEvent>(MouseEnterCell);
        }

        battleship = null;
    }
}