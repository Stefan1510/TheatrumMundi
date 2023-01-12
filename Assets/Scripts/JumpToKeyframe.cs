using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToKeyframe : MonoBehaviour
{
    List<float> keyFrames;
    private int _selection;
    private int _railSelection;
    [SerializeField] private GameObject _panelColorGradient;
    [SerializeField] private GameObject _imageTimelineRailBg;
    [SerializeField] private GameObject[] _railPanelsLineDraw;
    // Start is called before the first frame update
    void Start()
    {
        keyFrames = new List<float>();
        ChangeSelection(ImageTimelineSelection.GetRailType());         //0 - railspeed; 1 - light; 2 - music; 3 - background
        ChangeRailSelection(ImageTimelineSelection.GetRailNumber());     //der index der Rails in der Rail-Liste
    }

    // Update is called once per frame
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
        if (selectSelection >= 0 && selectSelection <= 3 )
        {
            _selection = selectSelection;
        }
        else
        {
            _selection = 1;
        }
    }
    public void ChangeRailSelection(int selectRailSelection)
    {
        if (selectRailSelection >= 0 && selectRailSelection <= 5)
        {
            _railSelection = selectRailSelection;
        }
        else
        {
            _railSelection = 0;
        }
    }


    public void PreviousKeyframe()
    {
        //Debug.LogError("<-- BING! BING! BING!");
        SelectKeyFrames();
        keyFrames.Sort();
        float timerNow = AnimationTimer.GetTime();
        float previousKeyframe = 0;
        foreach (float keyFrame in keyFrames)
        {
            if (keyFrame < timerNow)
            {
                previousKeyframe = keyFrame;
            }
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
        foreach (float keyFrame in keyFrames)
        {
            if (keyFrame > timerNow)
            {
                nextKeyframe = keyFrame;
            }
        }
        AnimationTimer.SetTime(nextKeyframe);
    }

    public void DeleteCurrentKeyframe()
    {
        int momentIndex = -1;
        switch (_selection)
        {
            case 0:     //Rails
                momentIndex = StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds.FindIndex(mom => mom.moment == AnimationTimer.GetTime());
                if (momentIndex > 0)
                {
                    StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds.Remove(StaticSceneData.StaticData.railElements[_railSelection].railElementSpeeds[momentIndex]);
                    _railPanelsLineDraw[_railSelection].GetComponent<DrawCurve>().ChangeCurve();
                }
                break;
            case 1:     // Light
                momentIndex = StaticSceneData.StaticData.lightingSets.FindIndex(mom => mom.moment == AnimationTimer.GetTime());
                Debug.LogWarning("Licht l�schen! +++ momentIndex " + momentIndex);
                if (momentIndex > 0)
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
                if (momentIndex > 0)
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
        {
            keyFrames.Add(lightingSet.moment);
            //Debug.LogWarning(lightingSet.moment);
        }
    }

    void GetMomentFromBackgroundPositions()
    {
        keyFrames.Clear();
        foreach(BackgroundPosition backgroundPosition in StaticSceneData.StaticData.backgroundPositions)
        {
            keyFrames.Add(backgroundPosition.moment);
        }
    }


    void SelectKeyFrames() //0 - railspeed; 1 - light; 2 - music; 3 - background
    {
        switch(_selection)
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
