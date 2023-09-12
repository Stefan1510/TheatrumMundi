using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSpeedController : MonoBehaviour
{
    public int railIndex;
    public float speed = 0;
    private float _speedAtTime;
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
        float v1, v2, t1, t2;
        float currentDistance = 0;
        float sRest = 0;

        List<RailElementSpeed> railElementSpeeds = StaticSceneData.StaticData.railElements[railIndex].railElementSpeeds;
        int momentAfter = railElementSpeeds.FindIndex(speed => speed.moment > tStart);      //sucht von vorne aus und findet den ersten Moment, der nach t liegt
        int momentBefore = railElementSpeeds.FindLastIndex(speed => speed.moment <= tStart); //sucht von hinten aus und findet den ersten Moment, der vor t liegt
        int momentCount = railElementSpeeds.Count;

        if (momentAfter == -1) //ein momentBefore existiert immer, da der erste Wert von railElementSpeeds bei Programmstart gesetzt wird
        {
            v1 = railElementSpeeds[momentBefore].speed;
            deltaT = distance / v1;
        }
        else
        {
            float tRest;

            v1 = railElementSpeeds[momentBefore].speed;
            t1 = railElementSpeeds[momentBefore].moment;
            t2 = railElementSpeeds[momentAfter].moment;
            v2 = railElementSpeeds[momentAfter].speed;

            // erst abstand ausrechnen bis 4.1
            tRest = distance / v1;

            if (tStart + tRest > t2)
            {
            }
            else
            {
                deltaT = tRest;
            }

            currentDistance = GetDistanceBetweenTwoMoments(tStart, t2, v1, v1);

            if (currentDistance < distance)
            {
                deltaT += t2 - tStart;
                sRest = distance - currentDistance;
                v1 = v2;
            }

            int i = momentAfter;

            // wenn bis zum naechsten Punkt noch keine 4,1 zurueckgelegt wurden
            while (i < momentCount - 1 && currentDistance < distance)
            {
                t1 = railElementSpeeds[i].moment;
                t2 = railElementSpeeds[i + 1].moment;
                v1 = railElementSpeeds[i].speed;
                v2 = railElementSpeeds[i + 1].speed;
                if (currentDistance + GetDistanceBetweenTwoMoments(t1, t2, v1, v1) < distance)
                {
                    currentDistance += GetDistanceBetweenTwoMoments(t1, t2, v1, v1);
                    deltaT += t2 - t1;
                }
                else
                {
                    sRest = distance - currentDistance;
                    break;
                }
                sRest -= currentDistance;
                i++;
            }
            tRest = sRest / v1;
            deltaT += tRest;
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
            distance = GetDistanceBetweenTwoMoments(0, t, v1, v1);
            _speedAtTime = v1;
        }
        else if (momentAfter == -1) // stimmt! 
        {
            for (int i = 0; i < momentBefore; i++)
            {
                t1 = railElementSpeeds[i].moment;
                t2 = railElementSpeeds[i + 1].moment;
                v1 = railElementSpeeds[i].speed;
                v2 = railElementSpeeds[i + 1].speed;
                distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v1);
                // Debug.Log("distance: "+distance);
            }
            t1 = t2;
            t2 = t;
            v1 = v2;
            v2 = railElementSpeeds[momentBefore].speed;
            distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v1);
            _speedAtTime = v2;
        }
        else if (momentBefore == 0) // stimmt noch nicht!
        {
            vt = UtilitiesTm.FloatRemap(t, railElementSpeeds[momentBefore].moment, railElementSpeeds[momentAfter].moment, railElementSpeeds[momentBefore].speed, railElementSpeeds[momentAfter].speed);
            v1 = railElementSpeeds[0].speed;
            v2 = vt;
            distance = GetDistanceBetweenTwoMoments(0, t, v1, v1);
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
                distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v1);
            }
            t1 = t2;
            t2 = t;
            v1 = v2;
            v2 = vt;
            distance += GetDistanceBetweenTwoMoments(t1, t2, v1, v1);
            _speedAtTime = vt;
        }
        return distance * Mathf.Abs(speed);
    }
    float GetDistanceBetweenTwoMoments(float t1, float t2, float v1, float v2)
    {
        float s = 0;    //zurueckgelegter Weg zwischen t1 und t2;
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
        //Debug.Log("zurueckgelegter weg zwischen "+ t1+" und "+t2+": "+s);
        return s;
    }
    public float GetSpeedAtTime(float t)
    {
        GetDistanceAtTime(t);
        return _speedAtTime;
    }
    public float GetDurationFromTime(float t)
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
