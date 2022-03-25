//Kris version fuer den UI-Slider als Time Slider
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSliderController : MonoBehaviour
{
    [SerializeField] private Text _textTime;
    private Slider _thisSlider;
    // Start is called before the first frame update
    void Start()
    {
        _thisSlider = GetComponent<Slider>();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
        _thisSlider.onValueChanged.AddListener(delegate { UpdateTimeSLider(); });
    }

    // Update is called once per frame
    void Update()
    {
        _thisSlider.value = AnimationTimer.GetTime();
        _textTime.text = UtilitiesTm.FloaTTimeToString(_thisSlider.value);
    }

    void UpdateTimeSLider()
    {
        AnimationTimer.SetTime(_thisSlider.value);
    }


}
