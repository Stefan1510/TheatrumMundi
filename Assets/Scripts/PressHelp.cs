using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressHelp : MonoBehaviour
{
    [SerializeField] GameObject helpButtonPressed, helpOverlayMenue2, helpOverlayMenue1, helpOverlayMenue3, helpOverlayMenue4, menueConfigMain, menuDirMain;

    private bool pressed = false;

    private void Awake()
    {
        helpOverlayMenue1.SetActive(false);
        helpButtonPressed.SetActive(false);
        helpOverlayMenue2.SetActive(false);
        helpOverlayMenue3.SetActive(false);
        
        Debug.Log("Button: "+helpButtonPressed+" hidden.");
        
        try
        {
            helpOverlayMenue4.SetActive(false);
        }
        catch (Exception ex)
        {
            if (ex is NullReferenceException || ex is UnassignedReferenceException)
            {
                return;
            }
            throw;
		}
    }
    public void OnClick()
    {
        if (pressed)
        {
            helpOverlayMenue1.SetActive(false);
            helpOverlayMenue2.SetActive(false);
            helpOverlayMenue3.SetActive(false);
            helpButtonPressed.SetActive(false);
            pressed = false;
                       
            try
            {
                helpOverlayMenue4.SetActive(false);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is UnassignedReferenceException)
                {
                    return;
                }
                throw;
            }
        }

        else
        {
                if (SceneManaging.menueActive == 1)
                {
                    helpButtonPressed.SetActive(true);
                    helpOverlayMenue1.SetActive(true);
                }
                else if (SceneManaging.menueActive == 2)
                {
                    helpButtonPressed.SetActive(true);
                    helpOverlayMenue2.SetActive(true);
                }
                else if (SceneManaging.menueActive == 3)
                {
                    helpButtonPressed.SetActive(true);
                    helpOverlayMenue3.SetActive(true);
                }
                else if (SceneManaging.menueActive == 4)
                {
                    helpButtonPressed.SetActive(true);
                    try
                    {
                        helpOverlayMenue4.SetActive(true);
                    }
                    catch (Exception ex)
                    {
                        if (ex is NullReferenceException || ex is UnassignedReferenceException)
                        {
                            return;
                        }
                        throw;
                    }

            }
            
            pressed = true;
        }
    }
}
