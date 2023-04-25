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
    // [HideInInspector] public List<ObjectsPoint> myObjectsPositionListLayer1 = new List<ObjectsPoint>();
    // [HideInInspector] public List<ObjectsPoint> myObjectsPositionListLayer2 = new List<ObjectsPoint>();
    [HideInInspector] public List<MusicPiece> myObjects = new List<MusicPiece>();
    [HideInInspector] public bool isTimelineOpen;
    public GameObject objectLibrary, parentMenue; // mainMenue
    // [HideInInspector] public List<GameObject> timelineInstanceObjects, figuresLayer1, figuresLayer2;
    [HideInInspector] public int sizeLayering = 1;
    public AudioClip[] clip;         // ought to be 6 audioclips
    #endregion
    #region private variables
    [SerializeField] Image timelineImage;
    [SerializeField] private GameObject gameController, UICanvas;
    [SerializeField] RailManager contentRailsMenue;
    [SerializeField] private BoxCollider2D _backgroundBoxCollider;
    [SerializeField] private GameObject prefabRect;
    [SerializeField] private TextMeshProUGUI spaceWarning;
    Vector2 sizeDeltaAsFactor;
    AudioSource audioSource;
    Vector2[] objectShelfPosition;
    Vector2 objectShelfSize;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, playingMusic, isInstance, playingSample, fading;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    Color colMusic, colMusicHighlighted;
    bool anyInstanceIsPlaying = false, firstTimeSecond = false;
    bool onlyPiecefinished = true, _toBeRemoved;
    Vector2 releaseObjMousePos, diff;
    double minX, maxX;
    private float railWidthAbsolute = 1670.4f;
    int maxTimeInSec, currentClip;
    GameObject[] figureObjects;
    [SerializeField] TextMeshProUGUI[] figCounterCircle;
    int currentClickedObjectIndex, sampleButtonPressed;
    int currentClickedInstanceObjectIndex;
    private float currentLossyScale, tmpTime;
    private string currentName;
    private int countOverlaps;
    private List<MusicPiece> listWithoutCurrentFigure = new List<MusicPiece>();
    private int currentPosInList;
    float _idleTimer = -1;
    private float alpha = 1;
    Color colSpaceWarning;
    #endregion
    #region Lists
    // Objects Position List
    [Serializable]
    public class MusicPiece
    {
        public string objName;
        public Vector2 position;        // x ist x-Position (anchoredPosition.x) des objekts, y ist die länge des objekts (sizeDelta.x)
        public int layer;
        public int neighborLeft;
        public int neighborRight;
        public GameObject musicPiece;
    }
    #endregion
    void Awake()
    {
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
        //figCounterCircle = new GameObject[figureObjects.Length];

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
        // for (int i = 0; i < figureObjects.Length; i++)
        // {
        //     //figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
        //     figCounterCircle[i].text = "0";
        // }

        List<GameObject> timelineObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects = new List<GameObject>();
        List<GameObject> figuresLayer1 = new List<GameObject>();
        List<GameObject> figuresLayer2 = new List<GameObject>();

        maxTimeInSec = (int)AnimationTimer.GetMaxTime();
        sizeDeltaAsFactor = new Vector2(1.0f, 1.0f);
    }
    /*private string identifyClickedObjectByList(List<GameObject> objectsOnTimeline)
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
    }*/
    public void openTimelineByClick(bool thisTimelineOpen, bool fromShelf)
    {
        if (fromShelf == false && SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive != 3)
        {
            UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf07(true);
        }
        if (thisTimelineOpen)
        {
            for (int i = 0; i < myObjects.Count; i++)
            {
                SceneManaging.highlight(myObjects[i].musicPiece, false, "music");
            }
        }
        else
        {
            // a different rail is open - close it
            for (int i = 0; i < contentRailsMenue.railList.Length; i++)
            {
                if (contentRailsMenue.railList[i].isTimelineOpen)
                {
                    contentRailsMenue.rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 20);
                    contentRailsMenue.rails[i].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 20);
                    contentRailsMenue.railList[i].isTimelineOpen = false;
                    for (int j = 0; j < contentRailsMenue.railList[i].myObjects.Count; j++)
                    {
                        SceneManaging.highlight(contentRailsMenue.railList[i].myObjects[j].figure3D, contentRailsMenue.railList[i].myObjects[j].figure, false, "figure");
                        contentRailsMenue.openCloseObjectInTimeline(true, contentRailsMenue.railList[i].myObjects, i);
                    }
                }
                contentRailsMenue.openCloseObjectInTimeline(false, contentRailsMenue.railList[i].myObjects, i);
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
            openCloseObjectInTimeline(true);
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
    private void updateObjectPosition(MusicPiece obj, Vector2 mousePos)
    {
        obj.musicPiece.transform.position = new Vector3(mousePos.x, mousePos.y, -1.0f);
        obj.position = new Vector2(obj.musicPiece.GetComponent<RectTransform>().anchoredPosition.x, obj.musicPiece.GetComponent<RectTransform>().sizeDelta.x);
    }
    private bool checkHittingTimeline(Vector2 mousePos)
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
    public void openCloseObjectInTimeline(bool timelineOpen)
    {
        for (int i = 0; i < myObjects.Count; i++)
        {
            myObjects[i].musicPiece.GetComponent<RectTransform>().anchoredPosition = new Vector3(myObjects[i].position.x, -GetComponent<RectTransform>().rect.height / 2, -1);

            if (timelineOpen)
            {
                if (sizeLayering == 1)
                {
                    SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 0, gameObject, myObjects[i].position.y);
                }
                else if (sizeLayering == 2)
                {
                    if (myObjects[i].layer == 1)
                    {
                        SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 1, gameObject, myObjects[i].position.y);
                        myObjects[i].musicPiece.GetComponent<BoxCollider2D>().offset = new Vector2(myObjects[i].musicPiece.GetComponent<BoxCollider2D>().offset.x, GetComponent<RectTransform>().rect.height / 4);
                    }
                    else
                    {
                        SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 2, gameObject, myObjects[i].position.y);
                        myObjects[i].musicPiece.GetComponent<BoxCollider2D>().offset = new Vector2(myObjects[i].musicPiece.GetComponent<BoxCollider2D>().offset.x, -GetComponent<RectTransform>().rect.height / 4);
                    }
                }
            }
            else
            {
                if (sizeLayering == 1)
                {
                    SceneManaging.scaleObject(myObjects[i].musicPiece, myObjects[i].position.y, 20, true);
                }
                else if (sizeLayering == 2)
                {
                    myObjects[i].musicPiece.GetComponent<RectTransform>().anchoredPosition = new Vector3(myObjects[i].position.x, -10, -1);

                    SceneManaging.scaleObject(myObjects[i].musicPiece, myObjects[i].position.y, 10, true);
                }
            }
        }
    }
    private void setObjectOnTimeline(GameObject fig, float y)
    {
        fig.transform.position = new Vector3(fig.transform.position.x, y, -1.0f);
    }
    private int countCopiesOfObject(GameObject fig, List<MusicPiece> tlObjs)
    {
        int c = 0;
        //count object with the same name as fig
        foreach (MusicPiece gO in tlObjs)
        {
            if (gO.objName.Contains(fig.name))
            {
                c++;
            }
        }
        return c;
    }
    private void CreateListWithoutCurrentFigure(MusicPiece mus)
    {
        listWithoutCurrentFigure.Clear();

        for (int i = 0; i < myObjects.Count; i++)
        {
            if (myObjects[i].objName != mus.objName)
            {
                listWithoutCurrentFigure.Add(myObjects[i]);
            }
        }
        //Debug.Log("liste ohne current figure: " + listWithoutCurrentFigure.Count);
    }
    public void removeObjectFromTimeline(MusicPiece obj)
    {
        int tmpNr = int.Parse(obj.musicPiece.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].text);


        figCounterCircle[tmpNr - 1].text = (currentCounterNr - 1).ToString();


        StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.objName.Substring(6, 2)) - 1].musicClipElementInstances.Remove(StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.objName.Substring(6, 2)) - 1].musicClipElementInstances[Int32.Parse(obj.objName.Substring(17))]);


        Destroy(obj.musicPiece);

        // erase from layer list
        myObjects.Remove(obj);
        if (!isSomethingOverlapping())
        {
            sizeLayering = 1;
            for (int i = 0; i < myObjects.Count; i++)
            {
                SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 0, gameObject, myObjects[i].position.y);
                myObjects[i].layer = 1;
            }
        }
        myObjects = myObjects.OrderBy(x => x.position.x).ToList();
        SceneManaging.CalculateNeighbors(myObjects);
    }
    private void LoopRight(int startIdx, MusicPiece mus, bool fromLeft)
    {
        RectTransform rectTransform = mus.musicPiece.GetComponent<RectTransform>();

        for (int i = startIdx; i < listWithoutCurrentFigure.Count; i++)
        {
            // wenn es einen rechten nachbarn hat (nicht sich selbst!)
            if (listWithoutCurrentFigure[i].neighborRight != -1)
            {
                //Debug.Log("HAT einen rechten nachbarn");
                // wenn zwischen den beiden genug platz ist
                if (listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborRight].position.x + 1 >= listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + mus.position.y)
                {
                    //Debug.Log("es ist genug platz");
                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        //Debug.Log("layer 1");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                        mus.layer = 1;
                    }
                    else
                    {
                        //Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y);
                        mus.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {
                    //Debug.Log("nicht genug platz. x: " + (listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborRight].position.x) + ", Figur: " + (listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + mus.position.y));
                    // hier passiert nichts, es muss weiter gesucht werden
                }
            }
            else
            {
                //Debug.Log("platz bis rand: " + (listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + rectSize) + ", rand: " + railwidthAbsolute);

                //Debug.Log("hat KEINEN rechten nachbarn");
                if (listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + mus.position.y - 50 < railWidthAbsolute)
                {
                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        //Debug.Log("layer 1");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                        mus.layer = 1;
                    }
                    else
                    {
                        //_Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y);
                        mus.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {
                    if (!fromLeft)
                    {
                        // hier muss links weitergesucht werden
                        //Debug.Log("kein platz bis zum rand, es muss links weitergesucht werden");
                        LoopLeft(listWithoutCurrentFigure.Count - 1, mus, true);
                    }
                    else
                    {
                        ////Debug.Log("alles voll!");
                        if (currentClickedInstanceObjectIndex != -1)
                        {
                            //Debug.Log("in der timeline");
                            // wenn maus links vom objekt ist
                            // if (Input.mousePosition.x < listWithoutCurrentFigure[currentClickedInstanceObjectIndex].figure.GetComponent<RectTransform>().position.x + listWithoutCurrentFigure[currentClickedInstanceObjectIndex].position.y / 2)
                            // {
                            //     listWithoutCurrentFigure[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                            // }
                            // else
                            // {
                            //     listWithoutCurrentFigure[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
                            // }
                        }
                        // vom shelf
                        else
                        {
                            // ins shelf zurueck!
                            spaceWarning.transform.position = new Vector2(spaceWarning.transform.position.x, gameObject.transform.position.y);
                            _toBeRemoved = true;
                            spaceWarning.enabled = true;
                            //Debug.Log("vom shelf");
                            _idleTimer = 0;
                        }
                    }
                    break;
                }
            }
        }
    }
    private void LoopLeft(int startIdx, MusicPiece mus, bool fromRight)
    {
        RectTransform rectTransform = mus.musicPiece.GetComponent<RectTransform>();

        for (int i = startIdx; i >= 0; i--)
        {
            // wenn es einen linken nachbarn hat
            if (listWithoutCurrentFigure[i].neighborLeft != -1)
            {
                //Debug.Log(listWithoutCurrentFigure[i].objName + " HAT einen linken nachbarn: " + listWithoutCurrentFigure[i].neighborLeft);
                // wenn zwischen den beiden genug platz ist
                if (listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborLeft].position.x + listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborLeft].position.y + mus.position.y <= listWithoutCurrentFigure[i].position.x + 1)
                {
                    //Debug.Log("es ist genug platz");
                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        //Debug.Log("layer 1");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                        mus.layer = 1;
                    }
                    else
                    {
                        //Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y);
                        mus.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x - mus.position.y, rectTransform.anchoredPosition.y);
                    break;
                }
                else
                {
                    //Debug.Log("nicht genug platz. x: " + (listWithoutCurrentFigure[i].position.x) + ", Figur: " + (listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborLeft].position.x + listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborLeft].position.y + mus.position.y));
                    // hier passiert nichts, es muss weiter gesucht werden
                }
            }
            else
            {
                //Debug.Log(listWithoutCurrentFigure[i].objName + " hat KEINEN linken nachbarn");
                if (listWithoutCurrentFigure[i].position.x - mus.position.y > 50)
                {
                    //Debug.Log("platz bis rand");
                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        //Debug.Log("layer 1");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                        mus.layer = 1;

                    }
                    else
                    {
                        //Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y);
                        mus.layer = 2;
                    }
                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x - mus.position.y, rectTransform.anchoredPosition.y);
                    break;
                }
                else
                {
                    // hier muss rechts weitergesucht werden
                    if (!fromRight)
                    {
                        //Debug.Log("kein platz bis zum rand, es muss rechts weitergesucht werden");
                        LoopRight(0, mus, true);
                    }
                    else
                    {
                        // ins shelf zurueck!
                        //Debug.Log("alles voll!");
                        if (currentClickedInstanceObjectIndex != -1)
                        {
                            //Debug.Log("in der timeline");
                            //rectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[listWithoutCurrentFigure[i].neighborLeft].position.x + railList[currentRailIndex].myObjects[listWithoutCurrentFigure[i].neighborLeft].position.y, rectTransform.anchoredPosition.y);

                            // wenn maus links vom objekt ist
                            // if (Input.mousePosition.x < listWithoutCurrentFigure[i].figure.GetComponent<RectTransform>().position.x + listWithoutCurrentFigure[currentClickedInstanceObjectIndex].position.y / 2)
                            // {
                            //     listWithoutCurrentFigure[i].figure.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                            // }
                            // else
                            // {
                            //     listWithoutCurrentFigure[i].figure.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
                            // }
                            //_toBeRemovedFromTimeline = true;
                        }
                        // vom shelf
                        else
                        {
                            spaceWarning.transform.position = new Vector2(spaceWarning.transform.position.x, transform.position.y);
                            _toBeRemoved = true;
                            spaceWarning.enabled = true;
                            _idleTimer = 0;
                            //Debug.Log("vom shelf");
                            //WarningAnimation();
                        }
                    }
                    break;
                }
            }
        }
    }
    private void FindFreeSpot(MusicPiece mus)
    {
        // find out index in the middle
        int middleIdx;
        RectTransform rectTransform = mus.musicPiece.GetComponent<RectTransform>();
        // //Debug.Log("count: " + listWithoutCurrentmusure.Count);

        if (listWithoutCurrentFigure.Count % 2 == 1)
        {
            middleIdx = ((listWithoutCurrentFigure.Count - 1) / 2);
        }
        else
        {
            middleIdx = (listWithoutCurrentFigure.Count / 2);
        }

        // maus ist LINKS vom bereich der figuren
        if (Input.mousePosition.x < listWithoutCurrentFigure[middleIdx].musicPiece.transform.position.x + (listWithoutCurrentFigure[middleIdx].position.y / 2) - 100)
        {
            //Debug.Log("_________________MAUS LINKS__________________");
            int idx = isCurrentFigureOverlapping(mus, "left", out countOverlaps, listWithoutCurrentFigure);
            if (countOverlaps == 1)
            {
                //Debug.Log("hier kann das element frei verschoben werden");
                // overlapping obj is layer 1, obj needs to be layer 2
                if (listWithoutCurrentFigure[idx].layer == 1)
                {
                    //Debug.Log("layer 1");
                    mus.layer = 2;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y);
                }
                // overlapping obj is layer 2, obj needs to be layer 1
                else
                {
                    //Debug.Log("layer 2");
                    mus.layer = 1;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                }
            }
            else
            {
                if (idx != -1)
                {
                    //Debug.Log("idx: " + idx);
                    // gibt es ein idx + 1 mit unterschiedlicher Layer?
                    if (idx < listWithoutCurrentFigure.Count - 1 && listWithoutCurrentFigure[idx + 1].layer != listWithoutCurrentFigure[idx].layer
                    && listWithoutCurrentFigure[idx + 1].objName != mus.objName)
                    {
                        //Debug.Log("es gibt ein idx+1: " + listWithoutCurrentFigure[idx + 1].objName);
                        // hat es einen linken nachbarn? 
                        LoopLeft(idx + 1, mus, false);
                    }
                    else
                    {
                        //Debug.Log("es gibt kein idx+1");

                        LoopLeft(idx, mus, false);
                    }
                }
                // nichts ueberlappt
                else
                {
                    //Debug.Log("not overlapping");
                    if (mus.layer != 1)
                    {
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                        mus.layer = 1;
                    }
                }
            }
        }
        // maus ist RECHTS vom bereich der figuren
        else
        {
            //Debug.Log("_________________MAUS RECHTS__________________");
            int idx = isCurrentFigureOverlapping(mus, "right", out countOverlaps, listWithoutCurrentFigure);
            if (countOverlaps == 1)
            {
                //Debug.Log("hier kann das element frei verschoben werden");
                // overlapping obj is layer 1, obj needs to be layer 2
                if (listWithoutCurrentFigure[idx].layer == 1)
                {
                    //Debug.Log("layer 1");
                    mus.layer = 2;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y);
                }
                // overlapping obj is layer 2, obj needs to be layer 1
                else
                {
                    //Debug.Log("layer 2");
                    mus.layer = 1;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                }
            }
            else
            {
                if (idx != -1)
                {
                    //Debug.Log("idx: " + idx);
                    // gibt es ein idx - 1 mit unterschiedlicher Layer?
                    if (idx > 0 && listWithoutCurrentFigure[idx - 1].layer != listWithoutCurrentFigure[idx].layer)
                    {
                        //Debug.Log("es gibt ein idx-1: " + listWithoutCurrentFigure[idx - 1].objName);
                        // hat es einen rechten nachbarn? 
                        LoopRight(idx - 1, mus, false);
                    }
                    else
                    {
                        //Debug.Log("es gibt kein idx-1");
                        LoopRight(idx, mus, false);
                    }
                }
                else
                {
                    //Debug.Log("not overlapping");
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y);
                    mus.layer = 1;
                }
            }
        }

        mus.position = new Vector2(rectTransform.anchoredPosition.x, mus.position.y);
        rectTransform.sizeDelta = new Vector2(mus.position.y, rectTransform.sizeDelta.y);
    }
    public void CreateNew2DInstance(int musObjNr, float moment, float savedLayer)
    {
        //create a copy of this timelineObject and keep the original one
        newCopyOfFigure = Instantiate(figureObjects[musObjNr]);

        //count objects from same kind
        int countName = 0;
        countName = countCopiesOfObject(figureObjects[musObjNr], myObjects);
        figCounterCircle[musObjNr].text = (countName + 1).ToString();
        newCopyOfFigure.name = figureObjects[musObjNr].name + "_instance" + countName.ToString("000");
        RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();

        // in der Besucherversion auf 60 sek kuerzen, weil gesamtes stueck nur 3 min lang ist
        float tmpLength;
        if (!gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
            tmpLength = UtilitiesTm.FloatRemap(60, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
        else
            tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);

        createRectangle(newCopyOfFigure, gameObject.GetComponent<RectTransform>().rect.height, tmpLength);
        ////Debug.Log("tmpLength: " + tmpLength);
        SceneManaging.scaleObject(newCopyOfFigure, tmpLength, gameObject.GetComponent<RectTransform>().rect.height, false);
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.x, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.y, -1);

        //parent and position
        float posX = UtilitiesTm.FloatRemap(moment, 0, AnimationTimer.GetMaxTime(), 0, gameObject.GetComponent<RectTransform>().rect.width);
        newCopyOfFigure.transform.SetParent(gameObject.transform);
        newCopyOfFigure.transform.localPosition = new Vector3(posX, 0, -1);

        //openTimelineByClick(true, true);
        newCopyOfFigure.transform.localScale = Vector3.one;

        // size of rectangle becomes size for figure that is clickable
        tmpRectTransform.position = new Vector3(tmpRectTransform.position.x, tmpRectTransform.position.y, -1.0f);
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);

        MusicPiece oP = new MusicPiece();
        oP.objName = newCopyOfFigure.name;
        oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x);
        oP.musicPiece = newCopyOfFigure;
        oP.neighborLeft = -1;
        oP.neighborRight = -1;
        myObjects.Add(oP);

        // tmpRectTransform.anchoredPosition = new Vector3(posX, GetComponent<RectTransform>().rect.height / 2, -1);
        // oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, rectSize);

        if (savedLayer == 1)
        {
            SceneManaging.scaleToLayerSize(newCopyOfFigure, 1, gameObject, tmpLength);
            sizeLayering = 2;
            oP.layer = 1;
        }
        else if (savedLayer == 0)
        {
            SceneManaging.scaleToLayerSize(newCopyOfFigure, 0, gameObject, tmpLength);
            oP.layer = 1;
        }
        else
        {
            SceneManaging.scaleToLayerSize(newCopyOfFigure, 2, gameObject, tmpLength);
            sizeLayering = 2;
            oP.layer = 2;
        }
    }
    private void createRectangle(GameObject obj, float height, double animLength)
    {
        //double tmpLength = (maxX - minX) / 614.0f * animLength;
        //animLength -= 25;
        GameObject imgObject = Instantiate(prefabRect);
        RectTransform trans = imgObject.GetComponent<RectTransform>();
        trans.transform.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;


        trans.sizeDelta = new Vector2((float)animLength, height); // custom size
        //Debug.Log("size: " + animLength);

        trans.pivot = new Vector2(0.0f, 0.5f);
        //set pivot point of sprite  to left border, so that rect aligns with sprite
        trans.anchorMin = trans.anchorMax = new Vector2(0, 0.5f);
        trans.anchoredPosition = new Vector2(0, 0);

        trans.SetSiblingIndex(0);

        Image image = imgObject.GetComponent<Image>();
        image.color = colMusic;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
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
        for (int i = 0; i < myObjects.Count; i++)
        {
            double startSec = UtilitiesTm.FloatRemap(myObjects[i].position.x, 0, gameObject.GetComponent<RectTransform>().rect.width, 0, AnimationTimer.GetMaxTime());
            double endSec;
            //Debug.Log("startsec: "+startSec);
            if (!gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
                endSec = startSec + 60;
            else
                endSec = startSec + myObjects[i].musicPiece.GetComponent<MusicLength>().musicLength;
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
            double startSec = UtilitiesTm.FloatRemap(myObjects[m].position.x, 0, gameObject.GetComponent<RectTransform>().rect.width, 0, AnimationTimer.GetMaxTime());
            //Debug.Log("start: " + startSec);
            tmpTime = AnimationTimer.GetTime();

            anyInstanceIsPlaying = true;

            if (playingMusic == false || once == true)
            {
                playingMusic = true;
                audioSource.time = tmpTime - (float)startSec;
                currentClip = ((int)Char.GetNumericValue(myObjects[m].musicPiece.name[07]) - 1); // object index
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
            double startSec = UtilitiesTm.FloatRemap(myObjects[n].position.x, 0, gameObject.GetComponent<RectTransform>().rect.width, 0, AnimationTimer.GetMaxTime());
            float tmpTime = AnimationTimer.GetTime();
            if (!firstTimeSecond) // es switcht von 1 auf 2 musikstuecke gleichzeitig
            {
                fading = false;
                firstTimeSecond = true;
            }
            if (!fading) StartCoroutine(FadeOut(audioSource, 1, myObjects[n].musicPiece, (tmpTime - (float)startSec)));

            anyInstanceIsPlaying = true;

            if (playingMusic == false || once == true)
            {
                playingMusic = true;
                audioSource.time = tmpTime - (float)startSec;
                currentClip = ((int)Char.GetNumericValue(myObjects[n].musicPiece.name[07]) - 1); // object index
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
                    StartCoroutine(FadeOut(audioSource, 1, myObjects[m].musicPiece, -1));
                    onlyPiecefinished = false;
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
    }
    private bool isSomethingOverlapping()
    {
        bool val = false;
        for (int i = 0; i < myObjects.Count; i++)
        {
            for (int j = 0; j < myObjects.Count; j++)
            {
                RectTransform tmpRectTransform = myObjects[j].musicPiece.GetComponent<RectTransform>();
                if (myObjects[j] != myObjects[i] && (tmpRectTransform.anchoredPosition.x >= myObjects[i].position.x
                && (tmpRectTransform.anchoredPosition.x <= (myObjects[i].position.x + myObjects[i].position.y))
                || ((tmpRectTransform.anchoredPosition.x + myObjects[j].position.y) >= myObjects[i].position.x)
                && tmpRectTransform.anchoredPosition.x <= myObjects[i].position.x))
                {
                    val = true;
                }
            }
        }
        return val;
    }
    private int isCurrentFigureOverlapping(MusicPiece obj, string dir, out int count, List<MusicPiece> musicList)
    {
        int val = -1;
        int val2 = -1;
        int val3 = -1;

        count = 0;
        if (dir == "right")
        {
            for (int i = 0; i < musicList.Count; i++)
            {
                //Debug.Log("name: " + figureList.objName);
                if (((musicList[i].position.x <= obj.position.x
                && musicList[i].position.x + musicList[i].position.y >= obj.position.x)
                || (musicList[i].position.x >= obj.position.x
                && musicList[i].position.x <= obj.position.x + obj.position.y))
                && musicList[i].objName != obj.objName)
                {
                    if (val2 != -1)
                        val3 = val2;
                    if (val == -1)
                        val = i;
                    val2 = i;

                    count++;
                    //Debug.LogWarning("val2: " + val2 + ", val3: " + val3);
                }
                if (count > 1)
                {
                    if (musicList[val2].layer == musicList[val3].layer)
                    {
                        //Debug.Log("count: " + count);
                        count = 1;
                    }
                    else
                    {
                        //Debug.Log("es gibt mehr als eine überlappung auf unterschiedlichen Ebenen");
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = musicList.Count - 1; i >= 0; i--)
            {
                if (((musicList[i].position.x <= obj.position.x
                && musicList[i].position.x + musicList[i].position.y >= obj.position.x)
                || (musicList[i].position.x >= obj.position.x
                && musicList[i].position.x <= obj.position.x + obj.position.y))
                && musicList[i].objName != obj.objName)
                {
                    if (val2 != -1)
                        val3 = val2;
                    if (val == -1)
                        val = i;
                    val2 = i;

                    count++;
                    //Debug.LogWarning("val2: " + val2 + ", val3: " + val3);
                }
                if (count > 1)
                {
                    if (musicList[val2].layer == musicList[val3].layer)
                    {
                        //Debug.Log("count: " + count);
                        count = 1;
                    }
                    else
                    {
                        //Debug.Log("es gibt mehr als eine überlappung auf unterschiedlichen Ebenen");
                        break;
                    }
                }
            }
        }

        return val;
    }
    void UpdatePositionVectorInformation(GameObject currentObj, List<MusicPiece> listFrom, List<MusicPiece> listTo)
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

        // timer
        if (_idleTimer >= 0 && _idleTimer < 3)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= 2)
            {
                SceneManaging.WarningAnimation(colSpaceWarning, spaceWarning);
            }
        }
        else if (_idleTimer >= 3)
        {
            _idleTimer = -1;
            alpha = 1;
            colSpaceWarning = new Color(1, 0, 0, alpha);
            spaceWarning.color = colSpaceWarning;
            spaceWarning.enabled = false;
        }


        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            //identify which gameobject you clicked
            currentClickedObjectIndex = SceneManaging.identifyClickedObject(figureObjects);         //method fills up the current clicked index
            currentClickedInstanceObjectIndex = SceneManaging.identifyClickedObjectByList(myObjects);
            editTimelineObject = false;                             //flag to prevent closing the timeline if you click an object in timeline
            releaseOnTimeline = false;                              //because you have not set anything on timeline 
            releaseObjMousePos = new Vector2(0.0f, 0.0f);

            int tmpI = -1;
            for (int i = 0; i < myObjects.Count; i++)
            {
                if (myObjects[i].musicPiece.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
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
                //wenn neues objekt genommen wird, soll altes aktuelles objekt unhighlighted werden
                for (int j = 0; j < myObjects.Count; j++)
                {
                    SceneManaging.highlight(myObjects[j].musicPiece, false, "music");
                }

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

                if (myObjects[currentClickedInstanceObjectIndex].musicPiece.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    // aktuellen namen speichern, um spaeter die aktuelle position nach dem sortieren rauszubekommen
                    currentName = myObjects[currentClickedInstanceObjectIndex].objName;

                    CreateListWithoutCurrentFigure(myObjects[currentClickedInstanceObjectIndex]);
                    SceneManaging.CalculateNeighbors(listWithoutCurrentFigure);

                    // click on delete-Button
                    Vector2 pos = myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.GetChild(1).GetChild(0).position;
                    RectTransform rect = myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();

                    if (myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.GetChild(1).GetChild(0).gameObject.activeSelf
                        && getMousePos.x >= pos.x - rect.sizeDelta.x / 2.5f && getMousePos.x <= pos.x + rect.sizeDelta.x / 2.5f
                        && getMousePos.y >= pos.y - rect.sizeDelta.y / 2.5f && getMousePos.y <= pos.y + rect.sizeDelta.y / 2.5f)
                    {
                        _toBeRemoved = true;
                    }
                    diff = new Vector2(getMousePos.x - myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.position.x, getMousePos.y - myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.position.y);

                    //set up some flags
                    draggingObject = true;
                    draggingOnTimeline = true;

                    //highlighting objects and showing delete button when clicked
                    if (SceneManaging.highlighted == false)
                    {
                        SceneManaging.highlight(myObjects[currentClickedInstanceObjectIndex].musicPiece, true, "music");
                    }
                    else if (SceneManaging.highlighted && myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.GetChild(0).GetComponent<Image>().color == colMusicHighlighted) // checkFigureHighlighted(timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject))  // check if second child (which is never the armature) has emission enabled (=is highlighted)
                    {
                        // highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex], false);
                    }
                    else
                    {
                        for (int i = 0; i < myObjects.Count; i++)
                        {
                            SceneManaging.highlight(myObjects[i].musicPiece, false, "music");
                        }
                        SceneManaging.highlight(myObjects[currentClickedInstanceObjectIndex].musicPiece, true, "music");
                    }
                }

            }

            //if you hit/clicked nothing with mouse
            else if (Physics2D.OverlapPoint(getMousePos) == false)
            {
                for (int i = 0; i < myObjects.Count; i++)
                {
                    SceneManaging.highlight(myObjects[i].musicPiece, false, "music");
                }
            }

        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline && isTimelineOpen)
        {
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                MusicPiece currentObj = myObjects[currentClickedInstanceObjectIndex];
                RectTransform currentPos = currentObj.musicPiece.GetComponent<RectTransform>();
                isInstance = true;

                // abfrage, ob reihenfolge sich getauscht hat
                if (currentName != currentObj.objName)
                {
                    for (int i = 0; i < myObjects.Count; i++)
                    {
                        if (myObjects[i].objName == currentName)
                        {
                            //Debug.Log("tauschen");
                            currentClickedInstanceObjectIndex = i;
                            //currentObj = currentObj;
                        }
                    }
                }

                //if you click an object in timeline (for dragging) //move object
                updateObjectPosition(currentObj, getMousePos - diff);
                setObjectOnTimeline(currentObj.musicPiece, transform.position.y); //snapping/lock y-axis

                #region Stapeln von Ebenen
                // if there is only one layer on Rail
                if (sizeLayering == 1)
                {
                    if (isCurrentFigureOverlapping(currentObj, "", out countOverlaps, myObjects) != -1)
                    {
                        SceneManaging.scaleToLayerSize(currentObj.musicPiece, 2, gameObject, currentObj.position.y);
                        sizeLayering = 2;
                        currentObj.layer = 2;

                        // scale all other timelineobjects of layer 1
                        for (int j = 0; j < myObjects.Count; j++)
                        {
                            if (myObjects[j].musicPiece != currentObj.musicPiece && myObjects[j].layer == 1)
                            {
                                SceneManaging.scaleToLayerSize(myObjects[j].musicPiece, 1, gameObject, currentObj.position.y);
                                myObjects[j].layer = 1;
                            }
                        }

                    }
                }

                // if there are two layers on Rail
                else
                {
                    // wenn nichts mehr überlappt
                    if (!isSomethingOverlapping())
                    {
                        sizeLayering = 1;
                        // scale all other timelineobjects of layer 0
                        for (int j = 0; j < myObjects.Count; j++)
                        {
                            SceneManaging.scaleToLayerSize(myObjects[j].musicPiece, 0, gameObject, myObjects[j].position.y);
                            myObjects[j].layer = 1;
                        }
                    }
                    else
                    {
                        FindFreeSpot(currentObj);
                    }
                }
                #endregion


                #region limits front and back of rail
                // limit front of rail
                if (currentPos.anchoredPosition.x <= 0)
                {
                    currentPos.anchoredPosition = new Vector2(0, currentPos.anchoredPosition.y);    // tendenziell muesste das eher in buttonUp
                    currentObj.position = new Vector2(0, currentObj.position.y);
                }

                // limit back of rail
                else if ((currentPos.anchoredPosition.x + currentPos.sizeDelta.x) > railWidthAbsolute)
                {
                    //currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - currentObj.transform.GetComponent<BoxCollider2D>().size.x), currentObj.GetComponent<RectTransform>().anchoredPosition.y, -1);
                    //currentObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().rect.width - currentSize, currentObj.GetComponent<RectTransform>().anchoredPosition.y);

                    currentPos.anchoredPosition = new Vector2(railWidthAbsolute - currentPos.sizeDelta.x, currentPos.anchoredPosition.y);
                    currentObj.position = new Vector2(railWidthAbsolute - currentPos.sizeDelta.x, currentObj.position.y);
                }

                // if (Physics2D.OverlapPoint(getMousePos) == false)       // mouse outside
                // {
                //     //SceneManaging.objectsTimeline = ((int)Char.GetNumericValue(timelineImage.name[17]) - 1);  // save old timeline to remove instance of this timeline
                //     draggingObject = true;
                //     draggingOnTimeline = false;
                // }
                #endregion
            }
            releaseOnTimeline = true;
        }
        //if something has been dragged outside of the timeline
        // else if (draggingOnTimeline == false && editTimelineObject == false && draggingObject && isInstance)
        // {
        //     bool hitTimeline;
        //     hitTimeline = checkHittingTimeline(getMousePos);

        //     myObjects[currentClickedInstanceObjectIndex].transform.GetChild(0).gameObject.SetActive(false);
        //     //moving on mouse pos
        //     updateObjectPosition(myObjects[currentClickedInstanceObjectIndex], getMousePos);
        //     releaseOnTimeline = false;
        // }

        // dragging an object from shelf to timeline
        if (draggingObject && editTimelineObject == false && isInstance == false)
        {
            //move object
            figureObjects[currentClickedObjectIndex].transform.position = new Vector3(getMousePos.x - diff.x, getMousePos.y - diff.y, -1.0f);

            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            bool hitTimeline = false;
            hitTimeline = checkHittingTimeline(getMousePos);

            if (hitTimeline)
            {
                // change parent back and snap
                setParent(figureObjects[currentClickedObjectIndex], gameObject);
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], this.transform.position.y);

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

            if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)
                {
                    //create a copy of this timelineObject and keep the original one
                    newCopyOfFigure = Instantiate(figureObjects[currentClickedObjectIndex]);
                    //count objects from same kind
                    int countName = 0;
                    countName = countCopiesOfObject(figureObjects[currentClickedObjectIndex], myObjects);
                    newCopyOfFigure.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");
                    RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();

                    // in der Besucherversion auf 60 sek kuerzen, weil gesamtes stueck nur 3 min lang ist
                    float tmpLength;
                    if (!gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
                        tmpLength = UtilitiesTm.FloatRemap(60, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
                    else
                        tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);

                    createRectangle(newCopyOfFigure, tmpLength, gameObject.GetComponent<RectTransform>().rect.height);

                    figCounterCircle[currentClickedObjectIndex].text = (countName + 1).ToString();

                    //parent and position
                    newCopyOfFigure.transform.SetParent(gameObject.transform);
                    newCopyOfFigure.transform.localScale = Vector3.one;

                    #region limit front and end of rail
                    //------------------------------------------limit front of rail---------------------------------------------//
                    if (figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().position.x < minX)  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
                    {
                        tmpRectTransform.position = new Vector3((float)minX, figureObjects[currentClickedObjectIndex].transform.position.y, -1);
                    }
                    //------------------------------------------limit back of rail---------------------------------------------//
                    else if ((tmpRectTransform.anchoredPosition.x + tmpLength) > railWidthAbsolute)
                    {
                        tmpRectTransform.position = new Vector2(tmpRectTransform.position.x, figureObjects[currentClickedObjectIndex].transform.position.y);
                        tmpRectTransform.anchoredPosition = new Vector3((railWidthAbsolute - tmpLength), tmpRectTransform.anchoredPosition.y, -1);
                    }
                    else
                    {
                        newCopyOfFigure.transform.position = new Vector3(figureObjects[currentClickedObjectIndex].transform.position.x, figureObjects[currentClickedObjectIndex].transform.position.y, -1);
                        //Debug.Log("anch pos: "+tmpRectTransform.anchoredPosition.x+", pos: "+newCopyOfFigure.transform.position.x);
                        // pos to mouse pos
                        //tmpRectTransform.transform.position = new Vector2(getMousePos.x, tmpRectTransform.transform.position.y);
                    }
                    #endregion

                    // size of rectangle becomes size for figure that is clickable (bring in front)
                    tmpRectTransform.localPosition = new Vector3(tmpRectTransform.localPosition.x, tmpRectTransform.localPosition.y, -1.0f);
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);
                    newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y);

                    //set original image back to shelf, position 2 to make it visible
                    figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                    figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                    //figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(false);

                    #region calculating Layer Overlap

                    MusicPiece oP = new MusicPiece();
                    oP.objName = newCopyOfFigure.name;
                    oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpLength);
                    oP.musicPiece = newCopyOfFigure;
                    oP.neighborLeft = -1;
                    oP.neighborRight = -1;
                    myObjects.Add(oP);

                    SceneManaging.scaleToLayerSize(newCopyOfFigure, 0, gameObject, tmpLength);
                    oP.layer = 1;

                    if (sizeLayering == 1)
                    {
                        if (isCurrentFigureOverlapping(oP, "", out countOverlaps, myObjects) != -1)
                        {
                            sizeLayering = 2;
                            //Debug.Log("something overlapping");
                            SceneManaging.scaleToLayerSize(newCopyOfFigure, 2, gameObject, tmpLength);
                            oP.layer = 2;
                            // others to 1
                            for (int i = 0; i < myObjects.Count; i++)
                            {
                                if (myObjects[i].layer == 1)
                                    SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 1, gameObject, tmpLength);
                            }
                            //Debug.Log("op: "+oP.position);
                        }

                    }
                    else
                    {
                        listWithoutCurrentFigure.Clear();
                        listWithoutCurrentFigure = myObjects;
                        FindFreeSpot(oP);
                    }
                    #endregion

                    myObjects = myObjects.OrderBy(w => w.position.x).ToList();
                    // an welcher position ist aktuelle Figur nach der Sortierung? (benoetigt fuer highlight und loeschen)
                    for (int i = 0; i < myObjects.Count; i++)
                    {
                        if (oP.objName == myObjects[i].objName)
                            currentPosInList = i;
                    }
                    SceneManaging.CalculateNeighbors(myObjects);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Save to SceneData:
                    MusicClipElementInstance musicClipElementInstance = new MusicClipElementInstance();
                    musicClipElementInstance.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex], myObjects);
                    musicClipElementInstance.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");
                    StaticSceneData.StaticData.musicClipElements[currentClickedObjectIndex].musicClipElementInstances.Add(musicClipElementInstance);

                    //Debug.Log("figure: " + myObjects[currentPosInList].musicPiece.name);
                    SceneManaging.highlight(myObjects[currentPosInList].musicPiece, true, "music");

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (_toBeRemoved)
                    {
                        Debug.Log("remove: " + myObjects[currentPosInList].objName);
                        removeObjectFromTimeline(myObjects[currentPosInList]);
                    }

                    // set index back to -1 because nothing is being clicked anymore
                    currentClickedObjectIndex = -1;
                }
                else
                {
                    // change position of vector in each case
                    for (int i = 0; i < myObjects.Count; i++)
                    {
                        if (myObjects[i].objName == myObjects[currentClickedInstanceObjectIndex].objName)
                        {
                            myObjects[i].position = new Vector2(myObjects[currentClickedInstanceObjectIndex].position.x, myObjects[currentClickedInstanceObjectIndex].position.y);
                        }
                    }
                    myObjects = myObjects.OrderBy(w => w.position.x).ToList();
                    SceneManaging.CalculateNeighbors(myObjects);

                    // if (_toBeRemovedFromTimeline)
                    // {
                    //     for (int i = 0; i < myObjects.Count; i++)
                    //     {
                    //         if (myObjects[i].objName == currentName)
                    //         {
                    //             Debug.Log("remove: " + myObjects[i].objName);
                    //             removeObjectFromTimeline(myObjects[i]);
                    //         }
                    //     }
                    // }
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
                removeObjectFromTimeline(myObjects[currentClickedInstanceObjectIndex]);
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //Save musictitle.moment to SceneData:
            for (int i = 0; i < myObjects.Count; i++)
            {
                int musObjIndex = Int32.Parse(myObjects[i].objName.Substring(6, 2)) - 1;
                int musInsIndex = Int32.Parse(myObjects[i].objName.Substring(17));
                float moment = UtilitiesTm.FloatRemap(myObjects[i].position.x, 0, gameObject.GetComponent<RectTransform>().rect.width, 0, AnimationTimer.GetMaxTime());

                StaticSceneData.StaticData.musicClipElements[musObjIndex].musicClipElementInstances[musInsIndex].moment = moment;
                if (myObjects[i].layer == 1)
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