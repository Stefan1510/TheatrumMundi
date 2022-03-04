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
    public Text timelineText;
    public Image timeSliderImage;
    private BoxCollider2D timeSlider;
    public GameObject rail3dObj;
    Vector2 textSize;
    //bool timelineOpen;				//timeline open/close
    bool clickingTimeline;          //if timeline is clicked
    bool movingOnTimeline;          //is the mouse over the timeline
    bool draggingOnTimeline;        //dragging the mouse over the timeline
    bool highlighted;              //highlighting of 3D-objects
    bool clickingObject;            //do you only click the object
    bool movingObject;              //is the mouse over the object
    bool draggingObject;            //dragging an object with mouse
    bool objectOnTimeline;          //is object on timeline
    bool doSomethingOnce;           //flag for do something once in a loop
    bool editTimelineObject;        //flag for shifting/clicking an object on timeline
    bool releaseOnTimeline;         //flag if you set an object on timeline
    bool playingMusic;
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
    GameObject mainMenue, parentMenue;
    GameObject timeSettings;
    GameObject[] figCounterCircle;

    GameObject[] figureObjects;
    GameObject[] figureObjects3D;
    int currentClickedObjectIndex;
    int currentClickedInstanceObjectIndex;

    public List<GameObject> timelineObjects;
    public List<GameObject> timelineInstanceObjects;
    public List<GameObject> timelineObjects3D;
    public List<GameObject> timelineInstanceObjects3D;

    //public GameObject gameController;
    private FigureElement ThisFigureElement;    //element to set 3d object
    public GameObject figure1;      //adliger, element 0
    public GameObject figure2;      //hirte, element 1
    public GameObject figure3;      //hirte02, element 2
    public GameObject figure4;      //elephant, element 3
    public GameObject figure5;      //esel, element 4
    public GameObject figure6;      //giraffe, element 5
    double fig1StartPos;
    double fig2StartPos;
    Vector3 railStartPos;
    Vector3 railEndPos;
    //Vector3 rail1StartPos;
    //Vector3 rail1EndPos;
    /*Vector3 rail2StartPos;*/
    //Vector3 rail3StartPos;
    /*Vector3 rail4StartPos;
	Vector3 rail5StartPos;
	Vector3 rail6StartPos;*/

    public AudioClip[] clip;         // ought to be 6 audioclips

    // Start is called before the first frame update
    void Awake()
    {
        timelineImage = this.GetComponent<Image>();
        //timeSliderImage=GetComponent<Image>();
        //timeSlider=timeSliderImage.GetComponent<BoxCollider2D>();
        SceneManager.timelineOpen = false;
        clickingTimeline = false;
        movingOnTimeline = false;
        draggingOnTimeline = false;
        highlighted = false;
        clickingObject = false;
        movingObject = false;
        draggingObject = false;
        objectOnTimeline = false;
        doSomethingOnce = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        playingMusic = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        currentClip = 0;


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
            figureObjects[i] = (objectLibrary.transform.GetChild(i).GetChild(1).gameObject);
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
            Debug.Log("defaultShelfParentArray: " + gaOb);

        //create a new tempGameObject
        newCopyOfFigure = new GameObject();

        figureObjects3D[0] = figure1;
        figureObjects3D[1] = figure2;
        figureObjects3D[2] = figure3;
        figureObjects3D[3] = figure4;
        figureObjects3D[4] = figure5;
        figureObjects3D[5] = figure6;

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
        //mainMenue=GameObject.Find("MenueDirectorMain");
        mainMenue = GameObject.Find("timelineArea");
        parentMenue = GameObject.Find("MenueDirectorMain");
        timeSettings = GameObject.Find("ImageTimeSettingsArea");
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timeSettings.SetActive(false);
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
        Debug.Log("railStartPos: " + railStartPos + "railEndPos: " + railEndPos);

        fig1StartPos = railStartPos.z;//-1.75f;
        fig2StartPos = railEndPos.z;//1.88f;

        minX = 301.0f;  //timeline-minX
        maxX = 1623.0f; //timeline-maxX
        minY = 428.0f;  //timeline-minY
        maxY = 438.0f;  //timeline-maxY
        maxTimeLength = "10:14";
        maxTimeInSec = 614;
        sizeDeltaAsFactor = new Vector2(1.0f, 1.0f);
        Debug.Log("++++++timeline calling");

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
    public void openCloseTimelineByClick(bool flag, Image tl, bool editObjOnTl)
    {
        float colliderScaleUpFacY = 50.0f;
        float colliderScaleDownFacY = 20.0f;    //this size is set in editor

        if (flag == false)
        {
            //set global flag
            SceneManager.timelineOpen = true;
            //scale up timeline
            tl.rectTransform.localScale = new Vector3(1, 5, 1);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, colliderScaleUpFacY);
            //scale down the text
            tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 0.2f, 1);
            //timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
        }
        else if (flag == true && editObjOnTl == false)
        {
            //set global flag
            SceneManager.timelineOpen = false;
            //scale down timeline
            tl.rectTransform.localScale = new Vector3(1, 1, 1);
            //scale down the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, colliderScaleDownFacY);
            //scale up the text
            tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 1, 1);
        }

        openCloseTimeSettings(SceneManager.timelineOpen, timeSettings);

    }
    public void openCloseTimelineByDrag(string open, Image tl)
    {
        float colliderScaleUpFacY = 50.0f;
        float colliderScaleDownFacY = 20.0f;    //this size is set in editor

        if (open == "open")
        {
            SceneManager.timelineOpen = true;
            //scale up timeline
            tl.rectTransform.localScale = new Vector3(1, 5, 1);
            //scale up the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, colliderScaleUpFacY);
            //scale down the text
            tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 0.2f, 1);
            //timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
            //scale down all objects on timeline
        }
        else
        {
            //if open=="close"
            SceneManager.timelineOpen = false;
            //scale down timeline
            tl.rectTransform.localScale = new Vector3(1, 1, 1);
            //scale down the collider
            tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, colliderScaleDownFacY);
            //scale up the text
            tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1, 1, 1);
            //scale up all objects on timeline
        }

        openCloseTimeSettings(SceneManager.timelineOpen, timeSettings);

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
        //save old parent
        oldParent = obj.transform.parent.gameObject;        //parent is a transform, to get the gameobject of this parent, use ".gameObject"
                                                            //Debug.Log("old parent "+oldParent);
    }
    public void setParent(GameObject obj, GameObject parentToSet)
    {
        saveParent(obj);
        //set new parent
        //Debug.Log("new parent "+parentToSet);
        obj.transform.SetParent(parentToSet.transform);
    }
    public void updateObjectPosition(GameObject obj, Vector2 mousePos)
    {
        //set parent
        //obj.transform.SetParent(mainMenue.transform);
        setParent(obj, mainMenue);
        //move object
        obj.transform.position = new Vector2(mousePos.x, mousePos.y);
        //set up flags

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
            Debug.Log("object hits timeline!");
            hit = true; ;
        }
        Debug.Log("drag and hit " + hit);
        return hit;
    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects, bool editObjOnTl)
    {
        float scaleUp = 50.0f;
        float scaleDown = 10.0f;
        float length = 100.0f;
        //Debug.Log("method: timelineObjects count: "+objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            //Debug.Log("object size: "+objects[i].GetComponent<RectTransform>().sizeDelta);
            //if timeline open scale ALL objects up
            if (timelineOpen)
            {
                scaleObject(objects[i], length, scaleUp);
                //objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,50.0f);
                //new Vector2(animationLength,scaleYUp);
            }
            else if ((timelineOpen == false) && (editObjOnTl == false))
            {
                //otherwise scale ALL objects down
                //Debug.Log("method: scale object down:");
                scaleObject(objects[i], length, scaleDown);
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
    public void set3DObject(int idx, GameObject[] obj3D)
    {
        //fig1StartPos=fig1StartPos+0.005f;
        //figure1.transform.position=new Vector3(rail1StartPos.x,figure1.transform.position.y,(float)fig1StartPos);		//start-pos on the left side in timeline
        obj3D[idx].transform.position = new Vector3(railStartPos.x, 0.0f, (float)fig1StartPos * (-1.0f));
        //fig2StartPos=fig2StartPos-0.005f;
        //figure2.transform.position=new Vector3(rail3StartPos.x,figure2.transform.position.y,(float)fig2StartPos);		//start-pos on the right side in timeline
    }
    public void set3DObjectOfInstance(GameObject obj3D)
    {
        obj3D.transform.position = new Vector3(railStartPos.x, 0.0f, (float)fig1StartPos * (-1.0f));
    }
    public void animate3DObjectByTime(GameObject fig3D, int startSec, double animLength, double timerTime, double railStartZ, double railEndZ)
    {
        //Debug.Log("method: fig: " + fig3D + " 3dobj: " + figure1 + " startSec: " + startSec + " animLength: " + animLength + " timerTime: " + timerTime);
        //Debug.Log("fig3d props: " + fig3D.transform.position);
        double figPos = fig3D.transform.position.z;
        double figStartPos = railStartZ * (-1.0f);      //*-1.0 changes startpoint and animation-direction
        animLength = 44.0f;
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
            fig3D.transform.position = new Vector3(railStartPos.x, fig3D.transform.position.y, (float)figPos);
        }
    }

    public void PlayMusic(GameObject fig3D, int startSec, double animLength, double timerTime, double railStartZ, double railEndZ)
    {

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
    public void createRectangle(GameObject obj, Vector2 startPos, Vector2 size, Color col) // start pos brauch ich eigentlich nicht
    {
        //int alpha=1;
        // Vector3 col = new Vector3(255, 120, 60);
        // Image rect = obj.AddComponent<Image>();              // an object can have only one Image-Component
        // rect.color = new Color(col.x, col.y, col.z);
        // rect.transform.position = startPos;          

        GameObject imgObject = new GameObject("RectBackground");
        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(size.x * 0.4f, 0.0f);
        trans.sizeDelta = size; // custom size

        Image image = imgObject.AddComponent<Image>();
        //Texture2D tex = Resources.Load<Texture2D>("red");
        image.color = col;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
        //imgObject.transform.SetParent(obj.transform);
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
        if ((timerTime >= startTime) && (timerTime <= (startTime + (animLength / 2.0))))	///2.0 because animLength=Pixel -> animLength/2~time
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
            point = new Vector3(0.0f, 0.0f, -2.0f);
        }
        else //(startEnd="end")
        {
            //calculate end point of rail 
            point = new Vector3(0.0f, 0.0f, 2.0f);
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
    public void removeObjectFromTimeline(string objName)
    {
        GameObject founded = new GameObject();
        //delete object in timelineInstanceObjects
        for (int i = 0; i < timelineInstanceObjects.Count; i++)
        {
            if (timelineInstanceObjects[i].name == objName)
            {
                founded = timelineInstanceObjects[i];
            }
        }
        timelineInstanceObjects.Remove(founded);

        //perhaps reduce the copies count
    }
    // Update is called once per frame
    async void Update()
    {
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
            Debug.Log("you clicked timelineobject: " + identifyClickedObjectByList(timelineInstanceObjects));
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
            Debug.Log("=====> found hit at index: " + tmpI);
            Debug.Log("=====> instance-array: " + timelineInstanceObjects.Count);

            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                //openCloseTimelineByClick(timelineOpen, timelineImage);
                openCloseTimelineByClick(SceneManager.timelineOpen, timelineImage, editTimelineObject);
                //minimize or maximize objects on timeline
                //openCloseObjectInTimeline(timelineOpen, timelineObjects);
                openCloseObjectInTimeline(SceneManager.timelineOpen, timelineInstanceObjects, editTimelineObject);
                movingOnTimeline = true;        //drag on the timeline
                                                //clickingTimeline=true;	//may be the same as timelineOpen
            }

            //if you click on an object in shelf 
            //first checking if you have an object clicked in shelf
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    clickingObject = true;  //for clicking
                    movingObject = true;        //for dragging

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
                    clickingObject = true;  //did you click the object
                    movingObject = true;        //want to drag the object
                    movingOnTimeline = true;    //access for moving an object on timeline

                    /*outline[currentClickedInstanceObjectIndex]=timelineInstanceObjects3D[currentClickedInstanceObjectIndex].GetComponent<Outline>();
                    outline[currentClickedInstanceObjectIndex].enabled = true;*/
                    //Debug.Log("++++++++++++++++++++++Outline Enabled: "+outline.enabled);
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

                highlighted = false;    // global variable since only one object can be highlighted - everything is unhighlighted
            }
            if (movingOnTimeline)
            {
                //space for dragging
                //Debug.Log("ready to drag something on the timeline...");
                draggingOnTimeline = true;
            }

            if (movingObject)
            {
                //Debug.Log("ready to drag an object from shelf...");
                draggingObject = true;
            }
        }

        // if timeline is open and something is being dragged
        if (draggingOnTimeline && editTimelineObject && SceneManager.timelineOpen)
        {
            //Debug.Log("+++++++++++++++++++++++++dragging on timeline......");
            //Debug.Log("--->before-if-index: "+currentClickedInstanceObjectIndex);
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identifiy-methods
            {
                //Debug.Log("--->index: "+currentClickedInstanceObjectIndex);
                //Debug.Log("index-name: "+timelineInstanceObjects[0].name);
                //Debug.Log("index-collider: "+timelineInstanceObjects[0].GetComponent<BoxCollider2D>());
                //Debug.Log("index-mousepos: "+Physics2D.OverlapPoint(getMousePos));

                //if you click an object in timeline
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //Debug.Log("++++++++++++++++++CLICKED"+timelineInstanceObjects[currentClickedInstanceObjectIndex].name+"+++++++++++++++++++++++"+timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<AudioSource>().clip.name+"playing+++++++++++++"+playingMusic);
                    //playing Music on click (test)
                    /*if(timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<AudioSource>() != null && playingMusic==false) 
                    { 
                        timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<AudioSource>().Play();
                        playingMusic=true;
                    }
                    else 
                    {
                        timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<AudioSource>().Stop();
                        playingMusic=false;
                    }*/

                    //Debug.Log("mooving timelineInstanceObjects: " + timelineInstanceObjects[currentClickedInstanceObjectIndex].name);
                    //highlighting
                    if (highlighted == false)
                    {
                        //outline.enabled=true;
                        highlighted = true;
                    }
                    //move object
                    updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos);
                    //snapping/lock y-axis
                    setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, this.transform.position.y);
                }
            }
            /*
			// if timeline is hit
			if (((getMousePos.x<=((maxX*sizeDeltaAsFactor.x))) && (getMousePos.x>=(minX*sizeDeltaAsFactor.x))) &&
			((getMousePos.y<=((maxY*sizeDeltaAsFactor.y))) && (getMousePos.y>=(minY*sizeDeltaAsFactor.y))))
			{
				//Debug.Log("timeline hitted: "+timelineImage.sprite.rect.width);
				
				//open/maximize the timeline
				timelineImage.rectTransform.localScale = new Vector3(1,5,1);
				float scaleObjY=55.0f;
				timelineImage.GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,scaleObjY);
				//Vector2 textPos=new Vector2(timelineText.GetComponent<RectTransform>().rect.position.x,timelineText.GetComponent<RectTransform>().rect.position.y);
				//Debug.Log("textpos "+textPos);
				timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
				//textPos=new Vector2(timelineText.GetComponent<RectTransform>().rect.position.x,timelineText.GetComponent<RectTransform>().rect.position.y);
				//Debug.Log("textpos "+textPos);
				//timelineText.GetComponent<RectTransform>().sizeDelta = new Vector2(textSize.x,textSize.y);
				//scale up the object (old)
				//objectSceneSize.y=objectDefaultSize.y;
				//scale the object to timeline-size
				objectSceneSize.y=55.0f;				
				//this maximize also the object
				//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
				//Debug.Log("object default width,height: "+objectDefaultSize+" | currenSize: "+objectSceneSize);
				figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
				//scale up the collider
				figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,objectSceneSize.y);
				//if object is on timeline -> allow to dragging
			}
			*/
        }

        // dragging an object from shelf to timeline
        if ((draggingObject) && (editTimelineObject == false))
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
            Debug.Log("mouseclick: " + hitTimeline);
            if (hitTimeline)
            {
                /*
				//name of figure/object which hits the timeline
				nameOfHittedObject=newCopyOfFigure.name;*/

                //open timeline, if its not open
                Debug.Log(">>>flag timelineOpen: " + SceneManager.timelineOpen);
                openCloseTimelineByDrag("open", timelineImage);
                //scale up object/figures in timeline
                openCloseObjectInTimeline(SceneManager.timelineOpen, timelineInstanceObjects, editTimelineObject);
                //and set animation-length
                float animationLength = 100.0f;     //length of objects animation
                float scaleYUp = 50.0f;             //size if the timeline is maximized
                                                    //figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(animationLength,scaleYUp);
                                                    //scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleYUp);

                //scale down the dragged figure
                if (timelineObjects.Count > 0)      //if you dont check this, you get an out-of-range exception
                {
                    //Debug.Log("--->scale up!");
                    scaleObjectsOfList(timelineObjects, animationLength, scaleYUp);
                }
                // change parent back
                setParent(figureObjects[currentClickedObjectIndex], mainMenue);

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
                Debug.Log("release object at pos: " + getMousePos + " " + Physics2D.OverlapPoint(getMousePos));

                //animation of 3d object not during dragging object on timeline ->outside of dragging

                //set up flags
            }
            else
            {
                //close timeline if you click e.g. in the shelf to get a new figure
                openCloseTimelineByDrag("close", timelineImage);
                //scale down also the figures on timeline
                openCloseObjectInTimeline(SceneManager.timelineOpen, timelineInstanceObjects, editTimelineObject);
                Debug.Log("+++close timeline by drag");
            }
        }

        //-------release mousebutton
        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            //Debug.Log("release mouse button at pos: " + getMousePos + " " + Physics2D.OverlapPoint(getMousePos));
            //Debug.Log("++++++left mouse button up");
            clickingTimeline = false;
            movingOnTimeline = false;
            draggingOnTimeline = false;
            clickingObject = false;
            movingObject = false;
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
                        createRectangle(newCopyOfFigure, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition, new Vector2(300, 50),Color.green);
                    }
                    else createRectangle(newCopyOfFigure, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition, new Vector2(300, 50),Color.cyan);
                    
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
                        GameObject curr3DObject = new GameObject();
                        curr3DObject = Instantiate(figureObjects3D[currentClickedObjectIndex]);
                        timelineInstanceObjects3D.Add(curr3DObject);
                        //set3DObject(currentClickedObjectIndex,figureObjects3D);
                        set3DObjectOfInstance(timelineInstanceObjects3D[timelineInstanceObjects3D.Count - 1]); //the last added object

                        //Debug.Log("original shelf-pos: "+objectShelfPosition[currentClickedObjectIndex]);
                        /*
                            figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                            //Debug.Log("original parent: "+objectShelfParent[currentClickedObjectIndex]+" count: "+objectShelfParent.Length);				

                            //objectShelfPosition[i]=new Vector2(figureObjects[i].transform.position.x,figureObjects[i].transform.position.y);
                            */

                        //show the instances as a pinned number
                    }
                }

                releaseOnTimeline = false;
                releaseObjMousePos.x = 0.0f;
                releaseObjMousePos.y = 0.0f;

                /*
                // ------------ Dinge, die fuer die Kulissen im Controler passieren muessen

                //ThisSceneryElement.parent = "Schiene" + statusReiter.ToString(); //hier bitte mal genau �berpr�fen wie die parents dann wirklich sind
                Debug.Log("---- Figurename -------- " + ThisFigureElement.name + " --- " + ThisFigureElement.parent + " --- " + ThisFigureElement.x);
                ThisFigureElement.z = GetComponent<RectTransform>().anchoredPosition.x/300;
                ThisFigureElement.y = GetComponent<RectTransform>().anchoredPosition.y/300+0.1f;  //die werte stimmen ungefaehr mit dem liveview ueberein


                // ------------ uebertragen der Daten aus dem Controller auf die 3D-Kulissen
                gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData); // dieses Zeile macht das gleiche und ist glaube besser.
                */
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

            if (gameObject.name == "ImageTimelineRail1")
            {

                //int idx = 0;
                for (int i = 0; i < timelineInstanceObjects.Count; i++)
                {
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

            else if (gameObject.name == "ImageTimelineRailMusic")
            {


                for (int i = 0; i < timelineInstanceObjects.Count; i++)
                {
                    double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], 100.0f, maxTimeInSec, minX, maxX);
                    double endSec = calculateMusicEndTimeInSec(timelineInstanceObjects[i], 180.0f, maxTimeInSec, minX, maxX);

                    //Debug.Log(", startSec: " + startSec + "endSec: " + endSec + ", Timer: " + AnimationTimer.GetTime() + ", Playmusic: " + playingMusic);

                    // wenn timer im bereich musikstuecks und musik ist nicht an
                    if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec && playingMusic == false)
                    {
                        playingMusic = true;
                        currentClip = (int)Char.GetNumericValue(timelineInstanceObjects[i].name[13]);
                        audioSource.clip = clip[currentClip];
                        audioSource.Play();
                        Debug.Log("++++++++++MUSIC STARTS ++++++";

                    }
                    // wenn timer im bereich des musikstuecks und musik ist an
                    else if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec && playingMusic == true)
                    {
                        //Debug.Log("+++++++++++++NOTHING"+ ", Playmusic: " + playingMusic+", currentClip: " + currentClip + "++++timelineInstanceObjects[i].name "+timelineInstanceObjects[i].name);
                    }
                    // wenn timer ausserhalb des musikstuecks und musik ist an
                    else if (playingMusic == true && AnimationTimer.GetTime() < startSec || AnimationTimer.GetTime() > endSec)
                    {
                        if (currentClip == (int)Char.GetNumericValue(timelineInstanceObjects[i].name[13]))
                        {
                            audioSource.Stop();
                            playingMusic = false;
                            Debug.Log("+++++++++++++MUSIC STOPPED");
                        }

                    }


                }

            }




            // double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], 100.0f, maxTimeInSec, minX, maxX);
            // bool testsMusic = checkFigureObjectShouldPlaying(timelineInstanceObjects[i], (int)startSec, 100.0f, AnimationTimer.GetTime());

            // if (testsMusic)
            // {
            //     PlayMusic(figureObjects[currentClickedObjectIndex], (int)startSec, 100.0f, AnimationTimer.GetTime(), railStartPos.z, railEndPos.z);
            //     Debug.Log(timelineInstanceObjects[currentClickedInstanceObjectIndex].name[6] + audioSource.clip.name + "current index: " + currentClickedObjectIndex);
            //     audioSource.clip = clip[currentClickedObjectIndex];
            //     audioSource.Play();
            //     playingMusic = true;



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
