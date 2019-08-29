using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleUi : MonoBehaviour
{
    [SerializeField] private Button _pauseTimeButton;
    [SerializeField] private float _pauseTimeMultiplier = 0;
    [SerializeField] private Button _normalTimeButton;
    [SerializeField] private float _normalTimeMultiplier = 1;
    [SerializeField] private Button _firstSpeedTimeButton;
    [SerializeField] private float _firstSpeedTimeMultiplier = 5f;
    [SerializeField] private Button _secondSpeedTimeButton;
    [SerializeField] private float _secondSpeedTimeMultiplier = 10f;

    [SerializeField] private System.Action _onDayOver;
    [SerializeField] private float _secondsPerDay = 60;
    private float _elapsedTimeCurrentDay;
    [SerializeField] private RectTransform _dayTimeBackgroundProgressRect;
    [SerializeField] private RectTransform _dayTimeProgressRect;
    private float _fullProgressValue;
    [SerializeField] private TextMeshProUGUI _dayNumberText;
    private int _dayNumber;
    [SerializeField] private TextMeshProUGUI _timeScaleText;

    void Start()
    {
        _fullProgressValue = _dayTimeBackgroundProgressRect.rect.width;
        _dayNumberText.text = "Day " + _dayNumber.ToString();
        _pauseTimeButton.onClick.AddListener(delegate { SetTimeScale(_pauseTimeMultiplier); });
        _normalTimeButton.onClick.AddListener(delegate { SetTimeScale(_normalTimeMultiplier); });
        _firstSpeedTimeButton.onClick.AddListener(delegate { SetTimeScale(_firstSpeedTimeMultiplier); });
        _secondSpeedTimeButton.onClick.AddListener(delegate { SetTimeScale(_secondSpeedTimeMultiplier); });
    }

    private void SetTimeScale(float value)
    {
        Time.timeScale = value;
        _timeScaleText.text = value.ToString() + 'x';
    }

    private void FixedUpdate()
    {
        _elapsedTimeCurrentDay += Time.deltaTime;
        float progress = _elapsedTimeCurrentDay / _secondsPerDay;
        _dayTimeProgressRect.sizeDelta = new Vector2(_fullProgressValue * progress, _dayTimeProgressRect.sizeDelta.y);
        if (!(progress >= 1f)) return;
        _onDayOver?.Invoke();
        _dayNumber++;
        _dayNumberText.text = "Day " + _dayNumber.ToString();
        _elapsedTimeCurrentDay = 0f;
    }
}
