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
    [SerializeField] private Toggle _toggleKeyConfigControlls;
    [SerializeField] private GameObject _panelControls;
    [SerializeField] private GameObject _panelRailSpeedControls;
    [SerializeField] private GameObject _panelLightControls;

    [SerializeField] private Slider sliderTestTestTestSlider;
    [SerializeField] private Slider sliderTestTestTestSlider2;

    private Slider _thisSlider;
    // Start is called before the first frame update
    void Start()
    {
        _thisSlider = GetComponent<Slider>();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        //_thisSlider.onValueChanged.AddListener(delegate { UpdateTimeSLider(); });
        _thisSlider.onValueChanged.AddListener((float value) => UpdateTimeSlider(value));
        _toggleKeyConfigControlls.onValueChanged.AddListener((bool value) => SwitchKeyConfigControls(value));
        UpdateTimeSlider(0);
        SwitchKeyConfigControls(false);
        ChangeControlsFromTimelineSelection();
        sliderTestTestTestSlider.onValueChanged.AddListener((float testTestTest) => TestTestTestTest(testTestTest));
        sliderTestTestTestSlider2.onValueChanged.AddListener((float testTestTest) => TestTestTestTest2(testTestTest));

    }

    void TestTestTestTest(float testTestTest)
    {
        int selection = (int)testTestTest;
        ImageTimelineSelection.SetRailType(selection);
    }
    void TestTestTestTest2(float testTestTest)
    {
        int selection = (int)testTestTest;
        ImageTimelineSelection.SetRailNumber(selection);
    }


    // Update is called once per frame
    void Update()
    {
        _thisSlider.value = AnimationTimer.GetTime();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        if (ImageTimelineSelection.UpdateNecessary())
        {
            ChangeControlsFromTimelineSelection();
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        Debug.LogError("Timeslider OnPointerUP");   //bitte auskommentieren
    }

    public void OnDrag(PointerEventData data)
    {

        Debug.LogError("Timeslider Scrub");     //bitte auskommentieren
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
        switch(ImageTimelineSelection.GetRailType())
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
                break;
        }
    }



}
