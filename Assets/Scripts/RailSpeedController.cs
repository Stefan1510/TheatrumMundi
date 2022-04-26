using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSpeedController : MonoBehaviour
{
    public int railIndex;
    public float speed;

    //private float _goneSeconds = 0;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, 0, 2 + GetDistanceAtTime(AnimationTimer.GetTime()) * speed);
        //Debug.Log(speed);
        //_goneSeconds += Time.deltaTime;
        //if (_goneSeconds >= 10 )
        //{
        //    Debug.LogWarning(GetDistanceAtTime(AnimationTimer.GetTime()));
        //}
    }


    //  t is the elapsed time
    //  t1, t2 are two moments, where t1 < than t2
    //  s0 initial distance from the origin
    //  s(t) is the distance from the origin at time t
    //  v0 is the initial velocity,
    //  v(t) is the velocity at time t 
    //  v1 and v2 are the velocities at time t1 and t2
    //  a is the uniform rate of acceleration.
    public float GetDistanceAtTime(float t)    //time in Sekunden
    {
        float distance = 0;
        float vt = 0, t1 = 0, t2 = 0, v1 = 0, v2 = 0;
        List<RailElementSpeed> railElementSpeeds = StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds;
        int momentAfter = railElementSpeeds.FindIndex(speed => speed.moment > t);      //sucht von vorne aus und findet den ersten Moment, der nach t liegt
        int momentBefore = railElementSpeeds.FindLastIndex(speed => speed.moment <= t); //sucht von hinten aus und findet den ersten Moment, der vor t liegt
        Debug.LogWarning("Before: " + momentBefore + " - After: " + momentAfter);
        if (momentBefore == 0 && momentAfter == -1) //ein momentBefore existiert immer, da der erste Wert von railElementSpeeds bei Programmstart gesetzt wird
        {
            v1 = v2 = railElementSpeeds[0].speed;
            distance = GetDistanceBetweenTwoMoments(0, t, v1, v2);
        }
        else if (momentAfter == -1)
        {
            for (int i = 0; i < momentBefore; i++)
            {
                t1 = railElementSpeeds[i].moment;
                t2 = railElementSpeeds[i + 1].moment;
                v1 = railElementSpeeds[i].speed;
                v2 = railElementSpeeds[i + 1].speed;
                distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v2);
            }
            t1 = t2;
            t2 = t;
            v1 = v2;
            v2 = railElementSpeeds[momentBefore].speed;
            distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v2);
        }
        else if (momentBefore == 0)
        {
            vt = UtilitiesTm.FloatRemap(t, railElementSpeeds[momentBefore].moment, railElementSpeeds[momentAfter].moment, railElementSpeeds[momentBefore].speed, railElementSpeeds[momentAfter].speed);
            v1 = railElementSpeeds[0].speed;
            v2 = vt;
            distance = GetDistanceBetweenTwoMoments(0, t, v1, v2);
        }
        else
        {
            vt = UtilitiesTm.FloatRemap(t, railElementSpeeds[momentBefore].moment, railElementSpeeds[momentAfter].moment, railElementSpeeds[momentBefore].speed, railElementSpeeds[momentAfter].speed);
            Debug.LogWarning("vt: " + vt);
            for (int i = 0; i < momentBefore; i++)
            {
                t1 = railElementSpeeds[i].moment;
                t2 = railElementSpeeds[i + 1].moment;
                v1 = railElementSpeeds[i].speed;
                v2 = railElementSpeeds[i + 1].speed;
                distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v2);
            }
            t1 = t2;
            t2 = t;
            v1 = v2;
            v2 = vt;
            distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v2);
        }
        return distance;
    }

    float GetDistanceBetweenTwoMoments(float t1, float t2, float v1, float v2)
    {
        float s = 0;    //zur�ckegeleger Weg zwischen t1 und t2;
        float a = 0;
        float t = t2 - t1;
        if (v1 == v2)
        {
            s = v1 * t;
        }
        else
        {
            a = (v2 - v1) / (t2 - t1);
            s = 0.5f * a * t * t + v1 * t;
        }
        return s;
    }

}
