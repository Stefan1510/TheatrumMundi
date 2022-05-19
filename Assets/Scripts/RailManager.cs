using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailManager : MonoBehaviour
{
    private Image timelineImage;
    AudioSource audioSource;
    public GameObject gameController;
    public Image timeSliderImage;
    private BoxCollider2D timeSlider;
    public GameObject rail3dObj;
    bool draggingOnTimeline;        //dragging the mouse over the timeline
    bool draggingObject;            //dragging an object with mouse
    bool editTimelineObject;        //flag for shifting/clicking an object on timeline
    bool releaseOnTimeline;         //flag if you set an object on timeline
    bool isInstance;
    public bool isTimelineOpen;
    Vector2 releaseObjMousePos;
    double minX, minY;
    double maxX;
    float railWidth; //, railHeight;
    int maxTimeInSec;
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
    GameObject[] figureObjects3D;
    int currentClickedObjectIndex;
    int currentClickedInstanceObjectIndex;
    public List<GameObject> timelineInstanceObjects;
    public List<GameObject> timelineInstanceObjects3D;
    private FigureElement ThisFigureElement;    //element to set 3d object
    float heightOpened, heightClosed;
    Color colFigure, colFigureHighlighted;
    private float currentLossyScale;

    void Awake()
    {
        timelineImage = this.GetComponent<Image>();
        SceneManaging.anyTimelineOpen = false;
        draggingOnTimeline = false;
        draggingObject = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        isTimelineOpen = false;
        isInstance = false;

        //load all objects given in the figuresShelf
        //Debug.Log("objectscount: "+objectLibrary.transform.childCount);
        figureObjects = new GameObject[objectLibrary.transform.childCount];         //instead of a count like "3"
        figureObjects3D = new GameObject[figureObjects.Length];     //instead of a count like "3"
        objectShelfPosition = new Vector2[figureObjects.Length];
        objectShelfSize = new Vector2[figureObjects.Length];
        objectShelfParent = new GameObject[figureObjects.Length];
        figCounterCircle = new GameObject[figureObjects.Length];
        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = (objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject);
            objectShelfSize[i] = new Vector2(figureObjects[i].GetComponent<RectTransform>().rect.width, figureObjects[i].GetComponent<RectTransform>().rect.height);
            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
        }
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
            figCounterCircle[i].transform.GetChild(0).GetComponent<Text>().text = "0";
        }
        colFigure = new Color(0.06f, 0.66f, .74f, 0.5f);
        colFigureHighlighted = new Color(0f, 0.87f, 1.0f, 0.5f);


        try
        {
            for (int i = 0; i < figureObjects3D.Length; i++)
            {
                figureObjects3D[i] = gameController.GetComponent<SceneDataController>().objectsFigureElements[i];
            }
        }
        catch (IndexOutOfRangeException ex) { }

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;
        heightOpened = 0.074f * Screen.height;
        heightClosed = 0.023f * Screen.height;    //this size is set in editor
        timeSettings = timeSliderImage.transform.GetChild(0).gameObject;
        currentLossyScale = 1.0f;
    }

    void Start()
    {
        ResetScreenSize();
        timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth, heightClosed);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth, heightClosed);

        timeSettings.SetActive(false);

        List<GameObject> timelineObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects = new List<GameObject>();
        List<GameObject> timelineObjects3D = new List<GameObject>();
        List<GameObject> timelineInstanceObjects3D = new List<GameObject>();
        //maxTimeLength = "10:14";
        maxTimeInSec = (int)AnimationTimer.GetMaxTime();

        for (int i = 0; i < figureObjects3D.Length - 1; i++) // ships dont have animation
        {
            //Debug.Log("++++++++figureObject: " + figureObjects3D[i]); // + ", isShip: " + figureObjects3D[i].GetComponent<FigureStats>().isShip);
            if (figureObjects3D[i].GetComponent<FigureStats>().isShip)
            {
                //Debug.Log("figure: " + figureObjects3D[i] + " is a ship.");
                for (int j = 0; j < figureObjects3D[i].GetComponent<MeshRenderer>().materials.Length; j++)
                {
                    figureObjects3D[i].GetComponent<MeshRenderer>().materials[j].DisableKeyword("_EMISSION");
                }
            }
            else
            {
                //Debug.Log("figure: " + figureObjects3D[i] + " is not a ship.");
                for (int j = 0; j < figureObjects3D[i].transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials.Length; j++)
                {
                    figureObjects3D[i].transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[j].DisableKeyword("_EMISSION");
                }
                figureObjects3D[i].GetComponent<Animator>().enabled = false;

            }
        }
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
        if (isAnyTimelineOpen() == false)
        {
            // Debug.Log("++++ es ist keine schiene geöffnet, deswegen wird geklickte Schiene geöffnet: " + tl);
            //set global flag
            SceneManaging.anyTimelineOpen = true;
            isTimelineOpen = true;
            //scale up timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
            //minimize or maximize objects on timeline
            openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
            ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(timelineImage.name[17]) - 1);
            ImageTimelineSelection.SetRailType(0);  // for rail-rails

        }
        else if (isAnyTimelineOpen() && editObjOnTl == false)
        {
            if (thisTimelineOpen)
            {
                // Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
                //close timeline
                isTimelineOpen = false;
                //scale down timeline and collider
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                openCloseObjectInTimeline(false, timelineInstanceObjects, editTimelineObject);
            }
            else
            {
                // Debug.Log("++++ geklickte Schiene ist zu, aber eine andere ist offen und wird geschlossen: " + tl);
                // a different rail is open - close it
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().isTimelineOpen = false;
                    openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects, editTimelineObject);
                    //Debug.Log("++++ Scaling down Rail: " + gameController.GetComponent<UIController>().Rails[i]);
                }
                for (int j = 0; j < 2; j++)
                {
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects, editObjOnTl);
                // open clicked rail
                //Debug.Log("++++ geklickte Schiene wird geöffnet: " + tl);
                //scale up timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
                //scale up the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
                openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
                isTimelineOpen = true;

                ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(timelineImage.name[17]) - 1);
                ImageTimelineSelection.SetRailType(0);  // for rail-rails
            }
        }
        openCloseTimeSettings(isAnyTimelineOpen(), timeSettings);

    }
    public void openCloseTimelineByDrag(string open, Image tl)
    {

        if (open == "open")
        {
            // if (gameController.GetComponent<UIController>().RailLightBG[0].isTimelineOpen)
            // {
            //     gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<RailLightManager>().openCloseTimelineByClick(false, timelineImage);
            // }
            // if (gameController.GetComponent<UIController>().RailLightBG[1].isTimelineOpen)
            // {
            //     gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<RailLightManager>().openCloseTimelineByClick(false, timelineImage);
            // }
            // if (gameController.GetComponent<UIController>().RailMusic.isTimelineOpen)
            // {
            //     gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseTimelineByClick(false, timelineImage, false);
            //     // Debug.Log("++++ Musikschiene ist offen und wird geschlossen: ");
            // }
            // Debug.Log("++++ geklickte Schiene ist zu und wird geöffnet: " + tl);
            isTimelineOpen = true;
            //scale up timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
            //scale up all objects on timeline
            openCloseObjectInTimeline(isTimelineOpen, timelineInstanceObjects, editTimelineObject);
            ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(timelineImage.name[17]) - 1);
            ImageTimelineSelection.SetRailType(0);  // for rail-rails
        }
        else
        {
            // Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
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
            if (gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().isTimelineOpen == true)
            {
                val = true;
            }
        }
        if (gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen == true) val = true;
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
    public bool checkHittingTimeline(Image tl, Vector2 mousePos)
    {
        bool hit = false;

        //calculate bounding-box related to the timeline-pos
        //Vector2 tlPos = new Vector2(tl.transform.position.x, tl.transform.position.y);
        Vector2 colSize = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);
        Debug.Log("mouse: " + Input.mousePosition.y + ", posY" + tl.transform.position.y + ", col Height: " + transform.position.y+railWidth );
        //if mouse hits the timeline while dragging an object
        if (mousePos.x <= maxX && mousePos.x > minX &&
        ((mousePos.y <= (tl.transform.position.y + (colSize.y / 2.0f))) && (mousePos.y > (tl.transform.position.y - (colSize.y / 2.0f)))))
        {
            Debug.Log("object hits timeline!");
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
            //Debug.Log("object size: "+objects[i].GetComponent<RectTransform>().sizeDelta);
            //if timeline open scale ALL objects up
            if (timelineOpen)
            {
                scaleObject(objects[i], length, heightOpened);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightOpened);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightOpened);
                objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -40.0f);
                //objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,50.0f);
                //new Vector2(animationLength,scaleYUp);
            }
            else if ((timelineOpen == false) && (editObjOnTl == false))
            {
                //otherwise scale ALL objects down
                //Debug.Log("method: scale object down:");
                scaleObject(objects[i], length, heightClosed);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightClosed);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightClosed);
                objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -10.0f);
                //objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,10.0f);
            }
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
    public void setObjectOnTimeline(GameObject fig, float x, float y)
    {
        fig.transform.position = new Vector3(fig.transform.position.x, y, 0.0f);
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
    public void createRectangle(GameObject obj, Vector2 size, Color col, double railMinX, double animLength) // start pos brauch ich eigentlich nicht
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
    public int countCopiesOfObject(GameObject fig)
    {
        int c = 0;
        for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
        {
            //count object with the same name as fig
            foreach (GameObject gO in gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects)
            {
                //if (gO.name==fig.name)
                if (gO.name.Contains(fig.name))
                {
                    c++;
                }
            }
        }
        return c;
    }
    public void removeObjectFromTimeline(GameObject obj, GameObject obj3D)
    {
        int tmpNr = int.Parse(obj.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text);
        Destroy(obj);
        Destroy(obj3D);
        timelineInstanceObjects.Remove(obj);
        timelineInstanceObjects3D.Remove(obj3D);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
    }
    public void removeObjectFromTimeline2D(GameObject obj) // if Delete Button is pressed (button only knows 2D-Figure, 3D-Figure has to be calculated here)
    {
        int tmpNr = int.Parse(obj.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text);
        int val = 0;
        for (int i = 0; i < timelineInstanceObjects.Count; i++)
        {
            if (obj.name == timelineInstanceObjects[i].name)
            {
                val = i;
            }
        }
        Destroy(obj);
        Destroy(timelineInstanceObjects3D[val]);
        timelineInstanceObjects.Remove(obj);
        timelineInstanceObjects3D.Remove(timelineInstanceObjects3D[val]);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();

        //erase from SceneData (passiert offenbar schon automatisch, weil bei ButtonUp immer wieder neu die Instances von timelineInstances geschrieben werden :) ) 
        // StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6,2))-1].figureInstanceElements.Remove(StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6,2))-1].figureInstanceElements[Int32.Parse(obj.name.Substring(17))]);
    }

    public int checkHittingAnyTimeline(GameObject obj, Vector2 mousePos)
    {
        int hit = -1;
        Image[] tl = new Image[gameController.GetComponent<UIController>().Rails.Length];
        for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
        {
            tl[i] = gameController.GetComponent<UIController>().Rails[i].GetComponent<Image>();
            Debug.Log("timeline: " + tl[i]);
            Vector2 tlPos = new Vector2(tl[i].transform.position.x, tl[i].transform.position.y);
            Vector2 colSize = new Vector2(tl[i].GetComponent<BoxCollider2D>().size.x, tl[i].GetComponent<BoxCollider2D>().size.y);
            //if mouse hits the timeline while dragging an object
            //my implementation of object-boundingbox
            if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
            ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
            {
                Debug.Log("object hits timeline!: " + tl[i]);
                hit = i;
            }
        }
        //Debug.Log("drag and hit " + hit);
        return hit;
    }

    public void highlight(GameObject obj3D, GameObject obj)
    {
        obj.transform.GetChild(0).GetComponent<Image>().color = colFigureHighlighted;
        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);   //show Delete-Button
        SceneManaging.highlighted = true;

        if (obj3D.GetComponent<FigureStats>().isShip)
        {
            for (int i = 0; i < obj3D.GetComponent<MeshRenderer>().materials.Length; i++)
            {
                obj3D.GetComponent<MeshRenderer>().materials[i].EnableKeyword("_EMISSION");
            }
        }
        else
        {
            for (int i = 0; i < obj3D.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
            {
                obj3D.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[i].EnableKeyword("_EMISSION");
            }
        }
    }
    public void unhighlight(GameObject obj3D, GameObject obj)
    {
        obj.transform.GetChild(0).GetComponent<Image>().color = colFigure;
        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);  //hide Delete-Button
        SceneManaging.highlighted = false;

        if (obj3D.GetComponent<FigureStats>().isShip)
        {
            for (int i = 0; i < obj3D.GetComponent<MeshRenderer>().materials.Length; i++)
            {
                obj3D.GetComponent<MeshRenderer>().materials[i].DisableKeyword("_EMISSION");
            }
        }
        else
        {
            for (int i = 0; i < obj3D.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
            {
                obj3D.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[i].DisableKeyword("_EMISSION");
            }
        }
    }

    public GameObject CreateNew2DInstance(int figureNr, float momentOrPosX, bool loadFromFile)
    {
        int countName = 0;
        countName = countCopiesOfObject(figureObjects[figureNr]);
        newCopyOfFigure = Instantiate(figureObjects[figureNr]);
        newCopyOfFigure.name = figureObjects[figureNr].name + "instance" + countName.ToString("000");
        //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
        updateObjectList(timelineInstanceObjects, newCopyOfFigure);
        figCounterCircle[figureNr].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();
        //parent and position
        newCopyOfFigure.transform.SetParent(gameObject.transform);
        if (loadFromFile)
        {
            float posX = UtilitiesTm.FloatRemap(momentOrPosX, 0, AnimationTimer.GetMaxTime(), gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2);
            Debug.LogWarning("moment " + momentOrPosX + " // posX " + posX);

            newCopyOfFigure.transform.localPosition = new Vector2(posX, figureObjects[figureNr].transform.localPosition.y);
            // openCloseObjectInTimeline(true,timelineInstanceObjects,false); 
            //openCloseTimelineByClick(true, timelineImage,false);
            createRectangle(newCopyOfFigure, new Vector2(300, 80), colFigure, minX, 100.0f);
            scaleObject(newCopyOfFigure, 100, 80);
            scaleObject(newCopyOfFigure.transform.GetChild(0).gameObject, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, 80);
            newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector2(newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.x + 25, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.y);
            scaleObject(newCopyOfFigure.transform.GetChild(1).gameObject, 100, 80);
            gameController.GetComponent<SceneDataController>().objects2dFigureInstances.Add(newCopyOfFigure);
            newCopyOfFigure.transform.localScale = Vector3.one;
            //newCopyOfFigure.transform.parent.GetChild(0).GetComponent<RectTransform>().position = new Vector2(newCopyOfFigure.transform.parent.GetChild(0).GetComponent<RectTransform>().position.x + 25, newCopyOfFigure.transform.parent.GetChild(0).GetComponent<RectTransform>().position.y);
            openCloseTimelineByClick(true, timelineImage, false);

        }
        else
        {
            newCopyOfFigure.transform.position = new Vector2(momentOrPosX, figureObjects[figureNr].transform.position.y);
            newCopyOfFigure.transform.localScale = Vector3.one;
            createRectangle(newCopyOfFigure, new Vector2(300, 80), colFigure, minX, 100.0f);
        }
        //set 3d object to default position
        GameObject curr3DObject = new GameObject();
        curr3DObject = Instantiate(figureObjects3D[figureNr]);
        timelineInstanceObjects3D.Add(curr3DObject);
        setParent(timelineInstanceObjects3D[timelineInstanceObjects3D.Count - 1], rail3dObj.transform.GetChild(0).gameObject);
        if (newCopyOfFigure.GetComponent<RectTransform>().position.x < (minX + 50))  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
        {
            newCopyOfFigure.GetComponent<RectTransform>().position = new Vector2((float)minX + 50, GetComponent<RectTransform>().position.y);
        }
        //Debug.Log("++++++++++++ 3D Object: " + curr3DObject.name + ", rail3d: " + rail3dObj + ", Pos: " + curr3DObject.transform.position + ", Instances of this obj: " + countName);//= countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects););
        return curr3DObject;
    }
    public void ResetScreenSize()
    {
        Debug.Log("lossyScale: "+gameObject.transform.lossyScale);
        //Debug.Log("Screen changed! ScreenX: " + Screen.width);

        minX = 0.146f * Screen.width;// / gameObject.transform.lossyScale.x; //301.0f;  //timeline-minX
        Debug.Log("minX: "+minX);
        railWidth = 0.69f * Screen.width / gameObject.transform.lossyScale.x;
        heightClosed = 0.023f * Screen.height / gameObject.transform.lossyScale.x;
        heightOpened = 0.074f * Screen.height / gameObject.transform.lossyScale.x;
        maxX = minX + railWidth;  //timeline-maxX
        //Debug.Log("rail start: " + minX);
        
        if (isTimelineOpen)
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth, heightOpened);
        }
        else
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth, heightClosed);
        }
    }
    void Update()
    {
        if (currentLossyScale != transform.lossyScale.x)
        {
            currentLossyScale = transform.lossyScale.x;
            Debug.Log("scale after: "+transform.lossyScale.x);
            ResetScreenSize();
        }
        Vector2 getMousePos = Input.mousePosition;
        // Debug.Log("mousePos: " + getMousePos);
        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //identify which gameobject you clicked
            string objectClicked = identifyClickedObject();         //method fills up the current clicked index
                                                                    //Debug.Log("you clicked object: " + objectClicked);
                                                                    //Debug.Log("you clicked timelineobject: " + identifyClickedObjectByList(timelineInstanceObjects) + ", Maus: " + Physics2D.OverlapPoint(getMousePos));
            identifyClickedObjectByList(timelineInstanceObjects);
            editTimelineObject = false;                             //flag to prevent closing the timeline if you click an object in timeline
            releaseOnTimeline = false;                              //because you have not set on timeline anything
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
                openCloseTimelineByClick(isTimelineOpen, timelineImage, editTimelineObject);
                //draggingOnTimeline = true;
            }

            //if you click on an object in shelf 
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos) && getMousePos.y >= 500.0f) // necessary, so that figures below the figure shelf are not clickable anymore
                {
                    draggingObject = true;
                }
            }
            //or check if you click an object in timeline
            if ((currentClickedInstanceObjectIndex != (-1)) && (editTimelineObject == true) && isTimelineOpen)
            {
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    draggingObject = true;
                    draggingOnTimeline = true;

                    //highlighting objects and showing delete button when clicked
                    if (SceneManaging.highlighted == false)
                    {
                        highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                        //timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects3D[currentClickedInstanceObjectIndex]));//, currentClickedObjectIndex));
                    }
                    else if (SceneManaging.highlighted && timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<Image>().color == colFigureHighlighted) // checkFigureHighlighted(timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject))  // check if second child (which is never the armature) has emission enabled (=is highlighted)
                    {
                        unhighlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                    }
                    else
                    {
                        for (int j = 0; j < gameController.GetComponent<UIController>().Rails.Length; j++)
                        {
                            for (int i = 0; i < gameController.GetComponent<UIController>().Rails[j].timelineInstanceObjects3D.Count; i++)
                            {
                                unhighlight(gameController.GetComponent<UIController>().Rails[j].timelineInstanceObjects3D[i], gameController.GetComponent<UIController>().Rails[j].timelineInstanceObjects[i]);
                            }
                            highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            // timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects3D[currentClickedInstanceObjectIndex]));//, currentClickedObjectIndex));
                        }
                    }

                }
            }

            //if you hit/clicked nothing with mouse
            if (Physics2D.OverlapPoint(getMousePos) == false)
            {
                // delete highlight of everything if nothing is clicked
                for (int i = 0; i < timelineInstanceObjects3D.Count; i++)
                {
                    unhighlight(timelineInstanceObjects3D[i], timelineInstanceObjects[i]);
                }
            }
        }

        // if timeline is open and something is being dragged
        if (draggingOnTimeline && editTimelineObject && isTimelineOpen)
        {
            //Debug.Log("+++++++++++++++++++++++++dragging on timeline......");
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                isInstance = true;
                //if you click an object in timeline (for dragging)
                //move object
                updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos); // hier muss die maus position minus pivot point genommen werden! oder so (s. dragdrop, da funktioniert das schon)
                                                                                                               //snapping/lock y-axis
                setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, this.transform.position.y);
                Debug.Log("pos X: " + timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().position.x);
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().position.x < (minX + 50))  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
                {
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().position = new Vector2((float)minX + 50, GetComponent<RectTransform>().position.y);    // tendenziell muesste das eher in buttonUp
                }

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
            int hitTimeline;
            hitTimeline = checkHittingAnyTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos);

            // if (hitTimeline != -1)  // if a timeline is hit
            // {
            //     // if (hitTimeline != ((int)Char.GetNumericValue(timelineImage.name[17]))) // if hit timeline does not equal the old one
            //     // {
            //     Debug.Log("hit timeline: "+hitTimeline);
            //     //open timeline, if its not open
            //     openCloseTimelineByDrag("open", gameController.GetComponent<UIController>().Rails[hitTimeline].GetComponent<Image>());

            //     //scale up object/figures in timeline
            //     openCloseObjectInTimeline(true, gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects, editTimelineObject);
            //     //and set animation-length
            //     float animationLength = 100.0f;     //length of objects animation
            //     float scaleY = 80.0f;               //size if the timeline is maximized

            //     // change rail of object!! erst im buttonUp

            //     //scale down the dragged figure (and childobject: image)
            //     scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex], animationLength, scaleY);
            //     scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject, animationLength, scaleY);
            //     scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject, animationLength, scaleY);
            //     timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(true);

            //     // change parent back
            //     setParent(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[currentClickedInstanceObjectIndex], gameObject);

            //     //snapping/lock y-axis
            //     setObjectOnTimeline(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, gameController.GetComponent<UIController>().Rails[hitTimeline].transform.position.y);

            //     //set placed figures to timelineobjects-list of remote rail
            //     updateObjectList(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineObjects, gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            //     // erase from old list (this)
            //     Debug.Log("added obj " + gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[currentClickedInstanceObjectIndex] + " to remote timeline: " + gameController.GetComponent<UIController>().Rails[hitTimeline].timelineObjects);
            //     updateObjectList(timelineObjects, timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            //     Debug.Log("removed obj " + timelineInstanceObjects[currentClickedInstanceObjectIndex] + " from this timeline: " + timelineObjects);

            //     //SceneManaging.objectsTimeline = SceneManaging.objectsTimeline; // -1?
            //     // }
            //     /*else if (hitTimeline == ((int)Char.GetNumericValue(timelineImage.name[17])))    // if hit timeline equals the old one
            //     {
            //         Debug.Log("hit timeline equals the old one: "+hitTimeline);
            //         //open timeline, if its not open
            //         openCloseTimelineByDrag("open", timelineImage);

            //         //scale up object/figures in timeline
            //         openCloseObjectInTimeline(SceneManaging.anyTimelineOpen, timelineInstanceObjects, editTimelineObject);
            //         //and set animation-length
            //         float animationLength = 100.0f;     //length of objects animation
            //         float scaleY = 80.0f;               //size if the timeline is maximized

            //         //scale down the dragged figure (and childobject: image)
            //         scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex], animationLength, scaleY);
            //         scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject, animationLength, scaleY);
            //         scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject, animationLength, scaleY);
            //         timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(true);

            //         // change parent back
            //         setParent(timelineInstanceObjects[currentClickedInstanceObjectIndex], gameObject);

            //         //snapping/lock y-axis
            //         setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, this.transform.position.y);

            //         //set placed figures to timelineobjects-list
            //         updateObjectList(timelineObjects, timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            //         // erase from old list

            //         //save position, where object on timeline is released + set flag 
            //         releaseOnTimeline = true;
            //         releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            //         //Debug.Log("release object at pos: " + getMousePos + " " + Physics2D.OverlapPoint(getMousePos));

            //         //set up flags*/
            //     // }

            //     //save position, where object on timeline is released + set flag 
            //     releaseOnTimeline = true;
            //     releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            //     //Debug.Log("release object at pos: " + getMousePos + " " + Physics2D.OverlapPoint(getMousePos));

            //     //set up flags
            // }

            // else // if no timeline is hit
            // {
            //Debug.Log("nothign hit");
            // //scale up the dragged figure and childobjects (button and rect)
            // scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex], 150, 150);
            timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(false);
            // scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject, 150, 150);
            //             //temporarily change parent, so that object appears in front of the shelf
            // timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.SetParent(parentMenue.transform);
            // //moving on mouse pos
            updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos);
            // Debug.Log("obj pos: "+timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position+", obj: "+timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            // //openCloseTimelineByDrag("close", timelineImage);

            // //make 3d object invisible for time of dragging
            // timelineInstanceObjects3D[currentClickedInstanceObjectIndex].SetActive(false);
            // //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            // openCloseTimelineByDrag("close", timelineImage);
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
            //string nameOfHitObject="";
            hitTimeline = checkHittingTimeline(timelineImage, getMousePos);

            int tmp = 0;
            if (transform.name.Contains("1"))
            {
                tmp = 1;
            }
            if (transform.name.Contains("2"))
            {
                tmp = 2;
            }
            if (transform.name.Contains("3"))
            {
                tmp = 3;
            }
            if (transform.name.Contains("4"))
            {
                tmp = 4;
            }
            if (transform.name.Contains("5"))
            {
                tmp = 5;
            }
            if (transform.name.Contains("6"))
            {
                tmp = 6;
            }

            if (hitTimeline)
            {
                //Debug.Log("X-Pos: " + figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().position.x + ", minX+50: " + (minX + 50));

                SceneManaging.timelineHit = tmp;
                openCloseTimelineByDrag("open", timelineImage);
                //scale up object/figures in timeline
                //openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
                //and set animation-length
                float animationLength = 100.0f;     //length of objects animation
                                                    //figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(animationLength,scaleYUp);
                                                    //scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleYUp);
                                                    //scale down the dragged figure (and childobject: image)
                scaleObject(figureObjects[currentClickedObjectIndex], animationLength, heightOpened);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, animationLength, heightOpened);

                // change parent back
                setParent(figureObjects[currentClickedObjectIndex], gameObject);

                //snapping/lock y-axis
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], figureObjects[currentClickedObjectIndex].transform.position.x, this.transform.position.y);

                //set placed figures to timelineobjects-list
                //updateObjectList(timelineObjects, figureObjects[currentClickedObjectIndex]);

                //save position, where object on timeline is released + set flag 
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);

                //animation of 3d object not during dragging object on timeline ->outside of dragging
            }
            else
            {
                if (SceneManaging.timelineHit == tmp)
                {
                    scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);
                }
                //close timeline if you click e.g. in the shelf to get a new figure
                openCloseTimelineByDrag("close", timelineImage);
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
                    GameObject curr3DObject;
                    //create a copy of this timelineObject and keep the original one
                    curr3DObject = CreateNew2DInstance(currentClickedObjectIndex, getMousePos.x, false);

                    //set original image back to shelf, position 2 to make it visible
                    setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                    scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);

                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Save to SceneData:
                    FigureInstanceElement thisFigureInstanceElement = new FigureInstanceElement();
                    thisFigureInstanceElement.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex]); //index
                    thisFigureInstanceElement.name = curr3DObject.name + "_" + countCopiesOfObject(figureObjects[currentClickedObjectIndex]).ToString("000");
                    thisFigureInstanceElement.railStart = (int)Char.GetNumericValue(timelineImage.name[17]) - 1; //railIndex
                    StaticSceneData.StaticData.figureElements[currentClickedObjectIndex].figureInstanceElements.Add(thisFigureInstanceElement);
                    gameController.GetComponent<SceneDataController>().objects3dFigureInstances.Add(curr3DObject);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////

                    currentClickedObjectIndex = -1; // set index back to -1 because nothing is being clicked anymore
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
                Debug.Log("remove");
                removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
            }

            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], 100.0f, maxTimeInSec, minX, maxX);

                float moment = UtilitiesTm.FloatRemap(timelineInstanceObjects[i].transform.localPosition.x, gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2, 0, AnimationTimer.GetMaxTime());

                timelineInstanceObjects3D[i].transform.localPosition = new Vector3(timelineInstanceObjects3D[i].transform.localPosition.x, 0, (rail3dObj.transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime((float)startSec)) / 10);
                //StaticSceneData.StaticData.figureElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(17))].moment = (float)startSec;
                StaticSceneData.StaticData.figureElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(17))].moment = moment;
                // Debug.Log("FigureInstance: " + StaticSceneData.StaticData.figureElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(17))].name);
                // Debug.Log("Moment: " + StaticSceneData.StaticData.figureElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(17))].moment);
                // Debug.Log("30 cm in 3D in Timeline: "+7.32f);
                // Debug.Log("rail: " + StaticSceneData.StaticData.figureElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(17))].railStart);
                // Debug.Log("isntanceNr: " + StaticSceneData.StaticData.figureElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(timelineInstanceObjects[i].name.Substring(17))].instanceNr);
            }

            releaseOnTimeline = false;
            releaseObjMousePos.x = 0.0f;
            releaseObjMousePos.y = 0.0f;
            draggingOnTimeline = false;
            isInstance = false;
        }

        for (int i = 0; i < timelineInstanceObjects3D.Count; i++)
        {
            // start Animation on play
            if (timelineInstanceObjects3D[i].GetComponent<FigureStats>().isShip == false)
            {
                if (SceneManaging.playing && !timelineInstanceObjects3D[i].GetComponent<Animator>().enabled)
                {
                    timelineInstanceObjects3D[i].GetComponent<Animator>().enabled = true;
                }
                else if (!SceneManaging.playing && timelineInstanceObjects3D[i].GetComponent<Animator>().enabled)
                {
                    timelineInstanceObjects3D[i].GetComponent<Animator>().enabled = false;
                }
            }
        }
    }
}
