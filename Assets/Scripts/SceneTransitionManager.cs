using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SceneTransitionManager : MonoBehaviour
{
    
    public static SceneTransitionManager Instance { get; private set; }

    // ui
    private VisualElement transitionsPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            return;
        }

        transitionsPanel = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("fade-transition");
    }

    public IEnumerator LoadScene(int buildIndex)
    {
        GetComponent<UIDocument>().sortingOrder = 100;
        yield return StartCoroutine(FadePanel(0f, 1f, 1f));

        SceneManager.LoadScene(buildIndex);

        yield return StartCoroutine(FadePanel(1f, 0f, 1f));
    }

    private IEnumerator FadePanel(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            transitionsPanel.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, alpha));
            yield return null;
        }

        transitionsPanel.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, endAlpha));
    }

}
