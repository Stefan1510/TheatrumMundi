using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyLibrary
{
    public void Test ()
    {
        Debug.Log("testitest");
    }
    
    public float ChangeSelectedValue(string str, Slider slider, InputField input)
    {
        float val;
        if (float.TryParse(str, out val))
        {
            if (val < 0f)
                val = 0f;
            if (val > 1f)
                val = 1f;
            slider.value = val;
            //ChangeHeightSchiene1Slider(val);

        }
        else
        {
            input.text = slider.value.ToString("0.00");
        }
        return val;
    }
}
