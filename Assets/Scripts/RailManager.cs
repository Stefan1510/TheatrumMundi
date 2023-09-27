using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

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
    [HideInInspector] public int currentRailIndex;
    public GameObject objectLibrary, UICanvas, parentMenue; // mainMenue
    #endregion
    #region private variables
    [SerializeField] GameObject scrollRect;
    [SerializeField] GameObject prefabRect;
    [SerializeField] TextMeshProUGUI spaceWarning;
    [SerializeField] RailMusicManager tmpRailMusicMan;
    [SerializeField] Image spaceWarningBorder;
    Vector2 objectShelfSize;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    float objectAnimationLength, rectSize;
    float figPictureSize = 100.0f;
    float heightRailPercent;
    Vector2 releaseObjMousePos, screenDifference;
    double minX, maxX;
    float railWidth, railwidthAbsolute;
    Color colFigure = new Color(0.06f, 0.66f, .74f, 0.5f);
    Color colFigureHighlighted = new Color(0f, 0.87f, 1.0f, 0.5f);
    Color colSpaceWarning;
    private GameObject[] figureObjects, figureObjects3D;
    private int currentClickedObjectIndex, currentClickedInstanceObjectIndex, hitTimeline = -1, flyerHit = -1;
    private int maxTimeInSec, currentPosInList;
    private float currentLossyScale;
    private float _timerFigure = 1;
    int currentSpaceActive = -1;
    private Vector2 diff;
    private float heightOpened, heightClosed, _spaceMax, _spaceMax3D;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, isInstance, _toBeRemoved, _toBeRemovedFromTimeline;
    int hitTimelineOld;
    private string currentName;
    private List<Figure> listWithoutCurrentFigure = new List<Figure>();
    private int countOverlaps;
    private float _idleTimer = -1;
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
            railList[i] = new Rail
            {
                sizeLayering = 1,
                isTimelineOpen = false
            };
        }
        currentRailIndex = 0;

        //load all objects given in the figuresShelf
        figureObjects = new GameObject[objectLibrary.transform.childCount];
        figureObjects3D = new GameObject[figureObjects.Length];

        objectShelfParent = new GameObject[figureObjects.Length];

        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject;

            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
        }
        for (int i = 0; i < figCounterCircle.Length; i++)
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
        _spaceMax = 0.1f * _spaceMax3D * 100 / maxTimeInSec * railwidthAbsolute;      // *100 fuer umrechnung m in cm, 0.1 fuer speed
        AnimationTimer.SetTime(0);
    }
    public void CalculateFigures(float timeAfter)
    {
        // from draw curve
        if (timeAfter == -1)
        {
            for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
            {
                RectTransform tmpRectTransform = railList[currentRailIndex].myObjects[j].figure.GetComponent<RectTransform>();
                float moment = UtilitiesTm.FloatRemap(tmpRectTransform.anchoredPosition.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
                rectSize = objectAnimationLength / AnimationTimer.GetMaxTime() * railwidthAbsolute;
                tmpRectTransform.sizeDelta = new Vector2(rectSize, tmpRectTransform.sizeDelta.y);
                railList[currentRailIndex].myObjects[j].position = new Vector2(railList[currentRailIndex].myObjects[j].position.x, rectSize);
                railList[currentRailIndex].myObjects[j].figure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, tmpRectTransform.sizeDelta.y);
                railList[currentRailIndex].myObjects[j].figure.GetComponent<BoxCollider2D>().size = tmpRectTransform.sizeDelta;
            }
        }
        else
        {
            // from ChangeMaxTime
            for (int i = 0; i < railList.Length; i++)
            {
                for (int j = 0; j < railList[i].myObjects.Count; j++)
                {
                    RectTransform tmpRectTransform = railList[i].myObjects[j].figure.GetComponent<RectTransform>();
                    float moment = UtilitiesTm.FloatRemap(tmpRectTransform.anchoredPosition.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                    objectAnimationLength = rails3D[i].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
                    rectSize = objectAnimationLength / (timeAfter * 60) * railwidthAbsolute;
                    float posX = UtilitiesTm.FloatRemap(moment, 0, timeAfter * 60, 0, railwidthAbsolute);

                    tmpRectTransform.anchoredPosition = new Vector2(posX, tmpRectTransform.anchoredPosition.y);
                    railList[i].myObjects[j].position = new Vector2(posX, rectSize);
                    tmpRectTransform.sizeDelta = new Vector2(rectSize, tmpRectTransform.sizeDelta.y);
                    railList[i].myObjects[j].figure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, tmpRectTransform.sizeDelta.y);
                    railList[i].myObjects[j].figure.GetComponent<BoxCollider2D>().size = tmpRectTransform.sizeDelta;
                }
                SceneManaging.CalculateNeighbors(railList[i].myObjects);
            }
        }
    }
    public int CountCopiesOfObject(string fig)
    {
        int c = 0;
        for (int i = 0; i < rails.Length; i++)
        {
            //count object with the same name as fig
            foreach (Figure oP in railList[i].myObjects)
            {
                if (oP.objName.Contains(fig))
                    c++;
            }
        }
        return c;
    }
    private int CheckHittingFlyerSpace(Vector2 mousePos)
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
    public int CheckHittingTimeline(Vector2 mousePos)
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
    private void FindFreeSpot(int railIndex, Figure fig)
    {
        // find out index in the middle
        int middleIdx;
        RectTransform rectTransform = fig.figure.GetComponent<RectTransform>();

        if (listWithoutCurrentFigure.Count % 2 == 1)
            middleIdx = (listWithoutCurrentFigure.Count - 1) / 2;
        else
            middleIdx = listWithoutCurrentFigure.Count / 2;

        // maus ist LINKS vom bereich der figuren
        if (Input.mousePosition.x < listWithoutCurrentFigure[middleIdx].figure.transform.position.x + (listWithoutCurrentFigure[middleIdx].position.y / 2) - 100)
        {
            int idx = IsCurrentFigureOverlapping(fig, "left", out countOverlaps, listWithoutCurrentFigure);
            if (countOverlaps == 1)
            {
                // overlapping obj is layer 1, obj needs to be layer 2
                if (listWithoutCurrentFigure[idx].layer == 1)
                {
                    fig.layer = 2;
                    SceneManaging.scaleToLayerSize(fig.figure, 2, rails[currentRailIndex], rectSize, false);
                }
                // overlapping obj is layer 2, obj needs to be layer 1
                else
                {
                    fig.layer = 1;
                    SceneManaging.scaleToLayerSize(fig.figure, 1, rails[currentRailIndex], rectSize, false);
                }
            }
            else
            {
                if (idx != -1)
                {
                    // gibt es ein idx + 1 mit unterschiedlicher Layer?
                    if (idx < listWithoutCurrentFigure.Count - 1 && listWithoutCurrentFigure[idx + 1].layer != listWithoutCurrentFigure[idx].layer
                    && listWithoutCurrentFigure[idx + 1].objName != fig.objName)
                    {
                        // hat es einen linken nachbarn? 
                        LoopLeft(railIndex, idx + 1, fig, false);
                    }
                    else
                    {
                        LoopLeft(railIndex, idx, fig, false);
                    }
                }
                // nichts ueberlappt
                else
                {
                    SceneManaging.scaleToLayerSize(fig.figure, 1, rails[currentRailIndex], rectSize, false);
                    fig.layer = 1;
                }
            }
        }
        // maus ist RECHTS vom bereich der figuren
        else
        {
            int idx = IsCurrentFigureOverlapping(fig, "right", out countOverlaps, listWithoutCurrentFigure);
            if (countOverlaps == 1)
            {
                // overlapping obj is layer 1, obj needs to be layer 2
                if (listWithoutCurrentFigure[idx].layer == 1)
                {
                    fig.layer = 2;
                    SceneManaging.scaleToLayerSize(fig.figure, 2, rails[currentRailIndex], rectSize, false);
                }
                // overlapping obj is layer 2, obj needs to be layer 1
                else
                {
                    fig.layer = 1;
                    SceneManaging.scaleToLayerSize(fig.figure, 1, rails[currentRailIndex], rectSize, false);
                }
            }
            else
            {
                if (idx != -1)
                {
                    // gibt es ein idx - 1 mit unterschiedlicher Layer?
                    if (idx > 0 && listWithoutCurrentFigure[idx - 1].layer != listWithoutCurrentFigure[idx].layer)
                    {
                        // hat es einen rechten nachbarn? 
                        LoopRight(railIndex, idx - 1, fig, false);
                    }
                    else
                    {
                        LoopRight(railIndex, idx, fig, false);
                    }
                }
                else
                {
                    SceneManaging.scaleToLayerSize(fig.figure, 1, rails[currentRailIndex], rectSize, false);
                    fig.layer = 1;
                }
            }
        }

        fig.position = new Vector2(rectTransform.anchoredPosition.x, rectSize);
        rectTransform.sizeDelta = new Vector2(rectSize, rectTransform.sizeDelta.y);
    }
    private int IsCurrentFigureOverlapping(Figure obj, string dir, out int count, List<Figure> figureList)
    {
        int val = -1;
        int val2 = -1;
        int val3 = -1;

        count = 0;

        if (dir == "right")
        {
            for (int i = 0; i < figureList.Count; i++)
            {
                if (((figureList[i].position.x <= obj.position.x
                && figureList[i].position.x + figureList[i].position.y >= obj.position.x)
                || (figureList[i].position.x >= obj.position.x
                && figureList[i].position.x <= obj.position.x + obj.position.y))
                && figureList[i].objName != obj.objName)
                {
                    if (val2 != -1)
                        val3 = val2;
                    if (val == -1)
                        val = i;
                    val2 = i;

                    count++;
                }
                if (count > 1)
                {
                    if (figureList[val2].layer == figureList[val3].layer)
                        count = 1;
                    else
                        break;
                }
            }
        }
        else
        {
            for (int i = figureList.Count - 1; i >= 0; i--)
            {
                if (((figureList[i].position.x <= obj.position.x
                && figureList[i].position.x + figureList[i].position.y >= obj.position.x)
                || (figureList[i].position.x >= obj.position.x
                && figureList[i].position.x <= obj.position.x + obj.position.y))
                && figureList[i].objName != obj.objName)
                {
                    if (val2 != -1)
                        val3 = val2;
                    if (val == -1)
                        val = i;
                    val2 = i;

                    count++;
                    ////Debug.LogWarning("val2: " + val2 + ", val3: " + val3);
                }
                if (count > 1)
                {
                    if (figureList[val2].layer == figureList[val3].layer)
                    {
                        ////Debug.Log("count: " + count);
                        count = 1;
                    }
                    else
                    {
                        ////Debug.Log("es gibt mehr als eine überlappung auf unterschiedlichen Ebenen");
                        break;
                    }
                }
            }
        }

        return val;
    }
    private bool IsRailAreaHit(Vector2 mousePos)
    {
        bool val = false;
        if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= Screen.height * 0.36f && mousePos.y > 0)
            val = true;
        return val;
    }
    private bool IsEnoughSpace(Figure currentObj, float minimumDistance)
    {
        bool enoughSpace = true;
        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
        {
            if (railList[currentRailIndex].myObjects[i].position.x - minimumDistance < currentObj.position.x && railList[currentRailIndex].myObjects[i].position.x > currentObj.position.x)
            {
                Debug.Log("da ist was links");
                enoughSpace = false;
            }
            else if (railList[currentRailIndex].myObjects[i].position.x + minimumDistance > currentObj.position.x && railList[currentRailIndex].myObjects[i].position.x < currentObj.position.x)
            {
                Debug.Log("da ist was rechts");
                enoughSpace = false;
            }
        }
        return enoughSpace;
    }
    private void LoopRight(int railIndex, int startIdx, Figure fig, bool fromLeft)
    {
        RectTransform rectTransform = fig.figure.GetComponent<RectTransform>();

        for (int i = startIdx; i < listWithoutCurrentFigure.Count; i++)
        {
            // wenn es einen rechten nachbarn hat (nicht sich selbst!)
            if (listWithoutCurrentFigure[i].neighborRight != -1)
            {
                //Debug.Log("HAT einen rechten nachbarn");
                // wenn zwischen den beiden genug platz ist
                if (listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborRight].position.x + 1 >= listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + rectSize)
                {
                    //Debug.Log("es ist genug platz");
                    // if (holdMinimumDistance(fig, _spaceMax)) ;

                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        //Debug.Log("layer 1");
                        SceneManaging.scaleToLayerSize(fig.figure, 1, rails[railIndex], rectSize, false);
                        fig.layer = 1;
                    }
                    else
                    {
                        //Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(fig.figure, 2, rails[railIndex], rectSize, false);
                        fig.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {
                    //Debug.Log("nicht genug platz. x: " + (listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborRight].position.x) + ", Figur: " + (listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + rectSize));
                    // hier passiert nichts, es muss weiter gesucht werden
                }
            }
            else
            {
                //Debug.Log("platz bis rand: " + (listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + rectSize) + ", rand: " + railwidthAbsolute);

                //Debug.Log("hat KEINEN rechten nachbarn");
                if (listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y + rectSize < railwidthAbsolute)
                {
                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        // Debug.Log("layer 1");
                        SceneManaging.scaleToLayerSize(fig.figure, 1, rails[railIndex], rectSize, false);
                        fig.layer = 1;
                    }
                    else
                    {
                        // Debug.Log("layer 2");
                        SceneManaging.scaleToLayerSize(fig.figure, 2, rails[railIndex], rectSize, false);
                        fig.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x + listWithoutCurrentFigure[i].position.y, rectTransform.anchoredPosition.y);

                    break;
                }
                else
                {
                    if (!fromLeft)
                    {
                        // hier muss links weitergesucht werden
                        // Debug.Log("kein platz bis zum rand, es muss links weitergesucht werden");
                        LoopLeft(currentRailIndex, listWithoutCurrentFigure.Count - 1, fig, true);
                    }
                    else
                    {
                        // Debug.Log("alles voll!");
                        if (currentClickedInstanceObjectIndex != -1)
                        {
                            // Debug.Log("in der timeline");
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
                            //    Debug.Log("vom shelf");
                            spaceWarning.transform.position = new Vector2(spaceWarning.transform.position.x, rails[railIndex].transform.position.y);
                            spaceWarningBorder.transform.position = new Vector2(spaceWarning.transform.position.x, rails[railIndex].transform.position.y);
                            _toBeRemoved = true;
                            spaceWarning.enabled = true;
                            spaceWarningBorder.enabled = true;
                            _idleTimer = 0;
                        }
                    }
                    break;
                }
            }
        }
    }
    private void LoopLeft(int railIndex, int startIdx, Figure fig, bool fromRight)
    {
        RectTransform rectTransform = fig.figure.GetComponent<RectTransform>();

        for (int i = startIdx; i >= 0; i--)
        {
            // wenn es einen linken nachbarn hat
            if (listWithoutCurrentFigure[i].neighborLeft != -1)
            {
                // wenn zwischen den beiden genug platz ist
                if (listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborLeft].position.x + listWithoutCurrentFigure[listWithoutCurrentFigure[i].neighborLeft].position.y + rectSize <= listWithoutCurrentFigure[i].position.x + 1)
                {
                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        SceneManaging.scaleToLayerSize(fig.figure, 1, rails[railIndex], rectSize, false);
                        fig.layer = 1;
                    }
                    else
                    {
                        SceneManaging.scaleToLayerSize(fig.figure, 2, rails[railIndex], rectSize, false);
                        fig.layer = 2;
                    }

                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x - rectSize, rectTransform.anchoredPosition.y);
                    break;
                }
                else
                {
                    // hier passiert nichts, es muss weiter gesucht werden
                }
            }
            else
            {
                if (listWithoutCurrentFigure[i].position.x - rectSize > 0)
                {
                    if (listWithoutCurrentFigure[i].layer == 1)
                    {
                        SceneManaging.scaleToLayerSize(fig.figure, 1, rails[railIndex], rectSize, false);
                        fig.layer = 1;
                    }
                    else
                    {
                        SceneManaging.scaleToLayerSize(fig.figure, 2, rails[railIndex], rectSize, false);
                        fig.layer = 2;
                    }
                    rectTransform.anchoredPosition = new Vector2(listWithoutCurrentFigure[i].position.x - rectSize, rectTransform.anchoredPosition.y);
                    break;
                }
                else
                {
                    // hier muss rechts weitergesucht werden
                    if (!fromRight)
                    {
                        LoopRight(currentRailIndex, 0, fig, true);
                    }
                    else
                    {
                        // ins shelf zurueck!
                        if (currentClickedInstanceObjectIndex != -1)
                        {
                        }
                        // vom shelf
                        else
                        {
                            // Debug.Log("vom shelf");
                            spaceWarning.transform.position = new Vector2(spaceWarning.transform.position.x, rails[currentRailIndex].transform.position.y);
                            spaceWarningBorder.transform.position = new Vector2(spaceWarning.transform.position.x, rails[railIndex].transform.position.y);
                            _toBeRemoved = true;
                            spaceWarning.enabled = true;
                            spaceWarningBorder.enabled = true;
                            _idleTimer = 0;
                        }
                    }
                    break;
                }
            }
        }
    }
    public void OpenTimelineByClick(bool thisTimelineOpen, int index, bool fromShelf)
    {
        if (fromShelf == false && SceneManaging.mainMenuActive == 2 && SceneManaging.directorMenueActive != 1)
        {
            UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf05(true);
        }
        if (thisTimelineOpen)
        {
            // if timeline is already open, unhighlight all
            for (int i = 0; i < railList[index].myObjects.Count; i++)
                SceneManaging.highlight(railList[index].myObjects[i].figure3D, railList[index].myObjects[i].figure, false, "figure");
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
                    SceneManaging.closeRail(rails[i].gameObject, railwidthAbsolute);
                    railList[i].isTimelineOpen = false;
                    OpenCloseObjectInTimeline(railList[i].myObjects, i, true);

                    for (int j = 0; j < railList[i].myObjects.Count; j++)
                    {
                        SceneManaging.highlight(railList[i].myObjects[j].figure3D, railList[i].myObjects[j].figure, false, "figure");
                        railList[i].myObjects[j].figure.GetComponent<BoxCollider2D>().enabled = false;
                    }
                }
            }
            // wenn bg oder licht schiene offen sind
            for (int j = 0; j < 2; j++)
            {
                if (tmpUIController.RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen)
                {
                    SceneManaging.closeRail(tmpUIController.RailLightBG[j].gameObject, railwidthAbsolute);
                    tmpUIController.RailLightBG[j].GetComponent<RailLightManager>().isTimelineOpen = false;
                }
            }
            // wenn music-rail offen ist
            if (tmpRailMusicMan.isTimelineOpen)
            {
                SceneManaging.closeRail(tmpUIController.RailMusic.gameObject, railwidthAbsolute);
                tmpRailMusicMan.OpenCloseObjectInTimeline(false);
                tmpRailMusicMan.isTimelineOpen = false;
                tmpRailMusicMan.musicPieceLengthDialog.SetActive(false);

                for (int k = 0; k < tmpRailMusicMan.myObjects.Count; k++)
                {
                    SceneManaging.highlight(tmpRailMusicMan.myObjects[k].musicPiece, false);
                }
            }

            // open clicked rail and collider
            rails[index].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 80);
            rails[index].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 80);
            OpenCloseObjectInTimeline(railList[index].myObjects, index, false);
            railList[index].isTimelineOpen = true;

            ImageTimelineSelection.SetRailNumber((int)Char.GetNumericValue(rails[index].name[17]) - 1);
            ImageTimelineSelection.SetRailType(0);  // for rail-rails
        }
    }
    public void OpenCloseObjectInTimeline(List<Figure> objects, int railIndex, bool boxCollZero)
    {
        float length = rectSize;
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].figure.GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].figure.GetComponent<RectTransform>().anchoredPosition.x, -rails[railIndex].GetComponent<RectTransform>().rect.height / 2, -1);

            if (railList[railIndex].sizeLayering == 1)
            {
                SceneManaging.scaleToLayerSize(objects[i].figure, 0, rails[railIndex], objects[i].position.y, boxCollZero);
            }
            else if (railList[railIndex].sizeLayering == 2)
            {
                if (objects[i].layer == 1)
                {
                    SceneManaging.scaleToLayerSize(objects[i].figure, 1, rails[railIndex], objects[i].position.y, boxCollZero);
                    objects[i].figure.GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].figure.GetComponent<BoxCollider2D>().offset.x, rails[railIndex].GetComponent<RectTransform>().rect.height / 4);
                }
                else
                {
                    SceneManaging.scaleToLayerSize(objects[i].figure, 2, rails[railIndex], objects[i].position.y, boxCollZero);
                    objects[i].figure.GetComponent<BoxCollider2D>().offset = new Vector2(objects[i].figure.GetComponent<BoxCollider2D>().offset.x, -rails[railIndex].GetComponent<RectTransform>().rect.height / 4);
                }
            }
        }
    }
    public void OpenTimelineByDrag(int index)
    {
        if (!railList[index].isTimelineOpen)
        {
            UIController tmpUIController = gameController.GetComponent<UIController>();
            if (tmpUIController.RailLightBG[0].isTimelineOpen)
            {
                SceneManaging.closeRail(tmpUIController.RailLightBG[0].gameObject, railwidthAbsolute);
                tmpUIController.RailLightBG[0].GetComponent<RailLightManager>().isTimelineOpen = false;
            }
            else if (tmpUIController.RailLightBG[1].isTimelineOpen)
            {
                SceneManaging.closeRail(tmpUIController.RailLightBG[1].gameObject, railwidthAbsolute);
                tmpUIController.RailLightBG[1].GetComponent<RailLightManager>().isTimelineOpen = false;
            }
            else if (tmpUIController.RailMusic.isTimelineOpen)
            {
                SceneManaging.closeRail(tmpUIController.RailMusic.gameObject, railwidthAbsolute);
                tmpUIController.RailMusic.GetComponent<RailMusicManager>().isTimelineOpen = false;
                tmpUIController.RailMusic.GetComponent<RailMusicManager>().OpenCloseObjectInTimeline(false);
            }
            for (int i = 0; i < rails.Length; i++)  // alle schienen schliessen
            {
                if (railList[i].isTimelineOpen)
                {
                    SceneManaging.closeRail(rails[i].gameObject, railwidthAbsolute);
                    railList[i].isTimelineOpen = false;
                    OpenCloseObjectInTimeline(railList[i].myObjects, i, true);

                    for (int j = 0; j < railList[i].myObjects.Count; j++)
                    {
                        SceneManaging.highlight(railList[i].myObjects[j].figure3D, railList[i].myObjects[j].figure, false, "figure");
                    }
                }
            }
            if (currentRailIndex != -1)
            {
                railList[currentRailIndex].isTimelineOpen = false;
                //scale down timeline, collider, scale up objects on timeline
                rails[currentRailIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[currentRailIndex].GetComponent<RectTransform>().rect.width, 20);
                rails[currentRailIndex].GetComponent<BoxCollider2D>().size = new Vector2(rails[currentRailIndex].GetComponent<BoxCollider2D>().size.x, 20);
            }
            //scale up hovered timeline
            rails[index].GetComponent<RectTransform>().sizeDelta = new Vector2(rails[index].GetComponent<RectTransform>().rect.width, 80);
            rails[index].GetComponent<BoxCollider2D>().size = new Vector2(rails[index].GetComponent<BoxCollider2D>().size.x, 80);
            railList[index].isTimelineOpen = true;

            OpenCloseObjectInTimeline(railList[index].myObjects, index, false);
        }
    }
    public void UpdateObjectPosition(Figure obj, Vector2 mousePos)
    {
        obj.figure.transform.position = new Vector3(mousePos.x, mousePos.y, -1.0f);
        obj.position = new Vector2(obj.figure.GetComponent<RectTransform>().anchoredPosition.x, obj.figure.GetComponent<RectTransform>().sizeDelta.x);
    }
    public void RemoveObjectFromTimeline(Figure obj)
    {
        int tmpNr = int.Parse(obj.figure.transform.GetChild(1).name.Substring(12));
        int currentCounterNr = int.Parse(figCounterCircle[tmpNr - 1].text);

        // erase from layer list
        railList[currentRailIndex].myObjects.Remove(obj);

        figCounterCircle[tmpNr - 1].text = (currentCounterNr - 1).ToString();

        // figure instance element rauskriegen (beim loeschen veraendert sich der platz im array, aber die endung ist noch 001 bspw.)
        for (int j = 0; j < StaticSceneData.StaticData.figureElements[Int32.Parse(obj.figure.name.Substring(6, 2)) - 1].figureInstanceElements.Count; j++)
        {
            if (StaticSceneData.StaticData.figureElements[Int32.Parse(obj.figure.name.Substring(6, 2)) - 1].figureInstanceElements[j].instanceNr == int.Parse(obj.figure.name.Substring(16)))
            {
                StaticSceneData.StaticData.figureElements[Int32.Parse(obj.figure.name.Substring(6, 2)) - 1].figureInstanceElements.Remove(StaticSceneData.StaticData.figureElements[Int32.Parse(obj.figure.name.Substring(6, 2)) - 1].figureInstanceElements[j]);
            }
        }

        Destroy(obj.figure);
        Destroy(obj.figure3D);

        if (!IsSomethingOverlapping(currentRailIndex))
        {
            railList[currentRailIndex].sizeLayering = 1;
            for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
            {
                SceneManaging.scaleToLayerSize(railList[currentRailIndex].myObjects[i].figure, 0, rails[currentRailIndex], railList[currentRailIndex].myObjects[i].position.y, false);
                railList[currentRailIndex].myObjects[i].layer = 1;
            }
        }
        railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(x => x.position.x).ToList();
        SceneManaging.CalculateNeighbors(railList[currentRailIndex].myObjects);

    }
    public GameObject CreateNew2DInstance(int figureNr, float momentOrPosX, int loadFromFile, int savedLayer, bool saveToSceneData)   // if layer = -1, then new instance, else layer has been saved
    {
        GameObject curr3DObject;
        if (loadFromFile != -1)
            currentRailIndex = loadFromFile;

        int countName = 0;
        countName = CountCopiesOfObject(figureObjects[figureNr].name);

        if (gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
            figCounterCircle[figureNr].text = (countName + 1).ToString();

        newCopyOfFigure = Instantiate(figureObjects[figureNr]);
        newCopyOfFigure.name = figureObjects[figureNr].name + "instance" + countName.ToString("000");
        RectTransform tmpRectTransform = newCopyOfFigure.GetComponent<RectTransform>();

        //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
        newCopyOfFigure.transform.position = new Vector2(momentOrPosX, newCopyOfFigure.transform.position.y);
        newCopyOfFigure.transform.SetParent(rails[currentRailIndex].transform);
        newCopyOfFigure.transform.localScale = Vector3.one;
        Figure oP = new Figure
        {
            objName = newCopyOfFigure.name,
            position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x),
            figure = newCopyOfFigure,
            neighborLeft = -1,
            neighborRight = -1
        };

        railList[currentRailIndex].myObjects.Add(oP);

        // from scenedataController
        if (loadFromFile != -1)
        {
            float posX = UtilitiesTm.FloatRemap(momentOrPosX, 0, AnimationTimer.GetMaxTime(), 0, railwidthAbsolute);

            objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(momentOrPosX);
            rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);
            SceneManaging.createRectangle(newCopyOfFigure, colFigure, rails[currentRailIndex].GetComponent<RectTransform>().rect.height, Instantiate(prefabRect), rectSize);

            if (savedLayer != -1)
            {
                tmpRectTransform.anchoredPosition = new Vector3(posX, -rails[currentRailIndex].GetComponent<RectTransform>().rect.height / 2, -1);
                oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, rectSize);

                if (savedLayer == 1)
                {
                    SceneManaging.scaleToLayerSize(newCopyOfFigure, 1, rails[currentRailIndex], rectSize, true);
                    railList[currentRailIndex].sizeLayering = 2;
                    oP.layer = 1;
                }
                else if (savedLayer == 0)
                {
                    SceneManaging.scaleToLayerSize(newCopyOfFigure, 0, rails[currentRailIndex], rectSize, true);
                    oP.layer = 1;
                }
                else
                {
                    SceneManaging.scaleToLayerSize(newCopyOfFigure, 2, rails[currentRailIndex], rectSize, true);
                    railList[currentRailIndex].sizeLayering = 2;
                    oP.layer = 2;
                }
            }
        }
        else
        {
            #region limit front and end of rail
            if (tmpRectTransform.anchoredPosition.x < 0)
            {
                tmpRectTransform.anchoredPosition = new Vector3(0, tmpRectTransform.anchoredPosition.y, -1);
                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
                oP.position = new Vector2(0, oP.position.y);

                float moment = UtilitiesTm.FloatRemap(tmpRectTransform.anchoredPosition.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
                rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);
            }
            // back of rail
            else if (tmpRectTransform.anchoredPosition.x + rectSize > railwidthAbsolute)
            {
                float moment = UtilitiesTm.FloatRemap(tmpRectTransform.anchoredPosition.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
                rectSize = railwidthAbsolute / (AnimationTimer.GetMaxTime() / objectAnimationLength);

                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
                tmpRectTransform.anchoredPosition = new Vector2(railwidthAbsolute - rectSize, tmpRectTransform.anchoredPosition.y);
                oP.position = new Vector2(railwidthAbsolute - rectSize, oP.position.y);
            }
            else
            {
                float moment = UtilitiesTm.FloatRemap(tmpRectTransform.anchoredPosition.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
                rectSize = objectAnimationLength / AnimationTimer.GetMaxTime() * railwidthAbsolute;
                newCopyOfFigure.transform.position = new Vector3(newCopyOfFigure.transform.position.x, figureObjects[figureNr].transform.position.y, -1);
            }
            #endregion

            SceneManaging.createRectangle(newCopyOfFigure, colFigure, rails[currentRailIndex].GetComponent<RectTransform>().rect.height, Instantiate(prefabRect), rectSize);
            Destroy(newCopyOfFigure.transform.GetChild(2).gameObject);

            #region calculating Layer Overlap
            SceneManaging.scaleToLayerSize(newCopyOfFigure, 0, rails[currentRailIndex], rectSize, false);
            oP.layer = 1;
            oP.position = new Vector2(tmpRectTransform.anchoredPosition.x, tmpRectTransform.sizeDelta.x);

            if (railList[currentRailIndex].sizeLayering == 1)
            {
                if (IsCurrentFigureOverlapping(oP, "", out countOverlaps, railList[currentRailIndex].myObjects) != -1)
                {
                    railList[currentRailIndex].sizeLayering = 2;
                    SceneManaging.scaleToLayerSize(newCopyOfFigure, 2, rails[currentRailIndex], rectSize, false);
                    oP.layer = 2;

                    // others to 1
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].layer == 1)
                            SceneManaging.scaleToLayerSize(railList[currentRailIndex].myObjects[i].figure, 1, rails[currentRailIndex], railList[currentRailIndex].myObjects[i].position.y, false);
                    }
                }
            }
            else
            {
                CreateListWithoutCurrentFigure(oP);
                FindFreeSpot(currentRailIndex, oP);
            }
            #endregion
        }

        //set 3d object to default position
        curr3DObject = Instantiate(figureObjects3D[figureNr]);
        oP.figure3D = curr3DObject;
        oP.figure3D.transform.SetParent(rails3D[currentRailIndex].transform.GetChild(0));

        //set original image back to shelf, position 2 to make it visible
        figureObjects[figureNr].transform.SetParent(objectShelfParent[figureNr].transform);
        figureObjects[figureNr].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
        figureObjects[figureNr].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
        figureObjects[figureNr].transform.SetSiblingIndex(1);
        figureObjects[figureNr].transform.GetChild(1).gameObject.SetActive(false);

        //holdMinimumDistance(oP, _spaceMax);

        if (saveToSceneData) // from flyer
        {
            // Save to SceneData:
            FigureInstanceElement thisFigureInstanceElement = new FigureInstanceElement
            {
                instanceNr = countName, //index
                name = curr3DObject.name + "_" + countName.ToString("000"),
                railStart = (int)char.GetNumericValue(rails[currentRailIndex].name[17]) - 1, //railIndex
                moment = momentOrPosX,
                layer = 0
            };

            StaticSceneData.StaticData.figureElements[figureNr].figureInstanceElements.Add(thisFigureInstanceElement);
            gameController.GetComponent<SceneDataController>().objects3dFigureInstances.Add(curr3DObject);
        }

        railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();

        // an welcher position ist aktuelle Figur nach der Sortierung? (benoetigt fuer highlight und loeschen)
        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
        {
            if (oP.objName == railList[currentRailIndex].myObjects[i].objName)
                currentPosInList = i;
        }

        // nachbarn berechnen
        SceneManaging.CalculateNeighbors(railList[currentRailIndex].myObjects);

        return curr3DObject;
    }
    private void CreateNewInstanceFlyer(int figureNr, int spaceNr)
    {
        int countName = CountCopiesOfObject(figureObjects[figureNr].name);

        if (gameController.GetComponent<UnitySwitchExpertUser>()._isExpert)
        {
            figCounterCircle[figureNr].text = (countName + 1).ToString();
        }

        // wenn schon eine figur drin ist, muss die raus
        if (SceneManaging.flyerSpace[spaceNr] != -1)
        {
            Destroy(flyerSpaces[spaceNr].transform.GetChild(1).gameObject);
            flyerSpaces[spaceNr].transform.GetChild(1).SetParent(parentMenue.transform);
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
        tmpRectTransform.localScale = Vector2.one;
    }
    public void ResetScreenSize()       // this probably has to be called globally, so that every Menue resizes (probably in the UIController). At the moment it is only scaled properly when rail tab is open e.g.
    {
        // I used the values that were put in FullHD (global: position.x) and calculated the percentage so that it works for all resolutions
        minX = 0.087f * Screen.width;               //timeline-rail-minX
        railWidth = 0.87f * Screen.width;           //railwidth=1670.4px
        heightClosed = heightRailPercent * Screen.height;
        heightOpened = 0.074f * Screen.height;
        maxX = minX + railWidth;
        //timeline-rail-maxX
        screenDifference = new Vector2(1920.0f / Screen.width, 1080.0f / Screen.height);

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
    public bool IsSomethingOverlapping(int index)
    {
        bool val = false;
        for (int i = 0; i < railList[index].myObjects.Count; i++)
        {
            for (int j = 0; j < railList[index].myObjects.Count; j++)
            {
                if (railList[index].myObjects[j].objName != railList[index].myObjects[i].objName)
                {
                    if (railList[index].myObjects[j].position.x >= railList[index].myObjects[i].position.x
                    && railList[index].myObjects[j].position.x <= (railList[index].myObjects[i].position.x + (railList[index].myObjects[i].position.y)))
                    {
                        val = true;
                    }
                    else if ((railList[index].myObjects[j].position.x + railList[index].myObjects[j].position.y) >= railList[index].myObjects[i].position.x
                    && railList[index].myObjects[j].position.x <= railList[index].myObjects[i].position.x)
                    {
                        val = true;
                    }
                }
            }
        }
        return val;
    }
    public void Update3DFigurePositions()
    {
        for (int k = 0; k < railList.Length; k++)
        {
            for (int i = 0; i < railList[k].myObjects.Count; i++)
            {
                float moment = UtilitiesTm.FloatRemap(railList[k].myObjects[i].position.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                float zPosFigure = rails3D[k].transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime(moment);

                if (zPosFigure < 0) // wenn die Figur auf eine Position noch vor dem empty gesetzt werden würde, würde sie erscheinen bevor sie auf den rails da ist
                {
                    zPosFigure = 500f;      // deshalb wird die Figur komplett außerhalb der Szene abgesetzt.
                }

                if (rails3D[k].name.Substring(7) == "1" || rails3D[k].name.Substring(7) == "3" || rails3D[k].name.Substring(7) == "5")
                {
                    railList[k].myObjects[i].figure3D.transform.localPosition = new Vector3(-rails3D[k].transform.GetChild(0).transform.localPosition.x, -rails3D[k].transform.GetChild(0).transform.localPosition.y - 0.01f, zPosFigure);
                }

                else
                {
                    railList[k].myObjects[i].figure3D.transform.localPosition = new Vector3(rails3D[k].transform.GetChild(0).transform.localPosition.x, -rails3D[k].transform.GetChild(0).transform.localPosition.y - 0.01f, zPosFigure);
                }

                if (rails3D[k].transform.GetChild(0).GetComponent<RailSpeedController>().railIndex % 2 == 1)
                {
                    railList[k].myObjects[i].figure3D.transform.localEulerAngles = new Vector3(railList[k].myObjects[i].figure3D.transform.localEulerAngles.x, 270, railList[k].myObjects[i].figure.transform.localEulerAngles.z);
                }
                else
                {
                    railList[k].myObjects[i].figure3D.transform.localEulerAngles = new Vector3(railList[k].myObjects[i].figure3D.transform.localEulerAngles.x, 270, railList[k].myObjects[i].figure3D.transform.localEulerAngles.z);
                }

                for (int j = 0; j < StaticSceneData.StaticData.figureElements[int.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements.Count; j++)
                {
                    if (StaticSceneData.StaticData.figureElements[int.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[j].instanceNr == int.Parse(railList[k].myObjects[i].figure.name.Substring(16)))
                    {
                        StaticSceneData.StaticData.figureElements[int.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[j].moment = moment;

                        if (railList[k].myObjects[i].layer == 1) //railList[k].myObjects.Contains(railList[k].myObjects[i]))
                        {
                            if (railList[k].sizeLayering == 1)
                                StaticSceneData.StaticData.figureElements[int.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[j].layer = 0;
                            else
                                StaticSceneData.StaticData.figureElements[int.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[j].layer = 1;
                        }
                        else
                            StaticSceneData.StaticData.figureElements[int.Parse(railList[k].myObjects[i].figure.name.Substring(6, 2)) - 1].figureInstanceElements[j].layer = 2;
                    }
                }
            }
        }
    }
    /*bool holdMinimumDistance(Figure currentObj, float minimumDistance)
    {
        RectTransform tmpRectTransform = currentObj.figure.GetComponent<RectTransform>();
        bool found = false;
        bool found2 = false;
        bool space = true;

        for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
        {
            // wenn innerhalb des mindestabstands rechts schon eine andere figur ist
            if (currentObj.objName != railList[currentRailIndex].myObjects[j].objName
            && currentObj.position.x < railList[currentRailIndex].myObjects[j].position.x + minimumDistance
            && currentObj.position.x > railList[currentRailIndex].myObjects[j].position.x)
            {
                // wenn dort, wo die figur nach der berechnung hinspringen würde, schon eine figur ist
                for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                {
                    if (railList[currentRailIndex].myObjects[i].position.x < currentObj.position.x + currentObj.position.y + minimumDistance
                        && railList[currentRailIndex].myObjects[i].position.x > currentObj.position.x)
                    {
                        Debug.Log("figure: " + railList[currentRailIndex].myObjects[i].objName);
                        found = true;
                    }
                }
                if (found)
                {
                    // noch andere seite probieren
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y > currentObj.position.x - minimumDistance
                        && railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y < currentObj.position.x)
                        {
                            Debug.Log("figure: " + railList[currentRailIndex].myObjects[i].objName);
                            found2 = true;
                        }
                    }
                    if (found2)
                    {
                        Debug.Log("kein platz!");
                        space = false;
                    }
                    else
                    {
                        Debug.Log("auf andere seite!");
                        tmpRectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[j].position.x - minimumDistance, tmpRectTransform.anchoredPosition.y);
                        currentObj.position = new Vector2(railList[currentRailIndex].myObjects[j].position.x - minimumDistance, tmpRectTransform.sizeDelta.x);
                    }
                }
                else
                {
                    Debug.Log("auf diese seite");
                    tmpRectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.anchoredPosition.y);
                    currentObj.position = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.sizeDelta.x);
                }

            }
            else if (currentObj.objName != railList[currentRailIndex].myObjects[j].objName
            && currentObj.position.x < railList[currentRailIndex].myObjects[j].position.x
            && currentObj.position.x > railList[currentRailIndex].myObjects[j].position.x - minimumDistance)
            {
                // wenn dort, wo die figur nach der berechnung hinspringen würde, schon eine figur ist
                for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                {
                    if (railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y > currentObj.position.x - minimumDistance
                        && railList[currentRailIndex].myObjects[i].position.x + railList[currentRailIndex].myObjects[i].position.y < currentObj.position.x)
                    {
                        Debug.Log("figure: " + railList[currentRailIndex].myObjects[i].objName);
                        found = true;
                    }
                }
                if (found)
                {
                    Debug.Log("auf andere seite!");
                    tmpRectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.anchoredPosition.y);
                    currentObj.position = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.sizeDelta.x);
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].position.x < currentObj.position.x + currentObj.position.y + minimumDistance
                            && railList[currentRailIndex].myObjects[i].position.x > currentObj.position.x)
                        {
                            Debug.Log("figure: " + railList[currentRailIndex].myObjects[i].objName);
                            found2 = true;
                            space = false;
                        }
                    }
                    if (found2)
                    {
                        tmpRectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.anchoredPosition.y);
                        currentObj.position = new Vector2(railList[currentRailIndex].myObjects[j].position.x + minimumDistance, tmpRectTransform.sizeDelta.x);
                    }
                    else
                    {
                        Debug.Log("kein platz!");
                    }
                }
                else
                {
                    Debug.Log("rechts");
                    tmpRectTransform.anchoredPosition = new Vector2(railList[currentRailIndex].myObjects[j].position.x - minimumDistance, tmpRectTransform.anchoredPosition.y);
                    currentObj.position = new Vector2(railList[currentRailIndex].myObjects[j].position.x - minimumDistance, tmpRectTransform.sizeDelta.x);
                }
            }
        }
        return space;
    }
    */
    private void CreateListWithoutCurrentFigure(Figure fig)
    {
        listWithoutCurrentFigure.Clear();

        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
        {
            if (railList[currentRailIndex].myObjects[i].objName != fig.objName)
            {
                listWithoutCurrentFigure.Add(railList[currentRailIndex].myObjects[i]);
            }
        }
    }
    void Update()
    {
        #region eachCycle
        if (currentLossyScale != transform.lossyScale.x)    // Abfrage, ob sich lossyScale geaendert hat dann werden die Schienen rescaled
        {
            currentLossyScale = transform.lossyScale.x;
            ResetScreenSize();
            // todo: resize all figures on timeline
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
        #endregion

        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            #region identifying
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            // Erstelle eine Liste von Raycast-Ergebnissen
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (!SceneManaging.tutorialActive)  // wenn tutorial gerade ausgeführt wird, soll nichts klickbar sein
            {
                //identify which gameobject you clicked
                if (Input.mousePosition.y > Screen.height * 0.45f)
                    currentClickedObjectIndex = SceneManaging.identifyClickedObject(figureObjects);         //method fills up the current clicked index
                if (currentRailIndex != -1)
                {
                    currentClickedInstanceObjectIndex = SceneManaging.identifyClickedObjectByList(railList[currentRailIndex].myObjects);
                    editTimelineObject = false;                             //flag to prevent closing the timeline if you click an object in timeline
                    releaseOnTimeline = false;                              //because you have not set anything on timeline 
                    releaseObjMousePos = new Vector2(0.0f, 0.0f);

                    int tmpI = -1;
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].figure.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos) && results[0].gameObject.name != "ImageTimeSliderOverlay")
                            tmpI = i;
                    }
                    if (tmpI >= 0)
                        editTimelineObject = true;
                    else
                        editTimelineObject = false;
                }

                //if you click the timeline with the mouse
                if (!SceneManaging.flyerActive)
                {
                    for (int i = 0; i < rails.Length; i++)
                    {
                        if (rails[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                        {
                            currentRailIndex = i;
                            //open or close timeline
                            OpenTimelineByClick(railList[i].isTimelineOpen, i, false);
                        }
                    }
                }

                // wenn theaterzettel aktiv ist    
                else
                {
                    if (currentSpaceActive != -1)
                    {
                        // click delete
                        Vector2 pos = flyerSpaces[currentSpaceActive].transform.GetChild(1).GetChild(0).GetChild(0).position;
                        RectTransform rect = flyerSpaces[currentSpaceActive].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
                        flyerSpaces[currentSpaceActive].GetComponent<Image>().color = new Color(.78f, .54f, .44f, 0f);
                        if (getMousePos.x >= pos.x - rect.sizeDelta.x / 2.5f && getMousePos.x <= pos.x + rect.sizeDelta.x / 2.5f && getMousePos.y >= pos.y - rect.sizeDelta.y / 2.5f && getMousePos.y <= pos.y + rect.sizeDelta.y / 2.5f)
                        {
                            SceneManaging.flyerSpace[currentSpaceActive] = -1;
                            Destroy(flyerSpaces[currentSpaceActive].transform.GetChild(1).gameObject);
                            // color field
                            flyerSpaces[currentSpaceActive].transform.GetChild(0).gameObject.SetActive(false);

                        }
                    }

                    int tmpHit = CheckHittingFlyerSpace(getMousePos);
                    // wenn eine figur im kaestchen ist und diese geklickt wird
                    if (tmpHit != -1 && SceneManaging.flyerSpace[tmpHit] != -1)
                    {
                        for (int i = 0; i < flyerSpaces.Length; i++)
                        {
                            SceneManaging.highlight(null, flyerSpaces[i], false, "flyer");
                        }
                        SceneManaging.highlight(null, flyerSpaces[tmpHit], true, "flyer");
                        currentSpaceActive = tmpHit;
                    }

                    // nothing clicked
                    else
                    {
                        if (currentSpaceActive != -1)
                        {
                            SceneManaging.highlight(null, flyerSpaces[currentSpaceActive].gameObject, false, "flyer");// flyerSpaces[currentSpaceActive].transform.GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(false);
                            currentSpaceActive = -1;
                        }
                    }
                }
                #endregion

                //if you click on an object in shelf
                if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
                {
                    if (currentRailIndex != -1)
                    {
                        //wenn neues objekt genommen wird, soll altes aktuelles objekt unhighlighted werden
                        for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                        {
                            //Debug.Log("figure: " + railList[currentRailIndex].myObjects[j].objName);
                            SceneManaging.highlight(railList[currentRailIndex].myObjects[j].figure3D, railList[currentRailIndex].myObjects[j].figure, false, "figure");
                        }
                    }
                    for (int i = 0; i < flyerSpaces.Length; i++)
                    {
                        SceneManaging.highlight(null, flyerSpaces[i], false, "flyer");
                    }

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
                    if (railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                    {
                        // aktuellen namen speichern, um spaeter die aktuelle position nach dem sortieren rauszubekommen
                        currentName = railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].objName;

                        CreateListWithoutCurrentFigure(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex]);
                        SceneManaging.CalculateNeighbors(listWithoutCurrentFigure);

                        // click on delete-Button
                        if (railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).gameObject.activeSelf
                            && getMousePos.x >= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.x - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x / 2.5f && getMousePos.x <= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.x + railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x / 2.5f
                            && getMousePos.y >= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.y - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.y / 2.5f && getMousePos.y <= railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).position.y + railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.y / 2.5f)
                        {
                            _toBeRemovedFromTimeline = true;
                        }

                        diff = new Vector2(getMousePos.x - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.position.x, getMousePos.y - railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.position.y);
                        draggingOnTimeline = true;
                        SceneManaging.dragging = true;

                        //highlighting objects and showing delete button when clicked
                        if (SceneManaging.highlighted == false)
                        {
                            SceneManaging.highlight(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure3D, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, true, "figure");
                        }
                        else if (SceneManaging.highlighted && railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure.transform.GetChild(0).GetComponent<Image>().color == colFigureHighlighted) // checkFigureHighlighted(timelineInstanceObjects3D[currentClickedInstanceObjectIndex].transform.GetChild(1).gameObject))  // check if second child (which is never the armature) has emission enabled (=is highlighted)
                        {
                            // highlight(timelineInstanceObjects3D[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex], false);
                        }
                        else
                        {
                            for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                            {
                                SceneManaging.highlight(railList[currentRailIndex].myObjects[i].figure3D, railList[currentRailIndex].myObjects[i].figure, false, "figure");
                            }
                            SceneManaging.highlight(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure3D, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, true, "figure");
                        }
                    }

                }
                //if you hit/clicked nothing with mouse
                else if (Physics2D.OverlapPoint(getMousePos) == false)
                {
                    if (currentRailIndex != -1)
                    {
                        // delete highlight of everything if nothing is clicked
                        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                        {
                            SceneManaging.highlight(railList[currentRailIndex].myObjects[i].figure3D, railList[currentRailIndex].myObjects[i].figure, false, "figure");
                        }
                    }
                }
            }

        }
        // if timeline is open and something is being dragged
        if (draggingOnTimeline)
        {
            // drag object
            if (currentClickedInstanceObjectIndex != -1)
            {
                Figure currentObj = railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex];
                RectTransform currentRect = currentObj.figure.GetComponent<RectTransform>();
                isInstance = true;

                // abfrage, ob reihenfolge sich geaendert hat
                if (currentName != currentObj.objName)
                {
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].objName == currentName)
                        {
                            currentClickedInstanceObjectIndex = i;
                            currentObj = railList[currentRailIndex].myObjects[i];
                        }
                    }
                }

                //if you click an object in timeline (for dragging)
                UpdateObjectPosition(currentObj, getMousePos - diff);
                currentObj.figure.transform.position = new Vector3(currentObj.figure.transform.position.x, rails[currentRailIndex].transform.position.y, -1.0f);

                #region limits front and back of rail
                // limit front of rail
                if (currentRect.anchoredPosition.x <= 0)
                {
                    currentRect.anchoredPosition = new Vector2(0, currentRect.anchoredPosition.y);
                    currentObj.position = new Vector2(0, currentObj.position.y);
                }

                // limit back of rail
                else if ((currentRect.anchoredPosition.x + currentRect.sizeDelta.x) > railwidthAbsolute)
                {
                    currentRect.anchoredPosition = new Vector2(railwidthAbsolute - currentRect.sizeDelta.x, currentRect.anchoredPosition.y);
                    currentObj.position = new Vector2(railwidthAbsolute - currentRect.sizeDelta.x, currentObj.position.y);
                }
                #endregion

                #region Stapeln von Ebenen
                if (SceneManaging.isExpert)
                {
                    float moment = UtilitiesTm.FloatRemap(currentRect.anchoredPosition.x, 0, railwidthAbsolute, 0, AnimationTimer.GetMaxTime());
                    objectAnimationLength = rails3D[currentRailIndex].transform.GetChild(0).GetComponent<RailSpeedController>().GetEndTimeFromStartTime(moment);
                    rectSize = objectAnimationLength / AnimationTimer.GetMaxTime() * railwidthAbsolute;
                }
                // if there is only one layer on Rail
                if (railList[currentRailIndex].sizeLayering == 1)
                {
                    if (IsCurrentFigureOverlapping(currentObj, "", out countOverlaps, railList[currentRailIndex].myObjects) != -1)
                    {
                        SceneManaging.scaleToLayerSize(currentObj.figure, 2, rails[currentRailIndex], rectSize, false);
                        railList[currentRailIndex].sizeLayering = 2;
                        currentObj.layer = 2;

                        // scale all other timelineobjects of layer 1
                        for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                        {
                            if (railList[currentRailIndex].myObjects[j].figure != currentObj.figure && railList[currentRailIndex].myObjects[j].layer == 1)
                            {
                                SceneManaging.scaleToLayerSize(railList[currentRailIndex].myObjects[j].figure, 1, rails[currentRailIndex], rectSize, false);
                                railList[currentRailIndex].myObjects[j].layer = 1;
                            }
                        }
                    }
                    else if (SceneManaging.isExpert)
                        SceneManaging.scaleToLayerSize(currentObj.figure, 0, rails[currentRailIndex], rectSize, false);
                }

                // if there are two layers on Rail
                else
                {
                    // wenn nichts mehr überlappt
                    if (!IsSomethingOverlapping(currentRailIndex))
                    {
                        railList[currentRailIndex].sizeLayering = 1;
                        // scale all timelineobjects to layer 0
                        for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                        {
                            SceneManaging.scaleToLayerSize(railList[currentRailIndex].myObjects[j].figure, 0, rails[currentRailIndex], rectSize, false);
                            railList[currentRailIndex].myObjects[j].layer = 1;
                        }
                    }
                    else
                        FindFreeSpot(currentRailIndex, currentObj);
                    // todo mindestabstand von 50 cm einhalten
                    //holdMinimumDistance(currentObj, _spaceMax);
                }

                #endregion
            }
            railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();
            releaseOnTimeline = true;
        }
        //if something has been dragged outside of the timeline
        /*if (draggingOnTimeline == false && editTimelineObject == false && draggingObject && isInstance)
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
                    SceneManaging.scaleObject(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, figPictureSize, rails[newHitTimeline].GetComponent<RectTransform>().sizeDelta.y, false);
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
                _toBeRemovedFromTimeline = true;
            }
        }*/
        // dragging an object from shelf to timeline / flyer
        if (draggingObject && editTimelineObject == false && isInstance == false)
        {
            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            if (_timerFigure <= 0.5f)
                _timerFigure += Time.deltaTime;
            scrollRect.GetComponent<ScrollRect>().enabled = false;

            //move object
            figureObjects[currentClickedObjectIndex].transform.position = new Vector3(getMousePos.x - diff.x, getMousePos.y - diff.y, -1.0f);

            if (!SceneManaging.flyerActive)
                hitTimeline = CheckHittingTimeline(getMousePos);
            else
                flyerHit = CheckHittingFlyerSpace(getMousePos);

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            if (hitTimeline != -1)
            {
                if (hitTimelineOld != hitTimeline)
                {
                    hitTimelineOld = hitTimeline;
                    OpenTimelineByDrag(hitTimeline);

                    figureObjects[currentClickedObjectIndex].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[hitTimeline].GetComponent<RectTransform>().rect.height);
                    figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(figPictureSize, rails[hitTimeline].GetComponent<RectTransform>().rect.height);
                    // rect sichtbar machen
                    figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(true);
                    currentRailIndex = hitTimeline;
                }

                //snapping/lock y-axis
                figureObjects[currentClickedObjectIndex].transform.position = new Vector2(getMousePos.x - (25 / screenDifference.x), rails[currentRailIndex].transform.position.y);

                //save position, where object on timeline is released + set flag
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }
            // wenn zwischenraum zwischen schienen getroffen wird
            else if (IsRailAreaHit(getMousePos) && !SceneManaging.flyerActive)
            {
                //snapping/lock y-axis
                figureObjects[currentClickedObjectIndex].transform.position = new Vector2(getMousePos.x - (25 / screenDifference.x), rails[currentRailIndex].transform.position.y);

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
                //move object
                figureObjects[currentClickedObjectIndex].transform.position = new Vector3(getMousePos.x - diff.x, getMousePos.y - diff.y, -1.0f);
                //rect unsichtbar machen
                figureObjects[currentClickedObjectIndex].transform.GetChild(1).gameObject.SetActive(false);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
                releaseOnTimeline = false;
                hitTimelineOld = -1;
            }
        }
        //-------release mousebutton
        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            if (!SceneManaging.sceneChanged) SceneManaging.sceneChanged = true;

            scrollRect.GetComponent<ScrollRect>().enabled = true;
            draggingObject = false;
            editTimelineObject = false;
            hitTimelineOld = -1;

            // flyer mode
            if (SceneManaging.flyerActive && currentClickedObjectIndex != -1)
            {
                if (flyerHit != -1)
                    CreateNewInstanceFlyer(currentClickedObjectIndex, flyerHit);

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

                if (flyerHit != -1)
                {
                    for (int i = 0; i < flyerSpaces.Length; i++)
                        SceneManaging.highlight(null, flyerSpaces[i], false, "flyer");
                    SceneManaging.highlight(null, flyerSpaces[flyerHit], true, "flyer");
                    currentSpaceActive = flyerHit;
                }

            }

            // if dropped on a timeline
            else if (releaseOnTimeline == true)
            {
                if (releaseObjMousePos == getMousePos)
                {
                    // if rail has been changed
                    /*if (changedRail)
                    {
                        Figure fig = railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex];

                        if (newHitTimeline != -1)
                        {
                            hitTimeline = newHitTimeline;
                        }

                        // figure auf neue schiene setzen (parent)
                        fig.figure.transform.SetParent(rails[hitTimeline].transform);
                        fig.figure.transform.GetChild(0).gameObject.SetActive(true); // set rect active
                        fig.figure3D.transform.SetParent(rails3D[currentRailIndex].transform.GetChild(0));

                        #region limit front and end of rail
                        RectTransform tmpRectTransform = fig.figure.GetComponent<RectTransform>();

                        if (tmpRectTransform.anchoredPosition.x < 0)
                        {
                            tmpRectTransform.anchoredPosition = new Vector3((0), tmpRectTransform.anchoredPosition.y, -1);
                            fig.position = new Vector2(0, fig.position.y);
                        }
                        // back of rail
                        else if (tmpRectTransform.anchoredPosition.x + rectSize > railwidthAbsolute)
                        {
                            // Todo Berechnung wie lang das rechteck dann sein muss
                            tmpRectTransform.anchoredPosition = new Vector2(railwidthAbsolute - rectSize, tmpRectTransform.anchoredPosition.y);
                            fig.position = new Vector2(railwidthAbsolute - rectSize, fig.position.y);
                        }
                        #endregion

                        #region LayerOverlap
                        // wenn figur size layering 1 hat
                        if (railList[hitTimeline].sizeLayering == 1)
                        {
                            // wenn figur überlappt muss sie auf layer 2
                            if (isCurrentFigureOverlapping(hitTimeline, fig, "", out countOverlaps, railList[hitTimeline].myObjects) != -1)
                            {
                                railList[hitTimeline].sizeLayering = 2;
                                SceneManaging.scaleToLayerSize(fig.figure, 2, rails[hitTimeline], rectSize, false);
                                fig.layer = 2;

                                // others to 1
                                for (int i = 0; i < railList[hitTimeline].myObjects.Count; i++)
                                {
                                    if (railList[hitTimeline].myObjects[i].objName != fig.objName)
                                    {
                                        SceneManaging.scaleToLayerSize(railList[hitTimeline].myObjects[i].figure, 1, rails[hitTimeline], railList[hitTimeline].myObjects[i].position.y, false);
                                        railList[hitTimeline].myObjects[i].layer = 1;
                                    }
                                }
                            }
                            //figur ueberlappt nicht und muss auf layer 1
                            else
                            {
                                SceneManaging.scaleToLayerSize(fig.figure, 0, rails[hitTimeline], rectSize, false);
                                fig.layer = 1;
                            }
                            fig.position = new Vector2(fig.figure.GetComponent<RectTransform>().anchoredPosition.x, rectSize);
                            // rectTransform.sizeDelta = new Vector2(rectSize, rectTransform.sizeDelta.y);
                        }

                        // wenn hittimeline size-layering 2 hat
                        else
                        {
                            // liste ohne current obj neu erstellen
                            listWithoutCurrentFigure.Clear();
                            listWithoutCurrentFigure = railList[hitTimeline].myObjects;
                            FindFreeSpot(hitTimeline, fig);
                        }

                        // from old 'myObjects' list to new (hittimeline) 'myobjects' list
                        //Debug.Log("highlight: " + railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].objName);
                        SceneManaging.highlight(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure3D, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].figure, true, "figure");
                        UpdatePositionVectorInformation(fig, railList[currentRailIndex].myObjects, railList[hitTimeline].myObjects);
                        railList[hitTimeline].myObjects = railList[hitTimeline].myObjects.OrderBy(w => w.position.x).ToList();
                        SceneManaging.CalculateNeighbors(railList[hitTimeline].myObjects);
                        #endregion

                        //snapping/lock y-axis
                        setObjectOnTimeline(fig.figure, rails[hitTimeline].transform.position.y);

                        //wenn auf current rail nichts mehr ueberlappt
                        if (!isSomethingOverlapping(currentRailIndex))
                        {
                            railList[currentRailIndex].sizeLayering = 1;
                            // scale all other timelineobjects to layer 0
                            for (int j = 0; j < railList[currentRailIndex].myObjects.Count; j++)
                            {
                                SceneManaging.scaleToLayerSize(railList[currentRailIndex].myObjects[j].figure, 0, rails[currentRailIndex], railList[currentRailIndex].myObjects[j].position.y, false);
                            }
                        }

                        // auf andere Schiene gesetzt (nicht die gleiche zurueck)
                        if (hitTimeline != currentRailIndex)
                        {
                            //3D object
                            StaticSceneData.StaticData.figureElements[Int32.Parse(railList[hitTimeline].myObjects[railList[hitTimeline].myObjects.Count - 1].objName.Substring(6, 2)) - 1].figureInstanceElements[Int32.Parse(railList[hitTimeline].myObjects[railList[hitTimeline].myObjects.Count - 1].objName.Substring(17))].railStart = hitTimeline;
                            railList[hitTimeline].myObjects[railList[hitTimeline].myObjects.Count - 1].figure3D.transform.SetParent(rails3D[hitTimeline].transform.GetChild(0));

                            // box Collider disablen, damit Schiene auch auf Figuren geklickt werden kann
                            for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                            {
                                railList[currentRailIndex].myObjects[i].figure.GetComponent<BoxCollider2D>().enabled = false;
                            }
                            currentRailIndex = hitTimeline;
                        }

                        // highlight figure with current name
                        // for (int i = 0; i < railList[hitTimeline].myObjects.Count; i++)
                        // {
                        //     if (railList[hitTimeline].myObjects[i].objName == currentName)
                        //     {
                        //         highlight(railList[hitTimeline].myObjects[i].figure3D, railList[hitTimeline].myObjects[i].figure, true);
                        //     }
                        // }
                        changedRail = false;
                    }
                    else
                    {*/
                    GameObject curr3DObject = CreateNew2DInstance(currentClickedObjectIndex, getMousePos.x, -1, -1, false);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Save to SceneData:
                    FigureInstanceElement thisFigureInstanceElement = new FigureInstanceElement
                    {
                        instanceNr = CountCopiesOfObject(figureObjects[currentClickedObjectIndex].name) - 1, //index
                        name = curr3DObject.name + "_" + (CountCopiesOfObject(figureObjects[currentClickedObjectIndex].name) - 1).ToString("000"),
                        railStart = (int)Char.GetNumericValue(rails[currentRailIndex].name[17]) - 1 //railIndex
                    };

                    StaticSceneData.StaticData.figureElements[currentClickedObjectIndex].figureInstanceElements.Add(thisFigureInstanceElement);
                    gameController.GetComponent<SceneDataController>().objects3dFigureInstances.Add(curr3DObject);
                    SceneManaging.highlight(railList[currentRailIndex].myObjects[currentPosInList].figure3D, railList[currentRailIndex].myObjects[currentPosInList].figure, true, "figure");
                    if (_toBeRemoved)
                    {
                        RemoveObjectFromTimeline(railList[currentRailIndex].myObjects[currentPosInList]);
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////

                    currentClickedObjectIndex = -1; // set index back to -1 because nothing is being clicked anymore
                }

                // if figure has been moved in rail
                else
                {
                    // change position of vector in each case
                    for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                    {
                        if (railList[currentRailIndex].myObjects[i].objName == railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].objName)
                            railList[currentRailIndex].myObjects[i].position = new Vector2(railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position.x, railList[currentRailIndex].myObjects[currentClickedInstanceObjectIndex].position.y);
                    }
                    railList[currentRailIndex].myObjects = railList[currentRailIndex].myObjects.OrderBy(w => w.position.x).ToList();
                    SceneManaging.CalculateNeighbors(railList[currentRailIndex].myObjects);

                    if (_toBeRemovedFromTimeline)
                    {
                        for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                        {
                            if (railList[currentRailIndex].myObjects[i].objName == currentName)
                                RemoveObjectFromTimeline(railList[currentRailIndex].myObjects[i]);
                        }
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
                    helpButton.GetComponent<PressHelp>().helpTextLiveView.SetActive(true);
                }

                figureObjects[currentClickedObjectIndex].transform.SetParent(objectShelfParent[currentClickedObjectIndex].transform);
                // bring object back to position two, so that its visible and change position back to default
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                figureObjects[currentClickedObjectIndex].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = objectShelfSize;
            }

            // if instance is dropped somewhere else than on timeline: delete instance
            else if (releaseOnTimeline == false && currentClickedInstanceObjectIndex != -1 && isInstance && _toBeRemovedFromTimeline)
            {
                for (int i = 0; i < railList[currentRailIndex].myObjects.Count; i++)
                {
                    if (railList[currentRailIndex].myObjects[i].objName == currentName)
                        RemoveObjectFromTimeline(railList[currentRailIndex].myObjects[i]);
                }
            }

            Update3DFigurePositions();

            // enable binnenanimation when playing
            for (int k = 0; k < railList.Length; k++)
            {
                for (int i = 0; i < railList[k].myObjects.Count; i++)
                {
                    // start Animation on play
                    if (railList[k].myObjects[i].figure3D.GetComponent<FigureStats>().isShip == false)
                    {
                        if (SceneManaging.playing && !railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled)
                            railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled = true;
                        else if (!SceneManaging.playing && railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled)
                            railList[k].myObjects[i].figure3D.GetComponent<Animator>().enabled = false;
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
    }
    public void PublicUpdate()
    {
        Update();
    }
}
