using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RailManager : MonoBehaviour
{
    #region public variables
    public GameObject[] rails = new GameObject[6];
    public Rail[] railList = new Rail[6];
    public GameObject[] rails3D = new GameObject[6];
    [SerializeField] private GameObject[] flyerSpaces = new GameObject[9];
    public TextMeshProUGUI[] figCounterCircle;
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
    float railWidth, railwidthAbsolute;
    Color colFigureAlpha = new Color(0.06f, 0.66f, .74f, 0.38f);
    Color colFigure = new Color(0.06f, 0.66f, .74f, 0.5f);
    Color colFigureHighlighted = new Color(0f, 0.87f, 1.0f, 0.5f);
    Color colFigureRed = new Color(1, 0, 0, 0.5f);
    private GameObject[] figureObjects, figureObjects3D;
    private int currentClickedObjectIndex, currentClickedInstanceObjectIndex, currentRailIndex, hitTimeline = -1, flyerHit = -1;
    private int count, maxTimeInSec;
    private float currentLossyScale;
    private float _timerFigure = 1;
    int newHitTimeline, currentSpaceActive = -1;
    private Vector2 diff;
    private FigureElement ThisFigureElement;    //element to set 3d object
    private float heightOpened, heightClosed, _spaceMax, _spaceMax3D;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, isInstance, changedRail, _toBeRemoved, _toBeRemovedFromTimeline;
    //private bool figureOnLiveView;
    int hitTimelineOld;
    private int _counterTouched = 0;
    #endregion
    #region Lists
    // Objects Position List
    public class Rail
    {
        public List<Figure> myObjects = new List<Figure>();
        public int sizeLayering;
        public bool isTimelineOpen;
    }
    [Serializable]
    public class Figure
    {
        public string objName;
        public Vector2 position;        // x ist x-Position (anchoredPosition.x) des objekts, y ist die länge des objekts (sizeDelta.x)
        public int layer;
        public int neighborLeft;
        public int neighborRight;
        public GameObject figure;
        public GameObject figure3D;
    }
    #endregion
    void Awake()
    {
        float format = (float)Screen.width / (float)Screen.height;
        if (format > 1.78f)
        {
            //Debug.Log("format: "+format);
            float widthNew = Screen.height * 1.77f;
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
        }
        currentRailIndex = 0;

        //load all objects given in the figuresShelf
        figureObjects = new GameObject[objectLibrary.transform.childCount];
        figureObjects3D = new GameObject[figureObjects.Length];

        objectShelfParent = new GameObject[figureObjects.Length];

        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = (objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject);

            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
        }
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i].text = "0";
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
    public string identifyClickedObject()  // from shelf
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
    public string identifyClickedObjectByList(List<Figure> objectsOnTimeline)       // from timeline
    {
        string objName = "";
        if (objectsOnTimeline.Count == 0)
            return "no object found! [-1]";             //error-handling if no clickable object found

        for (int i = 0; i < objectsOnTimeline.Count; i++)
        {
            //save last correct index
            int oldClickedObject = currentClickedInstanceObjectIndex;
            //which object in grid layout is clicked
            if (objectsOnTimeline[i].figure.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
            {
                //save the index of clicked object
                currentClickedInstanceObjectIndex = i;
                //if you click an new object
                if (oldClickedObject != currentClickedInstanceObjectIndex)
                {
                    //return objectsOnTimeline[i].name;
                }
                return objectsOnTimeline[i].figure.GetComponent<BoxCollider2D>().name;
            }
            else
            {
                objName = "no object clicked!";
            }
        }

        return objName;
    }
    private int isCurrentFigureOverlapping(int railIndex, Figure obj, string dir)
    {
        int val = -1;

        if (dir == "right")
        {
            for (int i = 0; i < railList[railIndex].myObjects.Count; i++)
            {
                //Debug.Log("name: " + railList[railIndex].myObjects[i].objName);
                if (((railList[railIndex].myObjects[i].position.x <= obj.position.x
                && railList[railIndex].myObjects[i].position.x + railList[railIndex].myObjects[i].position.y >= obj.position.x)
                || (railList[railIndex].myObjects[i].position.x >= obj.position.x
                && railList[railIndex].myObjects[i].position.x <= obj.position.x + obj.position.y))
                && railList[railIndex].myObjects[i].objName != obj.objName)
                {
                    val = i;
                    //Debug.Log("re name: " + railList[railIndex].myObjects[val].objName);
                    break;
                }
            }
        }
        else
        {
            for (int i = railList[railIndex].myObjects.Count - 1; i >= 0; i--)
            {
                if (((railList[railIndex].myObjects[i].position.x <= obj.position.x
                && railList[railIndex].myObjects[i].position.x + railList[railIndex].myObjects[i].position.y >= obj.position.x)
                || (railList[railIndex].myObjects[i].position.x >= obj.position.x
                && railList[railIndex].myObjects[i].position.x <= obj.position.x + obj.position.y))
                && railList[railIndex].myObjects[i].objName != obj.objName)
                {
                    val = i;
                    //Debug.Log("val: " + railList[railIndex].myObjects[val].objName);
                    break;
                }
            }
        }

        return val;
    }
    public void openTimelineByClick(bool thisTimelineOpen, int index, bool fromShelf)
    {
        //Debug.Log("erst");
        if (fromShelf == false && SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive != 1)
        {
            UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf05(true);
        }
        if (thisTimelineOpen)
        {
            // if timeline is already open, unhighlight all
            for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
            {
                highlight(railList[currentRailIndex].myObjects[i].figure3D, railList[currentRailIndex].myObjects[i].figure, false);
            }
        }
        else
        {
            UIController tmpUIController = gameController.GetComponent<UIController>();
            // a different rail is open - close it
            for (int i = 0; i < rails.Length; i++)
            {
                // wenn diese schiene offen ist
                if (railList[i].isTimelineOpen)
                {
                    rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                    rails[i].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                    railList[i].isTimelineOpen = false;
                    openCloseObjectInTimeline(false, railList[i].myObjects, i);

                    for (int j = 0; j < railList[i].myObjects.Count; j++)
                    {
                        highlight(railList[i].myObjects[j].figure3D, railList[i].myObjects[j].figure, false);
                    }
                }
            }
            // wenn bg oder licht schiene offen sind
            for (int j = 0; j < 2; j++)
            {
                if (tmpUIController.RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen)
                {
                    tmpUIController.RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                    tmpUIController.RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                    tmpUIController.RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
            }
            // wenn music-rail offen ist
            if (tmpUIController.RailMusic.GetComponent<RailMusicManager>().isTimelineOpen)
            {

                tmpUIController.RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                tmpUIController.RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);

                tmpUIController.RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                tmpUIController.RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, tmpUIController.RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects);

                for (int j = 0; j < tmpUIController.RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects.Count; j++)
                {
                    tmpUIController.RailMusic.GetComponent<RailMusicManager>().highlight(tmpUIController.RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects[j], false);
                }
            }

            // open clicked rail and collider
            rails[index].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 80);
            rails[index].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 80);
            openCloseObjectInTimeline(true, railList[index].myObjects, index);
            railList[index].isTimelineOpen = true;

            ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(rails[index].name[17]) - 1);
            ImageTimelineSelection.SetRailType(0);  // for rail-rails
        }
    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<Figure> objects, int railIndex)
    {
        float length = rectSize;

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].figure.GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].figure.GetComponent<RectTransform>().anchoredPosition.x, -rails[railIndex].GetComponent<RectTransform>().rect.height / 2, -1);
            //Debug.Log("height: " + rails[railIndex].GetComponent<RectTransform>().rect.height);

            if (timelineOpen)
            {
                if (railList[railIndex].sizeLayering == 1)
                {
                    scaleToLayerSize(objects[i].figure, 0, rails[railIndex]);
                }
                else if (railList[railIndex].sizeLayering == 2)
                {
                    if (objects[i].layer == 1) //railList[railIndex].myObjects.Contains(objects[i]))
                    {
                        scaleToLayerSize(objects[i].figure, 1, rails[railIndex]);
                        objects[i].figure.GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].figure.GetComponent<BoxCollider2D>().offset.x, rails[railIndex].GetComponent<RectTransform>().rect.height / 4);
                    }
                    else
                    {
                        scaleToLayerSize(objects[i].figure, 2, rails[railIndex]);
                        objects[i].figure.GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].figure.GetComponent<BoxCollider2D>().offset.x, -rails[railIndex].GetComponent<RectTransform>().rect.height / 4);
                    }
                }
            }
            else
            {
                if (railList[railIndex].sizeLayering == 1)
                {
                    scaleObject(objects[i].figure, length, 20, true);
                }
                else if (railList[railIndex].sizeLayering == 2)
                {
                    objects[i].figure.GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].figure.GetComponent<RectTransform>().anchoredPosition.x, -10, -1);

                    scaleObject(objects[i].figure, length, 10, true);
                }
            }
        }
    }
    public void openTimelineByDrag(int index)
    {
        if (!railList[index].isTimelineOpen)
        {
            UIController tmpUIController = gameController.GetComponent<UIController>();
            if (tmpUIController.RailLightBG[0].isTimelineOpen)
            {
                tmpUIController.RailLightBG[0].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                tmpUIController.RailLightBG[0].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                tmpUIController.RailLightBG[0].GetComponent<RailLightManager>().isTimelineOpen = false;
            }
            else if (tmpUIController.RailLightBG[1].isTimelineOpen)
            {
                tmpUIController.RailLightBG[1].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                tmpUIController.RailLightBG[1].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                tmpUIController.RailLightBG[1].GetComponent<RailLightManager>().isTimelineOpen = false;
            }
            else if (tmpUIController.RailMusic.isTimelineOpen)
            {
                tmpUIController.RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                tmpUIController.RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 20);
                tmpUIController.RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                tmpUIController.RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, tmpUIController.RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects);
            }
            for (int i = 0; i < rails.Length; i++)  // alle schienen schliessen
            {
                if (railList[i].isTimelineOpen)
                {
                    rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 20);
                    rails[i].GetComponent<BoxCollider2D>().size = new Vector2(rails[currentRailIndex].GetComponent<BoxCollider2D>().size.x, 20);
                    railList[i].isTimelineOpen = false;
                    openCloseObjectInTimeline(false, railList[i].myObjects, i);

                    for (int j = 0; j < railList[i].myObjects.Count; j++)
                    {
                        highlight(railList[i].myObjects[j].figure3D, railList[i].myObjects[j].figure, false);
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

            openCloseObjectInTimeline(true, railList[index].myObjects, index);
        }
    }
    public void updateObjectPosition(Figure obj, Vector2 mousePos)
    {
        obj.figure.transform.position = new Vector3(mousePos.x, mousePos.y, -1.0f);
        obj.position = new Vector2(obj.figure.GetComponent<RectTransform>().anchoredPosition.x, obj.figure.GetComponent<RectTransform>().sizeDelta.x);
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
    private int checkHittingFlyerSpace(Vector2 mousePos)
    {
        int hit = -1;
        for (int i = 0; i < flyerSpaces.Length; i++)
        {
            Vector2 colSize = new Vector2(flyerSpaces[i].GetComponent<BoxCollider2D>().size.x, flyerSpaces[i].GetComponent<BoxCollider2D>().size.y);
            //Debug.Log("colsize: "+colSize+", mousePos: "+mousePos);
            if (mousePos.x <= flyerSpaces[i].transform.position.x + (colSize.x / 2.0f) && mousePos.x > flyerSpaces[i].transform.position.x - (colSize.x / 2.0f) && mousePos.y <= flyerSpaces[i].transform.position.y + (colSize.y / 2.0f) && mousePos.y > flyerSpaces[i].transform.position.y - (colSize.y / 2.0f))
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
                //Debug.Log("scale to one");
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

        trans.SetSiblingIndex(0);
    }
    public int countCopiesOfObject(string fig)
    {
        int c = 0;
        for (int i = 0; i < rails.Length; i++)
        {
            //count object with the same name as fig
            foreach (Figure oP in railList[i].myObjects)
            {
                if (oP.objName.Contains(fig))
                {
                    c++;
                }
            }
        }
        return c;
    }
    public void removeObjectFromTimeline(Figure obj)
    {
        int tmpNr = int.Parse(obj.figure.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].text);

        // erase from layer list
        railList[currentRailIndex].myObjects.Remove(obj);

        figCounterCircle[tmpNr - 1].text = (currentCounterNr - 1).ToString();
        StaticSceneData.StaticData.figureElements[Int32.Parse(obj.objName.Substring(6, 2)) - 1].figureInstanceElements.Remove(StaticSceneData.StaticData.figureElements[Int32.Parse(obj.objName.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(obj.objName.Substring(17))]);

        Destroy(obj.figure);
        Destroy(obj.figure3D);

        if (!isSomethingOverlapping(currentRailIndex))
        {
            railList[currentRailIndex].sizeLayering = 1;
            for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
            {
                scaleToLayerSize(railList[currentRailIndex].myObjects[i].figure, 0, rails[currentRailIndex]);
            }
            railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(x => x.position.x).ToList();
        }
    }
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
    public GameObject CreateNew2DInstance(int figureNr, float momentOrPosX, int loadFromFile, int savedLayer, bool saveToSceneData)   // if layer = -1, then new instance, else layer has been saved
    {
        GameObject curr3DObject;
        if (loadFromFile != -1)
            currentRailIndex = loadFromFile;

        int countName = 0;
        countName = countCopiesOfObject(figureObjects[figureNr].name);

        if (gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
        {
            figCounterCircle[figureNr].text = (countName + 1).ToString();
        }

        newCopyOfFigure = Instantiate(figureObjects[figureNr]);
        newCopyOfFigure.name = figureObjects[figureNr].name + "instance" + countName.ToString("000");
        RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();

        //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
        //parent and position
        newCopyOfFigure.transform.SetParent(rails[currentRailIndex].transform);
        newCopyOfFigure.transform.localScale = Vector3.one;
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x - 50, newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y);

        Figure oP = new Figure();
        oP.objName = newCopyOfFigure.name;
        oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x);
        oP.figure = newCopyOfFigure;
        oP.neighborLeft = -1;
        oP.neighborRight = -1;
        railList[currentRailIndex].myObjects.Add(oP);

        // from scenedataController
        if (loadFromFile != -1)
        {
            float posX = (UtilitiesTm.FloatRemap(momentOrPosX, 0, AnimationTimer.GetMaxTime(), 0, railwidthAbsolute)) + 50;

            objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(momentOrPosX);
            rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);

            createRectangle(newCopyOfFigure, colFigure, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);

            if (savedLayer != -1)
            {
                tmpRectTransform.anchoredPosition = new Vector3(posX, -rails[currentRailIndex].GetComponent<RectTransform>().rect.height / 2, -1);
                oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, rectSize);

                //Debug.Log("neue figur: pos: " + oP.position.x);
                //Debug.Log("saved layer: " + savedLayer);

                if (savedLayer == 1)
                {
                    scaleToLayerSize(newCopyOfFigure, 1, rails[currentRailIndex]);
                    railList[currentRailIndex].sizeLayering = 2;
                    oP.layer = 1;

                }
                else if (savedLayer == 0)
                {
                    scaleToLayerSize(newCopyOfFigure, 0, rails[currentRailIndex]);
                    oP.layer = 1;
                }
                else
                {
                    scaleToLayerSize(newCopyOfFigure, 2, rails[currentRailIndex]);
                    railList[currentRailIndex].sizeLayering = 2;
                    oP.layer = 2;
                }
            }
        }
        else
        {
            #region limit front and end of rail
            if (tmpRectTransform.anchoredPosition.x < 50)        //newCopyOfFigure.transform.position.x - 50 < 0.03f * Screen.width)  // 50 is half the box Collider width (mouse pos is in the middle of the figure) erstmal standard 50, dann wird erst calkuliert, welche groesse am zeitpunkt korrekt ist 
            {
                //Debug.Log("front");
                tmpRectTransform.anchoredPosition = new Vector3(50, tmpRectTransform.anchoredPosition.y, -1);
                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
                oP.position = new Vector2(50, oP.position.y);

                float moment = UtilitiesTm.FloatRemap((tmpRectTransform.anchoredPosition.x - 50), 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());

                objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);

                rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);

                //Debug.Log("rect: " + rectSize);

                if (float.IsInfinity(objectAnimationLength))
                {
                    objectAnimationLength = 100;
                }
            }

            // back of rail
            else if (tmpRectTransform.anchoredPosition.x + rectSize > railwidthAbsolute)
            {
                float moment = UtilitiesTm.FloatRemap((tmpRectTransform.anchoredPosition.x - 50), 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());

                objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);

                rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);

                //Debug.Log("rect: " + rectSize);

                if (float.IsInfinity(objectAnimationLength))
                {
                    objectAnimationLength = 100;
                }

                //Debug.Log("back");
                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
                tmpRectTransform.anchoredPosition = new Vector2(railwidthAbsolute - rectSize + 50, tmpRectTransform.anchoredPosition.y);
                oP.position = new Vector2(railwidthAbsolute - rectSize + 50, oP.position.y);
            }
            else
            {
                float moment = UtilitiesTm.FloatRemap((tmpRectTransform.anchoredPosition.x - 50), 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());

                objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);

                rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);

                if (float.IsInfinity(objectAnimationLength))
                {
                    objectAnimationLength = 100;
                }

                //Debug.Log("else");
                newCopyOfFigure.transform.position = new Vector3(momentOrPosX, figureObjects[figureNr].transform.position.y, -1);
            }
            #endregion

            createRectangle(newCopyOfFigure, colFigure, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);
            Destroy(newCopyOfFigure.transform.GetChild(2).gameObject);

            #region calculating Layer Overlap
            scaleToLayerSize(newCopyOfFigure, 0, rails[currentRailIndex]);
            oP.layer = 1;
            oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x);

            if (railList[currentRailIndex].sizeLayering == 1)
            {
                if (isCurrentFigureOverlapping(currentRailIndex, oP, "") != -1)
                {
                    //Debug.Log("hier");
                    railList[currentRailIndex].sizeLayering = 2;
                    scaleToLayerSize(newCopyOfFigure, 2, rails[currentRailIndex]);
                    oP.layer = 2;
                    //Debug.Log("layer: " + oP.layer);

                    // others to 1
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].layer == 1)
                            scaleToLayerSize(railList[currentRailIndex].myObjects[i].figure, 1, rails[currentRailIndex]);
                    }
                }
            }
            else
            {
                FindFreeSpot(oP);
            }
            //railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();

            #endregion
        }

        //set 3d object to default position
        curr3DObject = Instantiate(figureObjects3D[figureNr]);
        //railList[currentRailIndex].timelineInstanceObjects3D.Add(curr3DObject);
        oP.figure3D = curr3DObject;
        oP.figure3D.transform.SetParent(rails3D[currentRailIndex].transform.GetChild(0));
        // railList[currentRailIndex].timelineInstanceObjects3D[railList[currentRailIndex].timelineInstanceObjects3D.Count - 1].transform.SetParent(rails3D[currentRailIndex].transform.GetChild(0));

        //set original image back to shelf, position 2 to make it visible
        figureObjects[figureNr].transform.SetParent(objectShelfParent[figureNr].transform);
        figureObjects[figureNr].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
        figureObjects[figureNr].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
        figureObjects[figureNr].transform.SetSiblingIndex(1);
        figureObjects[figureNr].transform.GetChild(1).gameObject.SetActive(false);

        holdMinimumDistance(oP, _spaceMax);

        if (saveToSceneData)
        {
            //tmpRectTransform.anchoredPosition = new Vector2(tmpRectTransform.anchoredPosition.x, -tmpRectTransform.anchoredPosition.y);
            scaleToLayerSize(newCopyOfFigure, 0, rails[currentRailIndex]);
            // Save to SceneData:
            FigureInstanceElement thisFigureInstanceElement = new FigureInstanceElement();
            thisFigureInstanceElement.instanceNr = countCopiesOfObject(figureObjects[figureNr].name); //index
            thisFigureInstanceElement.name = curr3DObject.name + "_" + countCopiesOfObject(figureObjects[figureNr].name).ToString("000");
            thisFigureInstanceElement.railStart = (int)Char.GetNumericValue(rails[currentRailIndex].name[17]) - 1; //railIndex

            StaticSceneData.StaticData.figureElements[figureNr].figureInstanceElements.Add(thisFigureInstanceElement);
            gameController.GetComponent<SceneDataController>().objects3dFigureInstances.Add(curr3DObject);
        }

        railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();

        // linken und rechten nachbarn berechnen
        CalculateNeighbors(oP);

        Debug.LogWarning("________________________________________________________________");
        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
        {
            Debug.LogWarning("vector " + i + ": " + railList[currentRailIndex].myObjects[i].objName);
        }

        return curr3DObject;
    }
    private void CreateNewInstanceFlyer(int figureNr, int spaceNr)
    {
        int countName = 0;
        countName = countCopiesOfObject(figureObjects[figureNr].name);

        if (gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
        {
            figCounterCircle[figureNr].text = (countName + 1).ToString();
        }

        // wenn schon eine figur drin ist, muss die raus
        if (SceneManaging.flyerSpace[spaceNr] != -1)
        {
            Destroy(flyerSpaces[spaceNr].transform.GetChild(0).gameObject);
        }

        // save figure nr 
        SceneManaging.flyerSpace[spaceNr] = figureNr;

        newCopyOfFigure = Instantiate(figureObjects[figureNr]);
        newCopyOfFigure.name = figureObjects[figureNr].name + "instance" + countName.ToString("000");

        RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();
        newCopyOfFigure.transform.SetParent(flyerSpaces[spaceNr].transform);
        tmpRectTransform.pivot = new Vector2(.5f, .5f);
        tmpRectTransform.anchorMin = new Vector2(.5f, .5f);
        tmpRectTransform.anchorMax = new Vector2(.5f, .5f);
        tmpRectTransform.sizeDelta = new Vector3(39, 39, 1);
        tmpRectTransform.anchoredPosition = Vector2.zero;
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(39, 39);

        // color field
        flyerSpaces[spaceNr].GetComponent<Image>().color = new Color(.63f, .25f, .1f);
    }
    private void CalculateNeighbors(Figure fig)    // calculates neighbors index
    {
        int index = -1;

        // index der aktuellen figur
        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
        {
            if (fig.objName == railList[currentRailIndex].myObjects[i].objName)
            {
                index = i;
                Debug.Log("i neigbor: " + i);
            }
        }

        // right neighbor
        for (int i = index + 1; i < railList[currentRailIndex].myObjects.Count; i++)
        {
            if (railList[currentRailIndex].myObjects[i].layer == fig.layer)
            {
                fig.neighborRight = i;
                railList[currentRailIndex].myObjects[i].neighborLeft = index;
                //Debug.Log("rechter nachbar: " + railList[currentRailIndex].myObjects[i].objName);
                //Debug.Log("linker nachbar von diesem: " + railList[currentRailIndex].myObjects[index].objName);

                break;
            }
        }

        // left neighbor
        for (int i = index - 1; i >= 0; i--)
        {
            if (railList[currentRailIndex].myObjects[i].layer == fig.layer)
            {
                fig.neighborLeft = i;
                railList[currentRailIndex].myObjects[i].neighborRight = index;
                //Debug.Log("linker nachbar: " + railList[currentRailIndex].myObjects[i].objName);
                //Debug.Log("rechter nachbar von diesem: " + railList[currentRailIndex].myObjects[index].objName);
                break;
            }
        }
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
        for (int i = 0; i < railList[index].myObjects.Count; i++)
        {
            for (int j = 0; j < railList[index].myObjects.Count; j++)
            {
                //Debug.Log("name j: " + railList[index].myObjects[j].objName + ", name i: " + railList[index].myObjects[i].objName + ", posx j: " + railList[index].myObjects[j].position.x + ", posx i: " + railList[index].myObjects[i].position.x);

                if (railList[index].myObjects[j].objName != railList[index].myObjects[i].objName)
                {
                    //Debug.Log("name1: " + railList[index].myObjects[j].objName + ", name2: " + railList[index].myObjects[i].objName);
                    if (railList[index].myObjects[j].position.x >= railList[index].myObjects[i].position.x
                    && railList[index].myObjects[j].position.x <= (railList[index].myObjects[i].position.x + (railList[index].myObjects[i].position.y)))
                    {
                        //Debug.Log("hi1");
                        val = true;
                    }
                    else if ((railList[index].myObjects[j].position.x + railList[index].myObjects[j].position.y) >= railList[index].myObjects[i].position.x
                    && railList[index].myObjects[j].position.x <= railList[index].myObjects[i].position.x)
                    {
                        //Debug.Log("hi2");
                        val = true;
                    }
                }
            }
        }
        return val;
    }
    private void LoopRight(int startIdx, Figure fig, bool fromLeft)
    {
        RectTransform rectTransform = fig.figure.GetComponent<RectTransform>();

        for (int i = startIdx; i < railList[currentRailIndex].myObjects.Count; i++)
        {
            // wenn es einen rechten nachbarn hat
            if (railList[currentRailIndex].myObjects[i].neighborRight != -1)
            {
                Debug.Log("HAT einen rechten nachbarn");
                // wenn zwischen den beiden genug platz ist
                if (railList[currentRailIndex].myObjects[railList[currentRailIndex].myObjects[i].neighborRight].position.x > railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y + rectSize)
                {
                    Debug.Log("es ist genug platz");
                    if (railList[currentRailIndex].myObjects[i].layer == 1)
                    {
                        Debug.Log("layer 1");
                        scaleToLayerSize(fig.figure, 1, rails[currentRailIndex]);
                        fig.layer = 1;
                    }
                    else
                    {
                        Debug.Log("layer 2");
                        scaleToLayerSize(fig.figure, 2, rails[currentRailIndex]);
                        fig.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {
                    // hier passiert nichts, es muss weiter gesucht werden
                }
            }
            else
            {
                Debug.Log("hat KEINEN rechten nachbarn");
                if (railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y + rectSize < railwidthAbsolute)
                {
                    Debug.Log("platz bis rand");
                    if (railList[currentRailIndex].myObjects[i].layer == 1)
                    {
                        Debug.Log("layer 1");
                        scaleToLayerSize(fig.figure, 1, rails[currentRailIndex]);
                        fig.layer = 1;
                    }
                    else
                    {
                        Debug.Log("layer 2");
                        scaleToLayerSize(fig.figure, 2, rails[currentRailIndex]);
                        fig.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {
                    if (!fromLeft)
                    {
                    Debug.Log("kein platz bis zum rand, es muss links weitergesucht werden");
                        LoopLeft(startIdx, fig, true);
                    }
                    else
                    {
                        Debug.Log("ab ins shelf mit dir!");
                        // ins shelf zurueck!
                    }
                    break;
                    // hier muss links weitergesucht werden
                }
            }
        }
    }
    private void LoopLeft(int startIdx, Figure fig, bool fromRight)
    {
        RectTransform rectTransform = fig.figure.GetComponent<RectTransform>();

        for (int i = startIdx; i >= 0; i--)
        {
            // wenn es einen linken nachbarn hat
            if (railList[currentRailIndex].myObjects[i].neighborLeft != -1)
            {
                Debug.Log("HAT einen linken nachbarn");
                // wenn zwischen den beiden genug platz ist
                if (railList[currentRailIndex].myObjects[railList[currentRailIndex].myObjects[i].neighborLeft].position.x + railList[currentRailIndex].myObjects[railList[currentRailIndex].myObjects[i].neighborLeft].position.y + rectSize < railList[currentRailIndex].myObjects[i].position.x)
                {
                    Debug.Log("es ist genug platz");
                    if (railList[currentRailIndex].myObjects[i].layer == 1)
                    {
                        Debug.Log("layer 1");
                        scaleToLayerSize(fig.figure, 1, rails[currentRailIndex]);
                        fig.layer = 1;
                    }
                    else
                    {
                        Debug.Log("layer 2");
                        scaleToLayerSize(fig.figure, 2, rails[currentRailIndex]);
                        fig.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[i].position.x - rectSize, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {
                    // hier passiert nichts, es muss weiter gesucht werden
                }
            }
            else
            {
                Debug.Log("hat KEINEN linken nachbarn");
                if (railList[currentRailIndex].myObjects[i].position.x - rectSize > 50)
                {
                    Debug.Log("platz bis rand");
                    if (railList[currentRailIndex].myObjects[i].layer == 1)
                    {
                        Debug.Log("layer 1");
                        scaleToLayerSize(fig.figure, 1, rails[currentRailIndex]);
                        fig.layer = 1;
                    }
                    else
                    {
                        Debug.Log("layer 2");
                        scaleToLayerSize(fig.figure, 2, rails[currentRailIndex]);
                        fig.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[i].position.x - rectSize, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {

                    // hier muss rechts weitergesucht werden
                    if (!fromRight)
                    {
                        Debug.Log("kein platz bis zum rand, es muss rechts weitergesucht werden");

                        LoopRight(startIdx - 1, fig, true);
                    }
                    else
                    {
                        Debug.Log("ab ins shelf mit dir!");
                        // ins shelf zurueck!
                    }

                    break;

                }
            }
        }
    }
    private void FindFreeSpot(Figure fig)
    {
        // find out index in the middle
        int middleIdx;
        RectTransform rectTransform = fig.figure.GetComponent<RectTransform>();

        if (railList[currentRailIndex].myObjects.Count % 2 == 1)
        {
            middleIdx = ((railList[currentRailIndex].myObjects.Count - 1) / 2) - 1;
        }
        else
        {
            middleIdx = (railList[currentRailIndex].myObjects.Count / 2) - 1;
        }
        //Debug.Log("mitte: " + middleIdx);

        // maus ist LINKS vom bereich der figuren
        if (Input.mousePosition.x < railList[currentRailIndex].myObjects[middleIdx].figure.transform.position.x + (railList[currentRailIndex].myObjects[middleIdx].position.y / 2))
        {
            Debug.Log("_________________MAUS LINKS__________________");
            int idx = isCurrentFigureOverlapping(currentRailIndex, fig, "left");
            if (idx != -1)
            {
                Debug.Log("idx: " + idx);
                // gibt es ein idx + 1 mit unterschiedlicher Layer?
                if (idx < railList[currentRailIndex].myObjects.Count - 1 && railList[currentRailIndex].myObjects[idx + 1].layer != railList[currentRailIndex].myObjects[idx].layer
                && railList[currentRailIndex].myObjects[idx + 1].objName != fig.objName)
                {
                    Debug.Log("es gibt ein idx+1: " + railList[currentRailIndex].myObjects[idx + 1].objName);

                    // hat es einen linken nachbarn? 
                    LoopLeft(idx + 1, fig, false);
                }
                else
                {
                    Debug.Log("es gibt kein idx+1");
                    LoopLeft(idx, fig, false);
                }
                // Debug.Log("fig: " + railList[currentRailIndex].myObjects[overlap].objName);
            }
            else
            {
                Debug.Log("not overlapping");
                scaleToLayerSize(fig.figure, 1, rails[currentRailIndex]);
                fig.layer = 1;
            }
        }
        // maus ist RECHTS vom bereich der figuren
        else
        {
            Debug.Log("maus ist rechts");
            int idx = isCurrentFigureOverlapping(currentRailIndex, fig, "right");

            if (idx != -1)
            {
                Debug.Log("idx: " + idx);
                // gibt es ein idx - 1 mit unterschiedlicher Layer?
                if (idx > 0 && railList[currentRailIndex].myObjects[idx - 1].layer != railList[currentRailIndex].myObjects[idx].layer)
                {
                    Debug.Log("es gibt ein idx-1: " + railList[currentRailIndex].myObjects[idx - 1].objName);

                    // hat es einen rechten nachbarn? 
                    LoopRight(idx - 1, fig, false);
                }
                else
                {
                    Debug.Log("es gibt kein idx-1");
                    LoopRight(idx, fig, false);
                }
                // Debug.Log("fig: " + railList[currentRailIndex].myObjects[overlap].objName);
            }
            else
            {
                Debug.Log("not overlapping");
                scaleToLayerSize(fig.figure, 1, rails[currentRailIndex]);
                fig.layer = 1;
            }
        }
        fig.position = new Vector2(rectTransform.anchoredPosition.x, rectSize);
        rectTransform.sizeDelta = new Vector2(rectSize, rectTransform.sizeDelta.y);
    }
    // private void LookForFreeSpot(Figure obj, int railIndex, bool create)
    // {
    //     int layer1Full = -1;
    //     int layer2Full = -1;

    //     //Debug.Log("count 1: " + railList[railIndex].myObjectsPositionListLayer1.Count + ", count 2: " + railList[railIndex].myObjectsPositionListLayer2.Count);

    //     for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
    //         // wenn linker rand des aktuellen obj weiter links ist als rechter rand des collisionsobjekts
    //         if (obj.position.x + obj.position.y > railList[railIndex].myObjectsPositionListLayer1[i].position.x
    //         && railList[railIndex].myObjectsPositionListLayer1[i].position.x + railList[railIndex].myObjectsPositionListLayer1[i].position.y > obj.position.x
    //         && railList[railIndex].myObjectsPositionListLayer1[i].objName != obj.objName)
    //         {
    //             //Debug.Log("layer1full: " + i);
    //             layer1Full = i;
    //         }

    //     for (int j = 0; j < railList[railIndex].myObjectsPositionListLayer2.Count; j++)
    //         if (obj.position.x + obj.position.y > railList[railIndex].myObjectsPositionListLayer2[j].position.x
    //         && railList[railIndex].myObjectsPositionListLayer2[j].position.x + railList[railIndex].myObjectsPositionListLayer2[j].position.y > obj.position.x
    //         && railList[railIndex].myObjectsPositionListLayer2[j].objName != obj.objName)
    //         {
    //             //Debug.Log("layer2Full: " + j);
    //             layer2Full = j;
    //         }

    //     // wenn alles voll ist
    //     if (layer2Full != -1 && layer1Full != -1)
    //     {
    //         Debug.Log("hi");

    //         // Debug.Log("1: " + railList[railIndex].myObjectsPositionListLayer1[layer1Full].position + ", 2: " + railList[railIndex].myObjectsPositionListLayer2[layer2Full].position);
    //         // Debug.Log("length: " + railIndex + ", layer1: " + layer1Full + ", layer2: " + layer2Full);
    //         // obj auf layer 1 ist weiter links
    //         if (railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x > railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x)
    //         {
    //             // Debug.Log("obj auf layer 1 ist weiter links: mouse: " + Input.mousePosition.x + ", obj: " + railList[railIndex].myObjectsPositionListLayer2[layer2Full].figure.transform.position.x);//+", anchored: " + railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x);
    //             // obj ist links von collision
    //             if (Input.mousePosition.x < railList[railIndex].myObjectsPositionListLayer2[layer2Full].figure.transform.position.x)
    //             {
    //                 //Debug.Log("left");
    //                 FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "left");
    //             }
    //             // obj ist rechts von collision
    //             else
    //             {
    //                 //Debug.Log("right");
    //                 FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "right");
    //             }
    //         }
    //         // obj auf layer 1 ist weiter rechts
    //         else
    //         {
    //             //Debug.Log("springst du hier hin? ");
    //             // obj ist links von collision
    //             if (Input.mousePosition.x < railList[railIndex].myObjectsPositionListLayer1[layer1Full].figure.transform.position.x)
    //             {
    //                 //Debug.Log("else left");
    //                 FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "left");
    //             }
    //             // obj ist rechts von collision
    //             else
    //             {
    //                 //Debug.Log("else right");
    //                 FindNextFreeSpot(obj, railList[railIndex].myObjectsPositionListLayer1[layer1Full].position.x, railList[railIndex].myObjectsPositionListLayer2[layer2Full].position.x, railIndex, create, "right");
    //             }
    //         }
    //     }
    //     // wenn nur layer 2 frei ist
    //     else if (layer1Full != -1)
    //     {
    //         //Debug.Log("nur layer 2 frei!");

    //         if (create)
    //         {

    //             scaleToLayerSize(obj.figure, 2, rails[railIndex]);
    //             railList[railIndex].myObjectsPositionListLayer2.Add(obj);
    //         }
    //         else
    //         {
    //             scaleToLayerSize(obj.figure, 2, rails[railIndex]);
    //             if (railList[railIndex].myObjectsPositionListLayer1.Contains(obj))
    //             {
    //                 UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer1, railList[railIndex].myObjectsPositionListLayer2);
    //             }

    //             // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer2.Count; i++)
    //             // {
    //             //     Debug.Log("2 vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer2[i].position.x);
    //             // }
    //         }
    //         railList[railIndex].myObjectsPositionListLayer2 = railList[railIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
    //     }
    //     // wenn nur layer 1 frei ist
    //     else if (layer2Full != -1)
    //     {
    //         //Debug.Log("nur layer 1 frei!");
    //         if (create)
    //         {
    //             scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //             railList[railIndex].myObjectsPositionListLayer1.Add(obj);
    //         }
    //         else
    //         {
    //             scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //             if (railList[railIndex].myObjectsPositionListLayer2.Contains(obj))
    //             {
    //                 UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);
    //                 //Debug.Log("count layer 1: " + railList[railIndex].myObjectsPositionListLayer1.Count);

    //                 // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
    //                 // {
    //                 //     Debug.Log("1 vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
    //                 // }
    //             }
    //             railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
    //         }
    //     }
    //     else    // alles frei
    //     {
    //         //Debug.Log("hi2");

    //         if (create)
    //         {
    //             scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //             railList[railIndex].myObjectsPositionListLayer1.Add(obj);
    //         }
    //         else
    //         {
    //             // Debug.Log("alles frei");
    //             scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //             if (railList[railIndex].myObjectsPositionListLayer2.Contains(obj))
    //             {
    //                 UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);

    //                 // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
    //                 // {
    //                 //     Debug.Log("1 vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
    //                 // }
    //             }
    //             railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
    //         }
    //     }
    // }
    // private void FindNextFreeSpot(Figure obj, float posXIn1, float posXIn2, int railIndex, bool create, string direction)
    // {
    //     float foundIndexLayer1;
    //     float foundIndexLayer2;
    //     RectTransform tmpRectTransform = obj.figure.GetComponent<RectTransform>();

    //     // mouse ist links von collision
    //     if (direction == "left")
    //     {
    //         foundIndexLayer1 = posXIn1;
    //         foundIndexLayer2 = posXIn2;

    //         //erstmal layer 1
    //         for (int i = railList[railIndex].myObjectsPositionListLayer1.Count - 1; i >= 0; i--)
    //         {
    //             if (railList[railIndex].myObjectsPositionListLayer1[i].objName != obj.objName)
    //             {
    //                 //Debug.Log("gefunden: " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
    //                 // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt -> dann wird die Stelle davor als gefunden genommen
    //                 if (railList[railIndex].myObjectsPositionListLayer1[i].position.x + railList[railIndex].myObjectsPositionListLayer1[i].position.y >= foundIndexLayer1 - obj.position.y
    //                 && railList[railIndex].myObjectsPositionListLayer1[i].position.x <= foundIndexLayer1)
    //                 {
    //                     foundIndexLayer1 = railList[railIndex].myObjectsPositionListLayer1[i].position.x;
    //                     break;
    //                     // Debug.Log("foundindex1: " + foundIndexLayer1);
    //                 }
    //             }
    //         }
    //         // dann layer 2
    //         for (int j = railList[railIndex].myObjectsPositionListLayer2.Count - 1; j >= 0; j--)
    //         {
    //             if (railList[railIndex].myObjectsPositionListLayer2[j].objName != obj.objName)
    //             {
    //                 //Debug.Log("gefunden: " + railList[railIndex].myObjectsPositionListLayer2[j].position.x + ", y: " + railList[railIndex].myObjectsPositionListLayer2[j].position.y);
    //                 // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt -> dann wird die Stelle davor als gefunden genommen
    //                 if (railList[railIndex].myObjectsPositionListLayer2[j].position.x + railList[railIndex].myObjectsPositionListLayer2[j].position.y >= foundIndexLayer2 - obj.position.y
    //                 && railList[railIndex].myObjectsPositionListLayer2[j].position.x <= foundIndexLayer2)
    //                 {
    //                     foundIndexLayer2 = railList[railIndex].myObjectsPositionListLayer2[j].position.x;
    //                     break;
    //                     // Debug.Log("foundindex2: " + foundIndexLayer2);
    //                 }
    //             }
    //         }
    //         // liegt platz auf layer1 naeher dran
    //         if (foundIndexLayer1 > foundIndexLayer2)
    //         {
    //             tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer1 - tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
    //             obj.position = new Vector2(foundIndexLayer1 - tmpRectTransform.sizeDelta.x, tmpRectTransform.sizeDelta.x);
    //             //Debug.Log("transform 0: " + tmpRectTransform.anchoredPosition + ", size: " + tmpRectTransform.sizeDelta.x);

    //             if (create)
    //             {
    //                 // wenn der rand im weg ist
    //                 if (foundIndexLayer1 - obj.position.y < 50)
    //                 {
    //                     // Debug.Log("delete: " + obj.objName);
    //                     _toBeRemoved = true;
    //                 }
    //                 else
    //                 {
    //                     railList[railIndex].myObjectsPositionListLayer1.Add(obj);
    //                     scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //                 }
    //             }
    //             else
    //             {
    //                 if (!railList[railIndex].myObjectsPositionListLayer1.Contains(obj))
    //                 {
    //                     scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //                     UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);
    //                 }
    //             }
    //             railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
    //         }
    //         // liegt platz auf layer2 naeher dran
    //         else
    //         {
    //             tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer2 - tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
    //             obj.position = new Vector2(foundIndexLayer2 - tmpRectTransform.sizeDelta.x, tmpRectTransform.sizeDelta.x);
    //             //Debug.Log("transform 1: " + tmpRectTransform.anchoredPosition + ", size: " + tmpRectTransform.sizeDelta.x);

    //             if (create)
    //             {
    //                 // abfrage, wenn der naechste freie platz ausserhalb der schiene waere
    //                 if (foundIndexLayer2 - tmpRectTransform.sizeDelta.x < 50)
    //                 {
    //                     // Debug.Log("deleeeete: " + obj.objName);
    //                     _toBeRemoved = true;
    //                 }
    //                 else
    //                 {
    //                     railList[railIndex].myObjectsPositionListLayer2.Add(obj);
    //                     scaleToLayerSize(obj.figure, 2, rails[railIndex]);
    //                 }
    //             }

    //             else
    //             {
    //                 if (!railList[railIndex].myObjectsPositionListLayer2.Contains(obj))
    //                 {
    //                     scaleToLayerSize(obj.figure, 2, rails[railIndex]);
    //                     UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer1, railList[railIndex].myObjectsPositionListLayer2);
    //                 }
    //             }

    //             railList[railIndex].myObjectsPositionListLayer2 = railList[railIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
    //         }
    //     }
    //     // mouse ist rechts von collision
    //     else
    //     {
    //         foundIndexLayer1 = posXIn1;
    //         foundIndexLayer2 = posXIn2;

    //         //erstmal layer 1
    //         for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
    //         {
    //             if (railList[railIndex].myObjectsPositionListLayer1[i].objName != obj.objName)
    //             {
    //                 // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt
    //                 if (railList[railIndex].myObjectsPositionListLayer1[i].position.x <= foundIndexLayer1 + tmpRectTransform.sizeDelta.x + tmpRectTransform.sizeDelta.x
    //                 && railList[railIndex].myObjectsPositionListLayer1[i].position.x >= foundIndexLayer1)
    //                 {
    //                     foundIndexLayer1 = railList[railIndex].myObjectsPositionListLayer1[i].position.x;
    //                     // Debug.Log("foundindex1: " + foundIndexLayer1);
    //                 }
    //             }
    //         }
    //         // dann layer 2
    //         for (int j = 0; j < railList[railIndex].myObjectsPositionListLayer2.Count; j++)
    //         {
    //             if (railList[railIndex].myObjectsPositionListLayer2[j].objName != obj.objName)
    //             {
    //                 // wenn der Beginn des naechsten objekts so liegt, DASS aktuelles objekt dazwischen passt
    //                 if (railList[railIndex].myObjectsPositionListLayer2[j].position.x <= foundIndexLayer2 + tmpRectTransform.sizeDelta.x + tmpRectTransform.sizeDelta.x
    //                 && railList[railIndex].myObjectsPositionListLayer2[j].position.x >= foundIndexLayer2)
    //                 {
    //                     foundIndexLayer2 = railList[railIndex].myObjectsPositionListLayer2[j].position.x;
    //                     // Debug.Log("foundindex2: " + foundIndexLayer2);
    //                 }
    //             }
    //         }

    //         // liegt platz auf layer1 naeher dran
    //         if (foundIndexLayer1 > foundIndexLayer2)
    //         {
    //             tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer2 + tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
    //             obj.position = new Vector2(foundIndexLayer2 + tmpRectTransform.sizeDelta.x, tmpRectTransform.sizeDelta.x);

    //             // Debug.Log("transform 2: " + tmpRectTransform.anchoredPosition + ", size: " + tmpRectTransform.sizeDelta.x);


    //             //Debug.Log("foundind1: " + foundIndexLayer1 + ", tmprect: " + tmpRectTransform.sizeDelta.x + ", maxX: " + maxX);
    //             if (create)
    //             {
    //                 if (foundIndexLayer1 + tmpRectTransform.sizeDelta.x > 1670.4f)
    //                 {
    //                     // Debug.Log("delete: " + currentClickedInstanceObjectIndex);
    //                     _toBeRemoved = true;
    //                 }
    //                 else
    //                 {
    //                     // Debug.Log("hi");
    //                     railList[railIndex].myObjectsPositionListLayer2.Add(obj);
    //                     scaleToLayerSize(obj.figure, 2, rails[railIndex]);
    //                 }
    //             }
    //             else
    //             {
    //                 if (!railList[railIndex].myObjectsPositionListLayer2.Contains(obj))
    //                 {
    //                     scaleToLayerSize(obj.figure, 2, rails[railIndex]);
    //                     UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer1, railList[railIndex].myObjectsPositionListLayer2);
    //                     // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer2.Count; i++)
    //                     // {
    //                     //     Debug.Log("layer 1 naeher vector! " + i + ": " + railList[railIndex].myObjectsPositionListLayer2[i].position.x);
    //                     // }
    //                 }
    //             }

    //             railList[railIndex].myObjectsPositionListLayer2 = railList[railIndex].myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
    //         }
    //         else
    //         {
    //             tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer1 + tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
    //             obj.position = new Vector2(foundIndexLayer1 + tmpRectTransform.sizeDelta.x, tmpRectTransform.sizeDelta.x);
    //             //Debug.Log("transform 3: " + tmpRectTransform.anchoredPosition + ", size: " + tmpRectTransform.sizeDelta.x);

    //             if (create)
    //             {
    //                 // abfrage, wenn der naechste freie platz ausserhalb der schiene waere
    //                 if (foundIndexLayer2 + tmpRectTransform.sizeDelta.x > 1670.4f)
    //                 {
    //                     // Debug.Log("deleeeete: " + currentClickedInstanceObjectIndex);
    //                     _toBeRemoved = true;
    //                 }
    //                 else
    //                 {
    //                     railList[railIndex].myObjectsPositionListLayer1.Add(obj);
    //                     scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //                 }
    //             }
    //             else
    //             {
    //                 if (!railList[railIndex].myObjectsPositionListLayer1.Contains(obj))
    //                 {
    //                     scaleToLayerSize(obj.figure, 1, rails[railIndex]);
    //                     UpdatePositionVectorInformation(obj, railList[railIndex].myObjectsPositionListLayer2, railList[railIndex].myObjectsPositionListLayer1);
    //                     // for (int i = 0; i < railList[railIndex].myObjectsPositionListLayer1.Count; i++)
    //                     // {
    //                     //     Debug.Log("layer 2 naeher! vector " + i + ": " + railList[railIndex].myObjectsPositionListLayer1[i].position.x);
    //                     // }
    //                 }
    //             }

    //             railList[railIndex].myObjectsPositionListLayer1 = railList[railIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
    //         }
    //     }
    //     //obj.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x);
    // }
    void UpdatePositionVectorInformation(Figure currentObj, List<Figure> listFrom, List<Figure> listTo)
    {
        for (int i = 0; i < listFrom.Count; i++)
        {
            if (listFrom[i].objName == currentObj.objName)
            {
                listFrom[i].position = currentObj.position;
                listTo.Add(listFrom[i]);
                listFrom.Remove(listFrom[i]);
            }
        }
    }
    void holdMinimumDistance(Figure currentObj, float minimumDistance)
    {
        RectTransform tmpRectTransform = currentObj.figure.GetComponent<RectTransform>();
        for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
        {
            if (currentObj.objName != railList[currentRailIndex].myObjects[j].objName
            && currentObj.position.x < railList[currentRailIndex].myObjects[j].position.x + minimumDistance
            && currentObj.position.x >= railList[currentRailIndex].myObjects[j].position.x)
            {
                tmpRectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.anchoredPosition.y);
                currentObj.position = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.sizeDelta.x);
            }
            else if (currentObj.objName != railList[currentRailIndex].myObjects[j].objName
            && currentObj.position.x < railList[currentRailIndex].myObjects[j].position.x
            && currentObj.position.x >= railList[currentRailIndex].myObjects[j].position.x - minimumDistance)
            {
                tmpRectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[j].position.x - minimumDistance, tmpRectTransform.anchoredPosition.y);
                currentObj.position = new Vector2(railList[currentRailIndex].myObjects[j].position.x - minimumDistance, tmpRectTransform.sizeDelta.x);
            }
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
            #region identifying
            //identify which gameobject you clicked
            identifyClickedObject();         //method fills up the current clicked index
            identifyClickedObjectByList(railList[currentRailIndex].myObjects);
            editTimelineObject = false;                             //flag to prevent closing the timeline if you click an object in timeline
            releaseOnTimeline = false;                              //because you have not set anything on timeline 
            releaseObjMousePos = new Vector2(0.0f, 0.0f);

            int tmpI = -1;
            for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
            {
                if (railList[currentRailIndex].myObjects[i].figure.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                    tmpI = i;
            }
            if (tmpI >= 0)
                editTimelineObject = true;
            else
                editTimelineObject = false;
            #endregion

            //if you click the timeline with the mouse
            if (!SceneManaging.flyerActive)
            {
                for (int i = 0; i < rails.Length; i++)
                    if (rails[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                    {
                        //open or close timeline
                        openTimelineByClick(railList[i].isTimelineOpen, i, false);
                        currentRailIndex = i;
                    }
            }

            // wenn theaterzettel aktiv ist    
            if (SceneManaging.flyerActive)
            {
                if (currentSpaceActive != -1)
                {
                    // click delete
                    Vector2 pos = flyerSpaces[currentSpaceActive].transform.GetChild(0).GetChild(0).GetChild(0).position;
                    RectTransform rect = flyerSpaces[currentSpaceActive].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
                    if (getMousePos.x >= pos.x - rect.sizeDelta.x / 2.5f && getMousePos.x <= pos.x + rect.sizeDelta.x / 2.5f && getMousePos.y >= pos.y - rect.sizeDelta.y / 2.5f && getMousePos.y <= pos.y + rect.sizeDelta.y / 2.5f)
                    {
                        SceneManaging.flyerSpace[currentSpaceActive] = -1;
                        Destroy(flyerSpaces[currentSpaceActive].transform.GetChild(0).gameObject);
                        // color field
                        flyerSpaces[currentSpaceActive].GetComponent<Image>().color = new Color(.78f, .54f, .44f);
                    }
                }

                int tmpHit = checkHittingFlyerSpace(getMousePos);
                // wenn eine figur im kaestchen ist und diese geklickt wird
                if (tmpHit != -1 && SceneManaging.flyerSpace[tmpHit] != -1)
                {
                    // delete button
                    flyerSpaces[tmpHit].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    if (currentSpaceActive != tmpHit && currentSpaceActive != -1)
                    {
                        flyerSpaces[currentSpaceActive].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    }
                    currentSpaceActive = tmpHit;
                }
                else
                {
                    if (currentSpaceActive != -1)
                    {
                        flyerSpaces[currentSpaceActive].transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        currentSpaceActive = -1;
                    }
                }
            }

            //if you click on an object in shelf
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                // default rect erstellen
                //wenn neues objekt genommen wird, soll altes aktuelles objekt unhighlighted werden
                for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                {
                    highlight(railList[currentRailIndex].myObjects[j].figure3D, railList[currentRailIndex].myObjects[j].figure, false);
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

                if (!SceneManaging.sceneChanged)
                    SceneManaging.sceneChanged = true;

                diff = new Vector2(getMousePos.x - figureObjects[currentClickedObjectIndex].transform.position.x, getMousePos.y - figureObjects[currentClickedObjectIndex].transform.position.y);

                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    draggingObject = true;
                    SceneManaging.dragging = true;
                }
            }
            //or check if you click an object in timeline
            else if (currentClickedInstanceObjectIndex != -1 && editTimelineObject && railList[currentRailIndex].isTimelineOpen)
            {
                if (!SceneManaging.sceneChanged) SceneManaging.sceneChanged = true;

                if (railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    // click on delete-Button
                    if (railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).gameObject.activeSelf
                        && getMousePos.x >= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.x - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x / 2.5f && getMousePos.x <= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.x + railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x / 2.5f
                        && getMousePos.y >= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.y - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.y / 2.5f && getMousePos.y <= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.y + railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.y / 2.5f)
                    {
                        // Debug.Log("delete");
                        _toBeRemovedFromTimeline = true;
                    }

                    diff = new Vector2(getMousePos.x - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.position.x, getMousePos.y - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.position.y);
                    draggingOnTimeline = true;
                    SceneManaging.dragging = true;

                    //highlighting objects and showing delete button when clicked
                    if (SceneManaging.highlighted == false)
                    {
                        highlight(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure3D, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, true);
                    }
                    else if (SceneManaging.highlighted && railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).GetComponent<Image>().color == colFigureHighlighted) // checkFigureHighlighted(timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject))  // check if second child (which is never the armature) has emission enabled (=is highlighted)
                    {
                        // highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex], false);
                    }
                    else
                    {
                        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                        {
                            highlight(railList[currentRailIndex].myObjects[i].figure3D, railList[currentRailIndex].myObjects[i].figure, false);
                        }
                        highlight(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure3D, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, true);
                    }
                }

            }

            //if you hit/clicked nothing with mouse
            else if (Physics2D.OverlapPoint(getMousePos) == false)
            {
                // delete highlight of everything if nothing is clicked
                for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                {
                    highlight(railList[currentRailIndex].myObjects[i].figure3D, railList[currentRailIndex].myObjects[i].figure, false);
                }
            }
        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline)
        {
            if (currentClickedInstanceObjectIndex != -1)  //is set in the identify-methods
            {
                Figure currentObj = railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex];
                isInstance = true;
                //if you click an object in timeline (for dragging)
                updateObjectPosition(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], getMousePos - diff);
                setObjectOnTimeline(currentObj.figure, rails[currentRailIndex].transform.position.y); //snapping/lock y-axis
                RectTransform currentPos = currentObj.figure.GetComponent<RectTransform>();

                #region Stapeln von Ebenen
                // if there is only one layer on Rail
                if (railList[currentRailIndex].sizeLayering == 1)
                {
                    if (isCurrentFigureOverlapping(currentRailIndex, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], "") != -1)
                    {
                        scaleToLayerSize(currentObj.figure, 2, rails[currentRailIndex]);

                        railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();

                        // for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
                        // {
                        //     Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer2[i].position.x);
                        // }

                        // scale all other timelineobjects of layer 1
                        for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                        {
                            if (railList[currentRailIndex].myObjects[j].figure != currentObj.figure && railList[currentRailIndex].myObjects[j].layer == 1)
                                scaleToLayerSize(railList[currentRailIndex].myObjects[j].figure, 1, rails[currentRailIndex]);
                        }

                        railList[currentRailIndex].sizeLayering = 2;
                        //Debug.Log("jetz 2");
                    }
                }

                // if there are two layers on Rail
                else if (railList[currentRailIndex].sizeLayering == 2)
                {
                    // wenn nichts mehr überlappt
                    if (!isSomethingOverlapping(currentRailIndex))
                    {
                        railList[currentRailIndex].sizeLayering = 1;
                        // scale all timelineobjects to layer 0
                        for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                        {
                            scaleToLayerSize(railList[currentRailIndex].myObjects[j].figure, 0, rails[currentRailIndex]);
                        }

                        railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();

                        // for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
                        // {
                        //     Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer1[i].position.x);
                        // }
                    }

                    else
                    {
                        //LookForFreeSpot(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], currentRailIndex, false);


                        // railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position = new Vector2()
                    }
                    // mindestabstand von 50 cm einhalten
                    holdMinimumDistance(currentObj, _spaceMax);
                }
                #endregion

                #region limits front and back of rail
                // limit front of rail
                if (currentPos.anchoredPosition.x <= 50)
                {
                    //Debug.Log("hi");
                    currentPos.anchoredPosition = new Vector2(50, currentPos.anchoredPosition.y);
                    currentObj.position = new Vector2(50, currentObj.position.y);

                    if (isCurrentFigureOverlapping(currentRailIndex, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], "") != -1)
                    {
                        _toBeRemovedFromTimeline = true;
                        currentObj.figure.transform.GetChild(0).GetComponent<Image>().color = colFigureRed;
                    }
                }

                // limit back of rail
                else if ((currentPos.anchoredPosition.x + currentPos.sizeDelta.x - 50) > (railwidthAbsolute))
                {
                    currentPos.anchoredPosition = new Vector2(railwidthAbsolute - currentPos.sizeDelta.x + 50, currentPos.anchoredPosition.y);
                    currentObj.position = new Vector2(railwidthAbsolute - currentPos.sizeDelta.x + 50, currentObj.position.y);

                    if (isCurrentFigureOverlapping(currentRailIndex, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], "") != -1)
                    {
                        _toBeRemovedFromTimeline = true;
                        currentObj.figure.transform.GetChild(0).GetComponent<Image>().color = colFigureRed;
                    }
                }
                #endregion

                // wenn figur aus der schiene gezogen wird
                else if (!(getMousePos.x > rails[currentRailIndex].transform.position.x && getMousePos.x < rails[currentRailIndex].transform.position.x + rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta.x / screenDifference.x
                && getMousePos.y < rails[currentRailIndex].transform.position.y + (rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta.y / screenDifference.y / 2) && getMousePos.y > rails[currentRailIndex].transform.position.y - (rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta.y / screenDifference.y / 2)))       // mouse outside
                {
                    draggingObject = true;
                    editTimelineObject = false;
                    draggingOnTimeline = false;

                    // delete button ausblenden
                    currentObj.figure.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                    currentObj.figure.GetComponent<RectTransform>().pivot = new Vector2(currentObj.figure.GetComponent<RectTransform>().pivot.x, 0.5f);

                    //temporarily change parent, so that object appears in front of the shelf
                    currentObj.figure.transform.SetParent(parentMenue.transform);
                    newHitTimeline = currentRailIndex;
                }
                else
                {
                    if (currentObj.figure.transform.GetChild(0).GetComponent<Image>().color == colFigureRed)
                    {
                        currentObj.figure.transform.GetChild(0).GetComponent<Image>().color = colFigure;
                        _toBeRemovedFromTimeline = false;
                    }
                }
            }
            releaseOnTimeline = true;
        }
        //if something has been dragged outside of the timeline
        if (draggingOnTimeline == false && editTimelineObject == false && draggingObject && isInstance)
        {
            // moving on mouse pos
            railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.position = new Vector3(getMousePos.x, getMousePos.y, -1.0f);

            hitTimeline = checkHittingTimeline(getMousePos);
            if (hitTimeline != -1)  // if a timeline is hit
            {
                if (hitTimelineOld != hitTimeline)
                {
                    hitTimelineOld = hitTimeline;
                    newHitTimeline = hitTimeline;
                    openTimelineByDrag(hitTimeline);

                    // scale down the dragged figure (and childobject: image)
                    scaleObject(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, figPictureSize, rails[newHitTimeline].GetComponent<RectTransform>().sizeDelta.y, false);
                }
                //snapping/lock y-axis
                setObjectOnTimeline(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, rails[newHitTimeline].transform.position.y);

                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
                changedRail = true;
            }

            // wenn zwischenraum zwischen schienen getroffen wird
            else if (isRailAreaHit(getMousePos))
            {
                //snapping/lock y-axis
                setObjectOnTimeline(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, rails[newHitTimeline].transform.position.y);

                railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[newHitTimeline].GetComponent<RectTransform>().rect.height);
                railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[newHitTimeline].GetComponent<RectTransform>().rect.height);
                railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).gameObject.SetActive(true);
                railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).gameObject.GetComponent<Image>().color = colFigureAlpha; // rect alpha
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
                changedRail = true;
            }

            else // if no timeline is hit
            {
                hitTimelineOld = -1;
                //temporarily change parent, so that object appears in front of the shelf
                railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.SetParent(parentMenue.transform);
                //scale up the dragged figure and childobjects (button and rect)
                railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
                railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).gameObject.SetActive(false); // rect hide

                releaseOnTimeline = false;
                changedRail = false;
                newHitTimeline = -1;
            }
        }
        // dragging an object from shelf to timeline / flyer
        if (draggingObject && editTimelineObject == false && isInstance == false)
        {
            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            if (_timerFigure <= 0.5f)
            {
                _timerFigure += Time.deltaTime;
            }
            scrollRect.GetComponent<ScrollRect>().enabled = false;
            //move object
            figureObjects[currentClickedObjectIndex].transform.position = new Vector3(getMousePos.x - diff.x, getMousePos.y - diff.y, -1.0f);
            // updateObjectPosition(figureObjects[currentClickedObjectIndex], getMousePos - diff);


            if (!SceneManaging.flyerActive)
            {
                hitTimeline = checkHittingTimeline(getMousePos);
                //Debug.Log("hitimeline: " + hitTimeline);
            }
            else
            {
                flyerHit = checkHittingFlyerSpace(getMousePos);
                //Debug.Log("hit: "+flyerHit);
            }

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            if (hitTimeline != -1)
            {
                if (hitTimelineOld != hitTimeline)
                {
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
            else if (isRailAreaHit(getMousePos) && !SceneManaging.flyerActive)
            {
                //snapping/lock y-axis
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], rails[currentRailIndex].transform.position.y);

                figureObjects[currentClickedObjectIndex].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[currentRailIndex].GetComponent<RectTransform>().rect.height);
                figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(true);
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }
            // flyeractive
            else if (SceneManaging.flyerActive)
            {
                RectTransform tmpRectTransform = figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>();

                // einen der spaces getroffen
                if (flyerHit != -1)
                {
                    figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(39, 39);
                    figureObjects[currentClickedObjectIndex].transform.SetParent(flyerSpaces[flyerHit].transform);

                    tmpRectTransform.pivot = new Vector2(.5f, .5f);
                    tmpRectTransform.anchorMin = new Vector2(.5f, .5f);
                    tmpRectTransform.anchorMax = new Vector2(.5f, .5f);
                    tmpRectTransform.sizeDelta = new Vector3(39, 39, 1);
                    tmpRectTransform.anchoredPosition = Vector2.zero;
                    //figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
                // ausserhalb des spaces
                else
                {
                    tmpRectTransform.pivot = new Vector2(0f, .5f);
                    tmpRectTransform.anchorMin = new Vector2(0, 1);
                    tmpRectTransform.anchorMax = new Vector2(0, 1);
                    tmpRectTransform.sizeDelta = new Vector2(150, 150);
                    figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
                }
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

            // clicked delete button
            if (_toBeRemoved && currentClickedInstanceObjectIndex != -1)
            {
                //Debug.Log("loeschen");
                removeObjectFromTimeline(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]);
            }

            // flyer mode
            if (SceneManaging.flyerActive && currentClickedObjectIndex != -1)
            {
                if (flyerHit != -1)
                {
                    //Debug.Log("reinsetzen in: " + flyerHit);
                    CreateNewInstanceFlyer(currentClickedObjectIndex, flyerHit);
                }

                //set original image back to shelf, position 2 to make it visible
                figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(false);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;

                RectTransform tmpRectTransform = figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>();
                tmpRectTransform.pivot = new Vector2(0f, .5f);
                tmpRectTransform.anchorMin = new Vector2(0, 1);
                tmpRectTransform.anchorMax = new Vector2(0, 1);
                tmpRectTransform.sizeDelta = objectShelfSize;
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
                        railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.SetParent(rails[hitTimeline].transform);
                        railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).gameObject.SetActive(true);        // set rect active
                                                                                                                                                                //3D object
                        railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure3D.transform.SetParent(rails3D[currentRailIndex].transform.GetChild(0));

                        #region limit front and end of rail
                        RectTransform tmpRectTransform = railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.GetComponent<RectTransform>();

                        if (tmpRectTransform.anchoredPosition.x < 50)
                        {
                            tmpRectTransform.anchoredPosition = new Vector3((50), tmpRectTransform.anchoredPosition.y, -1);
                            railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position = new Vector2(50, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position.y);
                        }
                        // back of rail
                        else if (tmpRectTransform.anchoredPosition.x + rectSize > railwidthAbsolute)
                        {
                            // Todo Berechnung wie lang das rechteck dann sein muss
                            tmpRectTransform.anchoredPosition = new Vector2(railwidthAbsolute - rectSize + 50, tmpRectTransform.anchoredPosition.y);
                            railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position = new Vector2(railwidthAbsolute - rectSize + 50, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position.y);
                        }
                        #endregion

                        #region LayerOverlap

                        if (railList[hitTimeline].sizeLayering == 1)
                        {
                            // wenn figur überlappt muss sie auf layer 2
                            if (isCurrentFigureOverlapping(hitTimeline, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], "") != -1)
                            {
                                railList[hitTimeline].sizeLayering = 2;
                                scaleToLayerSize(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, 2, rails[hitTimeline]);

                                // wenn die figur vorher zu layer 1 gehoert hat
                                if (railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].layer == 1) //railList[currentRailIndex].myObjectsPositionListLayer1.Contains(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]))
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                }
                                // wenn die figur vorher zu layer 2 gehoert hat
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                }
                                // others to 1
                                for (int i = 0; i < railList[hitTimeline].myObjects.Count; i++)
                                {
                                    if (railList[hitTimeline].myObjects[i].objName != railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].objName)
                                        scaleToLayerSize(railList[hitTimeline].myObjects[i].figure, 1, rails[hitTimeline]);
                                }
                                railList[hitTimeline].myObjects = railList[hitTimeline].myObjects.OrderBy(w => w.position.x).ToList();
                            }
                            //figur ueberlappt nicht und muss auf layer 1
                            else
                            {
                                scaleToLayerSize(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, 0, rails[hitTimeline]);
                                // wenn die figur vorher zu layer 1 gehoert hat
                                if (railList[currentRailIndex].myObjects.Contains(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]))
                                {
                                    //Debug.Log("hi 1");
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                    //Debug.Log("hi 2");
                                }
                                // wenn die figur vorher zu layer 2 gehoert hat
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                }
                                railList[hitTimeline].myObjects = railList[hitTimeline].myObjects.OrderBy(w => w.position.x).ToList();

                            }
                        }

                        // wenn hittimeline size-layering 2 hat
                        else
                        {
                            /* Achtung Schienenwechsel! 
                            LookForFreeSpot(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], hitTimeline, true);

                            // wenn obj layer 1 (hittimeline) zugeordnet wurde
                            if (railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].layer == 1) //railList[hitTimeline].myObjectsPositionListLayer1.Contains(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]))
                            {
                                // wenn obj vorher in layer 1 war (currentRailIndex)
                                if (railList[currentRailIndex].myObjectsPositionListLayer1.Contains(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]))
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                }
                                // wenn obj vorher in layer 2 war (currentRailIndex)
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                }
                                    railList[hitTimeline].myObjects = railList[hitTimeline].myObjects.OrderBy(w => w.position.x).ToList();
                                scaleToLayerSize(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, 1, rails[hitTimeline]);
                            }
                            // wenn obj layer 2 (hittimeline) zugeordnet wurde
                            else
                            {
                                scaleToLayerSize(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, 2, rails[hitTimeline]);
                                // wenn obj vorher in layer 1 war (currentRailIndex)
                                if (railList[currentRailIndex].myObjectsPositionListLayer1.Contains(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]))
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                }
                                // wenn obj vorher in layer 2 war (currentRailIndex)
                                else
                                {
                                    UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                                }
                                scaleToLayerSize(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, 1, rails[hitTimeline]);

                            } */
                        }
                        #endregion

                        //snapping/lock y-axis
                        setObjectOnTimeline(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, rails[hitTimeline].transform.position.y);

                        // from old 'myObjects' list to new (hittimeline) 'myobjects' list
                        UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex], railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                        // Debug.Log("hi 3");

                        if (hitTimeline != currentRailIndex)
                        {
                            //3D object
                            StaticSceneData.StaticData.figureElements[Int32.Parse(railList[hitTimeline].myObjects[railList[hitTimeline].myObjects.Count - 1].objName.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[hitTimeline].myObjects[railList[hitTimeline].myObjects.Count - 1].objName.Substring(17))].railStart = hitTimeline;
                            railList[hitTimeline].myObjects[railList[hitTimeline].myObjects.Count - 1].figure3D.transform.SetParent(rails3D[hitTimeline].transform.GetChild(0));
                            // Debug.Log("hi 4");

                            //wenn auf current rail nichts mehr ueberlappt
                            if (!isSomethingOverlapping(currentRailIndex))
                            {
                                railList[currentRailIndex].sizeLayering = 1;
                                // scale all other timelineobjects to layer 0
                                for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                                {
                                    scaleToLayerSize(railList[currentRailIndex].myObjects[j].figure, 0, rails[currentRailIndex]);
                                    // if (railList[currentRailIndex].myObjects[j].layer == 2) // railList[currentRailIndex].myObjectsPositionListLayer2.Contains(railList[currentRailIndex].myObjects[j]))
                                    // {
                                    //     UpdatePositionVectorInformation(railList[currentRailIndex].myObjects[j], railList[currentRailIndex].myObjectsPositionListLayer2, railList[currentRailIndex].myObjectsPositionListLayer1);
                                    //     railList[currentRailIndex].myObjectsPositionListLayer1 = railList[currentRailIndex].myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                                    // }
                                }
                            }
                            // Debug.Log("hi 5");

                            // box Collider disablen, damit Schiene auch auf Figuren geklickt werden kann
                            for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                            {
                                railList[currentRailIndex].myObjects[i].figure.GetComponent<BoxCollider2D>().enabled = false;
                            }
                            currentRailIndex = hitTimeline;
                        }
                        // Debug.Log("hi 6");



                        highlight(railList[hitTimeline].myObjects[railList[hitTimeline].myObjects.Count - 1].figure3D, railList[hitTimeline].myObjects[railList[currentRailIndex].myObjects.Count - 1].figure, true);
                        changedRail = false;
                    }
                    else
                    {
                        try
                        {
                            GameObject curr3DObject = CreateNew2DInstance(currentClickedObjectIndex, getMousePos.x, -1, -1, false);
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Save to SceneData:
                            FigureInstanceElement thisFigureInstanceElement = new FigureInstanceElement();
                            thisFigureInstanceElement.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex].name); //index
                            thisFigureInstanceElement.name = curr3DObject.name + "_" + countCopiesOfObject(figureObjects[currentClickedObjectIndex].name).ToString("000");
                            thisFigureInstanceElement.railStart = (int)Char.GetNumericValue(rails[currentRailIndex].name[17]) - 1; //railIndex

                            StaticSceneData.StaticData.figureElements[currentClickedObjectIndex].figureInstanceElements.Add(thisFigureInstanceElement);
                            gameController.GetComponent<SceneDataController>().objects3dFigureInstances.Add(curr3DObject);
                            highlight(railList[currentRailIndex].myObjects[railList[currentRailIndex].myObjects.Count - 1].figure3D, railList[currentRailIndex].myObjects[railList[currentRailIndex].myObjects.Count - 1].figure, true);
                            if (_toBeRemoved)
                            {
                                // Debug.Log("delete : " + railList[currentRailIndex].myObjects[railList[currentRailIndex].myObjects.Count - 1].objName);
                                removeObjectFromTimeline(railList[currentRailIndex].myObjects[railList[currentRailIndex].myObjects.Count - 1]);
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Debug.Log("Exception");
                        }
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////

                    }

                    currentClickedObjectIndex = -1; // set index back to -1 because nothing is being clicked anymore
                }

                // if figure has been moved in rail
                else
                {
                    // change position of vector in each case
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].objName == railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].objName)
                        {
                            railList[currentRailIndex].myObjects[i].position = new Vector2(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position.x, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position.y);
                        }
                    }
                    // Debug.Log("count 1: "+railList[currentRailIndex].myObjectsPositionListLayer1.Count+", count 2: "+railList[currentRailIndex].myObjectsPositionListLayer2.Count);


                    railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();

                    if (_toBeRemovedFromTimeline)
                    {
                        removeObjectFromTimeline(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]);
                    }

                    // Debug.Log("_________________________________________________________________");
                    // for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    // {
                    //     Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjects[i].position.x);
                    // }
                    // Debug.Log("______________1_________________");
                    // for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer1[i].position.x);
                    // }
                    // Debug.Log("_______________2________________");
                    // for (int i = 0; i < railList[currentRailIndex].myObjectsPositionListLayer2.Count; i++)
                    // {
                    //     Debug.Log("2 vector " + i + ": " + railList[currentRailIndex].myObjectsPositionListLayer2[i].position.x);
                    // }
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
                    helpButton.GetComponent<PressHelp>().helpTextLiveView.SetActive(true);
                }
                // wenn figur nur angetoucht wurde
                else if (_timerFigure < 0.5f)
                {
                    helpButton.GetComponent<PressHelp>().arrowPressed = true;
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
            {
                removeObjectFromTimeline(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]);
            }


            for (int k = 0; k < railList.Length; k++)
            {
                for (int i = 0; i < railList[k].myObjects.Count; i++)
                {
                    float moment = UtilitiesTm.FloatRemap(railList[k].myObjects[i].position.x - 50, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                    float zPosFigure = (rails3D[k].transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime(moment));
                    // Debug.Log("moment: " + moment + ", zpos: " + zPosFigure);

                    if (zPosFigure < 0) // wenn die Figur auf eine Position noch vor dem empty gesetzt werden würde, würde sie erscheinen bevor sie auf den rails da ist
                    {
                        zPosFigure = 500f;      // deshalb wird die Figur komplett außerhalb der Szene abgesetzt.
                    }

                    if (rails3D[k].name.Substring(7) == "1" || rails3D[k].name.Substring(7) == "3" || rails3D[k].name.Substring(7) == "5")
                    {
                        railList[k].myObjects[i].figure3D.transform.localPosition = new Vector3(-rails3D[k].transform.GetChild(0).transform.localPosition.x, (-rails3D[k].transform.GetChild(0).transform.localPosition.y - 0.01f), zPosFigure);
                    }

                    else
                    {
                        railList[k].myObjects[i].figure3D.transform.localPosition = new Vector3(rails3D[k].transform.GetChild(0).transform.localPosition.x, (-rails3D[k].transform.GetChild(0).transform.localPosition.y - 0.01f), zPosFigure);
                    }

                    if (rails3D[k].transform.GetChild(0).GetComponent<RailSpeedController>().railIndex % 2 == 1)
                    {
                        railList[k].myObjects[i].figure3D.transform.localEulerAngles = new Vector3(railList[k].myObjects[i].figure3D.transform.localEulerAngles.x, 270, railList[k].myObjects[i].figure.transform.localEulerAngles.z);
                    }
                    else
                    {
                        railList[k].myObjects[i].figure3D.transform.localEulerAngles = new Vector3(railList[k].myObjects[i].figure3D.transform.localEulerAngles.x, 270, railList[k].myObjects[i].figure3D.transform.localEulerAngles.z);
                    }
                    StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(17))].moment = moment;

                    if (railList[k].myObjects[i].layer == 1) //railList[k].myObjects.Contains(railList[k].myObjects[i]))
                    {
                        if (railList[k].sizeLayering == 1)
                            StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(17))].layer = 0;
                        else
                            StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(17))].layer = 1;
                    }
                    else
                    {
                        StaticSceneData.StaticData.figureElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[k].myObjects[i].figure.name.Substring(17))].layer = 2;
                    }
                }
            }
            releaseOnTimeline = false;
            releaseObjMousePos.x = 0.0f;
            releaseObjMousePos.y = 0.0f;
            draggingOnTimeline = false;
            isInstance = false;
            currentClickedObjectIndex = -1;
            currentClickedInstanceObjectIndex = -1;
            hitTimelineOld = -1;
            SceneManaging.dragging = false;
            _toBeRemoved = false;
            _toBeRemovedFromTimeline = false;
            hitTimeline = -1;

        }
        // enable binnenanimation when playing
        for (int k = 0; k < railList.Length; k++)
        {
            for (int i = 0; i < railList[k].myObjects.Count; i++)
            {
                // start Animation on play
                if (railList[k].myObjects[i].figure3D.GetComponent<FigureStats>().isShip == false)
                {
                    if (SceneManaging.playing && !railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled)
                    {
                        railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled = true;
                    }
                    else if (!SceneManaging.playing && railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled)
                    {
                        railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled = false;
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
