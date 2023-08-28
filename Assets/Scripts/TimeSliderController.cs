using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

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
    [SerializeField] private RailSpeedController tmpRailSpeedContr;
    [SerializeField] private RailMusicManager tmpRailMusicManager;
    [SerializeField] private LightAnimationRepresentation tmpLightAnim;
    [SerializeField] private UTJ.FrameCapturer.PlayerControls playerCtrls;
    private float railwidthAbsolute = 1670.4f;
    private Slider _thisSlider;
    #endregion
    void Start()
    {
        // if (SceneManaging.isExpert)
        imgTimelineSettingsArea.SetActive(true);
        _thisSlider = GetComponent<Slider>();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        _textMaxTimeBigLetters.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        GetComponent<Slider>().maxValue = AnimationTimer.GetMaxTime();

        _toggleKeyConfigControlls.onValueChanged.AddListener((bool value) => SwitchKeyConfigControls(value));
        UpdateTimeSlider();
        SwitchKeyConfigControls(false);
        ChangeControlsFromTimelineSelection();
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
        //Debug.Log("value: " + _thisSlider.value);
        if (!SceneManaging.tutorialActive)
        {
            //AnimationTimer.SetTime(_thisSlider.value);
            AnimationTimer.SetTime(_thisSlider.value);
            //Debug.Log("value: "+value);
        }
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
        for (int i = 0; i < tmpRailManager.railList.Length; i++)
        {
            for (int j = 0; j < tmpRailManager.railList[i].myObjects.Count; j++)
            {
                RectTransform tmpRectTransform = tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>();
                float tmpMoment = UtilitiesTm.FloatRemap(tmpRailManager.railList[i].myObjects[j].position.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                float objectAnimationLength = tmpRailSpeedContr.GetEndTimeFromStartTime(tmpMoment);
                float rectSize = objectAnimationLength / (60 * _sliderMaxLength.value) * railwidthAbsolute;
                float posX = UtilitiesTm.FloatRemap(tmpMoment, 0, _sliderMaxLength.value * 60, 0, railwidthAbsolute);

                tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>().sizeDelta.y);
                tmpRailManager.railList[i].myObjects[j].figure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>().sizeDelta.y);
                tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>().anchoredPosition.y);

                tmpRailManager.railList[i].myObjects[j].position = new Vector2(posX, rectSize);
            }
        }

        // update music
        for (int j = 0; j < tmpRailMusicManager.myObjects.Count; j++)
        {
            RectTransform tmpRectTransform = tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<RectTransform>();
            float tmpMoment = UtilitiesTm.FloatRemap(tmpRailMusicManager.myObjects[j].position.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
            float objectAnimationLength = tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<MusicLength>().musicLength;
            float rectSize = objectAnimationLength / (60 * _sliderMaxLength.value) * railwidthAbsolute;
            float posX = UtilitiesTm.FloatRemap(tmpMoment, 0, _sliderMaxLength.value * 60, 0, railwidthAbsolute);

            tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<RectTransform>().sizeDelta.y);
            tmpRailMusicManager.myObjects[j].musicPiece.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<RectTransform>().sizeDelta.y);
            tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, tmpRailMusicManager.myObjects[j].musicPiece.GetComponent<RectTransform>().anchoredPosition.y);

            tmpRailMusicManager.myObjects[j].position = new Vector2(posX, rectSize);
        }

        tmpAnimTimer.SetMaxTime(60 * (int)_sliderMaxLength.value);
        // Debug.Log("value: "+(int)_sliderMaxLength.value);
        _inputSliderLength.text = _sliderMaxLength.value.ToString("0");
        GetComponent<Slider>().maxValue = (60 * _sliderMaxLength.value);
        StaticSceneData.StaticData.pieceLength = int.Parse(_inputSliderLength.text);
        // Debug.Log("length "+StaticSceneData.StaticData.pieceLength);
        // update light
        tmpLightAnim.ChangeImage();
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
        // Debug.Log("change slider max length");
        tmpAnimTimer.SetMaxTime(60 * (int)_sliderMaxLength.value);
        GetComponent<Slider>().maxValue = (60 * _sliderMaxLength.value);
        StaticSceneData.StaticData.pieceLength = 60 * int.Parse(_inputSliderLength.text);
        GetComponent<Slider>().maxValue = 60 * _sliderMaxLength.value;
    }
}
