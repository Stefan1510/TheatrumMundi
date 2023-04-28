//Kris version fuer den UI-Slider als Time Slider
//
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimeSliderController : MonoBehaviour, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Text _textTime;
    [SerializeField] private Text _textMaxTime;
    [SerializeField] private Text _textFullScreen;
    [SerializeField] private Toggle _toggleKeyConfigControlls;
    [SerializeField] private GameObject _panelControls, _panelRailSpeedControls, _panelLightControls, _panelBackgroundPositionControls, imgTimelineSettingsArea;
    [SerializeField] private GameObject[] _keyButtons;

    private Slider _thisSlider;
    void Start()
    {
        // if (SceneManaging.isExpert)
         imgTimelineSettingsArea.SetActive(true);
        _thisSlider = GetComponent<Slider>();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        GetComponent<Slider>().maxValue = AnimationTimer.GetMaxTime();

        //_thisSlider.onValueChanged.AddListener(delegate { UpdateTimeSLider(); });
        _thisSlider.onValueChanged.AddListener((float value) => UpdateTimeSlider(value));
        _toggleKeyConfigControlls.onValueChanged.AddListener((bool value) => SwitchKeyConfigControls(value));
        UpdateTimeSlider(0);
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
        //Debug.Log("maxtime: "+AnimationTimer.GetMaxTime()+", slider: "+_thisSlider.value);
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

    void UpdateTimeSlider(float value)
    {

        if (!SceneManaging.tutorialActive)
        {
            //AnimationTimer.SetTime(_thisSlider.value);
            AnimationTimer.SetTime(value);
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
}
