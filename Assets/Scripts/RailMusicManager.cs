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
    [HideInInspector] public List<MusicPiece> myObjects = new List<MusicPiece>();
    [HideInInspector] public bool isTimelineOpen;
    public GameObject objectLibrary, parentMenue; // mainMenue
    [HideInInspector] public int sizeLayering = 1;
    public AudioClip[] clip;         // ought to be 6 audioclips
    public int sampleButtonPressed;
    #endregion
    #region private variables
    [SerializeField] Image timelineImage;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private UIController gameController;
    [SerializeField] RailManager contentRailsMenue;
    [SerializeField] private BoxCollider2D _backgroundBoxCollider;
    [SerializeField] private GameObject prefabRect;
    [SerializeField] private TextMeshProUGUI spaceWarning;
    [SerializeField] private Image spaceWarningBorder;
    [SerializeField] private GameObject musicPieceLengthDialog;
    AudioSource audioSource;
    Vector2 objectShelfSize;
    Vector2 screenDifference;
    int[] initialPieceLength = new int[6];
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, playingMusic, isInstance, fading;
    public bool playingSample;
    private bool musicLengthClicked;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    Color colMusic;
    bool anyInstanceIsPlaying = false, firstTimeSecond = true, newPiece = false;
    bool onlyPiecefinished = true, _toBeRemoved, _toBeRemovedFromTimeline;
    Vector2 releaseObjMousePos, diff;
    double minX, maxX;
    private float railWidthAbsolute = 1670.4f, railWidth;
    int maxTimeInSec, currentClip;
    GameObject[] figureObjects;
    [SerializeField] TextMeshProUGUI[] figCounterCircle;
    public Image[] sampleImages;
    int currentClickedObjectIndex;
    int saveIndexForChangePieceLength;
    int currentClickedInstanceObjectIndex;
    private float currentLossyScale, tmpTime;
    private string currentName;
    private int countOverlaps;
    private List<MusicPiece> listWithoutCurrentFigure = new List<MusicPiece>();
    private int currentPosInList;
    float _idleTimer = -1;
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
        //minX = timelineImage.transform.position.x;
        //maxX = minX + railWidth;
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
        objectShelfParent = new GameObject[figureObjects.Length];

        colMusic = new Color(0.21f, 0.51f, 0.267f, 0.5f);

        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject;
            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
            initialPieceLength[i] = figureObjects[i].GetComponent<MusicLength>().musicLength;
        }
        objectShelfSize = new Vector2(190, 146);

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;
        currentLossyScale = 1.0f;

        ResetScreenSize();
        timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidthAbsolute, 20);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidthAbsolute, 20);

        audioSource = GetComponent<AudioSource>();
        if (SceneManaging.isExpert)
        {
            for (int i = 0; i < figureObjects.Length; i++)
            {
                figCounterCircle[i].text = "0";
            }
        }

        maxTimeInSec = (int)AnimationTimer.GetMaxTime();
    }
    public void OpenTimelineByClick(bool thisTimelineOpen, bool fromShelf)
    {
        if (fromShelf == false && SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive != 3)
        {
            UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf07(true);
        }
        if (thisTimelineOpen)
        {
            for (int i = 0; i < myObjects.Count; i++)
            {
                SceneManaging.highlight(myObjects[i].musicPiece, false);
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
                    contentRailsMenue.OpenCloseObjectInTimeline(contentRailsMenue.railList[i].myObjects, i, true);

                    // for (int j = 0; j < contentRailsMenue.railList[i].myObjects.Count; j++)
                    // {
                    //     SceneManaging.highlight(contentRailsMenue.railList[i].myObjects[j].figure3D, contentRailsMenue.railList[i].myObjects[j].figure, false, "figure");
                    // }
                }
            }
            for (int k = 0; k < 2; k++)
            {
                if (gameController.RailLightBG[k].isTimelineOpen)
                {
                    gameController.RailLightBG[k].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 20);
                    gameController.RailLightBG[k].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 20);
                    gameController.RailLightBG[k].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
            }
            contentRailsMenue.currentRailIndex = -1;
            // open clicked rail and scale up timeline
            timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, 80);
            //scale up the collider
            timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, 80);
            OpenCloseObjectInTimeline(true);
            isTimelineOpen = true;
            ImageTimelineSelection.SetRailNumber(6);
            ImageTimelineSelection.SetRailType(2);  // for rail-rails
        }
    }
    private bool CheckHittingTimeline(Vector2 mousePos)
    {
        bool hit = false;
        Vector2 colSize = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);
        //if mouse hits the timeline while dragging an object
        if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= gameObject.transform.position.y + colSize.y * 2 && mousePos.y > gameObject.transform.position.y - colSize.y * 2)
            hit = true;
        return hit;
    }
    private int IsCurrentFigureOverlapping(MusicPiece obj, string dir, out int count, List<MusicPiece> musicList)
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
    public void OpenCloseObjectInTimeline(bool openTimeline)
    {
        for (int i = 0; i < myObjects.Count; i++)
        {
            myObjects[i].musicPiece.GetComponent<RectTransform>().anchoredPosition = new Vector3(myObjects[i].position.x, -GetComponent<RectTransform>().rect.height / 2, -1);

            if (openTimeline)
            {
                if (sizeLayering == 1)
                {
                    SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 0, gameObject, myObjects[i].position.y, false);
                }
                else if (sizeLayering == 2)
                {
                    if (myObjects[i].layer == 1)
                    {
                        SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 1, gameObject, myObjects[i].position.y, false);
                        myObjects[i].musicPiece.GetComponent<BoxCollider2D>().offset = new Vector2(myObjects[i].musicPiece.GetComponent<BoxCollider2D>().offset.x, GetComponent<RectTransform>().rect.height / 4);
                    }
                    else
                    {
                        SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 2, gameObject, myObjects[i].position.y, false);
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
    private int CountCopiesOfObject(GameObject fig, List<MusicPiece> tlObjs)
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
    public void RemoveObjectFromTimeline(MusicPiece obj)
    {
        int tmpNr = int.Parse(obj.musicPiece.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].text);

        figCounterCircle[tmpNr - 1].text = (currentCounterNr - 1).ToString();

        // instance element rauskriegen (beim loeschen veraendert sich der platz im array, aber die endung ist noch 001 bspw.)
        for (int j = 0; j < StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.musicPiece.name.Substring(6, 2)) - 1].musicClipElementInstances.Count; j++)
        {
            if (StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.musicPiece.name.Substring(6, 2)) - 1].musicClipElementInstances[j].instanceNr == int.Parse(obj.musicPiece.name.Substring(17)))
            {
                StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.musicPiece.name.Substring(6, 2)) - 1].musicClipElementInstances.Remove(StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.musicPiece.name.Substring(6, 2)) - 1].musicClipElementInstances[j]);
            }
        }

        Destroy(obj.musicPiece);
        audioSource.Stop();

        // erase from layer list
        myObjects.Remove(obj);
        if (!IsSomethingOverlapping())
        {
            sizeLayering = 1;
            for (int i = 0; i < myObjects.Count; i++)
            {
                SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 0, gameObject, myObjects[i].position.y, false);
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
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
                        mus.layer = 1;
                    }
                    else
                    {
                        //Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y, false);
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
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
                        mus.layer = 1;
                    }
                    else
                    {
                        //_Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y, false);
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
                            spaceWarningBorder.enabled = true;
                            spaceWarningBorder.transform.position = new Vector2(spaceWarning.transform.position.x, gameObject.transform.position.y);
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
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
                        mus.layer = 1;
                    }
                    else
                    {
                        //Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y, false);
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
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
                        mus.layer = 1;

                    }
                    else
                    {
                        //Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y, false);
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
                            spaceWarningBorder.enabled = true;
                            spaceWarningBorder.transform.position = new Vector2(spaceWarning.transform.position.x, gameObject.transform.position.y); _idleTimer = 0;
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
            int idx = IsCurrentFigureOverlapping(mus, "left", out countOverlaps, listWithoutCurrentFigure);
            if (countOverlaps == 1)
            {
                //Debug.Log("hier kann das element frei verschoben werden");
                // overlapping obj is layer 1, obj needs to be layer 2
                if (listWithoutCurrentFigure[idx].layer == 1)
                {
                    //Debug.Log("layer 1");
                    mus.layer = 2;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y, false);
                }
                // overlapping obj is layer 2, obj needs to be layer 1
                else
                {
                    //Debug.Log("layer 2");
                    mus.layer = 1;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
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
                    // if (mus.layer != 1)
                    // {
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
                    mus.layer = 1;
                    // }
                }
            }
        }
        // maus ist RECHTS vom bereich der figuren
        else
        {
            //Debug.Log("_________________MAUS RECHTS__________________");
            int idx = IsCurrentFigureOverlapping(mus, "right", out countOverlaps, listWithoutCurrentFigure);
            if (countOverlaps == 1)
            {
                //Debug.Log("hier kann das element frei verschoben werden");
                // overlapping obj is layer 1, obj needs to be layer 2
                if (listWithoutCurrentFigure[idx].layer == 1)
                {
                    //Debug.Log("layer 1");
                    mus.layer = 2;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 2, gameObject, mus.position.y, false);
                }
                // overlapping obj is layer 2, obj needs to be layer 1
                else
                {
                    //Debug.Log("layer 2");
                    mus.layer = 1;
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
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
                    SceneManaging.scaleToLayerSize(mus.musicPiece, 1, gameObject, mus.position.y, false);
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
        countName = CountCopiesOfObject(figureObjects[musObjNr], myObjects);
        figCounterCircle[musObjNr].text = (countName + 1).ToString();
        newCopyOfFigure.name = figureObjects[musObjNr].name + "_instance" + countName.ToString("000");
        RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();

        // in der Besucherversion auf 60 sek kuerzen, weil gesamtes stueck nur 3 min lang ist
        float tmpLength;
        if (!gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
            tmpLength = UtilitiesTm.FloatRemap(60, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
        else
            tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);

        //parent and position
        float posX = UtilitiesTm.FloatRemap(moment, 0, AnimationTimer.GetMaxTime(), 0, railWidthAbsolute);
        newCopyOfFigure.transform.SetParent(gameObject.transform);
        newCopyOfFigure.transform.localPosition = new Vector3(posX, 0, -1);

        CreateRectangle(newCopyOfFigure, gameObject.GetComponent<RectTransform>().rect.height, tmpLength);
        SceneManaging.scaleObject(newCopyOfFigure, tmpLength, gameObject.GetComponent<RectTransform>().rect.height, true);
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.x, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.y, -1);

        newCopyOfFigure.transform.localScale = Vector3.one;

        // size of rectangle becomes size for figure that is clickable
        tmpRectTransform.position = new Vector3(tmpRectTransform.position.x, tmpRectTransform.position.y, -1.0f);
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);

        MusicPiece oP = new MusicPiece()
        {
            objName = newCopyOfFigure.name,
            position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x),
            musicPiece = newCopyOfFigure,
            neighborLeft = -1,
            neighborRight = -1
        };
        myObjects.Add(oP);

        if (savedLayer == 1)
        {
            SceneManaging.scaleToLayerSize(newCopyOfFigure, 1, gameObject, tmpLength, true);
            sizeLayering = 2;
            oP.layer = 1;
        }
        else if (savedLayer == 0)
        {
            SceneManaging.scaleToLayerSize(newCopyOfFigure, 0, gameObject, tmpLength, true);
            oP.layer = 1;
        }
        else
        {
            SceneManaging.scaleToLayerSize(newCopyOfFigure, 2, gameObject, tmpLength, true);
            sizeLayering = 2;
            oP.layer = 2;
        }
        myObjects = myObjects.OrderBy(w => w.position.x).ToList();
    }
    private void CreateRectangle(GameObject obj, float height, double animLength)
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
        minX = 0.087f * Screen.width;               //timeline-rail-minX
        railWidth = 0.87f * Screen.width;           //railwidth=1670.4px
        maxX = minX + railWidth;
        screenDifference = new Vector2(1920.0f / Screen.width, 1080.0f / Screen.height);

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
        if (sampleButtonPressed == i)
        {
            playingSample = false;
            sampleImages[sampleButtonPressed].color = new Color(1, 1, 1, 0);
            sampleButtonPressed = -1;
        }
        else
        {
            playingSample = true;
            audioSource.clip = clip[i];
            sampleButtonPressed = i;
            for (int j = 0; j < sampleImages.Length; j++)
            {
                sampleImages[j].color = new Color(1, 1, 1, 0);
            }
            sampleImages[sampleButtonPressed].color = new Color(1, 1, 1, 1);
        }
    }
    private IEnumerator FadeOut(float FadeTime, GameObject obj, float time, int clipNr, bool newPiece)
    {
        fading = true;
        while (audioSource.volume > 0.1f)
        {
            audioSource.volume -= 1 * Time.deltaTime / FadeTime;
            yield return null;
        }
        currentClip = (int)char.GetNumericValue(obj.name[07]) - 1; // object index
        audioSource.clip = clip[currentClip];

        if (time != -1)
            StartCoroutine(FadeIn(1, time));
        else
        {
            audioSource.Stop();
            playingMusic = false;
            audioSource.volume = 1;
        }
        if (clipNr != -1)
            audioSource.clip = clip[clipNr];
        if (newPiece)
            this.newPiece = true;
    }
    private IEnumerator FadeIn(float FadeTime, float time)
    {
        audioSource.time = time;
        audioSource.Play();

        while (audioSource.volume < 0.9f)
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.volume = 1f;
    }
    private int GetCurrentMusicCount(out int m, out int n)
    {
        m = n = 0;
        int musicCount = 0;
        for (int i = 0; i < myObjects.Count; i++)
        {
            //Debug.Log("piece: "+myObjects[i].musicPiece.name);
            double startSec = UtilitiesTm.FloatRemap(myObjects[i].position.x, 0, gameObject.GetComponent<RectTransform>().rect.width, 0, AnimationTimer.GetMaxTime());
            double endSec;
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
                else if (musicCount == 2)
                {
                    n = i;
                }
            }
        }
        return musicCount;
    }
    public void PlayLatestPiece(bool once, bool fade)
    {
        if (GetCurrentMusicCount(out int m, out _) == 1)
        {
            double startSec = UtilitiesTm.FloatRemap(myObjects[m].position.x, 0, gameObject.GetComponent<RectTransform>().rect.width, 0, AnimationTimer.GetMaxTime());
            tmpTime = AnimationTimer.GetTime();

            anyInstanceIsPlaying = true;

            if (playingMusic == false || once == true || currentClip != ((int)char.GetNumericValue(myObjects[m].musicPiece.name[07]) - 1))
            {
                playingMusic = true;
                audioSource.time = tmpTime - (float)startSec;
                currentClip = (int)char.GetNumericValue(myObjects[m].musicPiece.name[07]) - 1; // object index
                audioSource.clip = clip[currentClip];
                audioSource.Play();
            }
            if (SceneManaging.updateMusic)  // scrubbing
            {
                playingMusic = false;
            }
            firstTimeSecond = true;
            audioSource.volume = 1.0f;
        }
        else if (GetCurrentMusicCount(out m, out int n) == 2)
        {
            double startSec = UtilitiesTm.FloatRemap(myObjects[n].position.x, 0, gameObject.GetComponent<RectTransform>().rect.width, 0, AnimationTimer.GetMaxTime());
            float tmpTime = AnimationTimer.GetTime();
            if (firstTimeSecond && playingMusic) // es switcht von 1 auf 2 musikstuecke gleichzeitig
            {
                firstTimeSecond = false;
                fading = false;
                if (fade)
                {
                    Debug.Log("fade out 1");
                    StartCoroutine(FadeOut(1, myObjects[n].musicPiece, tmpTime - (float)startSec, (int)char.GetNumericValue(myObjects[n].musicPiece.name[07]) - 1, true));
                }
                else
                {
                    Debug.Log("ohne fadeout 1");
                    currentClip = (int)char.GetNumericValue(myObjects[n].musicPiece.name[07]) - 1; // object index
                    audioSource.clip = clip[currentClip];
                    audioSource.time = tmpTime - (float)startSec;
                    audioSource.volume = 1.0f;
                    newPiece=true;
                    audioSource.Play();
                }
            }
            else if (newPiece && currentClip != ((int)char.GetNumericValue(myObjects[n].musicPiece.name[07]) - 1))
            {
                newPiece = false;
                fading = false;
                if (fade)
                {
                    Debug.Log("fade out 2");
                    StartCoroutine(FadeOut(1, myObjects[n].musicPiece, tmpTime - (float)startSec, (int)char.GetNumericValue(myObjects[n].musicPiece.name[07]) - 1, true));
                }
                else
                {
                    Debug.Log("ohne fadeout 2");
                    currentClip = (int)char.GetNumericValue(myObjects[n].musicPiece.name[07]) - 1; // object index
                    audioSource.clip = clip[currentClip];
                    audioSource.time = tmpTime - (float)startSec;
                    audioSource.volume = 1.0f;
                    audioSource.Play();
                    newPiece=true;
                }
            }

            anyInstanceIsPlaying = true;

            if (!playingMusic || once)
            {
                playingMusic = true;
                audioSource.time = tmpTime - (float)startSec;
                audioSource.Play();
            }
            if (SceneManaging.updateMusic)
            {
                playingMusic = false;
            }
        }
        else
        {
            if (onlyPiecefinished && playingMusic)
            {
                try
                {
                    StartCoroutine(FadeOut(1, myObjects[m].musicPiece, -1, -1, false));
                    onlyPiecefinished = false;
                }
                catch (ArgumentOutOfRangeException) { }
            }
            firstTimeSecond = false;
            playingMusic = false;
        }
    }
    private bool IsSomethingOverlapping()
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
    private void SetParent(GameObject obj, GameObject parentToSet)
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
    public void ClickSetMusicPieceLength(bool longer)
    {
        int newLength = 0;
        int nr = myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<MusicLength>().nr;
        musicLengthClicked = true;

        if ((longer && myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<MusicLength>().musicLength + 30 <= initialPieceLength[nr]) || (!longer && myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<MusicLength>().musicLength - 30 >= 30))
        {
            if (longer && myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<MusicLength>().musicLength + 30 <= initialPieceLength[nr])
            {
                newLength = 30;
                // myObjects[currentClickedInstanceObjectIndex].musicPiece.GetComponent<RectTransform>().sizeDelta = new Vector2(, );
            }
            else if (!longer && myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<MusicLength>().musicLength - 30 >= 30)
            {
                newLength = -30;
            }

            myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<MusicLength>().musicLength += newLength;

            myObjects[saveIndexForChangePieceLength].position.y = UtilitiesTm.FloatRemap(myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<MusicLength>().musicLength, 0, (float)AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);

            //  = new Vector2(myObjects[saveIndexForChangePieceLength].position.x, myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<RectTransform>().sizeDelta.x + newLength);
            SceneManaging.scaleObject(myObjects[saveIndexForChangePieceLength].musicPiece, myObjects[saveIndexForChangePieceLength].position.y, myObjects[saveIndexForChangePieceLength].musicPiece.GetComponent<RectTransform>().sizeDelta.y, false);
            myObjects = myObjects.OrderBy(w => w.position.x).ToList();
            SceneManaging.CalculateNeighbors(myObjects);
        }
    }
    private void UpdateObjectPosition(MusicPiece obj, Vector2 mousePos)
    {
        obj.musicPiece.transform.position = new Vector3(mousePos.x, mousePos.y, -1.0f);
        obj.position = new Vector2(obj.musicPiece.GetComponent<RectTransform>().anchoredPosition.x, obj.musicPiece.GetComponent<RectTransform>().sizeDelta.x);
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
                SceneManaging.WarningAnimation(colSpaceWarning, spaceWarningBorder);
            }
        }
        else if (_idleTimer >= 3)
        {
            _idleTimer = -1;
            SceneManaging.alpha = 1;
            colSpaceWarning = new Color(1, 0, 0, 1);
            spaceWarning.color = colSpaceWarning;
            spaceWarning.enabled = false;
            spaceWarningBorder.color = colSpaceWarning;
            spaceWarningBorder.enabled = false;
        }
        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            if (!SceneManaging.tutorialActive && !SceneManaging.flyerActive)
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
                        tmpI = i;
                }
                if (tmpI >= 0)
                    editTimelineObject = true;
                else
                    editTimelineObject = false;
                // if you click in slider window
                if (_backgroundBoxCollider == Physics2D.OverlapPoint(getMousePos))
                {
                    firstTimeSecond = true;
                    PlayLatestPiece(true, false);
                }
                //if you click the timeline with the mouse
                if (GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //open or close timeline
                    OpenTimelineByClick(isTimelineOpen, false);
                    musicPieceLengthDialog.SetActive(false);
                    musicLengthClicked = false;
                }

                //if you click on an object in shelf
                else if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
                {
                    //wenn neues objekt genommen wird, soll altes aktuelles objekt unhighlighted werden
                    for (int j = 0; j < myObjects.Count; j++)
                        SceneManaging.highlight(myObjects[j].musicPiece, false);

                    if (!SceneManaging.sceneChanged)
                        SceneManaging.sceneChanged = true;

                    diff = new Vector2(getMousePos.x - figureObjects[currentClickedObjectIndex].transform.position.x, getMousePos.y - figureObjects[currentClickedObjectIndex].transform.position.y);

                    if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                        draggingObject = true;
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
                            _toBeRemovedFromTimeline = true;
                            //Debug.Log("delete");
                        }
                        diff = new Vector2(getMousePos.x - myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.position.x, getMousePos.y - myObjects[currentClickedInstanceObjectIndex].musicPiece.transform.position.y);

                        //set up some flags
                        draggingObject = true;
                        draggingOnTimeline = true;

                        //highlighting objects and showing delete button when clicked
                        if (SceneManaging.highlighted == false)
                            SceneManaging.highlight(myObjects[currentClickedInstanceObjectIndex].musicPiece, true);
                        else
                        {
                            for (int i = 0; i < myObjects.Count; i++)
                                SceneManaging.highlight(myObjects[i].musicPiece, false);
                            SceneManaging.highlight(myObjects[currentClickedInstanceObjectIndex].musicPiece, true);
                        }
                    }
                }

                //if you hit/clicked nothing with mouse
                else if (Physics2D.OverlapPoint(getMousePos) == false)
                {
                    musicPieceLengthDialog.SetActive(false);
                    musicLengthClicked = false;
                    for (int i = 0; i < myObjects.Count; i++)
                        SceneManaging.highlight(myObjects[i].musicPiece, false);
                }
            }
        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline && isTimelineOpen)
        {
            if (currentClickedInstanceObjectIndex != (-1))  //is set in the identify-methods
            {
                MusicPiece currentObj = myObjects[currentClickedInstanceObjectIndex];
                RectTransform currentRect = currentObj.musicPiece.GetComponent<RectTransform>();
                isInstance = true;

                // abfrage, ob reihenfolge sich geaendert hat
                if (currentName != currentObj.objName)
                {
                    for (int i = 0; i < myObjects.Count; i++)
                    {
                        if (myObjects[i].objName == currentName)
                        {
                            currentClickedInstanceObjectIndex = i;
                        }
                    }
                }

                //if you click an object in timeline (for dragging) //move object
                UpdateObjectPosition(currentObj, getMousePos - diff);
                currentObj.musicPiece.transform.position = new Vector2(currentObj.musicPiece.transform.position.x, transform.position.y); //snapping/lock y-axis

                #region limits front and back of rail
                // limit front of rail
                if (currentRect.anchoredPosition.x < 0)
                {
                    currentRect.anchoredPosition = new Vector2(0, currentRect.anchoredPosition.y);
                    currentObj.position = new Vector2(0, currentObj.position.y);
                }
                // limit back of rail
                else if ((currentRect.anchoredPosition.x + currentRect.sizeDelta.x) > railWidthAbsolute)
                {
                    currentRect.anchoredPosition = new Vector2(railWidthAbsolute - currentRect.sizeDelta.x, currentRect.anchoredPosition.y);
                    currentObj.position = new Vector2(railWidthAbsolute - currentRect.sizeDelta.x, currentObj.position.y);
                }

                #endregion
                #region Stapeln von Ebenen
                // if there is only one layer on Rail
                if (sizeLayering == 1)
                {
                    if (IsCurrentFigureOverlapping(currentObj, "", out countOverlaps, myObjects) != -1)
                    {
                        SceneManaging.scaleToLayerSize(currentObj.musicPiece, 2, gameObject, currentObj.position.y, false);
                        sizeLayering = 2;
                        currentObj.layer = 2;

                        // scale all other timelineobjects of layer 1
                        for (int j = 0; j < myObjects.Count; j++)
                        {
                            if (myObjects[j].musicPiece != currentObj.musicPiece && myObjects[j].layer == 1)
                            {
                                SceneManaging.scaleToLayerSize(myObjects[j].musicPiece, 1, gameObject, myObjects[j].position.y, false);
                                myObjects[j].layer = 1;
                            }
                        }

                    }
                }

                // if there are two layers on Rail
                else
                {
                    // wenn nichts mehr überlappt
                    if (!IsSomethingOverlapping())
                    {
                        sizeLayering = 1;
                        // scale all other timelineobjects of layer 0
                        for (int j = 0; j < myObjects.Count; j++)
                        {
                            SceneManaging.scaleToLayerSize(myObjects[j].musicPiece, 0, gameObject, myObjects[j].position.y, false);
                            myObjects[j].layer = 1;
                        }
                    }
                    else
                    {
                        FindFreeSpot(currentObj);
                    }
                }
                #endregion

            }
            releaseOnTimeline = true;
        }
        // dragging an object from shelf to timeline
        if (draggingObject && editTimelineObject == false && isInstance == false)
        {
            //move object
            figureObjects[currentClickedObjectIndex].transform.position = new Vector3(getMousePos.x - diff.x, getMousePos.y - diff.y, -1.0f);

            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            bool hitTimeline = false;
            hitTimeline = CheckHittingTimeline(getMousePos);

            if (hitTimeline)
            {
                // change parent back and snap
                SetParent(figureObjects[currentClickedObjectIndex], gameObject);

                // snapping/lock y-axis
                figureObjects[currentClickedObjectIndex].transform.position = new Vector2(getMousePos.x - (50 / screenDifference.x), transform.position.y);

                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(100, timelineImage.GetComponent<RectTransform>().sizeDelta.y);

                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }
            else
            {
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(objectShelfSize.x, objectShelfSize.y);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(objectShelfSize.x, objectShelfSize.y);
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
                    countName = CountCopiesOfObject(figureObjects[currentClickedObjectIndex], myObjects);
                    newCopyOfFigure.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");
                    RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();
                    newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x + 50, figureObjects[currentClickedObjectIndex].transform.position.y, -1);
                    float tmpPos = tmpRectTransform.anchoredPosition.x;

                    // in der Besucherversion auf 60 sek kuerzen, weil gesamtes stueck nur 3 min lang ist
                    float tmpLength;
                    if (!SceneManaging.isExpert)
                        tmpLength = UtilitiesTm.FloatRemap(60, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
                    else
                        tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);

                    CreateRectangle(newCopyOfFigure, tmpLength, gameObject.GetComponent<RectTransform>().rect.height);
                    figCounterCircle[currentClickedObjectIndex].text = (countName + 1).ToString();

                    //parent and position
                    newCopyOfFigure.transform.SetParent(gameObject.transform);
                    newCopyOfFigure.transform.localScale = Vector3.one;
                    tmpRectTransform.anchoredPosition = new Vector2(tmpPos, tmpRectTransform.anchoredPosition.y);

                    #region limit front and end of rail
                    //------------------------------------------limit front of rail---------------------------------------------//
                    if (tmpRectTransform.anchoredPosition.x < 0)
                        tmpRectTransform.anchoredPosition = new Vector2(0, tmpRectTransform.anchoredPosition.y);
                    //------------------------------------------limit back of rail---------------------------------------------//
                    else if ((tmpRectTransform.anchoredPosition.x + tmpLength) > railWidthAbsolute)
                        tmpRectTransform.anchoredPosition = new Vector2(railWidthAbsolute - tmpLength, tmpRectTransform.anchoredPosition.y);
                    #endregion

                    // size of rectangle becomes size for figure that is clickable (bring in front)
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);
                    newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y);

                    //set original image back to shelf, position 2 to make it visible
                    figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                    figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);

                    #region calculating Layer Overlap
                    MusicPiece oP = new MusicPiece
                    {
                        objName = newCopyOfFigure.name,
                        position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpLength),
                        musicPiece = newCopyOfFigure,
                        neighborLeft = -1,
                        neighborRight = -1
                    };
                    myObjects.Add(oP);

                    SceneManaging.scaleToLayerSize(newCopyOfFigure, 0, gameObject, tmpLength, false);
                    oP.layer = 1;
                    if (sizeLayering == 1)
                    {
                        if (IsCurrentFigureOverlapping(oP, "", out countOverlaps, myObjects) != -1)
                        {
                            sizeLayering = 2;
                            SceneManaging.scaleToLayerSize(newCopyOfFigure, 2, gameObject, tmpLength, false);
                            oP.layer = 2;
                            // others to 1
                            for (int i = 0; i < myObjects.Count; i++)
                            {
                                if (myObjects[i].layer == 1)
                                    SceneManaging.scaleToLayerSize(myObjects[i].musicPiece, 1, gameObject, myObjects[i].position.y, false);
                            }
                        }
                    }
                    else
                    {
                        CreateListWithoutCurrentFigure(oP); //listWithoutCurrentFigure = railList[currentRailIndex].myObjects;
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
                    MusicClipElementInstance musicClipElementInstance = new MusicClipElementInstance
                    {
                        instanceNr = countName,
                        name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000")
                    };
                    StaticSceneData.StaticData.musicClipElements[currentClickedObjectIndex].musicClipElementInstances.Add(musicClipElementInstance);

                    SceneManaging.highlight(myObjects[currentPosInList].musicPiece, true);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (_toBeRemoved)
                    {
                        //Debug.Log("remove: " + myObjects[currentPosInList].objName);
                        RemoveObjectFromTimeline(myObjects[currentPosInList]);
                    }
                    if (SceneManaging.isExpert)
                    {
                        if (!musicLengthClicked)
                        {
                            musicPieceLengthDialog.SetActive(true);
                            musicPieceLengthDialog.transform.position = new Vector2(getMousePos.x, musicPieceLengthDialog.transform.position.y);
                            musicPieceLengthDialog.transform.SetAsLastSibling();
                            saveIndexForChangePieceLength = currentPosInList;
                        }

                        // set index back to -1 because nothing is being clicked anymore
                        currentClickedObjectIndex = -1;
                    }
                }

                else
                {
                    if (SceneManaging.isExpert)
                    {
                        if (!musicLengthClicked)
                        {
                            musicPieceLengthDialog.SetActive(true);
                            musicPieceLengthDialog.transform.position = new Vector2(getMousePos.x, musicPieceLengthDialog.transform.position.y);
                            musicPieceLengthDialog.transform.SetAsLastSibling();
                            saveIndexForChangePieceLength = currentClickedInstanceObjectIndex;
                        }
                    }

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

                    // click on delete button
                    if (_toBeRemovedFromTimeline)
                    {
                        for (int i = 0; i < myObjects.Count; i++)
                        {
                            if (myObjects[i].objName == currentName)
                            {
                                //Debug.Log("delete : "+myObjects[i].objName);
                                RemoveObjectFromTimeline(myObjects[i]);
                            }
                        }
                    }
                }

            }
            // if dropped somewhere else than timeline: bring figure back to shelf
            else if (releaseOnTimeline == false && currentClickedObjectIndex != -1 && isInstance == false)
            {
                SetParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                // bring object back to position two, so that its visible and change position back to default
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -73.0f);
            }
            else if (releaseOnTimeline == false && currentClickedInstanceObjectIndex != -1 && isInstance)
            {
                RemoveObjectFromTimeline(myObjects[currentClickedInstanceObjectIndex]);
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //Save musictitle.moment to SceneData:
            for (int i = 0; i < myObjects.Count; i++)
            {
                int musObjIndex = int.Parse(myObjects[i].objName.Substring(6, 2)) - 1;
                int musInsIndex = int.Parse(myObjects[i].objName.Substring(17));
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
            _toBeRemovedFromTimeline = false;
            musicLengthClicked = false;

        }

        // turning music on and off in playmode
        if (SceneManaging.playing)
        {
            PlayLatestPiece(false, false);
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
                    audioSource.Stop();
                    sampleImages[sampleButtonPressed].color = new Color(1, 1, 1, 0); // = false;
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