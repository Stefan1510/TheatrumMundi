using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToKeyframe : MonoBehaviour
{
    List<float> keyFrames;
    private int _selection;
    private int _railSelection;
    // Start is called before the first frame update
    void Start()
    {
        keyFrames = new List<float>();
        ChangeSelection(1);
        ChangeRailSelection(0);
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

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
                //GetMomentsFromBackgroundPosition
                break;
        }
            
    }
}
