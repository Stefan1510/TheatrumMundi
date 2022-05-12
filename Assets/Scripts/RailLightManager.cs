using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailLightManager : MonoBehaviour
{
    Image timelineImage;
    public GameObject gameController;
    public Text timelineText;
    public Image timeSliderImage;
    private BoxCollider2D timeSlider;
    [HideInInspector] public bool isTimelineOpen;
    Vector2 textSize;

    GameObject timeSettings;

    // Start is called before the first frame update
    void Awake()
    {
        timelineImage = this.GetComponent<Image>();
        //SceneManaging.anyTimelineOpen = false;
        timeSettings = timeSliderImage.transform.GetChild(0).gameObject; // GameObject.Find("ImageTimeSettingsArea");
    }

    void Start()
    {
        timeSettings.SetActive(false);
        isTimelineOpen = false;
        scaleObject(gameObject, 1335, 20);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1335,35);
    }

    public void scaleObject(GameObject fig, float x, float y)
    {
        //scale the object
        fig.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        //scale the collider, if object has one
        if (fig.GetComponent<BoxCollider2D>() == true)
        {
            fig.GetComponent<BoxCollider2D>().size = new Vector2(x, y);
        }
        else
        {
            //do nothing
        }
    }
    public void openCloseTimelineByClick(bool thisTimelineOpen, Image tl)
    {
        float heightOpened = 80.0f;
        float heightClosed = 20.0f;    //this size is set in editor

        if (isAnyTimelineOpen() == false)
        {
            //set global flag
            SceneManaging.anyTimelineOpen = true;
            isTimelineOpen = true;
            //scale up timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
            if (gameObject.name == "ImageTimelineRailLight")
            {
                ImageTimelineSelection.SetRailNumber(7);
                ImageTimelineSelection.SetRailType(1);  // for light-rail
            }
            else
            {
                ImageTimelineSelection.SetRailNumber(6);
                ImageTimelineSelection.SetRailType(3);  // for background
            }
        }
        else
        {
            if (thisTimelineOpen)
            {
                //close timeline
                isTimelineOpen = false;
                //scale down timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                //scale down the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed+15);
                //scale up the text
            }
            else
            {
                // a different rail is open - close it
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed+15);
                    gameController.GetComponent<UIController>().Rails[i].isTimelineOpen = false;
                    //Debug.Log("++++ Scaling down Rail: " + gameController.GetComponent<UIController>().Rails[i]);
                }

                for (int j = 0; j < 2; j++)
                {
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed+15);
                    gameController.GetComponent<UIController>().RailLightBG[j].isTimelineOpen = false;
                }

                gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed+15);
                gameController.GetComponent<UIController>().RailMusic.isTimelineOpen = false;
                // open clicked rail
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
                //scale up the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
                isTimelineOpen = true;
                if (gameObject.name == "ImageTimelineRailLight")
                {
                    ImageTimelineSelection.SetRailNumber(7);
                    ImageTimelineSelection.SetRailType(1);  // for light-rail

                }
                else
                {
                    ImageTimelineSelection.SetRailNumber(6);
                    ImageTimelineSelection.SetRailType(3);  // for background-rail 
                }
            }

        }
        openCloseTimeSettings(isAnyTimelineOpen(), timeSettings);
    }


    public bool isAnyTimelineOpen()
    {
        bool val = false;
        for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
        {
            if (gameController.GetComponent<UIController>().Rails[i].isTimelineOpen == true)
            {
                val = true;
            }
        }
        if (gameController.GetComponent<UIController>().RailMusic.isTimelineOpen == true) val = true;
        for (int j = 0; j < 2; j++) // 2, weil es nur zwei Schienen gibt 
        {
            if (gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen == true) val = true;
        }
    
        return val;
    }

    public void openCloseTimeSettings(bool tlOpen, GameObject ts)
    {
        //activate the timeSettings at the bottom of the timeslider
        //this method is called in the timelineOpen-methods
        if ((tlOpen)) // && (ts.activeSelf==false)
        {
            ts.SetActive(true);
        }
        else
        {
            ts.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("draggingOnTimeline: " + draggingOnTimeline + ", editTimelineObj: " + editTimelineObject + ", draggingObj: " + draggingObject + ", isInstance: " + isInstance);
        Vector2 getMousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                openCloseTimelineByClick(isTimelineOpen, timelineImage);
                Debug.Log("railtype: " + ImageTimelineSelection.GetRailType());
            }
        }
    }
}