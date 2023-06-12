using UnityEngine;
using UnityEngine.UI;

namespace RockVR.Video.Demo
{
    public class PlayerControls : MonoBehaviour
    {
        public GameObject gameController;
        public Button[] aPlayButtons;
        public Button[] aStopButtons;
        public Sprite[] aPlaySprites;
        public Sprite[] aPauseSprites;
        public Sprite[] aStopSprites;
        [SerializeField] private RailMusicManager tmpRailMusicMan;
        private float _f = 0.0f;
        private float _fps = 0.0f;
        private bool renderButtonPressed = false;
        void Start()
        {
            for (int i = 0; i < aPlayButtons.Length; i++)
            {
                aPlayButtons[i].transform.GetComponent<Image>().sprite = aPlaySprites[i];
            }
            for (int i = 0; i < aStopButtons.Length; i++)
            {
                aStopButtons[i].transform.GetComponent<Image>().sprite = aStopSprites[i];
            }
        }
        void Update()
        {
            _f += Time.deltaTime;
            if (_f >= 1.0f)
            {
                _fps = 1 / Time.deltaTime;
                _f -= 1.0f;
            }
            //TimerText.text = AnimationTimer.GetTime().ToString("0.00") + "\n - \nfps: " + _fps.ToString("0");
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
                    for (int i = 0; i < aPlayButtons.Length; i++)
                    {
                        aPlayButtons[i].transform.GetComponent<Image>().sprite = aPauseSprites[i];
                    }
                    break;
                case AnimationTimer.TimerState.playing:
                    AnimationTimer.PauseTimer();
                    SceneManaging.playing = false;
                    for (int i = 0; i < aPlayButtons.Length; i++)
                    {
                        aPlayButtons[i].transform.GetComponent<Image>().sprite = aPlaySprites[i];
                    }
                    break;
                case AnimationTimer.TimerState.paused:
                    AnimationTimer.StartTimer();
                    //start music
                    //tmpRailMusicMan.PlayLatestPiece(true);
                    SceneManaging.playing = true;
                    for (int i = 0; i < aPlayButtons.Length; i++)
                    {
                        aPlayButtons[i].transform.GetComponent<Image>().sprite = aPauseSprites[i];
                    }
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
                    for (int i = 0; i < aPlayButtons.Length; i++)
                    {
                        aPlayButtons[i].transform.GetComponent<Image>().sprite = aPlaySprites[i];
                    }
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
