using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailMusicManager : MonoBehaviour
{
    public Image timelineImage;
    AudioSource audioSource;
    public GameObject gameController;
    public Image timeSliderImage;
    private BoxCollider2D timeSlider;
    public GameObject rail3dObj;
    bool draggingOnTimeline;        //dragging the mouse over the timeline
    bool draggingObject;            //dragging an object with mouse
    bool doSomethingOnce;           //flag for do something once in a loop
    bool editTimelineObject;        //flag for shifting/clicking an object on timeline
    bool releaseOnTimeline;         //flag if you set an object on timeline
    bool playingMusic;
    bool isInstance;
    public bool isTimelineOpen;
    Vector2 releaseObjMousePos;
    double minX;
    double maxX;
    //double minY;
    //double maxY;
    string maxTimeLength;
    int maxTimeInSec;
    int currentClip;
    Vector2 sizeDeltaAsFactor;
    Vector2[] objectShelfPosition;
    Vector2[] objectShelfSize;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    Vector2 objectSceneSize;

    public GameObject objectLibrary;
    public GameObject parentMenue; // mainMenue
    GameObject timeSettings;
    GameObject[] figCounterCircle;

    GameObject[] figureObjects;
    int currentClickedObjectIndex;
    int currentClickedInstanceObjectIndex;

    public List<GameObject> timelineObjects;
    public List<GameObject> timelineInstanceObjects;
    Vector3 railStartPos;
    Vector3 railEndPos;
    Color colMusic;//, colMusicHighlighted;


    public AudioClip[] clip;         // ought to be 6 audioclips


    void Awake()
    {
        timelineImage = this.GetComponent<Image>();
        SceneManaging.anyTimelineOpen = false;
        draggingOnTimeline = false;
        draggingObject = false;
        doSomethingOnce = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        playingMusic = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        currentClip = 0;
        isTimelineOpen = false;
        isInstance = false;

        //load all objects given in the figuresShelf
        //Debug.Log("objectscount: "+objectLibrary.transform.childCount);
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

        //Debug.Log("++++++figures loaded: " + figureObjects.Length);
        //Debug.Log("++++++figures3D loaded: " + figureObjects3D.Length);

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;

        timeSettings = timeSliderImage.transform.GetChild(0).gameObject; // GameObject.Find("ImageTimeSettingsArea");
    }

    void Start()
    {
        scaleObject(gameObject, 1335, 15);
        audioSource = GetComponent<AudioSource>();
        timeSettings.SetActive(false);
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
            figCounterCircle[i].transform.GetChild(0).GetComponent<Text>().text = "0";
        }

        List<GameObject> timelineObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects = new List<GameObject>();
        railStartPos = new Vector3(0.0f, 0.0f, 0.0f);
        railEndPos = new Vector3(0.0f, 0.0f, 0.0f);
        railStartPos = getRailStartEndpoint(rail3dObj, "start");
        railEndPos = getRailStartEndpoint(rail3dObj, "end");

        minX = 301.0f;  //timeline-minX
        maxX = 1623.0f; //timeline-maxX
        maxTimeLength = "10:14";
        maxTimeInSec = 614;
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
    public void openCloseTimelineByClick(bool thisTimelineOpen, Image tl, bool editObjOnTl)
    {
        float heightOpened = 80.0f;
        float heightClosed = 15.0f;    //this size is set in editor

        if (isAnyTimelineOpen() == false)
        {
            Debug.Log("++++ es ist keine schiene geöffnet, deswegen wird geklickte Schiene geöffnet: " + tl);
            //set global flag
            SceneManaging.anyTimelineOpen = true;
            isTimelineOpen = true;
            //scale up timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
            //minimize or maximize objects on timeline
            openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
            ImageTimelineSelection.SetRailNumber(6);
            ImageTimelineSelection.SetRailType(2);  // for rail-rails

        }
        else if (isAnyTimelineOpen() && editObjOnTl == false)
        {
            if (thisTimelineOpen)
            {
                Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
                //close timeline
                isTimelineOpen = false;
                //scale down timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                //scale down the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                openCloseObjectInTimeline(false, timelineInstanceObjects, editTimelineObject);
            }
            else
            {
                Debug.Log("++++ geklickte Schiene ist zu, aber eine andere ist offen und wird geschlossen: " + tl);
                // a different rail is open - close it
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                    gameController.GetComponent<UIController>().Rails[i].isTimelineOpen = false;
                    //GameController.GetComponent<UIController>().Rails[i].transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 1, 1);
                    openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects, editTimelineObject);
                    Debug.Log("++++ Scaling down Rail: " + gameController.GetComponent<UIController>().Rails[i]);
                }
                for (int j = 0; j < 2; j++)
                {
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                gameController.GetComponent<UIController>().RailMusic.isTimelineOpen = false;
                // open clicked rail
                Debug.Log("++++ geklickte Schiene wird geöffnet: " + tl);
                //scale up timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
                //scale up the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
                openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
                isTimelineOpen = true;
                ImageTimelineSelection.SetRailNumber(6);
                ImageTimelineSelection.SetRailType(2);  // for rail-rails
            }

        }
        openCloseTimeSettings(isAnyTimelineOpen(), timeSettings);

    }
    public void openCloseTimelineByDrag(string open, Image tl)
    {
        float heightOpened = 80.0f;
        float heightClosed = 15.0f;    //this size is set in editor

        if (open == "open")
        {
            //Debug.Log("++++ geklickte Schiene ist zu und wird geöffnet: " + tl);
            isTimelineOpen = true;
            //scale up timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
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
            //Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
            isTimelineOpen = false;
            //scale down timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
            //scale down the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
            //scale up all objects on timeline
            openCloseObjectInTimeline(isTimelineOpen, timelineInstanceObjects, editTimelineObject);
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
        obj.transform.position = new Vector2(mousePos.x, mousePos.y);
        //set up flags
        //Debug.Log("mouse: " + mousePos);
    }
    public bool checkHittingTimeline(GameObject obj, Image tl, Vector2 mousePos)
    {
        bool hit = false;
        /*Debug.Log("tl pos "+tl.transform.position); //global pos are the real position-data
        Debug.Log("tl collider "+tl.GetComponent<BoxCollider2D>().size);
        Debug.Log("mouse "+mousePos);
        Debug.Log("obj collider "+obj.GetComponent<BoxCollider2D>().size);
        */
        //calculate bounding-box related to the timeline-pos
        Vector2 tlPos = new Vector2(tl.transform.position.x, tl.transform.position.y);
        Vector2 colSize = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, tl.GetComponent<BoxCollider2D>().size.y);
        //if mouse hits the timeline while dragging an object
        //my implementation of object-boundingbox
        if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
        ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
        {
            //Debug.Log("object hits timeline!");
            hit = true; ;
        }
        //Debug.Log("drag and hit " + hit);
        return hit;
    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects, bool editObjOnTl)
    {
        float scaleUp = 80.0f;
        float scaleDown = 15.0f;
        float length = 100.0f;
        //Debug.Log("method: timelineObjects count: "+objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            //Debug.Log("object size: "+objects[i].GetComponent<RectTransform>().sizeDelta);
            //if timeline open scale ALL objects up
            if (timelineOpen)
            {
                scaleObjectHeight(objects[i], length, scaleUp);
                scaleObjectHeight(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleUp);
                scaleObjectHeight(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleUp);
                objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -40.0f);
                //objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,50.0f);
                //new Vector2(animationLength,scaleYUp);
            }
            else if ((timelineOpen == false) && (editObjOnTl == false))
            {
                //otherwise scale ALL objects down
                //Debug.Log("method: scale object down:");
                scaleObjectHeight(objects[i], length, scaleDown);
                scaleObjectHeight(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleDown);
                scaleObjectHeight(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleDown);
                objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -6.0f);
                //objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,10.0f);
            }
        }
    }

    public void scaleObjectHeight(GameObject fig, float x, float y)
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
    public void scaleObjectsOfList(List<GameObject> objects, float x, float y)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            scaleObject(objects[i], x, y);
        }
    }
    public void setPivot(GameObject obj, float x, float y)
    {
        obj.GetComponent<RectTransform>().pivot = new Vector2(x, y);
    }
    public void setObjectOnTimeline(GameObject fig, float x, float y)
    {
        fig.transform.position = new Vector3(fig.transform.position.x, y, 0.0f);
    }
    public void ripObjectOffTimeline(GameObject fig, float x, float y)
    {
        fig.transform.position = new Vector3(x, y, 0.0f);
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
        trans.sizeDelta = new Vector2((float)animLength, 80.0f); // custom size

        Image image = imgObject.AddComponent<Image>();
        image.color = col;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
        //set pivot point of sprite back to midpoint of sprite
        obj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
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
    public Vector3 getRailStartEndpoint(GameObject r3DObj, string startEnd)
    {
        Vector3 point = new Vector3(0.0f, 0.0f, 0.0f);
        if (startEnd == "start")
        {
            //calculate start point of the rail
            point = new Vector3(0.0f, 0.0f, -2.2f);
        }
        else //(startEnd="end")
        {
            //calculate end point of rail 
            point = new Vector3(0.0f, 0.0f, 2.6f);
        }
        return point;
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
        int tmpNr = int.Parse(obj.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text);
        Destroy(obj);
        timelineInstanceObjects.Remove(obj);
        //Debug.Log("tmpNr: " + tmpNr + "currentCounterNr: " + currentCounterNr + ", Instances: " + timelineInstanceObjects.Count);
        Debug.Log("+++Removed: " + obj);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
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

    public void highlight(GameObject obj)
    {
        //obj.transform.GetChild(0).GetComponent<Image>().color = colMusicHighlighted;
        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);   //show Delete-Button
        SceneManaging.highlighted = true;
    }
    public void unhighlight(GameObject obj)
    {
        obj.transform.GetChild(0).GetComponent<Image>().color = colMusic;
        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);  //hide Delete-Button
        SceneManaging.highlighted = false;
    }

    void Update()
    {
        Vector2 getMousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //identify which gameobject you clicked
            string objectClicked = identifyClickedObject();         //method fills up the current clicked index
            identifyClickedObjectByList(timelineInstanceObjects);
            editTimelineObject = false;                             //flag to prevent closing the timeline if you click an object in timeline
            releaseOnTimeline = false;                              //because you have not set on timeline anything
            releaseObjMousePos = new Vector2(0.0f, 0.0f);

            int tmpI = -1;
            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                if (timelineInstanceObjects[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //Debug.Log("-----------found instance object!!!!->i: "+i);
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
                openCloseTimelineByClick(isTimelineOpen, timelineImage, editTimelineObject);
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
        if (draggingOnTimeline && editTimelineObject && isTimelineOpen)
        {
            //Debug.Log("+++++++++++++++++++++++++dragging on timeline......");
            //Debug.Log("--->before-if-index: "+currentClickedInstanceObjectIndex);
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                isInstance = true;
                //if you click an object in timeline (for dragging)
                //move object
                updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos); // hier muss die maus position minus pivot point genommen werden! oder so
                                                                                                               //snapping/lock y-axis
                setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, this.transform.position.y);

                if (Physics2D.OverlapPoint(getMousePos) == false)       // mouse outside
                {
                    SceneManaging.objectsTimeline = ((int)Char.GetNumericValue(timelineImage.name[17]) - 1);  // save old timeline to remove instance of this timeline
                    draggingObject = true;
                    editTimelineObject = false;
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
                openCloseTimelineByDrag("open", timelineImage);

                //scale up object/figures in timeline
                openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
                //and set animation-length
                float animationLength = 100.0f;     //length of objects animation
                float scaleY = 80.0f;             //size if the timeline is maximized
                                                  //figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(animationLength,scaleYUp);
                                                  //scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleYUp);
                                                  //scale down the dragged figure (and childobject: image)
                scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleY);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, animationLength, scaleY);

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

                scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);

                //close timeline if you click e.g. in the shelf to get a new figure
                openCloseTimelineByDrag("close", timelineImage);

                //scale down also the figures on timeline
                openCloseObjectInTimeline(false, timelineInstanceObjects, editTimelineObject);
                // scale up currentFigure

                releaseOnTimeline = false;
            }
        }

        //-------release mousebutton
        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            //Debug.Log("release mouse button at pos: " + getMousePos + " " + Physics2D.OverlapPoint(getMousePos));
            //Debug.Log("++++++left mouse button up");
            draggingObject = false;
            doSomethingOnce = false;
            editTimelineObject = false;

            if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)  //if release obj-pos and release mouse-button-pos is the same
                {
                    //create a copy of this timelineObject and keep the original one
                    newCopyOfFigure = Instantiate(figureObjects[currentClickedObjectIndex]);
                    Debug.Log("+++newcopy: " + newCopyOfFigure);
                    //count objects from same kind
                    int countName = 0;
                    countName = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    newCopyOfFigure.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName;

                    float tmpLength = ((float)maxX - (float)minX) * newCopyOfFigure.GetComponent<MusicLength>().musicLength / 614;//UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, 614, (float)minX, (float)maxX);
                    createRectangle(newCopyOfFigure, new Vector2(300, 50), colMusic, minX, tmpLength);
                    Debug.Log("musicLenght: " + tmpLength);


                    figCounterCircle[currentClickedObjectIndex].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();

                    //parent and position
                    newCopyOfFigure.transform.SetParent(gameObject.transform);
                    newCopyOfFigure.transform.position = new Vector2(figureObjects[currentClickedObjectIndex].transform.position.x, figureObjects[currentClickedObjectIndex].transform.position.y);

                    //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
                    updateObjectList(timelineInstanceObjects, newCopyOfFigure);

                    //Debug.Log("copies of this object:"+countCopiesOfObject(newCopyOfFigure,timelineObjects));

                    //set original image back to shelf
                    setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                    //scale to default values
                    scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);

                    //set this position
                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
                    // bring object back to position two, so that its visible
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);

                    //	Debug.Log("count timelineInstancesObjects: "+timelineInstanceObjects.Count);

                    // set index back to -1 because nothing is being clicked anymore
                    currentClickedObjectIndex = -1;
                }

            }

            // if dropped somewhere else than timeline: bring figure back to shelf
            else if (releaseOnTimeline == false && currentClickedObjectIndex != -1 && isInstance == false)
            {
                //Debug.Log("+++++++++++gameobject: " + gameObject.name + ", index: " + currentClickedObjectIndex);

                setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                // bring object back to position two, so that its visible and change position back to default
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
            }
            else if (releaseOnTimeline == false && currentClickedInstanceObjectIndex != -1 && isInstance)
            {
                Debug.Log("remove");
                removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            }

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
                double endSec = calculateMusicEndTimeInSec(timelineInstanceObjects[i],timelineInstanceObjects[i].GetComponent<MusicLength>().musicLength, maxTimeInSec, minX, maxX);
                float tmpTime = AnimationTimer.GetTime();
                Debug.Log(", startSec: " + startSec + "endSec: " + endSec + ", Timer: " + AnimationTimer.GetTime());// + ", LENGTH: " + tmpLength);

                // wenn timer im bereich musikstuecks und musik ist nicht an
                if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec)
                {
                    anyInstanceIsPlaying = true;
                    // hitObject = ((int)Char.GetNumericValue(timelineInstanceObjects[i].name[17])); // instance index
                    SceneManaging.hitObject = i;
                    SceneManaging.hittingSomething = true;
                    Debug.Log("hitobject: " + SceneManaging.hitObject);
                    if (playingMusic == false)
                    {
                        playingMusic = true;
                        audioSource.time = tmpTime - (float)startSec;
                        currentClip = ((int)Char.GetNumericValue(timelineInstanceObjects[i].name[07]) - 1); // object index
                        audioSource.clip = clip[currentClip];
                        audioSource.Play();
                        Debug.Log("++++++++++MUSIC STARTS ++++++ tmpTime: " + audioSource.time + ", current clip: " + currentClip + ", startsec: " + startSec + ", tmpTime: " + tmpTime);
                    }
                    if (SceneManaging.updateMusic)
                    {
                        {
                            Debug.Log("+++++++++++++++++++TimeSlider ist im music piece and slider is being dragged.");
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
        else
        {
            audioSource.Stop();
            playingMusic = false;
        }

        if (!anyInstanceIsPlaying)
        {
            audioSource.Stop();
            playingMusic = false;
        }
    }
}