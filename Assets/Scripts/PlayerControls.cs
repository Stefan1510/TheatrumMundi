using UnityEngine;
using UnityEngine.UI;
// using UnityEditor.Recorder;
using UnityEditor;
using TMPro;

// namespace RockVR.Video.Demo
namespace UTJ.FrameCapturer
{
    public class PlayerControls : MonoBehaviour
    {
        private bool pointAnimation;
        private float t = 0.0f;
        private int i;
        public GameObject gameController;
        public Button[] aPlayButtons;
        public Button[] aStopButtons;
        public Sprite[] aPlaySprites;
        public Sprite[] aPauseSprites;
        public Sprite[] aStopSprites;
        [SerializeField] private RailMusicManager tmpRailMusicMan;
        //private float _f = 0.0f;
        private float _fps = 0.0f;
        [SerializeField] private GameObject overlayRendering;
        [SerializeField] private TextMeshProUGUI textPercent, textContent, textTitle;
        [SerializeField] private MovieRecorder rec;
        [SerializeField] private GameObject buttonOkay;

        // var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        // var TestRecorderController = new RecorderController(controllerSettings);

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
            if (pointAnimation)
            {
                textPercent.text = (AnimationTimer.GetTime()/AnimationTimer._maxTime*100).ToString("0.0")+"%";
                if(rec.endFrame==rec.m_frame)
                {
                    Debug.Log("fertig!");
                    buttonOkay.SetActive(true);
                    rec.EndRecording();
                    textTitle.text="Dein Film ist fertig.";
                    textContent.text="Du kannst ihn dir im Windows Explorer unter 'C:/tmp/TheatrumMundi' anschauen.";
                    textPercent.gameObject.SetActive(false);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (SceneManaging.recording && overlayRendering.activeSelf)
                {
                    SceneManaging.recording = false;
                    rec.EndRecording();
                    overlayRendering.SetActive(false);
                    pointAnimation = false;
                    t = 0;
                }
            }
        }
        public void PressOkayOnFinish()
        {
            overlayRendering.SetActive(false);
            textContent.text="Das kann einige Minuten dauern.\nUm abzubrechen, bitte klicken.";
            textPercent.gameObject.SetActive(true);
            textTitle.text="Dein Film wird erstellt. ";
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
        public void ButtonRender(bool start)
        {
            if (start)
            {
                AnimationTimer.ResetTime();
                ButtonPlay();
                overlayRendering.SetActive(true);
                pointAnimation = true;
                rec.BeginRecording();
            }
        }
    }
}
