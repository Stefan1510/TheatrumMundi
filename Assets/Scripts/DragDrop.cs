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
    private ReiterActiveButton activeReiter;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 pos;
    private SceneryElement ThisSceneryElement;

    [HideInInspector] public GameObject parentStart;
    GameObject[] collection = new GameObject[8];

    [HideInInspector] public GameObject reiter1, reiter2, reiter3, reiter4, reiter5, reiter6, reiter7, reiter8, einstellungen, scenerysettings;
    [HideInInspector] public GameObject reiter1Active, reiter2Active, reiter3Active, reiter4Active, reiter5Active, reiter6Active, reiter7Active, reiter8Active;
    [HideInInspector] public GameObject gameController; // neue Zeile von Kris f�r den SceneryController

    [HideInInspector] public int statusReiter;
    [HideInInspector] public int schieneKulisse;

    private void Awake()
    {
        gameController = GameObject.Find("GameController"); // neue Zeile von Kris fuer den SceneryController
        reiter1 = GameObject.Find("Reiter1");
        reiter2 = GameObject.Find("Reiter2");
        reiter3 = GameObject.Find("Reiter3");
        reiter4 = GameObject.Find("Reiter4");
        reiter5 = GameObject.Find("Reiter5");
        reiter6 = GameObject.Find("Reiter6");
        reiter7 = GameObject.Find("Reiter7");
        reiter8 = GameObject.Find("Reiter8");
        einstellungen = GameObject.Find("Einstellungen");
        reiter1Active = GameObject.Find("Reiter1Active");
        reiter2Active = GameObject.Find("Reiter2Active");
        reiter3Active = GameObject.Find("Reiter3Active");
        reiter4Active = GameObject.Find("Reiter4Active");
        reiter5Active = GameObject.Find("Reiter5Active");
        reiter6Active = GameObject.Find("Reiter6Active");
        reiter7Active = GameObject.Find("Reiter7Active");
        reiter8Active = GameObject.Find("Reiter8Active");
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

        statusReiter = 1;           // statusReiter ist der aktuell geoeffnete Reiter
        schieneKulisse = 0;         // schieneKulisse ist 0, wenn sie im Shelf liegt, also keinem Reiter angehoert!

        SceneManager.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
        menuExtra.SetActive(false);

        parentStart = gameObject.transform.parent.gameObject;


    }

    public void Start()
    {
        setReiterActive(statusReiter);  // die funktion darf erst nach Awake ausgefuehrt werden, weil sonst die erste Schleife 
                                        // alles auf false setzt und die weiteren Reiter nicht mehr gefunden werden! 
                                        // also erst Awake fuer alle und dann aktiven Reiter setzen
        ThisSceneryElement = StaticSceneData.StaticData.sceneryElements.Find(x => x.name == gameObject.name.Substring(6));
    }


    public void setReiterActive(int stat) // stat ist statusReiter
    {
        reiter1Active.GetComponent<ReiterActiveButton>().Hide();
        reiter2Active.GetComponent<ReiterActiveButton>().Hide();
        reiter3Active.GetComponent<ReiterActiveButton>().Hide();
        reiter4Active.GetComponent<ReiterActiveButton>().Hide();
        reiter5Active.GetComponent<ReiterActiveButton>().Hide();
        reiter6Active.GetComponent<ReiterActiveButton>().Hide();
        reiter7Active.GetComponent<ReiterActiveButton>().Hide();
        reiter8Active.GetComponent<ReiterActiveButton>().Hide();

        if (stat == 1)
        {
            activeReiter = reiter1Active.GetComponent<ReiterActiveButton>();
            reiter1Active.GetComponent<ReiterActiveButton>().Show();
        }
        else if (stat == 2)
        {
            activeReiter = reiter2Active.GetComponent<ReiterActiveButton>();
            reiter2Active.GetComponent<ReiterActiveButton>().Show();
        }
        else if (stat == 3)
        {
            activeReiter = reiter3Active.GetComponent<ReiterActiveButton>();
            reiter3Active.GetComponent<ReiterActiveButton>().Show();
        }
        else if (stat == 4)
        {
            activeReiter = reiter4Active.GetComponent<ReiterActiveButton>();
            reiter4Active.GetComponent<ReiterActiveButton>().Show();
        }
        else if (stat == 5)
        {
            activeReiter = reiter5Active.GetComponent<ReiterActiveButton>();
            reiter5Active.GetComponent<ReiterActiveButton>().Show();
        }
        else if (stat == 6)
        {
            activeReiter = reiter6Active.GetComponent<ReiterActiveButton>();
            reiter6Active.GetComponent<ReiterActiveButton>().Show();
        }
        else if (stat == 7)
        {
            activeReiter = reiter7Active.GetComponent<ReiterActiveButton>();
            reiter7Active.GetComponent<ReiterActiveButton>().Show();
        }
        else if (stat == 8)
        {
            activeReiter = reiter8Active.GetComponent<ReiterActiveButton>();
            reiter8Active.GetComponent<ReiterActiveButton>().Show();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (SceneManager.showSettings)
        {
            menuExtra.SetActive(false);
            SceneManager.showSettings = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        canvasGroup.blocksRaycasts = false;
        gameObject.transform.SetParent(scenerysettings.transform);
        GetComponent<Image>().color = new Color(.5f, 1f, 0.3f, 1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;


        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung
        Debug.Log("PosX: " + GetComponent<RectTransform>().anchoredPosition.x + ", PosY: " + GetComponent<RectTransform>().anchoredPosition.y);
        //Debug.Log("Trigger Active: "+SceneManager.triggerActive+ ", Reiter: "+activeReiter+", Anzahl Kulissen: "+activeReiter.kulissen.Count+", this.Schiene: "+this.schieneKulisse);

        if (SceneManager.triggerActive != 0 && this.schieneKulisse != SceneManager.triggerActive)
        {
            statusReiter = SceneManager.triggerActive;
            this.schieneKulisse = SceneManager.triggerActive;
            setReiterActive(SceneManager.triggerActive);
            activeReiter.AddKulisse(this);
            //gameObject.transform.SetParent(activeReiter.transform);

        }

        else if (this.schieneKulisse != statusReiter && SceneManager.triggerEinstellungen)
        {
            activeReiter.AddKulisse(this);
            this.schieneKulisse = statusReiter;

            //gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData); // dieses Zeile macht das gleiche und ist glaube besser.
        }

        else if (this.schieneKulisse != statusReiter && SceneManager.triggerEinstellungen == false && GetComponent<TriggerSchiene>().schieneActive)
        {
            activeReiter.AddKulisse(this);
            this.schieneKulisse = statusReiter;



        }

        else if (SceneManager.triggerEinstellungen == false && this.schieneKulisse != 0 && SceneManager.triggerActive == 0 && GetComponent<TriggerSchiene>().schieneActive == false)
        {
            activeReiter.RemoveKulisse(this);
            this.schieneKulisse = 0;

            ThisSceneryElement.active = false;
            //Debug.Log("jetzt wird schieneKulisse 0 und die Kulisse removed!"+", von Reiter: "+activeReiter+", Count: "+activeReiter.kulissen.Count+", this: "+this);
            //Debug.Log("ENTFERNT: Kulisse: " + this.name + ", von Active Reiter: " + activeReiter.name + ", Liste: " + activeReiter.kulissen.Count + ", trigger Einstellungen: " + SceneManager.triggerEinstellungen);
        }

        //gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData); 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        if (statusReiter != 0)
        {
            gameObject.transform.SetParent(collection[(statusReiter - 1)].transform);   // collection geht von 0-7, deswegen 'statusReiter-1'


            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = statusReiter;
                ThisSceneryElement.active = true;
                ThisSceneryElement.parent = "Schiene" + statusReiter.ToString();    // Schiene1-8 funktioniert und wird korrekt ausgegeben, man sieht es nur leider im LiveView noch nicht (obwohl ganz unten createScene ausgefuehrt wird)
            }
            else if (SceneManager.triggerActive == statusReiter)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = statusReiter;
                ThisSceneryElement.active = true;
                ThisSceneryElement.parent = "Schiene" + statusReiter.ToString();
            }
            else
            {
                gameObject.transform.SetParent(parentStart.transform);
                ThisSceneryElement.parent = "Schiene1";
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
            }
        }


        else if (SceneManager.triggerEinstellungen && GetComponent<TriggerSchiene>().schieneActive == false)
        {
            GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
        }

        else
        {
            gameObject.transform.SetParent(parentStart.transform);
            ThisSceneryElement.active = false;                  // hier wird active false gesetzt, weil die kulisse zurueck ins shelf gesetzt wird
            rectTransform.anchoredPosition = pos;

        }

        // ------------ Dinge, die f�r die Kulissen im Controler passieren m�ssen

        //ThisSceneryElement.parent = "Schiene" + statusReiter.ToString(); //hier bitte mal genau �berpr�fen wie die parents dann wirklich sind
        Debug.Log("---- Kulissenname -------- " + ThisSceneryElement.name + " --- " + ThisSceneryElement.parent + " --- " + ThisSceneryElement.x);
        ThisSceneryElement.z = GetComponent<RectTransform>().anchoredPosition.x/300;
        ThisSceneryElement.y = GetComponent<RectTransform>().anchoredPosition.y/300+0.1f;  //die werte stimmen ungefaehr mit dem liveview ueberein


        // ------------ uebertragen der Daten aus dem Controller auf die 3D-Kulissen
        gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData); // dieses Zeile macht das gleiche und ist glaube besser.
    }

    public void OnClick()
    {
        if (schieneKulisse != 0)
        {
            if (SceneManager.showSettings)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SceneManager.showSettings = false;
                menuExtra.SetActive(false);
            }
            else if (SceneManager.showSettings == false)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                SceneManager.showSettings = true;
                menuExtra.SetActive(true);
            }
        }
        else
        {
            SceneManager.showSettings = false;
        }
    }

}
