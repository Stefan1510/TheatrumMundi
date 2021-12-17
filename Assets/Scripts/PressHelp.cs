using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressHelp : MonoBehaviour
{
    [SerializeField] GameObject helpButtonPressed;
    [SerializeField] GameObject helpOverlay;
    private bool pressed = false;

    private void Start()
    {
        helpOverlay.SetActive(false);
        helpButtonPressed.SetActive(false);
    }
    public void OnClick()
    {
        if (pressed)
        {
            helpButtonPressed.SetActive(false);
            helpOverlay.SetActive(false);
            pressed = false;
        }
        else
        {
            helpButtonPressed.SetActive(true);
            helpOverlay.SetActive(true);
            pressed = true;
        }
    }
}
