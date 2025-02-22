using UnityEngine;
using UnityEngine.UIElements;

public class WinUIManager : MonoBehaviour
{

    private Button continueButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        continueButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>();

        continueButton.clicked += OnClick;
    }

    private void OnClick()
    {
        SceneTransitionManager.Instance.LoadScene(0);
    }
}
