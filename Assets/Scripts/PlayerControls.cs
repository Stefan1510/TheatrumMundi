using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RockVR.Video.Demo
{
    public class PlayerControls : MonoBehaviour
    {
        public GameObject gameController;
        public Text TimerText;
        public Button PlayButton;
        public Button StopButton;
        public Sprite PauseSprite;
        public Sprite PlaySprite;
        public Sprite StopSprite;
        private float _f = 0.0f;
        private float _fps = 0.0f;
        private bool renderButtonPressed = false;
        // Start is called before the first frame update
        void Start()
        {
            PlayButton.transform.GetComponent<Image>().sprite = PlaySprite;
            StopButton.transform.GetComponent<Image>().sprite = StopSprite;
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
                    SceneManaging.playing = true;
                    PlayButton.transform.GetComponent<Image>().sprite = PauseSprite;
                    break;
                case AnimationTimer.TimerState.playing:
                    AnimationTimer.PauseTimer();
                    SceneManaging.playing = false;
                    PlayButton.transform.GetComponent<Image>().sprite = PlaySprite;
                    break;
                case AnimationTimer.TimerState.paused:
                    AnimationTimer.StartTimer();
                    SceneManaging.playing = true;
                    PlayButton.transform.GetComponent<Image>().sprite = PauseSprite;
                    break;
            }
        }
        public void ButtonStop()
        {
            switch (AnimationTimer.GetTimerState())
            {
                case AnimationTimer.TimerState.stopped:
                    break;
                case AnimationTimer.TimerState.playing:
                    AnimationTimer.StopTimer();
                    SceneManaging.playing = false;
                    PlayButton.transform.GetComponent<Image>().sprite = PlaySprite;
                    break;
                case AnimationTimer.TimerState.paused:
                    AnimationTimer.StopTimer();
                    SceneManaging.playing = false;
                    break;
            }
        }

        public void ButtonRender()
        {
            if (renderButtonPressed)
            {
                VideoCaptureCtrl.instance.StopCapture();
                renderButtonPressed = false;
                AnimationTimer.ResetTime();
            }
            else
            {
                AnimationTimer.ResetTime();

                VideoCaptureCtrl.instance.StartCapture();
                renderButtonPressed = true;
                AnimationTimer.StartTimer();
                SceneManaging.playing = true;
            }
        }

        //unused. for showing the use of SetTime()
        public void ChangeTime(float newTime)
        {
            AnimationTimer.SetTime(newTime);
        }
    }
}
