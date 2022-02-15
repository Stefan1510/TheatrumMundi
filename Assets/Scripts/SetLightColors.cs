using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLightColors : MonoBehaviour
{
    public GameObject[] ObjectsLights;
    public Slider SliderIntensity;

    //[SerializeField] private Slider _sliderTime;
    [SerializeField] private Image _imagePositionKnob;
    [SerializeField] private GameObject _representationPanel;

    private float _time;
    // Start is called before the first frame update
    private void Awake()
    {

    }

    void Start()
    {
        Color32 lightStartColor = new Color(ObjectsLights[0].GetComponent<Light>().color.r, ObjectsLights[0].GetComponent<Light>().color.g, ObjectsLights[0].GetComponent<Light>().color.b);
        StaticSceneData.StaticData.lightingSets.Add(new LightingSet
        {
            moment = 0,
            r = lightStartColor.r,
            g = lightStartColor.g,
            b = lightStartColor.b,
            intensity = ObjectsLights[0].GetComponent<Light>().intensity
        });
        _imagePositionKnob.gameObject.SetActive(false);

        StaticSceneData.StaticData.lightingSets.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die LightPropertiesList anhand der Eigenschaft moment

        float knobPos = AnimationTimer.GetTime();
        knobPos = UtilitiesTm.FloatRemap(knobPos, AnimationTimer.GetMinTime(), AnimationTimer.GetMaxTime(), 0, _representationPanel.GetComponent<RectTransform>().rect.width);
        Image knobIntstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
        knobIntstance.gameObject.SetActive(true);
        knobIntstance.transform.localPosition = new Vector3(knobPos, knobIntstance.transform.localPosition.y, knobIntstance.transform.localPosition.z);
        knobIntstance.GetComponent<Image>().color = new Color(StaticSceneData.StaticData.lightingSets[0].r, StaticSceneData.StaticData.lightingSets[0].g, StaticSceneData.StaticData.lightingSets[0].b);
    }

    public void ButtonClick(int buttonColor)
    {
        float intensityValue = SliderIntensity.value;
        float colorMoment = AnimationTimer.GetTime();
        int momentIndex = StaticSceneData.StaticData.lightingSets.FindIndex(mom => mom.moment == colorMoment);
        Debug.Log("mommentIndex: " + momentIndex);
        //Debug.Log("sizeDeltsa.x "+_representationPanel.GetComponent<RectTransform>().rect.width);
        float knobPos = UtilitiesTm.FloatRemap(colorMoment, AnimationTimer.GetMinTime(), AnimationTimer.GetMaxTime(), 0, _representationPanel.GetComponent<RectTransform>().rect.width);
        Image knobIntstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
        knobIntstance.gameObject.SetActive(true);
        knobIntstance.transform.localPosition = new Vector3(knobPos, knobIntstance.transform.localPosition.y, knobIntstance.transform.localPosition.z);
        LightingSet thisLightingSet = new LightingSet { moment = colorMoment, r = 255, g = 231, b = 121, intensity = intensityValue };
        switch (buttonColor)
        {
            case 0:     //yellow
                knobIntstance.GetComponent<Image>().color = new Color32(255, 231, 121, 255);
                thisLightingSet = new LightingSet { moment = colorMoment, r = 255, g = 231, b = 121, intensity = intensityValue };
                break;
            case 1:     //red
                knobIntstance.GetComponent<Image>().color = new Color32(255, 105, 104, 255);
                thisLightingSet = new LightingSet { moment = colorMoment, r = 255, g = 105, b = 104, intensity = intensityValue };
                break;
            case 2:     //blue
                knobIntstance.GetComponent<Image>().color = new Color32(0, 174, 239, 255);
                thisLightingSet = new LightingSet { moment = colorMoment, r = 0, g = 174, b = 255, intensity = intensityValue };
                break;
        }
        if (momentIndex == -1)
        {
            StaticSceneData.StaticData.lightingSets.Add(thisLightingSet);
        }
        else
        {
            StaticSceneData.StaticData.lightingSets[momentIndex] = new LightingSet { moment = thisLightingSet.moment, r = thisLightingSet.r, g = thisLightingSet.g, b = thisLightingSet.b, intensity = thisLightingSet.intensity };
        }
        StaticSceneData.StaticData.lightingSets.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die LightPropertiesList anhand der Eigenschaft moment
        _representationPanel.GetComponent<LightAnimationRepresentation>().ChangeImage();
    }

    // Update is called once per frame
    void Update()
    {
        //_time = AnimationTimer.GetTime();

        //if (AnimationTimer.GetTimerState() == AnimationTimer.TimerState.playing)
        //{
        //    if (_time <= _sliderTime.maxValue)
        //    {
        //        _sliderTime.GetComponent<Slider>().value = _time;
        //    }
        //    else
        //    {
        //        AnimationTimer.StopTimer();
        //        _time = 0;
        //        _sliderTime.GetComponent<Slider>().value = _time;
        //    }
        //}
    }
}