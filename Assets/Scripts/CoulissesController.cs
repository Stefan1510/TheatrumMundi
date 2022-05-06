using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoulisseManager : MonoBehaviour
{
    public GameObject gameController;
    public GameObject[] indexTabs;
    public GameObject[] collections;
    public GameObject[] indexTabImages;
    public GameObject[] coulisses;
    public GameObject scenerySettings;
    public GameObject schieneBild;
    public GameObject mainMenue;
    private GameObject[] parentStart;

    [HideInInspector] public float[] shelfSizeWidth;
    [HideInInspector] public float[] shelfSizeHeight;
    [HideInInspector] public bool[] isWide;


    int currentObjectIndex;
    bool dragging;
    private Color colHighlighted, colSilhouetteActive, colCoulisse, colSilhouette;


    void Start()
    {
        scenerySettings.SetActive(false);
        shelfSizeWidth = new float[coulisses.Length];
        shelfSizeHeight = new float[coulisses.Length];
        parentStart = new GameObject[coulisses.Length];

        isWide = new bool[coulisses.Length];

        // width and height of coulisses in shelf - saved in the arrays 'isWide', 'shelfSizeWidth' and 'shelfSizeHeight'
        for (int i = 0; i < coulisses.Length; i++)
        {
            if (coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth < coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight)
            {
                isWide[i] = false;
                shelfSizeWidth[i] = 200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth;
                coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight * coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth, 200);
                coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth[i], 200);
                coulisses[i].GetComponent<BoxCollider2D>().size = new Vector2(shelfSizeWidth[i], 200);
                //coulisses[i].GetComponent<BoxCollider2D>().offset = new Vector2(0, 100);
            }
            else
            {
                isWide[i] = true;
                shelfSizeHeight[i] = 200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight;
                coulisses[i].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200 / coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth * coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight);
                coulisses[i].transform.parent.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(200, shelfSizeHeight[i]);
                coulisses[i].GetComponent<BoxCollider2D>().size = new Vector2(200, shelfSizeHeight[i]);
                //coulisses[i].GetComponent<BoxCollider2D>().offset = new Vector2(0, shelfSizeHeight[i] / 2);
            }
            parentStart[i] = coulisses[i].transform.parent.gameObject;
        }

        colHighlighted = new Color(1f, .45f, 0.33f, 1f);
        colCoulisse = new Color(1f, 1f, 1f, 1f);
        colSilhouette = new Color(0f, 0f, 0f, 0.4f);
        colSilhouetteActive = new Color(0.6f, 0f, 0f, 0.4f);

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
                    // Debug.Log("Getroffen! Rail" + i);

                }
            }
            // setting Index of clicked Object
            currentObjectIndex = identifyClickedObjectIndex();
            // if a coulisse is hit
            if (currentObjectIndex != -1)
            {
                Debug.Log("Hi! Ich bin die Kulisse: " + coulisses[currentObjectIndex]);
                dragging = true;
                highlight(currentObjectIndex, true);
                // coulisses[currentObjectIndex].transform.parent.GetChild(0).GetComponent<RectTransform>().GetComponent<Image>().color = colSilhouetteActive;

                coulisses[currentObjectIndex].transform.SetParent(mainMenue.transform);
                // //Debug.Log("Schiene Width: " + schieneBild.rectTransform.rect.width + ", Kulisse Width: " + gameObject.rectTransform.rect.width + ", kulisse soll width: " + schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseWidth);
                float valueScale = schieneBild.GetComponent<RectTransform>().rect.width / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseWidth / coulisses[currentObjectIndex].GetComponent<RectTransform>().rect.width;
                coulisses[currentObjectIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(schieneBild.GetComponent<RectTransform>().rect.width / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseHeight, schieneBild.GetComponent<RectTransform>().rect.width / 410 * coulisses[currentObjectIndex].GetComponent<CoulisseStats>().CoulisseHeight);
                coulisses[currentObjectIndex].GetComponent<BoxCollider2D>().size = coulisses[currentObjectIndex].GetComponent<RectTransform>().sizeDelta;
            }

        }

        if (dragging)
        {
            coulisses[currentObjectIndex].transform.position = new Vector2(getMousePos.x, getMousePos.y);
            coulisses[currentObjectIndex].transform.SetParent(mainMenue.transform);

            int hitIndexTab = checkHittingIndexTab(indexTabs, getMousePos);
            if(hitIndexTab!=-1)
            {
                setIndexTabActive(hitIndexTab);
            }
        }



        if (Input.GetMouseButtonUp(0)) //left mouse button up
        {
            dragging = false;
            placeInShelf(currentObjectIndex);
            highlight(currentObjectIndex, false);
        }
    }

    public int checkHittingIndexTab(GameObject[] tabs, Vector2 mousePos)
    {
        int hit = -1;
        /*Debug.Log("tl pos "+tl.transform.position); //global pos are the real position-data
        Debug.Log("tl collider "+tl.GetComponent<BoxCollider2D>().size);
        Debug.Log("mouse "+mousePos);
        Debug.Log("obj collider "+obj.GetComponent<BoxCollider2D>().size);
        */
        //calculate bounding-box related to the timeline-pos
        for (int i = 0; i < tabs.Length; i++)
        {
            Vector2 tlPos = new Vector2(tabs[i].transform.position.x, tabs[i].transform.position.y);
            Vector2 colSize = new Vector2(tabs[i].GetComponent<BoxCollider2D>().size.x, tabs[i].GetComponent<BoxCollider2D>().size.y);
            //if mouse hits the timeline while dragging an object
            //my implementation of object-boundingbox
            if (((mousePos.x <= (tlPos.x + (colSize.x / 2.0f))) && (mousePos.x > (tlPos.x - (colSize.x / 2.0f)))) &&
            ((mousePos.y <= (tlPos.y + (colSize.y / 2.0f))) && (mousePos.y > (tlPos.y - (colSize.y / 2.0f)))))
            {
                //Debug.Log("object hits timeline!");
                hit = i; 
            }
        }

        //Debug.Log("drag and hit " + hit);
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
    }
    public void highlight(int i, bool pleaseHighlight)
    {
        if (pleaseHighlight) coulisses[i].GetComponent<Image>().color = colHighlighted;
        else coulisses[i].GetComponent<Image>().color = colCoulisse;
    }
}
