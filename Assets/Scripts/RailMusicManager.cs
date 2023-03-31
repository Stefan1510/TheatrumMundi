using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RailMusicManager : MonoBehaviour
{
    #region public variables
    [HideInInspector] public List<ObjectsPoint> myObjectsPositionListLayer1 = new List<ObjectsPoint>();
    [HideInInspector] public List<ObjectsPoint> myObjectsPositionListLayer2 = new List<ObjectsPoint>();
    public Image timelineImage;
    public GameObject gameController, UICanvas, contentRailsMenue;
    [HideInInspector] public bool isTimelineOpen;
    public GameObject objectLibrary, parentMenue; // mainMenue
    [HideInInspector] public List<GameObject> timelineInstanceObjects, figuresLayer1, figuresLayer2;
    public int sizeLayering = 1;
    public AudioClip[] clip;         // ought to be 6 audioclips
    #endregion
    #region private variables
    [SerializeField] private BoxCollider2D _backgroundBoxCollider;
    Vector2 sizeDeltaAsFactor;
    AudioSource audioSource;
    Vector2[] objectShelfPosition;
    Vector2 objectShelfSize;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, playingMusic, isInstance, playingSample, fading;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    Color colMusic, colMusicHighlighted;
    //float heightClosed, heightOpened;
    bool anyInstanceIsPlaying = false, firstTimeSecond = false;
    bool onlyPiecefinished = true, _toBeRemoved;
    Vector2 releaseObjMousePos, diff;
    double minX, maxX;
    private float railWidthAbsolute = 1670.4f;
    int maxTimeInSec, currentClip;
    GameObject[] figCounterCircle, figureObjects;
    int currentClickedObjectIndex, sampleButtonPressed;
    int currentClickedInstanceObjectIndex;
    private float currentLossyScale, tmpTime;
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
        timelineImage = this.GetComponent<Image>();
        minX = timelineImage.transform.position.x;
        maxX = minX + timelineImage.GetComponent<RectTransform>().sizeDelta.x;
        clip = gameController.GetComponent<SceneDataController>().objectsMusicClips;
        draggingOnTimeline = false;
        draggingObject = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        playingMusic = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        currentClip = 0;
        isTimelineOpen = false;
        sizeLayering = 1;
        isInstance = false;
        sampleButtonPressed = -1;

        //load all objects given in the figuresShelf
        figureObjects = new GameObject[objectLibrary.transform.childCount];
        objectShelfPosition = new Vector2[figureObjects.Length];
        objectShelfParent = new GameObject[figureObjects.Length];
        figCounterCircle = new GameObject[figureObjects.Length];

        colMusic = new Color(0.21f, 0.51f, 0.267f, 0.5f);
        colMusicHighlighted = new Color(0.21f, 0.81f, 0.267f, 0.5f);

        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = (objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject);
            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
        }
        objectShelfSize = new Vector2(190, 146);

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;
        currentLossyScale = 1.0f;

        ResetScreenSize();
        timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidthAbsolute, 20);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidthAbsolute, 20);

        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
            figCounterCircle[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
        }

        List<GameObject> timelineObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects = new List<GameObject>();
        List<GameObject> figuresLayer1 = new List<GameObject>();
        List<GameObject> figuresLayer2 = new List<GameObject>();

        maxTimeInSec = (int)AnimationTimer.GetMaxTime();
        sizeDeltaAsFactor = new Vector2(1.0f, 1.0f);
    }
    private string identifyClickedObject()   //old
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
                return figureObjects[i].GetComponent<BoxCollider2D>().name;
            }
            else
            {
                objName = "no object clicked!";
            }
        }

        return objName;
    }
    private string identifyClickedObjectByList(List<GameObject> objectsOnTimeline)
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
                return objectsOnTimeline[i].GetComponent<BoxCollider2D>().name;
            }
            else
            {
                objName = "no object clicked!";
            }
        }

        return objName;
    }
    public void openTimelineByClick(bool thisTimelineOpen, bool fromShelf)
    {
        if (fromShelf == false && SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive != 3)
        {
            UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf07(true);
        }
        if (thisTimelineOpen)
        {
            for (int i = 0; i < timelineInstanceObjects.Count; i++)
            {
                highlight(timelineInstanceObjects[i], false);
            }
        }
        else
        {
            // a different rail is open - close it
            for (int i = 0; i < contentRailsMenue.GetComponent<RailManager>().railList.Length; i++)
            {
                if (contentRailsMenue.GetComponent<RailManager>().railList[i].isTimelineOpen)
                {
                    contentRailsMenue.GetComponent<RailManager>().rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 20);
                    contentRailsMenue.GetComponent<RailManager>().rails[i].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 20);
                    contentRailsMenue.GetComponent<RailManager>().railList[i].isTimelineOpen = false;
                    for (int j = 0; j < contentRailsMenue.GetComponent<RailManager>().railList[i].myObjects.Count; j++)
                    {
                        contentRailsMenue.GetComponent<RailManager>().highlight(contentRailsMenue.GetComponent<RailManager>().railList[i].myObjects[j].figure3D, contentRailsMenue.GetComponent<RailManager>().railList[i].myObjects[j].figure, false);
                        contentRailsMenue.GetComponent<RailManager>().openCloseObjectInTimeline(true, contentRailsMenue.GetComponent<RailManager>().railList[i].myObjects, i);
                    }
                }
                contentRailsMenue.GetComponent<RailManager>().openCloseObjectInTimeline(false, contentRailsMenue.GetComponent<RailManager>().railList[i].myObjects, i);
            }
            for (int k = 0; k < 2; k++)
            {
                if (gameController.GetComponent<UIController>().RailLightBG[k].isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 20);
                    gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 20);
                    gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<RailLightManager>().isTimelineOpen = false;
                }

            }
            // open clicked rail and scale up timeline
            timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 80);
            //scale up the collider
            timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 80);
            openCloseObjectInTimeline(true, timelineInstanceObjects);
            isTimelineOpen = true;
            ImageTimelineSelection.SetRailNumber(6);
            ImageTimelineSelection.SetRailType(2);  // for rail-rails
        }
    }
    private void setParent(GameObject obj, GameObject parentToSet)
    {
        try
        {
            GameObject oldParent;
            if (obj.transform.parent != null)
                //save old parent
                oldParent = obj.transform.parent.gameObject;
            //set new parent
            obj.transform.SetParent(parentToSet.transform);
        }
        catch (NullReferenceException) { }
    }
    private void updateObjectPosition(GameObject obj, Vector2 mousePos)
    {
        //set parent and move object
        setParent(obj, gameObject);
        obj.transform.position = new Vector2(mousePos.x, mousePos.y);
    }
    private bool checkHittingTimeline(GameObject obj, Vector2 mousePos)
    {
        bool hit = false;
        Vector2 colSize = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);
        //if mouse hits the timeline while dragging an object
        if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= timelineImage.transform.position.y + (colSize.y / 2.0f) && mousePos.y > timelineImage.transform.position.y - (colSize.y / 2.0f))
        {
            hit = true; ;
        }
        return hit;
    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -GetComponent<RectTransform>().rect.height / 2, -1);
            //Debug.Log("height: " + .GetComponent<RectTransform>().rect.height);

            if (timelineOpen)
            {
                if (sizeLayering == 1)
                {
                    scaleToLayerSize(objects[i], 0);
                }
                else if (sizeLayering == 2)
                {
                    if (figuresLayer1.Contains(objects[i]))
                    {
                        scaleToLayerSize(objects[i], 1);
                        objects[i].GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].GetComponent<BoxCollider2D>().offset.x, GetComponent<RectTransform>().rect.height / 4);
                    }
                    else
                    {
                        scaleToLayerSize(objects[i], 2);
                        objects[i].GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].GetComponent<BoxCollider2D>().offset.x, -GetComponent<RectTransform>().rect.height / 4);
                    }
                }
            }
            else
            {
                if (sizeLayering == 1)
                {
                    scaleObject(objects[i], objects[i].GetComponent<RectTransform>().sizeDelta.x, 20, true);
                }
                else if (sizeLayering == 2)
                {
                    objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -10, -1);

                    scaleObject(objects[i], objects[i].GetComponent<RectTransform>().sizeDelta.x, 10, true);
                }
            }
        }
    }
    private void scaleObject(GameObject fig, float x, float y, bool boxCollZero)
    {
        //scale the object
        fig.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        fig.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        //Debug.Log("fig 1: " + fig.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x + ", x: " + x);
        fig.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(100, y);
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
    }
    private void setObjectOnTimeline(GameObject fig, float x, float y)
    {
        fig.transform.position = new Vector2(fig.transform.position.x, y);
        fig.transform.localPosition = new Vector3(fig.transform.localPosition.x, fig.transform.localPosition.y, -1);
    }
    private void updateObjectList(List<GameObject> objects, GameObject obj)
    {
        //check if the object is already in list
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
        }
    }
    private void createRectangle(GameObject obj, float height, double animLength)
    {
        //double tmpLength = (maxX - minX) / 614.0f * animLength;
        //animLength -= 25;
        GameObject imgObject = new GameObject("RectBackground");
        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;

        trans.sizeDelta = new Vector2((float)animLength, height); // custom size
        //Debug.Log("size: " + animLength);

        trans.pivot = new Vector2(0.0f, 0.5f);
        //set pivot point of sprite  to left border, so that rect aligns with sprite
        trans.anchorMin = trans.anchorMax = new Vector2(0, 0.5f);
        trans.anchoredPosition = new Vector2(0, 0);

        trans.SetSiblingIndex(0);

        Image image = imgObject.AddComponent<Image>();
        image.color = colMusic;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
    }
    private int calculateFigureStartTimeInSec(GameObject fig, double animLength, int maxTimeLengthInSec)
    {
        int sec = 0;
        double tmpX = fig.GetComponent<RectTransform>().position.x - minX + fig.GetComponent<RectTransform>().sizeDelta.x / 2;  //x-pos is screenX from left border, minX is the rail-startpoint
        Vector2 tmpSize = fig.GetComponent<RectTransform>().sizeDelta;
        double tmpMinX = (double)tmpX - (tmpSize.x / 2.0f); //if tmpX is the midpoint of figure
        //get figure minX related to timelineMinX
        double percentageOfRail = tmpMinX / (maxX - minX);  //max-min=real length, percentage: e.g. 0,3823124 (38%)
        sec = (int)(((double)maxTimeLengthInSec) * percentageOfRail);
        return sec;
    }
    private int countCopiesOfObject(GameObject fig, List<GameObject> tlObjs)
    {
        int c = 0;
        //count object with the same name as fig
        foreach (GameObject gO in tlObjs)
        {
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

        // erase from layer list
        if (figuresLayer1.Contains(obj))
        {
            //Debug.Log("wanted: " + obj.name);
            for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
            {
                //Debug.Log("obj: " + myObjectsPositionListLayer1[i].objName);
                if (myObjectsPositionListLayer1[i].objName == obj.name)
                {
                    myObjectsPositionListLayer1.Remove(myObjectsPositionListLayer1[i]);
                }
            }
            figuresLayer1.Remove(obj);

            //myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(x => x.position.x).ToList();
        }
        else if (figuresLayer2.Contains(obj))
        {
            // Debug.Log("wanted: " + obj.name);
            for (int i = 0; i < myObjectsPositionListLayer2.Count; i++)
            {
                //Debug.Log("obj: " + myObjectsPositionListLayer2[i].objName);
                if (myObjectsPositionListLayer2[i].objName == obj.name)
                {
                    myObjectsPositionListLayer2.Remove(myObjectsPositionListLayer2[i]);
                }
            }
            figuresLayer2.Remove(obj);

            //myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(x => x.position.x).ToList();
        }

        Destroy(obj);
        timelineInstanceObjects.Remove(obj);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
        StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].musicClipElementInstances.Remove(StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].musicClipElementInstances[Int32.Parse(obj.name.Substring(17, 3))]);

        // falls nach dem loeschen nichts mehr ueberlappt, dann alles gross skalieren
        // if (!isSomethingOverlapping())
        // {
        //     sizeLayering = 1;
        //     for (int i = 0; i < timelineInstanceObjects.Count; i++)
        //     {
        //         scaleToLayerSize(timelineInstanceObjects[i], 0);
        //         if (figuresLayer2.Contains(timelineInstanceObjects[i]))
        //         {
        //             figuresLayer2.Remove(timelineInstanceObjects[i]);
        //             figuresLayer1.Add(timelineInstanceObjects[i]);
        //             UpdatePositionVectorInformation(timelineInstanceObjects[i], myObjectsPositionListLayer2, myObjectsPositionListLayer1);
        //         }
        //     }
        //     myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(x => x.position.x).ToList();
        // }
    }
    // private bool checkHittingTimeline(GameObject obj, Vector2 mousePos)
    // {
    //     bool hit = false;

    //     Vector2 tlPos = new Vector2(timelineImage.transform.position.x, timelineImage.transform.position.y);
    //     Vector2 colSize = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, timelineImage.GetComponent<BoxCollider2D>().size.y);
    //     //if mouse hits the timeline while dragging an object
    //     if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
    //     ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
    //     {
    //         hit = true;
    //     }
    //     return hit;
    // }
    public void highlight(GameObject obj, bool highlightOn)
    {
        if (highlightOn)
        {
            obj.transform.GetChild(0).GetComponent<Image>().color = colMusicHighlighted;
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
    public void CreateNew2DInstance(int musObjNr, float moment, float savedLayer)
    {
        //create a copy of this timelineObject and keep the original one
        newCopyOfFigure = Instantiate(figureObjects[musObjNr]);

        //count objects from same kind
        int countName = 0;
        countName = countCopiesOfObject(figureObjects[musObjNr], timelineInstanceObjects);
        figCounterCircle[musObjNr].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();
        newCopyOfFigure.name = figureObjects[musObjNr].name + "_instance" + countName.ToString("000");
        RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();

        // in der Besucherversion auf 60 sek kuerzen, weil gesamtes stueck nur 3 min lang ist
        float tmpLength;
        if (!gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
            tmpLength = UtilitiesTm.FloatRemap(60, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
        else
            tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);

        createRectangle(newCopyOfFigure, gameObject.GetComponent<RectTransform>().rect.height, tmpLength);
        //Debug.Log("tmpLength: " + tmpLength);
        scaleObject(newCopyOfFigure, tmpLength, gameObject.GetComponent<RectTransform>().rect.height, false);
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.x, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.y, -1);

        //parent and position
        float posX = UtilitiesTm.FloatRemap(moment, 0, AnimationTimer.GetMaxTime(), gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2);
        newCopyOfFigure.transform.SetParent(gameObject.transform);
        newCopyOfFigure.transform.localPosition = new Vector3(posX, 0, -1);

        //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
        updateObjectList(timelineInstanceObjects, newCopyOfFigure);
        //openTimelineByClick(true, true);
        newCopyOfFigure.transform.localScale = Vector3.one;

        // size of rectangle becomes size for figure that is clickable
        tmpRectTransform.position = new Vector3(tmpRectTransform.position.x, tmpRectTransform.position.y, -1.0f);
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);

        ObjectsPoint oP = new ObjectsPoint();
        oP.objName = newCopyOfFigure.name;
        oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x);

        // tmpRectTransform.anchoredPosition = new Vector3(posX, GetComponent<RectTransform>().rect.height / 2, -1);
        // oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, rectSize);

        if (savedLayer == 1)
        {
            scaleToLayerSize(newCopyOfFigure, 1);
            figuresLayer1.Add(newCopyOfFigure);
            myObjectsPositionListLayer1.Add(oP);
            sizeLayering = 2;
            myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
        }
        else if (savedLayer == 0)
        {
            scaleToLayerSize(newCopyOfFigure, 0);
            figuresLayer1.Add(newCopyOfFigure);
            myObjectsPositionListLayer1.Add(oP);
            myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
        }
        else
        {
            scaleToLayerSize(newCopyOfFigure, 2);
            figuresLayer2.Add(newCopyOfFigure);
            sizeLayering = 2;
            myObjectsPositionListLayer2.Add(oP);
            myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
        }
    }
    public void ResetScreenSize()
    {
        //minX = 0.087f * Screen.width;// / gameObject.transform.lossyScale.x; //301.0f;  //timeline-minX
        //railWidth = 0.87f * Screen.width;           //railwidth=1670.4px; / gameObject.transform.lossyScale.x;
        //maxX = minX + railWidth;  //timeline-maxX
        if (isTimelineOpen)
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidthAbsolute, 80);
        }
        else
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidthAbsolute, 20);
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
    private IEnumerator FadeOut(AudioSource audioSource, float FadeTime, GameObject obj, float time)
    {
        fading = true;
        while (audioSource.volume > 0.1f)
        {
            audioSource.volume -= 1 * Time.deltaTime / FadeTime;
            yield return null;
        }
        currentClip = ((int)Char.GetNumericValue(obj.name[07]) - 1); // object index
        audioSource.clip = clip[currentClip];

        if (time != -1)
            StartCoroutine(FadeIn(audioSource, 1, time));
        else
        {
            audioSource.Stop();
            playingMusic = false;
            audioSource.volume = 1;
        }
    }
    private IEnumerator FadeIn(AudioSource audioSource, float FadeTime, float time)
    {
        audioSource.time = time;
        float startVolume = 0.2f;

        audioSource.Play();

        while (audioSource.volume < 0.9f)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.volume = 1f;
    }
    private int GetCurrentMusicCount(out int m, out int n, out int o)
    {
        m = n = o = 0;
        int musicCount = 0;
        for (int i = 0; i < timelineInstanceObjects.Count; i++)
        {
            double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[i], timelineInstanceObjects[i].GetComponent<MusicLength>().musicLength, maxTimeInSec);
            double endSec;
            if (!gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
                endSec = startSec + 60;
            else
                endSec = startSec + timelineInstanceObjects[i].GetComponent<MusicLength>().musicLength;
            float tmpTime = AnimationTimer.GetTime();

            // wenn timer im bereich musikstuecks und musik ist nicht an
            if (AnimationTimer.GetTime() >= startSec && AnimationTimer.GetTime() <= endSec)
            {
                musicCount++;
                if (musicCount == 1) m = i;
                else if (musicCount == 2) n = i;
                else if (musicCount == 3) o = i;
            }
        }
        return musicCount;
    }
    private void PlayLatestPiece(bool once)
    {
        if (GetCurrentMusicCount(out int m, out int n, out int o) == 1)
        {
            double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[m], timelineInstanceObjects[m].GetComponent<MusicLength>().musicLength, maxTimeInSec);
            //Debug.Log("start: " + startSec);
            //double endSec = calculateMusicEndTimeInSec(timelineInstanceObjects[m], timelineInstanceObjects[m].GetComponent<MusicLength>().musicLength, maxTimeInSec);
            tmpTime = AnimationTimer.GetTime();

            anyInstanceIsPlaying = true;

            if (playingMusic == false || once == true)
            {
                playingMusic = true;
                audioSource.time = tmpTime - (float)startSec;
                currentClip = ((int)Char.GetNumericValue(timelineInstanceObjects[m].name[07]) - 1); // object index
                audioSource.clip = clip[currentClip];
                audioSource.Play();
            }
            if (SceneManaging.updateMusic)  // scrubbing
            {
                playingMusic = false;
            }
            firstTimeSecond = false;
        }
        else if (GetCurrentMusicCount(out m, out n, out o) == 2)
        {
            double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[n], timelineInstanceObjects[n].GetComponent<MusicLength>().musicLength, maxTimeInSec);
            float tmpTime = AnimationTimer.GetTime();
            if (!firstTimeSecond) // es switcht von 1 auf 2 musikstuecke gleichzeitig
            {
                fading = false;
                firstTimeSecond = true;
            }
            if (!fading) StartCoroutine(FadeOut(audioSource, 1, timelineInstanceObjects[n], (tmpTime - (float)startSec)));

            anyInstanceIsPlaying = true;

            if (playingMusic == false || once == true)
            {
                playingMusic = true;
                audioSource.time = tmpTime - (float)startSec;
                currentClip = ((int)Char.GetNumericValue(timelineInstanceObjects[n].name[07]) - 1); // object index
                audioSource.clip = clip[currentClip];
                audioSource.Play();
            }
            if (SceneManaging.updateMusic)
            {
                playingMusic = false;
            }
        }

        /*else if (GetCurrentMusicCount(out m, out n, out o) == 3)
        {
            Debug.Log("3");
            if (!firstTimeThird)
            {
                fading = false;
                firstTimeThird = true;
            }
            double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[o], timelineInstanceObjects[o].GetComponent<MusicLength>().musicLength, maxTimeInSec);
            float tmpTime = AnimationTimer.GetTime();

            if (!fading) StartCoroutine(FadeOut(audioSource, 1, timelineInstanceObjects[o], (tmpTime - (float)startSec)));
            anyInstanceIsPlaying = true;

            if (playingMusic == false || once == true)
            {
                playingMusic = true;
                audioSource.time = tmpTime - (float)startSec;
                currentClip = ((int)Char.GetNumericValue(timelineInstanceObjects[o].name[07]) - 1); // object index
                audioSource.clip = clip[currentClip];
                audioSource.Play();
            }
            if (SceneManaging.updateMusic)
            {
                playingMusic = false;
            }
        }*/
        else
        {
            firstTimeSecond = false;
            playingMusic = false;
            //Debug.Log("m: " + m);
            if (onlyPiecefinished)
            {
                try
                {
                    StartCoroutine(FadeOut(audioSource, 1, timelineInstanceObjects[m], -1));
                    onlyPiecefinished = false;
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
    }
    private void scaleDeleteButton(GameObject btn, float posY, float size, float colSize)
    {
        btn.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(30, posY, -1);
        btn.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
    }
    private void scaleToLayerSize(GameObject obj, int layer)
    {
        float rectSize = obj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

        switch (layer)
        {
            case 0:                 // only one layer
                scaleDeleteButton(obj, 20, 40, 45);
                scaleObject(obj, rectSize, GetComponent<RectTransform>().rect.height, false);      //scale the figure-picture in timeline to x: 100 and y: 80px
                obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 0.5f, -1);
                obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
                // obj.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                obj.GetComponent<BoxCollider2D>().offset = new Vector2(obj.GetComponent<BoxCollider2D>().size.x / 2, 0);
                break;
            case 1:                 // 2 layers, but object in layer 1
                scaleDeleteButton(obj, 0, 35, 40);
                scaleObject(obj, rectSize, GetComponent<RectTransform>().rect.height / 2, false);
                obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 0, -1);
                obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height / 2);
                // obj.GetComponent<BoxCollider2D>().offset = new Vector2(0, obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                obj.GetComponent<BoxCollider2D>().offset = new Vector2(obj.GetComponent<BoxCollider2D>().size.x / 2, obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                break;
            case 2:                 // 2 layers and object in layer 2
                scaleDeleteButton(obj, 0, 35, 40);
                scaleObject(obj, rectSize, GetComponent<RectTransform>().rect.height / 2, false);
                obj.transform.GetComponent<RectTransform>().pivot = new Vector3(obj.transform.GetComponent<RectTransform>().pivot.x, 1, -1);
                obj.GetComponent<BoxCollider2D>().size = new Vector2(obj.transform.GetChild(0).GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height / 2);
                //obj.GetComponent<BoxCollider2D>().offset = new Vector2(0, -obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                obj.GetComponent<BoxCollider2D>().offset = new Vector2(obj.GetComponent<BoxCollider2D>().size.x / 2, -obj.transform.GetChild(0).GetComponent<RectTransform>().rect.height / 2);
                break;
        }
    }
    private bool isSomethingOverlapping()
    {
        bool val = false;
        for (int i = 0; i < timelineInstanceObjects.Count; i++)
        {
            for (int j = 0; j < timelineInstanceObjects.Count; j++)
            {
                RectTransform tmpRectTransform = timelineInstanceObjects[j].GetComponent<RectTransform>();
                if (timelineInstanceObjects[j] != timelineInstanceObjects[i] && (tmpRectTransform.anchoredPosition.x >= timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x
                && (tmpRectTransform.anchoredPosition.x <= (timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[i].GetComponent<BoxCollider2D>().size.x))
                || ((tmpRectTransform.anchoredPosition.x + timelineInstanceObjects[j].GetComponent<BoxCollider2D>().size.x) >= timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x)
                && tmpRectTransform.anchoredPosition.x <= timelineInstanceObjects[i].GetComponent<RectTransform>().anchoredPosition.x))
                {
                    val = true;
                }
            }
        }
        return val;
    }
    private void LookForFreeSpot(GameObject obj, bool create)
    {
        int layer1Full = -1;
        int layer2Full = -1;
        //Debug.Log("count 1: " + myObjectsPositionListLayer1.Count + ", count 2: " + myObjectsPositionListLayer2.Count);
        for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
            // wenn linker rand des aktuellen obj weiter links ist als rechter rand des collisionsobjekts
            if (obj.GetComponent<RectTransform>().anchoredPosition.x + obj.GetComponent<RectTransform>().sizeDelta.x > myObjectsPositionListLayer1[i].position.x
            && myObjectsPositionListLayer1[i].position.x + myObjectsPositionListLayer1[i].position.y > obj.GetComponent<RectTransform>().anchoredPosition.x
            && myObjectsPositionListLayer1[i].objName != obj.name)
            {
                //Debug.Log("layer1full: " + i);
                layer1Full = i;
            }
        for (int j = 0; j < myObjectsPositionListLayer2.Count; j++)
            if (obj.GetComponent<RectTransform>().anchoredPosition.x + obj.GetComponent<RectTransform>().sizeDelta.x > myObjectsPositionListLayer2[j].position.x
            && myObjectsPositionListLayer2[j].position.x + myObjectsPositionListLayer2[j].position.y > obj.GetComponent<RectTransform>().anchoredPosition.x
            && myObjectsPositionListLayer2[j].objName != obj.name)
            {
                //Debug.Log("layer2Full: " + j);
                layer2Full = j;
            }

        // wenn alles voll ist
        if (layer2Full != -1 && layer1Full != -1)
        {
            // Debug.Log("1: " + myObjectsPositionListLayer1[layer1Full].position + ", 2: " + myObjectsPositionListLayer2[layer2Full].position);
            // Debug.Log("length: " + railIndex + ", layer1: " + layer1Full + ", layer2: " + layer2Full);
            // obj auf layer 1 ist weiter links
            if (myObjectsPositionListLayer2[layer2Full].position.x > myObjectsPositionListLayer1[layer1Full].position.x)
            {
                //Debug.Log("obj auf layer 1 ist weiter links: mouse: " + Input.mousePosition.x + ", obj: " + figuresLayer1[layer1Full].transform.position.x);
                // obj ist links von collision
                if (Input.mousePosition.x < figuresLayer1[layer1Full].transform.position.x)
                {
                    //Debug.Log("left");
                    FindNextFreeSpot(obj, myObjectsPositionListLayer1[layer1Full].position.x, myObjectsPositionListLayer2[layer2Full].position.x, create, "left");
                }
                // obj ist rechts von collision
                else
                {
                    //Debug.Log("right");
                    FindNextFreeSpot(obj, myObjectsPositionListLayer1[layer1Full].position.x, myObjectsPositionListLayer2[layer2Full].position.x, create, "right");
                }
            }
            // obj auf layer 1 ist weiter rechts
            else
            {
                //Debug.Log("springst du hier hin? ");
                // obj ist links von collision
                if (Input.mousePosition.x < myObjectsPositionListLayer2[layer2Full].position.x)
                {
                    //Debug.Log("else left");
                    FindNextFreeSpot(obj, myObjectsPositionListLayer1[layer1Full].position.x, myObjectsPositionListLayer2[layer2Full].position.x, create, "left");
                }
                // obj ist rechts von collision
                else
                {
                    //Debug.Log("else right");
                    FindNextFreeSpot(obj, myObjectsPositionListLayer1[layer1Full].position.x, myObjectsPositionListLayer2[layer2Full].position.x, create, "right");
                }
            }
        }
        // wenn nur layer 2 frei ist
        else if (layer1Full != -1)
        {
            //Debug.Log("nur layer 2 frei!");
            if (figuresLayer1.Contains(obj))
            {
                scaleToLayerSize(obj, 2);
                figuresLayer1.Remove(obj);
                figuresLayer2.Add(obj);

                UpdatePositionVectorInformation(obj, myObjectsPositionListLayer1, myObjectsPositionListLayer2);


                // for (int i = 0; i < myObjectsPositionListLayer2.Count; i++)
                // {
                //     Debug.Log("2 vector " + i + ": " + myObjectsPositionListLayer2[i].position.x);
                // }
            }
            myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
        }
        // wenn nur layer 1 frei ist
        else if (layer2Full != -1)
        {
            //Debug.Log("nur layer 1 frei!");
            if (create)
            {
                scaleToLayerSize(obj, 1);
                figuresLayer1.Add(obj);
            }
            else
            {
                scaleToLayerSize(obj, 1);
                if (figuresLayer2.Contains(obj))
                {
                    figuresLayer2.Remove(obj);
                    figuresLayer1.Add(obj);

                    UpdatePositionVectorInformation(obj, myObjectsPositionListLayer2, myObjectsPositionListLayer1);
                    Debug.Log("count layer 1: " + myObjectsPositionListLayer1.Count);

                    // for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("1 vector " + i + ": " + myObjectsPositionListLayer1[i].position.x);
                    // }
                }
                myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
            }
        }
        else    // alles frei
        {
            if (create)
            {
                scaleToLayerSize(obj, 1);
                figuresLayer1.Add(obj);
            }
            else
            {
                // Debug.Log("alles frei");
                scaleToLayerSize(obj, 1);
                if (figuresLayer2.Contains(obj))
                {
                    figuresLayer2.Remove(obj);
                    figuresLayer1.Add(obj);

                    UpdatePositionVectorInformation(obj, myObjectsPositionListLayer2, myObjectsPositionListLayer1);

                    // for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("1 vector " + i + ": " + myObjectsPositionListLayer1[i].position.x);
                    // }
                }
                myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
            }
        }
    }
    private void FindNextFreeSpot(GameObject obj, float posXIn1, float posXIn2, bool create, string direction)
    {
        float foundIndexLayer1;
        float foundIndexLayer2;
        RectTransform tmpRectTransform = obj.GetComponent<RectTransform>();

        // mouse ist links von collision
        if (direction == "left")
        {
            foundIndexLayer1 = posXIn1;//- obj.GetComponent<RectTransform>().sizeDelta.x;
            foundIndexLayer2 = posXIn2;//- obj.GetComponent<RectTransform>().sizeDelta.x;
                                       //Debug.Log("ind 1: " + foundIndexLayer1 + ", ind 2: " + foundIndexLayer2);

            //erstmal layer 1
            for (int i = myObjectsPositionListLayer1.Count - 1; i >= 0; i--)
            {
                //Debug.Log("name: " + myObjectsPositionListLayer1[i].objName + ", pos: " + (myObjectsPositionListLayer1[i].position.x + myObjectsPositionListLayer1[i].position.y) + ", foundindex: " + foundIndexLayer1);// - obj.GetComponent<RectTransform>().sizeDelta.x));
                // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt -> dann wird die Stelle davor als gefunden genommen
                if (myObjectsPositionListLayer1[i].position.x + myObjectsPositionListLayer1[i].position.y >= foundIndexLayer1 - tmpRectTransform.sizeDelta.x
                && myObjectsPositionListLayer1[i].position.x <= foundIndexLayer1
                && myObjectsPositionListLayer1[i].objName != obj.name)
                {
                    foundIndexLayer1 = myObjectsPositionListLayer1[i].position.x;//- obj.GetComponent<RectTransform>().sizeDelta.x;
                                                                                 //Debug.Log("name 1: " + myObjectsPositionListLayer1[i].objName);
                }
            }
            // dann layer 2
            for (int j = myObjectsPositionListLayer2.Count - 1; j >= 0; j--)
            {
                //Debug.Log("name: " + myObjectsPositionListLayer2[j].objName + ", pos: " + (myObjectsPositionListLayer2[j].position.x + myObjectsPositionListLayer2[j].position.y) + ", foundindex: " + foundIndexLayer2);// - obj.GetComponent<RectTransform>().sizeDelta.x));
                // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt -> dann wird die Stelle davor als gefunden genommen
                if (myObjectsPositionListLayer2[j].position.x + myObjectsPositionListLayer2[j].position.y >= foundIndexLayer2 - tmpRectTransform.sizeDelta.x
                && myObjectsPositionListLayer2[j].position.x <= foundIndexLayer2
                && myObjectsPositionListLayer2[j].objName != obj.name)
                {
                    foundIndexLayer2 = myObjectsPositionListLayer2[j].position.x;//- obj.GetComponent<RectTransform>().sizeDelta.x;
                                                                                 //Debug.Log("name 2: " + myObjectsPositionListLayer2[j].objName);
                }
            }
            // liegt platz auf layer1 naeher dran
            if (foundIndexLayer1 > foundIndexLayer2)
            {
                // wenn der rand im weg ist
                if (foundIndexLayer1 - tmpRectTransform.sizeDelta.x - tmpRectTransform.sizeDelta.x / 2 < 0)
                {
                    if (create)
                    {
                        Debug.Log("deleeeete: " + currentClickedObjectIndex);
                        //removeObjectFromTimeline2D(obj, true);
                        _toBeRemoved = true;
                    }
                }

                tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer1 - tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
                if (!figuresLayer1.Contains(obj))
                {
                    scaleToLayerSize(obj, 1);
                    figuresLayer1.Add(obj);
                    figuresLayer2.Remove(obj);
                    UpdatePositionVectorInformation(obj, myObjectsPositionListLayer2, myObjectsPositionListLayer1);
                }

                myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();

                // for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                // {
                //     //Debug.Log("1 vector " + i + ": " + myObjectsPositionListLayer1[i].position.x);
                // }

            }
            // liegt platz auf layer2 naeher dran
            else
            {
                // abfrage, wenn der naechste freie platz ausserhalb der schiene waere
                if (foundIndexLayer2 - tmpRectTransform.sizeDelta.x - tmpRectTransform.sizeDelta.x / 2 < 0)
                {
                    Debug.Log("deleeeete: " + currentClickedObjectIndex);
                    //removeObjectFromTimeline2D(obj, true);
                    _toBeRemoved = true;
                }
                tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer2 - tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
                if (!figuresLayer2.Contains(obj))
                {
                    scaleToLayerSize(obj, 2);
                    figuresLayer2.Add(obj);
                    figuresLayer1.Remove(obj);
                    UpdatePositionVectorInformation(obj, myObjectsPositionListLayer1, myObjectsPositionListLayer2);
                }
                myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();

                // for (int k = 0; k < myObjectsPositionListLayer2.Count; k++)
                // {
                //     Debug.Log("2 vector " + k + ": " + myObjectsPositionListLayer2[k].position);
                // }

            }
        }
        // mouse ist rechts von collision
        else
        {
            foundIndexLayer1 = posXIn1;
            foundIndexLayer2 = posXIn2;

            //erstmal layer 1
            for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
            {
                // wenn der Beginn des naechsten objekts so liegt, dass aktuelles objekt NICHT dazwischen passt
                if (myObjectsPositionListLayer1[i].position.x <= foundIndexLayer1 + tmpRectTransform.sizeDelta.x + tmpRectTransform.sizeDelta.x
                && myObjectsPositionListLayer1[i].position.x >= foundIndexLayer1
                && myObjectsPositionListLayer1[i].objName != obj.name)
                {
                    foundIndexLayer1 = myObjectsPositionListLayer1[i].position.x;
                }
            }
            // dann layer 2
            for (int j = 0; j < myObjectsPositionListLayer2.Count; j++)
            {
                // wenn der Beginn des naechsten objekts so liegt, DASS aktuelles objekt dazwischen passt
                if (myObjectsPositionListLayer2[j].position.x <= foundIndexLayer2 + tmpRectTransform.sizeDelta.x + tmpRectTransform.sizeDelta.x
                && myObjectsPositionListLayer2[j].position.x >= foundIndexLayer2
                && myObjectsPositionListLayer2[j].objName != obj.name)
                {
                    foundIndexLayer2 = myObjectsPositionListLayer2[j].position.x;
                }
            }

            // liegt platz auf layer1 naeher dran
            if (foundIndexLayer1 > foundIndexLayer2)
            {
                tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer2 + tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
                if (!figuresLayer2.Contains(obj))
                {
                    scaleToLayerSize(obj, 2);
                    figuresLayer2.Add(obj);
                    figuresLayer1.Remove(obj);
                    UpdatePositionVectorInformation(obj, myObjectsPositionListLayer1, myObjectsPositionListLayer2);
                    // for (int i = 0; i < myObjectsPositionListLayer2.Count; i++)
                    // {
                    //     Debug.Log("layer 1 naeher vector! " + i + ": " + myObjectsPositionListLayer2[i].position.x);
                    // }
                }
                myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();

            }
            else
            {
                tmpRectTransform.anchoredPosition = new Vector2(foundIndexLayer1 + tmpRectTransform.sizeDelta.x, tmpRectTransform.anchoredPosition.y);
                if (!figuresLayer1.Contains(obj))
                {
                    scaleToLayerSize(obj, 1);
                    figuresLayer1.Add(obj);
                    figuresLayer2.Remove(obj);
                    UpdatePositionVectorInformation(obj, myObjectsPositionListLayer2, myObjectsPositionListLayer1);
                    // for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                    // {
                    //     Debug.Log("layer 2 naeher! vector " + i + ": " + myObjectsPositionListLayer1[i].position.x);
                    // }
                }
                myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
            }
        }
    }
    private bool isCurrentFigureOverlapping(GameObject obj)
    {
        bool val = false;
        RectTransform tmpRectTransform = obj.GetComponent<RectTransform>();
        for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
        {
            //Debug.Log("pos1: " + myObjectsPositionListLayer1[i].position.x + ", pos obj: " + tmpRectTransform.anchoredPosition.x);

            if (((myObjectsPositionListLayer1[i].position.x < tmpRectTransform.anchoredPosition.x
            && myObjectsPositionListLayer1[i].position.x + myObjectsPositionListLayer1[i].position.y > tmpRectTransform.anchoredPosition.x)
            || (myObjectsPositionListLayer1[i].position.x > tmpRectTransform.anchoredPosition.x
            && myObjectsPositionListLayer1[i].position.x < tmpRectTransform.anchoredPosition.x + tmpRectTransform.sizeDelta.x))
            && myObjectsPositionListLayer1[i].objName != obj.name)
            {
                //Debug.Log("name: " + obj.name + ", anderer: " + myObjectsPositionListLayer1[i].objName);
                val = true;
            }
        }
        return val;
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
            // if you click in slider window
            if (_backgroundBoxCollider == Physics2D.OverlapPoint(getMousePos))
            {
                PlayLatestPiece(true);
            }
            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                openTimelineByClick(isTimelineOpen, false);
            }

            //if you click on an object in shelf
            else if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                if (!SceneManaging.sceneChanged)
                    SceneManaging.sceneChanged = true;

                diff = new Vector2(getMousePos.x - figureObjects[currentClickedObjectIndex].transform.position.x, getMousePos.y - figureObjects[currentClickedObjectIndex].transform.position.y);

                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    draggingObject = true;
                }
            }
            //or check if you click an object in timeline
            else if (currentClickedInstanceObjectIndex != -1 && editTimelineObject)
            {
                if (!SceneManaging.sceneChanged)
                    SceneManaging.sceneChanged = true;

                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    // click on delete-Button
                    Vector2 pos = timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).position;
                    RectTransform rect = timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();

                    if (timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetChild(1).GetChild(0).gameObject.activeSelf
                        && getMousePos.x >= pos.x - rect.sizeDelta.x / 2.5f && getMousePos.x <= pos.x + rect.sizeDelta.x / 2.5f
                        && getMousePos.y >= pos.y - rect.sizeDelta.y / 2.5f && getMousePos.y <= pos.y + rect.sizeDelta.y / 2.5f)
                    {
                        _toBeRemoved = true;
                    }
                    diff = new Vector2(getMousePos.x - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, getMousePos.y - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.y);

                    //set up some flags
                    draggingObject = true;
                    draggingOnTimeline = true;

                    //highlighting objects and showing delete button when clicked
                    for (int i = 0; i < timelineInstanceObjects.Count; i++)
                    {
                        highlight(timelineInstanceObjects[i], false);
                    }
                    highlight(timelineInstanceObjects[currentClickedInstanceObjectIndex], true);
                }

            }

            //if you hit/clicked nothing with mouse
            else if (Physics2D.OverlapPoint(getMousePos) == false)
            {
                for (int i = 0; i < timelineInstanceObjects.Count; i++)
                {
                    highlight(timelineInstanceObjects[i], false);
                }
            }

        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline && isTimelineOpen)
        {
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                GameObject currentObj = timelineInstanceObjects[currentClickedInstanceObjectIndex];
                isInstance = true;
                //if you click an object in timeline (for dragging) //move object
                updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos - diff);
                //snapping/lock y-axis
                setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, this.transform.position.y);
                float currentPos = currentObj.GetComponent<RectTransform>().anchoredPosition.x;
                float currentSize = currentObj.GetComponent<RectTransform>().sizeDelta.x;

                #region Stapeln von Ebenen
                // if there is only one layer on Rail
                if (sizeLayering == 1)
                {
                    if (isCurrentFigureOverlapping(currentObj))
                    {
                        scaleToLayerSize(currentObj, 2);

                        figuresLayer1.Remove(currentObj);
                        figuresLayer2.Add(currentObj);

                        UpdatePositionVectorInformation(currentObj, myObjectsPositionListLayer1, myObjectsPositionListLayer2);
                        myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                        // for (int i = 0; i < myObjectsPositionListLayer2.Count; i++)
                        // {
                        //     Debug.Log("vector " + i + ": " + myObjectsPositionListLayer2[i].position.x);
                        // }

                        // scale all other timelineobjects of layer 1
                        for (int j = 0; j < figuresLayer1.Count; j++)
                        {
                            if (figuresLayer1[j] != currentObj) scaleToLayerSize(figuresLayer1[j], 1);
                        }

                        sizeLayering = 2;
                    }
                }

                // if there are two layers on Rail
                else if (sizeLayering == 2)
                {
                    // wenn nichts mehr überlappt
                    if (!isSomethingOverlapping())
                    {
                        sizeLayering = 1;
                        // scale all other timelineobjects of layer 0
                        for (int j = 0; j < timelineInstanceObjects.Count; j++)
                        {
                            scaleToLayerSize(timelineInstanceObjects[j], 0);
                            if (figuresLayer2.Contains(timelineInstanceObjects[j]))
                            {
                                figuresLayer2.Remove(timelineInstanceObjects[j]);
                                figuresLayer1.Add(timelineInstanceObjects[j]);
                            }
                        }
                        for (int i = 0; i < myObjectsPositionListLayer2.Count; i++)
                        {
                            myObjectsPositionListLayer1.Add(myObjectsPositionListLayer2[i]);
                            myObjectsPositionListLayer2.Remove(myObjectsPositionListLayer2[i]);
                            // Debug.Log("current obj: " + currentObj + ", pos: " + currentObj.GetComponent<RectTransform>().anchoredPosition.x);
                        }
                        myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();

                        // for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                        // {
                        //     Debug.Log("vector " + i + ": " + myObjectsPositionListLayer1[i].position.x);
                        // }
                    }

                    else
                    {
                        LookForFreeSpot(currentObj, false);
                    }
                }
                #endregion


                //-------------------------------------------------limit front of rail------------------------------------------//
                if (currentPos <= 0)
                {
                    currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, currentObj.GetComponent<RectTransform>().anchoredPosition.y, -1);    // tendenziell muesste das eher in buttonUp
                }

                //-------------------------------------------------limit back of rail------------------------------------------//
                else if ((currentPos + currentSize) > GetComponent<RectTransform>().rect.width)
                {
                    //currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - currentObj.transform.GetComponent<BoxCollider2D>().size.x), currentObj.GetComponent<RectTransform>().anchoredPosition.y, -1);
                    currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().rect.width - currentSize, currentObj.GetComponent<RectTransform>().anchoredPosition.y);
                }

                if (Physics2D.OverlapPoint(getMousePos) == false)       // mouse outside
                {
                    //SceneManaging.objectsTimeline = ((int)Char.GetNumericValue(timelineImage.name[17]) - 1);  // save old timeline to remove instance of this timeline
                    draggingObject = true;
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
            updateObjectPosition(figureObjects[currentClickedObjectIndex], getMousePos - diff);

            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            bool hitTimeline = false;
            hitTimeline = checkHittingTimeline(figureObjects[currentClickedObjectIndex], getMousePos);

            if (hitTimeline)
            {
                // change parent back and snap
                setParent(figureObjects[currentClickedObjectIndex], gameObject);
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], figureObjects[currentClickedObjectIndex].transform.position.x, this.transform.position.y);

                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(100, timelineImage.GetComponent<RectTransform>().sizeDelta.y);

                //set placed figures to timelineobjects-list
                //updateObjectList(timelineObjects, figureObjects[currentClickedObjectIndex]);

                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }
            else
            {
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(objectShelfSize.x, objectShelfSize.y);
                // scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize.x, objectShelfSize.y, false);
                // scale up currentFigure
                releaseOnTimeline = false;
            }
        }
        //-------release mousebutton
        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            draggingObject = false;
            editTimelineObject = false;

            if (_toBeRemoved)
            {
                removeObjectFromTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex]);
            }

            else if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)  //if release obj-pos and release mouse-button-pos is the same
                {
                    //create a copy of this timelineObject and keep the original one
                    newCopyOfFigure = Instantiate(figureObjects[currentClickedObjectIndex]);
                    //count objects from same kind
                    int countName = 0;
                    countName = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    newCopyOfFigure.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");
                    RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();

                    // in der Besucherversion auf 60 sek kuerzen, weil gesamtes stueck nur 3 min lang ist
                    float tmpLength;
                    if (!gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
                        tmpLength = UtilitiesTm.FloatRemap(60, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
                    else
                        tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);

                    createRectangle(newCopyOfFigure, gameObject.GetComponent<RectTransform>().rect.height, tmpLength);

                    figCounterCircle[currentClickedObjectIndex].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();

                    //parent and position
                    newCopyOfFigure.transform.SetParent(gameObject.transform);
                    newCopyOfFigure.transform.localScale = Vector3.one;

                    //------------------------------------------limit front of rail---------------------------------------------//
                    if (figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().position.x < (minX + 0.03f * Screen.width))  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
                    {
                        tmpRectTransform.position = new Vector3((float)minX + 0.03f * Screen.width, figureObjects[currentClickedObjectIndex].transform.position.y, -1);    // tendenziell muesste das eher in buttonUp
                    }
                    //------------------------------------------limit back of rail---------------------------------------------//
                    else if ((tmpRectTransform.anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) > (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width))
                    {
                        tmpRectTransform.anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x), tmpRectTransform.anchoredPosition.y, -1);
                    }
                    else
                    {
                        newCopyOfFigure.transform.position = new Vector3(figureObjects[currentClickedObjectIndex].transform.position.x, figureObjects[currentClickedObjectIndex].transform.position.y, -1);
                        //Debug.Log("anch pos: "+tmpRectTransform.anchoredPosition.x+", pos: "+newCopyOfFigure.transform.position.x);
                    }

                    // pos to mouse pos
                    tmpRectTransform.transform.position = new Vector2(getMousePos.x, tmpRectTransform.transform.position.y);

                    // size of rectangle becomes size for figure that is clickable (bring in front)
                    tmpRectTransform.localPosition = new Vector3(tmpRectTransform.localPosition.x, tmpRectTransform.localPosition.y, -1.0f);
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);

                    newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y);


                    //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
                    updateObjectList(timelineInstanceObjects, newCopyOfFigure);

                    //set original image back to shelf, position 2 to make it visible
                    figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                    figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                    //figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(false);

                    #region calculating Layer Overlap

                    ObjectsPoint oP = new ObjectsPoint();
                    oP.objName = newCopyOfFigure.name;
                    oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpLength);

                    scaleToLayerSize(newCopyOfFigure, 0);

                    if (sizeLayering == 1)
                    {
                        if (isSomethingOverlapping())
                        {
                            sizeLayering = 2;
                            //Debug.Log("something overlapping");
                            figuresLayer2.Add(newCopyOfFigure);
                            scaleToLayerSize(newCopyOfFigure, 2);
                            myObjectsPositionListLayer2.Add(oP);
                            myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();

                            // others to 1
                            for (int i = 0; i < figuresLayer1.Count; i++)
                            {
                                scaleToLayerSize(figuresLayer1[i], 1);
                            }
                            //Debug.Log("op: "+oP.position);
                        }
                        else
                        {
                            figuresLayer1.Add(newCopyOfFigure);
                            myObjectsPositionListLayer1.Add(oP);
                            //sortVectorValues(myObjectsPositionListLayer1, myObjectsPositionListLayer1.Count - 1);
                            myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                            // for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                            // {
                            //     Debug.Log("vector " + i + ": " + myObjectsPositionListLayer1[i].position);
                            // }
                        }
                    }
                    else
                    {
                        //Debug.Log("figure: " + newCopyOfFigure);
                        LookForFreeSpot(newCopyOfFigure, true);
                        oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x);
                        if (figuresLayer1.Contains(newCopyOfFigure))
                        {
                            myObjectsPositionListLayer1.Add(oP);
                            myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                            // for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                            // {
                            //     Debug.Log("vector " + i + ": " + myObjectsPositionListLayer1[i].position);
                            // }
                            scaleToLayerSize(newCopyOfFigure, 1);
                        }
                        else
                        {
                            myObjectsPositionListLayer2.Add(oP);
                            myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                            // for (int i = 0; i < myObjectsPositionListLayer2.Count; i++)
                            // {
                            //     Debug.Log("2 vector " + i + ": " + myObjectsPositionListLayer2[i].position);
                            // }
                            scaleToLayerSize(newCopyOfFigure, 2);
                        }
                    }
                    #endregion

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Save to SceneData:
                    MusicClipElementInstance musicClipElementInstance = new MusicClipElementInstance();
                    musicClipElementInstance.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    musicClipElementInstance.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");
                    StaticSceneData.StaticData.musicClipElements[currentClickedObjectIndex].musicClipElementInstances.Add(musicClipElementInstance);

                    for(int i = 0;i<timelineInstanceObjects.Count;i++)
                    {
                        highlight(timelineInstanceObjects[i],false);
                    }
                    highlight(timelineInstanceObjects[timelineInstanceObjects.Count - 1], true);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////

                    // set index back to -1 because nothing is being clicked anymore
                    currentClickedObjectIndex = -1;
                }
                else
                {
                    // change position of vector in each case
                    for (int i = 0; i < myObjectsPositionListLayer1.Count; i++)
                    {
                        if (myObjectsPositionListLayer1[i].objName == timelineInstanceObjects[currentClickedInstanceObjectIndex].name)
                        {
                            myObjectsPositionListLayer1[i].position = new Vector2(timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x, timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().sizeDelta.x);
                        }

                    }
                    for (int i = 0; i < myObjectsPositionListLayer2.Count; i++)
                    {
                        if (myObjectsPositionListLayer2[i].objName == timelineInstanceObjects[currentClickedInstanceObjectIndex].name)
                        {
                            myObjectsPositionListLayer2[i].position = new Vector2(timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x, timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().sizeDelta.x);
                        }
                    }

                    myObjectsPositionListLayer1 = myObjectsPositionListLayer1.OrderBy(w => w.position.x).ToList();
                    myObjectsPositionListLayer2 = myObjectsPositionListLayer2.OrderBy(w => w.position.x).ToList();
                }

            }
            // if dropped somewhere else than timeline: bring figure back to shelf
            else if (releaseOnTimeline == false && currentClickedObjectIndex != -1 && isInstance == false)
            {
                setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                // bring object back to position two, so that its visible and change position back to default
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -73.0f);
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

                StaticSceneData.StaticData.musicClipElements[musObjIndex].musicClipElementInstances[musInsIndex].moment = moment;
                if (figuresLayer1.Contains(timelineInstanceObjects[i]))
                {
                    if (sizeLayering == 1)
                    {
                        StaticSceneData.StaticData.musicClipElements[musObjIndex].musicClipElementInstances[musInsIndex].layer = 0;
                    }
                    else
                    {
                        StaticSceneData.StaticData.musicClipElements[musObjIndex].musicClipElementInstances[musInsIndex].layer = 1;
                    }
                }
                else
                {
                    StaticSceneData.StaticData.musicClipElements[musObjIndex].musicClipElementInstances[musInsIndex].layer = 2;
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            releaseOnTimeline = false;
            releaseObjMousePos.x = 0.0f;
            releaseObjMousePos.y = 0.0f;
            draggingOnTimeline = false;
            isInstance = false;
            onlyPiecefinished = true;
            _toBeRemoved = false;
        }

        // turning music on and off in playmode
        if (SceneManaging.playing)
        {
            PlayLatestPiece(false);
        }

        ///////////////////////////playing Sample/////////////////////////
        else if (playingSample)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.time = 50f;
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
            firstTimeSecond = false;
        }

        if (!anyInstanceIsPlaying && playingSample == false)
        {
            audioSource.Stop();
            playingMusic = false;
            firstTimeSecond = false;
        }
    }
}