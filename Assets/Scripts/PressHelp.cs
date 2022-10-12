using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressHelp : MonoBehaviour
{
    [SerializeField] GameObject helpButtonPressed, helpOverlayMenue2, helpOverlayMenue1, helpOverlayMenue3, helpOverlayMenue4, menueConfigMain, menuDirMain, aboutScreen;

    private bool pressed = false, secondHighlight = false;
    private float _idleTimer;
    private float _helpAnimDuration;
    private float _helpAnimWaitTime;
    private Color32 _buttonColorStart;
    private Color32 _buttonColorAttention;

    private void Awake()
    {
        _helpAnimDuration = 1;
        _helpAnimWaitTime = 10;
        helpOverlayMenue1.SetActive(false);
        helpButtonPressed.SetActive(false);
        helpOverlayMenue2.SetActive(false);
        helpOverlayMenue3.SetActive(false);
        aboutScreen.SetActive(false);
        _buttonColorStart = new Color(255, 255, 255, 0);
        //_buttonColorAttention = new Color32(221, 159, 63, 255);
        _buttonColorAttention = new Color32(255, 255, 255, 70);

        Debug.Log("Button: " + helpButtonPressed + " hidden.");

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

    private void HelpAnimation()
    {
        float x = _idleTimer - _helpAnimWaitTime;
        float y = -4 * Mathf.Pow(x - 0.5f, 2) + 1;
        //Color32 buttonColor = Color32.Lerp(_buttonColorStart, _buttonColorAttention, y);
        //transform.GetChild(0).GetComponent<Image>().color = buttonColor;
        Color32 buttonColor = Color32.Lerp(_buttonColorStart, _buttonColorAttention, y);
        transform.GetChild(1).GetComponent<Image>().color = buttonColor;
        transform.GetChild(0).localScale = new Vector3(1 + y / 4, 1 + y / 4, 1 + y / 4);
        transform.GetChild(1).localScale = new Vector3(1 + y / 4, 1 + y / 4, 1 + y / 4);
        if (x > 1 && secondHighlight == false)
        {
            _idleTimer = 10.01f;
            secondHighlight = true;
        }
    }
    private void StopHelpAnimation()
    {
        transform.GetChild(1).GetComponent<Image>().color = _buttonColorStart;
        transform.localScale = Vector3.one;
        _idleTimer = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopHelpAnimation();
            if (pressed)
            {
                helpOverlayMenue1.SetActive(false);
                helpOverlayMenue2.SetActive(false);
                helpOverlayMenue3.SetActive(false);
                helpButtonPressed.SetActive(false);
                pressed = false;
            }
        }
        if (Input.anyKeyDown)
        {
            StopHelpAnimation();
        }
        if (Input.GetMouseButton(0))
        {
            StopHelpAnimation();
        }
        _idleTimer += Time.deltaTime;
        if (_idleTimer > _helpAnimWaitTime)
        {
            HelpAnimation();
        }
        if (_idleTimer >= _helpAnimWaitTime + _helpAnimDuration)
        {
            StopHelpAnimation();
            secondHighlight = false;
        }
    }
    public void OnClick(int i)
    {
        if (i == 0) // help
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
                if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 1 || SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 1)
                {
                    helpButtonPressed.SetActive(true);
                    helpOverlayMenue1.SetActive(true);
                    Debug.Log("overlay 1");
                }
                else if (SceneManaging.configMenueActive == 2 || SceneManaging.directorMenueActive == 2)
                {
                    helpButtonPressed.SetActive(true);
                    helpOverlayMenue2.SetActive(true);
                    Debug.Log("overlay 2");
                }
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 3 || SceneManaging.directorMenueActive == 3 && SceneManaging.mainMenuActive == 2)
                {
                    helpButtonPressed.SetActive(true);
                    helpOverlayMenue3.SetActive(true);
                    Debug.Log("overlay 3");
                }
                else if (SceneManaging.configMenueActive == 4)
                {
                    helpButtonPressed.SetActive(true);
                    try
                    {
                        helpOverlayMenue4.SetActive(true);
                        Debug.Log("overlay 4");
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
        else if (i == 1) // open about
        {
            aboutScreen.SetActive(true);
        }
        else // close about
        {
            aboutScreen.SetActive(false);
        }
    }
}
