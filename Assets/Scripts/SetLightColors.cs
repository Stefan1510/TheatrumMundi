using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLightColors : MonoBehaviour
{
    public GameObject GameController;
    public Slider SliderIntensity;
    private objectsLightElement[] objectsLight;
    private int _savedButtonColor;
    [SerializeField] private GameObject _representationPanel;

    private void Awake()
    {
        //objectsLight = GameObject.Find("GameController").GetComponent<SceneDataController>().objectsLightElements;
        objectsLight = GameController.GetComponent<SceneDataController>().objectsLightElements;
        _savedButtonColor = 0;
        ButtonClick(0);
        //ChangeLightColor();
        SliderIntensity.onValueChanged.AddListener((float value) => ChangeLiveColor(value));
    }

    public void ButtonClick(int buttonColor)
    {
        _savedButtonColor = buttonColor;
        ChangeLiveColor(SliderIntensity.value);
        float intensityValue = SliderIntensity.value;
        float colorMoment = AnimationTimer.GetTime();
        if (colorMoment == AnimationTimer.GetMaxTime())  // letzter frame kann nicht gesetzt werden
        {
            colorMoment -= 1;
        }
        int momentIndex = StaticSceneData.StaticData.lightingSets.FindIndex(mom => mom.moment == colorMoment);
        LightingSet thisLightingSet = new LightingSet { moment = colorMoment, r = 255, g = 231, b = 121, intensity = intensityValue };
        switch (buttonColor)
        {
            case 0:     //yellow
                //knobIntstance.GetComponent<Image>().color = new Color32(255, 231, 121, 255);
                thisLightingSet = new LightingSet { moment = colorMoment, r = 255, g = 231, b = 121, intensity = intensityValue };
                break;
            case 1:     //red
                //knobIntstance.GetComponent<Image>().color = new Color32(255, 105, 104, 255);
                thisLightingSet = new LightingSet { moment = colorMoment, r = 255, g = 105, b = 104, intensity = intensityValue };
                break;
            case 2:     //blue
                //knobIntstance.GetComponent<Image>().color = new Color32(0, 174, 239, 255);
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
        ChangeLightColor();
    }

    public void ChangeAnimationLightIntensity()
    {
        ButtonClick(_savedButtonColor);
    }

    public void ChangeLightColor()
    {
        float TimeNow = AnimationTimer.GetTime();
        int _listLength = StaticSceneData.StaticData.lightingSets.Count;
        Color32 colorNow;
        float _interpolationStep;
        float _lightAnimationIntensity;
        float _lightSpotIntensity;
        for (int i = 0; i < _listLength - 1; i++)
        {
            if ((StaticSceneData.StaticData.lightingSets[i].moment <= TimeNow) && (TimeNow < StaticSceneData.StaticData.lightingSets[i + 1].moment))
            {
                _interpolationStep = (TimeNow - StaticSceneData.StaticData.lightingSets[i].moment) / (StaticSceneData.StaticData.lightingSets[i + 1].moment - StaticSceneData.StaticData.lightingSets[i].moment);
                Color32 color1 = new Color32(StaticSceneData.StaticData.lightingSets[i].r, StaticSceneData.StaticData.lightingSets[i].g, StaticSceneData.StaticData.lightingSets[i].b, 255);
                Color32 color2 = new Color32(StaticSceneData.StaticData.lightingSets[i + 1].r, StaticSceneData.StaticData.lightingSets[i + 1].g, StaticSceneData.StaticData.lightingSets[i + 1].b, 255);
                colorNow = Color32.Lerp(color1, color2, _interpolationStep);
                _lightAnimationIntensity = Mathf.Lerp(StaticSceneData.StaticData.lightingSets[i].intensity, StaticSceneData.StaticData.lightingSets[i + 1].intensity, _interpolationStep);

                foreach (objectsLightElement element in objectsLight)
                {
                    int lightIndex = StaticSceneData.StaticData.lightElements.FindIndex(le => le.name == element.goLightElement.name);
                    _lightSpotIntensity = StaticSceneData.StaticData.lightElements[lightIndex].intensity;
                    element.goLightElement.GetComponent<Light>().intensity = _lightAnimationIntensity * _lightSpotIntensity;
                    if (element.lightStagePosition != 0) //(element.lightStagePosition == 1 || element.lightStagePosition == 2 || element.lightStagePosition == 3)
                    {
                        element.goLightElement.GetComponent<Light>().color = new Color32(colorNow.r, colorNow.g, colorNow.b, 255);
                    }
                }
            }
        }
    }

    void ChangeLiveColor(float lightAnimationIntensityValue)
    {
        foreach (objectsLightElement element in objectsLight)
        {
            if (element.lightStagePosition != 0)    // alle ausser buehennbeleuchtung aendern farbe
            {
                switch (_savedButtonColor)
                {
                    case 0:     //yellow
                        element.goLightElement.GetComponent<Light>().color = new Color32(255, 231, 121, 255);
                        break;
                    case 1:     //red
                        element.goLightElement.GetComponent<Light>().color = new Color32(255, 105, 104, 255);
                        break;
                    case 2:     //blue
                        element.goLightElement.GetComponent<Light>().color = new Color32(0, 174, 255, 255);
                        break;
                }
            }
            int lightIndex = StaticSceneData.StaticData.lightElements.FindIndex(le => le.name == element.goLightElement.name);
            float _lightSpotIntensity = StaticSceneData.StaticData.lightElements[lightIndex].intensity;
            element.goLightElement.GetComponent<Light>().intensity = lightAnimationIntensityValue * _lightSpotIntensity;
        }
    }

    void Update()
    {
        //ChangeLightColor();

        //if (AnimationTimer.GetTimerState() == AnimationTimer.TimerState.playing)
        //{
        //    ChangeLightColor();
        //}
        //else if()
        //{

        //}
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