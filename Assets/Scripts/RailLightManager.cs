using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailLightManager : MonoBehaviour
{
    #region variables
    [SerializeField] Image timelineImage;
    [SerializeField] private UIController gameController;
    [SerializeField] private RailManager contentRailsMenue;
    [SerializeField] private LightAnimationRepresentation tmpLightAnim;
    [HideInInspector] public bool isTimelineOpen;
    private float heightOpened, heightClosed;
    private float maxX, minX;
    private float railWidthAbsolute = 1670.4f, railWidth;
    private float currentLossyScale = 0.0f;
    #endregion
    void Start()
    {
        ResetScreenSize();
        
        if (!SceneManaging.isExpert)
            GetComponent<BoxCollider2D>().enabled = false;
    }
    public void openTimelineByClick(bool thisTimelineOpen)
    {
        if (!thisTimelineOpen)
        {
            // a different rail is open - close it
            for (int i = 0; i < contentRailsMenue.railList.Length; i++)
            {
                if (contentRailsMenue.railList[i].isTimelineOpen)
                {
                    contentRailsMenue.rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 20);
                    contentRailsMenue.rails[i].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 20);
                    contentRailsMenue.railList[i].isTimelineOpen = false;
                    contentRailsMenue.OpenCloseObjectInTimeline(contentRailsMenue.railList[i].myObjects, i, true);
                }
            }

            for (int j = 0; j < 2; j++)
            {
                if (gameController.RailLightBG[j].isTimelineOpen)
                {
                    gameController.RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 20);
                    gameController.RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 20);
                    gameController.RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
            }

            // wenn music-rail offen ist
            if (gameController.RailMusic.isTimelineOpen)
            {
                SceneManaging.closeRail(gameController.RailMusic.gameObject, railWidthAbsolute);
                gameController.RailMusic.OpenCloseObjectInTimeline(false);
                gameController.RailMusic.isTimelineOpen = false;
                gameController.RailMusic.musicPieceLengthDialog.SetActive(false);

                for (int k = 0; k < gameController.RailMusic.myObjects.Count; k++)
                {
                    SceneManaging.highlight(gameController.RailMusic.myObjects[k].musicPiece, false);
                }
            }

            // open clicked rail
            timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 80);
            //scale up the collider
            timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 80);
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
            tmpLightAnim.ChangeImage();
        }

    }
    public void ResetScreenSize()
    {
        minX = 0.087f * Screen.width;               //timeline-rail-minX
        railWidth = 0.87f * Screen.width;           //railwidth=1670.4px
        maxX = minX + railWidth;

        if (isTimelineOpen)
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidthAbsolute, 80);
        }
        else
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidthAbsolute, 20);
        }
    }
    void FixedUpdate()
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
            if (GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                openTimelineByClick(isTimelineOpen);
            }
        }
    }
}