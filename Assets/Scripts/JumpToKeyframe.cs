using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpToKeyframe : MonoBehaviour
{
    List<float> keyFrames;
    private int _selection;
    public int _railSelection;
    [SerializeField] private GameObject _panelColorGradient;
    [SerializeField] private GameObject _imageTimelineRailBg;
    public GameObject[] _railPanelsLineDraw;
    [SerializeField] private Slider _sliderLightIntensity;
    [SerializeField] private Slider _sliderRailSpeed;
    [SerializeField] private Slider _sliderBackgroundPosition;

    void Start()
    {
        keyFrames = new List<float>();
        ChangeSelection(ImageTimelineSelection.GetRailType());         //0 - railspeed; 1 - light; 2 - music; 3 - background
        ChangeRailSelection(ImageTimelineSelection.GetRailNumber());     //der index der Rails in der Rail-Liste
    }
    void Update()
    {
        if (ImageTimelineSelection.UpdateNecessary())
        {
            ChangeSelection(ImageTimelineSelection.GetRailType());
            ChangeRailSelection(ImageTimelineSelection.GetRailNumber());
        }
    }
    public void ChangeSelection(int selectSelection)
    {
        if (selectSelection >= 0 && selectSelection <= 3)
            _selection = selectSelection;
        else
            _selection = 1;
    }
    public void ChangeRailSelection(int selectRailSelection)
    {
        if (selectRailSelection >= 0 && selectRailSelection <= 5)
            _railSelection = selectRailSelection;
        else
            _railSelection = 0;
    }
    public void PreviousKeyframe()
    {
        //Debug.LogError("<-- BING! BING! BING!");
        SelectKeyFrames();
        keyFrames.Sort();
        float timerNow = AnimationTimer.GetTime();
        float previousKeyframe = 0;
        int knobIndex = 0;
        int railIndex = 0;

        for (int i = 0; i < keyFrames.Count; i++)
        {
            if (keyFrames[i] < timerNow)
            {
                previousKeyframe = keyFrames[i];
                knobIndex = keyFrames.Count - 1 - i;
            }
        }

        if (ImageTimelineSelection.GetRailType() == 0)
        {
            railIndex = ImageTimelineSelection.GetRailNumber();
            _sliderRailSpeed.value = StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds[knobIndex].speed;
            // Debug.Log("rail speed: " + StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds[knobIndex].speed);
        }
        else if (ImageTimelineSelection.GetRailType() == 1)
        {
            // Debug.Log("lighting set: " + StaticSceneData.StaticData.lightingSets[knobIndex].intensity);
            _sliderLightIntensity.value = StaticSceneData.StaticData.lightingSets[knobIndex].intensity;
            // Debug.Log("value: " + _sliderLightIntensity.value);
        }
        else if (ImageTimelineSelection.GetRailType() == 3)
        {
            _sliderBackgroundPosition.value = StaticSceneData.StaticData.backgroundPositions[knobIndex].yPosition;
        }
        AnimationTimer.SetTime(previousKeyframe);
    }
    public void NextKeyframe()
    {
        //Debug.LogError("BONG! BONG! BONG! -->");
        SelectKeyFrames();
        keyFrames.Reverse();
        float timerNow = AnimationTimer.GetTime();
        float nextKeyframe = AnimationTimer.GetMaxTime();
        int knobIndex = 0;
        int railIndex = 0;

        for (int i = 0; i < keyFrames.Count; i++)   // (float keyFrame in keyFrames)
        {
            if (keyFrames[i] > timerNow)
            {
                nextKeyframe = keyFrames[i];
                // Debug.Log("i: " + (keyFrames.Count - 1 - i) + ", Keyframe: " + nextKeyframe);
                knobIndex = keyFrames.Count - 1 - i;
            }
        }
        if (ImageTimelineSelection.GetRailType() == 0)
        {
            railIndex = ImageTimelineSelection.GetRailNumber();
            _sliderRailSpeed.value = StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds[knobIndex].speed;
            // Debug.Log("rail speed: " + StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds[knobIndex].speed);
        }
        else if (ImageTimelineSelection.GetRailType() == 1)
        {
            // Debug.Log("lighting set: " + StaticSceneData.StaticData.lightingSets[knobIndex].intensity);
            _sliderLightIntensity.value = StaticSceneData.StaticData.lightingSets[knobIndex].intensity;
            // Debug.Log("value: " + _sliderLightIntensity.value);
        }
        else if (ImageTimelineSelection.GetRailType() == 3)
        {
            _sliderBackgroundPosition.value = StaticSceneData.StaticData.backgroundPositions[knobIndex].yPosition;
        }
        AnimationTimer.SetTime(nextKeyframe);
        // float intensityValue = SliderIntensity.value;
    }
    public void DeleteCurrentKeyframe(bool changeMaxLength)
    {
        int momentIndex = -1;
        switch (_selection)
        {
            case 0:     //Rails
                momentIndex = StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds.FindIndex(mom => mom.moment == AnimationTimer.GetTime());
                if(changeMaxLength && momentIndex > AnimationTimer._maxTime)
                {
                    StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds.Remove(StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds[momentIndex]);
                    _railPanelsLineDraw[_railSelection].GetComponent<DrawCurve>().ChangeCurve();
                }
                else if (momentIndex > 0)
                {
                    StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds.Remove(StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds[momentIndex]);
                    _railPanelsLineDraw[_railSelection].GetComponent<DrawCurve>().ChangeCurve();
                }
                break;
            case 1:     // Light
                momentIndex = StaticSceneData.StaticData.lightingSets.FindIndex(mom => mom.moment == AnimationTimer.GetTime());
                if(changeMaxLength && momentIndex > AnimationTimer._maxTime)
                {
                    StaticSceneData.StaticData.lightingSets.Remove(StaticSceneData.StaticData.lightingSets[momentIndex]);
                    _panelColorGradient.GetComponent<LightAnimationRepresentation>().ChangeImage();
                }
                else if (momentIndex > 0)
                {
                    StaticSceneData.StaticData.lightingSets.Remove(StaticSceneData.StaticData.lightingSets[momentIndex]);
                    _panelColorGradient.GetComponent<LightAnimationRepresentation>().ChangeImage();
                }
                break;
            case 2:
                //GetMomentsFromMusicVolume
                break;
            case 3:     //BackgroundPosition
                momentIndex = StaticSceneData.StaticData.backgroundPositions.FindIndex(mom => mom.moment == AnimationTimer.GetTime());
                if(changeMaxLength && momentIndex > AnimationTimer._maxTime)
                {
                    StaticSceneData.StaticData.backgroundPositions.Remove(StaticSceneData.StaticData.backgroundPositions[momentIndex]);
                    _imageTimelineRailBg.GetComponent<DrawCurveBg>().ChangeCurve();
                }
                else if (momentIndex > 0)
                {
                    StaticSceneData.StaticData.backgroundPositions.Remove(StaticSceneData.StaticData.backgroundPositions[momentIndex]);
                    _imageTimelineRailBg.GetComponent<DrawCurveBg>().ChangeCurve();
                }
                break;
        }
    }
    void GetMomentsFromRailElementSpeeds()
    {
        keyFrames.Clear();
        foreach (RailElementSpeed railElementSpeed in StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds)
        {
            keyFrames.Add(railElementSpeed.moment);
        }
    }
    void GetMomentsFromLightingSets()
    {
        keyFrames.Clear();
        foreach (LightingSet lightingSet in StaticSceneData.StaticData.lightingSets)
            keyFrames.Add(lightingSet.moment);
    }
    void GetMomentFromBackgroundPositions()
    {
        keyFrames.Clear();
        foreach (BackgroundPosition backgroundPosition in StaticSceneData.StaticData.backgroundPositions)
        {
            keyFrames.Add(backgroundPosition.moment);
        }
    }
    void SelectKeyFrames() //0 - railspeed; 1 - light; 2 - music; 3 - background
    {
        switch (_selection)
        {
            case 0:
                GetMomentsFromRailElementSpeeds();
                break;
            case 1:
                GetMomentsFromLightingSets();
                break;
            case 2:
                //GetMomentsFromMusicVolume
                break;
            case 3:
                GetMomentFromBackgroundPositions();
                break;
        }

    }
}
