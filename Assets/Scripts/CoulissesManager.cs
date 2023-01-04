using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoulissesManager : MonoBehaviour
{
    #region public variables
    public GameObject gameController;
    public GameObject[] indexTabs, collections, indexTabImages;
    public GameObject scenerySettings, mainMenue, textPositionZCoulisses, colliderSettings, colliderRailImage;
    public GameObject deleteButton, sliderX, sliderY, sliderPosX, sliderPosY;
    public Text sceneryName;
    public List<GameObject> coulissesOnRails;
    [HideInInspector] public GameObject[] coulisses;
    [HideInInspector] public float railWidth, railHeight;
    [HideInInspector] public GameObject[] parentStart;
    [HideInInspector] public float[] shelfSizeWidth, shelfSizeHeight;
    [HideInInspector] public bool[] isWide, isHighlighted, isMirrored;

    #endregion
    #region private variables
    [SerializeField] GameObject scrollViewScenery;
    int currentObjectIndex, clickInSettingsWindow;
    bool dragging;
    private bool sliding;
    private Color colHighlightedGrey, colHighlightedGreen, colSilhouetteActive, colCoulisse, colSilhouette;
    private bool justChanging, objectInField;
    private int currentTabIndex, publicSibling;
    private bool objectInRail, clickOnDelete, clickCoulisseFromShelf;
    private float railMinX, windowWidth, windowHeight, windowMinX;
    Vector2 diff;
    private float railMinY, windowMinY, currentLossyScale;
    #endregion
    void Awake()
    {
        SceneManaging.objectInIndexTab = -1;
        coulisses = gameController.GetComponent<UIController>().goButtonSceneryElements;
        currentObjectIndex = -1;
        scenerySettings.SetActive(false);
        deleteButton.SetActive(false);
        shelfSizeWidth = new float[coulisses.Length];
        shelfSizeHeight = new float[coulisses.Length];
        parentStart = new GameObject[coulisses.Length];
        isWide = new bool[coulisses.Length];
        isHighlighted = new bool[coulisses.Length];
        isMirrored = new bool[coulisses.Length];
        currentLossyScale = 1.0f;

        // width and height of coulisses in shelf - saved in the arrays 'isWide', 'shelfSizeWidth' and 'shelfSizeHeight'
        for (int i = 0; i < coulisses.Length; i++)
        {
            if (coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth < coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight)
            {
                isWide[i] = false;
                shelfSizeWidth[i] = Screen.width * 0.104f / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth / coulisses[i].transform.lossyScale.x;
                shelfSizeHeight[i] = Screen.height * 0.185f / coulisses[i].transform.lossyScale.y;
                coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.104f / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth / coulisses[i].transform.lossyScale.x, Screen.height * 0.185f / coulisses[i].transform.lossyScale.x);
                coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], Screen.height * 0.185f / coulisses[i].transform.lossyScale.x);
                coulisses[i].GetComponent<BoxCollider2D>().size = new Vector2(shelfSizeWidth[i], Screen.height * 0.185f);
            }
            else
            {
                isWide[i] = true;
                shelfSizeHeight[i] = Screen.height * 0.185f / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight / coulisses[i].transform.lossyScale.x;
                shelfSizeWidth[i] = Screen.width * 0.104f / coulisses[i].transform.lossyScale.x;
                coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.104f / coulisses[i].transform.lossyScale.x, Screen.height * 0.185f / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight / coulisses[i].transform.lossyScale.x);
                coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.104f / coulisses[i].transform.lossyScale.x, shelfSizeHeight[i]);
                coulisses[i].GetComponent<BoxCollider2D>().size = new Vector2(Screen.width * 0.104f, shelfSizeHeight[i]);
            }
            parentStart[i] = coulisses[i].transform.parent.gameObject;
            coulisses[i].GetComponent<RectTransform>().localPosition = new Vector2(0.0f, -coulisses[i].GetComponent<RectTransform>().rect.height / 2);
            coulisses[i].GetComponent<BoxCollider2D>().offset = new Vector2(0, -coulisses[i].GetComponent<RectTransform>().localPosition.y);
        }
    }
    void Start()
    {
        colHighlightedGrey = new Color(.5f, .5f, 0.5f, .7f);    // falls doch rote farbe: 1f, .45f, 0.33f, 1f
        colHighlightedGreen = new Color(0f, .8f, 0f, .7f);
        colCoulisse = new Color(1f, 1f, 1f, 1f);
        colSilhouette = new Color(0f, 0f, 0f, 0.4f);
        colSilhouetteActive = new Color(0.6f, 0f, 0f, 0.4f);

        ResetScreenSize();

        setIndexTabActive(0);
        //mainMenue.GetComponent<RectTransform>().pivot = collections[0].GetComponent<RectTransform>().pivot;
        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
    }
    void Update()
    {
        Vector2 getMousePos = Input.mousePosition;

        // if resolution has been changed and lossyScale is different than before (doesn't happen exactly when Screen Size is reset...)
        if (currentLossyScale != coulisses[0].transform.lossyScale.x)
        {
            currentLossyScale = coulisses[0].transform.lossyScale.x;
            for (int i = 0; i < coulisses.Length; i++)
            {
                if (!isWide[i])
                {
                    shelfSizeWidth[i] = Screen.width * 0.104f / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth / coulisses[i].transform.lossyScale.x;
                    shelfSizeHeight[i] = shelfSizeWidth[i] * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth;

                    coulisses[i].GetComponent<RectTransform>().sizeDelta = coulisses[i].GetComponent<BoxCollider2D>().size = coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], shelfSizeHeight[i]);
                    if (!coulissesOnRails.Contains(coulisses[i]))
                    {
                        coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition.x, -100);
                    }
                }
                else
                {
                    shelfSizeWidth[i] = Screen.width * 0.104f / coulisses[i].transform.lossyScale.x;
                    shelfSizeHeight[i] = shelfSizeWidth[i] / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight;

                    coulisses[i].GetComponent<RectTransform>().sizeDelta = coulisses[i].GetComponent<BoxCollider2D>().size = coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], shelfSizeHeight[i]);
                    if (!coulissesOnRails.Contains(coulisses[i]))
                    {
                        coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition.x, -shelfSizeHeight[i] / 2);
                    }
                }
            }
            for (int j = 0; j < coulissesOnRails.Count; j++)
            {
                coulissesOnRails[j].GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / 410 * coulissesOnRails[j].GetComponent<CoulisseStats>().CoulisseWidth / coulissesOnRails[j].transform.lossyScale.x, railWidth / 410 * coulissesOnRails[j].GetComponent<CoulisseStats>().CoulisseHeight / coulissesOnRails[j].transform.lossyScale.y);
                coulissesOnRails[j].GetComponent<BoxCollider2D>().size = coulissesOnRails[j].GetComponent<RectTransform>().sizeDelta;
                coulissesOnRails[j].GetComponent<BoxCollider2D>().offset = new Vector2(0, coulissesOnRails[j].GetComponent<RectTransform>().rect.height / 2);
            }
        }
        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            // if indexTab is clicked
            for (int i = 0; i < indexTabs.Length; i++)
            {
                if (indexTabs[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
                {
                    if (isAnythingHighlighted())
                    {
                        for (int j = 0; j < coulissesOnRails.Count; j++)
                        {
                            int k = int.Parse(coulissesOnRails[j].name.Substring(8, 2)) - 1;
                            highlight(k, 0);
                        }
                    }
                    setIndexTabActive(i);
                }
            }

            clickInSettingsWindow = isSettingsWindowClicked();
            clickOnDelete = isDeleteButtonClicked();

            // if settingswindow (feineinstellungen) is clicked
            if (clickInSettingsWindow != -1)
            {
                sliding = true;
            }
            // if delete button is clicked
            else if (clickOnDelete)
            {
                removeCoulisse();
            }
            else
            {
                currentObjectIndex = identifyClickedObjectIndex();
            }

            // if a coulisse is clicked
            if (currentObjectIndex != -1 && clickInSettingsWindow == -1)
            {
                scrollViewScenery.GetComponent<ScrollRect>().enabled = false;
                diff = new Vector2(getMousePos.x - coulisses[currentObjectIndex].transform.position.x, getMousePos.y - coulisses[currentObjectIndex].transform.position.y);
                dragging = true;
                if (isAnythingHighlighted())
                {
                    for (int i = 0; i < coulissesOnRails.Count; i++)
                    {
                        int j = int.Parse(coulissesOnRails[i].name.Substring(8, 2)) - 1;
                        highlight(j, 0);
                    }
                    highlight(currentObjectIndex, 2);
                    showDeleteButton(deleteButton, coulisses[currentObjectIndex], false);
                }
                else
                {
                    highlight(currentObjectIndex, 2);
                    showDeleteButton(deleteButton, coulisses[currentObjectIndex], false);
                }
                // if coulisse is clicked in shelf ('first time')
                if (coulisses[currentObjectIndex].GetComponent<RectTransform>().rect.width.ToString().Substring(0, 5) == shelfSizeWidth[currentObjectIndex].ToString().Substring(0, 5))
                {
                    coulisses[currentObjectIndex].GetComponent<RectTransform>().sizeDelta = coulisses[currentObjectIndex].GetComponent<BoxCollider2D>().size = new Vector2(railWidth / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseWidth / coulisses[currentObjectIndex].transform.lossyScale.x, railWidth / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseHeight / coulisses[currentObjectIndex].transform.lossyScale.y);
                    coulisses[currentObjectIndex].GetComponent<BoxCollider2D>().offset = new Vector2(0, coulisses[currentObjectIndex].GetComponent<RectTransform>().rect.height / 2);
                    clickCoulisseFromShelf = true;
                }
                publicSibling = coulisses[currentObjectIndex].transform.GetSiblingIndex();
                textPositionZCoulisses.GetComponent<Text>().text = (publicSibling + 1) + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
                coulisses[currentObjectIndex].transform.SetParent(mainMenue.transform);
            }

            else if (currentObjectIndex == -1 && clickInSettingsWindow == -1) // if nothing is hit
            {
                for (int i = 0; i < coulissesOnRails.Count; i++)
                {
                    int j = int.Parse(coulissesOnRails[i].name.Substring(8, 2)) - 1;
                    highlight(j, 0);
                }
                StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
            }
        }

        if (dragging)
        {
            if (coulisses[currentObjectIndex].GetComponent<RectTransform>().rect.width == shelfSizeWidth[currentObjectIndex])
            {
                coulisses[currentObjectIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseWidth / coulisses[currentObjectIndex].transform.lossyScale.x, railWidth / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseHeight / coulisses[currentObjectIndex].transform.lossyScale.y);
            }
            coulisses[currentObjectIndex].transform.position = new Vector2(getMousePos.x - diff.x, getMousePos.y - diff.y); // diff is difference of mouse position to clicked position on coulisse so that the coulisse doesnt jump to the pivot all the time
            coulisses[currentObjectIndex].transform.SetParent(mainMenue.transform);
            // if coulisse is hitting a tab
            int hitIndexTab = checkHittingIndexTab(indexTabs, getMousePos);
            if (hitIndexTab != -1)
            {
                SceneManaging.objectInIndexTab = hitIndexTab;
                if (SceneManaging.openUp)
                {
                    setIndexTabActive(hitIndexTab);
                }
            }
            // window or rail tab is hit
            else if (checkHittingSettings(coulisses[currentObjectIndex]))
            {
                if (clickCoulisseFromShelf)
                {
                    coulisses[currentObjectIndex].GetComponent<Image>().color = colHighlightedGreen;
                }
                else
                {
                    highlight(currentObjectIndex, 2);
                    showDeleteButton(deleteButton, coulisses[currentObjectIndex], false);
                    sliderX.GetComponent<Slider>().value = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
                    sliderY.GetComponent<Slider>().value = (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y / 260) + 0.515f;
                }
                justChanging = true;
                objectInField = true;
                objectInRail = false;
                if (checkHitting(colliderRailImage.GetComponent<RectTransform>().anchoredPosition, colliderRailImage.GetComponent<RectTransform>().sizeDelta, coulisses[currentObjectIndex]))
                {
                    objectInRail = true;
                }
            }
            // nothing is hit
            else
            {
                highlight(currentObjectIndex, 1);
                objectInField = false;
                SceneManaging.objectInIndexTab = -1;
                objectInRail = false;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y / 260 + 0.515f;
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].x = 0.062f;
            StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
            ////////////////////////////////////////////////////////////////////////////////////////////
        }

        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            dragging = false;
            scrollViewScenery.GetComponent<ScrollRect>().enabled = true;

            if (currentObjectIndex != -1)
            {
                if (sliding)
                {
                    //////////////////////////////////////////////////////////////////////////
                    // all coulisses need to be organised if one of the siblings changed
                    for (int i = 0; i < coulisses[currentObjectIndex].transform.parent.transform.childCount; i++)
                    {
                        StaticSceneData.StaticData.sceneryElements[int.Parse(coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).name.Substring(8, 2)) - 1].zPos = coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).GetSiblingIndex();
                    }

                    //write Text: layer of current coulisse
                    textPositionZCoulisses.GetComponent<Text>().text = (coulisses[currentObjectIndex].transform.GetSiblingIndex() + 1) + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y) / 260 + .515f;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].x = 0.062f;
                    StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements

                    ////////////////////////////////////////////////////////////////////////
                }
                // if object is in settingsField
                else if (objectInField)
                {
                    for (int i = 0; i < collections.Length; i++)
                    {
                        // if object is on rail image
                        if (objectInRail)
                        {
                            if (currentTabIndex == i)
                            {
                                coulisses[currentObjectIndex].transform.SetParent(collections[i].transform);
                            }
                        }
                        else
                        {
                            if (currentTabIndex == i)
                            {
                                coulisses[currentObjectIndex].transform.SetParent(collections[i].transform);
                                coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x, -130);
                            }
                        }
                        // limitierungen (snapping if dropped on the left/right of the min/max-value)
                        if (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x < -540)
                        {
                            coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(-540, coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y);
                        }
                        if (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x > 540)
                        {
                            coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(540, coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y);
                        }
                        if (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y < -260)
                        {
                            coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x, -260);
                        }
                        if (!scenerySettings.activeSelf)
                        {
                            highlight(currentObjectIndex, 2);
                        }
                    }
                    showDeleteButton(deleteButton, coulisses[currentObjectIndex], true);

                    //publicSibling = coulisses[currentObjectIndex].transform.GetSiblingIndex();
                    if (clickCoulisseFromShelf)
                    {
                        int tmpCount = collections[currentTabIndex].transform.childCount;
                        textPositionZCoulisses.GetComponent<Text>().text = tmpCount + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
                        coulisses[currentObjectIndex].transform.SetSiblingIndex(tmpCount);
                    }
                    else
                    {
                        coulisses[currentObjectIndex].transform.SetSiblingIndex(publicSibling);
                        textPositionZCoulisses.GetComponent<Text>().text = (publicSibling + 1) + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
                    }
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].active = true;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].parent = "Schiene" + (currentTabIndex + 1).ToString();
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].railnumber = currentTabIndex + 1;
                    sliderX.GetComponent<Slider>().value = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
                    sliderY.GetComponent<Slider>().value = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y / 260 + 0.515f;

                    if (!coulissesOnRails.Contains(coulisses[currentObjectIndex])) coulissesOnRails.Add(coulisses[currentObjectIndex]);

                    //////////////////////////////////////////////////////////////////////////
                    // all coulisses need to be organised if one of the siblings changed
                    for (int i = 0; i < coulisses[currentObjectIndex].transform.parent.transform.childCount; i++)
                    {
                        StaticSceneData.StaticData.sceneryElements[int.Parse(coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).name.Substring(8, 2)) - 1].zPos = coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).GetSiblingIndex();
                        //textPositionZCoulisses.GetComponent<Text>().text = (coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).GetSiblingIndex() + 1) + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
                    }
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y) / 260 + .515f;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].x = 0.062f;
                    StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
                    ////////////////////////////////////////////////////////////////////////
                }

                // if object is on a tab
                else if (SceneManaging.objectInIndexTab != -1)
                {
                    coulisses[currentObjectIndex].transform.SetParent(collections[SceneManaging.objectInIndexTab].transform);
                    coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(0.0f, 0.0f);
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].active = true;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].parent = "Schiene" + (currentTabIndex + 1).ToString();
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].railnumber = currentTabIndex + 1;
                    sliderX.GetComponent<Slider>().value = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
                    sliderY.GetComponent<Slider>().value = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y / 260 + 0.515f;//+ 1;
                    coulissesOnRails.Add(coulisses[currentObjectIndex]);
                    showDeleteButton(deleteButton, coulisses[currentObjectIndex], true);
                    //////////////////////////////////////////////////////////////////////////
                    // all coulisses need to be organised if one of the siblings changed
                    for (int i = 0; i < coulisses[currentObjectIndex].transform.parent.transform.childCount; i++)
                    {
                        StaticSceneData.StaticData.sceneryElements[int.Parse(coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).name.Substring(8, 2)) - 1].zPos = coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).GetSiblingIndex();
                        textPositionZCoulisses.GetComponent<Text>().text = (coulisses[currentObjectIndex].transform.parent.transform.GetChild(i).GetSiblingIndex() + 1) + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
                    }
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y) / 260 - .035f;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].x = 0.062f;
                    StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
                    ////////////////////////////////////////////////////////////////////////
                }
                // if object is outside the window (back to shelf)
                else
                {
                    placeInShelf(currentObjectIndex);
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].active = false;
                }
                // set default values since nothing is clicked anymore
                objectInField = false;
                SceneManaging.objectInIndexTab = -1;
                objectInRail = false;
            }
            sliding = false;
            justChanging = false;
            clickCoulisseFromShelf = false;
        }
    }
    public void ChangeElementPositionX()
    {
        //2D-Kulisse
        sliderPosX.GetComponent<InputField>().text = sliderX.GetComponent<Slider>().value.ToString();
        if (justChanging == false)
        {
            coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(sliderX.GetComponent<Slider>().value * 270, coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y);
        }
        //3D-Kulisse
        StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = sliderX.GetComponent<RectTransform>().GetComponent<Slider>().value;
        StaticSceneData.Sceneries3D();
    }
    public void ChangeElementPositionY()
    {
        //2D-Kulisse
        sliderPosY.GetComponent<InputField>().text = sliderY.GetComponent<Slider>().value.ToString();
        coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x, (sliderY.GetComponent<Slider>().value - 0.515f) * 260);

        //3D-Kulisse
        StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = sliderY.GetComponent<RectTransform>().GetComponent<Slider>().value;
        StaticSceneData.Sceneries3D();
    }
    public void InputValuePosition(bool height)
    {
        if (height)
        {
            Debug.Log("text hoehe eingegeben"+float.Parse(sliderPosY.GetComponent<InputField>().text));
            //2D-Kulisse
            sliderY.GetComponent<Slider>().value = float.Parse(sliderPosY.GetComponent<InputField>().text);
            coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x, (sliderY.GetComponent<Slider>().value - 0.515f) * 260);

            //3D-Kulisse
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = sliderY.GetComponent<RectTransform>().GetComponent<Slider>().value;
            StaticSceneData.Sceneries3D();
        }
        else
        {
            Debug.Log("text links rechts eingegeben"+float.Parse(sliderPosX.GetComponent<InputField>().text));
            //2D-Kulisse
            sliderX.GetComponent<Slider>().value = float.Parse(sliderPosX.GetComponent<InputField>().text);
            if (justChanging == false)
            {
                coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(sliderX.GetComponent<Slider>().value * 270, coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y);
            }
            //3D-Kulisse
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = sliderX.GetComponent<RectTransform>().GetComponent<Slider>().value;
            StaticSceneData.Sceneries3D();
        }
    }
    private bool checkHitting(Vector2 pos1, Vector2 rect, GameObject obj2)
    {
        bool hit = false;

        Vector2 pos2 = new Vector2(obj2.GetComponent<RectTransform>().anchoredPosition.x, obj2.GetComponent<RectTransform>().anchoredPosition.y);
        Vector2 collider2 = new Vector2(obj2.GetComponent<RectTransform>().rect.width, obj2.GetComponent<RectTransform>().rect.height);
        if (pos2.x - collider2.x / 2.0f <= pos1.x + (rect.x / 2.0f) && pos2.y <= pos1.y + rect.y / 2
        && pos2.x + collider2.x / 2.0f >= pos1.x - rect.x / 2.0f && pos2.y + collider2.y >= pos1.y - rect.y / 2)
        {
            hit = true;
        }
        return hit;
    }
    private bool checkHittingSettings(GameObject obj2)
    {
        bool hit = false;
        if (obj2.GetComponent<RectTransform>().anchoredPosition.x - (obj2.GetComponent<RectTransform>().sizeDelta.x / 2) <= mainMenue.GetComponent<RectTransform>().sizeDelta.x + (obj2.GetComponent<RectTransform>().sizeDelta.x / 2) / 2 && obj2.GetComponent<RectTransform>().anchoredPosition.x + (obj2.GetComponent<RectTransform>().sizeDelta.x / 2) >= -(mainMenue.GetComponent<RectTransform>().sizeDelta.x / 2)
        && obj2.GetComponent<RectTransform>().anchoredPosition.y <= mainMenue.GetComponent<RectTransform>().sizeDelta.y / 2 && obj2.GetComponent<RectTransform>().anchoredPosition.y + obj2.GetComponent<RectTransform>().sizeDelta.y >= -(mainMenue.GetComponent<RectTransform>().sizeDelta.y / 2))
        {
            hit = true;
        }
        return hit;
    }
    private int checkHittingIndexTab(GameObject[] tabs, Vector2 mousePos)
    {
        int hit = -1;
        //calculate bounding-box related to the indexTab-pos
        for (int i = 0; i < tabs.Length; i++)
        {
            Vector2 tlPos = new Vector2(tabs[i].transform.position.x, tabs[i].transform.position.y);
            Vector2 colSize = new Vector2(tabs[i].GetComponent<BoxCollider2D>().size.x, tabs[i].GetComponent<BoxCollider2D>().size.y);
            //my implementation of object-boundingbox
            if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
            ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
            {
                hit = i;
            }
        }
        return hit;
    }
    private void highlight(int i, int color) // color: 0 = unhighlight ; 1 = highlight grey ; 2 = highlight green
    {
        if (color == 1)
        {
            coulisses[i].GetComponent<Image>().color = colHighlightedGrey;
            scenerySettings.SetActive(false);
            showDeleteButton(deleteButton, coulisses[i], false);
        }
        else if (color == 2)
        {
            coulisses[i].GetComponent<Image>().color = colHighlightedGreen;
            scenerySettings.SetActive(true);
            showDeleteButton(deleteButton, coulisses[i], true);
            sceneryName.text = coulisses[i].GetComponent<CoulisseStats>().description;
            //StaticSceneData.StaticData.sceneryElements[i].emission = true;
            StaticSceneData.StaticData.sceneryElements[i].outline = true;
            isHighlighted[i] = true;
        }
        else
        {
            coulisses[i].GetComponent<Image>().color = colCoulisse;
            showDeleteButton(deleteButton, coulisses[i], false);
            StaticSceneData.StaticData.sceneryElements[i].outline = false;
            isHighlighted[i] = false;
            scenerySettings.SetActive(false);
        }
    }
    private int identifyClickedObjectIndex()
    {
        int index = -1;
        for (int j = 0; j < coulisses.Length; j++)
        {
            if (coulisses[j].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
            {
                index = j;
            }
        }
        return index;
    }
    private bool isDeleteButtonClicked()
    {
        bool clicked = false;
        if (deleteButton.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
        {
            clicked = true;
        }
        return clicked;
    }
    private bool isAnythingHighlighted()
    {
        bool val = false;
        for (int i = 0; i < coulissesOnRails.Count; i++)
        {
            int j = int.Parse(coulissesOnRails[i].name.Substring(8, 2)) - 1;
            if (isHighlighted[j])
            {
                val = true;
            }
        }
        return val;
    }
    private int isSettingsWindowClicked()
    {
        int index = currentObjectIndex;
        if (scenerySettings.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
        {
        }
        else index = -1;
        return index;
    }
    private void mirrorObject()
    {
        if (isMirrored[currentObjectIndex])
        {
            isMirrored[currentObjectIndex] = false;
            coulisses[currentObjectIndex].GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].mirrored = false;
        }
        else
        {
            isMirrored[currentObjectIndex] = true;
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].mirrored = true;
            coulisses[currentObjectIndex].GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
        StaticSceneData.Sceneries3D();
    }
    private void pressPlus()
    {
        if (coulisses[currentObjectIndex].transform.GetSiblingIndex() < coulisses[currentObjectIndex].transform.parent.transform.childCount - 1)
        {
            coulisses[currentObjectIndex].transform.SetSiblingIndex(coulisses[currentObjectIndex].transform.GetSiblingIndex() + 1);
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].zPos = coulisses[currentObjectIndex].transform.GetSiblingIndex();
            textPositionZCoulisses.GetComponent<Text>().text = (coulisses[currentObjectIndex].transform.GetSiblingIndex() + 1) + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
            StaticSceneData.Sceneries3D();
        }

    }
    private void pressMinus()
    {
        if (coulisses[currentObjectIndex].transform.GetSiblingIndex() > 0)
        {
            coulisses[currentObjectIndex].transform.SetSiblingIndex(coulisses[currentObjectIndex].transform.GetSiblingIndex() - 1);
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].zPos = coulisses[currentObjectIndex].transform.GetSiblingIndex();
            textPositionZCoulisses.GetComponent<Text>().text = (coulisses[currentObjectIndex].transform.GetSiblingIndex() + 1) + "/" + coulisses[currentObjectIndex].transform.parent.transform.childCount;
            StaticSceneData.Sceneries3D();
        }
    }
    public void placeInShelf(int i)
    {
        coulisses[i].transform.SetParent(parentStart[i].transform);
        coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], shelfSizeHeight[i]);
        coulisses[i].GetComponent<RectTransform>().localPosition = new Vector2(0.0f, -(coulisses[i].GetComponent<RectTransform>().rect.height / 2));
        coulisses[i].GetComponent<BoxCollider2D>().offset = new Vector2(0, -coulisses[i].GetComponent<RectTransform>().localPosition.y);
        coulisses[i].GetComponent<BoxCollider2D>().size = new Vector2(shelfSizeWidth[i], shelfSizeHeight[i]);
        highlight(i, 0);
        if (coulissesOnRails.Contains(coulisses[i]))
        {
            coulissesOnRails.Remove(coulisses[i]);
        }
    }
    public void ResetScreenSize()
    {
        railWidth = 0.586f * Screen.width;
        railMinX = (0.68f * Screen.width) - (railWidth / 2);
        railHeight = 0.093f * Screen.height;
        railMinY = 0.07f * Screen.height - (railHeight / 2);
        mainMenue.transform.position = colliderSettings.transform.position;
    }
    private void removeCoulisse()
    {
        int i = int.Parse(deleteButton.transform.parent.name.Substring(8, 2)) - 1;
        placeInShelf(i);
    }
    private void showDeleteButton(GameObject deleteButton, GameObject parent, bool show)
    {
        if (show)
        {
            deleteButton.SetActive(true);
            deleteButton.transform.SetParent(parent.transform);
            deleteButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-55.0f, 89.0f);
        }
        else
        {
            deleteButton.SetActive(false);
        }
    }
    private void setIndexTabActive(int currentIndex)
    {
        for (int i = 0; i < indexTabs.Length; i++)
        {
            collections[i].SetActive(false);
            indexTabImages[i].SetActive(false);
        }
        collections[currentIndex].SetActive(true);
        indexTabImages[currentIndex].SetActive(true);
        currentTabIndex = currentIndex;
    }
    public void PublicUpdate()
    {
        Debug.Log("hello");
        for (int i = 0; i < coulisses.Length; i++)
        {
            if (!isWide[i])
            {
                shelfSizeWidth[i] = Screen.width * 0.104f / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth / coulisses[i].transform.lossyScale.x;
                shelfSizeHeight[i] = shelfSizeWidth[i] * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth;

                coulisses[i].GetComponent<RectTransform>().sizeDelta = coulisses[i].GetComponent<BoxCollider2D>().size = coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], shelfSizeHeight[i]);
                if (!coulissesOnRails.Contains(coulisses[i]))
                {
                    coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition.x, -100);
                }
            }
            else
            {
                shelfSizeWidth[i] = Screen.width * 0.104f / coulisses[i].transform.lossyScale.x;
                shelfSizeHeight[i] = shelfSizeWidth[i] / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight;

                coulisses[i].GetComponent<RectTransform>().sizeDelta = coulisses[i].GetComponent<BoxCollider2D>().size = coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], shelfSizeHeight[i]);
                if (!coulissesOnRails.Contains(coulisses[i]))
                {
                    coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(coulisses[i].transform.GetComponent<RectTransform>().anchoredPosition.x, -shelfSizeHeight[i] / 2);
                }
            }
        }
        for (int j = 0; j < coulissesOnRails.Count; j++)
        {
            coulissesOnRails[j].GetComponent<RectTransform>().sizeDelta = new Vector2(railWidth / 410 * coulissesOnRails[j].GetComponent<CoulisseStats>().CoulisseWidth / coulissesOnRails[j].transform.lossyScale.x, railWidth / 410 * coulissesOnRails[j].GetComponent<CoulisseStats>().CoulisseHeight / coulissesOnRails[j].transform.lossyScale.y);
            coulissesOnRails[j].GetComponent<BoxCollider2D>().size = coulissesOnRails[j].GetComponent<RectTransform>().sizeDelta;
            coulissesOnRails[j].GetComponent<BoxCollider2D>().offset = new Vector2(0, coulissesOnRails[j].GetComponent<RectTransform>().rect.height / 2);
        }
    }
}
