using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchLiveView : MonoBehaviour
{
    public GameObject liveViewFullscreenObj;
    public Button PlayBtn;
    public Sprite PlayIcon;
    public Sprite PauseIcon;

    void Start()
    {
        SwitchToNormalsceen();
    }
    public void SwitchToFullscreen()
    {
        liveViewFullscreenObj.SetActive(true);
        SceneManaging.fullscreenOn = true;
    }
    public void SwitchToNormalsceen()
    {
        SceneManaging.fullscreenOn = false;
        liveViewFullscreenObj.SetActive(false);
    }
    public void changePlayIcon()
    {
        //Debug.Log("timerstate: "+AnimationTimer.GetTimerState());
        switch (AnimationTimer.GetTimerState())
        {
            case AnimationTimer.TimerState.stopped:
                PlayBtn.transform.GetComponent<Image>().sprite = PlayIcon;
                break;
            case AnimationTimer.TimerState.playing:
                PlayBtn.transform.GetComponent<Image>().sprite = PauseIcon;
                break;
            case AnimationTimer.TimerState.paused:
                PlayBtn.transform.GetComponent<Image>().sprite = PlayIcon;
                break;
        }
    }
}
