using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchLiveView : MonoBehaviour
{
    public GameObject liveViewFullscreenObj;
    public Button PlayBtn, StopBtn;
    public Sprite PlayIcon;
    public Sprite PauseIcon;
    [SerializeField] private GameObject configMenue;

    void Start()
    {
        SwitchToNormalsceen();
    }
    void Update()
    {
        Debug.Log("update: "+liveViewFullscreenObj.activeSelf);
    }
    public void SwitchToFullscreen()
    {
        Debug.Log("h''_");
        liveViewFullscreenObj.SetActive(true);
        Debug.Log("active: "+liveViewFullscreenObj.activeSelf);
        SceneManaging.fullscreenOn = true;
        if(configMenue.activeSelf)
        {
            PlayBtn.interactable = false;
            StopBtn.interactable = false;
        }
        else
        {
            PlayBtn.interactable = true;
            StopBtn.interactable = true;
        }
    }
    public void SwitchToNormalsceen()
    {
        SceneManaging.fullscreenOn = false;
        liveViewFullscreenObj.SetActive(false);
        Debug.Log("und hier ");
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
