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
    [SerializeField] private Toggle _toggleKeyConfigControlls;
    [SerializeField] private GameObject _panelControls;
    [SerializeField] private GameObject _panelRailSpeedControls;
    [SerializeField] private GameObject _panelLightControls;
    [SerializeField] private GameObject _panelBackgroundPositionControls;

    private Slider _thisSlider;
    // Start is called before the first frame update
    void Start()
    {
        _thisSlider = GetComponent<Slider>();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
        //_thisSlider.onValueChanged.AddListener(delegate { UpdateTimeSLider(); });
        _thisSlider.onValueChanged.AddListener((float value) => UpdateTimeSlider(value));
        _toggleKeyConfigControlls.onValueChanged.AddListener((bool value) => SwitchKeyConfigControls(value));
        UpdateTimeSlider(0);
        SwitchKeyConfigControls(false);
        ChangeControlsFromTimelineSelection();
    }

    // Update is called once per frame
    void Update()
    {
        _thisSlider.value = AnimationTimer.GetTime();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
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
        //AnimationTimer.SetTime(_thisSlider.value);
        AnimationTimer.SetTime(value);
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
    }



}
