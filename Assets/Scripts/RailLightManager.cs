using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailLightManager : MonoBehaviour
{
    public Image timelineImage;
    public GameObject gameController;
    public Text timelineText;
    public Image timeSliderImage;
    private BoxCollider2D timeSlider;
    [HideInInspector] public bool isTimelineOpen;
    Vector2 textSize;

    GameObject timeSettings;
    private float heightOpened, heightClosed;
    private float maxX;
    private float railWidth;
    private float minX;
    private float currentLossyScale = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        timelineImage = this.GetComponent<Image>();
        timeSettings = timeSliderImage.transform.GetChild(0).gameObject; // GameObject.Find("ImageTimeSettingsArea");
    }

    void Start()
    {
        timeSettings.SetActive(false);
        isTimelineOpen = false;

        ResetScreenSize();
        timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
    }
    public void openTimelineByClick(bool thisTimelineOpen, Image tl)
    {
        if (isAnyTimelineOpen() == false)
        {
            //set global flag
            SceneManaging.anyTimelineOpen = true;
            isTimelineOpen = true;
            //scale up timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
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
                /*//close timeline
                isTimelineOpen = false;
                //scale down timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed/ gameObject.transform.lossyScale.x);
                //scale down the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed/ gameObject.transform.lossyScale.x);
                //scale up the text*/
            }
            else
            {
                // a different rail is open - close it
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().Rails[i].isTimelineOpen = false;
                    //Debug.Log("++++ Scaling down Rail: " + gameController.GetComponent<UIController>().Rails[i]);
                }

                for (int j = 0; j < 2; j++)
                {
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[j].isTimelineOpen = false;
                }

                gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                gameController.GetComponent<UIController>().RailMusic.isTimelineOpen = false;
                // open clicked rail
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
                //scale up the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
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

    public void ResetScreenSize()
    {
        Debug.Log("lossyScale: " + gameObject.transform.lossyScale);
        //Debug.Log("Screen changed! ScreenX: " + Screen.width);

        minX = 0.146f * Screen.width;// / gameObject.transform.lossyScale.x; //301.0f;  //timeline-minX
        Debug.Log("minX: " + minX);
        railWidth = 0.69f * Screen.width;// / gameObject.transform.lossyScale.x;
        heightClosed = 0.018f * Screen.height;// / gameObject.transform.lossyScale.x;
        heightOpened = 0.074f * Screen.height;// / gameObject.transform.lossyScale.x;
        maxX = minX + railWidth;  //timeline-maxX
        //Debug.Log("rail start: " + minX);
        Debug.Log("isTimelineopen: " + isTimelineOpen + "heightclosed: " + heightClosed);
        if (isTimelineOpen)
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightOpened / gameObject.transform.lossyScale.x);
            Debug.Log("size box collider: " + gameObject.GetComponent<BoxCollider2D>().size.y);
        }
        else
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
            Debug.Log("size: " + timelineImage.GetComponent<RectTransform>().sizeDelta.y);
        }
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
        if (currentLossyScale != transform.lossyScale.x)
        {
            currentLossyScale = transform.lossyScale.x;
            Debug.Log("scale after: " + transform.lossyScale.x);
            ResetScreenSize();
        }
        //Debug.Log("draggingOnTimeline: " + draggingOnTimeline + ", editTimelineObj: " + editTimelineObject + ", draggingObj: " + draggingObject + ", isInstance: " + isInstance);
        Vector2 getMousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                openTimelineByClick(isTimelineOpen, timelineImage);
                Debug.Log("railtype: " + ImageTimelineSelection.GetRailType());
            }
        }
    }
}