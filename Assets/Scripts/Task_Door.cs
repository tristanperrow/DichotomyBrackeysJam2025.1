using System.Collections;
using UnityEngine;

public class Task_Door : Task
{

    public static Task_Door Instance;

    [SerializeField] private float _timeBetweenActive = 30f;
    [SerializeField] private float _timeUntilFail = 30f;
    [SerializeField] private float _knockInterval = 7.5f;

    private float _activationTimer = 0f;
    private float _activeDuration = 0f;
    private float _knockTimer = 0f;

    [SerializeField] private AudioSource _knockSound;
    [SerializeField] private AudioSource _answerSound;

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

    private void Update()
    {
        // Prevent base class from timing out
        timeRemaining = int.MaxValue;

        if (!isActive)
        {
            _activationTimer += Time.deltaTime;
            if (_activationTimer >= _timeBetweenActive)
            {
                ActivateTask();
                _activationTimer = 0f;
            }
        }
        else
        {
            _activeDuration += Time.deltaTime;
            _knockTimer += Time.deltaTime;

            // knocking
            if (_knockTimer >= _knockInterval)
            {
                StartCoroutine(Knock());
                _knockTimer = 0f;
            }

            // failing
            if (_activeDuration >= _timeUntilFail)
            {
                DoorTaskFail();
            }
        }
    }

    // knock three times
    private IEnumerator Knock()
    {
        _knockSound.Play();

        yield return new WaitForSeconds(0.5f);

        _knockSound.Play();

        yield return new WaitForSeconds(0.5f);

        _knockSound.Play();
    }

    private void DoorTaskFail()
    {
        // fade screen to black, shoot player, then transition to lose screen
        Debug.Log("Failed door task...");
        StartCoroutine(SceneTransitionManager.Instance.LoadScene(2));
    }

    public override void OpenTask()
    {
        // don't open task, instead just state that you've checked on the door
        isActive = false;
        _activationTimer = 0f;
        _activeDuration = 0f;
        _knockTimer = 0f;

        _answerSound.Play();
    }

    protected override void OnActivatedTask()
    {
        // on activation
        _activeDuration = 0f;
        _knockTimer = 0f;

        // if random door timers
        _timeBetweenActive = Random.Range(30f, 45f);

        StartCoroutine(Knock());
    }
}
