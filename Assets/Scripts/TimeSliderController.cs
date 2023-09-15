using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Linq;

public class TimeSliderController : MonoBehaviour, IPointerUpHandler, IDragHandler
{
    #region variables
    [SerializeField] private Text _textTime;
    [SerializeField] private Text _textMaxTime, _textMaxTimeBigLetters;
    [SerializeField] private Text _textFullScreen;
    [SerializeField] private Toggle _toggleKeyConfigControlls;
    [SerializeField] private GameObject _panelControls, _panelRailSpeedControls, _panelLightControls, _panelBackgroundPositionControls, imgTimelineSettingsArea;
    [SerializeField] private GameObject[] _keyButtons;
    [SerializeField] private Slider _sliderMaxLength;
    [SerializeField] private GameObject _settingsLength;
    [SerializeField] private TMP_InputField _inputSliderLength;
    [SerializeField] private AnimationTimer tmpAnimTimer;
    [SerializeField] private RailManager tmpRailManager;
    [SerializeField] private RailMusicManager tmpRailMusicManager;
    [SerializeField] private LightAnimationRepresentation tmpLightAnim;
    [SerializeField] private DrawCurveBg tmpBG;
    [SerializeField] private UTJ.FrameCapturer.PlayerControls playerCtrls;
    [SerializeField] private GameObject[] verticalTimeLines;
    [SerializeField] private GameObject[] timeStamps;
    private float railwidthAbsolute = 1670.4f;
    private Slider _thisSlider;
    #endregion
    void Start()
    {
        imgTimelineSettingsArea.SetActive(true);
        _thisSlider = GetComponent<Slider>();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        _textMaxTimeBigLetters.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        GetComponent<Slider>().maxValue = AnimationTimer.GetMaxTime();

        _toggleKeyConfigControlls.onValueChanged.AddListener(SwitchKeyConfigControls);
        UpdateTimeSlider();
        SwitchKeyConfigControls(false);
        ChangeControlsFromTimelineSelection();

        //place vertical timelines
        if (SceneManaging.isExpert)
        {
            foreach (GameObject vL in verticalTimeLines)
                vL.GetComponent<RectTransform>().sizeDelta = new Vector2(vL.GetComponent<RectTransform>().sizeDelta.x, 460);
        }
        for (int i = 0; i <= AnimationTimer._maxTime / 60; i++)
        {
            verticalTimeLines[i].SetActive(true);
            timeStamps[i].SetActive(true);
            float posX = UtilitiesTm.FloatRemap(i * 60, 0, AnimationTimer.GetMaxTime(), 0, railwidthAbsolute);
            verticalTimeLines[i].transform.localPosition = new Vector2(posX, verticalTimeLines[i].transform.localPosition.y);
            timeStamps[i].transform.localPosition = new Vector2(posX, timeStamps[i].transform.localPosition.y);
        }
    }
    void Update()
    {
        _thisSlider.value = AnimationTimer.GetTime();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        if (SceneManaging.fullscreenOn)
        {
            _textFullScreen.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        }
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        _textMaxTimeBigLetters.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        if (ImageTimelineSelection.UpdateNecessary())
        {
            ChangeControlsFromTimelineSelection();
        }
        if (_thisSlider.value == AnimationTimer.GetMaxTime())
        {
            playerCtrls.ButtonStop();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (_settingsLength.GetComponent<BoxCollider2D>() != Physics2D.OverlapPoint(Input.mousePosition))
                _settingsLength.SetActive(false);
        }
    }
    public void OnPointerUp(PointerEventData data)
    {
        SceneManaging.updateMusic = false;
    }
    public void OnDrag(PointerEventData data)
    {
        SceneManaging.updateMusic = true;
    }
    public void UpdateTimeSlider()
    {
        if (!SceneManaging.tutorialActive)
            AnimationTimer.SetTime(_thisSlider.value);
        if (tmpRailMusicManager.playingSample)
        {
            tmpRailMusicManager.playingSample = false;
            tmpRailMusicManager.sampleImages[tmpRailMusicManager.sampleButtonPressed].color = new Color(1, 1, 1, 0);
        }
    }
    void SwitchKeyConfigControls(bool value)
    {
        _panelControls.SetActive(value);
        //Debug.LogWarning("_toggleKeyConfigControlls " + value);
    }
    void ChangeControlsFromTimelineSelection()
    {
        _panelRailSpeedControls.SetActive(false);
        _panelLightControls.SetActive(false);
        _panelBackgroundPositionControls.SetActive(false);
        if (SceneManaging.isExpert)
        {
            switch (ImageTimelineSelection.GetRailType())
            {
                case 0:
                    _panelRailSpeedControls.SetActive(true);

                    break;
                case 1:
                    _panelLightControls.SetActive(true);
                    break;
                case 2:
                    break;
                case 3:
                    _panelBackgroundPositionControls.SetActive(true);
                    break;
            }
        }
        else
        {
            switch (ImageTimelineSelection.GetRailType())
            {
                case 0:
                    imgTimelineSettingsArea.SetActive(false);
                    foreach (GameObject key in _keyButtons) key.gameObject.SetActive(false);
                    break;
                case 1:
                    _panelLightControls.SetActive(true);
                    foreach (GameObject key in _keyButtons) key.gameObject.SetActive(true);
                    imgTimelineSettingsArea.SetActive(true);
                    break;
                case 2:
                    imgTimelineSettingsArea.SetActive(false);
                    foreach (GameObject key in _keyButtons) key.gameObject.SetActive(false);
                    break;
                case 3:
                    imgTimelineSettingsArea.SetActive(false);
                    foreach (GameObject key in _keyButtons) key.gameObject.SetActive(false);
                    break;
            }
        }

    }
    public void ChangeMaxLength()
    {
        //Figuren neu berechnen (moment berechnen)
        tmpRailManager.CalculateFigures(_sliderMaxLength.value);

        tmpAnimTimer.SetMaxTime(60 * (int)_sliderMaxLength.value);
        _inputSliderLength.text = _sliderMaxLength.value.ToString("0");
        GetComponent<Slider>().maxValue = 60 * _sliderMaxLength.value;
        StaticSceneData.StaticData.pieceLength = int.Parse(_inputSliderLength.text);

        // update rail keyframes
        JumpToKeyframe tmpJump = GetComponent<JumpToKeyframe>();
        //delete keyframes later than max time
        tmpJump.DeleteKeyframeFromChangeMaxLength();

        //update Light
        tmpLightAnim.ChangeImage();

        // update music
        for (int j = 0; j < tmpRailMusicManager.myObjects.Count; j++)
        {
            RectTransform tmpRectTransform = tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<RectTransform>();
            float tmpMoment = UtilitiesTm.FloatRemap(tmpRailMusicManager.myObjects[j].position.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
            float objectAnimationLength = tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<MusicLength>().musicLength;
            float rectSize = objectAnimationLength / (60 * _sliderMaxLength.value) * railwidthAbsolute;
            float posX = UtilitiesTm.FloatRemap(tmpMoment, 0, _sliderMaxLength.value * 60, 0, railwidthAbsolute);

            tmpRectTransform.sizeDelta = new Vector2(rectSize, tmpRectTransform.sizeDelta.y);
            tmpRailMusicManager.myObjects[j].musicPiece.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, tmpRectTransform.sizeDelta.y);
            tmpRectTransform.anchoredPosition = new Vector2(posX, tmpRectTransform.anchoredPosition.y);

            tmpRailMusicManager.myObjects[j].position = new Vector2(posX, rectSize);
        }



        //update vertical time lines
        for (int i = 0; i < verticalTimeLines.Length; i++)
        {
            verticalTimeLines[i].SetActive(false);
            timeStamps[i].SetActive(false);
        }

        for (int i = 0; i <= AnimationTimer._maxTime / 60; i++)
        {
            verticalTimeLines[i].SetActive(true);
            timeStamps[i].SetActive(true);
            float posX = UtilitiesTm.FloatRemap(i * 60, 0, AnimationTimer.GetMaxTime(), 0, railwidthAbsolute);
            verticalTimeLines[i].transform.localPosition = new Vector2(posX, verticalTimeLines[i].transform.localPosition.y);
            timeStamps[i].transform.localPosition = new Vector2(posX, timeStamps[i].transform.localPosition.y);
        }

        //update background
        tmpBG.ChangeCurve();
    }
    public void PressInfoButton(bool on)
    {
        if (on)
        {
            if (!SceneManaging.tutorialActive && !SceneManaging.aboutActive)
                _settingsLength.SetActive(true);
        }
        else
        {
            _settingsLength.SetActive(false);
        }
    }
    public void ChangeSliderValue()
    {
        _sliderMaxLength.value = int.Parse(_inputSliderLength.text);
        tmpAnimTimer.SetMaxTime(60 * (int)_sliderMaxLength.value);
        GetComponent<Slider>().maxValue = 60 * _sliderMaxLength.value;
        StaticSceneData.StaticData.pieceLength = 60 * int.Parse(_inputSliderLength.text);
        GetComponent<Slider>().maxValue = 60 * _sliderMaxLength.value;
    }
}
