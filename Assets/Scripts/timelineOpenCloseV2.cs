using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timelineOpenCloseV2 : MonoBehaviour
{
    Image timelineImage;
    Outline outline;
    AudioSource audioSource;
    public GameObject gameController;
    public Text timelineText;
    public Image timeSliderImage;
    private BoxCollider2D timeSlider;
    public GameObject rail3dObj;
    Vector2 textSize;
    //bool timelineOpen;			//timeline open/close
    //bool clickingTimeline;          //if timeline is clicked
    //bool movingOnTimeline;          //is the mouse over the timeline
    bool draggingOnTimeline;        //dragging the mouse over the timeline
    //bool highlighted;               //global variable since only one object can be highlighted - everything else is unhighlighted
    //bool clickingObject;            //if you only click the object
    //bool movingObject;              //is the mouse over the object
    bool draggingObject;            //dragging an object with mouse
    //bool objectOnTimeline;          //is object on timeline
    bool doSomethingOnce;           //flag for do something once in a loop
    bool editTimelineObject;        //flag for shifting/clicking an object on timeline
    bool releaseOnTimeline;         //flag if you set an object on timeline
    bool playingMusic;
    bool isInstance;
    public bool isTimelineOpen;
    Vector2 releaseObjMousePos;
    double minX;
    double maxX;
    double minY;
    double maxY;
    string maxTimeLength;
    int maxTimeInSec;
    int currentClip;
    Vector2 sizeDeltaAsFactor;
    //public Button myButton;
    //Vector2 objectDefaultPosition;
    Vector2[] objectShelfPosition;
    Vector2[] objectShelfSize;
    GameObject[] objectShelfParent;
    //temp gameobject
    GameObject newCopyOfFigure;
    //Vector2 objectDefaultSize;
    Vector2 objectSceneSize;

    public GameObject objectLibrary;
    public GameObject parentMenue; // mainMenue
    GameObject timeSettings;
    GameObject[] figCounterCircle;

    GameObject[] figureObjects;
    GameObject[] figureObjects3D;
    //public GameObject[] figure3D;
    int currentClickedObjectIndex;
    int currentClickedInstanceObjectIndex;

    public List<GameObject> timelineObjects;
    public List<GameObject> timelineInstanceObjects;
    public List<GameObject> timelineObjects3D;
    public List<GameObject> timelineInstanceObjects3D;
    private FigureElement ThisFigureElement;    //element to set 3d object

    double fig1StartPos;
    double fig2StartPos;
    Vector3 railStartPos;
    Vector3 railEndPos;
    Color colFigure = new Color(0.06f, 0.66f, .74f, 0.5f);
    Color colFigureHighlighted = new Color(0f, 0.87f, 1.0f, 0.5f);


    public AudioClip[] clip;         // ought to be 6 audioclips

    // Start is called before the first frame update
    void Awake()
    {
        timelineImage = this.GetComponent<Image>();
        //timeSliderImage=GetComponent<Image>();
        //timeSlider=timeSliderImage.GetComponent<BoxCollider2D>();
        SceneManaging.anyTimelineOpen = false;
        //clickingTimeline = false;
        //movingOnTimeline = false;
        draggingOnTimeline = false;
        //highlighted = false;
        //clickingObject = false;
        //movingObject = false;
        draggingObject = false;
        //objectOnTimeline = false;
        doSomethingOnce = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        playingMusic = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        currentClip = 0;
        isTimelineOpen = false;
        isInstance = false;


        /*
		objectDefaultPosition=myButton.transform.position;
		objectDefaultSize.x=myButton.GetComponent<RectTransform>().rect.width;
		objectDefaultSize.y=myButton.GetComponent<RectTransform>().rect.height;
		objectSceneSize.x=myButton.GetComponent<RectTransform>().rect.width;
		objectSceneSize.y=myButton.GetComponent<RectTransform>().rect.height;
		*/

        textSize = new Vector2(timelineText.GetComponent<RectTransform>().sizeDelta.x, timelineText.GetComponent<RectTransform>().sizeDelta.y);

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
            //Debug.Log("one figure: "+figureObjects[0]);					//returns the empty
            //Debug.Log("one figure: "+figureObjects[0].GetChild(1));		//returns the childs of this empty e.g. the button

            //store default position
            //this creates new vectr2-objects (posistion and size of the elements in shelf)
            //f3dobjectShelfPosition[i] = figureObjects[i].GetComponent<RectTransform>().anchoredPosition;
            objectShelfSize[i] = new Vector2(figureObjects[i].GetComponent<RectTransform>().rect.width, figureObjects[i].GetComponent<RectTransform>().rect.height);
            //Debug.Log("defaultShelfPosition: " + objectShelfPosition[i] + " ++++++++++defaultShelfSize: " + objectShelfSize[i]);
            //store default parentName
            //this creates new game-objects named like the parent :(
            //objectShelfParent[i]=new GameObject(figureObjects[i].transform.parent.gameObject.name);
            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;

            //GameObject tmp=new GameObject();
            //tmp.name=figureObjects[i].transform.parent.gameObject.name;
            //objectShelfParent[i]=tmp;
            //Destroy(tmp);
            //make a true clone of gameobject
            //objectShelfParent[i]=Instantiate(figureObjects[i].transform.parent.gameObject);
            //Debug.Log("defaultShelfParent: "+objectShelfParent[i]);
        }

        foreach (GameObject gaOb in objectShelfParent)
            //Debug.Log("defaultShelfParentArray: " + gaOb);

            //create a new tempGameObject
            newCopyOfFigure = new GameObject();

        try
        {
            for (int i = 0; i < figureObjects3D.Length; i++)
            {
                figureObjects3D[i] = gameController.GetComponent<SceneDataController>().objectsFigureElements[i];
            }

        }
        catch (IndexOutOfRangeException ex)
        {

        }
        Debug.Log("++++++figures loaded: " + figureObjects.Length);
        Debug.Log("++++++figures3D loaded: " + figureObjects3D.Length);

        /*//example for the first element
		objectDefaultPosition=figureObjects[0].transform.position;
		objectDefaultSize.x=figureObjects[0].GetComponent<RectTransform>().rect.width;
		objectDefaultSize.y=figureObjects[0].GetComponent<RectTransform>().rect.height;
		objectSceneSize.x=figureObjects[0].GetComponent<RectTransform>().rect.width;
		objectSceneSize.y=figureObjects[0].GetComponent<RectTransform>().rect.height;
		//Debug.Log("object width,height: "+objectDefaultSize);*/
        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;

        //change parent to root
        //figureObjects[0].GetChild(1).transform.SetParent(this.transform);	//parent to timeline
        //this is the root-parent
        //mainMenue = GameObject.Find("timelineArea");
        //parentMenue = GameObject.Find("MenueDirectorMain");
        timeSettings = timeSliderImage.transform.GetChild(0).gameObject; // GameObject.Find("ImageTimeSettingsArea");

    }

    void Start()
    {
        scaleObject(gameObject, 1335, 15);//openCloseTimelineByClick(true,timelineImage,false);
        audioSource = GetComponent<AudioSource>();
        timeSettings.SetActive(false);
        //outline.enabled = false;
        //figCounterCircle[0] = figureObjects[0].transform.parent.GetChild(2).gameObject;
        //figCounterCircle[0] = GameObject.Find("ImageCountFigObj01");
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
            figCounterCircle[i].transform.GetChild(0).GetComponent<Text>().text = "0";
        }

        //get figure element
        //gameController = GameObject.Find("GameController");
        //Debug.Log(">>>figure elements existing: "+StaticSceneData.StaticData.figureElements);
        //ThisFigureElement = StaticSceneData.StaticData.figureElements.Find(x => x.name == gameObject.name.Substring(6));
        //Debug.Log(">>>figure element: "+ThisFigureElement);
        //Debug.Log(">>>figure elements existing: "+figure1.name);

        //rail1StartPos=new Vector3(0.0f,-0.009f,-2.0f);
        //rail1EndPos=new Vector3(0.0f,-0.009f,2.0f);
        /*rail2StartPos=new Vector3(-0.119f,0.146f,-2.0f);*/
        //rail3StartPos=new Vector3(-0.319f,0.146f,-2.0f);
        /*rail4StartPos=new Vector3(-0.439f,0.146f,-2.0f);
		rail5StartPos=new Vector3(-0.642f,0.146f,-2.0f);
		rail6StartPos=new Vector3(-0.759f,0.146f,-2.0f);*/
        //Debug.Log("start... "+rail1StartPos);
        //Debug.Log("start... "+rail1EndPos);
        //list of objects in the timeline
        //create an empty list
        List<GameObject> timelineObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects = new List<GameObject>();
        List<GameObject> timelineObjects3D = new List<GameObject>();
        List<GameObject> timelineInstanceObjects3D = new List<GameObject>();
        //Debug.Log("railInfo: "+ rail3dObj + "Pos: "+rail3dObj.transform.position); //bsp:'schiene1' and '(0.5,-0.5,0.0)' y=up-vector
        railStartPos = new Vector3(0.0f, 0.0f, 0.0f);
        railEndPos = new Vector3(0.0f, 0.0f, 0.0f);
        railStartPos = getRailStartEndpoint(rail3dObj, "start");
        railEndPos = getRailStartEndpoint(rail3dObj, "end");
        //Debug.Log("railStartPos: " + railStartPos + "railEndPos: " + railEndPos);

        fig1StartPos = railStartPos.z;//-1.75f;
        fig2StartPos = railEndPos.z;//1.88f;

        minX = 301.0f;  //timeline-minX
        maxX = 1623.0f; //timeline-maxX
        minY = 428.0f;  //timeline-minY
        maxY = 438.0f;  //timeline-maxY
        maxTimeLength = "10:14";
        maxTimeInSec = 614;
        sizeDeltaAsFactor = new Vector2(1.0f, 1.0f);
        //Debug.Log("++++++timeline calling");

        // disable animations and disable emission
        if (gameObject.name != "ImageTimelineRailMusic")
        {
            for (int i = 0; i < figureObjects3D.Length; i++)
            {
                for (int j = 0; j < figureObjects3D[i].transform.childCount; j++)
                {
                    try
                    {
                        figureObjects3D[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_EMISSION");
                    }
                    catch (MissingComponentException ex) { }
                }
                try
                {
                    figureObjects3D[i].GetComponent<Animator>().enabled = false;
                }
                catch (MissingComponentException ex) { }


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
        float heightOpened = 80.0f;
        float heightClosed = 15.0f;    //this size is set in editor

        if (isAnyTimelineOpen() == false)
        {
            //Debug.Log("++++ es ist keine schiene geöffnet, deswegen wird geklickte Schiene geöffnet: " + tl);
            //set global flag
            SceneManaging.anyTimelineOpen = true;
            isTimelineOpen = true;
            //scale up timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
            //scale down the text
            //tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 0.2f, 1);
            //timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
            //minimize or maximize objects on timeline
            openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
            if (gameObject.name == "ImageTimelineRailMusic")
            {
                ImageTimelineSelection.SetRailNumber(6);
                ImageTimelineSelection.SetRailType(2);  // for rail-rails
            }
            else
            {
                ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(timelineImage.name[17]) - 1);
                ImageTimelineSelection.SetRailType(0);  // for rail-rails
            }

        }
        else if (isAnyTimelineOpen() && editObjOnTl == false)
        {
            if (thisTimelineOpen)
            {
                //Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
                //close timeline
                isTimelineOpen = false;
                //scale down timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                //scale down the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                //scale up the text
                //tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 1, 1);
                openCloseObjectInTimeline(false, timelineInstanceObjects, editTimelineObject);
            }
            else
            {
                //Debug.Log("++++ geklickte Schiene ist zu, aber eine andere ist offen und wird geschlossen: " + tl);
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
                gameController.GetComponent<UIController>().RailLight.GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                gameController.GetComponent<UIController>().RailLight.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                gameController.GetComponent<UIController>().RailLight.isTimelineOpen = false;
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
                gameController.GetComponent<UIController>().RailMusic.isTimelineOpen = false;
                // open clicked rail
                //Debug.Log("++++ geklickte Schiene wird geöffnet: " + tl);
                //scale up timeline
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightOpened);
                //scale up the collider
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightOpened);
                openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
                isTimelineOpen = true;
                if (gameObject.name == "ImageTimelineRailMusic")
                {
                    ImageTimelineSelection.SetRailNumber(6);
                    ImageTimelineSelection.SetRailType(2);  // for rail-rails
                }
                else
                {
                    ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(timelineImage.name[17]) - 1);
                    ImageTimelineSelection.SetRailType(0);  // for rail-rails
                }
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
            tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 0.2f, 1);
            //timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
            //scale up all objects on timeline
            openCloseObjectInTimeline(isTimelineOpen, timelineInstanceObjects, editTimelineObject);
            if (gameObject.name == "ImageTimelineRailMusic")
            {
                ImageTimelineSelection.SetRailNumber(6);
                ImageTimelineSelection.SetRailType(2);  // for rail-rails
            }
            else
            {
                ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(timelineImage.name[17]) - 1);
                ImageTimelineSelection.SetRailType(0);  // for rail-rails
            }
        }
        else
        {
            //Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
            //if open=="close"
            isTimelineOpen = false;
            //scale down timeline
            tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed);
            //scale down the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed);
            //scale up the text
            tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 1, 1);
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
        if (gameController.GetComponent<UIController>().RailLight.isTimelineOpen == true) val = true;
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
        Debug.Log("mouse: "+mousePos);
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

            /*if (timelineOpen)
            {
                scaleObject(objects[i], length, scaleUp);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleUp);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleUp);
                //objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,50.0f);
                //new Vector2(animationLength,scaleYUp);

            }
            else if ((timelineOpen == false) && (editObjOnTl == false))
            {
                //otherwise scale ALL objects down
                //Debug.Log("method: scale object down:");
                scaleObject(objects[i], length, scaleDown);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleDown);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, scaleDown);
                //objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,10.0f);
            }*/

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
    /*public void set3DObject(int idx, GameObject[] obj3D)
    {
        //fig1StartPos=fig1StartPos+0.005f;
        //figure1.transform.position=new Vector3(rail1StartPos.x,figure1.transform.position.y,(float)fig1StartPos);		//start-pos on the left side in timeline
        obj3D[idx].transform.position = new Vector3(railStartPos.x, -0.3f, (float)fig1StartPos * (-1.0f));
        //fig2StartPos=fig2StartPos-0.005f;
        //figure2.transform.position=new Vector3(rail3StartPos.x,figure2.transform.position.y,(float)fig2StartPos);		//start-pos on the right side in timeline
    }
    public void set3DObjectOfInstance(GameObject obj3D)
    {
        obj3D.transform.position = new Vector3(0.0f, 0.0f, (float)fig1StartPos * (-1.0f));
    }*/
    public void animate3DObjectByTime(GameObject fig3D, int startSec, double animLength, double timerTime, double railStartZ, double railEndZ)
    {
        //Debug.Log("method: fig: " + fig3D + " 3dobj: " + figure1 + " startSec: " + startSec + " animLength: " + animLength + " timerTime: " + timerTime);
        //Debug.Log("fig3d props: " + fig3D.transform.position.x + ", rail X: " + rail3dObj.transform.position.x);
        double figPos = fig3D.transform.position.z;
        double figStartPos = railStartZ * (-1.0f);      //*-1.0 changes startpoint and animation-direction
        animLength = 44.0f;

        float tmpX = 0.0f;
        tmpX = float.Parse(rail3dObj.GetComponent<Text>().text);

        if ((timerTime >= startSec) && (timerTime <= (startSec + (int)animLength)))
        {
            //calculate the rail-length
            double railLength = (Mathf.Abs((float)railStartZ) + Mathf.Abs((float)railEndZ));    //range: -2 to +2 = length of 4
                                                                                                //calculate the relation of timer time to object time
                                                                                                //double percentageOfTime=(double)timerTime/((double)startSec+animLength);		//e.g. 66/(57+100) -> wrong
            double percentageOfTime = ((double)timerTime - (double)startSec) / animLength;      //e.g. 66-57=9 -> we are currently at 9/100
                                                                                                //calculate the railpos related to the time-percentage
                                                                                                //double railPos=railLength*percentageOfTime;									//0,41 at timer: 66 -> wrong
            double railPos = (railLength * percentageOfTime) * (-1.0f) + figStartPos;                       // 4=100, 9/100 from 4: 36/400
                                                                                                            //figPos=figPos-0.05f;
            figPos = railPos;
            //Debug.Log("if: rail-length: " + railLength + " figStartPos: " + figStartPos + " percentage: " + percentageOfTime + " railPos: " + railPos + " timerTime: " + timerTime);
            //Debug.Log("if: figPos: " + figPos);
            //fig3D.transform.position=new Vector3(rail3StartPos.x,fig3D.transform.position.y,(float)figPos);
            fig3D.transform.position = new Vector3(tmpX, rail3dObj.transform.position.y - 0.03f, (float)figPos);
        }
    }
    public bool isInList(List<GameObject> objects, GameObject obj)
    {
        return objects.Contains(obj);
    }
    public bool moveTimeSlider(Image myTL)
    {
        Vector2 mousePos = Input.mousePosition;
        //Debug.Log("timeslider mouse-pos: "+mousePos);
        //Debug.Log("timeslider-image "+myTL);
        //test if you move the slider or click elsewhere in the timelinearea
        //calculate bounding-box related to the timeline-pos
        Vector2 tlPos = new Vector2(myTL.transform.position.x, myTL.transform.position.y);
        Vector2 colSize = new Vector2(myTL.GetComponent<BoxCollider2D>().size.x, myTL.GetComponent<BoxCollider2D>().size.y);
        //Debug.Log("timeslider colBox: "+myTL.GetComponent<BoxCollider2D>().size);
        //Debug.Log("timeslider pos: "+myTL.transform.position);
        //if mouse hits the timeline while dragging an object
        //my implementation of object-boundingbox
        if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
        ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
        {
            Debug.Log("timeslider hit!");
            return true;
        }
        else
            return false;
    }
    public void createRectangle(GameObject obj, Vector2 size, Color col, double railMinX, double animLength) // start pos brauch ich eigentlich nicht
    {
        //int alpha=1;
        // Vector3 col = new Vector3(255, 120, 60);
        // Image rect = obj.AddComponent<Image>();              // an object can have only one Image-Component
        // rect.color = new Color(col.x, col.y, col.z);
        // rect.transform.position = startPos;          

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
        trans.sizeDelta = new Vector2(100.0f, 80.0f); // custom size


        Image image = imgObject.AddComponent<Image>();
        //Texture2D tex = Resources.Load<Texture2D>("red");
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

    public bool checkFigureObjectShouldPlaying(GameObject fig, int startTime, double animLength, double timerTime)
    {
        if ((timerTime >= startTime) && (timerTime <= (startTime + (animLength / 2.0))))    ///2.0 because animLength=Pixel -> animLength/2~time
        {
            return true;
        }
        else
            return false;
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
    public void removeObjectFromTimeline(GameObject obj, GameObject obj3D)
    {
        int tmpNr = int.Parse(obj.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text);
        Destroy(obj);
        Destroy(obj3D);
        timelineInstanceObjects.Remove(obj);
        timelineInstanceObjects3D.Remove(obj3D);
        //Debug.Log("tmpNr: " + tmpNr + "currentCounterNr: " + currentCounterNr + ", Instances: " + timelineInstanceObjects.Count);
        Debug.Log("+++Removed: " + obj);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
    }
    public void unhighlight(GameObject obj3D, GameObject obj)
    {
        //obj3D.GetComponent<Outline>().enabled = false;
        for (int i = 0; i < obj3D.transform.childCount; i++)
        {
            //Debug.Log("Child: "+obj3D.transform.GetChild(i));
            try
            {
                obj3D.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_EMISSION");
                obj3D.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.SetColor("_Emission_Color", Color.black);
                obj.transform.GetChild(0).GetComponent<Image>().color = colFigure;
            }
            catch (MissingComponentException ex)
            {

            }

        }
        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);  //hide Delete-Button
        SceneManaging.highlighted = false;
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
        Debug.Log("drag and hit " + hit);
        return hit;
    }

    public void highlight(GameObject obj3D, GameObject obj)
    {
        for (int i = 0; i < obj3D.transform.childCount; i++)
        {
            //Debug.Log("Child: "+obj3D.transform.GetChild(i));
            try
            {
                obj3D.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_EMISSION");
                obj3D.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.SetColor("_Emission_Color", Color.black);
                obj.transform.GetChild(0).GetComponent<Image>().color = colFigureHighlighted;
            }
            catch (MissingComponentException ex)
            {

            }

        }
        obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);   //show Delete-Button
        SceneManaging.highlighted = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("draggingOnTimeline: " + draggingOnTimeline + ", editTimelineObj: " + editTimelineObject + ", draggingObj: " + draggingObject + ", isInstance: " + isInstance);
        Vector2 getMousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //Debug.Log("---status---");
            //Debug.Log("timeline-instance-object count: " + timelineInstanceObjects.Count);
            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                //Debug.Log("timeline-instance-object nr: " + i + " name: " + timelineInstanceObjects[i].name);
            }
            //Debug.Log("---statusEnd---");

            //Debug.Log("click mouse button at pos: " + getMousePos + " " + Physics2D.OverlapPoint(getMousePos));
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
                /*Debug.Log("instance collider: "+timelineInstanceObjects[i].GetComponent<BoxCollider2D>());
                Debug.Log("instance mouse: "+getMousePos);
                Debug.Log("instance mouse: "+Physics2D.OverlapPoint(getMousePos));
                Debug.Log("instance mouse: "+Physics2D.OverlapPoint(Input.mousePosition));*/
                //if(timelineInstanceObjects[i].GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(Input.mousePosition))
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
            //Debug.Log("=====> found hit at index: " + tmpI);
            //Debug.Log("=====> instance-array: " + timelineInstanceObjects.Count);

            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline

                openCloseTimelineByClick(isTimelineOpen, timelineImage, editTimelineObject);
                draggingOnTimeline = true;
                //movingOnTimeline = true;        //drag on the timeline
                //clickingTimeline=true;	//may be the same as timelineOpen
            }

            //if you click on an object in shelf 
            //first checking if you have an object clicked in shelf
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    //clickingObject = true;    //for clicking
                    //movingObject = true;        //for dragging
                    draggingObject = true;

                    //save default values
                    //objectDefaultPosition=figureObjects[currentClickedObjectIndex].transform.position;
                    //objectDefaultSize.x=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.width;
                    //objectDefaultSize.y=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.height;
                }
            }
            //or check if you click an object in timeline
            if ((currentClickedInstanceObjectIndex != (-1)) && (editTimelineObject == true))
            {
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    //clickingObject = true;  //did you click the object
                    //movingObject = true;        //want to drag the object
                    //movingOnTimeline = true;    //access for moving an object on timeline
                    draggingObject = true;
                    draggingOnTimeline = true;

                    timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects3D[currentClickedInstanceObjectIndex]));//, currentClickedObjectIndex));

                    //highlighting objects and showing delete button when clicked
                    if (gameObject.name != "ImageTimelineRailMusic")
                    {
                        //Debug.Log("+++highlighted: " + SceneManaging.highlighted);
                        if (SceneManaging.highlighted == false)
                        {
                            highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                        }
                        else if (SceneManaging.highlighted && timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.IsKeywordEnabled("_EMISSION"))    // check if second child (which is never the armature) has emission enabled (=is highlighted)
                        {
                            unhighlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                        }
                        else
                        {
                            //Debug.Log("etwas ist gehighlited aber nicht das. ");
                            for (int j = 0; j < gameController.GetComponent<UIController>().Rails.Length; j++)
                            {
                                for (int i = 0; i < gameController.GetComponent<UIController>().Rails[j].timelineInstanceObjects3D.Count; i++)
                                {
                                    //Debug.Log("+++++++Schiene: " + gameController.GetComponent<UIController>().Rails[j] + ", obj: " + gameController.GetComponent<UIController>().Rails[j].timelineInstanceObjects3D[i]);
                                    unhighlight(gameController.GetComponent<UIController>().Rails[j].timelineInstanceObjects3D[i], gameController.GetComponent<UIController>().Rails[j].timelineInstanceObjects[i]);
                                }
                                highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            }
                        }
                    }
                }
            }

            //if you hit/clicked nothing with mouse
            if (Physics2D.OverlapPoint(getMousePos) == false)
            {
                //Debug.Log("----> scale down timeline objects: "+timelineOpen);
                //scale down figures on timeline
                //openCloseObjectInTimeline(timelineOpen, timelineObjects);
                //	openCloseObjectInTimeline(timelineOpen, timelineInstanceObjects);
                //Debug.Log("name of timeline-objects: "+timelineObjects[0]);
                //scale down timeline
                //-->openCloseTimelineByClick, see above

                // delete highlight of everything if nothing is clicked
                for (int i = 0; i < timelineInstanceObjects3D.Count; i++)
                {
                    unhighlight(timelineInstanceObjects3D[i], timelineInstanceObjects[i]);
                }
            }
            /*if (movingOnTimeline)
            {
                //space for dragging
                //Debug.Log("ready to drag something on the timeline...");
                draggingOnTimeline = true;
            }

            if (movingObject)
            {
                //Debug.Log("ready to drag an object from shelf...");
                draggingObject = true;
            }*/
        }

        // if timeline is open and something is being dragged
        if (draggingOnTimeline && editTimelineObject && isTimelineOpen)
        {
            //Debug.Log("+++++++++++++++++++++++++dragging on timeline......");
            //Debug.Log("--->before-if-index: "+currentClickedInstanceObjectIndex);
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                isInstance = true;
                //Debug.Log("--->index: "+currentClickedInstanceObjectIndex);
                //Debug.Log("index-name: "+timelineInstanceObjects[0].name);
                //Debug.Log("index-collider: "+timelineInstanceObjects[0].GetComponent<BoxCollider2D>());
                //Debug.Log("index-mousepos: "+Physics2D.OverlapPoint(getMousePos));

                //if you click an object in timeline (for dragging)

                //Debug.Log("moving timelineInstanceObjects: " + timelineInstanceObjects[currentClickedInstanceObjectIndex].name);
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

                    /* movingObject = true;
                    openCloseTimelineByDrag("close", timelineImage);*/
                }
            }
            releaseOnTimeline = true;
        }
        //if something has been dragged outside of the timeline
        if (draggingOnTimeline == false && editTimelineObject == false && draggingObject && isInstance)
        {
            // Debug.Log("+++something is outside the timeline: " + this.GetComponent<BoxCollider2D>() + ", Maus: " + Physics2D.OverlapPoint(getMousePos) + ", obj: " + timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            // //scale up the dragged figure (and childobject: image)
            // scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex], 150, 150);
            // timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(false);
            // scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject, 150, 150);
            // //snapping disable
            // updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos);

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
            Debug.Log("nothign hit");
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
        // }

        // dragging an object from shelf to timeline
        if (draggingObject && editTimelineObject == false && isInstance == false)
        {
            Debug.Log("dragging a shelf-selected object on timeline.");
            //move object
            updateObjectPosition(figureObjects[currentClickedObjectIndex], getMousePos);

            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            //updateObjectPosition(newCopyOfFigure,getMousePos);
            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            bool hitTimeline = false;
            //string nameOfHitObject="";
            hitTimeline = checkHittingTimeline(figureObjects[currentClickedObjectIndex], timelineImage, getMousePos);
            //hitTimeline=checkHittingTimeline(newCopyOfFigure, timelineImage, getMousePos);
            //Debug.Log("mouseclick: " + hitTimeline);

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
                SceneManaging.timelineHit = tmp;

                //open timeline, if its not open
                //Debug.Log(">>>flag timelineOpen: " + SceneManaging.timelineOpen);

                openCloseTimelineByDrag("open", timelineImage);

                //scale up object/figures in timeline
                openCloseObjectInTimeline(true, timelineInstanceObjects, editTimelineObject);
                //and set animation-length
                float animationLength = 100.0f;     //length of objects animation
                float scaleY = 80.0f;             //size if the timeline is maximized
                                                  //figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(animationLength,scaleYUp);
                                                  //scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleYUp);

                //set pivot so that is placed in front
                //setPivot(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, 0.0f, 0.5f);
                //scale down the dragged figure (and childobject: image)
                scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleY);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, animationLength, scaleY);


                //figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y);
                /*if (timelineObjects.Count > 0)      //if you dont check this, you get an out-of-range exception
                {
                    //Debug.Log("--->scale up!");
                    scaleObjectsOfList(timelineObjects, animationLength, scaleYUp);
                    for(int i=0;i<timelineObjects.Count;i++)
                    {
                        Debug.Log("Objekt scaled: "+timelineObjects[i]);
                    }
                }*/

                // change parent back
                setParent(figureObjects[currentClickedObjectIndex], gameObject);

                //snapping/lock y-axis
                //figureObjects[currentClickedObjectIndex].transform.position=new Vector3(figureObjects[currentClickedObjectIndex].transform.position.x, this.transform.position.y,0.0f);
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], figureObjects[currentClickedObjectIndex].transform.position.x, this.transform.position.y);
                //setObjectOnTimeline(newCopyOfFigure,newCopyOfFigure.transform.position.x,this.transform.position.y);

                //set placed figures to timelineobjects-list
                updateObjectList(timelineObjects, figureObjects[currentClickedObjectIndex]);
                //updateObjectList(timelineObjects,newCopyOfFigure);

                //createRectangle(figureObjects[currentClickedObjectIndex], new Vector2(50.0f,50.0f), new Vector2(50.0f,50.0f));

                //calculate the time the animation starts to run
                //double startSec=calculateFigureStartTimeInSec(figureObjects[currentClickedObjectIndex], 100.0f, maxTimeInSec, minX, maxX);

                //set 3d object to default position
                //set3DObject(currentClickedObjectIndex,figureObjects3D);
                //updateObjectList(timelineObjects3D,figureObjects3D[currentClickedObjectIndex]);

                //updateObjectList(timelineObjects3D,newCopyOfFigure);

                //save position, where object on timeline is released + set flag 
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
                //Debug.Log("release object at pos: " + getMousePos + " " + Physics2D.OverlapPoint(getMousePos));

                //animation of 3d object not during dragging object on timeline ->outside of dragging

                //set up flags
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
                /*for(int i = 0; i<gameController.GetComponent<UIController>().Rails.Length ; i++)
                {
                    if(gameController.GetComponent<UIController>().Rails[i].isTimelineOpen)
                    {
                        openCloseTimelineByDrag("close", gameController.GetComponent<UIController>().Rails[i].GetComponent<Image>());
                    } 
                }
                if(gameController.GetComponent<UIController>().RailMusic.isTimelineOpen) openCloseTimelineByDrag("close", gameController.GetComponent<UIController>().RailMusic.GetComponent<Image>());
                if(gameController.GetComponent<UIController>().RailLight.isTimelineOpen) openCloseTimelineByDrag("close", gameController.GetComponent<UIController>().RailLight.GetComponent<Image>());*/
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
            //clickingTimeline = false;
            //movingOnTimeline = false;

            //clickingObject = false;
            //movingObject = false;
            draggingObject = false;
            doSomethingOnce = false;
            editTimelineObject = false;
            //Destroy(newCopyOfFigure);


            if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)  //if release obj-pos and release mouse-button-pos is the same
                {
                    //Debug.Log("you released a figure on timeline");
                    //replace timeline-figure with an instance

                    //create a copy of this timelineObject and keep the original one
                    //newCopyOfFigure=figureObjects[currentClickedObjectIndex];	//wrong!
                    //we need a fully independend clone
                    newCopyOfFigure = Instantiate(figureObjects[currentClickedObjectIndex]);
                    //set properties of object
                    //count objects from same kind
                    int countName = 0;
                    //name
                    countName = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    newCopyOfFigure.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName;
                    if (gameObject.name == "ImageTimelineRailMusic")
                    {
                        createRectangle(newCopyOfFigure, new Vector2(300, 50), Color.green, minX, (double)int.Parse(newCopyOfFigure.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text));
                    }
                    else
                    {

                        createRectangle(newCopyOfFigure, new Vector2(300, 50), colFigure, minX, 50.0f);
                    }

                    figCounterCircle[currentClickedObjectIndex].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();

                    //parent
                    newCopyOfFigure.transform.SetParent(figureObjects[currentClickedObjectIndex].transform.parent.gameObject.transform);
                    //position
                    newCopyOfFigure.transform.position = new Vector2(figureObjects[currentClickedObjectIndex].transform.position.x, figureObjects[currentClickedObjectIndex].transform.position.y);
                    //set collider


                    //add object to list which objects are on timeline
                    //set placed figures to timelineInstanceObjects-list
                    updateObjectList(timelineInstanceObjects, newCopyOfFigure);

                    //count the instances
                    //	Debug.Log("number of instances on timeline: "+timelineInstanceObjects.Count);

                    //Debug.Log("copies of this object:"+countCopiesOfObject(newCopyOfFigure,timelineObjects));

                    //set default parent
                    setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                    Debug.Log("Set parent to: " + objectShelfParent);
                    //scale to default values
                    scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y);

                    //set back the original figure-image to shelf
                    //Debug.Log("default shelf-pos: "+objectShelfPosition[currentClickedObjectIndex]);
                    //Debug.Log("default shelf-empty: "+figureObjects[currentClickedObjectIndex].transform.parent.gameObject.name);
                    //Debug.Log("default shelf-empty-pos: "+figureObjects[currentClickedObjectIndex].transform.parent.gameObject.transform.position);

                    //get position of parent-empty-object
                    //Vector2 tmpVec=new Vector2(figureObjects[currentClickedObjectIndex].transform.parent.gameObject.transform.position.x,figureObjects[currentClickedObjectIndex].transform.parent.gameObject.transform.position.y);

                    //set this position
                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
                    // bring object back to position two, so that its visible
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);

                    //remove from timelineObjects-list
                    //	Debug.Log("count timelineObjects: "+timelineObjects.Count);
                    //	timelineObjects.Remove(figureObjects[currentClickedObjectIndex]);
                    //	Debug.Log("count timelineObjects after remove: "+timelineObjects.Count);

                    //	Debug.Log("count timelineInstancesObjects: "+timelineInstanceObjects.Count);

                    //set 3d object to default position
                    if (gameObject.name != "ImageTimelineRailMusic")
                    {
                        GameObject curr3DObject = new GameObject();
                        curr3DObject = Instantiate(figureObjects3D[currentClickedObjectIndex]);
                        timelineInstanceObjects3D.Add(curr3DObject);
                        Debug.Log("++++++++++++ PosX curr3DObj: " + curr3DObject.transform.position.x);
                        //set3DObject(currentClickedObjectIndex,figureObjects3D);
                        //set3DObjectOfInstance(timelineInstanceObjects3D[timelineInstanceObjects3D.Count - 1]); //the last added object
                        setParent(timelineInstanceObjects3D[timelineInstanceObjects3D.Count - 1], rail3dObj);
                    }


                    //Debug.Log("original shelf-pos: "+objectShelfPosition[currentClickedObjectIndex]);
                    /*
                        figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                        //Debug.Log("original parent: "+objectShelfParent[currentClickedObjectIndex]+" count: "+objectShelfParent.Length);				

                        //objectShelfPosition[i]=new Vector2(figureObjects[i].transform.position.x,figureObjects[i].transform.position.y);
                        */

                    //show the instances as a pinned number

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
                removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex],timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
            }

            releaseOnTimeline = false;
            releaseObjMousePos.x = 0.0f;
            releaseObjMousePos.y = 0.0f;
            draggingOnTimeline = false;
            isInstance = false;
        }

        //------------------UPDATE-LOOP---------------------------
        //Debug.Log("update ");
        //Debug.Log("update. ");
        //Debug.Log("update.. ");
        //Debug.Log("update... ");
        //Debug.Log("figObj: "+figureObjects[currentClickedObjectIndex].ToString());
        //animate the 3d object controlled by time
        //double startSec=calculateFigureStartTimeInSec(figureObjects[currentClickedObjectIndex], 100.0f, maxTimeInSec, minX, maxX);

        /*int idx=0;
        for(int i=0;i<timelineObjects.Count;i++)
        {
            idx=idx+1;
            double startSec=calculateFigureStartTimeInSec(timelineObjects[i], 100.0f, maxTimeInSec, minX, maxX);
            //Debug.Log("startSec: "+startSec);
            bool tests=checkFigureObjectShouldPlaying(timelineObjects[i], (int)startSec, 100.0f, AnimationTimer.GetTime());
            //Debug.Log("tests: "+tests);
            if (tests)
            {
                animate3DObjectByTime(timelineObjects3D[i], (int)startSec, 100.0f, AnimationTimer.GetTime(), railStartPos.z, railEndPos.z);
            }
        }*/

        if (gameObject.name == "ImageTimelineRailMusic")
        {
            if (SceneManaging.playing)
            {
                //Debug.Log("+++++++++++++++++updateMusic: " + SceneManaging.updateMusic);
                //Debug.Log("+++++++++++++++++Music is playing");
                int hitObject = 0;
                for (int i = 0; i < timelineInstanceObjects.Count; i++)
                {
                    double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], 100.0f, maxTimeInSec, minX, maxX);
                    double endSec = calculateMusicEndTimeInSec(timelineInstanceObjects[i], 180.0f, maxTimeInSec, minX, maxX);
                    float tmpTime = AnimationTimer.GetTime();
                    //Debug.Log(", startSec: " + startSec + "endSec: " + endSec + ", Timer: " + AnimationTimer.GetTime() + ", Playmusic: " + playingMusic);

                    // wenn timer im bereich musikstuecks und musik ist nicht an
                    if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec && playingMusic == false)
                    {
                        playingMusic = true;

                        audioSource.time = tmpTime - (float)startSec;
                        currentClip = ((int)Char.GetNumericValue(timelineInstanceObjects[i].name[07]) - 1); // object index
                        audioSource.clip = clip[currentClip];
                        audioSource.Play();
                        hitObject = ((int)Char.GetNumericValue(timelineInstanceObjects[i].name[17])); // instance index
                                                                                                      //Debug.Log("++++++++++MUSIC STARTS ++++++ tmpTime: " + audioSource.time + ", current clip: " + currentClip + ", startsec: " + startSec + ", tmpTime: " + tmpTime);

                    }
                    else if (SceneManaging.updateMusic)
                    {
                        if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec)
                        {
                            //Debug.Log("+++++++++++++++++++TimeSlider ist im music piece and slider is being dragged.");
                            audioSource.time = tmpTime - (float)startSec;
                        }
                        if ((AnimationTimer.GetTime() < startSec || AnimationTimer.GetTime() > endSec) && playingMusic == true)
                        {
                            //Debug.Log("+++++++++++++++++++TimeSlider ist ausserhalb und .");
                            audioSource.Stop();
                            playingMusic = false;
                        }

                    }

                }
            }
            else
            {
                audioSource.Stop();
                playingMusic = false;
            }

        }

        else
        {
            //int idx = 0;
            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                // start Animation on play
                if (SceneManaging.playing)
                {
                    try
                    {
                        timelineInstanceObjects3D[i].GetComponent<Animator>().enabled = true;
                    }
                    catch (MissingComponentException ex) { }
                }
                // stop Animation if not playing
                else if (SceneManaging.playing == false) // && timelineInstanceObjects3D[i].GetComponent<Animator>().enabled)
                {
                    try
                    {
                        timelineInstanceObjects3D[i].GetComponent<Animator>().enabled = false;
                    }
                    catch (MissingComponentException ex) { }
                }
                //idx = idx + 1;
                double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], 100.0f, maxTimeInSec, minX, maxX);
                bool tests = checkFigureObjectShouldPlaying(timelineInstanceObjects[i], (int)startSec, 100.0f, AnimationTimer.GetTime());
                //Debug.Log("tests: "+tests);

                //if object should be playing (tests=true):
                if (tests)
                {
                    animate3DObjectByTime(timelineInstanceObjects3D[i], (int)startSec, 100.0f, AnimationTimer.GetTime(), railStartPos.z, railEndPos.z);
                    //Debug.Log("+++++++++++++++++++startSec: "+startSec+" +++++++++++++ Timertime: "+AnimationTimer.GetTime()+timelineInstanceObjects[currentClickedInstanceObjectIndex].name);

                }
                /*if (timelineInstanceObjects[currentClickedInstanceObjectIndex].name[6] == 'F')
                {
                    Debug.Log(timelineInstanceObjects[currentClickedInstanceObjectIndex].name[6]);
                    animate3DObjectByTime(timelineInstanceObjects3D[i], (int)startSec, 100.0f, AnimationTimer.GetTime(), railStartPos.z, railEndPos.z);
                }*/

            }
        }

        //animate3DObjectByTime(figureObjects[currentClickedObjectIndex], (int)startSec, 100.0f, AnimationTimer.GetTime(), rail1StartPos.z, rail1EndPos.z);

        /*string st="";
        st=figureObjects[currentClickedObjectIndex].name;
        Debug.Log("st: "+st);
        if (st=="ButtonFigObj01")
        {
            Debug.Log("case: "+st);
            animate3DObjectByTime(figureObjects3D[0], (int)startSec, 100.0f, AnimationTimer.GetTime(), rail1StartPos.z, rail1EndPos.z);
        }
        if (st=="ButtonFigObj02")
        {
            Debug.Log("case: "+st);
            animate3DObjectByTime(figureObjects3D[1], (int)startSec, 100.0f, AnimationTimer.GetTime(), rail1StartPos.z, rail1EndPos.z);
        }
        */

        /*
        //
        if (Input.GetMouseButtonUp(0) && (!movingObject))
        {
            bool hitTimeline=false;
            if(currentClickedObjectIndex>=0)
                hitTimeline=checkHittingTimeline(figureObjects[currentClickedObjectIndex], timelineImage, getMousePos);
            else
                hitTimeline=false;
            //set object position back to shelf
            if ((!hitTimeline) && (currentClickedObjectIndex>=0) && (!moveTimeSlider(timeSliderImage)))
            {
                //Debug.Log("timeSlider="+moveTimeSlider(timeSliderImage));
                string objectClicked=identifyClickedObject();	//get name of identified object
                bool isPresent=isInList(timelineObjects,figureObjects[currentClickedObjectIndex]);
                if (isPresent)
                {
                    Debug.Log("you released object in space "+objectClicked+" bool: "+isPresent+" name: "+figureObjects[currentClickedObjectIndex].name);
                    Debug.Log("empty-parent: "+objectShelfParent[currentClickedObjectIndex]);
                    Debug.Log("empty-child: "+figureObjects[currentClickedObjectIndex].transform.GetChild(0));
                    figureObjects[currentClickedObjectIndex].transform.position=objectShelfPosition[currentClickedObjectIndex];
                    scaleObject(figureObjects[currentClickedObjectIndex],250.0f,250.0f);
                    //remove object out of objectlist
                    timelineObjects.Remove(figureObjects[currentClickedObjectIndex]);
                    Debug.Log("timelineObjects count: "+timelineObjects.Count);
                }
                else
                {
                    Debug.Log("else released object in space "+objectClicked+" bool: "+isPresent+" name: "+figureObjects[currentClickedObjectIndex].name);

                }
            }
        }*/
    }
}
