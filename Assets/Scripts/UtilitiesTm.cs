using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilitiesTm
{
    public static float FloatRemap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }
}
