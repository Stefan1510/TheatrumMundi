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
    public static int _maxTime = 3 * 60;

    public enum TimerState { stopped, playing, paused };
    private static TimerState _timerState = TimerState.stopped;
    void Start()
    {
        if (SceneManaging.isExpert)
        {
            _maxTime = 10 * 60;
        }
        PauseTimer();
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
    public void SetMaxTime(int maxTime)
    {
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

}
