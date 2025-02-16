using UnityEngine;
using UnityEngine.Events;

public class DayManager : MonoBehaviour
{

    static DayManager Instance;

    public UnityEvent dayOverEvent;
    public float endTime = 36000;

    // time the day started
    private float _dayTime;
    // time the day will end
    private float _endTime = 36000;
    // if the day has ended
    private bool _hasDayEnded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogWarning("There are multiple DayManager classes.");
        }
    }

    private void Start()
    {
        _dayTime = 0;
        _endTime = endTime;
        _hasDayEnded = false;
    }

    private void Update()
    {
        _dayTime += Time.deltaTime * 120;
        if (_dayTime > _endTime)
        {
            if (!_hasDayEnded)
            {
                _hasDayEnded = true;
                dayOverEvent.Invoke();
            }
        }
        else
        {
            Debug.Log(GetDayTimeString());
        }
    }

    public string GetDayTimeString()
    {
        int hours = Mathf.FloorToInt(_dayTime / 3600);
        int minutes = Mathf.FloorToInt((_dayTime % 3600) / 60);
        return $"{hours:00}:{minutes:00}";
    }
}
