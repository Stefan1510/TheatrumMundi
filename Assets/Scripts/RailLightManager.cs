using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailLightManager : MonoBehaviour
{
    #region variables
    [SerializeField] Image timelineImage;
    [SerializeField] private GameObject gameController;
    [SerializeField] private RailManager contentRailsMenue;
    [HideInInspector] public bool isTimelineOpen;
    private float heightOpened, heightClosed;
    private float maxX, minX;
    private float railWidth;
    private float currentLossyScale = 0.0f;
    #endregion
    void Start()
    {
        ResetScreenSize();
        //timelineImage.rectTransform.sizeDelta = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
    }
    public void openTimelineByClick(bool thisTimelineOpen)
    {
        if (!thisTimelineOpen)
            // a different rail is open - close it
            for (int i = 0; i < contentRailsMenue.rails.Length; i++)
            {
                contentRailsMenue.rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                contentRailsMenue.rails[i].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                contentRailsMenue.railList[i].isTimelineOpen = false;
                contentRailsMenue.openCloseObjectInTimeline(false, contentRailsMenue.railList[i].myObjects, i);
                for (int j = 0; j < contentRailsMenue.railList[i].myObjects.Count; j++)
                {
                    SceneManaging.highlight(contentRailsMenue.railList[i].myObjects[j].figure3D, contentRailsMenue.railList[i].myObjects[j].figure, false, true);
                }
            }

        for (int j = 0; j < 2; j++)
        {
            gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
            gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
            gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
        }

        gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
        gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
        gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
        gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects);

        for (int j = 0; j < gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects.Count; j++)
        {
            SceneManaging.highlight(null, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects[j], false, false);
        }

        // open clicked rail
        timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
        //scale up the collider
        timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
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
    public void ResetScreenSize()
    {
        minX = 0.146f * Screen.width;// / gameObject.transform.lossyScale.x; //301.0f;  //timeline-minX
                                     //Debug.Log("minX: " + minX);
        railWidth = 0.87f * Screen.width;           //railwidth=1670.4px / gameObject.transform.lossyScale.x;
        heightClosed = 0.018f * Screen.height;// / gameObject.transform.lossyScale.x;
        heightOpened = 0.074f * Screen.height;// / gameObject.transform.lossyScale.x;
        maxX = minX + railWidth;  //timeline-maxX

        if (isTimelineOpen)
        {
            timelineImage.rectTransform.sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightOpened / gameObject.transform.lossyScale.x);
        }
        else
        {
            timelineImage.rectTransform.sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
        }
    }

    /*public void openCloseTimeSettings(bool tlOpen, GameObject ts)
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
    }*/

    void Update()
    {
        if (currentLossyScale != transform.lossyScale.x)
        {
            currentLossyScale = transform.lossyScale.x;
            ResetScreenSize();
        }
        Vector2 getMousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                openTimelineByClick(isTimelineOpen);
            }
        }
    }
}