using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class SceneManaging
{
    #region variables
    public static int objectInIndexTab;
    public static bool playing;
    public static bool dragging;
    public static bool updateMusic = false;
    public static bool highlighted;                 //global variable since only one object can be highlighted - everything else is unhighlighted
    //public static int objectsTimeline;
    public static bool openUp;
    public static bool sceneChanged;
    public static int configMenueActive;
    public static int directorMenueActive;
    public static int mainMenuActive;
    public static bool isPreviewLoaded;    // Variable zur Abfrage, ob die Szene in der Vorschau auch geladen wurde.
    public static bool isExpert;
    // flyer
    public static bool flyerActive;
    public static int[] flyerSpace = new int[9];
    //timeslider
    public static bool fullscreenOn = false;
    public static Color _colFigure = new Color(0.06f, 0.66f, .74f, 0.5f);
    public static Color _colFigureHighlighted = new Color(0f, 0.87f, 1.0f, 0.5f);
    public static Color _colMusic = new Color(0.21f, 0.51f, 0.267f, 0.5f);
    public static Color _colMusicHighlighted = new Color(0.21f, 0.81f, 0.267f, 0.5f);
    public static Color _colFlyerHighlighted = new Color(1, .5f, .25f);
    public static Color _colFlyer = new Color(.78f, .46f, .31f);
    public static Color _colFlyerSpace = new Color(.78f, .54f, .44f);
    #endregion
    public static void createRectangle(GameObject obj, Color col, double rectHeight, GameObject prefab, double tmpLength)
    {
        //double tmpLength = rectSize;//calcSecondsToPixel(objectAnimationLength, maxTimeInSec) / 2 + 25;
        GameObject imgObject = prefab;
        RectTransform trans = imgObject.GetComponent<RectTransform>();
        trans.SetParent(obj.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(-50, 0);

        trans.sizeDelta = new Vector2((float)tmpLength, (float)rectHeight);    //size related to animationLength
        Image image = imgObject.GetComponent<Image>();
        image.color = col;
        var tempColor = image.color;
        tempColor.a = 0.5f;
        image.color = tempColor;

        trans.SetSiblingIndex(0);
    }
    public static void scaleObject(GameObject fig, float x, float y, bool boxCollZero)
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
    public static void scaleDeleteButton(GameObject btn, float posY, float size, float colSize)
    {
        btn.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(30, posY, -1);
        btn.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
    }
    public static void scaleToLayerSize(GameObject obj, int layer, GameObject timeline, float rectSize)
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
    public static int identifyClickedObject(GameObject[] figureObjects)  // from shelf
    {
        int idx = -1;
        for (int i = 0; i < figureObjects.Length; i++)
        {
            //which object in grid layout is clicked
            if (figureObjects[i].GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
            {
                idx = i;
                break;
            }
        }
        return idx;
    }
    public static int identifyClickedObjectByList(List<RailManager.Figure> objectsOnTimeline)       // from timeline
    {
        int idx = -1;
        for (int i = 0; i < objectsOnTimeline.Count; i++)
        {
            //which object in grid layout is clicked
            if (objectsOnTimeline[i].figure.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(Input.mousePosition))
            {
                idx = i;
            }
        }
        return idx;
    }
    public static void highlight(GameObject obj3D, GameObject obj, bool highlightOn, string type)
    {
        Color colHighlighted;
        Color col;

        if (type == "figure")
        {
            colHighlighted = _colFigureHighlighted;
            col = _colFigure;
        }
        else if (type == "music")
        {
            colHighlighted = _colMusicHighlighted;
            col = _colMusic;
        }
        else
        {
            colHighlighted = _colFlyerHighlighted;
            col = _colFlyer;
        }
        if (highlightOn)
        {
            if (type == "flyer")
            {
                // Debug.Log("delete button: " + obj.transform.GetChild(1).name);
                // if (obj.transform.childCount > 1)
                // {
                obj.GetComponent<Image>().color = colHighlighted;
                obj.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(true);   //show Delete-Button
                // }
                obj.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                obj.transform.GetChild(0).GetComponent<Image>().color = colHighlighted;
                obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);   //show Delete-Button
                SceneManaging.highlighted = true;

                if (obj3D != null)
                {
                    if (obj3D.GetComponent<FigureStats>().isShip)
                        obj3D.GetComponent<cakeslice.Outline>().enabled = true;
                    else
                        obj3D.transform.GetChild(1).GetComponent<cakeslice.Outline>().enabled = true;
                }
            }
        }
        else
        {
            if (type == "flyer")
            {
                if (obj.transform.childCount > 1)
                {
                    //Debug.Log("hier?");
                    obj.GetComponent<Image>().color = col;
                    obj.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(false);   //show Delete-Button
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    //Debug.Log("childcount: " + obj.transform.childCount);
                    obj.GetComponent<Image>().color = _colFlyerSpace;

                }
                SceneManaging.highlighted = false;
            }
            else
            {
                obj.transform.GetChild(0).GetComponent<Image>().color = col;
                obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);  //hide Delete-Button
                SceneManaging.highlighted = false;

                if (obj3D != null)
                {
                    if (obj3D.GetComponent<FigureStats>().isShip)
                        obj3D.GetComponent<cakeslice.Outline>().enabled = false;
                    else
                        obj3D.transform.GetChild(1).GetComponent<cakeslice.Outline>().enabled = false;
                }
            }
        }
    }
    public static void closeRail(GameObject rail, float railwidthAbsolute)
    {
        rail.GetComponent<RectTransform>().sizeDelta = new Vector2(railwidthAbsolute, 20);
        rail.GetComponent<BoxCollider2D>().size = new Vector2(railwidthAbsolute, 20);
    }

}
