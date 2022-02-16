using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Highlighting : MonoBehaviour
{
    [SerializeField] private GameObject figure;
    Outline outline;

    public void Start()
    {
        figure = GameObject.Find("Elefant_LowpolyAnimation_Copy");
        outline = figure.GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnClick()
    {
        if(outline.enabled == true)
        outline.enabled = false;
        else 
        outline.enabled = true;
    }
}
