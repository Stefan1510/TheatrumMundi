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
	
    //// Start is called before the first frame update
    void Start()
    {
        SwitchToNormalsceen();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SwitchToFullscreen()
    {
		liveViewFullscreenObj.SetActive(true);
    }
    public void SwitchToNormalsceen()
    {
		liveViewFullscreenObj.SetActive(false);
    }
	public void changePlayIcon()
	{
		//Debug.Log("timerstate: "+AnimationTimer.GetTimerState());
		switch (AnimationTimer.GetTimerState())
		{
            case AnimationTimer.TimerState.stopped:
                PlayBtn.transform.GetComponent<Image>().sprite = PauseIcon;
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
