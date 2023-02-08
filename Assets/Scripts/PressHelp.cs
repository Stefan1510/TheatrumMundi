using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressHelp : MonoBehaviour
{
    [SerializeField] GameObject helpButtonPressed, helpOverlayMenue2, helpOverlayMenue1, helpOverlayMenue3, helpOverlayMenue4, helpOverlayMenue5, helpOverlayMenue6, menueConfigMain, menuDirMain, aboutScreen;

    public bool pressed = false;
    private bool pressedLiveView = false, secondHighlight = false;
    private float _idleTimer;
    private float _timerOverlay;
    private float _helpAnimDuration;
    private float _helpAnimWaitTime;
    private Color32 _buttonColorStart;
    private Color32 _buttonColorAttention;
    public GameObject helpTextLiveView;

    private void Start()
    {
        _helpAnimDuration = 1;
        _helpAnimWaitTime = 10;
        helpOverlayMenue1.SetActive(false);
        helpButtonPressed.SetActive(false);
        helpOverlayMenue2.SetActive(false);
        helpOverlayMenue3.SetActive(false);
        aboutScreen.SetActive(false);
        _buttonColorStart = new Color(255, 255, 255, 0);
        _buttonColorAttention = new Color32(255, 255, 255, 70);

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
    private void StopOverlay()
    {
        _timerOverlay = 0;
        //pressed = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //StopHelpAnimation();
            helpTextLiveView.SetActive(false);
            pressedLiveView = false;

            if (pressed)
            {
                helpOverlayMenue1.SetActive(false);
                helpOverlayMenue2.SetActive(false);
                helpOverlayMenue3.SetActive(false);
                helpOverlayMenue4.SetActive(false);
                helpOverlayMenue5.SetActive(false);
                helpOverlayMenue6.SetActive(false);
                helpButtonPressed.SetActive(false);
                pressed = false;
            }
            //StopOverlay();
        }
        if (Input.anyKeyDown)
        {
            StopHelpAnimation();
            StopOverlay();
        }
        _idleTimer += Time.deltaTime;
        _timerOverlay += Time.deltaTime;
        if (_idleTimer > _helpAnimWaitTime && !SceneManaging.playing)
        {
            HelpAnimation();
        }
        if (_idleTimer >= _helpAnimWaitTime + _helpAnimDuration)
        {
            StopHelpAnimation();
            secondHighlight = false;
        }
        if (_timerOverlay > 200 && _timerOverlay < 200.5f && !SceneManaging.playing && !SceneManaging.dragging)
        {
            OnClick(0);
            pressed = false;
        }
        else if (_timerOverlay > 200.5f)
            pressed = true;
    }
    public void ClickOnLiveView()
    {
        if (pressedLiveView)
        {
            helpTextLiveView.SetActive(false);
            pressedLiveView = false;
            //Debug.Log("pressed: " + pressedLiveView);
        }
        else
        {
            helpTextLiveView.SetActive(true);
            if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 1)  // Buehne
            {
                helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links eine Schiene aus und bearbeite ihre Höhe und Position.";
            }
            else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 1)   // Figuren
            {
                helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links eine Figur aus und ziehe sie auf die Schiene unten.";
            }
            else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 2)    // kulissen
            {
                helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links eine Kulisse aus und ziehe sie auf die Schiene unten rechts.";
            }
            /*else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 2)    // (light director) verworfen
            {
                helpOverlayMenue2.SetActive(true);
                Debug.Log("overlay 6");
            }*/
            else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 3)  // licht
            {
                helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Im Shelf links kannst du Lichter erstellen bearbeiten. ";
            }
            else if (SceneManaging.directorMenueActive == 3 && SceneManaging.mainMenuActive == 2)  // musik
            {
                helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Bitte wähle links ein Musikstück aus und ziehe es auf die Schiene unten.";
            }
            else if (SceneManaging.configMenueActive == 4)  // speichern
            {
                helpTextLiveView.transform.GetChild(0).GetComponent<Text>().text = "Das ist der LiveView. Links im Shelf hast du die Möglichkeit, Szenen zu laden oder zu speichern.";
            }
            //pressed = true;
            pressedLiveView = true;
        }
    }
    public void OnClick(int i)
    {
        if (i == 0) // help
        {
            Debug.Log("pressed: "+pressed);
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
                helpButtonPressed.SetActive(true);
                if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 1)  // Buehne
                {
                    
                    helpOverlayMenue1.SetActive(true);
                }
                else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 1)   // Figuren
                {
                    Debug.Log("hier");
                    helpOverlayMenue5.SetActive(true);
                }
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 2)    // kulissen
                {
                    helpOverlayMenue2.SetActive(true);
                }
                /*else if (SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive == 2)    // (light director) verworfen
                {
                    helpOverlayMenue2.SetActive(true);
                    Debug.Log("overlay 6");
                }*/
                else if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 3)  // licht
                {
                    helpOverlayMenue3.SetActive(true);
                }
                else if (SceneManaging.directorMenueActive == 3 && SceneManaging.mainMenuActive == 2)  // musik
                {
                    helpOverlayMenue6.SetActive(true);
                }
                else if (SceneManaging.configMenueActive == 4)  // speichern
                {
                    helpOverlayMenue4.SetActive(true);
                }
                pressed = true;
                //Debug.Log("true");
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
