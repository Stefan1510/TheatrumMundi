using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RailManager : MonoBehaviour
{
    public class Rail
    {
        public List<ObjectsPoint> myObjectsPositionListLayer1 = new List<ObjectsPoint>();
        public List<ObjectsPoint> myObjectsPositionListLayer2 = new List<ObjectsPoint>();
        public List<GameObject> timelineInstanceObjects, timelineInstanceObjects3D, figuresLayer1, figuresLayer2; //figuresLayer3, figuresLayer4;
        public int sizeLayering;
        public bool isTimelineOpen;
    }
    #region public variables
    public GameObject[] rails = new GameObject[6];
    public Rail[] railList = new Rail[6];
    public GameObject[] rails3D = new GameObject[6];
    [HideInInspector] public GameObject[] figCounterCircle;
    public GameObject gameController;
    public GameObject liveView;
    public GameObject helpButton;
    public GameObject objectLibrary, UICanvas, parentMenue; // mainMenue
    #endregion
    #region private variables
    [SerializeField] GameObject scrollRect, areaFiguresPut;
    Vector2 objectShelfSize;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    float objectAnimationLength, rectSize;
    float figPictureSize = 100.0f;
    float heightRailPercent;
    Vector2 releaseObjMousePos, screenDifference;
    double minX, maxX;
    float railWidth, publicPosX, railwidthAbsolute;
    Vector3 railStartPoint, railEndPoint;
    Color colFigureAlpha = new Color(0.06f, 0.66f, .74f, 0.38f);
    Color colFigure = new Color(0.06f, 0.66f, .74f, 0.5f);
    Color colFigureHighlighted = new Color(0f, 0.87f, 1.0f, 0.5f);
    private GameObject[] figureObjects, figureObjects3D;
    private int currentClickedObjectIndex, currentClickedInstanceObjectIndex, currentRailIndex, hitTimeline;
    private int count, maxTimeInSec;
    private float currentLossyScale;
    private float _timerFigure = 1;
    int newHitTimeline;
    private Vector2 diff;
    private FigureElement ThisFigureElement;    //element to set 3d object
    private float heightOpened, heightClosed, _spaceMax, _spaceMax3D;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, isInstance, changedRail, _toBeRemoved;
    //private bool figureOnLiveView;
    int hitTimelineOld;
    private int _counterTouched = 0;
    #endregion
    #region Lists
    // Objects Position List

    [Serializable]
    public class ObjectsPoint
    {
        public string objName;
        public Vector2 position;        // x ist x-Position (anchoredPosition.x) des objekts, y ist die länge des objekts (sizeDelta.x)

    }
    #endregion
    void Awake()
    {
        float format = (float)Screen.width / (float)Screen.height;
        if(format >1.78f)
        {
            //Debug.Log("format: "+format);
            float widthNew = Screen.height * 1.77f;
            // Screen.SetResolution((int)widthNew, (int)Screen.height, true);
        }

        if (gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
        {
            heightRailPercent = 0.018f;
        }
        else
        {
            heightRailPercent = 0.022f;
            GetComponent<VerticalLayoutGroup>().spacing = 20;
        }
        draggingOnTimeline = false;
        draggingObject = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        isInstance = false;
        hitTimelineOld = -1;
        railwidthAbsolute = 1670.4f;

        for (int i = 0; i < rails.Length; i++)
        {
            railList[i] = new Rail();
            railList[i].sizeLayering = 1;
            railList[i].isTimelineOpen = false;
            railList[i].timelineInstanceObjects = new List<GameObject>();
            railList[i].timelineInstanceObjects3D = new List<GameObject>();
            railList[i].figuresLayer1 = new List<GameObject>();
            railList[i].figuresLayer2 = new List<GameObject>();
        }
        currentRailIndex = 0;

        railStartPoint = new Vector3(0.0f, 0.0f, -2.2f);
        railEndPoint = new Vector3(0.0f, 0.0f, 2.6f);

        //load all objects given in the figuresShelf
        figureObjects = new GameObject[objectLibrary.transform.childCount];
        figureObjects3D = new GameObject[figureObjects.Length];
        //objectShelfPosition = new Vector2[figureObjects.Length];

        objectShelfParent = new GameObject[figureObjects.Length];
        figCounterCircle = new GameObject[figureObjects.Length];

        //liveView.GetComponent<BoxCollider2D>().size = liveView.GetComponent<RectTransform>().sizeDelta;

        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = (objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject);

            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
        }
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
            figCounterCircle[i].transform.GetChild(0).GetComponent<Text>().text = "0";
        }


        objectShelfSize = new Vector2(figureObjects[0].GetComponent<RectTransform>().rect.width, figureObjects[0].GetComponent<RectTransform>().rect.height);

        try
        {
            for (int i = 0; i < figureObjects3D.Length; i++)
            {
                figureObjects3D[i] = gameController.GetComponent<SceneDataController>().objectsFigureElements[i];
            }
        }
        catch (IndexOutOfRangeException) { }

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;
        currentLossyScale = 0.0f;   // lossyScale is global Scale that I use to get the moment when ScreenSize is being changed (doesn't happen exactly when Screen Size is changed, but somehow shortly after, so I use this parameter instead)

        ResetScreenSize();
        // for (int i = 0; i < 6; i++)
        // {
        //     rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(railwidthAbsolute, heightClosed / rails[currentRailIndex].transform.lossyScale.x);
        //     rails[i].GetComponent<BoxCollider2D>().size = new Vector2(railwidthAbsolute, heightClosed / rails[currentRailIndex].transform.lossyScale.x * 1.2f);
        // }

        List<GameObject> timelineInstanceObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects3D = new List<GameObject>();
        maxTimeInSec = (int)AnimationTimer.GetMaxTime();

        for (int i = 0; i < figureObjects3D.Length; i++) // ships dont have animation
        {
            if (figureObjects3D[i].GetComponent<FigureStats>().isShip)
            {
                figureObjects3D[i].GetComponent<cakeslice.Outline>().enabled = false;
            }
            else
            {
                figureObjects3D[i].transform.GetChild(1).GetComponent<cakeslice.Outline>().enabled = false;
                figureObjects3D[i].GetComponent<Animator>().enabled = false;
            }
        }

        _spaceMax3D = 0.5f; // eingehaltener Platz zwischen Figuren soll 50 cm sein
        _spaceMax = (0.1f * _spaceMax3D * 100) / maxTimeInSec * railwidthAbsolute;      // *100 fuer umrechnung m in cm, 0.1 fuer speed
        AnimationTimer.SetTime(0);
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
    private bool isCurrentFigureOverlapping(int railIndex, GameObject obj)
    {
        bool val = false;
        for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
        {
            //Debug.Log("pos1: " + railList[railIndex].myObjectsPositionListLayer1[i].position.x + ", pos obj: " + obj.GetComponent<RectTransform>().anchoredPosition.x);

            if (((railList[railIndex].myObjectsPositionListLayer1[i].position.x < obj.GetComponent<RectTransform>().anchoredPosition.x
            && railList[railIndex].myObjectsPositionListLayer1[i].position.x + railList[railIndex].myObjectsPositionListLayer1[i].position.y > obj.GetComponent<RectTransform>().anchoredPosition.x)
            || (railList[railIndex].myObjectsPositionListLayer1[i].position.x > obj.GetComponent<RectTransform>().anchoredPosition.x
            && railList[railIndex].myObjectsPositionListLayer1[i].position.x < obj.GetComponent<RectTransform>().anchoredPosition.x + obj.GetComponent<RectTransform>().sizeDelta.x))
            && railList[railIndex].myObjectsPositionListLayer1[i].objName != obj.name)
            {
                Debug.Log("name: " + obj.name + ", anderer: " + railList[railIndex].myObjectsPositionListLayer1[i].objName);
                val = true;
            }
        }
        return val;
    }
    public void openTimelineByClick(bool thisTimelineOpen, int index, bool fromShelf)
    {
        if (fromShelf == false && SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive != 1)
        {
            UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf05(true);
        }
        if (thisTimelineOpen)
        {
            // if timeline is already open, unhighlight all
            for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects3D.Count; i++)
            {
                highlight(railList[currentRailIndex].timelineInstanceObjects3D[i], railList[currentRailIndex].timelineInstanceObjects[i], false);
            }
        }
        else
        {
            // a different rail is open - close it
            for (int i = 0; i < rails.Length; i++)
            {
                rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                rails[i].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                railList[i].isTimelineOpen = false;
                openCloseObjectInTimeline(false, railList[i].timelineInstanceObjects, i);
                for (int j = 0; j < railList[i].timelineInstanceObjects3D.Count; j++)
                {
                    highlight(railList[i].timelineInstanceObjects3D[j], railList[i].timelineInstanceObjects[j], false);
                }
            }
            for (int j = 0; j < 2; j++)
            {
                gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
            }
            gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
            gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
            gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
            gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects);

            for (int j = 0; j < gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects.Count; j++)
            {
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().highlight(gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects[j], false);
            }

            // open clicked rail and collider
            rails[index].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 80);
            rails[index].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 80);
            openCloseObjectInTimeline(true, railList[index].timelineInstanceObjects, index);
            //Debug.Log(railList[index].timelineInstanceObjects[0].GetComponent<RectTransform>().sizeDelta);
            railList[index].isTimelineOpen = true;
            // Debug.Log("open: " + index + ", obj: " + railList[index].timelineInstanceObjects.Count + ", layering: " + railList[index].sizeLayering);

            ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(rails[index].name[17]) - 1);
            ImageTimelineSelection.SetRailType(0);  // for rail-rails
        }
    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects, int railIndex)
    {
        float length = rectSize;

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -rails[railIndex].GetComponent<RectTransform>().rect.height / 2, -1);
            Debug.Log("height: "+rails[railIndex].GetComponent<RectTransform>().rect.height);

            if (timelineOpen)
            {
                if (railList[railIndex].sizeLayering == 1)
                {
                    scaleToLayerSize(objects[i], 0, rails[railIndex]);
                }
                else if (railList[railIndex].sizeLayering == 2)
                {
                    if (railList[railIndex].figuresLayer1.Contains(objects[i]))
                    {
                        scaleToLayerSize(objects[i], 1, rails[railIndex]);
                        objects[i].GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].GetComponent<BoxCollider2D>().offset.x, rails[railIndex].GetComponent<RectTransform>().rect.height / 4);
                    }
                    else
                    {
                        scaleToLayerSize(objects[i], 2, rails[railIndex]);
                        objects[i].GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].GetComponent<BoxCollider2D>().offset.x, -rails[railIndex].GetComponent<RectTransform>().rect.height / 4);
                    }
                }
            }
            else
            {
                if (railList[railIndex].sizeLayering == 1)
                {
                    scaleObject(objects[i], length, 10, true);
                }
                else if (railList[railIndex].sizeLayering == 2)
                {
                    objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -(heightClosed / rails[railIndex].transform.lossyScale.x / 2), -1);

                    scaleObject(objects[i], length, 10, true);
                }
            }
        }
    }
    public void openTimelineByDrag(int index)
    {
        if (!railList[index].isTimelineOpen)
        {
            if (gameController.GetComponent<UIController>().RailLightBG[0].isTimelineOpen)
            {
                gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<RailLightManager>().isTimelineOpen = false;
            }
            else if (gameController.GetComponent<UIController>().RailLightBG[1].isTimelineOpen)
            {
                gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<RailLightManager>().isTimelineOpen = false;
            }
            else if (gameController.GetComponent<UIController>().RailMusic.isTimelineOpen)
            {
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects);
            }
            for (int i = 0; i < rails.Length; i++)  // alle schienen schliessen
            {
                if (railList[i].isTimelineOpen)
                {
                    rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                    rails[i].GetComponent<BoxCollider2D>().size = new Vector2(rails[currentRailIndex].GetComponent<BoxCollider2D>().size.x, 20);
                    railList[i].isTimelineOpen = false;
                    openCloseObjectInTimeline(false, railList[i].timelineInstanceObjects, i);
                    for (int j = 0; j < railList[i].timelineInstanceObjects3D.Count; j++)
                    {
                        highlight(railList[i].timelineInstanceObjects3D[j], railList[i].timelineInstanceObjects[j], false);
                    }
                }
            }
            railList[currentRailIndex].isTimelineOpen = false;
            //scale down timeline, collider, scale up objects on timeline
            rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[currentRailIndex].GetComponent<RectTransform>().rect.width, 20);
            rails[currentRailIndex].GetComponent<BoxCollider2D>().size = new Vector2(rails[currentRailIndex].GetComponent<BoxCollider2D>().size.x, 20);
            //scale up hovered timeline
            rails[index].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 80);
            rails[index].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 80);
            railList[index].isTimelineOpen = true;

            openCloseObjectInTimeline(true, railList[index].timelineInstanceObjects, index);
        }
    }
    public void updateObjectPosition(GameObject obj, Vector2 mousePos)
    {
        obj.transform.position = new Vector3(mousePos.x, mousePos.y, -1.0f);
    }
    public int checkHittingTimeline(Vector2 mousePos)
    {
        int hit = -1;
        for (int i = 0; i < rails.Length; i++)
        {
            Vector2 colSize = new Vector2(rails[i].GetComponent<BoxCollider2D>().size.x, rails[i].GetComponent<BoxCollider2D>().size.y);
            //Debug.Log("colsize: "+colSize+", mousePos: "+mousePos);
            if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= rails[i].transform.position.y + (colSize.y / 2.0f) && mousePos.y > rails[i].transform.position.y - (colSize.y / 2.0f))
                hit = i;
        }
        return hit;
    }
    private bool isRailAreaHit(Vector2 mousePos)
    {
        bool val = false;
        if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= areaFiguresPut.transform.position.y + areaFiguresPut.GetComponent<RectTransform>().sizeDelta.y / 2 && mousePos.y > areaFiguresPut.transform.position.y - areaFiguresPut.GetComponent<RectTransform>().sizeDelta.y / 2)
            val = true;
        return val;
    }
    public void scaleDeleteButton(GameObject btn, float posY, float size, float colSize)
    {
        btn.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(30, posY, -1);
        btn.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
    }
    public void scaleObject(GameObject fig, float x, float y, bool boxCollZero)
    {
        //scale the object
        fig.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        fig.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        fig.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(100, y);

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
    }
    public void scaleToLayerSize(GameObject obj, int layer, GameObject timeline)
    {
        switch (layer)
        {
            case 0:                 // only one layer
                scaleDeleteButton(obj, 20, 40, 45);
                scaleObject(obj, rectSize, timeline.GetComponent<RectTransform>().rect.height, false);      //scale the figure-picture in timeline to x: 100 and y: 80px
                obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 0.5f, -1);
                obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, timeline.GetComponent<RectTransform>().rect.height);
                obj.GetComponent<BoxCollider2D>().offset = new Vector2((obj.GetComponent<BoxCollider2D>().size.x - 100) / 2, 0);
                break;
            case 1:                 // 2 layers, but object in layer 1
                scaleDeleteButton(obj, 0, 35, 40);
                scaleObject(obj, rectSize, timeline.GetComponent<RectTransform>().rect.height / 2, false);
                obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 0, -1);
                obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, timeline.GetComponent<RectTransform>().rect.height / 2);
                obj.GetComponent<BoxCollider2D>().offset = new Vector2((obj.GetComponent<BoxCollider2D>().size.x - 100) / 2, obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                break;
            case 2:                 // 2 layers and object in layer 2
                scaleDeleteButton(obj, 0, 35, 40);
                scaleObject(obj, rectSize, timeline.GetComponent<RectTransform>().rect.height / 2, false);
                obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 1, -1);
                obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, timeline.GetComponent<RectTransform>().rect.height / 2);
                obj.GetComponent<BoxCollider2D>().offset = new Vector2((obj.GetComponent<BoxCollider2D>().size.x - 100) / 2, -obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                break;
        }
    }
    public void setObjectOnTimeline(GameObject fig, float y)
    {
        fig.transform.position = new Vector3(fig.transform.position.x, y, -1.0f);
    }
    public void updateObjectList(List<GameObject> objects, GameObject obj)
    {
        //check if the object is already in list
        if (!objects.Contains(obj)) objects.Add(obj);
    }
    public void createRectangle(GameObject obj, Color col, double rectHeight)
    {
        double tmpLength = rectSize;//calcSecondsToPixel(objectAnimationLength, maxTimeInSec) / 2 + 25;
        GameObject imgObject = new GameObject("RectBackground");
        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(-50, 0);

        trans.sizeDelta = new Vector2((float)tmpLength, (float)rectHeight);    //size related to animationLength
        Image image = imgObject.AddComponent<Image>();
        image.color = col;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;

        // if (defaultBox)
        // {
        //     trans.anchoredPosition = new Vector2(0, 0);
        //     tmpLength = 100;
        // }
        // else
        // {
        trans.SetSiblingIndex(0);
        // }
        // RectTransform trans;
        // GameObject imgObject;
        // Image image;
        // double tmpLength;

        // if (defaultBox)
        // {
        //     imgObject = new GameObject("RectBackground");
        //     trans = imgObject.AddComponent<RectTransform>();
        //     trans.anchoredPosition = new Vector2(0, 0);
        //     tmpLength = 100;
        //     image = imgObject.AddComponent<Image>();

        // }
        // else
        // {
        //     imgObject = obj.transform.GetChild(0).gameObject;
        //     tmpLength = rectSize;//calcSecondsToPixel(objectAnimationLength, maxTimeInSec) / 2 + 25;
        //     trans = obj.transform.GetChild(1).GetComponent<RectTransform>(); // hier ist rect noch an stelle 1
        //     trans.anchoredPosition = new Vector2(-50, 0);
        //     image = imgObject.GetComponent<Image>();
        // }

        // trans.transform.SetParent(obj.transform); // setting parent
        // trans.SetSiblingIndex(0);
        // trans.sizeDelta = new Vector2((float)tmpLength, (float)rectHeight);    //size related to animationLength
        // image.color = col;
        // var tempColor = image.color;
        // tempColor.a = 0.5f;
        // image.color = tempColor;
        // trans.localScale = Vector3.one;

    }
    /*public int calculateFigureStartTimeInSec(GameObject fig, double animLength, int maxTimeLengthInSec, double railMinX, double railMaxX)
    {
        //PROBLEMS:
        //animLength are in seconds but this is not 100 pixel!!! ->calc seconds to pixel is required
        //tmpX is not correct calculated

        int sec = 0;
        // double tmpX = fig.transform.position.x - railMinX;  //x-pos is screenX from left border, railMinX is the rail-startpoint
        // Vector2 tmpSize = fig.GetComponent<BoxCollider2D>().size;
        // double tmpMinX = (double)tmpX - (tmpSize.x / 2.0f); //if tmpX is the midpoint of figure
        // double tmpMaxX = tmpMinX + animLength;  //length of figure is related to rail speed, here named as animationLength
        //                                         //get figure minX related to timelineMinX
        // double percentageOfRail = tmpMinX / (railMaxX - railMinX);  //max-min=real length, percentage: e.g. 0,3823124 (38%)
        // sec = (int)(((double)maxTimeLengthInSec) * percentageOfRail);


        double tmpX = fig.transform.GetChild(0).gameObject.transform.position.x - (fig.GetComponent<RectTransform>().sizeDelta.x / 2 / 1920.0f * (double)Screen.width);
        tmpX = tmpX - railMinX;
        double percentageOfRail = (tmpX) / (railMaxX - railMinX);           //percent (between 0 and 1) of blue-rect-startpoint in relation to the complete rail
        sec = (int)(((double)maxTimeLengthInSec) * percentageOfRail);   //seconds of percentage
        return sec;
    }*/
    /*public Vector3 getRailStartEndpoint(GameObject r3DObj, string startEnd)
    {
        //this returns the coordinate (not pixel!) values of a rail
        //railStartPoint & railEndPoint = global variables
        Vector3 point = new Vector3(0.0f, 0.0f, 0.0f);
        if (startEnd == "start")
        {
            //calculate start point of the rail
            point = new Vector3(railStartPoint.x, railStartPoint.y, railStartPoint.z);
        }
        else //(startEnd="end")
        {
            //calculate end point of rail
            point = new Vector3(railEndPoint.x, railEndPoint.y, railEndPoint.z);
        }
        return point;
    }*/
    public int countCopiesOfObject(GameObject fig)
    {
        int c = 0;
        for (int i = 0; i < rails.Length; i++)
        {
            //count object with the same name as fig
            foreach (GameObject gO in railList[i].timelineInstanceObjects)
            {
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

        // erase from layer list
        if (railList[currentRailIndex].figuresLayer1.Contains(obj))
        {
            Debug.Log("wanted: " + obj.name);
            for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
            {
                Debug.Log("obj: " + railList[currentRailIndex].myObjectsPositionListLayer1[i].objName);
                if (railList[currentRailIndex].myObjectsPositionListLayer1[i].objName == obj.name)
                {

                    railList[currentRailIndex].myObjectsPositionListLayer1.Remove(railList[currentRailIndex].myObjectsPositionListLayer1[i]);
                }
            }
            railList[currentRailIndex].figuresLayer1.Remove(obj);

            //railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(x => x.position.x).ToList();
        }
        else if (railList[currentRailIndex].figuresLayer2.Contains(obj))
        {
            Debug.Log("wanted: " + obj.name);
            for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
            {
                Debug.Log("obj: " + railList[currentRailIndex].myObjectsPositionListLayer2[i].objName);
                if (railList[currentRailIndex].myObjectsPositionListLayer2[i].objName == obj.name)
                {
                    railList[currentRailIndex].myObjectsPositionListLayer2.Remove(railList[currentRailIndex].myObjectsPositionListLayer2[i]);

                }
            }
            railList[currentRailIndex].figuresLayer2.Remove(obj);

            //railList[currentRailIndex].myObjectsPositionListLayer2 = railList[currentRailIndex].myObjectsPositionListLayer2.OrderBy(x => x.position.x).ToList();
        }

        railList[currentRailIndex].timelineInstanceObjects.Remove(obj);
        railList[currentRailIndex].timelineInstanceObjects3D.Remove(obj3D);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
        StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements.Remove(StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(obj.name.Substring(17))]);

        Destroy(obj);
        Destroy(obj3D);

        if (!isSomethingOverlapping(currentRailIndex))
        {
            railList[currentRailIndex].sizeLayering = 1;
            for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects.Count; i++)
            {
                scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[i], 0, rails[currentRailIndex]);
                if (railList[currentRailIndex].figuresLayer2.Contains(railList[currentRailIndex].timelineInstanceObjects[i]))
                {
                    railList[currentRailIndex].figuresLayer2.Remove(railList[currentRailIndex].timelineInstanceObjects[i]);
                    railList[currentRailIndex].figuresLayer1.Add(railList[currentRailIndex].timelineInstanceObjects[i]);
                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[i], railList[currentRailIndex].myObjectsPositionListLayer2, railList[currentRailIndex].myObjectsPositionListLayer1);
                }
            }
            railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(x => x.position.x).ToList();
        }
    }
    public void removeObjectFromTimeline2D(GameObject obj, bool create) // if Delete Button is pressed (button only knows 2D-Figure, 3D-Figure has to be calculated here)
    {
        int tmpNr = int.Parse(obj.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text);
        int val = 0;

        // 3D only if not created
        if (!create)
        {
            for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects.Count; i++)
            {
                if (obj.name == railList[currentRailIndex].timelineInstanceObjects[i].name)
                    val = i;
            }

            Destroy(railList[currentRailIndex].timelineInstanceObjects3D[val]);
            railList[currentRailIndex].timelineInstanceObjects3D.Remove(railList[currentRailIndex].timelineInstanceObjects3D[val]);
            StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements.Remove(StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(obj.name.Substring(17))]);

        }
        //      Debug.Log("val: "+val);

        //Debug.Log("obj: " + railList[currentRailIndex].timelineInstanceObjects3D[val].name);

        // erase from layer list
        if (railList[currentRailIndex].figuresLayer1.Contains(obj))
        {
            railList[currentRailIndex].figuresLayer1.Remove(obj);
            for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
            {
                if (railList[currentRailIndex].myObjectsPositionListLayer1[i].objName == obj.name)
                    railList[currentRailIndex].myObjectsPositionListLayer1.Remove(railList[currentRailIndex].myObjectsPositionListLayer1[i]);
            }
        }
        else if (railList[currentRailIndex].figuresLayer2.Contains(railList[currentRailIndex].timelineInstanceObjects[val]))
        {
            railList[currentRailIndex].figuresLayer2.Remove(railList[currentRailIndex].timelineInstanceObjects[val]);
            for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
            {
                if (railList[currentRailIndex].myObjectsPositionListLayer2[i].objName == obj.name)
                    railList[currentRailIndex].myObjectsPositionListLayer2.Remove(railList[currentRailIndex].myObjectsPositionListLayer2[i]);
            }
        }

        Destroy(obj);

        railList[currentRailIndex].timelineInstanceObjects.Remove(obj);

        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
        Debug.Log("index: " + currentRailIndex);

        if (!isSomethingOverlapping(currentRailIndex))
        {
            railList[currentRailIndex].sizeLayering = 1;
            for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects.Count; i++)
            {
                scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[i], 0, rails[currentRailIndex]);
                if (railList[currentRailIndex].figuresLayer2.Contains(railList[currentRailIndex].timelineInstanceObjects[i]))
                {
                    railList[currentRailIndex].figuresLayer2.Remove(railList[currentRailIndex].timelineInstanceObjects[i]);
                    railList[currentRailIndex].figuresLayer1.Add(railList[currentRailIndex].timelineInstanceObjects[i]);
                }
            }
            railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(x => x.position.x).ToList();
        }
    }
    /*public int checkHittingAnyTimeline(GameObject obj, Vector2 mousePos)
    {
        int hit = -1;
        Image[] tl = new Image[rails.Length];
        for (int i = 0; i < rails.Length; i++)
        {
            tl[i] = rails[i].GetComponent<Image>();
            Vector2 colSize;
            if (railList[i].isTimelineOpen) colSize = new Vector2(railWidth, heightOpened);
            else colSize = new Vector2(railWidth, heightClosed);
            Vector2 tlPos = new Vector2(tl[i].transform.position.x + (colSize.x / 2), tl[i].transform.position.y);
            //if mouse hits the timeline while dragging an object
            //my implementation of object-boundingbox
            if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
            ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
            {
                hit = i;
            }
        }
        return hit;
    }*/
    public void highlight(GameObject obj3D, GameObject obj, bool highlightOn)
    {
        if (highlightOn)
        {
            obj.transform.GetChild(0).GetComponent<Image>().color = colFigureHighlighted;
            obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);   //show Delete-Button
            SceneManaging.highlighted = true;

            if (obj3D.GetComponent<FigureStats>().isShip) obj3D.GetComponent<cakeslice.Outline>().enabled = true;
            else obj3D.transform.GetChild(1).GetComponent<cakeslice.Outline>().enabled = true;
        }
        else
        {
            obj.transform.GetChild(0).GetComponent<Image>().color = colFigure;
            obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);  //hide Delete-Button
            SceneManaging.highlighted = false;
            if (obj3D.GetComponent<FigureStats>().isShip) obj3D.GetComponent<cakeslice.Outline>().enabled = false;
            else obj3D.transform.GetChild(1).GetComponent<cakeslice.Outline>().enabled = false;

        }
    }
    public GameObject CreateNew2DInstance(int figureNr, float momentOrPosX, int loadFromFile, int savedLayer)   // if layer = -1, then new instance, else layer has been saved
    {
        GameObject curr3DObject;
        if (loadFromFile != -1) currentRailIndex = loadFromFile;

        int countName = 0;
        countName = countCopiesOfObject(figureObjects[figureNr]);

        if (gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
        {
            figCounterCircle[figureNr].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();
        }

        newCopyOfFigure = Instantiate(figureObjects[figureNr]);
        newCopyOfFigure.name = figureObjects[figureNr].name + "instance" + countName.ToString("000");

        //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
        updateObjectList(railList[currentRailIndex].timelineInstanceObjects, newCopyOfFigure);
        //parent and position
        newCopyOfFigure.transform.SetParent(rails[currentRailIndex].transform);
        newCopyOfFigure.transform.localScale = Vector3.one;
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x - 50, newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y);

        ObjectsPoint oP = new ObjectsPoint();
        oP.objName = newCopyOfFigure.name;
        oP.position = new Vector2(newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x, newCopyOfFigure.GetComponent<RectTransform>().sizeDelta.x);

        // from scenedataController
        if (loadFromFile != -1)
        {
            float posX = (UtilitiesTm.FloatRemap(momentOrPosX, 0, AnimationTimer.GetMaxTime(), 0, railwidthAbsolute)) + 50;

            objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(momentOrPosX);
            rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);

            createRectangle(newCopyOfFigure, colFigure, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);

            if (savedLayer != -1)
            {
                newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3(posX, rails[currentRailIndex].GetComponent<RectTransform>().rect.height / 2, -1);
                oP.position = new Vector2(newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x, rectSize);

                //Debug.Log("neue figur: pos: " + oP.position.x);
                //Debug.Log("saved layer: " + savedLayer);
                if (savedLayer == 1)
                {
                    scaleToLayerSize(newCopyOfFigure, 1, rails[currentRailIndex]);
                    railList[currentRailIndex].figuresLayer1.Add(newCopyOfFigure);
                    railList[currentRailIndex].myObjectsPositionListLayer1.Add(oP);
                    railList[currentRailIndex].sizeLayering = 2;
                    railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                }
                else if (savedLayer == 0)
                {
                    scaleToLayerSize(newCopyOfFigure, 0, rails[currentRailIndex]);
                    railList[currentRailIndex].figuresLayer1.Add(newCopyOfFigure);
                    railList[currentRailIndex].myObjectsPositionListLayer1.Add(oP);
                    railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                }
                else
                {
                    scaleToLayerSize(newCopyOfFigure, 2, rails[currentRailIndex]);
                    railList[currentRailIndex].figuresLayer2.Add(newCopyOfFigure);
                    railList[currentRailIndex].sizeLayering = 2;
                    railList[currentRailIndex].myObjectsPositionListLayer2.Add(oP);
                    railList[currentRailIndex].myObjectsPositionListLayer2 = railList[currentRailIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                }

            }
        }
        else
        {
            #region limit front and end of rail
            if (newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x < 0)        //newCopyOfFigure.transform.position.x - 50 < 0.03f * Screen.width)  // 50 is half the box Collider width (mouse pos is in the middle of the figure) erstmal standard 50, dann wird erst calkuliert, welche groesse am zeitpunkt korrekt ist 
            {
                Debug.Log("hi");
                newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3((50), newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
            }
            // back of rail
            else if (newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x + 100 > railwidthAbsolute)
            {
                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
                //Debug.Log("railwidth: "+railwidthAbsolute+", rail 0: "+rails[0].GetComponent<RectTransform>().rect.width);

                newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector2(rails[0].GetComponent<RectTransform>().rect.width - 50, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y);
                //Debug.Log("rect width: " + rails[0].GetComponent<RectTransform>().rect.width + ", sizedelta: " + 50);
            }
            else
            {
                newCopyOfFigure.transform.position = new Vector3(momentOrPosX, figureObjects[figureNr].transform.position.y, -1);
            }
            #endregion

            float moment = UtilitiesTm.FloatRemap((newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x - 50), 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
            objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
            rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);
            if (float.IsInfinity(objectAnimationLength))
            {
                objectAnimationLength = 100;
            }
            createRectangle(newCopyOfFigure, colFigure, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);
            Destroy(newCopyOfFigure.transform.GetChild(2).gameObject);

            #region calculating Layer Overlap
            scaleToLayerSize(newCopyOfFigure, 0, rails[currentRailIndex]);
            oP.position = new Vector2(newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x, newCopyOfFigure.GetComponent<RectTransform>().sizeDelta.x);


            if (railList[currentRailIndex].sizeLayering == 1)
            {
                if (isSomethingOverlapping(currentRailIndex))
                {
                    railList[currentRailIndex].sizeLayering = 2;
                    //Debug.Log("something overlapping");
                    railList[currentRailIndex].figuresLayer2.Add(newCopyOfFigure);
                    scaleToLayerSize(newCopyOfFigure, 2, rails[currentRailIndex]);
                    railList[currentRailIndex].myObjectsPositionListLayer2.Add(oP);
                    railList[currentRailIndex].myObjectsPositionListLayer2 = railList[currentRailIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();

                    // others to 1
                    for (int i = 0; i < railList[currentRailIndex].figuresLayer1.Count; i++)
                    {
                        scaleToLayerSize(railList[currentRailIndex].figuresLayer1[i], 1, rails[currentRailIndex]);
                    }
                }
                else
                {
                    railList[currentRailIndex].figuresLayer1.Add(newCopyOfFigure);
                    railList[currentRailIndex].myObjectsPositionListLayer1.Add(oP);
                    //sortVectorValues(railList[currentRailIndex].myObjectsPositionListLayer1, railList[currentRailIndex].myObjectsPositionListLayer1.Count - 1);
                    railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                    // for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer1[i].position.x);
                    // }
                }
            }
            else
            {
                Debug.Log("figure: " + newCopyOfFigure + ", railIndex: " + currentRailIndex);
                LookForFreeSpot(newCopyOfFigure, currentRailIndex, true);
                oP.position = new Vector2(newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x, newCopyOfFigure.GetComponent<RectTransform>().sizeDelta.x);
                if (railList[currentRailIndex].figuresLayer1.Contains(newCopyOfFigure))
                {
                    railList[currentRailIndex].myObjectsPositionListLayer1.Add(oP);
                    railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                    // for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer1[i].position.x);
                    // }
                    scaleToLayerSize(newCopyOfFigure, 1, rails[currentRailIndex]);
                }
                else
                {
                    railList[currentRailIndex].myObjectsPositionListLayer2.Add(oP);
                    railList[currentRailIndex].myObjectsPositionListLayer2 = railList[currentRailIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                    // for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
                    // {
                    //     Debug.Log("2 vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer2[i].position.x);
                    // }
                    scaleToLayerSize(newCopyOfFigure, 2, rails[currentRailIndex]);
                }
            }
            #endregion
        }
        //set 3d object to default position
        curr3DObject = Instantiate(figureObjects3D[figureNr]);
        railList[currentRailIndex].timelineInstanceObjects3D.Add(curr3DObject);
        railList[currentRailIndex].timelineInstanceObjects3D[railList[currentRailIndex].timelineInstanceObjects3D.Count - 1].transform.SetParent(rails3D[currentRailIndex].transform.GetChild(0));

        //set original image back to shelf, position 2 to make it visible
        figureObjects[figureNr].transform.SetParent(objectShelfParent[figureNr].transform);
        figureObjects[figureNr].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
        figureObjects[figureNr].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
        figureObjects[figureNr].transform.SetSiblingIndex(1);
        figureObjects[figureNr].transform.GetChild(1).gameObject.SetActive(false);
        return curr3DObject;
    }
    public void ResetScreenSize()       // this probably has to be called globally, so that every Menue resizes (probably in the UIController). At the moment it is only scaled properly when rail tab is open e.g.
    {
        // // I used the values that were put in FullHD (global: position.x) and calculated the percentage so that it works for all resolutions
        minX = 0.087f * Screen.width;               //timeline-rail-minX
        railWidth = 0.87f * Screen.width;           //railwidth=1670.4px
        heightClosed = heightRailPercent * Screen.height;
        heightOpened = 0.074f * Screen.height;
        maxX = minX + railWidth;
        //timeline-rail-maxX
        screenDifference = new Vector2(1920.0f / (float)Screen.width, 1080.0f / (float)Screen.height);

        //liveView.GetComponent<BoxCollider2D>().size = liveView.GetComponent<RectTransform>().sizeDelta;

        for (int i = 0; i < railList.Length; i++)
        {
            if (railList[i].isTimelineOpen)
            {
                rails[i].GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = rails[i].GetComponent<BoxCollider2D>().size = new Vector2(1670.4f, 80);
            }
            else
            {
                rails[i].GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(1670.4f, 20);
                rails[i].GetComponent<BoxCollider2D>().size = new Vector2(1670.4f, 20);
            }
        }
    }
    public bool isSomethingOverlapping(int index)
    {
        bool val = false;
        for (int i = 0; i < railList[index].timelineInstanceObjects.Count; i++)
        {
            for (int j = 0; j < railList[index].timelineInstanceObjects.Count; j++)
            {
                if (railList[index].timelineInstanceObjects[j] != railList[index].timelineInstanceObjects[i] && (railList[index].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x >= railList[index].timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x
                && (railList[index].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x <= (railList[index].timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x + railList[index].timelineInstanceObjects[i].GetComponent<BoxCollider2D>().size.x))
                || ((railList[index].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x + railList[index].timelineInstanceObjects[j].GetComponent<BoxCollider2D>().size.x) >= railList[index].timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x)
                && railList[index].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x <= railList[index].timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x))
                {
                    val = true;
                }
            }
        }
        return val;
    }
    private void LookForFreeSpot(GameObject obj, int railIndex, bool create)
    {
        int layer1Full = -1;
        int layer2Full = -1;
        Debug.Log("count 1: " + railList[railIndex].myObjectsPositionListLayer1.Count + ", count 2: " + railList[railIndex].myObjectsPositionListLayer2.Count);
        for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
            // wenn linker rand des aktuellen obj weiter links ist als rechter rand des collisionsobjekts
            if (obj.GetComponent<RectTransform>().anchoredPosition.x + obj.GetComponent<RectTransform>().sizeDelta.x > railList[railIndex].myObjectsPositionListLayer1[i].position.x
            && railList[railIndex].myObjectsPositionListLayer1[i].position.x + railList[railIndex].myObjectsPositionListLayer1[i].position.y > obj.GetComponent<RectTransform>().anchoredPosition.x
            && railList[railIndex].myObjectsPositionListLayer1[i].objName != obj.name)
            {
                Debug.Log("layer1full: " + i);
                layer1Full = i;
            }
        for (int j = 0; j < railList[railIndex].myObjectsPositionListLayer2.Count; j++)
            if (obj.GetComponent<RectTransform>().anchoredPosition.x + obj.GetComponent<RectTransform>().sizeDelta.x > railList[railIndex].myObjectsPositionListLayer2[j].position.x
            && railList[railIndex].myObjectsPositionListLayer2[j].position.x + railList[railIndex].myObjectsPositionListLayer2[j].position.y > obj.GetComponent<RectTransform>().anchoredPosition.x
            && railList[railIndex].myObjectsPositionListLayer2[j].objName != obj.name)
            {
                Debug.Log("layer2Full: " + j);
                layer2Full = j;
            }

        // wenn alles voll ist
        if (layer2Full != -1 && layer1Full != -1)
        {
            Debug.Log("1: " + railList[railIndex].myObjectsPositionListLayer1[layer1Full].position + ", 2: " + railList[railIndex].myObjectsPositionListLayer2[layer2Full].position);
            Debug.Log("length: " + railIndex + ", layer1: " + layer1Full + ", layer2: " + layer2Full);
            // obj auf layer 1 ist weiter links
            if (railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x > railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x)
            {
                Debug.Log("obj auf layer 1 ist weiter links: mouse: " + Input.mousePosition.x + ", obj: " + railList[railIndex].figuresLayer1[layer1Full].transform.position.x);
                // obj ist links von collision
                if (Input.mousePosition.x < railList[railIndex].figuresLayer1[layer1Full].transform.position.x)
                {
                    Debug.Log("left");
                    FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "left");
                }
                // obj ist rechts von collision
                else
                {
                    Debug.Log("right");
                    FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "right");
                }
            }
            // obj auf layer 1 ist weiter rechts
            else
            {
                Debug.Log("springst du hier hin? ");
                // obj ist links von collision
                if (Input.mousePosition.x < railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x)
                {
                    Debug.Log("else left");
                    FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "left");
                }
                // obj ist rechts von collision
                else
                {
                    Debug.Log("else right");
                    FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "right");
                }
            }
        }
        // wenn nur layer 2 frei ist
        else if (layer1Full != -1)
        {
            //Debug.Log("nur layer 2 frei!");
            if (railList[railIndex].figuresLayer1.Contains(obj))
            {
                scaleToLayerSize(obj, 2, rails[railIndex]);
                railList[railIndex].figuresLayer1.Remove(obj);
                railList[railIndex].figuresLayer2.Add(obj);

                UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer1, railList[railIndex].myObjectsPositionListLayer2);


                // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer2.Count; i++)
                // {
                //     Debug.Log("2 vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer2[i].position.x);
                // }
            }
            railList[railIndex].myObjectsPositionListLayer2 = railList[railIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
        }
        // wenn nur layer 1 frei ist
        else if (layer2Full != -1)
        {
            //Debug.Log("nur layer 1 frei!");
            if (create)
            {
                scaleToLayerSize(obj, 1, rails[railIndex]);
                railList[railIndex].figuresLayer1.Add(obj);
            }
            else
            {
                scaleToLayerSize(obj, 1, rails[railIndex]);
                if (railList[railIndex].figuresLayer2.Contains(obj))
                {
                    railList[railIndex].figuresLayer2.Remove(obj);
                    railList[railIndex].figuresLayer1.Add(obj);

                    UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);


                    // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("1 vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
                    // }
                }
                railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
            }
        }
        else    // alles frei
        {
            if (create)
            {
                scaleToLayerSize(obj, 1, rails[railIndex]);
                railList[railIndex].figuresLayer1.Add(obj);
            }
            else
            {
                Debug.Log("alles frei");
                scaleToLayerSize(obj, 1, rails[railIndex]);
                if (railList[railIndex].figuresLayer2.Contains(obj))
                {
                    railList[railIndex].figuresLayer2.Remove(obj);
                    railList[railIndex].figuresLayer1.Add(obj);

                    UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);

                    // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("1 vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
                    // }
                }
                railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
            }
        }
    }
    private void FindNextFreeSpot(GameObject obj, float posXIn1, float posXIn2, int railIndex, bool create, string direction)
    {
        float foundIndexLayer1;
        float foundIndexLayer2;

        // mouse ist links von collision
        if (direction == "left")
        {
            foundIndexLayer1 = posXIn1;//- obj.GetComponent<RectTransform>().sizeDelta.x;
            foundIndexLayer2 = posXIn2;//- obj.GetComponent<RectTransform>().sizeDelta.x;
            //Debug.Log("ind 1: " + foundIndexLayer1 + ", ind 2: " + foundIndexLayer2);

            //erstmal layer 1
            for (int i = railList[railIndex].myObjectsPositionListLayer1.Count - 1; i >= 0; i--)
            {
                //Debug.Log("name: " + railList[railIndex].myObjectsPositionListLayer1[i].objName + ", pos: " + (railList[railIndex].myObjectsPositionListLayer1[i].position.x + railList[railIndex].myObjectsPositionListLayer1[i].position.y) + ", foundindex: " + foundIndexLayer1);// - obj.GetComponent<RectTransform>().sizeDelta.x));
                // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt -> dann wird die Stelle davor als gefunden genommen
                if (railList[railIndex].myObjectsPositionListLayer1[i].position.x + railList[railIndex].myObjectsPositionListLayer1[i].position.y >= foundIndexLayer1 - obj.GetComponent<RectTransform>().sizeDelta.x
                && railList[railIndex].myObjectsPositionListLayer1[i].position.x <= foundIndexLayer1
                && railList[railIndex].myObjectsPositionListLayer1[i].objName != obj.name)
                {
                    foundIndexLayer1 = railList[railIndex].myObjectsPositionListLayer1[i].position.x;//- obj.GetComponent<RectTransform>().sizeDelta.x;
                    //Debug.Log("name 1: " + railList[railIndex].myObjectsPositionListLayer1[i].objName);
                }
            }
            // dann layer 2
            for (int j = railList[railIndex].myObjectsPositionListLayer2.Count - 1; j >= 0; j--)
            {
                //Debug.Log("name: " + railList[railIndex].myObjectsPositionListLayer2[j].objName + ", pos: " + (railList[railIndex].myObjectsPositionListLayer2[j].position.x + railList[railIndex].myObjectsPositionListLayer2[j].position.y) + ", foundindex: " + foundIndexLayer2);// - obj.GetComponent<RectTransform>().sizeDelta.x));
                // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt -> dann wird die Stelle davor als gefunden genommen
                if (railList[railIndex].myObjectsPositionListLayer2[j].position.x + railList[railIndex].myObjectsPositionListLayer2[j].position.y >= foundIndexLayer2 - obj.GetComponent<RectTransform>().sizeDelta.x
                && railList[railIndex].myObjectsPositionListLayer2[j].position.x <= foundIndexLayer2
                && railList[railIndex].myObjectsPositionListLayer2[j].objName != obj.name)
                {
                    foundIndexLayer2 = railList[railIndex].myObjectsPositionListLayer2[j].position.x;//- obj.GetComponent<RectTransform>().sizeDelta.x;
                    //Debug.Log("name 2: " + railList[railIndex].myObjectsPositionListLayer2[j].objName);
                }
            }
            // liegt platz auf layer1 naeher dran
            if (foundIndexLayer1 > foundIndexLayer2)
            {
                // wenn der rand im weg ist
                if (foundIndexLayer1 - obj.GetComponent<RectTransform>().sizeDelta.x - obj.GetComponent<RectTransform>().sizeDelta.x / 2 < 0)
                {
                    if (create)
                    {
                        Debug.Log("deleeeete: " + currentClickedObjectIndex);
                        //removeObjectFromTimeline2D(obj, true);
                        _toBeRemoved = true;
                    }
                }

                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(foundIndexLayer1 - obj.GetComponent<RectTransform>().sizeDelta.x, obj.GetComponent<RectTransform>().anchoredPosition.y);
                if (!railList[railIndex].figuresLayer1.Contains(obj))
                {
                    scaleToLayerSize(obj, 1, rails[railIndex]);
                    railList[railIndex].figuresLayer1.Add(obj);
                    railList[railIndex].figuresLayer2.Remove(obj);
                    UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);
                }

                railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();

                // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
                // {
                //     //Debug.Log("1 vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
                // }

            }
            // liegt platz auf layer2 naeher dran
            else
            {
                // abfrage, wenn der naechste freie platz ausserhalb der schiene waere
                if (foundIndexLayer2 - obj.GetComponent<RectTransform>().sizeDelta.x - obj.GetComponent<RectTransform>().sizeDelta.x / 2 < 0)
                {
                    Debug.Log("deleeeete: " + currentClickedObjectIndex);
                    //removeObjectFromTimeline2D(obj, true);
                    _toBeRemoved = true;
                }
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(foundIndexLayer2 - obj.GetComponent<RectTransform>().sizeDelta.x, obj.GetComponent<RectTransform>().anchoredPosition.y);
                if (!railList[railIndex].figuresLayer2.Contains(obj))
                {
                    scaleToLayerSize(obj, 2, rails[railIndex]);
                    railList[railIndex].figuresLayer2.Add(obj);
                    railList[railIndex].figuresLayer1.Remove(obj);
                    UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer1, railList[railIndex].myObjectsPositionListLayer2);
                }
                railList[railIndex].myObjectsPositionListLayer2 = railList[railIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();

                // for (int k = 0; k < railList[railIndex].myObjectsPositionListLayer2.Count; k++)
                // {
                //     Debug.Log("2 vector " + k + ": " + railList[railIndex].myObjectsPositionListLayer2[k].position);
                // }

            }
        }
        // mouse ist rechts von collision
        else
        {
            foundIndexLayer1 = posXIn1;
            foundIndexLayer2 = posXIn2;

            //erstmal layer 1
            for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
            {
                // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt
                if (railList[railIndex].myObjectsPositionListLayer1[i].position.x <= foundIndexLayer1 + obj.GetComponent<RectTransform>().sizeDelta.x + obj.GetComponent<RectTransform>().sizeDelta.x
                && railList[railIndex].myObjectsPositionListLayer1[i].position.x >= foundIndexLayer1
                && railList[railIndex].myObjectsPositionListLayer1[i].objName != obj.name)
                {
                    foundIndexLayer1 = railList[railIndex].myObjectsPositionListLayer1[i].position.x;
                }
            }
            // dann layer 2
            for (int j = 0; j < railList[railIndex].myObjectsPositionListLayer2.Count; j++)
            {
                // wenn der Beginn des naechsten objekts so liegt, DASS aktuelles objekt dazwischen passt
                if (railList[railIndex].myObjectsPositionListLayer2[j].position.x <= foundIndexLayer2 + obj.GetComponent<RectTransform>().sizeDelta.x + obj.GetComponent<RectTransform>().sizeDelta.x
                && railList[railIndex].myObjectsPositionListLayer2[j].position.x >= foundIndexLayer2
                && railList[railIndex].myObjectsPositionListLayer2[j].objName != obj.name)
                {
                    foundIndexLayer2 = railList[railIndex].myObjectsPositionListLayer2[j].position.x;
                }
            }

            // liegt platz auf layer1 naeher dran
            if (foundIndexLayer1 > foundIndexLayer2)
            {
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(foundIndexLayer2 + obj.GetComponent<RectTransform>().sizeDelta.x, obj.GetComponent<RectTransform>().anchoredPosition.y);
                if (!railList[railIndex].figuresLayer2.Contains(obj))
                {
                    scaleToLayerSize(obj, 2, rails[railIndex]);
                    railList[railIndex].figuresLayer2.Add(obj);
                    railList[railIndex].figuresLayer1.Remove(obj);
                    UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer1, railList[railIndex].myObjectsPositionListLayer2);
                    // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer2.Count; i++)
                    // {
                    //     Debug.Log("layer 1 naeher vector! " + i + ": " + railList[railIndex].myObjectsPositionListLayer2[i].position.x);
                    // }
                }
                railList[railIndex].myObjectsPositionListLayer2 = railList[railIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();

            }
            else
            {
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(foundIndexLayer1 + obj.GetComponent<RectTransform>().sizeDelta.x, obj.GetComponent<RectTransform>().anchoredPosition.y);
                if (!railList[railIndex].figuresLayer1.Contains(obj))
                {
                    scaleToLayerSize(obj, 1, rails[railIndex]);
                    railList[railIndex].figuresLayer1.Add(obj);
                    railList[railIndex].figuresLayer2.Remove(obj);
                    UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);
                    // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("layer 2 naeher! vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
                    // }
                }
                railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
            }
        }
    }
    void UpdatePositionVectorInformation(GameObject currentObj, List<ObjectsPoint> listFrom, List<ObjectsPoint> listTo)
    {
        for (int i = 0; i < listFrom.Count; i++)
        {
            if (listFrom[i].objName == currentObj.name)
            {
                listFrom[i].position = new Vector2(currentObj.GetComponent<RectTransform>().anchoredPosition.x, currentObj.GetComponent<RectTransform>().sizeDelta.x);
                listTo.Add(listFrom[i]);
                listFrom.Remove(listFrom[i]);
            }
        }
    }
    void holdMinimumDistance(GameObject currentObj, float minimumDistance)
    {
        for (int j = 0; j < railList[currentRailIndex].timelineInstanceObjects.Count; j++)
        {
            if (currentObj != railList[currentRailIndex].timelineInstanceObjects[j]
            && currentObj.GetComponent<RectTransform>().anchoredPosition.x < railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x + minimumDistance
            && currentObj.GetComponent<RectTransform>().anchoredPosition.x >= railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x)
            {
                currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x + minimumDistance, railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.y);
            }
            else if (currentObj != railList[currentRailIndex].timelineInstanceObjects[j]
            && currentObj.GetComponent<RectTransform>().anchoredPosition.x < railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x
            && currentObj.GetComponent<RectTransform>().anchoredPosition.x >= railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x - minimumDistance)
            {
                currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x - minimumDistance, railList[currentRailIndex].timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
    }
    public void updateDataWhenOpeningNewScene()
    {
        for (int i = 0; i < railList.Length; i++)
        {
            railList[i].myObjectsPositionListLayer1.Clear();
            railList[i].myObjectsPositionListLayer2.Clear();

            foreach (GameObject figure in railList[i].figuresLayer1)
            {
                ObjectsPoint oP = new ObjectsPoint();
                oP.objName = figure.name;
                oP.position = new Vector2(figure.GetComponent<RectTransform>().anchoredPosition.x, figure.GetComponent<RectTransform>().sizeDelta.x);
                railList[i].myObjectsPositionListLayer1.Add(oP);
                railList[i].myObjectsPositionListLayer1 = railList[i].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
            }
            foreach (GameObject figure in railList[i].figuresLayer2)
            {
                ObjectsPoint oP = new ObjectsPoint();
                oP.objName = figure.name;
                oP.position = new Vector2(figure.GetComponent<RectTransform>().anchoredPosition.x, figure.GetComponent<RectTransform>().sizeDelta.x);
                railList[i].myObjectsPositionListLayer2.Add(oP);
                railList[i].myObjectsPositionListLayer2 = railList[i].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
            }
            Debug.Log("rail: " + i + ": Count: " + railList[i].myObjectsPositionListLayer1.Count);
        }
    }
    void Update()
    {
        if (currentLossyScale != transform.lossyScale.x)    // Abfrage, ob sich lossyScale geaendert hat dann werden die Schienen rescaled
        {
            currentLossyScale = transform.lossyScale.x;
            ResetScreenSize();
            // todo: resize all figures on timeline
        }
        Vector2 getMousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //identify which gameobject you clicked
            identifyClickedObject();         //method fills up the current clicked index
            identifyClickedObjectByList(railList[currentRailIndex].timelineInstanceObjects);
            editTimelineObject = false;                             //flag to prevent closing the timeline if you click an object in timeline
            releaseOnTimeline = false;                              //because you have not set on timeline anything
            releaseObjMousePos = new Vector2(0.0f, 0.0f);

            int tmpI = -1;
            for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects.Count; i++)
            {
                if (railList[currentRailIndex].timelineInstanceObjects[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                    tmpI = i;
            }
            if (tmpI >= 0)
                editTimelineObject = true;
            else
                editTimelineObject = false;
            //if you click the timeline with the mouse
            for (int i = 0; i < rails.Length; i++)
                if (rails[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //open or close timeline
                    openTimelineByClick(railList[i].isTimelineOpen, i, false);
                    currentRailIndex = i;
                }
            //if you click on an object in shelf
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                // default rect erstellen
                //wenn neues objekt genommen wird, soll altes aktuelles objekt unhighlighted werden
                for (int j = 0; j < railList[currentRailIndex].timelineInstanceObjects.Count; j++)
                {
                    highlight(railList[currentRailIndex].timelineInstanceObjects3D[j], railList[currentRailIndex].timelineInstanceObjects[j], false);
                }
                if (!helpButton.GetComponent<PressHelp>().arrowPressed)
                {
                    if (_counterTouched < 2)
                        _counterTouched++;
                    else if (_counterTouched == 2)
                    {
                        _timerFigure = 0.0f;
                        //_counterTouched = 0;
                    }
                }
                else
                    _timerFigure = 1;

                if (!SceneManaging.sceneChanged) SceneManaging.sceneChanged = true;
                diff = new Vector2(getMousePos.x - figureObjects[currentClickedObjectIndex].transform.position.x, getMousePos.y - figureObjects[currentClickedObjectIndex].transform.position.y);
                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    draggingObject = true;
                    SceneManaging.dragging = true;
                }
            }

            if (currentClickedInstanceObjectIndex != -1)
            {
                if (railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).gameObject.activeSelf
                    && getMousePos.x >= railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).position.x - railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x / 2.5f && getMousePos.x <= railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).position.x + railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x / 2.5f
                    && getMousePos.y >= railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).position.y - railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.y / 2.5f && getMousePos.y <= railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).position.y + railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.y / 2.5f)
                {
                    _toBeRemoved = true;
                }
            }

            //or check if you click an object in timeline
            if (currentClickedInstanceObjectIndex != -1 && editTimelineObject && railList[currentRailIndex].isTimelineOpen)
            {
                if (!SceneManaging.sceneChanged) SceneManaging.sceneChanged = true;

                if (railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    diff = new Vector2(getMousePos.x - railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, getMousePos.y - railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.y);
                    draggingOnTimeline = true;
                    SceneManaging.dragging = true;

                    //highlighting objects and showing delete button when clicked
                    if (SceneManaging.highlighted == false)
                    {
                        highlight(railList[currentRailIndex].timelineInstanceObjects3D[currentClickedInstanceObjectIndex], railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], true);
                    }
                    else if (SceneManaging.highlighted && railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<Image>().color == colFigureHighlighted) // checkFigureHighlighted(timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject))  // check if second child (which is never the armature) has emission enabled (=is highlighted)
                    {
                        // highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex], false);
                    }
                    else
                    {
                        for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects3D.Count; i++)
                        {
                            highlight(railList[currentRailIndex].timelineInstanceObjects3D[i], railList[currentRailIndex].timelineInstanceObjects[i], false);
                        }
                        highlight(railList[currentRailIndex].timelineInstanceObjects3D[currentClickedInstanceObjectIndex], railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], true);
                    }
                }
            }

            //if you hit/clicked nothing with mouse
            if (Physics2D.OverlapPoint(getMousePos) == false)
            {
                // delete highlight of everything if nothing is clicked
                for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects3D.Count; i++)
                {
                    highlight(railList[currentRailIndex].timelineInstanceObjects3D[i], railList[currentRailIndex].timelineInstanceObjects[i], false);
                }
            }
        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline)
        {
            if (currentClickedInstanceObjectIndex != -1)  //is set in the identify-methods
            {
                GameObject currentObj = railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex];
                isInstance = true;
                //if you click an object in timeline (for dragging)
                updateObjectPosition(currentObj, getMousePos - diff);
                setObjectOnTimeline(currentObj, rails[currentRailIndex].transform.position.y); //snapping/lock y-axis
                float currentPos = currentObj.GetComponent<RectTransform>().anchoredPosition.x;
                float currentSize = currentObj.GetComponent<RectTransform>().sizeDelta.x;

                #region Stapeln von Ebenen
                // if there is only one layer on Rail
                if (railList[currentRailIndex].sizeLayering == 1)
                {
                    if (isCurrentFigureOverlapping(currentRailIndex, currentObj))
                    {
                        scaleToLayerSize(currentObj, 2, rails[currentRailIndex]);

                        railList[currentRailIndex].figuresLayer1.Remove(currentObj);
                        railList[currentRailIndex].figuresLayer2.Add(currentObj);

                        UpdatePositionVectorInformation(currentObj, railList[currentRailIndex].myObjectsPositionListLayer1, railList[currentRailIndex].myObjectsPositionListLayer2);
                        railList[currentRailIndex].myObjectsPositionListLayer2 = railList[currentRailIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                        for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
                        {
                            Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer2[i].position.x);
                        }

                        // scale all other timelineobjects of layer 1
                        for (int j = 0; j < railList[currentRailIndex].figuresLayer1.Count; j++)
                        {
                            if (railList[currentRailIndex].figuresLayer1[j] != currentObj) scaleToLayerSize(railList[currentRailIndex].figuresLayer1[j], 1, rails[currentRailIndex]);
                        }

                        railList[currentRailIndex].sizeLayering = 2;
                    }
                }

                // if there are two layers on Rail
                else if (railList[currentRailIndex].sizeLayering == 2)
                {
                    LookForFreeSpot(currentObj, currentRailIndex, false);
                    // wenn nichts mehr überlappt
                    if (!isSomethingOverlapping(currentRailIndex))
                    {
                        railList[currentRailIndex].sizeLayering = 1;
                        // scale all other timelineobjects of layer 0
                        for (int j = 0; j < railList[currentRailIndex].timelineInstanceObjects.Count; j++)
                        {
                            scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[j], 0, rails[currentRailIndex]);
                            if (railList[currentRailIndex].figuresLayer2.Contains(railList[currentRailIndex].timelineInstanceObjects[j]))
                            {
                                railList[currentRailIndex].figuresLayer2.Remove(railList[currentRailIndex].timelineInstanceObjects[j]);
                                railList[currentRailIndex].figuresLayer1.Add(railList[currentRailIndex].timelineInstanceObjects[j]);
                            }
                        }
                        //UpdatePositionVectorInformation(currentObj, railList[currentRailIndex].myObjectsPositionListLayer2, railList[currentRailIndex].myObjectsPositionListLayer1);
                        for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
                        {
                            railList[currentRailIndex].myObjectsPositionListLayer2[i].position = new Vector2(currentObj.GetComponent<RectTransform>().anchoredPosition.x, currentObj.GetComponent<RectTransform>().sizeDelta.x);
                            railList[currentRailIndex].myObjectsPositionListLayer1.Add(railList[currentRailIndex].myObjectsPositionListLayer2[i]);
                            railList[currentRailIndex].myObjectsPositionListLayer2.Remove(railList[currentRailIndex].myObjectsPositionListLayer2[i]);
                        }
                        railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();

                        for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
                        {
                            Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer1[i].position.x);
                        }

                    }
                    // mindestabstand von 50 cm einhalten
                    holdMinimumDistance(currentObj, _spaceMax);
                }
                #endregion

                #region limits front and back of rail
                // -------------------------------------------------limit front of rail-------------------------------------------------------------------------//
                if (currentPos  <= 50)
                {
                    currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, currentObj.GetComponent<RectTransform>().anchoredPosition.y);
                }

                //-------------------------------------------------limit back of rail------------------------------------------------------------------------------//
                if ((currentPos + currentSize-50 ) > (rails[currentRailIndex].GetComponent<RectTransform>().rect.width))
                {
                    currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(rails[currentRailIndex].GetComponent<RectTransform>().rect.width-currentSize+50, currentObj.GetComponent<RectTransform>().anchoredPosition.y);
                }
                #endregion

                // wenn figur aus der schiene gezogen wird
                if (!(getMousePos.x > rails[currentRailIndex].transform.position.x && getMousePos.x < rails[currentRailIndex].transform.position.x + rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta.x / screenDifference.x
                && getMousePos.y < rails[currentRailIndex].transform.position.y + (rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta.y / screenDifference.y / 2) && getMousePos.y > rails[currentRailIndex].transform.position.y - (rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta.y / screenDifference.y / 2)))       // mouse outside
                {
                    draggingObject = true;
                    editTimelineObject = false;
                    draggingOnTimeline = false;

                    // delete button ausblenden
                    currentObj.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                    currentObj.GetComponent<RectTransform>().pivot = new Vector2(currentObj.GetComponent<RectTransform>().pivot.x, 0.5f);

                    //temporarily change parent, so that object appears in front of the shelf
                    currentObj.transform.SetParent(parentMenue.transform);
                    newHitTimeline = currentRailIndex;
                }
            }
            releaseOnTimeline = true;
        }
        //if something has been dragged outside of the timeline
        if (draggingOnTimeline == false && editTimelineObject == false && draggingObject && isInstance)
        {
            // moving on mouse pos
            railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position = new Vector3(getMousePos.x, getMousePos.y, -1.0f);

            hitTimeline = checkHittingTimeline(getMousePos);
            if (hitTimeline != -1)  // if a timeline is hit
            {
                if (hitTimelineOld != hitTimeline)
                {
                    hitTimelineOld = hitTimeline;
                    newHitTimeline = hitTimeline;
                    openTimelineByDrag(hitTimeline);

                    // scale down the dragged figure (and childobject: image)
                    scaleObject(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], figPictureSize, rails[newHitTimeline].GetComponent<RectTransform>().sizeDelta.y, false);
                }
                //snapping/lock y-axis
                setObjectOnTimeline(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], rails[newHitTimeline].transform.position.y);

                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
                changedRail = true;
            }

            // wenn zwischenraum zwischen schienen getroffen wird
            else if (isRailAreaHit(getMousePos))
            {
                //snapping/lock y-axis
                setObjectOnTimeline(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], rails[newHitTimeline].transform.position.y);

                railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[newHitTimeline].GetComponent<RectTransform>().rect.height);
                railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[newHitTimeline].GetComponent<RectTransform>().rect.height);
                railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject.SetActive(true);
                railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.GetComponent<Image>().color = colFigureAlpha; // rect alpha
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
                changedRail = true;
            }

            else // if no timeline is hit
            {
                hitTimelineOld = -1;
                //temporarily change parent, so that object appears in front of the shelf
                railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.SetParent(parentMenue.transform);
                //scale up the dragged figure and childobjects (button and rect)
                railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
                railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(false); // rect hide

                releaseOnTimeline = false;
                changedRail = false;
                newHitTimeline = -1;
            }
        }
        // dragging an object from shelf to timeline
        if (draggingObject && editTimelineObject == false && isInstance == false)
        {
            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            if (_timerFigure <= 0.5f)
            {
                _timerFigure += Time.deltaTime;
                //Debug.Log("timer: " + _timerFigure);
            }
            scrollRect.GetComponent<ScrollRect>().enabled = false;
            //move object
            updateObjectPosition(figureObjects[currentClickedObjectIndex], getMousePos - diff);

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction

            hitTimeline = checkHittingTimeline(getMousePos);

            if (hitTimeline != -1)
            {
                if (hitTimelineOld != hitTimeline)
                {
                    Debug.Log("hier");
                    hitTimelineOld = hitTimeline;
                    openTimelineByDrag(hitTimeline);

                    figureObjects[currentClickedObjectIndex].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[hitTimeline].GetComponent<RectTransform>().rect.height);
                    figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[hitTimeline].GetComponent<RectTransform>().rect.height);
                    // rect sichtbar machen
                    figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(true);
                    currentRailIndex = hitTimeline;
                }

                //snapping/lock y-axis
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], rails[currentRailIndex].transform.position.y);

                //save position, where object on timeline is released + set flag
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }
            // wenn zwischenraum zwischen schienen getroffen wird
            else if (isRailAreaHit(getMousePos))
            {
                //snapping/lock y-axis
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], rails[currentRailIndex].transform.position.y);

                figureObjects[currentClickedObjectIndex].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);
                figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(true);
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }

            else
            {
                //rect unscihtbar machen
                figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(false);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
                releaseOnTimeline = false;
                hitTimelineOld = -1;
            }
        }
        //-------release mousebutton
        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            scrollRect.GetComponent<ScrollRect>().enabled = true;
            draggingObject = false;
            editTimelineObject = false;
            hitTimelineOld = -1;

            if (_toBeRemoved)
            {
                removeObjectFromTimeline(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
            }
            // if dropped on a timeline
            else if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)
                {
                    if (changedRail)    // if rail has been changed
                    {
                        if (newHitTimeline != -1)
                        {
                            hitTimeline = newHitTimeline;
                        }
                        //2D object
                        railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.SetParent(rails[hitTimeline].transform);
                        railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(true);        // set rect active
                                                                                                                                                                       //3D object
                        railList[currentRailIndex].timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.SetParent(rails3D[currentRailIndex].transform.GetChild(0));

                        if (railList[hitTimeline].sizeLayering == 1)
                        {
                            // wenn figur überlappt muss sie auf layer 2
                            if (isCurrentFigureOverlapping(hitTimeline, railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                            {
                                railList[hitTimeline].sizeLayering = 2;
                                railList[hitTimeline].figuresLayer2.Add(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                                scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], 2, rails[hitTimeline]);

                                // wenn die figur vorher zu layer 1 gehoert hat
                                if (railList[currentRailIndex].figuresLayer1.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer2);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer2.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer2 = railList[hitTimeline].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                                    for (int k = 0; k < railList[hitTimeline].myObjectsPositionListLayer2.Count; k++)
                                    {
                                        Debug.Log("vector " + k + ": " + railList[hitTimeline].myObjectsPositionListLayer2[k].position);
                                    }
                                }
                                // wenn die figur vorher zu layer 2 gehoert hat
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer2);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer2.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer2 = railList[hitTimeline].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                                    for (int k = 0; k < railList[hitTimeline].myObjectsPositionListLayer2.Count; k++)
                                    {
                                        Debug.Log("vector " + k + ": " + railList[hitTimeline].myObjectsPositionListLayer2[k].position);
                                    }
                                }
                                // others to 1
                                for (int i = 0; i < railList[hitTimeline].figuresLayer1.Count; i++)
                                {
                                    if (railList[hitTimeline].figuresLayer1[i].name != railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].name)
                                        scaleToLayerSize(railList[hitTimeline].figuresLayer1[i], 1, rails[hitTimeline]);
                                }

                            }
                            //figur ueberlappt nicht und muss auf layer 1
                            else
                            {
                                railList[hitTimeline].figuresLayer1.Add(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                                scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], 0, rails[hitTimeline]);
                                // wenn die figur vorher zu layer 1 gehoert hat
                                if (railList[currentRailIndex].figuresLayer1.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer1);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer1.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer1 = railList[hitTimeline].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                                    for (int k = 0; k < railList[hitTimeline].myObjectsPositionListLayer1.Count; k++)
                                    {
                                        Debug.Log("vector " + k + ": " + railList[hitTimeline].myObjectsPositionListLayer1[k].position);
                                    }
                                }
                                // wenn die figur vorher zu layer 2 gehoert hat
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer1);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer1.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer1 = railList[hitTimeline].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                                    for (int k = 0; k < railList[hitTimeline].myObjectsPositionListLayer1.Count; k++)
                                    {
                                        Debug.Log("vector " + k + ": " + railList[hitTimeline].myObjectsPositionListLayer1[k].position);
                                    }
                                }
                            }
                        }

                        // wenn hittimeline sizerlayering 2 hat
                        else
                        {
                            LookForFreeSpot(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], hitTimeline, true);
                            // wenn obj layer 1 (hittimeline) zugeordnet wurde
                            if (railList[hitTimeline].figuresLayer1.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                            {
                                // wenn obj vorher in layer 1 war (currentRailIndex)
                                if (railList[currentRailIndex].figuresLayer1.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer1);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer1.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer1 = railList[hitTimeline].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                                }
                                // wenn obj vorher in layer 2 war (currentRailIndex)
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer1);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer1.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer1 = railList[hitTimeline].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                                }

                                scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], 1, rails[hitTimeline]);
                            }
                            // wenn obj layer 2 (hittimeline) zugeordnet wurde
                            else
                            {
                                scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], 2, rails[hitTimeline]);
                                // wenn obj vorher in layer 1 war (currentRailIndex)
                                if (railList[currentRailIndex].figuresLayer1.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer1, railList[hitTimeline].myObjectsPositionListLayer2);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer2.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer2 = railList[hitTimeline].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                                }
                                // wenn obj vorher in layer 2 war (currentRailIndex)
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer2);
                                    // sortVectorValues(railList[hitTimeline].myObjectsPositionListLayer2, railList[hitTimeline].myObjectsPositionListLayer2.Count - 1);
                                    railList[hitTimeline].myObjectsPositionListLayer2 = railList[hitTimeline].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                                }
                            }
                        }

                        //snapping/lock y-axis
                        setObjectOnTimeline(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], rails[hitTimeline].transform.position.y);

                        if (hitTimeline != currentRailIndex)
                        {
                            // alle objekte von figurenebenen der current rail loeschen
                            if (railList[currentRailIndex].figuresLayer2.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                                railList[currentRailIndex].figuresLayer2.Remove(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            else if (railList[currentRailIndex].figuresLayer1.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                                railList[currentRailIndex].figuresLayer1.Remove(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]);

                            //von timeline obj alt loeschen, zu neu hinzufuegen
                            updateObjectList(railList[hitTimeline].timelineInstanceObjects, railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            railList[currentRailIndex].timelineInstanceObjects.Remove(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]);

                            //3D object
                            railList[hitTimeline].timelineInstanceObjects3D.Add(railList[currentRailIndex].timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
                            railList[currentRailIndex].timelineInstanceObjects3D.Remove(railList[currentRailIndex].timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
                            StaticSceneData.StaticData.figureElements[Int32.Parse(railList[hitTimeline].timelineInstanceObjects[railList[hitTimeline].timelineInstanceObjects.Count - 1].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[hitTimeline].timelineInstanceObjects[railList[hitTimeline].timelineInstanceObjects.Count - 1].name.Substring(17))].railStart = hitTimeline;
                            railList[hitTimeline].timelineInstanceObjects3D[railList[hitTimeline].timelineInstanceObjects3D.Count - 1].transform.SetParent(rails3D[hitTimeline].transform.GetChild(0));

                            //wenn auf current rail nichts mehr ueberlappt
                            if (!isSomethingOverlapping(currentRailIndex))
                            {
                                railList[currentRailIndex].sizeLayering = 1;
                                // scale all other timelineobjects to layer 0
                                for (int j = 0; j < railList[currentRailIndex].timelineInstanceObjects.Count; j++)
                                {
                                    scaleToLayerSize(railList[currentRailIndex].timelineInstanceObjects[j], 0, rails[currentRailIndex]);
                                    if (railList[currentRailIndex].figuresLayer2.Contains(railList[currentRailIndex].timelineInstanceObjects[j]))
                                    {
                                        UpdatePositionVectorInformation(railList[currentRailIndex].timelineInstanceObjects[j], railList[currentRailIndex].myObjectsPositionListLayer2, railList[currentRailIndex].myObjectsPositionListLayer1);
                                        railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();

                                        railList[currentRailIndex].figuresLayer2.Remove(railList[currentRailIndex].timelineInstanceObjects[j]);
                                        railList[currentRailIndex].figuresLayer1.Add(railList[currentRailIndex].timelineInstanceObjects[j]);
                                    }
                                }
                            }

                            // box Collider disablen, damit Schiene auch auf Figuren geklickt werden kann
                            for (int i = 0; i < railList[currentRailIndex].timelineInstanceObjects.Count; i++)
                            {
                                railList[currentRailIndex].timelineInstanceObjects[i].GetComponent<BoxCollider2D>().enabled = false;
                            }
                            currentRailIndex = hitTimeline;
                        }
                        else
                        {
                            if (railList[currentRailIndex].figuresLayer1.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]) && railList[currentRailIndex].figuresLayer2.Contains(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                            {
                                Debug.Log("beide beinhalten figur");
                                railList[currentRailIndex].figuresLayer1.Remove(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            }
                        }

                        highlight(railList[hitTimeline].timelineInstanceObjects3D[railList[hitTimeline].timelineInstanceObjects3D.Count - 1], railList[hitTimeline].timelineInstanceObjects[railList[currentRailIndex].timelineInstanceObjects.Count - 1], true);
                        changedRail = false;
                    }
                    else
                    {
                        GameObject curr3DObject = CreateNew2DInstance(currentClickedObjectIndex, getMousePos.x, -1, -1);
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////
                        // Save to SceneData:
                        FigureInstanceElement thisFigureInstanceElement = new FigureInstanceElement();
                        thisFigureInstanceElement.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex]); //index
                        thisFigureInstanceElement.name = curr3DObject.name + "_" + countCopiesOfObject(figureObjects[currentClickedObjectIndex]).ToString("000");
                        thisFigureInstanceElement.railStart = (int)Char.GetNumericValue(rails[currentRailIndex].name[17]) - 1; //railIndex

                        StaticSceneData.StaticData.figureElements[currentClickedObjectIndex].figureInstanceElements.Add(thisFigureInstanceElement);
                        gameController.GetComponent<SceneDataController>().objects3dFigureInstances.Add(curr3DObject);
                        highlight(railList[currentRailIndex].timelineInstanceObjects3D[railList[currentRailIndex].timelineInstanceObjects3D.Count - 1], railList[currentRailIndex].timelineInstanceObjects[railList[currentRailIndex].timelineInstanceObjects.Count - 1], true);
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////
                        if (_toBeRemoved)
                        {
                            //Debug.Log("delete : " + railList[currentRailIndex].timelineInstanceObjects3D[railList[currentRailIndex].timelineInstanceObjects3D.Count - 1]);
                            removeObjectFromTimeline(railList[currentRailIndex].timelineInstanceObjects[railList[currentRailIndex].timelineInstanceObjects3D.Count - 1], railList[currentRailIndex].timelineInstanceObjects3D[railList[currentRailIndex].timelineInstanceObjects3D.Count - 1]);
                        }
                    }

                    currentClickedObjectIndex = -1; // set index back to -1 because nothing is being clicked anymore
                }

                // if figure has been moved in rail
                else
                {
                    // change position of vector in each case
                    for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjectsPositionListLayer1[i].objName == railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].name)
                            railList[currentRailIndex].myObjectsPositionListLayer1[i].position = new Vector2(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x, railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().sizeDelta.x);
                    }
                    for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjectsPositionListLayer2[i].objName == railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].name)
                            railList[currentRailIndex].myObjectsPositionListLayer2[i].position = new Vector2(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x, railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().sizeDelta.x);
                    }
                }
            }

            // if dropped somewhere else than timeline: bring figure back to shelf
            else if (releaseOnTimeline == false && currentClickedObjectIndex != -1 && isInstance == false)
            {
                // wenn in live view gedroppt
                if (figureObjects[currentClickedObjectIndex].transform.position.x > liveView.transform.position.x - liveView.GetComponent<RectTransform>().sizeDelta.x / 2
                && figureObjects[currentClickedObjectIndex].transform.position.x < liveView.transform.position.x + liveView.GetComponent<RectTransform>().sizeDelta.x / 2
                && figureObjects[currentClickedObjectIndex].transform.position.y > liveView.transform.position.y - liveView.GetComponent<RectTransform>().sizeDelta.y / 2
                && figureObjects[currentClickedObjectIndex].transform.position.y < liveView.transform.position.y + liveView.GetComponent<RectTransform>().sizeDelta.y / 2)
                {
                    //Debug.Log("hi");
                    helpButton.GetComponent<PressHelp>().helpTextLiveView.SetActive(true);
                }
                // wenn im schienenbereich, aber nicht auf schiene gedroppt
                // if (areaFiguresPut.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                // {
                //     Debug.Log("hier");
                // }
                // wenn figur nur angetoucht wurde
                else if (_timerFigure < 0.5f)
                {
                    helpButton.GetComponent<PressHelp>().arrowPressed = true;
                    //helpButton.GetComponent<PressHelp>().OnClick(0);
                    helpButton.GetComponent<PressHelp>()._arrowHelp.SetActive(true);
                    _counterTouched = 0;
                }

                figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                // bring object back to position two, so that its visible and change position back to default
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
            }

            // if instance is dropped somewhere else than on timeline: delete instance
            else if (releaseOnTimeline == false && currentClickedInstanceObjectIndex != -1 && isInstance)
                removeObjectFromTimeline(railList[currentRailIndex].timelineInstanceObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);


            for (int k = 0; k < railList.Length; k++)
            {
                for (int i = 0; i < railList[k].timelineInstanceObjects.Count; i++)
                {
                    float moment = UtilitiesTm.FloatRemap(railList[k].timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x - 50, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                    //Debug.Log("moment: " + moment);
                    float zPosFigure = (rails3D[k].transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime(moment));

                    if (zPosFigure < 0) // wenn die Figur auf eine Position noch vor dem empty gesetzt werden würde, würde sie erscheinen bevor sie auf den rails da ist
                    {
                        zPosFigure = 500f;      // deshalb wird die Figur komplett außerhalb der Szene abgesetzt.
                    }
                    if (rails3D[k].name.Substring(7) == "1" || rails3D[k].name.Substring(7) == "3" || rails3D[k].name.Substring(7) == "5")
                    {
                        railList[k].timelineInstanceObjects3D[i].transform.localPosition = new Vector3(-rails3D[k].transform.GetChild(0).transform.localPosition.x, (-rails3D[k].transform.GetChild(0).transform.localPosition.y - 0.01f), zPosFigure);
                    }
                    else
                    {
                        railList[k].timelineInstanceObjects3D[i].transform.localPosition = new Vector3(rails3D[k].transform.GetChild(0).transform.localPosition.x, (-rails3D[k].transform.GetChild(0).transform.localPosition.y - 0.01f), zPosFigure);
                    }

                    //this is for: (kris) damit die Figuren auch in die Richtung schauen, in die sie laufen
                    if (rails3D[k].transform.GetChild(0).GetComponent<RailSpeedController>().railIndex % 2 == 1)
                    {
                        railList[k].timelineInstanceObjects3D[i].transform.localEulerAngles = new Vector3(railList[k].timelineInstanceObjects3D[i].transform.localEulerAngles.x, 270, railList[k].timelineInstanceObjects3D[i].transform.localEulerAngles.z);
                        //Debug.Log("hello");
                    }
                    else
                    {
                        railList[k].timelineInstanceObjects3D[i].transform.localEulerAngles = new Vector3(railList[k].timelineInstanceObjects3D[i].transform.localEulerAngles.x, 270, railList[k].timelineInstanceObjects3D[i].transform.localEulerAngles.z);
                        //Debug.Log("hello");
                    }
                    StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(17))].moment = moment;
                    if (railList[k].figuresLayer1.Contains(railList[k].timelineInstanceObjects[i]))
                    {
                        if (railList[k].sizeLayering == 1)
                            StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(17))].layer = 0;
                        else
                            StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(17))].layer = 1;
                    }
                    else
                    {
                        StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].timelineInstanceObjects[i].name.Substring(17))].layer = 2;
                    }
                }
            }

            releaseOnTimeline = false;
            releaseObjMousePos.x = 0.0f;
            releaseObjMousePos.y = 0.0f;
            draggingOnTimeline = false;
            isInstance = false;
            //figureOnLiveView = false;
            currentClickedObjectIndex = -1;
            currentClickedInstanceObjectIndex = -1;
            hitTimelineOld = -1;
            SceneManaging.dragging = false;
            _toBeRemoved = false;
        }
        // enable binnenanimation when playing
        for (int k = 0; k < railList.Length; k++)
        {
            for (int i = 0; i < railList[k].timelineInstanceObjects3D.Count; i++)
            {
                // start Animation on play
                if (railList[k].timelineInstanceObjects3D[i].GetComponent<FigureStats>().isShip == false)
                {
                    if (SceneManaging.playing && !railList[k].timelineInstanceObjects3D[i].GetComponent<Animator>().enabled)
                    {
                        railList[k].timelineInstanceObjects3D[i].GetComponent<Animator>().enabled = true;
                    }
                    else if (!SceneManaging.playing && railList[k].timelineInstanceObjects3D[i].GetComponent<Animator>().enabled)
                    {
                        railList[k].timelineInstanceObjects3D[i].GetComponent<Animator>().enabled = false;
                    }
                }
            }
        }
    }
    public void PublicUpdate()
    {
        Update();
    }
}
