using UnityEngine;
using UnityEngine.UI;
// using UnityEditor.Recorder;
using UnityEditor;
using TMPro;
using System.IO;
using System.Collections;

// namespace RockVR.Video.Demo
namespace UTJ.FrameCapturer
{
    public class PlayerControls : MonoBehaviour
    {
        private bool pointAnimation;
        public GameObject gameController;
        public Button[] aPlayButtons;
        public Button[] aStopButtons;
        public Sprite[] aPlaySprites;
        public Sprite[] aPauseSprites;
        public Sprite[] aStopSprites;
        [SerializeField] private Snapshot _snapshot;
        [SerializeField] private TimeSliderController _tmpSlider;
        //private float _f = 0.0f;
        [SerializeField] private GameObject overlayRendering;
        [SerializeField] private TextMeshProUGUI textPercent, textContent, textTitle;
        [SerializeField] private MovieRecorder rec;
        [SerializeField] private GameObject buttonOkay;
        private string _fileName;
        private bool render = false;

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
        void FixedUpdate()
        {
            if (pointAnimation)
            {
                textPercent.text = (AnimationTimer.GetTime() / AnimationTimer._maxTime * 100).ToString("0.0") + "%";
                if (rec.endFrame == rec.m_frame)
                {
                    buttonOkay.SetActive(true);
                    rec.EndRecording();
                    textTitle.text = "Dein Film ist fertig.";
                    textContent.text = "Du kannst ihn dir im Windows Explorer unter 'C:/tmp/TheatrumMundi' anschauen.";
                    textPercent.gameObject.SetActive(false);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (SceneManaging.recording && overlayRendering.activeSelf)
                {
                    SceneManaging.recording = false;
                    rec.EndRecording();
                    ButtonPlay();
                    overlayRendering.SetActive(false);
                    pointAnimation = false;
                }
            }
        }
        void Update()
        {
            if (render)
            {
                for (int i = 0; i < 100; i++)
                {
                    _snapshot.CallTakeSnapshot(_fileName + "/" + i);
                    Debug.Log("time: " + i);
                    textPercent.text = i.ToString();

                    _tmpSlider._thisSlider.value = i / 25;
                    AnimationTimer.SetTime(i / 25);
                }
                render=false;
            }
        }
        public void PressOkayOnFinish()
        {
            overlayRendering.SetActive(false);
            textContent.text = "Das kann einige Minuten dauern.\nUm abzubrechen, bitte klicken.";
            textPercent.gameObject.SetActive(true);
            textTitle.text = "Dein Film wird erstellt. ";
            buttonOkay.SetActive(false);
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
            overlayRendering.SetActive(true);
            _fileName = "C:/tmp/TheatrumMundi/" + "neu";
            Directory.CreateDirectory(_fileName);
            render = true;
            //AnimationTimer.ResetTime();
            //ButtonPlay();
            float _maxFrames = AnimationTimer.GetMaxTime() * 25;

            // pointAnimation = true;
            // rec.BeginRecording();
        }
    }
}
