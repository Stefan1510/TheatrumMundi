using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class TimeSliderController : MonoBehaviour, IPointerUpHandler, IDragHandler
{
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

    private Slider _thisSlider;
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
                for (int k = 0; k < StaticSceneData.StaticData.figureElements[Int32.Parse(tmpRailManager.railList[i].myObjects[j].figure.name.Substring(6, 2)) - 1].figureInstanceElements.Count; k++)
                {
                    //Debug.Log("name: "+StaticSceneData.StaticData.figureElements[Int32.Parse(tmpRailManager.railList[i].myObjects[j].figure.name.Substring(6, 2)) - 1].figureInstanceElements[k].name+", name2: "+tmpRailManager.railList[i].myObjects[j].figure.name);
                    if (StaticSceneData.StaticData.figureElements[Int32.Parse(tmpRailManager.railList[i].myObjects[j].figure.name.Substring(6, 2)) - 1].figureInstanceElements[k].instanceNr == int.Parse(tmpRailManager.railList[i].myObjects[j].figure.name.Substring(16)))
                    {
                        Debug.Log("figure: " + StaticSceneData.StaticData.figureElements[Int32.Parse(tmpRailManager.railList[i].myObjects[j].figure.name.Substring(6, 2)) - 1].figureInstanceElements[k].name + ", moment: " + StaticSceneData.StaticData.figureElements[Int32.Parse(tmpRailManager.railList[i].myObjects[j].figure.name.Substring(6, 2)) - 1].figureInstanceElements[k].moment);
                        float tmpMoment = StaticSceneData.StaticData.figureElements[Int32.Parse(tmpRailManager.railList[i].myObjects[j].figure.name.Substring(6, 2)) - 1].figureInstanceElements[k].moment;
                        float posX = UtilitiesTm.FloatRemap(tmpMoment,0,_sliderMaxLength.value*60,0,1670.4f);
                        tmpRailManager.railList[i].myObjects[j].position = new Vector2(posX,tmpRailManager.railList[i].myObjects[j].position.y);
                        tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>().anchoredPosition = new Vector2(tmpRailManager.railList[i].myObjects[j].position.x,tmpRailManager.railList[i].myObjects[j].figure.GetComponent<RectTransform>().anchoredPosition.y);
                    }
                }
                //Debug.Log("figure: "+tmpRailManager.railList[i].myObjects[j].figure+", moment: "+tmpRailManager.railList[i].myObjects[j].position);
                //float moment = float moment = UtilitiesTm.FloatRemap((tmpRectTransform.anchoredPosition.x), 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
            }
        }
        //Debug.Log("value: " + (_sliderMaxLength.value));
        tmpAnimTimer.SetMaxTime(60 * _sliderMaxLength.value);
        _inputSliderLength.text = _sliderMaxLength.value.ToString("0");
        GetComponent<Slider>().maxValue = (60 * _sliderMaxLength.value);
        StaticSceneData.StaticData.pieceLength = 60 * int.Parse(_inputSliderLength.text);

    }
    public void PressInfoButton(bool on)
    {
        if (on)
            _settingsLength.SetActive(true);
        else
            _settingsLength.SetActive(false);
    }
    public void ChangeSliderValue()
    {
        _sliderMaxLength.value = int.Parse(_inputSliderLength.text);
        tmpAnimTimer.SetMaxTime(60 * _sliderMaxLength.value);
        GetComponent<Slider>().maxValue = (60 * _sliderMaxLength.value);
        StaticSceneData.StaticData.pieceLength = 60 * int.Parse(_inputSliderLength.text);
        GetComponent<Slider>().maxValue = 60 * _sliderMaxLength.value;
        tmpRailManager.recalculateLengthOfFigures();
    }
}
