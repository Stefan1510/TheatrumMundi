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
        private float _f = 0.0f;
        private float _fps = 0.0f;
        [SerializeField] private GameObject overlayRendering;
        [SerializeField] private MovieRecorder rec;

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
                /*if (t == 0)
                {
                    if (i < 2)
                        i++;
                    else
                        i = 0;
                    switch (i)
                    {
                        case 0: 
                        overlayRendering.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dein Film wird erstellt.";
                        break;
                        case 1: 
                        overlayRendering.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dein Film wird erstellt..";
                        break;
                        case 2: 
                        overlayRendering.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dein Film wird erstellt...";
                        break;
                    }
                }
                t += Time.deltaTime;
                if (t >= 0.1f)
                {
                    t = 0;
                }*/
                overlayRendering.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (AnimationTimer.GetTime()/AnimationTimer._maxTime).ToString("000")+"%";
                // Debug.Log("t: "+t*100+"t%2: "+t*100%2);
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
