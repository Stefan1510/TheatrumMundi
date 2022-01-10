using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    public Text TimerText;
    public Button PlayButton;
    public Button PauseButton;
    public Sprite PauseSprite;
    public Sprite PlaySprite;
    public Sprite StopSprite;
    private float _f = 0.0f;
    private float _fps = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        PlayButton.transform.GetComponent<Image>().sprite = PlaySprite;
        PauseButton.transform.GetComponent<Image>().sprite = PauseSprite;
    }

    // Update is called once per frame
    void Update()
    {
        _f += Time.deltaTime;
        if (_f >= 1.0f)
        {
            _fps = 1 / Time.deltaTime;
            _f -= 1.0f;
        }
        TimerText.text = AnimationTimer.GetTime().ToString("0.00") + "\n - \nfps: " + _fps.ToString("0");
    }

    public void ButtonReset()
    {
        AnimationTimer.ResetTime();
    }

    public void ButtonPlay()
    {
        switch (AnimationTimer.GetTimerState())
        {
            case AnimationTimer.TimerState.stopped:
                AnimationTimer.StartTimer();
                PlayButton.transform.GetComponent<Image>().sprite = StopSprite;
                break;
            case AnimationTimer.TimerState.playing:
                AnimationTimer.StopTimer();
                PlayButton.transform.GetComponent<Image>().sprite = PlaySprite;
                break;
            case AnimationTimer.TimerState.paused:
                AnimationTimer.StartTimer();
                PlayButton.transform.GetComponent<Image>().sprite = StopSprite;
                break;
        }
    }
    public void ButtonPause()
    {
        switch (AnimationTimer.GetTimerState())
        {
            case AnimationTimer.TimerState.stopped:
                break;
            case AnimationTimer.TimerState.playing:
                AnimationTimer.PauseTimer();
                PlayButton.transform.GetComponent<Image>().sprite = PlaySprite;
                break;
            case AnimationTimer.TimerState.paused:
                break;
        }
    }

    //unused. for showing the use of SetTime()
    public void ChangeTime(float newTime)
    {
        AnimationTimer.SetTime(newTime);
    }
}
