using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationTimer : MonoBehaviour
{
    private static bool _runTimer = false;
    private static float _timer = 0.0f;
    private static float _minTime = 0.0f;
    public static float _maxTime = 3 * 60;
    [SerializeField] private Slider _sliderMaxLength;
    [SerializeField] private GameObject _settingsLength;
    [SerializeField] private TMP_InputField _inputSliderLength;
    [SerializeField] private TextMeshProUGUI _inputText;
    public enum TimerState { stopped, playing, paused };
    private static TimerState _timerState = TimerState.stopped;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManaging.isExpert)
        {
            _maxTime = 10 * 60 + 14;
        }
        else
        {
            _maxTime = 3 * 60;
        }
        ResetTime();

    }

    void Update()
    {
        if (_runTimer)
            _timer += Time.deltaTime;
    }
    public static float GetMinTime()
    {
        return _minTime;
    }
    public void SetMaxTime(float maxTime)
    {
        Debug.Log("max: "+maxTime);
        _maxTime = maxTime;
    }
    public static float GetMaxTime()
    {
        return _maxTime;
    }
    public static float GetTime()
    {
        return _timer;
    }
    public static void ResetTime()
    {
        _timer = 0.0f;
    }

    public static void SetTime(float time)
    {
        _timer = time;
    }

    public static void PauseTimer()
    {
        _runTimer = false;
        _timerState = TimerState.paused;
    }

    public static void StopTimer()
    {
        _timer = 0.0f;
        _runTimer = false;
        _timerState = TimerState.stopped;
    }

    public static void StartTimer()
    {
        _runTimer = true;
        _timerState = TimerState.playing;
    }

    public static TimerState GetTimerState()
    {
        return _timerState;
    }
    public void ChangeMaxLength()
    {
        Debug.Log("value: " + (_sliderMaxLength.value));
        _maxTime = 60 * _sliderMaxLength.value;
        _inputSliderLength.text = _sliderMaxLength.value.ToString("0");
        StaticSceneData.StaticData.pieceLength = 60 * int.Parse(_inputSliderLength.text);
    }
    public void PressInfoButton(bool on)
    {
        if (on)
            _settingsLength.SetActive(true);
        else
            _settingsLength.SetActive(false);
    }
    public void ChangeSliderValue()
    {
        _sliderMaxLength.value = int.Parse(_inputSliderLength.text);
        _maxTime = 60 * _sliderMaxLength.value;
        StaticSceneData.StaticData.pieceLength = 60 * int.Parse(_inputSliderLength.text);
    }
}
