using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{

    // scene

    // ui definitions
    private Button continueButton;
    private Button newGameButton;
    private Button optionsButton;
    private Button button;

    // transitions

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        continueButton = root.Q<Button>("continue-button");
        newGameButton = root.Q<Button>("new-game-button");
        optionsButton = root.Q<Button>("options-button");
        button = root.Q<Button>("button");

        // events
        newGameButton.clicked += StartGame;
    }

    public void StartGame()
    {
        // load the engine room
        StartCoroutine(SceneTransitionManager.Instance.LoadScene(1));
    }
}
