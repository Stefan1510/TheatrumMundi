using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSpeedController : MonoBehaviour
{
    public int railIndex;
    public float speed;
    private float _speedAtTime;
    private float _durationFromTime;

    // Update is called once per frame
    void Update()
    {
        if (railIndex % 2 == 0)
        {
            transform.position = new Vector3(0, transform.parent.localPosition.y, 2 + GetDistanceAtTime(AnimationTimer.GetTime()) * speed);
        }
        else
        {
            transform.position = new Vector3(0, transform.parent.localPosition.y, -(2 + GetDistanceAtTime(AnimationTimer.GetTime()) * speed));
        }

        //Debug.Log(speed);
        //_goneSeconds += Time.deltaTime;
        //if (_goneSeconds >= 10 )
        //{
        //    Debug.LogWarning(GetDistanceAtTime(AnimationTimer.GetTime())*speed);
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

    

    public float GetEndTimeFromStartTime(float tStart, float distance = 4.1f)
    {
        float deltaT = 0;
        float vt = 0, t1 = 0, t2 = 0, v1 = 0, v2 = 0, a=0;
        float currentDistance = 0;

        List<RailElementSpeed> railElementSpeeds = StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds;
        int momentAfter = railElementSpeeds.FindIndex(speed => speed.moment > tStart);      //sucht von vorne aus und findet den ersten Moment, der nach t liegt
        int momentBefore = railElementSpeeds.FindLastIndex(speed => speed.moment <= tStart); //sucht von hinten aus und findet den ersten Moment, der vor t liegt
        int momentCount = railElementSpeeds.Count;
        
        // Debug.LogWarning("Before: " + momentBefore + " - After: " + momentAfter);
        if (momentAfter == -1) //ein momentBefore existiert immer, da der erste Wert von railElementSpeeds bei Programmstart gesetzt wird
        {
            v1 = v2 = railElementSpeeds[momentBefore].speed;
            //distance = GetDistanceBetweenTwoMoments(0, tStart, v1, v2);
            deltaT = 2 * distance / v1;
        }
        else
        {
            t1 = railElementSpeeds[momentBefore].moment;
            t2 = railElementSpeeds[momentAfter].moment;
            v1 = railElementSpeeds[momentBefore].speed;
            v2 = railElementSpeeds[momentAfter].speed;
            vt = UtilitiesTm.FloatRemap(tStart, t1, t2, v1, v2);
            currentDistance = GetDistanceBetweenTwoMoments(tStart, t2, vt, v2);

            Debug.LogWarning(" t1   ; tStart ;   t2   ;   v1   ;   vt   ;   v2   ; currentDistance \n   \t   " + t1 + " ; " + tStart + "  " + t2 + " ; " + v1 + " ; " + vt + " ; " + v2 + " ; " + currentDistance);


            if (currentDistance < distance)
            {
                deltaT += (t2 - tStart);
            }
            else
            {
                currentDistance = 0;
            }

            int i = momentAfter;
            //while (i < momentCount - 1 && currentDistance < distance)
            //{
            //    t1 = railElementSpeeds[i].moment;
            //    t2 = railElementSpeeds[i + 1].moment;
            //    v1 = railElementSpeeds[i].speed;
            //    v2 = railElementSpeeds[i + 1].speed;
            //    currentDistance += GetDistanceBetweenTwoMoments(t1, t2, v1, v2);
            //    if (currentDistance < distance)
            //    {
            //        deltaT += (t2 - t1);
            //    }
            //    i++;
            //}
            //if (currentDistance < distance)
            {
                t1 = railElementSpeeds[i].moment;
                t2 = railElementSpeeds[i + 1].moment;
                v1 = railElementSpeeds[i].speed;
                v2 = railElementSpeeds[i + 1].speed;
                a = (v2 - v1) / (t2 - t1);
                float sRest = distance - currentDistance;
                //float tRest = Mathf.Sqrt((2 * sRest) / a);
                float tRest = -(v1 / a) + Mathf.Sqrt(((v1 * v1) / (a * a)) + (2 * sRest / a));

                deltaT += tRest;
                Debug.LogWarning("sRest ; deltaT ; distance ; currentDistance \n" + sRest + " ; " + deltaT + " ; " + distance + " ; " + currentDistance);
            }
        }


        return deltaT;
    }

    public float GetDistanceAtTime(float t)    //time in Sekunden
    {
        float distance = 0;
        float vt = 0, t1 = 0, t2 = 0, v1 = 0, v2 = 0;
        List<RailElementSpeed> railElementSpeeds = StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds;
        int momentAfter = railElementSpeeds.FindIndex(speed => speed.moment > t);      //sucht von vorne aus und findet den ersten Moment, der nach t liegt
        int momentBefore = railElementSpeeds.FindLastIndex(speed => speed.moment <= t); //sucht von hinten aus und findet den ersten Moment, der vor t liegt
        // Debug.LogWarning("Before: " + momentBefore + " - After: " + momentAfter);
        if (momentBefore == 0 && momentAfter == -1) //ein momentBefore existiert immer, da der erste Wert von railElementSpeeds bei Programmstart gesetzt wird
        {
            v1 = v2 = railElementSpeeds[0].speed;
            distance = GetDistanceBetweenTwoMoments(0, t, v1, v2);
            _speedAtTime = v1;
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
            _speedAtTime = v2;
        }
        else if (momentBefore == 0)
        {
            vt = UtilitiesTm.FloatRemap(t, railElementSpeeds[momentBefore].moment, railElementSpeeds[momentAfter].moment, railElementSpeeds[momentBefore].speed, railElementSpeeds[momentAfter].speed);
            v1 = railElementSpeeds[0].speed;
            v2 = vt;
            distance = GetDistanceBetweenTwoMoments(0, t, v1, v2);
            _speedAtTime = vt;
        }
        else
        {
            vt = UtilitiesTm.FloatRemap(t, railElementSpeeds[momentBefore].moment, railElementSpeeds[momentAfter].moment, railElementSpeeds[momentBefore].speed, railElementSpeeds[momentAfter].speed);
            //Debug.LogWarning("vt: " + vt);
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
            _speedAtTime = vt;
        }
        return distance*Mathf.Abs(speed);
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

    public float GetSpeedAtTime(float t)
    {
        GetDistanceAtTime(t);
        return _speedAtTime;
    }

    public float GetDurationFromTime (float t)
    {
        float startDistance = GetDistanceAtTime(t);
        float endDistance = startDistance + 4.1f;
        float duration = 0;
        float distance = startDistance;

        while (distance <= endDistance)
        {
            distance = GetDistanceAtTime(t + duration);
            duration += 0.1f;
        }
        return duration;
    }

}
