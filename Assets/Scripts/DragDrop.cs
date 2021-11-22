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



    public GameObject reiter1, reiter2, reiter3, reiter4, reiter5, reiter6, reiter7, reiter8, einstellungen, content, scenerysettings;
    public GameObject reiter1Active, reiter2Active, reiter3Active, reiter4Active, reiter5Active, reiter6Active, reiter7Active, reiter8Active;


    public int statusReiter;
    public int schieneKulisse;


    private void Awake()
    {
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
        content = GameObject.Find("ContentSeneryObjects");
        scenerysettings = GameObject.Find("MenuSettingsSceneryObjects");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        pos = rectTransform.anchoredPosition;

        statusReiter = 1;           // statusReiter ist der aktuell geoeffnete Reiter
        schieneKulisse = 0;         // schieneKulisse ist 0, wenn sie im Shelf liegt, also keinem Reiter angehoert!

        SceneManager.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
        menuExtra.SetActive(false);
    }

    public void Start()
    {
        setReiterActive(statusReiter);  // die funktion darf erst nach Awake ausgefuehrt werden, weil sonst die erste Schleife 
                                        // alles auf false setzt und die weiteren Reiter nicht mehr gefunden werden! 
                                        // also erst Awake fuer alle und dann aktiven Reiter setzen
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
        if(SceneManager.showDeleteButton)
        {
            menuExtra.SetActive(false);
            SceneManager.showDeleteButton = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        canvasGroup.blocksRaycasts = false;
        gameObject.transform.SetParent(scenerysettings.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung
        //Debug.Log("StatusReiter: " + statusReiter + ", SchieneKulisse: " + schieneKulisse + ", Liste: " + activeReiter.kulissen.Count);

        if (SceneManager.triggerActive != 0 && this.schieneKulisse != SceneManager.triggerActive)
        {
            Debug.Log("Test");
            statusReiter = SceneManager.triggerActive;
            this.schieneKulisse = SceneManager.triggerActive;
            setReiterActive(SceneManager.triggerActive);
            activeReiter.AddKulisse(this);
            //Debug.Log("Trigger Active: "+SceneManager.triggerActive+ ", Reiter: "+activeReiter+", Anzahl Kulissen: "+activeReiter.kulissen.Count+", this.Schiene: "+this.schieneKulisse);
        }

        else if (this.schieneKulisse != statusReiter && SceneManager.triggerEinstellungen)
        {
            activeReiter.AddKulisse(this);
            this.schieneKulisse = statusReiter;
            Debug.Log("Schiene hinzugefuegt, da im einstellungsfenster.");
        }

        else if (this.schieneKulisse != statusReiter && SceneManager.triggerEinstellungen == false && GetComponent<TriggerSchiene>().schieneActive)
        {
            activeReiter.AddKulisse(this);
            this.schieneKulisse = statusReiter;
            Debug.Log("Schiene hinzugefuegt, da im einstellungsfenster.");
        }

        else if (SceneManager.triggerEinstellungen == false && this.schieneKulisse != 0 && SceneManager.triggerActive == 0 && GetComponent<TriggerSchiene>().schieneActive == false)
        {
            activeReiter.RemoveKulisse(this);
            this.schieneKulisse = 0;
            
            //Debug.Log("jetzt wird schieneKulisse 0 und die Kulisse removed!"+", von Reiter: "+activeReiter+", Count: "+activeReiter.kulissen.Count+", this: "+this);
            //Debug.Log("ENTFERNT: Kulisse: " + this.name + ", von Active Reiter: " + activeReiter.name + ", Liste: " + activeReiter.kulissen.Count + ", trigger Einstellungen: " + SceneManager.triggerEinstellungen);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //Debug.Log("Status Reiter: "+statusReiter+", this.SchieneKulisse: "+this.schieneKulisse+", this.schieneBild: "+this.schieneBild);
        if (statusReiter == 1)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 1;
            }
            else if (SceneManager.triggerActive == 1)   // wenn Maus ueber Reiter1 ist
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 1;
            }

            else
            {
                gameObject.transform.SetParent(content.transform);
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                
            }
        }

        else if (statusReiter == 2)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 2)
                {
                    this.schieneKulisse = 2;
                }

            }
            else if (SceneManager.triggerActive == 2)
            {
                if (schieneKulisse != 2)
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                    //activeReiter.AddKulisse(this);
                    this.schieneKulisse = 2;
                }
                else
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                gameObject.transform.SetParent(content.transform);
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                
            }
        }

        else if (statusReiter == 3)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 3)
                {
                    //activeReiter.AddKulisse(this);
                    this.schieneKulisse = 3;
                }

            }

            else if (SceneManager.triggerActive == 3)
            {
                if (schieneKulisse != 3)
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                    this.schieneKulisse = 3;
                }
                else
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                gameObject.transform.SetParent(content.transform);
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
            }
        }

        else if (statusReiter == 4)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 4)
                {
                    this.schieneKulisse = 4;
                }

            }
            else if (SceneManager.triggerActive == 4)
            {
                if (schieneKulisse != 4)
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                    this.schieneKulisse = 4;
                }
                else
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                gameObject.transform.SetParent(content.transform);
            }
        }

        else if (statusReiter == 5)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 5)
                {
                    this.schieneKulisse = 5;
                }
                else
                {
                }

            }
            else if (SceneManager.triggerActive == 5)
            {
                if (schieneKulisse != 5)
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                    this.schieneKulisse = 5;
                }
                else
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                gameObject.transform.SetParent(content.transform);
            }
        }

        else if (statusReiter == 6)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 6)
                {
                    this.schieneKulisse = 6;
                }
                else
                {

                }

            }
            else if (SceneManager.triggerActive == 6)
            {
                if (schieneKulisse != 6)
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                    this.schieneKulisse = 6;
                }
                else
                {
                    GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;

                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                gameObject.transform.SetParent(content.transform);
            }
        }

        else if (statusReiter == 7)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 7;
            }
            else if (SceneManager.triggerActive == 7)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 7;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                gameObject.transform.SetParent(content.transform);
            }
        }

        else if (statusReiter == 8)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 8;
            }
            else if (SceneManager.triggerActive == 8)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 8;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                gameObject.transform.SetParent(content.transform);
            }
        }
        else if (SceneManager.triggerEinstellungen && GetComponent<TriggerSchiene>().schieneActive == false)
        {
            GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("Kulisse: " + this.name + ", Reiter: "+activeReiter.name + "List Count: " + activeReiter.kulissen.Count);
        }

        else
        {
            Debug.Log("else");
            gameObject.transform.SetParent(content.transform);
            rectTransform.anchoredPosition = pos;
            
        }

        
    }

}
