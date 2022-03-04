using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToKeyframe : MonoBehaviour
{
    List<float> keyFrames;
    // Start is called before the first frame update
    void Start()
    {
        keyFrames = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PreviousKeyframe()
    {
        //Debug.LogError("<-- BING! BING! BING!");
        keyFrames = getMomentsFromLightAnimation();
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
        keyFrames = getMomentsFromLightAnimation();
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

    List<float> getMomentsFromLightAnimation()
    {
        List<float> moments = new List<float>();
        List<LightingSet> lightingSets = StaticSceneData.StaticData.lightingSets;
        foreach (LightingSet lightingSet in lightingSets)
        {
            moments.Add(lightingSet.moment);
            //Debug.LogWarning(lightingSet.moment);
        }
        return moments;
    }

}
