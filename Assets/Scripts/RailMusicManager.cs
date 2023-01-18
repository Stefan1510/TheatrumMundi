using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailMusicManager : MonoBehaviour
{
    #region variables
    public Image timelineImage;
    AudioSource audioSource;
    public GameObject gameController, UICanvas, contentRailsMenue;
    //public GameObject[] musicSamples;
    //private BoxCollider2D timeSlider;
    bool draggingOnTimeline, draggingObject, editTimelineObject, releaseOnTimeline, playingMusic, isInstance, playingSample, fading;
    [HideInInspector] public bool isTimelineOpen;
    bool anyInstanceIsPlaying = false, firstTimeSecond = false, firstTimeThird = false;
    Vector2 releaseObjMousePos, diff;
    double minX, maxX;
    private float railWidth;
    int maxTimeInSec, currentClip;
    Vector2 sizeDeltaAsFactor;
    Vector2[] objectShelfPosition, objectShelfSize;
    GameObject[] objectShelfParent;
    GameObject newCopyOfFigure;
    //Vector2 objectSceneSize;

    public GameObject objectLibrary, parentMenue; // mainMenue
    GameObject[] figCounterCircle, figureObjects;
    int currentClickedObjectIndex, sampleButtonPressed;
    int currentClickedInstanceObjectIndex;
    private float currentLossyScale;
    public List<GameObject> timelineObjects, timelineInstanceObjects;
    Color colMusic, colMusicHighlighted;
    float heightClosed, heightOpened;

    public AudioClip[] clip;         // ought to be 6 audioclips
    #endregion
    void Awake()
    {
        clip = gameController.GetComponent<SceneDataController>().objectsMusicClips;
        timelineImage = this.GetComponent<Image>();
        //SceneManaging.anyTimelineOpen = false;
        draggingOnTimeline = false;
        draggingObject = false;
        editTimelineObject = false;
        releaseOnTimeline = false;
        playingMusic = false;
        releaseObjMousePos = new Vector2(0.0f, 0.0f);
        currentClip = 0;
        isTimelineOpen = false;
        isInstance = false;
        sampleButtonPressed = -1;

        //load all objects given in the figuresShelf
        figureObjects = new GameObject[objectLibrary.transform.childCount];         //instead of a count like "3"
        objectShelfPosition = new Vector2[figureObjects.Length];
        objectShelfSize = new Vector2[figureObjects.Length];
        objectShelfParent = new GameObject[figureObjects.Length];
        figCounterCircle = new GameObject[figureObjects.Length];

        colMusic = new Color(0.21f, 0.51f, 0.267f, 0.5f);
        colMusicHighlighted = new Color(0.21f, 0.81f, 0.267f, 0.5f);

        for (int i = 0; i < objectLibrary.transform.childCount; i++)
        {
            //collect objects
            figureObjects[i] = (objectLibrary.transform.GetChild(i).transform.GetChild(1).gameObject);
            objectShelfSize[i] = new Vector2(figureObjects[i].GetComponent<RectTransform>().rect.width, figureObjects[i].GetComponent<RectTransform>().rect.height);
            objectShelfParent[i] = figureObjects[i].transform.parent.gameObject;
        }

        currentClickedObjectIndex = -1;
        currentClickedInstanceObjectIndex = -1;
        currentLossyScale = 1.0f;

        ResetScreenSize();
        timelineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);

        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < figureObjects.Length; i++)
        {
            figCounterCircle[i] = figureObjects[i].transform.parent.GetChild(2).gameObject;
            figCounterCircle[i].transform.GetChild(0).GetComponent<Text>().text = "0";
        }

        List<GameObject> timelineObjects = new List<GameObject>();
        List<GameObject> timelineInstanceObjects = new List<GameObject>();

        maxTimeInSec = (int)AnimationTimer.GetMaxTime();
        sizeDeltaAsFactor = new Vector2(1.0f, 1.0f);
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
                    //Debug.Log("open: " + contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects.Count);
                    contentRailsMenue.GetComponent<RailManager>().rails[i].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    contentRailsMenue.GetComponent<RailManager>().rails[i].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed * 1.2f / gameObject.transform.lossyScale.x);
                    //contentRailsMenue.GetComponent<RailManager>().openCloseObjectInTimeline(true, contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects, i);
                    contentRailsMenue.GetComponent<RailManager>().railList[i].isTimelineOpen = false;
                    for (int j = 0; j < contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects3D.Count; j++)
                    {
                        contentRailsMenue.GetComponent<RailManager>().highlight(contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects3D[j], contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects[j], false);
                        contentRailsMenue.GetComponent<RailManager>().openCloseObjectInTimeline(true, contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects, i);
                    }
                }
                contentRailsMenue.GetComponent<RailManager>().openCloseObjectInTimeline(false, contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects, i);
            }
            for (int k = 0; k < 2; k++)
            {
                if (gameController.GetComponent<UIController>().RailLightBG[k].isTimelineOpen)
                {
                    gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<RectTransform>().sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightClosed / gameObject.transform.lossyScale.x);
                    gameController.GetComponent<UIController>().RailLightBG[k].GetComponent<RailLightManager>().isTimelineOpen = false;
                }

            }
            // open clicked rail and scale up timeline
            timelineImage.rectTransform.sizeDelta = new Vector2(timelineImage.rectTransform.rect.width, heightOpened / gameObject.transform.lossyScale.x);
            //scale up the collider
            timelineImage.GetComponent<BoxCollider2D>().size = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, heightOpened / gameObject.transform.lossyScale.x);
            openCloseObjectInTimeline(true, timelineInstanceObjects);
            isTimelineOpen = true;
            ImageTimelineSelection.SetRailNumber(6);
            ImageTimelineSelection.SetRailType(2);  // for rail-rails
        }
    }
    public void setParent(GameObject obj, GameObject parentToSet)
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
    public void updateObjectPosition(GameObject obj, Vector2 mousePos)
    {
        //set parent and move object
        setParent(obj, gameObject);
        obj.transform.position = new Vector2(mousePos.x, mousePos.y);
    }
    public bool checkHittingTimeline(GameObject obj, Image tl, Vector2 mousePos)
    {
        bool hit = false;
        Vector2 colSize = new Vector2(GetComponent<BoxCollider2D>().size.x * gameObject.transform.lossyScale.x, GetComponent<BoxCollider2D>().size.y * gameObject.transform.lossyScale.x);
        //if mouse hits the timeline while dragging an object
        if (mousePos.x <= maxX && mousePos.x > minX && mousePos.y <= tl.transform.position.y + (colSize.y / 2.0f) && mousePos.y > tl.transform.position.y - (colSize.y / 2.0f))
        {
            hit = true; ;
        }
        return hit;
    }
    public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects)
    {
        float length = 100.0f;
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(objects[i].GetComponent<RectTransform>().anchoredPosition.x, -gameObject.GetComponent<RectTransform>().rect.height / 2, -1);
            //if timeline open scale ALL objects up
            if (timelineOpen)
            {
                scaleObject(objects[i], length, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
            }
            else
            {
                //otherwise scale ALL objects down
                scaleObject(objects[i], length, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, true);
                scaleObject(objects[i].transform.GetChild(0).gameObject, objects[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, true);
                scaleObject(objects[i].transform.GetChild(1).gameObject, objects[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, true);
            }
        }
    }
    public void scaleObject(GameObject fig, float x, float y, bool boxCollZero)
    {
        //scale the object
        fig.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
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
    public void setObjectOnTimeline(GameObject fig, float x, float y)
    {
        fig.transform.position = new Vector2(fig.transform.position.x, y);
        fig.transform.localPosition = new Vector3(fig.transform.localPosition.x, fig.transform.localPosition.y, -1);
    }
    public void updateObjectList(List<GameObject> objects, GameObject obj)
    {
        //check if the object is already in list
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
        }
    }
    public void createRectangle(GameObject obj, float height, double animLength)
    {
        //double tmpLength = (maxX - minX) / 614.0f * animLength;
        //animLength -= 25;
        GameObject imgObject = new GameObject("RectBackground");
        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;

        trans.sizeDelta = new Vector2((float)animLength, height); // custom size

        trans.pivot = new Vector2(0.0f, 0.5f);
        //set pivot point of sprite  to left border, so that rect aligns with sprite
        //obj.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 0.5f);
        trans.anchorMin = trans.anchorMax = new Vector2(0, 0.5f);
        trans.anchoredPosition = new Vector2(0, 0);

        //trans.position = obj.transform.position;
        trans.SetSiblingIndex(0);

        Image image = imgObject.AddComponent<Image>();
        image.color = colMusic;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;
        //set pivot point of sprite back to midpoint of sprite
        //obj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
    }
    public int calculateFigureStartTimeInSec(GameObject fig, double animLength, int maxTimeLengthInSec)
    {
        int sec = 0;
        double tmpX = fig.GetComponent<RectTransform>().position.x - minX;  //x-pos is screenX from left border, minX is the rail-startpoint
        Vector2 tmpSize = fig.GetComponent<RectTransform>().sizeDelta;
        double tmpMinX = (double)tmpX - (tmpSize.x / 2.0f); //if tmpX is the midpoint of figure
        //get figure minX related to timelineMinX
        double percentageOfRail = tmpMinX / (maxX - minX);  //max-min=real length, percentage: e.g. 0,3823124 (38%)
        sec = (int)(((double)maxTimeLengthInSec) * percentageOfRail);
        return sec;
    }
    public int calculateMusicEndTimeInSec(GameObject fig, double animLength, int maxTimeLengthInSec)
    {
        int sec = 0;
        double tmpX = fig.GetComponent<RectTransform>().position.x - minX;  //x-pos is screenX from left border, minX is the rail-startpoint
        Vector2 tmpSize = fig.GetComponent<RectTransform>().sizeDelta;
        double tmpMinX = (double)tmpX - (tmpSize.x / 2.0f) + 25; //if tmpX is the midpoint of figure
        //get figure minX related to timelineMinX
        double percentageOfRail = tmpMinX / (maxX - minX);  //max-min=real length, percentage: e.g. 0,3823124 (38%)
        sec = (int)((((double)maxTimeLengthInSec) * percentageOfRail) + animLength);
        return sec;
    }
    public int countCopiesOfObject(GameObject fig, List<GameObject> tlObjs)
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
        Destroy(obj);
        timelineInstanceObjects.Remove(obj);
        figCounterCircle[tmpNr - 1].transform.GetChild(0).GetComponent<Text>().text = (currentCounterNr - 1).ToString();
        StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].musicClipElementInstances.Remove(StaticSceneData.StaticData.musicClipElements[Int32.Parse(obj.name.Substring(6, 2)) - 1].musicClipElementInstances[Int32.Parse(obj.name.Substring(17, 3))]);
    }
    public bool checkHittingTimeline(GameObject obj, Vector2 mousePos)
    {
        bool hit = false;

        Vector2 tlPos = new Vector2(timelineImage.transform.position.x, timelineImage.transform.position.y);
        Vector2 colSize = new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x, timelineImage.GetComponent<BoxCollider2D>().size.y);
        //if mouse hits the timeline while dragging an object
        if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
        ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
        {
            hit = true;
        }
        return hit;
    }
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
    public void CreateNew2DInstance(int musObjNr, float moment)
    {
        //create a copy of this timelineObject and keep the original one
        newCopyOfFigure = Instantiate(figureObjects[musObjNr]);

        //count objects from same kind
        int countName = 0;
        countName = countCopiesOfObject(figureObjects[musObjNr], timelineInstanceObjects);
        newCopyOfFigure.name = figureObjects[musObjNr].name + "_instance" + countName.ToString("000");

        //float tmpLength = ((float)maxX - (float)minX) * newCopyOfFigure.GetComponent<MusicLength>().musicLength / maxTimeInSec;//UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, 614, (float)minX, (float)maxX);

        float tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
        createRectangle(newCopyOfFigure, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, tmpLength);

        scaleObject(newCopyOfFigure, 100, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
        scaleObject(newCopyOfFigure.transform.GetChild(0).gameObject, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);
        newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.x + 26, newCopyOfFigure.transform.GetChild(0).gameObject.GetComponent<RectTransform>().position.y, -1);

        figCounterCircle[musObjNr].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();

        //parent and position
        float posX = UtilitiesTm.FloatRemap(moment, 0, AnimationTimer.GetMaxTime(), gameObject.GetComponent<RectTransform>().rect.width / -2, gameObject.GetComponent<RectTransform>().rect.width / 2);
        newCopyOfFigure.transform.SetParent(gameObject.transform);
        newCopyOfFigure.transform.localPosition = new Vector3(posX, 0, -1);
        scaleObject(newCopyOfFigure.transform.GetChild(1).gameObject, 100, gameObject.GetComponent<RectTransform>().rect.height * 0.96f, false);

        //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
        updateObjectList(timelineInstanceObjects, newCopyOfFigure);
        openTimelineByClick(true, true);
        newCopyOfFigure.transform.localScale = Vector3.one;

        // size of rectangle becomes size for figure that is clickable
        newCopyOfFigure.GetComponent<RectTransform>().position = new Vector3(newCopyOfFigure.GetComponent<RectTransform>().position.x, newCopyOfFigure.GetComponent<RectTransform>().position.y, -1.0f);
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2 - 50, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);
    }
    public void ResetScreenSize()
    {
        minX = 0.087f * Screen.width;// / gameObject.transform.lossyScale.x; //301.0f;  //timeline-minX
        railWidth = 0.87f * Screen.width;           //railwidth=1670.4px; / gameObject.transform.lossyScale.x;
        heightClosed = 0.018f * Screen.height;// / gameObject.transform.lossyScale.x;
        heightOpened = 0.074f * Screen.height;// / gameObject.transform.lossyScale.x;
        maxX = minX + railWidth;  //timeline-maxX
        if (isTimelineOpen)
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightOpened / gameObject.transform.lossyScale.x);
        }
        else
        {
            timelineImage.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<BoxCollider2D>().size = new Vector2(railWidth / gameObject.transform.lossyScale.x, heightClosed / gameObject.transform.lossyScale.x);
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
    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime, GameObject obj, float time)
    {
        fading = true;
        while (audioSource.volume > 0.1f)
        {
            audioSource.volume -= 1 * Time.deltaTime / FadeTime;
            yield return null;
        }
        currentClip = ((int)Char.GetNumericValue(obj.name[07]) - 1); // object index
        audioSource.clip = clip[currentClip];

        StartCoroutine(FadeIn(audioSource, 1, time));
    }
    public IEnumerator FadeIn(AudioSource audioSource, float FadeTime, float time)
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
            double endSec = calculateMusicEndTimeInSec(timelineInstanceObjects[i], timelineInstanceObjects[i].GetComponent<MusicLength>().musicLength, maxTimeInSec);
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

            //if you click the timeline with the mouse
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
            {
                //open or close timeline
                openTimelineByClick(isTimelineOpen, false);
            }

            //if you click on an object in shelf
            //first checking if you have an object clicked in shelf
            if (currentClickedObjectIndex != (-1))      //is set in the identify-methods
            {
                if(!SceneManaging.sceneChanged) SceneManaging.sceneChanged = true;
                if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
                    //set up some flags
                    draggingObject = true;
                }
            }
            //or check if you click an object in timeline
            if ((currentClickedInstanceObjectIndex != (-1)) && (editTimelineObject == true))
            {
                if(!SceneManaging.sceneChanged) SceneManaging.sceneChanged = true;
                diff = new Vector2(getMousePos.x - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, getMousePos.y - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.y);
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(getMousePos))
                {
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
            if (Physics2D.OverlapPoint(getMousePos) == false)
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
                isInstance = true;
                //if you click an object in timeline (for dragging) //move object
                updateObjectPosition(timelineInstanceObjects[currentClickedInstanceObjectIndex], getMousePos - diff);
                //snapping/lock y-axis
                setObjectOnTimeline(timelineInstanceObjects[currentClickedInstanceObjectIndex], timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.position.x, this.transform.position.y);

                //-------------------------------------------------limit front of rail------------------------------------------//
                if (timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x <= 0) // (0.03f * Screen.width))  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
                {
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.y, -1);    // tendenziell muesste das eher in buttonUp
                }

                //-------------------------------------------------limit back of rail------------------------------------------//
                if ((timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.x + timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<BoxCollider2D>().size.x) > (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width))
                {
                    timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - timelineInstanceObjects[currentClickedInstanceObjectIndex].transform.GetComponent<BoxCollider2D>().size.x), timelineInstanceObjects[currentClickedInstanceObjectIndex].GetComponent<RectTransform>().anchoredPosition.y, -1);
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
            updateObjectPosition(figureObjects[currentClickedObjectIndex], getMousePos);

            //temporarily change parent, so that object appears in front of the shelf
            figureObjects[currentClickedObjectIndex].transform.SetParent(parentMenue.transform);

            //if you hit the timeline > object snap to timeline and is locked in y-movement-direction
            bool hitTimeline = false;
            hitTimeline = checkHittingTimeline(figureObjects[currentClickedObjectIndex], timelineImage, getMousePos);

            if (hitTimeline)
            {
                // change parent back
                setParent(figureObjects[currentClickedObjectIndex], gameObject);

                //snapping/lock y-axis
                setObjectOnTimeline(figureObjects[currentClickedObjectIndex], figureObjects[currentClickedObjectIndex].transform.position.x, this.transform.position.y);

                scaleObject(figureObjects[currentClickedObjectIndex], 100, timelineImage.GetComponent<RectTransform>().sizeDelta.y, false);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, 100, timelineImage.GetComponent<RectTransform>().sizeDelta.y, false);

                //set placed figures to timelineobjects-list
                updateObjectList(timelineObjects, figureObjects[currentClickedObjectIndex]);

                //save position, where object on timeline is released + set flag
                releaseOnTimeline = true;
                releaseObjMousePos = new Vector2(getMousePos.x, getMousePos.y);
            }
            else
            {
                scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
                scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
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
                    //create a copy of this timelineObject and keep the original one
                    newCopyOfFigure = Instantiate(figureObjects[currentClickedObjectIndex]);
                    //count objects from same kind
                    int countName = 0;
                    countName = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    newCopyOfFigure.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");

                    float tmpLength = UtilitiesTm.FloatRemap(newCopyOfFigure.GetComponent<MusicLength>().musicLength, 0, AnimationTimer.GetMaxTime(), 0, (float)gameObject.GetComponent<RectTransform>().rect.width);
                    createRectangle(newCopyOfFigure, gameObject.GetComponent<RectTransform>().rect.height, tmpLength);

                    figCounterCircle[currentClickedObjectIndex].transform.GetChild(0).GetComponent<Text>().text = (countName + 1).ToString();

                    //parent and position
                    newCopyOfFigure.transform.SetParent(gameObject.transform);
                    newCopyOfFigure.transform.localScale = Vector3.one;

                    //------------------------------------------limit front of rail---------------------------------------------//
                    if (figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().position.x < (minX + 0.03f * Screen.width))  // 50 is half the box Collider width (mouse pos is in the middle of the figure)
                    {
                        newCopyOfFigure.GetComponent<RectTransform>().position = new Vector3((float)minX + 0.03f * Screen.width, figureObjects[currentClickedObjectIndex].transform.position.y, -1);    // tendenziell muesste das eher in buttonUp
                    }
                    else
                    {
                        newCopyOfFigure.transform.position = new Vector3(figureObjects[currentClickedObjectIndex].transform.position.x, figureObjects[currentClickedObjectIndex].transform.position.y, -1);
                    }

                    //------------------------------------------limit back of rail---------------------------------------------//
                    if ((newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.x + newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x) > (GetComponent<RectTransform>().rect.width + 0.03f * Screen.width))
                    {
                        newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + (0.03f * Screen.width) - newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x), newCopyOfFigure.GetComponent<RectTransform>().anchoredPosition.y, -1);
                    }

                    // size of rectangle becomes size for figure that is clickable
                    newCopyOfFigure.GetComponent<RectTransform>().localPosition = new Vector3(newCopyOfFigure.GetComponent<RectTransform>().localPosition.x, newCopyOfFigure.GetComponent<RectTransform>().localPosition.y, -1.0f);
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size = newCopyOfFigure.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
                    newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset = new Vector2(newCopyOfFigure.transform.GetComponent<BoxCollider2D>().size.x / 2 - 50, newCopyOfFigure.transform.GetComponent<BoxCollider2D>().offset.y);

                    newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, newCopyOfFigure.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y);
                    
                    //add object to list which objects are on timeline, set placed figures to timelineInstanceObjects-list
                    updateObjectList(timelineInstanceObjects, newCopyOfFigure);
                    //set original image back to shelf
                    setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                    //scale to default values
                    scaleObject(figureObjects[currentClickedObjectIndex], objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);
                    scaleObject(figureObjects[currentClickedObjectIndex].transform.GetChild(0).gameObject, objectShelfSize[currentClickedObjectIndex].x, objectShelfSize[currentClickedObjectIndex].y, false);

                    //set this position
                    figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(95.0f, -73.0f);
                    // bring object back to position two, so that its visible
                    figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Save to SceneData:
                    MusicClipElementInstance musicClipElementInstance = new MusicClipElementInstance();
                    musicClipElementInstance.instanceNr = countCopiesOfObject(figureObjects[currentClickedObjectIndex], timelineInstanceObjects);
                    musicClipElementInstance.name = figureObjects[currentClickedObjectIndex].name + "_instance" + countName.ToString("000");
                    StaticSceneData.StaticData.musicClipElements[currentClickedObjectIndex].musicClipElementInstances.Add(musicClipElementInstance);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////

                    // set index back to -1 because nothing is being clicked anymore
                    currentClickedObjectIndex = -1;
                }

            }
            // if dropped somewhere else than timeline: bring figure back to shelf
            else if (releaseOnTimeline == false && currentClickedObjectIndex != -1 && isInstance == false)
            {
                setParent(figureObjects[currentClickedObjectIndex], objectShelfParent[currentClickedObjectIndex]);
                // bring object back to position two, so that its visible and change position back to default
                figureObjects[currentClickedObjectIndex].transform.SetSiblingIndex(1);
                figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(95.0f, -73.0f);
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
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            releaseOnTimeline = false;
            releaseObjMousePos.x = 0.0f;
            releaseObjMousePos.y = 0.0f;
            draggingOnTimeline = false;
            isInstance = false;
        }

        // turning music on and off in playmode
        if (SceneManaging.playing)
        {
            if (GetCurrentMusicCount(out int m, out int n, out int o) == 1)
            {
                double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[m], timelineInstanceObjects[m].GetComponent<MusicLength>().musicLength, maxTimeInSec);
                //Debug.Log("start: " + startSec);
                double endSec = calculateMusicEndTimeInSec(timelineInstanceObjects[m], timelineInstanceObjects[m].GetComponent<MusicLength>().musicLength, maxTimeInSec);
                float tmpTime = AnimationTimer.GetTime();

                anyInstanceIsPlaying = true;

                if (playingMusic == false)
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
                firstTimeSecond = firstTimeThird = false;
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

                if (playingMusic == false)
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
                firstTimeThird = false;
            }

            else if (GetCurrentMusicCount(out m, out n, out o) == 3)
            {
                if (!firstTimeThird)
                {
                    fading = false;
                    firstTimeThird = true;
                }
                double startSec = calculateFigureStartTimeInSec(timelineInstanceObjects[o], timelineInstanceObjects[o].GetComponent<MusicLength>().musicLength, maxTimeInSec);
                float tmpTime = AnimationTimer.GetTime();

                if (!fading) StartCoroutine(FadeOut(audioSource, 1, timelineInstanceObjects[o], (tmpTime - (float)startSec)));
                anyInstanceIsPlaying = true;

                if (playingMusic == false)
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
            }
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
            firstTimeSecond = firstTimeThird = false;
        }

        if (!anyInstanceIsPlaying && playingSample == false)
        {
            audioSource.Stop();
            playingMusic = false;
            firstTimeSecond = firstTimeThird = false;
        }
    }
}