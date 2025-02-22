using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{

    // state


    // ui definitions
    private VisualElement mainMenuContainer;
    private VisualElement optionsMenuContainer;

    private Button continueButton;
    private Button newGameButton;
    private Button optionsButton;
    private Button exitButton;

    private Button applyButton;
    private Button backButton;

    // transitions

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        mainMenuContainer = root.Q<VisualElement>("main-menu");
        optionsMenuContainer = root.Q<VisualElement>("options-menu");

        continueButton = root.Q<Button>("continue-button");
        newGameButton = root.Q<Button>("new-game-button");
        optionsButton = root.Q<Button>("options-button");
        exitButton = root.Q<Button>("button");

        applyButton = optionsMenuContainer.Q<Button>("apply-button");
        backButton = optionsMenuContainer.Q<Button>("back-button");

        // events
        //continueButton.clicked += ContinueGame;
        newGameButton.clicked += StartGame;
        optionsButton.clicked += OpenOptionsMenu;
        exitButton.clicked += QuitGame;

        // options events
        backButton.clicked += CloseOptionsMenu;
    }

    public void Start()
    {
        SceneTransitionManager.Instance.GetComponent<UIDocument>().sortingOrder = 0;
    }

    public void StartGame()
    {
        // load the engine room
        StartCoroutine(SceneTransitionManager.Instance.LoadScene(1));
    }

    public void OpenOptionsMenu()
    {
        mainMenuContainer.style.display = DisplayStyle.None;
        optionsMenuContainer.style.display = DisplayStyle.Flex;
    }

    public void CloseOptionsMenu()
    {
        mainMenuContainer.style.display = DisplayStyle.Flex;
        optionsMenuContainer.style.display = DisplayStyle.None;
    }

    public void QuitGame()
    {
        PlayerPrefs.SetInt("Day", PlayerSave.Instance.day);
        Application.Quit();
    }
}
