using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject schieneBild;
    [SerializeField] public GameObject menuExtra;
    [SerializeField] private GameObject ElementName;
    [SerializeField] private GameObject sliderX, sliderY;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [HideInInspector] public Vector2 pos;
    public SceneryElement ThisSceneryElement;
    public float CoulisseWidth;
    public float CoulisseHeight;

    [HideInInspector] public GameObject parentStart;
    public GameObject silhouette;
    [HideInInspector] public GameObject scenerysettings;
    public GameObject gameController;

    //[HideInInspector] public int statusReiter;
    [HideInInspector] public int schieneKulisse;
    [HideInInspector] public float shelfSizeWidth, shelfSizeHeight;
    [HideInInspector] public bool isWide;
    private bool highlighted;

    private void Awake()
    {
        scenerysettings = GameObject.Find("MenueConfigMain");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        pos = rectTransform.anchoredPosition;

        //statusReiter = 1;           // statusReiter ist der aktuell geoeffnete Reiter
        schieneKulisse = 0;         // schieneKulisse ist 0, wenn die Kulisse im Shelf liegt, also keinem Reiter angehoert!

        SceneManaging.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
        menuExtra.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

        parentStart = gameObject.transform.parent.gameObject;
    }

    public void Start()
    {
        setReiterActive(SceneManaging.statusReiter);  // die funktion darf erst nach Awake ausgefuehrt werden, weil sonst die erste Schleife 
                                                      // alles auf false setzt und die weiteren Reiter nicht mehr gefunden werden! 
                                                      // also erst Awake fuer alle und dann aktiven Reiter setzen
        ThisSceneryElement = StaticSceneData.StaticData.sceneryElements.Find(x => x.name == gameObject.name.Substring(6));

        if (CoulisseWidth < CoulisseHeight)
        {
            isWide = false;
            shelfSizeWidth = 200 / CoulisseHeight * CoulisseWidth;
            rectTransform.sizeDelta = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
            silhouette.GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth, 200);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(shelfSizeWidth, 200);
        }
        else
        {
            isWide = true;
            shelfSizeHeight = 200 / CoulisseWidth * CoulisseHeight;
            rectTransform.sizeDelta = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
            silhouette.GetComponent<RectTransform>().sizeDelta = new Vector2(200, shelfSizeHeight);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(200, shelfSizeHeight);
        }
    }

    public void RemoveCoulisse()
    {
        if (ThisSceneryElement.mirrored)
        {
            ThisSceneryElement.mirrored = false;
            GetComponent<RectTransform>().localScale = new Vector3(1, 1.0f, 1.0f);
        }
        transform.GetChild(1).gameObject.SetActive(false);
        ThisSceneryElement.active = false;
        transform.SetParent(parentStart.transform);
        ThisSceneryElement.parent = "Schiene1";
        GetComponent<RectTransform>().anchoredPosition = pos;
        schieneKulisse = 0;
        if (isWide)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
            GetComponent<BoxCollider2D>().size = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
            GetComponent<BoxCollider2D>().size = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
        }
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        menuExtra.SetActive(false);
        gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
    }

    public void setReiterActive(int stat) // stat ist statusReiter
    {
        for (int i = 0; i <= 7; i++)
        {
            gameController.GetComponent<UIController>().goReiterActive[i].SetActive(false);
        }

        if (stat != 0)
        {
            gameController.GetComponent<UIController>().goReiterActive[stat - 1].SetActive(true);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (SceneManaging.showSettings)
        {
            setElementInactive(this);
        }

        canvasGroup.blocksRaycasts = false;
        gameObject.transform.SetParent(scenerysettings.transform);
        GetComponent<Image>().color = new Color(.5f, 1f, 0.3f, 1f);
        //Debug.Log("Schiene Width: " + schieneBild.GetComponent<RectTransform>().rect.width + ", Kulisse Width: " + gameObject.GetComponent<RectTransform>().rect.width + ", kulisse soll width: " + schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseWidth);
        float valueScale = schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseWidth / gameObject.GetComponent<RectTransform>().rect.width;
        rectTransform.sizeDelta = new Vector2(schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseWidth, schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseHeight);
        gameObject.GetComponent<BoxCollider2D>().size = rectTransform.sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung

        //Debug.Log("Status Reiter: "+SceneManaging.statusReiter+ ", this.Schiene: "+this.schieneKulisse+"schieneActive: "+GetComponent<TriggerSchiene>().schieneActive);

        if (SceneManaging.triggerActive != 0 && this.schieneKulisse != SceneManaging.triggerActive)
        {
            SceneManaging.statusReiter = SceneManaging.triggerActive;
            this.schieneKulisse = SceneManaging.triggerActive;
            setReiterActive(SceneManaging.triggerActive);
        }
        if (GetComponent<TriggerSchiene>().schieneActive)
        {
            this.schieneKulisse = SceneManaging.statusReiter;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        if (SceneManaging.statusReiter != 0)
        {
            gameObject.transform.SetParent(gameController.GetComponent<UIController>().Collection[(SceneManaging.statusReiter - 1)].transform);   // collection geht von 0-7, deswegen 'statusReiter-1'

            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = SceneManaging.statusReiter;
                ThisSceneryElement.active = true;
                ThisSceneryElement.parent = "Schiene" + SceneManaging.statusReiter.ToString();
                ThisSceneryElement.railnumber = SceneManaging.statusReiter;
            }
            else if (SceneManaging.triggerActive == SceneManaging.statusReiter)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = SceneManaging.statusReiter;
                ThisSceneryElement.active = true;
                ThisSceneryElement.parent = "Schiene" + SceneManaging.statusReiter.ToString();
                ThisSceneryElement.railnumber = SceneManaging.statusReiter;
            }
            else
            {
                gameObject.transform.SetParent(parentStart.transform);
                ThisSceneryElement.active = false;
                ThisSceneryElement.parent = "Schiene1";
                ThisSceneryElement.railnumber = 1;
                rectTransform.anchoredPosition = pos;
                if (isWide)
                {
                    rectTransform.sizeDelta = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
                    gameObject.GetComponent<BoxCollider2D>().size = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
                }
                else
                {
                    rectTransform.sizeDelta = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
                    gameObject.GetComponent<BoxCollider2D>().size = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
                }
                this.schieneKulisse = 0;
            }
        }


        else if (SceneManaging.triggerEinstellungen && GetComponent<TriggerSchiene>().schieneActive == false)
        {
            GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
        }

        else
        {
            gameObject.transform.SetParent(parentStart.transform);
            ThisSceneryElement.active = false;                  // hier wird active false gesetzt, weil die kulisse zurueck ins shelf gesetzt wird
            rectTransform.anchoredPosition = pos;
        }

        // ------------ Dinge, die fuer die Kulissen im Controler passieren muessen

        ThisSceneryElement.z = GetComponent<RectTransform>().anchoredPosition.x / 300;
        ThisSceneryElement.y = GetComponent<RectTransform>().anchoredPosition.y / 300;  //die werte stimmen ungefaehr mit dem liveview ueberein
        ThisSceneryElement.x = 0.062f;
        //Debug.Log("Scenery Element: " + ThisSceneryElement.name + ", ThisSceneryElement.active: " + ThisSceneryElement.active + ", Posz: " + ThisSceneryElement.z + ", Parent: " + ThisSceneryElement.parent);
        // ------------ uebertragen der Daten aus dem Controller auf die 3D-Kulissen

        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements

        //Debug.Log("parent: "+gameObject.transform.parent);
        //Debug.Log("Trigger Active: "+SceneManaging.triggerActive+ ", schieneActive: "+GetComponent<TriggerSchiene>().schieneActive);
    }

    public void setElementActive(DragDrop dragdrop)
    {
        dragdrop.GetComponent<Image>().color = new Color(.5f, 1f, 0.3f, 1f);
        // highlight LV Element 
        SceneManaging.showSettings = true;
        dragdrop.menuExtra.SetActive(true);
        dragdrop.transform.GetChild(0).gameObject.SetActive(true);
        dragdrop.transform.GetChild(1).gameObject.SetActive(true);
        ElementName.GetComponent<Text>().text = gameObject.transform.GetChild(0).GetComponent<Text>().text;
        highlighted = true;
    }

    public void setElementInactive(DragDrop dragdrop)
    {
        dragdrop.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        dragdrop.transform.GetChild(0).gameObject.SetActive(false);
        dragdrop.transform.GetChild(1).gameObject.SetActive(false);
        SceneManaging.showSettings = false;
        dragdrop.menuExtra.SetActive(false);
        highlighted = false;
    }

    public void OnClick()
    {
        SceneManaging.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManaging die aktuelle Kulisse uebergeben
        sliderX.GetComponent<Slider>().value = GetComponent<RectTransform>().anchoredPosition.x / 200;
        sliderY.GetComponent<Slider>().value = GetComponent<RectTransform>().anchoredPosition.y / 100;

        if (schieneKulisse != 0) // if object is not in shelf
        {
            if (SceneManaging.showSettings && this.highlighted) // this coulisse is highlighted
            {
                setElementInactive(this);
            }
            else if (SceneManaging.showSettings && this.highlighted == false) // different coulisse is highlighted
            {
                for (int i = 0; i < this.transform.parent.childCount; i++)  // set inactive all coulisses of this collection --> in reiterbutton script: on click/change unhighlight ALL coulisses!
                {
                    setElementInactive(gameObject.transform.parent.GetChild(i).GetComponent<DragDrop>());
                    Debug.Log("Child: " + gameObject.transform.parent.GetChild(i).GetComponent<DragDrop>());
                }
                setElementActive(this);
            }

            else    // nothing is highlighted
            {
                setElementActive(this);
            }
        }
        else // if you click in shelf to get a new coulisse
        {
            for (int i = 0; i < this.transform.parent.childCount; i++)  // set inactive all coulisses of this collection --> in reiterbutton script: on click/change unhighlight ALL coulisses!
            {
                setElementInactive(gameObject.transform.parent.GetChild(i).GetComponent<DragDrop>());
                Debug.Log("Child: " + gameObject.transform.parent.GetChild(i).GetComponent<DragDrop>());
            }
        }
    }


}
