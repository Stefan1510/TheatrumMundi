using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PressHelp : MonoBehaviour
{
    #region variables
    [SerializeField] private GameObject helpButtonPressed, helpOverlayMenue2, helpOverlayMenue1, helpOverlayMenue3, helpOverlayMenue4, helpOverlayMenue5, helpOverlayMenue6, helpOverlayFlyer, aboutScreenVisitor, aboutScreenExpert;
    [SerializeField] private GameObject timeSliderBubbleFigure, timeSliderBubbleMusic, gameController, loadSaveDialog, panelWarningInput;
    [SerializeField] private GameObject maskTimeSlider, _timeSliderPlayButton;
    [SerializeField] private TextMeshProUGUI tutText;
    [SerializeField] private GameObject tutorialCountImage;
    [SerializeField] private GameObject flyer;
    [SerializeField] private GameObject liveView;
    [SerializeField] private GameObject codeReminder;
    [SerializeField] private GameObject helpOverlaySave1, helpOverlaySave2, helpOverlaySave3;

    public GameObject _arrowHelp;
    GameObject _timeSliderBubble;
    [SerializeField] GameObject _countdown;
    [SerializeField] GameObject _introOverlay;
    [SerializeField] GameObject imageOkayStartOverlay;
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
        aboutScreenVisitor.SetActive(false);
        aboutScreenExpert.SetActive(false);
        _buttonColorStart = new Color(255, 255, 255, 0);
        _buttonColorAttention = new Color32(255, 255, 255, 70);
        _maxTimeArrow = 40;
        _maxTimeCountdown = 60;

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

        //destroy save-helpOverlays, because they are not there in Expert Version
        if (SceneManaging.isExpert)
        {
            Destroy(helpOverlaySave1);
            Destroy(helpOverlaySave2);
            Destroy(helpOverlaySave3);
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
            if (!SceneManaging.aboutActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.dialogActive)
            {
                if (pressed)
                {
                    helpOverlayMenue1.SetActive(false);
                    helpOverlayMenue2.SetActive(false);
                    helpOverlayMenue3.SetActive(false);
                    helpOverlayMenue4.SetActive(false);
                    helpOverlayMenue5.SetActive(false);
                    helpOverlayMenue6.SetActive(false);
                    pressed = false;

                    maskTimeSlider.SetActive(false);
                    _tutorialCounter = -1;
                    tutorialCountImage.SetActive(false);
                    SceneManaging.tutorialActive = false;
                }

                else
                {
                    if (SceneManaging.flyerActive)  // theaterzettel
                    {
                        helpOverlayFlyer.SetActive(true);
                        _tutorialCounter = 0;
                        helpOverlayFlyer.transform.GetChild(0).gameObject.SetActive(true);
                        _publicHelpMenue = helpOverlayFlyer;
                    }
                    else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 1)  // Buehne
                    {
                        helpOverlayMenue1.SetActive(true);
                        _tutorialCounter = 0;
                        _publicHelpMenue = helpOverlayMenue1;
                        helpOverlayMenue1.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 1)   // Figuren
                    {
                        helpOverlayMenue5.SetActive(true);
                        _tutorialCounter = 0;
                        helpOverlayMenue5.transform.GetChild(0).gameObject.SetActive(true);
                        _publicHelpMenue = helpOverlayMenue5;
                        _timeSliderBubble = timeSliderBubbleFigure;
                    }
                    else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 2)    // kulissen
                    {
                        helpOverlayMenue2.SetActive(true);
                        _publicHelpMenue = helpOverlayMenue2;
                        _tutorialCounter = 0;
                        helpOverlayMenue2.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 3)  // licht
                    {
                        helpOverlayMenue3.SetActive(true);
                        _publicHelpMenue = helpOverlayMenue3;
                        helpOverlayMenue3.transform.GetChild(0).gameObject.SetActive(true);
                        _tutorialCounter = 0;
                    }
                    else if (SceneManaging.directorMenueActive == 3 && SceneManaging.mainMenuActive == 2)  // musik
                    {
                        helpOverlayMenue6.SetActive(true);
                        _publicHelpMenue = helpOverlayMenue6;
                        _tutorialCounter = 0;
                        helpOverlayMenue6.transform.GetChild(0).gameObject.SetActive(true);
                        _timeSliderBubble = timeSliderBubbleMusic;
                    }
                    else if (SceneManaging.configMenueActive == 4)  // speichern
                    {
                        helpOverlayMenue4.SetActive(true);
                        _publicHelpMenue = helpOverlayMenue4;
                        helpOverlayMenue4.transform.GetChild(0).gameObject.SetActive(true);
                        _tutorialCounter = 0;
                    }
                    tutorialCountImage.SetActive(true);
                    tutText.text = "Hinweis " + _tutorialCounter + "/" + _publicHelpMenue.transform.childCount;
                    SceneManaging.tutorialActive = true;
                    pressed = true;
                }
            }
        }
        else if (i == 1) // open about
        {
            if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.dialogActive)
            {
                if (SceneManaging.isExpert)
                    aboutScreenExpert.SetActive(true);
                else
                    aboutScreenVisitor.SetActive(true);
                SceneManaging.aboutActive = true;
            }
        }
        else // close about
        {
            if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.dialogActive)
            {
                aboutScreenVisitor.SetActive(false);
                aboutScreenExpert.SetActive(false);
                SceneManaging.aboutActive = false;
            }
        }
    }
    public void OnClickBack()
    {
        maskTimeSlider.SetActive(false);

        if (_tutorialCounter == 1)
        {
            helpOverlayMenue1.SetActive(false);
            helpOverlayMenue2.SetActive(false);
            helpOverlayMenue3.SetActive(false);
            helpOverlayMenue4.SetActive(false);
            helpOverlayMenue5.SetActive(false);
            helpOverlayMenue6.SetActive(false);
            pressed = false;

            maskTimeSlider.SetActive(false);
            _tutorialCounter = -1;
            tutorialCountImage.SetActive(false);
            SceneManaging.tutorialActive = false;
        }
        else
        {
            _tutorialCounter -= 2;
        }
    }
    public void ClickOnLiveView()
    {
        if (pressedLiveView)
        {
            helpTextLiveView.SetActive(false);
            pressedLiveView = false;
        }
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
                pressedLiveView = true;
            }
        }
    }
    public void ClickOkayOnStartOverlay()
    {
        imageOkayStartOverlay.SetActive(false);
    }
    private void LateUpdate()
    {
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
            _isClicked = true;
            // tutorial
            if (_tutorialCounter != -1 && _tutorialCounter < _publicHelpMenue.transform.childCount)
            {
                for (int i = 0; i < _publicHelpMenue.transform.childCount; i++)
                {
                    _publicHelpMenue.transform.GetChild(i).gameObject.SetActive(false);
                }
                _publicHelpMenue.transform.GetChild(_tutorialCounter).gameObject.SetActive(true);

                // object menu
                if (_publicHelpMenue == helpOverlayMenue5)
                {
                    if (_tutorialCounter == 0)   // visitor/expert version shelf
                    {
                        if (!SceneManaging.isExpert)
                            helpOverlayMenue5.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        else
                            helpOverlayMenue5.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    }
                    else if (_tutorialCounter == 3) // visitor/expert version figures direction
                    {
                        if (!SceneManaging.isExpert)
                            helpOverlayMenue5.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
                        else
                            helpOverlayMenue5.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
                    }
                    else if (_tutorialCounter == 4) // timeslider
                    {
                        _timeSliderBubble.transform.position = _timeSliderPlayButton.transform.position;
                        maskTimeSlider.SetActive(true);
                        _timeSliderBubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(_timeSliderBubble.GetComponent<RectTransform>().anchoredPosition.x, 13);
                    }
                    else if (_tutorialCounter == 5) // nach timeslider
                    {
                        maskTimeSlider.SetActive(false);
                    }
                }
                // music menu
                else if (_publicHelpMenue == helpOverlayMenue6)
                {
                    if (_tutorialCounter == 0)   // visitor/expert version
                    {
                        if (!SceneManaging.isExpert)
                            helpOverlayMenue6.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        else
                            helpOverlayMenue6.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    }
                    else if (_tutorialCounter == 1)
                    {
                        if (!SceneManaging.isExpert)
                            helpOverlayMenue6.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                        else
                            helpOverlayMenue6.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                    }
                    else if (_tutorialCounter == 2)
                    {
                        if (!SceneManaging.isExpert)
                            helpOverlayMenue6.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                        else
                            helpOverlayMenue6.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                    }
                    else if (_tutorialCounter == 3) // timeslider
                    {
                        _timeSliderBubble.transform.position = _timeSliderPlayButton.transform.position;
                        maskTimeSlider.SetActive(true);
                        _timeSliderBubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(_timeSliderBubble.GetComponent<RectTransform>().anchoredPosition.x, 13);
                    }
                    else if (_tutorialCounter == 4) // nach timeslider
                    {
                        maskTimeSlider.SetActive(false);
                    }
                }
                // expert/visitor for coulisses
                else if (_publicHelpMenue == helpOverlayMenue2)
                {
                    if (_tutorialCounter == 0)
                    {
                        if (!SceneManaging.isExpert)
                        {
                            helpOverlayMenue2.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        }
                        else
                        {
                            helpOverlayMenue2.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                        }
                    }
                }
                _tutorialCounter++;
                tutText.text = "Hinweis " + _tutorialCounter + "/" + _publicHelpMenue.transform.childCount;
            }
            else
            {
                maskTimeSlider.SetActive(false);
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
                catch (NullReferenceException) { }

            }
            if (helpTextLiveView.activeSelf)
            {
                helpTextLiveView.SetActive(false);
                pressedLiveView = false;
                offFromClick = true;
            }

            _isClicked = false;
            _timerOverlay = 0;
            _idleTimer = 0;
            offFromClick = false;
        }
        if (_tutorialCounter == -1 && !_isClicked && AnimationTimer.GetTimerState() != AnimationTimer.TimerState.playing && !SceneManaging.isExpert)
        {
            _idleTimer += Time.deltaTime;
            if (!_newScene)
                _timerOverlay += Time.deltaTime;
        }
        if (_idleTimer > _helpAnimWaitTime && AnimationTimer.GetTimerState() != AnimationTimer.TimerState.playing)
        {
            HelpAnimation();
        }
        if (_idleTimer >= _helpAnimWaitTime + _helpAnimDuration)
        {
            StopHelpAnimation();
            secondHighlight = false;
        }
        if (_timerOverlay > _maxTimeArrow && _timerOverlay < _maxTimeCountdown && AnimationTimer.GetTimerState() != AnimationTimer.TimerState.playing && !SceneManaging.dragging && !arrowPressed)
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
                StartCoroutine(gameController.GetComponent<SaveFileController>().LoadFileFromServer("Musterszene_leer_Visitor.json", "fromCode"));
                _newScene = true;

                if (SceneManaging.saveDialogActive)
                {
                    loadSaveDialog.SetActive(false);
                    SceneManaging.saveDialogActive = false;
                    panelWarningInput.SetActive(false);
                    gameController.GetComponent<SaveFileController>().ResetTabs(0);
                }
                else if (SceneManaging.flyerActive)
                {
                    flyer.SetActive(false);
                    SceneManaging.flyerActive = false;
                }
                else if (SceneManaging.fullscreenOn)
                {
                    liveView.SetActive(false);
                    SceneManaging.fullscreenOn = false;
                }

                _introOverlay.SetActive(true);
                codeReminder.SetActive(false);
            }
        }
    }
}
