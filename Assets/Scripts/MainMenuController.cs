using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{

    // sounds
    [SerializeField] private AudioSource startup;
    [SerializeField] private AudioSource click1;
    [SerializeField] private AudioSource click2;
    [SerializeField] private AudioSource click3;

    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string masterVolumeP = "Master";
    [SerializeField] private string sfxVolumeP = "SFX";
    [SerializeField] private string ambianceVolumeP = "Ambiance";

    // state


    // ui definitions
    private VisualElement mainMenuContainer;
    private VisualElement optionsMenuContainer;

    private Button continueButton;
    private Button newGameButton;
    private Button optionsButton;
    private Button exitButton;

    private Label dayLabel;

    private Slider masterVolumeSlider;
    private Slider sfxVolumeSlider;
    private Slider ambianceVolumeSlider;

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

        dayLabel = root.Q<Label>("day-label");

        masterVolumeSlider = optionsMenuContainer.Q<Slider>("master-volume-slider");
        sfxVolumeSlider = optionsMenuContainer.Q<Slider>("sfx-volume-slider");
        ambianceVolumeSlider = optionsMenuContainer.Q<Slider>("ambiance-volume-slider");

        applyButton = optionsMenuContainer.Q<Button>("apply-button");
        backButton = optionsMenuContainer.Q<Button>("back-button");

        // events
        continueButton.clicked += ContinueGame;
        newGameButton.clicked += StartGame;
        optionsButton.clicked += OpenOptionsMenu;
        exitButton.clicked += QuitGame;

        // options events
        applyButton.clicked += ApplyOptions;
        backButton.clicked += CloseOptionsMenu;
    }

    public void Start()
    {
        SceneTransitionManager.Instance.GetComponent<UIDocument>().sortingOrder = 0;

        // make sure the continue button is active if it should be
        if (PlayerSave.Instance.day > 0)
        {
            continueButton.RemoveFromClassList("menuButtonInactive");
            continueButton.AddToClassList("menuButton");

            dayLabel.style.fontSize = 30;
            if (PlayerSave.Instance.day < 2)
            {
                // TODO: different day names for the training days?
                dayLabel.text = "day " + (PlayerSave.Instance.day + 1);
            } else
            {
                dayLabel.text = "day " + (PlayerSave.Instance.day + 1);
            }
        }
        else
        {
            continueButton.RemoveFromClassList("menuButton");
            continueButton.AddToClassList("menuButtonInactive");

            dayLabel.style.fontSize = 0;
        }

        // initial volumes
        SetVolume(masterVolumeP, PlayerSave.Instance.masterVolume);
        SetVolume(sfxVolumeP, PlayerSave.Instance.sfxVolume);
        SetVolume(ambianceVolumeP, PlayerSave.Instance.ambianceVolume);
    }

    public void ContinueGame()
    {
        if (PlayerSave.Instance.day == 0) return;
        PlayRandomClickSound();
        // load the engine room
        StartCoroutine(SceneTransitionManager.Instance.LoadScene(1));
    }

    public void StartGame()
    {
        PlayRandomClickSound();
        // load the engine room
        PlayerSave.Instance.day = 0;
        StartCoroutine(SceneTransitionManager.Instance.LoadScene(1));
    }

    public void OpenOptionsMenu()
    {
        // load options
        masterVolumeSlider.value = PlayerSave.Instance.masterVolume;
        sfxVolumeSlider.value = PlayerSave.Instance.sfxVolume;
        ambianceVolumeSlider.value = PlayerSave.Instance.ambianceVolume;

        // show menu
        PlayRandomClickSound();
        mainMenuContainer.style.display = DisplayStyle.None;
        optionsMenuContainer.style.display = DisplayStyle.Flex;
    }

    public void ApplyOptions()
    {
        // save the options
        PlayerSave.Instance.masterVolume = masterVolumeSlider.value;
        PlayerSave.Instance.sfxVolume = sfxVolumeSlider.value;
        PlayerSave.Instance.SaveSettings();

        // change actual mixer settings here
        SetVolume(masterVolumeP, PlayerSave.Instance.masterVolume);
        SetVolume(sfxVolumeP, PlayerSave.Instance.sfxVolume);
        SetVolume(ambianceVolumeP, PlayerSave.Instance.ambianceVolume);

        // then play random click sound
        PlayRandomClickSound();
    }

    public void CloseOptionsMenu()
    {
        PlayRandomClickSound();
        mainMenuContainer.style.display = DisplayStyle.Flex;
        optionsMenuContainer.style.display = DisplayStyle.None;
    }

    public void QuitGame()
    {
        PlayRandomClickSound();
        PlayerSave.Instance.Save();
        Application.Quit();
    }

    public void PlayRandomClickSound()
    {
        int ind = Random.Range(0, 3);
        switch (ind)
        {
            case 0:
                click1.Play();
                break;
            case 1:
                click2.Play();
                break;
            case 2:
                click3.Play();
                break;
        }
    }

    private void SetVolume(string parameter, float volumeL)
    {
        float volumeDB;
        if (volumeL > 0)
        {
            volumeDB = Mathf.Log10(volumeL) * 20;
        } else
        {
            volumeDB = -80;
        }

        masterMixer.SetFloat(parameter, volumeDB);
    }
}
