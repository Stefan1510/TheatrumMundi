using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailManager : MonoBehaviour
{
    #region variables
    public Image timelineImage;
    public GameObject gameController, rail3dObj, menue1;
    public Image timeSliderImage;
    private BoxCollider2D timeSlider;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, isInstance, changedRail, alreadyCountedMinus, alreadyCountedPlus;
    public bool isTimelineOpen;
    Vector2 releaseObjMousePos;
    double minX, minY, maxX;
    float railWidth, publicPosX;
    Vector3 railStartPoint, railEndPoint;
    int maxTimeInSec;
    Vector2[] objectShelfPosition, objectShelfSize;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    Vector2 objectSceneSize;
    float objectAnimationLength;

    public GameObject objectLibrary, UICanvas, parentMenue; // mainMenue
    GameObject timeSettings;
    GameObject[] figCounterCircle, figureObjects, figureObjects3D;
    int currentClickedObjectIndex, currentClickedInstanceObjectIndex, hitTimeline, sizeLayering;
    public int layerOverlaps, count;
    public List<GameObject> timelineInstanceObjects, timelineInstanceObjects3D, figuresLayer1, figuresLayer2, figuresLayer3, figuresLayer4;
    private FigureElement ThisFigureElement;    //element to set 3d object
    float heightOpened, heightClosed;
    Color colFigure, colFigureHighlighted;
    private float currentLossyScale;
    private Vector2 diff;
    #endregion
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
        sizeLayering = 1;

        railStartPoint = new Vector3(0.0f, 0.0f, -2.2f);
        railEndPoint = new Vector3(0.0f, 0.0f, 2.6f);

        //load all objects given in the figuresShelf
        figureObjects = new GameObject[objectLibrary.transform.childCount];         //instead of a count like "3"
        figureObjects3D = new GameObject[figureObjects.Length];
        objectShelfPosition = new Vector2[figureObjects.Length];
        objectShelfSize = new Vector2[figureObjects.Length];
        objectShelfParent = new GameObject[figureObjects.Length];
        figCounterCircle = new GameObject[figureObjects.Length];

        objectAnimationLength = 100.0f;                                             //length of current object-animation

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
        catch (IndexOutOfRangeException) { }

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;
        timeSettings = timeSliderImage.transform.GetChild(0).gameObject;
        currentLossyScale = 0.0f;   // lossyScale is global Scale that I use to get the moment when ScreenSize is being changed (doesn't happen exactly when Screen Size is changed, but somehow shortly after, so I use this parameter instead)

        ResetScreenSize();
        timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x * 1.2f);

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
    public void openTimelineByClick(bool thisTimelineOpen, Image tlImage, bool fromShelf)
    {
        if (fromShelf == false && SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive != 1)
        {
            //Debug.Log("from shelf = false");
            UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf05(true);
        }
        if (isAnyTimelineOpen() == false)
        {
            // Debug.Log("++++ es ist keine schiene geöffnet, deswegen wird geklickte Schiene geöffnet: " + tl);
            //set global flag
            SceneManaging.anyTimelineOpen = true;

            isTimelineOpen = true;
            //scale up timeline
            tlImage.rectTransform.sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
            //scale up the collider
            tlImage.GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
            //minimize or maximize objects on timeline
            openCloseObjectInTimeline(true, timelineInstanceObjects, gameObject);
            ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(tlImage.name[17]) - 1);
            ImageTimelineSelection.SetRailType(0);  // for rail-rails
            // if (!menue1.activeSelf)
            // {
            //     parentMenue.GetComponent<ObjectShelf>().ButtonShelf01();
            // }
        }
        else if (isAnyTimelineOpen())
        {
            if (thisTimelineOpen)
            {
                // if timeline is already open, unhighlight all
                for (int i = 0; i < timelineInstanceObjects3D.Count; i++)
                {
                    highlight(timelineInstanceObjects3D[i], timelineInstanceObjects[i], false);
                }
                /*// Debug.Log("++++ geklickte Schiene ist offen und wird geschlossen: " + tl);
                //close timeline
                isTimelineOpen = false;
                //scale down timeline and collider
                tl.rectTransform.sizeDelta = new Vector2(tl.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                tl.GetComponent<BoxCollider2D>().size = new Vector2(tl.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                openCloseObjectInTimeline(false, timelineInstanceObjects, editTimelineObject);*/
            }
            else
            {
                // if (!menue1.activeSelf)
                // {
                //     parentMenue.GetComponent<ObjectShelf>().ButtonShelf01();
                // }
                //Debug.Log("++++ geklickte Schiene ist zu, aber eine andere ist offen und wird geschlossen: " + tlImage);
                // a different rail is open - close it
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().isTimelineOpen = false;
                    openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects, gameController.GetComponent<UIController>().Rails[i].gameObject);
                    for (int j = 0; j < gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().timelineInstanceObjects3D.Count; j++)
                    {
                        highlight(gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().timelineInstanceObjects3D[j], gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects[j], false);
                    }
                    //Debug.Log("++++ Scaling down Rail: " + gameController.GetComponent<UIController>().Rails[i]);
                }
                for (int j = 0; j < 2; j++)
                {
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RectTransform>().sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects, false);

                for (int j = 0; j < gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects.Count; j++)
                {
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().highlight(gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects[j], false);
                }

                // open clicked rail
                //Debug.Log("++++ geklickte Schiene wird geöffnet: " + tl);
                //scale up timeline
                tlImage.rectTransform.sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
                //scale up the collider
                tlImage.GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
                openCloseObjectInTimeline(true, timelineInstanceObjects, gameObject);
                isTimelineOpen = true;

                ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(tlImage.name[17]) - 1);
                ImageTimelineSelection.SetRailType(0);  // for rail-rails

            }
        }
        openCloseTimeSettings(isAnyTimelineOpen(), timeSettings);

    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects, GameObject timeline)
    {
        float length = objectAnimationLength;

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -gameObject.GetComponent<RectTransform>().rect.height / 2, -1);

            if (timelineOpen)
            {
                if (timeline.GetComponent<RailManager>().sizeLayering == 1)
                {
                    scaleObject(objects[i], length, heightOpened / gameObject.transform.lossyScale.x, false);
                    scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                    scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                }
                else if (timeline.GetComponent<RailManager>().sizeLayering == 2)
                {
                    scaleObject(objects[i], length, heightOpened / gameObject.transform.lossyScale.x / 2, false);
                    scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f / 2, false);
                    scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f / 2, false);
                    scaleObject(objects[i].GetComponent<BoxCollider2D>().gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f / 2, false);
                }
            }
            else
            {
                if (timeline.GetComponent<RailManager>().sizeLayering == 1)
                {
                    scaleObject(objects[i], length, heightClosed / gameObject.transform.lossyScale.x, true);
                    scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightClosed * 0.96f, true); // img
                    scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightClosed * 0.96f, true); // rect
                }
                else if (timeline.GetComponent<RailManager>().sizeLayering == 2)
                {
                    objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -(heightClosed / gameObject.transform.lossyScale.x * 0.96f / 2), -1);

                    scaleObject(objects[i], length, heightClosed / gameObject.transform.lossyScale.x / 2, true);
                    scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightClosed / gameObject.transform.lossyScale.x * 0.96f / 2, true);
                    scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, heightClosed / gameObject.transform.lossyScale.x * 0.96f / 2, true);
                }
            }
        }
    }
    public void openTimelineByDrag(Image tlImage)
    {
        // if (open == "open")
        if (isAnyTimelineOpen() == false)
        {
            isTimelineOpen = true;
            //scale up timeline, collider ,scale up all objects on timeline
            tlImage.rectTransform.sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
            tlImage.GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
            openCloseObjectInTimeline(isTimelineOpen, timelineInstanceObjects, gameObject);
            ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(tlImage.name[17]) - 1);
            ImageTimelineSelection.SetRailType(0);  // for rail-rails
        }
        else
        {
            if (!isTimelineOpen)
            {
                if (gameController.GetComponent<UIController>().RailLightBG[0].isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<RectTransform>().sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[0].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
                if (gameController.GetComponent<UIController>().RailLightBG[1].isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<RectTransform>().sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[1].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
                if (gameController.GetComponent<UIController>().RailMusic.isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<RectTransform>().sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                    gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().timelineInstanceObjects, false);
                }
                for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
                {
                    if (gameController.GetComponent<UIController>().Rails[i].isTimelineOpen)
                    {
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                        gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().isTimelineOpen = false;
                        openCloseObjectInTimeline(false, gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects, gameController.GetComponent<UIController>().Rails[i].gameObject);
                        for (int j = 0; j < gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().timelineInstanceObjects3D.Count; j++)
                        {
                            highlight(gameController.GetComponent<UIController>().Rails[i].GetComponent<RailManager>().timelineInstanceObjects3D[j], gameController.GetComponent<UIController>().Rails[i].timelineInstanceObjects[j], false);
                        }
                    }
                }

                isTimelineOpen = false;
                //scale down timeline, collider, scale up objects on timeline
                tlImage.rectTransform.sizeDelta = new Vector2(tlImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                tlImage.GetComponent<BoxCollider2D>().size = new Vector2(tlImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                openCloseObjectInTimeline(isTimelineOpen, timelineInstanceObjects, gameObject);
            }
            openCloseTimeSettings(isAnyTimelineOpen(), timeSettings);
        }
    }
    public void openCloseTimeSettings(bool tlOpen, GameObject ts)
    {
        //activate the timeSettings at the bottom of the timeslider
        if ((tlOpen)) // && (ts.activeSelf==false)
        {
            ts.SetActive(true);
        }
        else
        {
            ts.SetActive(false);
        }
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
    public void saveParent(GameObject obj)
    {
        GameObject oldParent;
        if (obj.transform.parent != null)
            //save old parent
            oldParent = obj.transform.parent.gameObject;        //parent is a transform, to get the gameobject of this parent, use ".gameObject"
    }
    public void setParent(GameObject obj, GameObject parentToSet)
    {
        try
        {
            saveParent(obj);
            obj.transform.SetParent(parentToSet.transform);
        }
        catch (NullReferenceException)
        { }
    }
    public void updateObjectPosition(GameObject obj, Vector2 mousePos)
    {
        setParent(obj, gameObject);
        obj.transform.position = new Vector3(mousePos.x, mousePos.y, -1.0f);
    }
    public bool checkHittingTimeline(Image tl, Vector2 mousePos)
    {
        bool hit = false;
        Vector2 colSize = new Vector2(GetComponent<BoxCollider2D>().size.x * gameObject.transform.lossyScale.x, GetComponent<BoxCollider2D>().size.y * gameObject.transform.lossyScale.x);
        if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= tl.transform.position.y + (colSize.y / 2.0f) && mousePos.y > tl.transform.position.y - (colSize.y / 2.0f)) hit = true; ;
        return hit;
    }
    public void scaleObject(GameObject fig, float x, float y, bool boxCollZero)
    {
        //scale the object
        fig.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
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
        if (layer == 0) // only one layer
        {
            scaleObject(obj, 100, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);      //scale the figure-picture in timeline to x: 100 and y: 80px
            scaleObject(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x, timeline.GetComponent<RectTransform>().rect.height * 0.96f, false);
            scaleObject(obj.transform.GetChild(1).gameObject, 100, timeline.GetComponent<RectTransform>().rect.height * 0.96f, false);
            obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 0.5f, -1);
            obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, timeline.GetComponent<RectTransform>().rect.height);
            obj.GetComponent<BoxCollider2D>().offset = new Vector2(obj.GetComponent<BoxCollider2D>().offset.x, 0);
        }
        if (layer == 1) // 2 layers, but object in layer 1
        {
            scaleObject(obj, 100, gameObject.GetComponent<RectTransform>().rect.height / 2 * 0.96f, false);
            scaleObject(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, timeline.GetComponent<RectTransform>().rect.height / 2 * 0.96f, false);
            scaleObject(obj.transform.GetChild(1).gameObject, timeline.GetComponent<RectTransform>().rect.height * 0.96f, timeline.GetComponent<RectTransform>().rect.height / 2 * 0.96f, false);
            obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 0, -1);

            obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, timeline.GetComponent<RectTransform>().rect.height / 2);
            obj.GetComponent<BoxCollider2D>().offset = new Vector2(obj.GetComponent<BoxCollider2D>().offset.x, obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
        }
        else if (layer == 2)    // 2 layers and object in layer 2
        {
            scaleObject(obj, 100, gameObject.GetComponent<RectTransform>().rect.height / 2 * 0.96f, false);
            scaleObject(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, timeline.GetComponent<RectTransform>().rect.height / 2 * 0.96f, false);
            scaleObject(obj.transform.GetChild(1).gameObject, timeline.GetComponent<RectTransform>().rect.height * 0.96f, timeline.GetComponent<RectTransform>().rect.height / 2 * 0.96f, false);
            obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 1, -1);
            obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, timeline.GetComponent<RectTransform>().rect.height / 2);
            obj.GetComponent<BoxCollider2D>().offset = new Vector2(obj.GetComponent<BoxCollider2D>().offset.x, -obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
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
    public double calcSecondsToPixel(double animLength, int maxTimeLengthInSec)
    {
        double px = 0;
        double width = maxX - minX;
        px = (width * animLength) / maxTimeLengthInSec;
        return px;
    }
    public void createRectangle(GameObject obj, Color col, double rectHeight) // start pos brauch ich eigentlich nicht
    {
        double tmpLength = 100;//calcSecondsToPixel(objectAnimationLength, maxTimeInSec) / 2 + 25;
        GameObject imgObject = new GameObject("RectBackground");
        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(0, 0);
        trans.SetSiblingIndex(0);
        trans.sizeDelta = new Vector2((float)tmpLength, (float)rectHeight);    //size related to animationLength
        Image image = imgObject.AddComponent<Image>();
        image.color = col;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
    }
    public int calculateFigureStartTimeInSec(GameObject fig, double animLength, int maxTimeLengthInSec, double railMinX, double railMaxX)
    {
        //PROBLEMS:
        //animLength are in seconds but this is not 100 pixel!!! ->calc seconds to pixel is required
        //tmpX is not correct calculated

        //Debug.Log("fig: "+fig+" animLength: "+animLength+" timeLength: "+maxTimeLengthInSec+" railMinX: "+railMinX+" railMaxX: "+railMaxX);
        //        Debug.Log("Mouse position: " + (Input.mousePosition));
        //Debug.Log("figure position: "+fig.transform.position);
        //Debug.Log("box collider: "+fig.GetComponent<BoxCollider2D>().size);
        //Debug.Log("rail3DObj_startX: "+rail3dObj.transform.position.x+" rail3DObj_width: "+rail3dObj.GetComponent<RectTransform>().rect.width);
        int sec = 0;
        /*double tmpX = fig.transform.position.x - railMinX;  //x-pos is screenX from left border, railMinX is the rail-startpoint
        Vector2 tmpSize = fig.GetComponent<BoxCollider2D>().size;
        double tmpMinX = (double)tmpX - (tmpSize.x / 2.0f); //if tmpX is the midpoint of figure
        double tmpMaxX = tmpMinX + animLength;  //length of figure is related to rail speed, here named as animationLength
                                                //get figure minX related to timelineMinX
        double percentageOfRail = tmpMinX / (railMaxX - railMinX);  //max-min=real length, percentage: e.g. 0,3823124 (38%)
        sec = (int)(((double)maxTimeLengthInSec) * percentageOfRail);  */


        //Debug.Log("method: fig: "+fig+" tmpX: "+tmpX+" tmpSize: "+tmpSize+" tmpMinX: "+tmpMinX+" railMaxX: "+railMaxX+" percentage: "+percentageOfRail+" sec: "+sec);
        //Debug.Log("figXPos: " + fig.transform.position.x + " railMinX: " + railMinX);
        // Debug.Log("screen size: " + (double)(50.0f / 1920.0f * (double)Screen.width));
        double tmpX = fig.transform.GetChild(0).gameObject.transform.position.x - (50.0f / 1920.0f * (double)Screen.width);
        //Debug.Log("sizeDelta.x: " + fig.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x + ", rect width: " + fig.transform.GetChild(0).gameObject.GetComponent<RectTransform>().rect.width/2);
        tmpX = tmpX - railMinX;
        //Debug.Log("tmpXBlueRect: " + tmpX);
        double percentageOfRail = (tmpX) / (railMaxX - railMinX);           //percent (between 0 and 1) of blue-rect-startpoint in relation to the complete rail
        sec = (int)(((double)maxTimeLengthInSec) * percentageOfRail);   //seconds of percentage
        return sec;
    }
    public Vector3 getRailStartEndpoint(GameObject r3DObj, string startEnd)
    {
        //this returns the coordinate (not pixel!) values of a rail
        //railStartPoint & railEndPoint = global variables
        Vector3 point = new Vector3(0.0f, 0.0f, 0.0f);
        if (startEnd == "start")
        {
            //calculate start point of the rail
            //point = new Vector3(0.0f, 0.0f, -2.2f);
            point = new Vector3(railStartPoint.x, railStartPoint.y, railStartPoint.z);
        }
        else //(startEnd="end")
        {
            //calculate end point of rail 
            //point = new Vector3(0.0f, 0.0f, 2.6f);
            point = new Vector3(railEndPoint.x, railEndPoint.y, railEndPoint.z);
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

        // erase from layer list
        if (figuresLayer1.Contains(obj))
        {
            figuresLayer1.Remove(obj);
        }
        if (figuresLayer2.Contains(obj))
        {
            figuresLayer2.Remove(obj);
            layerOverlaps--;
            Debug.LogWarning("overlaps: " + layerOverlaps);
        }
        Debug.Log("removed: " + obj);
        // if (figuresLayer3.Contains(obj))
        // {
        //     figuresLayer3.Remove(obj);
        // }
        // if (figuresLayer4.Contains(obj))
        // {
        //     figuresLayer4.Remove(obj);
        // }

        timelineInstanceObjects.Remove(obj);
        timelineInstanceObjects3D.Remove(obj3D);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
        StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements.Remove(StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(obj.name.Substring(17))]);

        Destroy(obj);
        Destroy(obj3D);
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
        // erase from layer list
        if (figuresLayer1.Contains(timelineInstanceObjects[val]))
        {
            figuresLayer1.Remove(timelineInstanceObjects[val]);
        }
        if (figuresLayer2.Contains(timelineInstanceObjects[val]))
        {
            figuresLayer2.Remove(timelineInstanceObjects[val]);
            layerOverlaps--;
        }
        // if (figuresLayer3.Contains(timelineInstanceObjects[val]))
        // {
        //     figuresLayer3.Remove(timelineInstanceObjects[val]);
        // }
        // if (figuresLayer4.Contains(timelineInstanceObjects[val]))
        // {
        //     figuresLayer4.Remove(timelineInstanceObjects[val]);
        // }
        Destroy(obj);
        Destroy(timelineInstanceObjects3D[val]);
        timelineInstanceObjects.Remove(obj);
        timelineInstanceObjects3D.Remove(timelineInstanceObjects3D[val]);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();

        StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements.Remove(StaticSceneData.StaticData.figureElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(obj.name.Substring(17))]);
    }
    public int checkHittingAnyTimeline(GameObject obj, Vector2 mousePos)
    {
        int hit = -1;
        Image[] tl = new Image[gameController.GetComponent<UIController>().Rails.Length];
        for (int i = 0; i < gameController.GetComponent<UIController>().Rails.Length; i++)
        {
            tl[i] = gameController.GetComponent<UIController>().Rails[i].GetComponent<Image>();
            Vector2 tlPos = new Vector2(tl[i].transform.position.x, tl[i].transform.position.y);
            Vector2 colSize = new Vector2(tl[i].GetComponent<BoxCollider2D>().size.x, tl[i].GetComponent<BoxCollider2D>().size.y);
            //if mouse hits the timeline while dragging an object
            //my implementation of object-boundingbox
            if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
            ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
            {
                //Debug.Log("object hits timeline!: " + tl[i]);
                hit = i;
            }
        }
        //Debug.Log("drag and hit " + hit);
        return hit;
    }
    public void highlight(GameObject obj3D, GameObject obj, bool highlightOn)
    {
        if (highlightOn)
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
        else
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
        newCopyOfFigure.transform.SetParent(timelineImage.transform);
        newCopyOfFigure.transform.localScale = Vector3.one;

        if (loadFromFile)
        {
            float posX = UtilitiesTm.FloatRemap(momentOrPosX, 0, AnimationTimer.GetMaxTime(), gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2);
            newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3(posX, figureObjects[figureNr].GetComponent<RectTransform>().anchoredPosition.y, -1);
            gameController.GetComponent<SceneDataController>().objects2dFigureInstances.Add(newCopyOfFigure);
        }
        else
        {
            //------------------------------------------limit front of rail---------------------------------------------//
            if (newCopyOfFigure.transform.position.x < 0.03f * Screen.width)  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
            {
                newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.03f * Screen.width, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
            }
            else
            {
                newCopyOfFigure.transform.position = new Vector3(momentOrPosX, figureObjects[figureNr].transform.position.y, -1);
            }
            //-------------------------------------------------limit back of rail------------------------------------------//
            if ((newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) > (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width))
            {
                newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x), newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
            }

            //set original image back to shelf, position 2 to make it visible
            setParent(figureObjects[figureNr], objectShelfParent[figureNr]);
            scaleObject(figureObjects[figureNr], objectShelfSize[figureNr].x, objectShelfSize[figureNr].y, false);
            scaleObject(figureObjects[figureNr].transform.GetChild(0).gameObject, objectShelfSize[figureNr].x, objectShelfSize[figureNr].y, false);
            figureObjects[figureNr].GetComponent<RectTransform>().anchoredPosition = new Vector2(75.0f, -75.0f);
            figureObjects[figureNr].transform.SetSiblingIndex(1);
        }

        float moment = UtilitiesTm.FloatRemap((newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x - 50), gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2, 0, AnimationTimer.GetMaxTime()) - 100;
        objectAnimationLength = rail3dObj.transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
        if (float.IsInfinity(objectAnimationLength))
        {
            objectAnimationLength = 100;
        }
        createRectangle(newCopyOfFigure, colFigure, gameObject.GetComponent<RectTransform>().rect.height * 0.96f);

        //////////////////////////////////////////////// calculating Layer Overlap ////////////////////////////////////////////

        if (isCreatedFigureOverlapping(newCopyOfFigure) == 1)   // layer 2 contains overlapping object
        {
            float posX = 0;

            bool val = true;    // layer 1 ist frei
            for (int j = 0; j < figuresLayer1.Count; j++)  // abfrage, ob auf layer 1 eine figur ist
            {
                if (newCopyOfFigure.GetComponent<RectTransform>().position.x - figuresLayer1[j].GetComponent<RectTransform>().position.x < figuresLayer1[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x)
                {
                    val = false;
                    posX = figuresLayer1[j].GetComponent<RectTransform>().anchoredPosition.x + figuresLayer1[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
                }
            }
            if (val)
            {
                scaleToLayerSize(newCopyOfFigure, 1, gameObject);
                newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.x + 25, newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.y, -1);
                figuresLayer1.Add(newCopyOfFigure);

            }
            else
            {
                layerOverlaps++;
                newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.x + 25, newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.y, -1);
                if (publicPosX < posX)
                {
                    scaleToLayerSize(newCopyOfFigure, 2, gameObject);
                    figuresLayer2.Add(newCopyOfFigure);
                    newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3(publicPosX, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
                }
                else
                {
                    scaleToLayerSize(newCopyOfFigure, 1, gameObject);
                    figuresLayer1.Add(newCopyOfFigure);
                    newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3(posX, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
                }
            }
        }
        else if (isCreatedFigureOverlapping(newCopyOfFigure) == 2)  // layer 1 contains overlapping object
        {
            float posX = 0;
            bool val = true;    // layer 2 ist frei
            for (int j = 0; j < figuresLayer2.Count; j++)  // abfrage, ob auf layer 2 eine figur ist
            {
                if (newCopyOfFigure.GetComponent<RectTransform>().position.x - figuresLayer2[j].GetComponent<RectTransform>().position.x < figuresLayer2[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x)
                {
                    val = false;
                    posX = figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x + figuresLayer2[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
                }
            }
            if (val)
            {
                layerOverlaps++;
                scaleToLayerSize(newCopyOfFigure, 2, gameObject);
                //newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.x + 25, newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.y, -1);
                figuresLayer2.Add(newCopyOfFigure);
                if (sizeLayering == 1)
                {
                    // scale down all the other figures
                    for (int i = 0; i < timelineInstanceObjects.Count; i++)
                    {
                        if (figuresLayer1.Contains((timelineInstanceObjects[i])))
                        {
                            scaleToLayerSize(timelineInstanceObjects[i], 1, gameObject);
                            // size of rectangle becomes size for figure that is clickable
                            timelineInstanceObjects[i].transform.GetComponent<BoxCollider2D>().size = timelineInstanceObjects[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
                            timelineInstanceObjects[i].transform.GetComponent<BoxCollider2D>().offset = new Vector2(timelineInstanceObjects[i].transform.GetComponent<BoxCollider2D>().size.x / 2 - 50f, timelineInstanceObjects[i].transform.GetComponent<BoxCollider2D>().offset.y);
                        }
                    }
                    sizeLayering = 2;
                }
            }
            else    // layer 2 ist nicht frei
            {
                newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.x + 25, newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position.y, -1);
                if (publicPosX < posX)
                {
                    newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3(publicPosX, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
                    scaleToLayerSize(newCopyOfFigure, 1, gameObject);
                    figuresLayer1.Add(newCopyOfFigure);
                }
                else
                {
                    newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3(posX, newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
                    scaleToLayerSize(newCopyOfFigure, 2, gameObject);
                    figuresLayer2.Add(newCopyOfFigure);
                }
            }
        }
        else
        {
            figuresLayer1.Add(newCopyOfFigure);
            if (sizeLayering == 2)
            {
                scaleToLayerSize(newCopyOfFigure, 1, gameObject);
            }
            else //if (sizeLayering == 1)
            {
                scaleToLayerSize(newCopyOfFigure, 0, gameObject);
            }
            //newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.x + 25, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.y, -1);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // size of rectangle becomes size for figure that is clickable
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2 - 50f, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);


        //set 3d object to default position
        GameObject curr3DObject = Instantiate(figureObjects3D[figureNr]);
        timelineInstanceObjects3D.Add(curr3DObject);
        setParent(timelineInstanceObjects3D[timelineInstanceObjects3D.Count - 1], rail3dObj.transform.GetChild(0).gameObject);

        // if (!loadFromFile)
        // {
        //     newCopyOfFigure.transform.GetChild(0).localPosition = new Vector3(newCopyOfFigure.transform.GetChild(0).localPosition.x - 25, newCopyOfFigure.transform.GetChild(0).localPosition.y, newCopyOfFigure.transform.GetChild(0).localPosition.z);
        // }

        if (isTimelineOpen)
        {
            openTimelineByClick(true, timelineImage, true);
        }
        else
        {
            openTimelineByClick(false, timelineImage, true);
        }
        return curr3DObject;
    }
    public void ResetScreenSize()       // this probably has to be called globally, so that every Menue resizes (probably in the UIController). At the moment it is only scaled properly when rail tab is open e.g.
    {
        // I used the values that were put in FullHD (global: position.x) and calculated the percentage so that it works for all resolutions 
        minX = 0.087f * Screen.width;               //timeline-rail-minX
        railWidth = 0.87f * Screen.width;           //railwidth=1670.4px
        heightClosed = 0.018f * Screen.height;
        heightOpened = 0.074f * Screen.height;
        maxX = minX + railWidth;                    //timeline-rail-maxX

        if (isTimelineOpen)
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightOpened / gameObject.transform.lossyScale.x);
            //Debug.Log("size box collider: " + gameObject.GetComponent<BoxCollider2D>().size.y);
        }
        else
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x * 1.2f);
        }
    }
    public bool isSomethingOverlapping()
    {
        bool val = false;
        for (int i = 0; i < timelineInstanceObjects.Count; i++)
        {
            for (int j = 0; j < timelineInstanceObjects.Count; j++)
            {
                if (timelineInstanceObjects[j] != timelineInstanceObjects[i] && (timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x > timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x
                && (timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x <= (timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[i].GetComponent<BoxCollider2D>().size.x))
                || ((timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[j].GetComponent<BoxCollider2D>().size.x) >= timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x)
                && timelineInstanceObjects[j].GetComponent<RectTransform>().anchoredPosition.x < timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x))
                {
                    val = true;
                }
            }
        }
        return val;
    }
    public int isCreatedFigureOverlapping(GameObject obj)
    {
        int val = 0;
        for (int i = 0; i < timelineInstanceObjects.Count; i++)
        {
            if (obj != timelineInstanceObjects[i] && (obj.GetComponent<RectTransform>().anchoredPosition.x > timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x        // abfrage, dass nicht gleiche figur und rechts von ihr
            && (obj.GetComponent<RectTransform>().anchoredPosition.x <= (timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x + 100))
            || ((obj.GetComponent<RectTransform>().anchoredPosition.x + obj.GetComponent<BoxCollider2D>().size.x) >= timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x)
            && obj.GetComponent<RectTransform>().anchoredPosition.x < timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x))
            {
                if (figuresLayer2.Contains(timelineInstanceObjects[i]))
                {
                    if (figuresLayer1.Contains(obj)) val = 3;
                    else if (figuresLayer2.Contains(obj)) val = 5;
                    else val = 1;

                    publicPosX = timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x + 1;

                }
                else if (figuresLayer1.Contains(timelineInstanceObjects[i]))
                {
                    if (figuresLayer2.Contains(obj)) val = 5;
                    else if (figuresLayer1.Contains(obj)) val = 6;
                    else val = 2;
                    publicPosX = timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x + 1;
                }
            }
        }
        return val;
    }
    void Update()
    {
        if (currentLossyScale != transform.lossyScale.x)    // Abfrage, ob sich lossyScale geaendert hat dann werden die Schienen rescaled
        {
            currentLossyScale = transform.lossyScale.x;
            ResetScreenSize();
        }
        Vector2 getMousePos = Input.mousePosition;
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
                openTimelineByClick(isTimelineOpen, timelineImage, false);
            }
            //if you click on an object in shelf 
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                //diff = new Vector2(getMousePos.x - figureObjects[currentClickedObjectIndex].transform.position.x, getMousePos.y - figureObjects[currentClickedObjectIndex].transform.position.y);
                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    draggingObject = true;
                }
            }
            //or check if you click an object in timeline
            if ((currentClickedInstanceObjectIndex != (-1)) && (editTimelineObject == true) && isTimelineOpen)
            {
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    diff = new Vector2(getMousePos.x - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, getMousePos.y - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.y);
                    //set up some flags
                    //draggingObject = true;
                    draggingOnTimeline = true;

                    //highlighting objects and showing delete button when clicked
                    if (SceneManaging.highlighted == false)
                    {
                        highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex], true);
                        //timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects3D[currentClickedInstanceObjectIndex]));//, currentClickedObjectIndex));
                    }
                    else if (SceneManaging.highlighted && timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<Image>().color == colFigureHighlighted) // checkFigureHighlighted(timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject))  // check if second child (which is never the armature) has emission enabled (=is highlighted)
                    {
                        // highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex], false);
                    }
                    else
                    {
                        for (int i = 0; i < timelineInstanceObjects3D.Count; i++)
                        {
                            highlight(timelineInstanceObjects3D[i], timelineInstanceObjects[i], false);
                        }
                        highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex], true);
                        // timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects3D[currentClickedInstanceObjectIndex]));//, currentClickedObjectIndex));
                    }
                }
            }

            //if you hit/clicked nothing with mouse
            if (Physics2D.OverlapPoint(getMousePos) == false)
            {
                // delete highlight of everything if nothing is clicked
                for (int i = 0; i < timelineInstanceObjects3D.Count; i++)
                {
                    highlight(timelineInstanceObjects3D[i], timelineInstanceObjects[i], false);
                }
            }
        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline && isTimelineOpen)
        {
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                isInstance = true;
                //if you click an object in timeline (for dragging)
                updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos - diff); //snapping/lock y-axis
                setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], this.transform.position.y);

                //---------------------------------------------------------------------Stapeln von Ebenen-----------------------------------------------------
                if (sizeLayering == 1)  // if there is only one layer on Rail
                {
                    if (isSomethingOverlapping())
                    {
                        scaleToLayerSize(timelineInstanceObjects[currentClickedInstanceObjectIndex], 2, gameObject);

                        figuresLayer1.Remove(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                        figuresLayer2.Add(timelineInstanceObjects[currentClickedInstanceObjectIndex]);

                        // scale all other timelineobjects of layer 1
                        for (int j = 0; j < figuresLayer1.Count; j++)
                        {
                            if (figuresLayer1[j] != timelineInstanceObjects[currentClickedInstanceObjectIndex]) scaleToLayerSize(figuresLayer1[j], 1, gameObject);
                        }

                        if (!alreadyCountedPlus)
                        {
                            layerOverlaps++;
                            alreadyCountedPlus = true;
                        }
                        sizeLayering = 2;
                        alreadyCountedMinus = false;
                    }

                }

                else if (sizeLayering == 2) // if there are two layers on Rail
                {
                    for (int i = 0; i < timelineInstanceObjects.Count; i++)
                    {
                        if (!isSomethingOverlapping())
                        {
                            if (!alreadyCountedMinus && layerOverlaps > 0)
                            {
                                layerOverlaps--;
                                alreadyCountedMinus = true;
                            }
                        }
                        else if (isCreatedFigureOverlapping(timelineInstanceObjects[currentClickedInstanceObjectIndex]) == 3)   // obj layer 1, collision layer 2
                        {
                            int val = 0;    // layer 1 ist frei
                            for (int j = 0; j < figuresLayer1.Count; j++)  // abfrage, ob auf layer 1 eine figur ist
                            {
                                if (timelineInstanceObjects[currentClickedInstanceObjectIndex] != figuresLayer1[j] &&
                                (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x - figuresLayer1[j].GetComponent<RectTransform>().anchoredPosition.x <= figuresLayer1[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x)
                                && timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x >= figuresLayer1[j].GetComponent<RectTransform>().anchoredPosition.x)
                                {
                                    val = 1;
                                    publicPosX = figuresLayer1[j].GetComponent<RectTransform>().anchoredPosition.x + figuresLayer1[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
                                }
                                else if (timelineInstanceObjects[currentClickedInstanceObjectIndex] != figuresLayer1[j] &&
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x < figuresLayer1[j].GetComponent<RectTransform>().anchoredPosition.x &&
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x >= figuresLayer1[j].GetComponent<RectTransform>().anchoredPosition.x)
                                {
                                    val = 2;
                                    publicPosX = figuresLayer1[j].GetComponent<RectTransform>().anchoredPosition.x - figuresLayer1[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
                                }
                            }
                            if (val != 0) timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(publicPosX, timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().anchoredPosition.y, -1);
                        }
                        else if (isCreatedFigureOverlapping(timelineInstanceObjects[currentClickedInstanceObjectIndex]) == 5)   // obj auf layer 2, collision auf 1
                        {
                            int val = 0;    // layer 2 ist frei
                            for (int j = 0; j < figuresLayer2.Count; j++)  // abfrage, ob auf layer2 eine figur ist
                            {
                                if (timelineInstanceObjects[currentClickedInstanceObjectIndex] != figuresLayer2[j] &&
                                (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x - figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x <= figuresLayer2[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x
                                && timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x >= figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x))
                                {
                                    val = 1;
                                    publicPosX = figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x + figuresLayer2[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
                                }
                                else if (timelineInstanceObjects[currentClickedInstanceObjectIndex] != figuresLayer2[j] &&
                                (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x < figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x &&
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x >= figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x))
                                {
                                    val = 2;
                                    publicPosX = figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x - figuresLayer2[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
                                }
                            }
                            if (val != 0) timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(publicPosX, timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().anchoredPosition.y, -1);
                        }
                        else if (isCreatedFigureOverlapping(timelineInstanceObjects[currentClickedInstanceObjectIndex]) == 6)   // beide auf layer 1
                        {
                            int val = 0;    // layer 2 ist frei
                            for (int j = 0; j < figuresLayer2.Count; j++)  // abfrage, ob auf layer 2 eine figur ist
                            {
                                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x >= figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x &&
                                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x - figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x <= figuresLayer2[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x)
                                {
                                    val = 1;
                                }
                                else if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x < figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x &&
                                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().sizeDelta.x >= figuresLayer2[j].GetComponent<RectTransform>().anchoredPosition.x)
                                {
                                    val = 2;
                                    publicPosX = publicPosX - (2 * timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x);
                                }
                            }
                            if (val == 0)
                            {
                                figuresLayer1.Remove(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                                figuresLayer2.Add(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                                // change pos y
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().pivot = new Vector3(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().pivot.x, 1, -1);
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>().offset = new Vector2(timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>().offset.x, -timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                                layerOverlaps++;
                            }
                            else
                            {
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(publicPosX, timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().anchoredPosition.y, -1);
                            }
                        }

                        else
                        {
                            if (figuresLayer2.Contains(timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                            {
                                layerOverlaps--;
                                figuresLayer2.Remove(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                                figuresLayer1.Add(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().pivot = new Vector3(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<RectTransform>().pivot.x, 0, -1);
                                timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>().offset = new Vector2(timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>().offset.x, timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                            }
                        }
                        if (layerOverlaps == 0)
                        {
                            //scale all timelineobjects
                            for (int j = 0; j < timelineInstanceObjects.Count; j++)
                            {
                                scaleToLayerSize(timelineInstanceObjects[j], 0, gameObject);
                                if (figuresLayer2.Contains(timelineInstanceObjects[j]))
                                {
                                    figuresLayer2.Remove(timelineInstanceObjects[j]);
                                    figuresLayer1.Add(timelineInstanceObjects[j]);
                                }
                            }
                            sizeLayering = 1;
                            alreadyCountedPlus = false;
                        }
                    }
                }

                // ----------------------------------------------------------------------------------------------------------------------------------------------

                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x - (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().sizeDelta.x / 2) <= 0)
                {
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector3((timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().sizeDelta.x / 2), timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.y, -1);
                }

                //-------------------------------------------------limit back of rail------------------------------------------//
                if ((timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<BoxCollider2D>().size.x) > (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width))
                {
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<BoxCollider2D>().size.x), timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.y, -1);
                }

                if (Physics2D.OverlapPoint(getMousePos) == false)       // mouse outside
                {
                    SceneManaging.objectsTimeline = ((int)Char.GetNumericValue(timelineImage.name[17]) - 1);  // save old timeline to remove instance of this timeline
                    draggingObject = true;
                    editTimelineObject = false;
                    draggingOnTimeline = false;

                    // delete button ausblenden
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            releaseOnTimeline = true;
        }
        //if something has been dragged outside of the timeline
        if (draggingOnTimeline == false && editTimelineObject == false && draggingObject && isInstance)
        {
            // moving on mouse pos
            timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position = new Vector3(getMousePos.x, getMousePos.y, -1.0f);

            hitTimeline = checkHittingAnyTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos);

            if (hitTimeline != -1)  // if a timeline is hit
            {
                //open timeline, if its not open
                gameController.GetComponent<UIController>().Rails[hitTimeline].openTimelineByDrag(gameController.GetComponent<UIController>().Rails[hitTimeline].GetComponent<Image>());

                // scale down the dragged figure (and childobject: image)
                scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex], 100f, heightOpened, false);
                double tmpLength = calcSecondsToPixel(objectAnimationLength, maxTimeInSec);
                scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject, timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x, heightOpened, false);    // rect
                scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject, heightOpened, heightOpened, false);    // sprite of figure

                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
                changedRail = true;
            }

            else // if no timeline is hit
            {
                //temporarily change parent, so that object appears in front of the shelf
                setParent(timelineInstanceObjects[currentClickedInstanceObjectIndex], parentMenue);
                //scale up the dragged figure and childobjects (button and rect)
                scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex], 150, 150, false);
                scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject, 150, 150, false);
                timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(false);
                //scaleObject(timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject, 150, 150, false);

                releaseOnTimeline = false;
                changedRail = false;
            }
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
                SceneManaging.timelineHit = tmp;
                openTimelineByDrag(timelineImage);

                float figPictureSize = 100.0f;      //length/width of figure-picture in timeline
                                                    //figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(animationLength,scaleYUp);
                                                    //scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleYUp);
                                                    //scale down the dragged figure (and childobject: image)
                if (sizeLayering == 1)
                {
                    scaleObject(figureObjects[currentClickedObjectIndex], figPictureSize, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, figPictureSize, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                }
                else if (sizeLayering == 2)
                {
                    scaleObject(figureObjects[currentClickedObjectIndex], figPictureSize, gameObject.GetComponent<RectTransform>().rect.height * 0.96f / 2, false);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, figPictureSize, gameObject.GetComponent<RectTransform>().rect.height * 0.96f / 2, false);
                }
                // change parent back
                setParent(figureObjects[currentClickedObjectIndex], gameObject);

                //snapping/lock y-axis
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], this.transform.position.y);

                //save position, where object on timeline is released + set flag 
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }

            else
            {
                if (SceneManaging.timelineHit == tmp)
                {
                    scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
                }
                //close timeline if you click e.g. in the shelf to get a new figure
                // openTimelineByDrag("close", timelineImage);
                releaseOnTimeline = false;
            }
        }
        //-------release mousebutton
        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            draggingObject = false;
            editTimelineObject = false;

            // if dropped on a timeline
            if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)
                {
                    if (changedRail)    // if rail has been changed
                    {
                        if (hitTimeline == (int.Parse(gameObject.name.Substring(17)) - 1))  // if rail is the same as before
                        {
                            timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.SetParent(gameObject.transform);
                            timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(true);        // set rect active
                                                                                                                                                //von timeline obj alt loeschen, zu neu hinzufuegen
                            setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], gameObject.transform.position.y);
                            //3D object
                            timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.SetParent(rail3dObj.transform.GetChild(0));

                            // hier muss noch das rechteck neu berechnet werden
                        }
                        else
                        {
                            // organise layers
                            if (figuresLayer2.Contains(timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                            {
                                figuresLayer2.Remove(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            }
                            else if (figuresLayer1.Contains(timelineInstanceObjects[currentClickedInstanceObjectIndex]))
                            {
                                figuresLayer1.Remove(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            }


                            //2D object
                            timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.SetParent(gameController.GetComponent<UIController>().Rails[hitTimeline].transform);
                            timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(true);        // set rect active

                            if (gameController.GetComponent<UIController>().Rails[hitTimeline].sizeLayering == 1)
                            {
                                scaleToLayerSize(timelineInstanceObjects[currentClickedInstanceObjectIndex], 0, gameController.GetComponent<UIController>().Rails[hitTimeline].gameObject);
                                gameController.GetComponent<UIController>().Rails[hitTimeline].figuresLayer1.Add(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
                            }

                            //von timeline obj alt loeschen, zu neu hinzufuegen
                            updateObjectList(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects, timelineInstanceObjects[currentClickedInstanceObjectIndex]);

                            //snapping/lock y-axis
                            setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], gameController.GetComponent<UIController>().Rails[hitTimeline].transform.position.y);
                            timelineInstanceObjects.Remove(timelineInstanceObjects[currentClickedInstanceObjectIndex]);

                            //3D object
                            timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.SetParent(gameController.GetComponent<SceneDataController>().objectsRailElements[hitTimeline].transform.GetChild(0));
                            gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects3D.Add(timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
                            timelineInstanceObjects3D.Remove(timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
                            //Debug.Log("staticSceneData: " + StaticSceneData.StaticData.figureElements[Int32.Parse(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects.Count - 1].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects.Count - 1].name.Substring(17))].railStart);
                            StaticSceneData.StaticData.figureElements[Int32.Parse(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects.Count - 1].name.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects[gameController.GetComponent<UIController>().Rails[hitTimeline].timelineInstanceObjects.Count - 1].name.Substring(17))].railStart = hitTimeline;

                            changedRail = false;
                        }
                    }
                    else
                    {
                        GameObject curr3DObject;
                        //create a copy of this timelineObject and keep the original one
                        curr3DObject = CreateNew2DInstance(currentClickedObjectIndex, getMousePos.x, false);
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////
                        // Save to SceneData:
                        FigureInstanceElement thisFigureInstanceElement = new FigureInstanceElement();
                        thisFigureInstanceElement.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex]); //index
                        thisFigureInstanceElement.name = curr3DObject.name + "_" + countCopiesOfObject(figureObjects[currentClickedObjectIndex]).ToString("000");
                        thisFigureInstanceElement.railStart = (int)Char.GetNumericValue(timelineImage.name[17]) - 1; //railIndex
                        StaticSceneData.StaticData.figureElements[currentClickedObjectIndex].figureInstanceElements.Add(thisFigureInstanceElement);
                        gameController.GetComponent<SceneDataController>().objects3dFigureInstances.Add(curr3DObject);
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    }
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

            // if instance is dropped somewhere else than on timeline: delete instance
            else if (releaseOnTimeline == false && currentClickedInstanceObjectIndex != -1 && isInstance)
            {
                removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects3D[currentClickedInstanceObjectIndex]);
            }

            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], objectAnimationLength, maxTimeInSec, minX, maxX);

                float moment = UtilitiesTm.FloatRemap(timelineInstanceObjects[i].transform.localPosition.x, gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2, 0, AnimationTimer.GetMaxTime());
                float zPosFigure = (rail3dObj.transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime((float)startSec));

                if (zPosFigure < 0) // wenn die Figur auf eine Position noch vor dem empty gesetzt werden würde, würde sie erscheinen bevor sie auf den rails da ist
                {
                    zPosFigure = 500f;      // deshalb wird die Figur komplett außerhalb der Szene abgesetzt.
                }
                //Debug.Log("SUBSTRING: "+rail3dObj.name.Substring(7));
                if (rail3dObj.name.Substring(7) == "1" || rail3dObj.name.Substring(7) == "3" || rail3dObj.name.Substring(7) == "5")
                {
                    //timelineInstanceObjects3D[i].transform.localPosition = new Vector3(-rail3dObj.transform.GetChild(0).transform.localPosition.x, (-rail3dObj.transform.GetChild(0).transform.localPosition.y - 0.01f), (rail3dObj.transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime((float)startSec)) / 10);
                    timelineInstanceObjects3D[i].transform.localPosition = new Vector3(-rail3dObj.transform.GetChild(0).transform.localPosition.x, (-rail3dObj.transform.GetChild(0).transform.localPosition.y - 0.01f), zPosFigure);
                }
                else
                {
                    timelineInstanceObjects3D[i].transform.localPosition = new Vector3(rail3dObj.transform.GetChild(0).transform.localPosition.x, (-rail3dObj.transform.GetChild(0).transform.localPosition.y - 0.01f), zPosFigure);
                }

                //this is for: (kris) damit die Figuren auch in die Richtung schauen, in die sie laufen
                if (rail3dObj.transform.GetChild(0).GetComponent<RailSpeedController>().railIndex % 2 == 1)
                {
                    timelineInstanceObjects3D[i].transform.localEulerAngles = new Vector3(timelineInstanceObjects3D[i].transform.localEulerAngles.x, 270, timelineInstanceObjects3D[i].transform.localEulerAngles.z);
                }
                // Debug.Log("rail pos: " + (-rail3dObj.transform.GetChild(0).transform.localPosition.x));
                // Debug.Log("obj3D on rail pos: " + (-rail3dObj.transform.GetChild(0).transform.localPosition));
                // Debug.Log("instObj3D on rail pos: " + (timelineInstanceObjects3D[i].transform.localPosition));
                // Debug.Log("GetDistAtTime: " + (rail3dObj.transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime((float)startSec)));
                // Debug.Log("startSec: " + ((float)startSec));
                // Debug.Log("GetDistAtTime/10: " + ((rail3dObj.transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime((float)startSec)) / 10));
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

        // enable binnenanimation when playing
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

    public void PublicUpdate()
    {
        Update();
    }
}
