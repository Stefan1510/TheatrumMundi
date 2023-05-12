using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PressHelp : MonoBehaviour
{
    #region variables
    [SerializeField] GameObject helpButtonPressed, helpOverlayMenue2, helpOverlayMenue1, helpOverlayMenue3, helpOverlayMenue4, helpOverlayMenue5, helpOverlayMenue6, helpOverlayFlyer, aboutScreen;
    [SerializeField] GameObject timeSliderBubbleFigure, timeSliderBubbleMusic, gameController;
    [SerializeField] GameObject maskTimeSlider, _timeSliderPlayButton;
    [SerializeField] TextMeshProUGUI tutText;
    [SerializeField] GameObject tutorialCountImage;
    public GameObject _arrowHelp;
    GameObject _timeSliderBubble;
    [SerializeField] GameObject _countdown;
    GameObject _publicHelpMenue;
    [HideInInspector] public bool pressed = false, arrowPressed = false;
    private bool _newScene = true, _isClicked = false;
    private bool pressedLiveView = false, secondHighlight = false, offFromClick;
    private float _idleTimer;
    private float _timerOverlay;
    private float _helpAnimDuration;
    private float _helpAnimWaitTime;
    private Color32 _buttonColorStart;
    private Color32 _buttonColorAttention;
    public GameObject helpTextLiveView;
    private int _tutorialCounter;
    private int _maxTimeArrow, _maxTimeCountdown;
    #endregion
    private void Start()
    {
        _tutorialCounter = -1;
        _helpAnimDuration = 1;
        _helpAnimWaitTime = 10;
        helpOverlayMenue1.SetActive(false);
        helpButtonPressed.SetActive(false);
        helpOverlayMenue2.SetActive(false);
        helpOverlayMenue3.SetActive(false);
        aboutScreen.SetActive(false);
        _buttonColorStart = new Color(255, 255, 255, 0);
        _buttonColorAttention = new Color32(255, 255, 255, 70);
        _maxTimeArrow = 20;
        _maxTimeCountdown = 40;

        try
        {
            helpOverlayMenue4.SetActive(false);
        }
        catch (Exception ex)
        {
            if (ex is NullReferenceException || ex is UnassignedReferenceException)
            {
                return;
            }
            throw;
        }
    }
    private void HelpAnimation()
    {
        float x = _idleTimer - _helpAnimWaitTime;
        float y = -4 * Mathf.Pow(x - 0.5f, 2) + 1;
        Color32 buttonColor = Color32.Lerp(_buttonColorStart, _buttonColorAttention, y);
        transform.GetChild(1).GetComponent<Image>().color = buttonColor;
        transform.GetChild(0).localScale = new Vector3(1 + y / 4, 1 + y / 4, 1 + y / 4);
        transform.GetChild(1).localScale = new Vector3(1 + y / 4, 1 + y / 4, 1 + y / 4);
        if (x > 1 && secondHighlight == false)
        {
            _idleTimer = 10.01f;
            secondHighlight = true;
        }
    }
    private void StopHelpAnimation()
    {
        transform.GetChild(1).GetComponent<Image>().color = _buttonColorStart;
        transform.localScale = Vector3.one;
        _idleTimer = 0;
    }
    private void StopOverlay()
    {
        _timerOverlay = 0;
        _arrowHelp.SetActive(false);
        arrowPressed = false;
    }
    public void OnClick(int i)
    {
        if (i == 0) // help
        {
            //Debug.Log("pressed: " + pressed);
            if (pressed)
            {
                helpOverlayMenue1.SetActive(false);
                helpOverlayMenue2.SetActive(false);
                helpOverlayMenue3.SetActive(false);
                helpOverlayMenue4.SetActive(false);
                helpOverlayMenue5.SetActive(false);
                helpOverlayMenue6.SetActive(false);
                helpButtonPressed.SetActive(false);
                pressed = false;

                maskTimeSlider.SetActive(false);
                _tutorialCounter = -1;
                tutorialCountImage.SetActive(false);
                SceneManaging.tutorialActive = false;
            }

            else
            {
                helpButtonPressed.SetActive(true);
                if (SceneManaging.flyerActive)  // theaterzettel
                {
                    helpOverlayFlyer.SetActive(true);
                    _tutorialCounter = 1;
                    helpOverlayFlyer.transform.GetChild(0).gameObject.SetActive(true);
                    _publicHelpMenue = helpOverlayFlyer;
                }
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 1)  // Buehne
                {
                    helpOverlayMenue1.SetActive(true);
                    _tutorialCounter = 1;
                    _publicHelpMenue = helpOverlayMenue1;
                    helpOverlayMenue1.transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 1)   // Figuren
                {
                    // Debug.Log("hier");
                    helpOverlayMenue5.SetActive(true);
                    _tutorialCounter = 1;
                    helpOverlayMenue5.transform.GetChild(0).gameObject.SetActive(true);
                    _publicHelpMenue = helpOverlayMenue5;
                    _timeSliderBubble = timeSliderBubbleFigure;
                }
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 2)    // kulissen
                {
                    helpOverlayMenue2.SetActive(true);
                    _publicHelpMenue = helpOverlayMenue2;
                    _tutorialCounter = 1;
                    helpOverlayMenue2.transform.GetChild(0).gameObject.SetActive(true);

                }
                /*else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 2)    // (light director) verworfen
                {
                    helpOverlayMenue2.SetActive(true);
                    Debug.Log("overlay 6");
                }*/
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 3)  // licht
                {
                    helpOverlayMenue3.SetActive(true);
                    _publicHelpMenue = helpOverlayMenue3;
                    helpOverlayMenue3.transform.GetChild(0).gameObject.SetActive(true);
                    _tutorialCounter = 1;
                }
                else if (SceneManaging.directorMenueActive == 3 && SceneManaging.mainMenuActive == 2)  // musik
                {
                    helpOverlayMenue6.SetActive(true);
                    _publicHelpMenue = helpOverlayMenue6;
                    _tutorialCounter = 1;
                    helpOverlayMenue6.transform.GetChild(0).gameObject.SetActive(true);
                    _timeSliderBubble = timeSliderBubbleMusic;
                }
                else if (SceneManaging.configMenueActive == 4)  // speichern
                {
                    helpOverlayMenue4.SetActive(true);
                    _publicHelpMenue = helpOverlayMenue4;
                    helpOverlayMenue4.transform.GetChild(0).gameObject.SetActive(true);
                    _tutorialCounter = 1;
                }
                tutorialCountImage.SetActive(true);
                tutText.text = "Hinweis " + _tutorialCounter + "/" + _publicHelpMenue.transform.childCount;
                SceneManaging.tutorialActive = true;
                pressed = true;
                //Debug.Log("true");
            }

        }
        else if (i == 1) // open about
        {
            aboutScreen.SetActive(true);
        }
        else // close about
        {
            aboutScreen.SetActive(false);
        }
    }
    public void ClickOnLiveView()
    {
        // if (pressedLiveView)
        // {
        //     helpTextLiveView.SetActive(false);
        //     pressedLiveView = false;
        //     Debug.Log("pressed: " + pressedLiveView);
        //     //offFromClick = true;
        // }
        if (!pressedLiveView)
        {
            if (!offFromClick)
            {
                helpTextLiveView.SetActive(true);
                if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 1)  // Buehne
                {
                    helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links eine Schiene aus und bearbeite ihre Höhe und Position.";
                }
                else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 1)   // Figuren
                {
                    helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links eine Figur aus und ziehe sie auf die Schiene unten.";
                }
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 2)    // kulissen
                {
                    helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links eine Kulisse aus und ziehe sie auf die Schiene unten rechts.";
                }
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 3)  // licht
                {
                    helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Im Shelf links kannst du Lichter erstellen und bearbeiten. ";
                }
                else if (SceneManaging.directorMenueActive == 3 && SceneManaging.mainMenuActive == 2)  // musik
                {
                    helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links ein Musikstück aus und ziehe es auf die Schiene unten.";
                }
                else if (SceneManaging.configMenueActive == 4)  // speichern
                {
                    helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Links im Shelf hast du die Möglichkeit, Szenen zu laden oder zu speichern.";
                }
                //pressed = true;
                pressedLiveView = true;
                Debug.Log("pressed: " + pressedLiveView);
            }
        }
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isClicked = true;
            // tutorial
            if (_tutorialCounter != -1 && _tutorialCounter < _publicHelpMenue.transform.childCount)
            {
                //Debug.Log("counter: " + _tutorialCounter);
                for (int i = 0; i < _publicHelpMenue.transform.childCount; i++)
                {
                    _publicHelpMenue.transform.GetChild(i).gameObject.SetActive(false);
                }
                _publicHelpMenue.transform.GetChild(_tutorialCounter).gameObject.SetActive(true);

                if (_publicHelpMenue == helpOverlayMenue5 || _publicHelpMenue == helpOverlayMenue6)
                {
                    if (_tutorialCounter == 2) // timeslider
                    {
                        _timeSliderBubble.transform.position = _timeSliderPlayButton.transform.position;
                        maskTimeSlider.SetActive(true);
                        _timeSliderBubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(_timeSliderBubble.GetComponent<RectTransform>().anchoredPosition.x, 13);
                    }
                    else if (_tutorialCounter == 3) // nach timeslider
                    {
                        maskTimeSlider.SetActive(false);
                    }
                }
                _tutorialCounter++;
                tutText.text = "Hinweis " + _tutorialCounter + "/" + _publicHelpMenue.transform.childCount;
            }
            else
            {
                try
                {
                    tutorialCountImage.SetActive(false);
                    _publicHelpMenue.SetActive(false);
                    _tutorialCounter = -1;
                    helpButtonPressed.SetActive(false);
                    pressed = false;
                    for (int i = 0; i < _publicHelpMenue.transform.childCount; i++)
                    {
                        _publicHelpMenue.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    SceneManaging.tutorialActive = false;
                }
                catch (NullReferenceException)
                {
                }

            }
            if (helpTextLiveView.activeSelf)
            {
                helpTextLiveView.SetActive(false);
                pressedLiveView = false;
                Debug.Log("pressed: " + pressedLiveView);
                offFromClick = true;
            }
        }

        if (Input.anyKeyDown)
        {
            StopHelpAnimation();
            StopOverlay();
            _newScene = false;

            if (_countdown.activeSelf)
                _countdown.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isClicked = false;
            _timerOverlay = 0;
            _idleTimer = 0;
            offFromClick = false;
        }
        if (_tutorialCounter == -1 && !_isClicked)
        {
            _idleTimer += Time.deltaTime;
            // if (!_newScene && !SceneManaging.playing)
            //_timerOverlay += Time.deltaTime;
        }
        if (_idleTimer > _helpAnimWaitTime && !SceneManaging.playing)
        {
            HelpAnimation();
        }
        if (_idleTimer >= _helpAnimWaitTime + _helpAnimDuration)
        {
            StopHelpAnimation();
            secondHighlight = false;
        }
        if (_timerOverlay > _maxTimeArrow && _timerOverlay < _maxTimeCountdown && !SceneManaging.playing && !SceneManaging.dragging && !arrowPressed)
        {
            _arrowHelp.SetActive(true);
            arrowPressed = true;
        }
        else if (_timerOverlay >= _maxTimeCountdown && arrowPressed)
        {
            _arrowHelp.SetActive(false);
            arrowPressed = false;
            _countdown.SetActive(true);
        }
        else if (_timerOverlay > _maxTimeCountdown)
        {
            for (int i = 0; i < 11; i++)
            {
                if (_timerOverlay > _maxTimeCountdown + i)
                    _countdown.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Achtung! Die Szene wird in " + (10 - i) + " Sekunden gelöscht.\nZum Abbrechen bitte klicken.";
            }
            if (_timerOverlay > _maxTimeCountdown + 10)
            {
                _countdown.SetActive(false);
                _timerOverlay = 0;
                StartCoroutine(gameController.GetComponent<SaveFileController>().LoadFileFromWWW("*Musterszene_leer.json", "fromCode"));
                _newScene = true;
            }
        }
    }
}
