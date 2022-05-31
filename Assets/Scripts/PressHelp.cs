using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressHelp : MonoBehaviour
{
    [SerializeField] GameObject helpButtonPressed, helpOverlay, menueConfigMain, menuDirMain;

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
            if (SceneManaging.menueActive == 1)
            {

            }
            else if (SceneManaging.menueActive == 2)
            {
                helpButtonPressed.SetActive(true);
                helpOverlay.SetActive(true);
            }
            else if (SceneManaging.menueActive == 3)
            {

            }
            else if (SceneManaging.menueActive == 4)
            {

            }

            pressed = true;
        }
    }
}
