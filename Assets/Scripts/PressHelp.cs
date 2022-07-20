using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressHelp : MonoBehaviour
{
    [SerializeField] GameObject helpButtonPressed, helpOverlayMenue2, helpOverlayMenue1, helpOverlayMenue3, helpOverlayMenue4, menueConfigMain, menuDirMain;

    private bool pressed = false;
    private float _idleTimer;
    private float _helpAnimDuration;
    private float _helpAnimWaitTime;

    private void Awake()
    {
        _helpAnimDuration = 1;
        _helpAnimWaitTime = 10;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _idleTimer = 0;
        }
        _idleTimer += Time.deltaTime;
        if (_idleTimer > _helpAnimWaitTime)
        {
            float x = _idleTimer - _helpAnimWaitTime;
            float y = -4 * Mathf.Pow(x - 0.5f, 2) + 1;
            Color32 buttonColor = Color32.Lerp(Color.white, new Color32(255, 191, 63, 255), y);
            GetComponent<Button>().image.color = buttonColor;
            transform.localScale = new Vector3(1 + y / 4, 1 + y / 4, 1 + y / 4);
            //Debug.LogWarning(x + " : " + y);
        }
        if (_idleTimer >= _helpAnimWaitTime + _helpAnimDuration)
        {
            //Debug.LogWarning(_idleTimer);
            GetComponent<Button>().image.color = Color.white;
            transform.localScale = Vector3.one;
            _idleTimer = 0;
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
