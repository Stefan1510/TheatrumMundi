using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoulissesManager : MonoBehaviour
{
    public GameObject gameController;
    public GameObject[] indexTabs;
    public GameObject[] collections;
    public GameObject[] indexTabImages;
    public GameObject[] coulisses;
    public GameObject scenerySettings;
    public GameObject schieneBild;
    public GameObject mainMenue;
    public GameObject colliderSettings;
    public GameObject deleteButton;
    private GameObject[] parentStart;


    [HideInInspector] public float[] shelfSizeWidth;
    [HideInInspector] public float[] shelfSizeHeight;
    [HideInInspector] public bool[] isWide;
    [HideInInspector] public bool[] isHighlighted;


    int currentObjectIndex;
    bool dragging;
    private Color colHighlighted, colSilhouetteActive, colCoulisse, colSilhouette;
    private bool objectInField;
    private int currentTabIndex;
    private bool objectInRail;

    Vector2 schieneBildPos, schieneBildCollider, colliderSettingsPos, colliderSettingsCollider;
    Vector2 diff;

    void Awake()
    {
        SceneManaging.objectInIndexTab = -1;
    }
    void Start()
    {
        scenerySettings.SetActive(false);
        shelfSizeWidth = new float[coulisses.Length];
        shelfSizeHeight = new float[coulisses.Length];
        parentStart = new GameObject[coulisses.Length];
        isWide = new bool[coulisses.Length];
        isHighlighted = new bool[coulisses.Length];

        // width and height of coulisses in shelf - saved in the arrays 'isWide', 'shelfSizeWidth' and 'shelfSizeHeight'
        for (int i = 0; i < coulisses.Length; i++)
        {
            if (coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth < coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight)
            {
                isWide[i] = false;
                shelfSizeWidth[i] = 200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth;
                shelfSizeHeight[i] = 200;
                coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth, 200);
                coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], 200);
                coulisses[i].GetComponent<BoxCollider2D>().size = new Vector2(shelfSizeWidth[i], 200);
                //coulisses[i].GetComponent<BoxCollider2D>().offset = new Vector2(0, 100);
            }
            else
            {
                isWide[i] = true;
                shelfSizeHeight[i] = 200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight;
                shelfSizeWidth[i] = 200;
                coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight);
                coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(200, shelfSizeHeight[i]);
                coulisses[i].GetComponent<BoxCollider2D>().size = new Vector2(200, shelfSizeHeight[i]);
                //coulisses[i].GetComponent<BoxCollider2D>().offset = new Vector2(0, shelfSizeHeight[i] / 2);
            }
            parentStart[i] = coulisses[i].transform.parent.gameObject;
            StaticSceneData.StaticData.sceneryElements[i].emission = false;


        }

        colHighlighted = new Color(1f, .45f, 0.33f, 1f);
        colCoulisse = new Color(1f, 1f, 1f, 1f);
        colSilhouette = new Color(0f, 0f, 0f, 0.4f);
        colSilhouetteActive = new Color(0.6f, 0f, 0f, 0.4f);

        schieneBildPos = new Vector2(schieneBild.GetComponent<RectTransform>().position.x, schieneBild.GetComponent<RectTransform>().transform.position.y);
        schieneBildCollider = new Vector2(schieneBild.GetComponent<BoxCollider2D>().size.x, schieneBild.GetComponent<BoxCollider2D>().size.y);
        colliderSettingsPos = new Vector2(colliderSettings.GetComponent<RectTransform>().position.x, colliderSettings.GetComponent<RectTransform>().transform.position.y);
        colliderSettingsCollider = new Vector2(colliderSettings.GetComponent<BoxCollider2D>().size.x, colliderSettings.GetComponent<BoxCollider2D>().size.y);

        setIndexTabActive(0);
        mainMenue.GetComponent<RectTransform>().pivot = collections[0].GetComponent<RectTransform>().pivot;
        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
    }

    void Update()
    {
        Vector2 getMousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) //left mouse button down
        {
            for (int i = 0; i < indexTabs.Length; i++)
            {
                if (indexTabs[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
                {
                    setIndexTabActive(i);
                }
            }
            // setting Index of clicked Object
            currentObjectIndex = identifyClickedObjectIndex();

            // if a coulisse is hit
            if (currentObjectIndex != -1)
            {
                diff = new Vector2(getMousePos.x - coulisses[currentObjectIndex].transform.position.x, getMousePos.y - coulisses[currentObjectIndex].transform.position.y);
                dragging = true;
                if (isHighlighted[currentObjectIndex])
                {
                    highlight(currentObjectIndex, false);
                }
                else
                {
                    if (isAnythingHighlighted())
                    {
                        for (int j = 0; j < coulisses.Length; j++)
                        {
                            highlight(j, false);
                        }
                        highlight(currentObjectIndex, true);
                    }
                    else highlight(currentObjectIndex, true);
                }
                // coulisses[currentObjectIndex].transform.parent.GetChild(0).GetComponent<RectTransform>().GetComponent<Image>().color = colSilhouetteActive;

                coulisses[currentObjectIndex].transform.SetParent(mainMenue.transform);
                // Debug.Log("Schiene Width: " + schieneBild.GetComponent<RectTransform>().rect.width + ", Kulisse Width: " + coulisses[currentObjectIndex].GetComponent<RectTransform>().rect.width + ", kulisse soll width: " + schieneBild.GetComponent<RectTransform>().rect.width / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseWidth);
                coulisses[currentObjectIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(schieneBild.GetComponent<RectTransform>().rect.width / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseWidth, schieneBild.GetComponent<RectTransform>().rect.width / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseHeight);
                coulisses[currentObjectIndex].GetComponent<BoxCollider2D>().size = coulisses[currentObjectIndex].GetComponent<RectTransform>().sizeDelta;
            }
            else // if nothing is hit
            {
                for (int i = 0; i < coulisses.Length; i++)
                {
                    highlight(i, false);        //todo: liste anlegen mit nur kulissen die auf schiene sind (damit nicht ALLE unhighlighted werden muessen)
                }
            }
        }

        if (dragging)
        {
            coulisses[currentObjectIndex].transform.position = new Vector2(getMousePos.x - diff.x, getMousePos.y - diff.y); // diff is difference of mouse position to clicked position on coulisse so that the coulisse doesnt jump to the pivot all the time
            coulisses[currentObjectIndex].transform.SetParent(mainMenue.transform);

            ////////////////////////////////////////////////////////////////////////////////////////////
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y) / 260 + .02f;
            StaticSceneData.StaticData.sceneryElements[currentObjectIndex].x = 0.062f;
            StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
            ////////////////////////////////////////////////////////////////////////////////////////////

            // if coulisse is hitting a tab
            int hitIndexTab = checkHittingIndexTab(indexTabs, getMousePos);
            if (hitIndexTab != -1)
            {
                SceneManaging.objectInIndexTab = hitIndexTab;
                // Debug.Log("Ich bin im IndexTab! " + SceneManaging.objectInIndexTab);
                if (SceneManaging.openUp)
                {
                    setIndexTabActive(hitIndexTab);
                }
            }
            else if (checkHitting(colliderSettingsPos, colliderSettingsCollider, coulisses[currentObjectIndex]))
            {
                objectInField = true;
                objectInRail = false;
                // Debug.Log("Ich bin auf im Fenster! " + objectInField);
                if (checkHitting(schieneBildPos, schieneBildCollider, coulisses[currentObjectIndex]))
                {
                    objectInRail = true;
                    // Debug.Log("Ich bin auf der Schiene! " + objectInRail);
                }
            }
            else
            {
                objectInField = false;
                SceneManaging.objectInIndexTab = -1;
                objectInRail = false;
            }
        }

        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            dragging = false;

            if (currentObjectIndex != -1)
            {
                // Debug.Log("currentTab: "+currentTabIndex);
                // if object is in settingsField
                if (objectInField)
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
                                coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x, 0.0f);
                            }
                        }
                    }
                    // if (isHighlighted[currentObjectIndex])
                    // {
                    //     highlight(currentObjectIndex, false);
                    // }
                    // else
                    // {
                    //     highlight(currentObjectIndex, true);
                    // }

                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].active = true;
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].parent = "Schiene" + (currentTabIndex + 1).ToString();
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].railnumber = currentTabIndex + 1;
                }


                // if object is on a tab
                else if (SceneManaging.objectInIndexTab != -1)
                {
                    coulisses[currentObjectIndex].transform.SetParent(collections[SceneManaging.objectInIndexTab].transform);
                    coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition = new Vector2(0.0f, 0.0f);
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].active = true;

                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].parent = "Schiene" + (currentTabIndex + 1).ToString();
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].railnumber = currentTabIndex + 1;
                    if (isHighlighted[currentObjectIndex])
                    {
                        highlight(currentObjectIndex, false);
                    }
                    else
                    {
                        highlight(currentObjectIndex, true);
                    }
                }

                // if object is outside the window (back to shelf)
                else
                {
                    placeInShelf(currentObjectIndex);
                    highlight(currentObjectIndex, false);
                    StaticSceneData.StaticData.sceneryElements[currentObjectIndex].active = false;
                }
                // set default values since nothing is clicked anymore
                objectInField = false;
                SceneManaging.objectInIndexTab = -1;
                objectInRail = false;

                ///////////////////////////////////////////////
                StaticSceneData.StaticData.sceneryElements[currentObjectIndex].z = coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.x / 270;
                Debug.Log("posY 2D: " + coulisses[currentObjectIndex].GetComponent<RectTransform>().position.y + ", localposY 2D: " + coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y);
                StaticSceneData.StaticData.sceneryElements[currentObjectIndex].y = (coulisses[currentObjectIndex].GetComponent<RectTransform>().localPosition.y) / 260 + .02f;
                StaticSceneData.StaticData.sceneryElements[currentObjectIndex].x = 0.062f;
                StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
                ////////////////////////////////////////////////////////////////////////
            }

        }
    }
    public int checkHittingIndexTab(GameObject[] tabs, Vector2 mousePos)
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
    public bool checkHitting(Vector2 pos1, Vector2 collider1, GameObject obj2)
    {
        bool hit = false;

        Vector2 pos2 = new Vector2(obj2.transform.position.x, obj2.transform.position.y);
        Vector2 collider2 = new Vector2(obj2.GetComponent<BoxCollider2D>().size.x, obj2.GetComponent<BoxCollider2D>().size.y);

        if (pos2.x - collider2.x / 2.0f <= pos1.x + (collider1.x / 2.0f) && pos2.y - collider2.y / 2.0f <= pos1.y + (collider1.y / 2.0f)
        && pos2.x + collider2.x / 2.0f >= pos1.x - collider1.x / 2.0f && pos2.y + collider2.y / 2.0f >= pos1.y - collider1.y / 2.0f)
        {
            hit = true;
        }
        return hit;
    }
    public void setIndexTabActive(int currentIndex)
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

    public int identifyClickedObjectIndex()
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

    public void placeInShelf(int i)
    {
        coulisses[i].transform.SetParent(parentStart[i].transform);
        coulisses[i].GetComponent<RectTransform>().localPosition = new Vector2(0.0f, 0.0f);
        coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], shelfSizeHeight[i]);
        highlight(i, false);
    }
    public void highlight(int i, bool pleaseHighlight)
    {
        if (pleaseHighlight)
        {
            coulisses[i].GetComponent<Image>().color = colHighlighted;
            scenerySettings.SetActive(true);
            showDeleteButton(deleteButton, coulisses[i], true);
            StaticSceneData.StaticData.sceneryElements[i].emission = true;
            isHighlighted[i] = true;
        }

        else
        {
            coulisses[i].GetComponent<Image>().color = colCoulisse;
            showDeleteButton(deleteButton, coulisses[i], false);
            StaticSceneData.StaticData.sceneryElements[i].emission = true;
            isHighlighted[i] = false;
        }

    }
    public bool isAnythingHighlighted()
    {
        bool val = false;
        for (int i = 0; i < coulisses.Length; i++)
        {
            if (isHighlighted[i])
            {
                val = true;
            }
        }
        return val;
    }
    public void showDeleteButton(GameObject deleteButton, GameObject parent, bool show)
    {
        if (show)
        {
            deleteButton.SetActive(true);
            deleteButton.transform.SetParent(parent.transform);
            deleteButton.GetComponent<RectTransform>().localPosition = new Vector2(-55.0f, 90.0f);
        }
        else
        {
            deleteButton.SetActive(false);
        }
    }
    public void removeCoulisse()
    {
        int i = int.Parse(deleteButton.transform.parent.name.Substring(8, 2)) - 1;
        Debug.Log("Remove!!" + gameObject.name + "deletebutton: " + deleteButton.transform.parent.name);
        placeInShelf(i);
    }
}
