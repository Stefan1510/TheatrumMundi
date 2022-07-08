using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailMusicManager : MonoBehaviour
{
    public Image timelineImage, timeSliderImage;
    AudioSource audioSource;
    public GameObject gameController, menue3;
    //public GameObject[] musicSamples;
    private BoxCollider2D timeSlider;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, playingMusic, isInstance, playingSample;
    public bool isTimelineOpen;
    Vector2 releaseObjMousePos, diff;
    double minX, maxX;
    private float railWidth;
    int maxTimeInSec, currentClip;
    Vector2 sizeDeltaAsFactor;
    Vector2[] objectShelfPosition, objectShelfSize;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    Vector2 objectSceneSize;

    public GameObject objectLibrary, parentMenue; // mainMenue
    GameObject timeSettings;
    GameObject[] figCounterCircle, figureObjects;
    int currentClickedObjectIndex, sampleButtonPressed;
    int currentClickedInstanceObjectIndex;
    private float currentLossyScale;
    public List<GameObject> timelineObjects, timelineInstanceObjects;
    Color colMusic;//, colMusicHighlighted;
    float heightClosed, heightOpened;

    public AudioClip[] clip;         // ought to be 6 audioclips


    void Awake()
    {
        clip = gameController.GetComponent<SceneDataController>().objectsMusicClips;
        timelineImage = this.GetComponent<Image>();
        SceneManaging.anyTimelineOpen = false;
        draggingOnTimeline = false;
        draggingObject = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        playingMusic = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        currentClip = 0;
        isTimelineOpen = false;
        isInstance = false;
        sampleButtonPressed = -1;

        //load all objects given in the figuresShelf
        figureObjects = new GameObject[objectLibrary.transform.childCount];         //instead of a count like "3"
        objectShelfPosition = new Vector2[figureObjects.Length];
        objectShelfSize = new Vector2[figureObjects.Length];
        objectShelfParent = new GameObject[figureObjects.Length];
        figCounterCircle = new GameObject[figureObjects.Length];

        colMusic = new Color(0.21f, 0.51f, 0.267f, 0.5f);
        //colMusicHighlighted = new Color(0.21f,0.81f, 0.267f, 0.5f);

        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = (objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject);
            objectShelfSize[i] = new Vector2(figureObjects[i].GetComponent<RectTransform>().rect.width, figureObjects[i].GetComponent<RectTransform>().rect.height);
            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
        }

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;
        currentLossyScale = 1.0f;

        timeSettings = timeSliderImage.transform.GetChild(0).gameObject; // GameObject.Find("ImageTimeSettingsArea");
    }
    void Start()
    {
        ResetScreenSize();
        timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);

        audioSource = GetComponent<AudioSource>();
        timeSettings.SetActive(false);
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
            figCounterCircle[i].transform.GetChild(0).GetComponent<Text>().text = "0";
        }

        List<GameObject> timelineObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects = new List<GameObject>();

        maxTimeInSec = (int)AnimationTimer.GetMaxTime();
        sizeDeltaAsFactor = new Vector2(1.0f, 1.0f);
    }
    public string identifyClickedObject()   //old
    {
        string objName = "";
        if (figureObjects.Length == 0)
            return "no object found! [-1]";             //error-handling if no clickable object found

        for (int i = 0; i < figureObjects.Length; i++)
        {
            //save last correct index
            int oldClickedObject = currentClickedObjectIndex;
            //which object in grid layout is clicked
            if (figureObjects[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
            {
                //save the index of clicked object
                currentClickedObjectIndex = i;
                //if you click an new object
                if (oldClickedObject != currentClickedObjectIndex)
                {
                    //set objectSceneSize-Variables back to default
                    //objectDefaultPosition=figureObjects[currentClickedObjectIndex].transform.position;
                    //objectDefaultSize.x=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.width;
                    //objectDefaultSize.y=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.height;
                    //objectSceneSize.x=objectDefaultSize.x;
                    //objectSceneSize.y=objectDefaultSize.y;
                    //objectOnTimeline=false;
                }
                return figureObjects[i].GetComponent<BoxCollider2D>().name;
            }
            else
            {
                objName = "no object clicked!";
            }
        }

        return objName;
    }
    public string identifyClickedObjectByList(List<GameObject> objectsOnTimeline)
    {
        string objName = "";
        if (objectsOnTimeline.Count == 0)
            return "no object found! [-1]";             //error-handling if no clickable object found

        for (int i = 0; i < objectsOnTimeline.Count; i++)
        {
            //save last correct index
            int oldClickedObject = currentClickedInstanceObjectIndex;
            //Debug.Log("+++++++++++++++++++++Object: "+objectsOnTimeline[i].GetComponent<BoxCollider2D>()+", Mouse position: "+Physics2D.OverlapPoint(Input.mousePosition));
            //which object in grid layout is clicked
            if (objectsOnTimeline[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
            {
                //save the index of clicked object
                currentClickedInstanceObjectIndex = i;
                //if you click an new object
                if (oldClickedObject != currentClickedInstanceObjectIndex)
                {
                    //return objectsOnTimeline[i].name;
                }
                return objectsOnTimeline[i].GetComponent<BoxCollider2D>().name;
            }
            else
            {
                objName = "no object clicked!";
            }
        }

        return objName;
    }
    public void openTimelineByClick(bool thisTimelineOpen)
    {
        if (isAnyTimelineOpen() == false)
        {
            // Debug.Log("++++ es ist keine schiene geöffnet, deswegen wird geklickte Schiene geöffnet: " + tl);
            //set global flag
            SceneManaging.anyTimelineOpen = true;
            isTimelineOpen = true;
            //scale up timeline
            timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
            //scale up the collider
            timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
            //minimize or maximize objects on timeline
            openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
            ImageTimelineSelection.SetRailNumber(6);
            ImageTimelineSelection.SetRailType(2);  // for rail-rails
            // if (!menue3.activeSelf)
            // {
            //     parentMenue.GetComponent<ObjectShelf>().ButtonShelf03();
            // }
        }
        else if (isAnyTimelineOpen())
        {
            if (thisTimelineOpen)
            {
                /*// Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
                //close timeline
                isTimelineOpen = false;
                //scale down timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed/ gameObject.transform.lossyScale.x);
                //scale down the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed/ gameObject.transform.lossyScale.x);
                openCloseObjectInTimeline(false, timelineInstanceObjects, editTimelineObject);*/
            }
            else
            {
                // if (!menue3.activeSelf)
                // {
                //     parentMenue.GetComponent<ObjectShelf>().ButtonShelf03();
                // }
                // Debug.Log("++++ geklickte Schiene ist zu, aber eine andere ist offen und wird geschlossen: " + tl);
                // a different rail is open - close it
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    if (gameController.GetComponent<UIController>().Rails[i].isTimelineOpen)
                    {
                        Debug.LogWarning("Rail: " + (i + 1) + " ist offen. ");
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().isTimelineOpen = false;
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects);

                        for (int j = 0; j < gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().timelineInstanceObjects3D.Count; j++)
                        {
                            gameController.GetComponent<UIController>().Rails[i].highlight(gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects3D[j], gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects[j], false);
                            Debug.LogWarning("TimelineinstanceObjects: " + gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects[j]);
                        }
                    }
                }
                for (int k = 0; k < 2; k++)
                {
                    if (gameController.GetComponent<UIController>().RailLightBG[k].isTimelineOpen)
                    {
                        gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<RailLightManager>().isTimelineOpen = false;
                    }

                }


                // gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                // gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                // gameController.GetComponent<UIController>().RailMusic.isTimelineOpen = false;
                // open clicked rail
                //Debug.Log("++++ geklickte Schiene wird geöffnet: " + tl);
                //scale up timeline
                timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
                //scale up the collider
                timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
                openCloseObjectInTimeline(true, timelineInstanceObjects, false);
                isTimelineOpen = true;
                ImageTimelineSelection.SetRailNumber(6);
                ImageTimelineSelection.SetRailType(2);  // for rail-rails
            }

        }
        openCloseTimeSettings(isAnyTimelineOpen(), timeSettings);

    }
    public void openCloseTimelineByDrag()
    {
        if (isAnyTimelineOpen() == false)
        {
            //Debug.Log("++++ geklickte Schiene ist zu und wird geöffnet: " + tl);
            isTimelineOpen = true;
            //scale up timeline
            timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
            //scale up the collider
            timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
            //scale down the text
            //tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 0.2f, 1);
            //timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
            //scale up all objects on timeline
            openCloseObjectInTimeline(isTimelineOpen, timelineInstanceObjects, editTimelineObject);
            ImageTimelineSelection.SetRailNumber(6);
            ImageTimelineSelection.SetRailType(2);  // for rail-rails
        }
        else
        {
            if (isTimelineOpen)
            {
                //Debug.Log("Schiene ist schon offen, es passiert nix.");
            }
            else
            {
                // Debug.Log("Eine andere Schiene ist offen: ");
                if (gameController.GetComponent<UIController>().RailLightBG[0].isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<RailLightManager>().isTimelineOpen = false;

                }
                else if (gameController.GetComponent<UIController>().RailLightBG[1].isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
                else if (gameController.GetComponent<UIController>().RailMusic.isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects, false);
                    //unhighlight(gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>(). .timelineInstanceObjects3D[i], gameController.GetComponent<UIController>().RailMusic.timelineInstanceObjects[i]);
                    // Debug.Log("++++ Musikschiene ist offen und wird geschlossen: ");
                }
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    if (gameController.GetComponent<UIController>().Rails[i].isTimelineOpen)
                    {
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().isTimelineOpen = false;
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects);
                        for (int j = 0; j < gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().timelineInstanceObjects3D.Count; j++)
                        {
                            gameController.GetComponent<UIController>().Rails[i].highlight(gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects3D[j], gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects[j], false);
                        }
                    }
                }
                // //Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
                // isTimelineOpen = false;
                // //scale down timeline
                // timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                // //scale down the collider
                // timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                // //scale up all objects on timeline
                // openCloseObjectInTimeline(isTimelineOpen, timelineInstanceObjects, editTimelineObject);
            }
            openCloseTimeSettings(isAnyTimelineOpen(), timeSettings);
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
    public void saveParent(GameObject obj)
    {
        GameObject oldParent;
        if (obj.transform.parent != null)
            //save old parent
            oldParent = obj.transform.parent.gameObject;        //parent is a transform, to get the gameobject of this parent, use ".gameObject"
                                                                //Debug.Log("old parent "+oldParent);
    }
    public void setParent(GameObject obj, GameObject parentToSet)
    {
        try
        {
            saveParent(obj);
            //set new parent
            //Debug.Log("new parent "+parentToSet);
            obj.transform.SetParent(parentToSet.transform);
        }
        catch (NullReferenceException ex)
        {

        }

    }
    public void updateObjectPosition(GameObject obj, Vector2 mousePos)
    {
        //set parent
        //obj.transform.SetParent(mainMenue.transform);
        setParent(obj, gameObject);
        //move object
        obj.transform.position = new Vector3(mousePos.x, mousePos.y, -1.0f);
        //set up flags
        //Debug.Log("mouse: " + mousePos);
    }
    public bool checkHittingTimeline(GameObject obj, Image tl, Vector2 mousePos)
    {
        bool hit = false;
        Vector2 colSize = new Vector2(GetComponent<BoxCollider2D>().size.x * gameObject.transform.lossyScale.x, GetComponent<BoxCollider2D>().size.y * gameObject.transform.lossyScale.x);
        //Debug.Log("mouse: " + Input.mousePosition.y + ", posY" + tl.transform.position.y + ", col Height: " + colSize.y);
        //if mouse hits the timeline while dragging an object
        if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= tl.transform.position.y + (colSize.y / 2.0f) && mousePos.y > tl.transform.position.y - (colSize.y / 2.0f))
        {
            //Debug.Log("object hits timeline!");
            hit = true; ;
        }
        //Debug.Log("drag and hit " + hit);
        return hit;
    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects, bool editObjOnTl)
    {
        float length = 100.0f;
        //Debug.Log("method: timelineObjects count: "+objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -gameObject.GetComponent<RectTransform>().rect.height / 2);
            //if timeline open scale ALL objects up
            if (timelineOpen)
            {
                scaleObject(objects[i], length, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);

            }
            else
            {
                //otherwise scale ALL objects down
                //Debug.Log("method: scale object down:");
                scaleObject(objects[i], length, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, true);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, true);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, true);
            }
        }
    }
    public void scaleObject(GameObject fig, float x, float y, bool boxCollZero)
    {
        //scale the object
        fig.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        //scale the collider, if object has one
        if (fig.GetComponent<BoxCollider2D>() == true)
        {
            if (boxCollZero)
            {
                // get rid of Box Collider, if scaling down, so timeline can always be clicked
                fig.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                //scale the collider, if object has one
                fig.GetComponent<BoxCollider2D>().enabled = true;
                fig.GetComponent<BoxCollider2D>().size = new Vector2(fig.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x, y);
            }
        }
        else
        {
            //do nothing
        }
    }
    public void setObjectOnTimeline(GameObject fig, float x, float y)
    {
        fig.transform.position = new Vector3(fig.transform.position.x, y, -1.0f);
    }
    public void updateObjectList(List<GameObject> objects, GameObject obj)
    {
        //check if the object is already in list
        if (objects.Contains(obj))
        {
            //do nothing
        }
        else
        {
            objects.Add(obj);
        }
    }
    public void createRectangle(GameObject obj, Vector2 size, Color col, double railMinX, double animLength)
    {
        double tmpLength = (maxX - minX) / 614.0f * animLength;

        GameObject imgObject = new GameObject("RectBackground");
        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.pivot = new Vector2(0.0f, 0.5f);
        //set pivot point of sprite  to left border, so that rect aligns with sprite
        obj.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 0.5f);
        trans.anchoredPosition = new Vector2((obj.GetComponent<RectTransform>().rect.width / 2) * (-1), 0.0f);
        trans.SetSiblingIndex(0);
        trans.sizeDelta = new Vector2((float)animLength, size.y); // custom size

        Image image = imgObject.AddComponent<Image>();
        image.color = col;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
        //set pivot point of sprite back to midpoint of sprite
        obj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        obj.GetComponent<RectTransform>().position = new Vector3(obj.GetComponent<RectTransform>().position.x, obj.GetComponent<RectTransform>().position.y, -1);
    }
    public int calculateFigureStartTimeInSec(GameObject fig, double animLength, int maxTimeLengthInSec, double railMinX, double railMaxX)
    {
        int sec = 0;
        double tmpX = fig.transform.position.x - railMinX;  //x-pos is screenX from left border, railMinX is the rail-startpoint
        Vector2 tmpSize = fig.GetComponent<BoxCollider2D>().size;
        double tmpMinX = (double)tmpX - (tmpSize.x / 2.0f); //if tmpX is the midpoint of figure
        double tmpMaxX = tmpMinX + animLength;  //length of figure is related to rail speed, here named as animationLength
                                                //get figure minX related to timelineMinX
        double percentageOfRail = tmpMinX / (railMaxX - railMinX);  //max-min=real length, percentage: e.g. 0,3823124 (38%)
        sec = (int)(((double)maxTimeLengthInSec) * percentageOfRail);
        //Debug.Log("method: fig: "+fig+" tmpX: "+tmpX+" tmpSize: "+tmpSize+" tmpMinX: "+tmpMinX+" railMaxX: "+railMaxX+" percentage: "+percentageOfRail+" sec: "+sec);
        return sec;
    }
    public int calculateMusicEndTimeInSec(GameObject fig, double animLength, int maxTimeLengthInSec, double railMinX, double railMaxX)
    {
        int sec = 0;
        double tmpX = fig.transform.position.x - railMinX;  //x-pos is screenX from left border, railMinX is the rail-startpoint
        Vector2 tmpSize = fig.GetComponent<BoxCollider2D>().size;
        double tmpMinX = (double)tmpX - (tmpSize.x / 2.0f); //if tmpX is the midpoint of figure
        double tmpMaxX = tmpMinX + animLength;  //length of figure is related to rail speed, here named as animationLength
                                                //get figure minX related to timelineMinX
        double percentageOfRail = tmpMinX / (railMaxX - railMinX);  //max-min=real length, percentage: e.g. 0,3823124 (38%)
        sec = (int)((((double)maxTimeLengthInSec) * percentageOfRail) + animLength);
        return sec;
    }
    public int countCopiesOfObject(GameObject fig, List<GameObject> tlObjs)
    {
        int c = 0;
        //count object with the same name as fig
        foreach (GameObject gO in tlObjs)
        {
            //if (gO.name==fig.name)
            if (gO.name.Contains(fig.name))
            {
                c++;
            }
        }
        return c;
    }
    public void removeObjectFromTimeline(GameObject obj)
    {
        //Debug.LogError(obj.name);
        int tmpNr = int.Parse(obj.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text);
        Destroy(obj);
        timelineInstanceObjects.Remove(obj);
        //Debug.Log("tmpNr: " + tmpNr + "currentCounterNr: " + currentCounterNr + ", Instances: " + timelineInstanceObjects.Count);
        Debug.Log("+++Removed: " + obj);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
        StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].musicClipElementInstances.Remove(StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].musicClipElementInstances[Int32.Parse(obj.name.Substring(17, 3))]);
    }
    public bool checkHittingTimeline(GameObject obj, Vector2 mousePos)
    {
        bool hit = false;

        Vector2 tlPos = new Vector2(timelineImage.transform.position.x, timelineImage.transform.position.y);
        Vector2 colSize = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, timelineImage.GetComponent<BoxCollider2D>().size.y);
        //if mouse hits the timeline while dragging an object
        //my implementation of object-boundingbox
        if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
        ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
        {
            Debug.Log("object hits timeline!: " + timelineImage);
            hit = true;
        }

        Debug.Log("drag and hit " + hit);
        return hit;
    }
    public void highlight(GameObject obj, bool highlightOn)
    {
        if (highlightOn)
        {
            //obj.transform.GetChild(0).GetComponent<Image>().color = colMusicHighlighted;
            obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);   //show Delete-Button
            SceneManaging.highlighted = true;
        }
        else
        {
            obj.transform.GetChild(0).GetComponent<Image>().color = colMusic;
            obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);  //hide Delete-Button
            SceneManaging.highlighted = false;
        }
    }
    public void CreateNew2DInstance(int musObjNr, float moment)
    {
        //create a copy of this timelineObject and keep the original one
        newCopyOfFigure = Instantiate(figureObjects[musObjNr]);
        //Debug.Log("+++newcopy: " + newCopyOfFigure);
        //count objects from same kind
        int countName = 0;
        countName = countCopiesOfObject(figureObjects[musObjNr], timelineInstanceObjects);
        newCopyOfFigure.name = figureObjects[musObjNr].name + "_instance" + countName.ToString("000");

        float tmpLength = ((float)maxX - (float)minX) * newCopyOfFigure.GetComponent<MusicLength>().musicLength / maxTimeInSec;//UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, 614, (float)minX, (float)maxX);
        createRectangle(newCopyOfFigure, new Vector2(300, gameObject.GetComponent<RectTransform>().rect.height * 0.96f), colMusic, minX, tmpLength);
        scaleObject(newCopyOfFigure, 100, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
        scaleObject(newCopyOfFigure.transform.GetChild(0).gameObject, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector2(newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.x + 26, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.y);

        figCounterCircle[musObjNr].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();

        //parent and position
        float posX = UtilitiesTm.FloatRemap(moment, 0, AnimationTimer.GetMaxTime(), gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2);
        newCopyOfFigure.transform.SetParent(gameObject.transform);
        newCopyOfFigure.transform.localPosition = new Vector2(posX, newCopyOfFigure.transform.localPosition.y);
        scaleObject(newCopyOfFigure.transform.GetChild(1).gameObject, 100, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);

        //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
        updateObjectList(timelineInstanceObjects, newCopyOfFigure);
        openTimelineByClick(true);
        newCopyOfFigure.transform.localScale = Vector3.one;

        //openTimelineByClick(false);

        ////set original image back to shelf
        //setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[musObjNr]);
        ////scale to default values
        //scaleObject(figureObjects[musObjNr], objectShelfSize[musObjNr].x, objectShelfSize[musObjNr].y);
        //scaleObject(figureObjects[musObjNr].transform.GetChild(0).gameObject, objectShelfSize[musObjNr].x, objectShelfSize[musObjNr].y);

        ////set this position
        //figureObjects[musObjNr].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
        //// bring object back to position two, so that its visible
        //figureObjects[musObjNr].transform.SetSiblingIndex(1);
    }
    public void ResetScreenSize()
    {
        //Debug.Log("lossyScale: " + gameObject.transform.lossyScale);
        //Debug.Log("Screen changed! ScreenX: " + Screen.width);

        minX = 0.146f * Screen.width;// / gameObject.transform.lossyScale.x; //301.0f;  //timeline-minX
        //Debug.Log("minX: " + minX);
        railWidth = 0.69f * Screen.width;// / gameObject.transform.lossyScale.x;
        heightClosed = 0.018f * Screen.height;// / gameObject.transform.lossyScale.x;
        heightOpened = 0.074f * Screen.height;// / gameObject.transform.lossyScale.x;
        maxX = minX + railWidth;  //timeline-maxX
                                  //Debug.Log("rail start: " + minX);
                                  //Debug.Log("isTimelineopen: " + isTimelineOpen + "heightclosed: " + heightClosed);
        if (isTimelineOpen)
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightOpened / gameObject.transform.lossyScale.x);
            //Debug.Log("size box collider: " + gameObject.GetComponent<BoxCollider2D>().size.y);
        }
        else
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
            //Debug.Log("size: " + timelineImage.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
    public void PlaySample(int i)
    {
        if (sampleButtonPressed == (i - 1))
        {
            playingSample = false;
            sampleButtonPressed = -1;
        }
        else
        {
            playingSample = true;
            audioSource.clip = clip[i - 1];
            sampleButtonPressed = (i - 1);
        }
    }

    void Update()
    {
        if (currentLossyScale != transform.lossyScale.x)
        {
            currentLossyScale = transform.lossyScale.x;
            //Debug.Log("scale after: " + transform.lossyScale.x);
            ResetScreenSize();
        }

        Vector2 getMousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //identify which gameobject you clicked
            string objectClicked = identifyClickedObject();         //method fills up the current clicked index
            identifyClickedObjectByList(timelineInstanceObjects);
            editTimelineObject = false;                             //flag to prevent closing the timeline if you click an object in timeline
            releaseOnTimeline = false;                              //because you have not set anything on timeline 
            releaseObjMousePos = new Vector2(0.0f, 0.0f);

            int tmpI = -1;
            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                if (timelineInstanceObjects[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    tmpI = i;
                }
            }
            if (tmpI >= 0)
            {
                editTimelineObject = true;
            }
            else
            {
                editTimelineObject = false;
            }

            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                openTimelineByClick(isTimelineOpen);
                //draggingOnTimeline = true;
            }

            //if you click on an object in shelf
            //first checking if you have an object clicked in shelf
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    draggingObject = true;
                }
            }
            //or check if you click an object in timeline
            if ((currentClickedInstanceObjectIndex != (-1)) && (editTimelineObject == true))
            {
                diff = new Vector2(getMousePos.x - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, getMousePos.y - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.y);
                //Debug.Log("clicked");
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    draggingObject = true;
                    draggingOnTimeline = true;
                }

            }

            //if you hit/clicked nothing with mouse
            if (Physics2D.OverlapPoint(getMousePos) == false)
            {
            }
        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline && isTimelineOpen)
        {
            //Debug.Log("+++++++++++++++++++++++++dragging on timeline......");
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                isInstance = true;
                //if you click an object in timeline (for dragging)
                //move object
                updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos - diff); // hier muss die maus position minus pivot point genommen werden! oder so
                //snapping/lock y-axis
                setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, this.transform.position.y);

                //-------------------------------------------------limit front of rail------------------------------------------//
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().position.x < (minX + 0.035f * Screen.width))
                {
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().position = new Vector2((float)minX + 0.035f * Screen.width, GetComponent<RectTransform>().position.y);    // tendenziell muesste das eher in buttonUp
                }
                //-------------------------------------------------limit back of rail------------------------------------------//
                if ((timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) > (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width))
                {
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x), newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y);
                    //Debug.LogWarning("JAAAAA! posx: " + (newCopyOfFigure.transform.position.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) + ", anchored Pos: " + (newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) + ", maxX: " + (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width));
                }

                if (Physics2D.OverlapPoint(getMousePos) == false)       // mouse outside
                {
                    SceneManaging.objectsTimeline = ((int)Char.GetNumericValue(timelineImage.name[17]) - 1);  // save old timeline to remove instance of this timeline
                    draggingObject = true;
                    //editTimelineObject = false;
                    draggingOnTimeline = false;
                }
            }
            releaseOnTimeline = true;
        }
        //if something has been dragged outside of the timeline
        if (draggingOnTimeline == false && editTimelineObject == false && draggingObject && isInstance)
        {
            bool hitTimeline;
            hitTimeline = checkHittingTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos);

            timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(false);
            //moving on mouse pos
            updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos);
            releaseOnTimeline = false;
        }
        // dragging an object from shelf to timeline
        if (draggingObject && editTimelineObject == false && isInstance == false)
        {
            //move object
            updateObjectPosition(figureObjects[currentClickedObjectIndex], getMousePos);

            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            bool hitTimeline = false;
            hitTimeline = checkHittingTimeline(figureObjects[currentClickedObjectIndex], timelineImage, getMousePos);

            if (hitTimeline)
            {
                //open timeline, if its not open
                openCloseTimelineByDrag();

                //scale up object/figures in timeline
                openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
                //and set animation-length
                float animationLength = 100.0f;     //length of objects animation

                scaleObject(figureObjects[currentClickedObjectIndex], animationLength, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, animationLength, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);

                // change parent back
                setParent(figureObjects[currentClickedObjectIndex], gameObject);

                //snapping/lock y-axis
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], figureObjects[currentClickedObjectIndex].transform.position.x, this.transform.position.y);

                //set placed figures to timelineobjects-list
                updateObjectList(timelineObjects, figureObjects[currentClickedObjectIndex]);

                //save position, where object on timeline is released + set flag
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
                //set up flags
            }
            else
            {
                scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
                //close timeline if you click e.g. in the shelf to get a new figure
                // openCloseTimelineByDrag();
                // openCloseObjectInTimeline(false, timelineInstanceObjects, editTimelineObject);
                // scale up currentFigure
                releaseOnTimeline = false;
            }
        }
        //-------release mousebutton
        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            draggingObject = false;
            editTimelineObject = false;

            if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)  //if release obj-pos and release mouse-button-pos is the same
                {
                    //create a copy of this timelineObject and keep the original one
                    newCopyOfFigure = Instantiate(figureObjects[currentClickedObjectIndex]);
                    //count objects from same kind
                    int countName = 0;
                    countName = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    newCopyOfFigure.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");

                    float tmpLength = ((float)maxX - (float)minX) * newCopyOfFigure.GetComponent<MusicLength>().musicLength / 614;//UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, 614, (float)minX, (float)maxX);
                    createRectangle(newCopyOfFigure, new Vector2(300, gameObject.GetComponent<RectTransform>().rect.height * 0.96f), colMusic, minX, tmpLength);

                    figCounterCircle[currentClickedObjectIndex].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();

                    //parent and position
                    newCopyOfFigure.transform.SetParent(gameObject.transform);
                    newCopyOfFigure.transform.localScale = Vector3.one;

                    //------------------------------------------limit front of rail---------------------------------------------//
                    if (figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().position.x < (minX + 0.03f * Screen.width))  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
                    {
                        //Debug.Log("achtung! " + figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().position.x + ", min X: " + minX);
                        newCopyOfFigure.GetComponent<RectTransform>().position = new Vector2((float)minX + 0.03f * Screen.width, figureObjects[currentClickedObjectIndex].transform.position.y);    // tendenziell muesste das eher in buttonUp
                    }
                    else
                    {
                        newCopyOfFigure.transform.position = new Vector2(figureObjects[currentClickedObjectIndex].transform.position.x, figureObjects[currentClickedObjectIndex].transform.position.y);
                    }

                    // size of rectangle becomes size for figure that is clickable
                    newCopyOfFigure.GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.GetComponent<RectTransform>().position.x, newCopyOfFigure.GetComponent<RectTransform>().position.y, -1.0f);
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2 - 50, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);


                    //------------------------------------------limit back of rail---------------------------------------------//
                    if ((newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) > (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width))
                    {
                        newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector2((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x), newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y);
                        //Debug.LogWarning("JAAAAA! posx: " + (newCopyOfFigure.transform.position.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) + ", anchored Pos: " + (newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) + ", maxX: " + (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width));
                    }
                    else
                    {
                        //Debug.LogWarning("NEEEIN! posx: " + (newCopyOfFigure.transform.position.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) + ", anchored Pos: " + (newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) + ", maxX: " + (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width));
                    }

                    //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
                    updateObjectList(timelineInstanceObjects, newCopyOfFigure);

                    //set original image back to shelf
                    setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                    //scale to default values
                    scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);

                    //set this position
                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
                    // bring object back to position two, so that its visible
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Save to SceneData:
                    MusicClipElementInstance musicClipElementInstance = new MusicClipElementInstance();
                    musicClipElementInstance.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    musicClipElementInstance.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");
                    //float moment = UtilitiesTm.FloatRemap(timelineInstanceObjects[currentClickedObjectIndex].transform.localPosition.x, gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2, 0, AnimationTimer.GetMaxTime());
                    //musicClipElementInstance.moment = moment;
                    StaticSceneData.StaticData.musicClipElements[currentClickedObjectIndex].musicClipElementInstances.Add(musicClipElementInstance);

                    Debug.LogWarning(JsonUtility.ToJson(StaticSceneData.StaticData, true));

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////

                    // set index back to -1 because nothing is being clicked anymore
                    currentClickedObjectIndex = -1;
                }

            }
            // if dropped somewhere else than timeline: bring figure back to shelf
            else if (releaseOnTimeline == false && currentClickedObjectIndex != -1 && isInstance == false)
            {
                setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                // bring object back to position two, so that its visible and change position back to default
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
            }
            else if (releaseOnTimeline == false && currentClickedInstanceObjectIndex != -1 && isInstance)
            {
                removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //Save musictitle.moment to SceneData:
            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                int musObjIndex = Int32.Parse(timelineInstanceObjects[i].name.Substring(6, 2)) - 1;
                int musInsIndex = Int32.Parse(timelineInstanceObjects[i].name.Substring(17));
                float moment = UtilitiesTm.FloatRemap(timelineInstanceObjects[i].transform.localPosition.x, gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2, 0, AnimationTimer.GetMaxTime());

                //Debug.LogWarning(timelineInstanceObjects[i].name.Substring(17) + " _ " + musInsIndex + " _ mom " + moment);

                StaticSceneData.StaticData.musicClipElements[musObjIndex].musicClipElementInstances[musInsIndex].moment = moment;
                Debug.LogWarning(JsonUtility.ToJson(StaticSceneData.StaticData, true));
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            releaseOnTimeline = false;
            releaseObjMousePos.x = 0.0f;
            releaseObjMousePos.y = 0.0f;
            draggingOnTimeline = false;
            isInstance = false;
        }

        bool anyInstanceIsPlaying = false;
        // turning music on and off in playmode
        if (SceneManaging.playing)
        {
            //Debug.Log("+++++++++++++++++updateMusic: " + SceneManaging.updateMusic);
            //Debug.Log("+++++++++++++++++Music is playing");
            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                //float tmpLength = ((float)maxX - (float)minX) * timelineInstanceObjects[i].GetComponent<MusicLength>().musicLength / 614;//UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, 614, (float)minX, (float)maxX);
                double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], timelineInstanceObjects[i].GetComponent<MusicLength>().musicLength, maxTimeInSec, minX, maxX);
                double endSec = calculateMusicEndTimeInSec(timelineInstanceObjects[i], timelineInstanceObjects[i].GetComponent<MusicLength>().musicLength, maxTimeInSec, minX, maxX);
                float tmpTime = AnimationTimer.GetTime();
                //Debug.Log(", startSec: " + startSec + "endSec: " + endSec + ", Timer: " + AnimationTimer.GetTime());// + ", LENGTH: " + tmpLength);

                // wenn timer im bereich musikstuecks und musik ist nicht an
                if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec)
                {
                    anyInstanceIsPlaying = true;
                    // hitObject = ((int)Char.GetNumericValue(timelineInstanceObjects[i].name[17])); // instance index
                    SceneManaging.hitObject = i;
                    SceneManaging.hittingSomething = true;
                    //Debug.Log("hitobject: " + SceneManaging.hitObject);
                    if (playingMusic == false)
                    {
                        playingMusic = true;
                        audioSource.time = tmpTime - (float)startSec;
                        currentClip = ((int)Char.GetNumericValue(timelineInstanceObjects[i].name[07]) - 1); // object index
                        audioSource.clip = clip[currentClip];
                        audioSource.Play();
                        //Debug.Log("++++++++++MUSIC STARTS ++++++ tmpTime: " + audioSource.time + ", current clip: " + currentClip + ", startsec: " + startSec + ", tmpTime: " + tmpTime);
                    }
                    if (SceneManaging.updateMusic)
                    {
                        {
                            //Debug.Log("+++++++++++++++++++TimeSlider ist im music piece and slider is being dragged.");
                            //audioSource.time = tmpTime - (float)startSec;
                            //currentClip = ((int)Char.GetNumericValue(timelineInstanceObjects[i].name[07]) - 1); // object index
                            //audioSource.clip = clip[currentClip];
                            playingMusic = false;
                        }
                    }
                    // else if (AnimationTimer.GetTime() < startSec || AnimationTimer.GetTime() > endSec && playingMusic == true)
                    // {
                    //     if(((int)Char.GetNumericValue(timelineInstanceObjects[SceneManaging.hitObject].name[7])-1) == currentClip)
                    //     {
                    //         Debug.Log(" slider ist beim aktuellen clip ");
                    //     }
                    //     playingMusic = false;
                    //     Debug.Log("+++++++++++++++++++TimeSlider ist ausserhalb und die Musik stoppt.");
                    //     audioSource.Stop();
                    //     SceneManaging.hittingSomething = false;
                    //     SceneManaging.hitObject = -1;
                    // }

                    // else if (SceneManaging.updateMusic)
                    // {
                    //     if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec)
                    //     {
                    //         Debug.Log("+++++++++++++++++++TimeSlider ist im music piece and slider is being dragged.");
                    //         audioSource.time = tmpTime - (float)startSec;
                    //     }
                    // if ((AnimationTimer.GetTime() < startSec || AnimationTimer.GetTime() > endSec) && playingMusic == true)
                    // {
                    //     Debug.Log("+++++++++++++++++++TimeSlider wird aussesrhalb des stuecks gescrubbt.");
                    //     audioSource.Stop();
                    //     playingMusic = false;
                    // }
                }

                //else if (SceneManaging.hitObject != 0 && SceneManaging.hitObject != 1 && SceneManaging.hitObject != 2 && SceneManaging.hitObject != 3 && SceneManaging.hitObject != 4 && SceneManaging.hitObject != 5)
                //{
                //    audioSource.Stop();
                //    playingMusic = false;
                //    Debug.Log("stop");
                //}
            }
        }

        ///////////////////////////playing Sample/////////////////////////
        else if (playingSample)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.time = 50f;
                //Debug.Log("hier");
                audioSource.Play();
            }
            else
            {
                if (audioSource.time > 60f)
                {
                    playingSample = false;
                }
            }
        }
        /////////////////////////////////////////////////////////////////////

        else
        {
            audioSource.Stop();
            playingMusic = false;
        }

        if (!anyInstanceIsPlaying && playingSample == false)
        {
            audioSource.Stop();
            playingMusic = false;
        }
    }
}