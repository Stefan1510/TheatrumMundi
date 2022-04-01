using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageTimelineSelection
{
    private static int _tlRailType = 0;     //0 - railspeed; 1 - light; 2 - music; 3 - background
    private static int _tlRailNumber = 0;   //der index der Rails in der Rail-Liste
    private static bool _updateNecessary = false;

    public static int GetRailType()
    {
        return _tlRailType;
    }

    public static void SetRailType(int railType)
    {
        _tlRailType = railType;
        _updateNecessary = true;
    }
    public static int GetRailNumber()
    {
        return _tlRailNumber;
    }
    public static void SetRailNumber(int railNumber)
    {
        _tlRailNumber = railNumber;
        _updateNecessary = true;
    }
    public static bool UpdateNecessary()
    {
        return _updateNecessary;
    }
}
