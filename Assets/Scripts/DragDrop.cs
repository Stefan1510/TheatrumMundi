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

    [HideInInspector] public GameObject parentStart;
    [HideInInspector] GameObject[] collection = new GameObject[8];
    [HideInInspector] GameObject[] reiterActive = new GameObject[8];
    [HideInInspector] public GameObject scenerysettings;
    [HideInInspector] public GameObject gameController; // neue Zeile von Kris fÜr den SceneryController

    //[HideInInspector] public int statusReiter;
    [HideInInspector] public int schieneKulisse;

    private void Awake()
    {
        gameController = GameObject.Find("GameController"); // neue Zeile von Kris fuer den SceneryController
        reiterActive[0] = GameObject.Find("Reiter1Active");
        reiterActive[1] = GameObject.Find("Reiter2Active");
        reiterActive[2] = GameObject.Find("Reiter3Active");
        reiterActive[3] = GameObject.Find("Reiter4Active");
        reiterActive[4] = GameObject.Find("Reiter5Active");
        reiterActive[5] = GameObject.Find("Reiter6Active");
        reiterActive[6] = GameObject.Find("Reiter7Active");
        reiterActive[7] = GameObject.Find("Reiter8Active");
        collection[0] = GameObject.Find("Collection1");
        collection[1] = GameObject.Find("Collection2");
        collection[2] = GameObject.Find("Collection3");
        collection[3] = GameObject.Find("Collection4");
        collection[4] = GameObject.Find("Collection5");
        collection[5] = GameObject.Find("Collection6");
        collection[6] = GameObject.Find("Collection7");
        collection[7] = GameObject.Find("Collection8");
        scenerysettings = GameObject.Find("MenueConfigMain");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        pos = rectTransform.anchoredPosition;

        //statusReiter = 1;           // statusReiter ist der aktuell geoeffnete Reiter
        schieneKulisse = 0;         // schieneKulisse ist 0, wenn die Kulisse im Shelf liegt, also keinem Reiter angehoert!

        SceneManaging.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
        menuExtra.SetActive(false);

        parentStart = gameObject.transform.parent.gameObject;
    }

    public void Start()
    {
        setReiterActive(SceneManaging.statusReiter);  // die funktion darf erst nach Awake ausgefuehrt werden, weil sonst die erste Schleife 
                                        // alles auf false setzt und die weiteren Reiter nicht mehr gefunden werden! 
                                        // also erst Awake fuer alle und dann aktiven Reiter setzen
        ThisSceneryElement = StaticSceneData.StaticData.sceneryElements.Find(x => x.name == gameObject.name.Substring(6));
        Debug.Log("DRAGDROP++++++++++++++++++++pos: "+pos);
    }


    public void setReiterActive(int stat) // stat ist statusReiter
    {
        for (int i = 0; i <= 7; i++)
        {
            reiterActive[i].SetActive(false);
        }

        if (stat != 0)
        {
            reiterActive[stat-1].SetActive(true);
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung

        Debug.Log("Status Reiter: "+SceneManaging.statusReiter+ ", this.Schiene: "+this.schieneKulisse+"schieneActive: "+GetComponent<TriggerSchiene>().schieneActive);

        if (SceneManaging.triggerActive != 0 && this.schieneKulisse != SceneManaging.triggerActive)
        {
            SceneManaging.statusReiter = SceneManaging.triggerActive;
            this.schieneKulisse = SceneManaging.triggerActive;
            setReiterActive(SceneManaging.triggerActive);
        }
        if(GetComponent<TriggerSchiene>().schieneActive)
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
            gameObject.transform.SetParent(collection[(SceneManaging.statusReiter - 1)].transform);   // collection geht von 0-7, deswegen 'statusReiter-1'

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
        Debug.Log("Scenery Element: "+ThisSceneryElement.name+", ThisSceneryElement.active: "+ThisSceneryElement.active+", Posz: "+ThisSceneryElement.z+", Parent: "+ThisSceneryElement.parent);
        // ------------ uebertragen der Daten aus dem Controller auf die 3D-Kulissen

        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements

        //Debug.Log("parent: "+gameObject.transform.parent);
        //Debug.Log("Trigger Active: "+SceneManaging.triggerActive+ ", schieneActive: "+GetComponent<TriggerSchiene>().schieneActive);
    }

    public void setElementActive(DragDrop dragdrop)
    {
        dragdrop.GetComponent<Image>().color = new Color(.5f, 1f, 0.3f, 1f);
        SceneManaging.showSettings = true;
        dragdrop.menuExtra.SetActive(true);
        dragdrop.transform.GetChild(0).gameObject.SetActive(true);
        dragdrop.transform.GetChild(1).gameObject.SetActive(true);
        ElementName.GetComponent<Text>().text = gameObject.transform.GetChild(0).GetComponent<Text>().text;
    }

    public void setElementInactive(DragDrop dragdrop)
    {
        dragdrop.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        dragdrop.transform.GetChild(0).gameObject.SetActive(false);
        dragdrop.transform.GetChild(1).gameObject.SetActive(false);
        SceneManaging.showSettings = false;
        dragdrop.menuExtra.SetActive(false);
    }

    public void OnClick()
    {
        SceneManaging.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManaging die aktuelle Kulisse uebergeben
        //Debug.Log("posX: " + this.GetComponent<RectTransform>().anchoredPosition.x/200+", SliderValue: "+sliderX.GetComponent<Slider>().value);
        sliderX.GetComponent<Slider>().value = GetComponent<RectTransform>().anchoredPosition.x/200;
        sliderY.GetComponent<Slider>().value = GetComponent<RectTransform>().anchoredPosition.y/100;

        if (schieneKulisse != 0)
        {
            for (int i = 0; i < this.transform.parent.childCount; i++)
            {
                setElementInactive(gameObject.transform.parent.GetChild(i).GetComponent<DragDrop>());
                Debug.Log("Child: " + gameObject.transform.parent.GetChild(i).GetComponent<DragDrop>());
            }

            if (SceneManaging.showSettings)
            {
                setElementInactive(this);
            }
            else if (SceneManaging.showSettings == false)
            {
                setElementActive(this);
            }
        }
        else
        {
            setElementInactive(this);
        }
    }

}
