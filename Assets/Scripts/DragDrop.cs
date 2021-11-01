using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject schieneBild;
    public ReiterButton activeReiter;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Vector2 pos;



    public GameObject reiter1, reiter2, reiter3, reiter4, reiter5, reiter6, reiter7, reiter8, einstellungen;
    public GameObject reiter1Active, reiter2Active, reiter3Active, reiter4Active, reiter5Active, reiter6Active, reiter7Active, reiter8Active;
    float currentTime;
    int delay;

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
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        pos = rectTransform.anchoredPosition;
        currentTime = 0f;
        delay = 0;

        statusReiter = 1;           // statusReiter ist der aktuell geoeffnete Reiter (am anfang 1)
        schieneKulisse = 0;         // schieneKulisse ist 0, wenn sie im Shelf liegt, also keinem Reiter angehoert!

        SceneManager.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
    }

    public void Start()
    {
        setReiterActive(statusReiter);  // die funktion darf erst nach Awake ausgefuehrt werden, weil sonst die erste Schleife 
                                        // alles auf false setzt und die weiteren Reiter nicht mehr gefunden werden! 
                                        // also erst Awake fuer alle und dann aktiven Reiter setzen
    }

    private void Update()
    {
        //Debug.Log("currentTime: " + currentTime + ", MouseOver: " + SceneManager.mouse_over + ", delay: " + delay);
        //Debug.Log("ActiveReiter: "+ activeReiter+", Status Reiter: "+statusReiter);
        if (SceneManager.mouse_over)
        {
            if (currentTime <= .5f)
            {
                currentTime += Time.deltaTime;
            }
            else if (currentTime > .5f)
            {
                if (delay != 0 && this.schieneKulisse != delay)
                {
                    statusReiter = delay;
                    this.schieneKulisse = delay;
                    setReiterActive(delay);
                    activeReiter.AddKulisse(this);
                }
                else { currentTime = 0f; }

                /*if (delay == 1 && this.schieneKulisse!=1)
                {
                    statusReiter = 1;
                    this.schieneKulisse = 1;
                    setReiterActive(1);
                    activeReiter.AddKulisse(this);
                }
                else if (delay == 2&& this.schieneKulisse!=2)
                {
                    statusReiter = 2;
                    this.schieneKulisse = 2;
                    setReiterActive(2);
                    activeReiter.AddKulisse(this);
                }

                else if (delay == 3&& this.schieneKulisse!=3)
                {
                    statusReiter = 3;
                    this.schieneKulisse = 3;
                    setReiterActive(3);
                    activeReiter.AddKulisse(this);
                }

                else if (delay == 4&& this.schieneKulisse!=4)
                {
                    statusReiter = 4;
                    this.schieneKulisse = 4;
                    setReiterActive(4);
                    activeReiter.AddKulisse(this);
                    //Debug.Log("Kulisse: " + this.name + ", Reiter: "+activeReiter.name + "List Count: " + activeReiter.kulissen.Count);
                }

                else if (delay == 5&& this.schieneKulisse!=5)
                {
                    statusReiter = 5;
                    this.schieneKulisse = 5;
                    setReiterActive(5);
                    activeReiter.AddKulisse(this);
                    //Debug.Log("Kulisse: " + this.name + ", Reiter: "+activeReiter.name + "List Count: " + activeReiter.kulissen.Count);
                }

                else if (delay == 6&& this.schieneKulisse!=6)
                {
                    statusReiter = 6;
                    this.schieneKulisse = 6;
                    setReiterActive(6);
                    activeReiter.AddKulisse(this);
                    //Debug.Log("Kulisse: " + this.name + ", Reiter: "+activeReiter.name + "List Count: " + activeReiter.kulissen.Count);
                }*/
            }
            
        }
        else
        {
            currentTime = 0f;
            delay = 0;
        }
    }

    public void setReiterActive(int stat) // stat ist statusReiter
    {
        reiter1Active.GetComponent<ReiterButton>().Hide();
        reiter2Active.GetComponent<ReiterButton>().Hide();
        reiter3Active.GetComponent<ReiterButton>().Hide();
        reiter4Active.GetComponent<ReiterButton>().Hide();
        reiter5Active.GetComponent<ReiterButton>().Hide();
        reiter6Active.GetComponent<ReiterButton>().Hide();
        reiter7Active.GetComponent<ReiterButton>().Hide();
        reiter8Active.GetComponent<ReiterButton>().Hide();

        if (stat == 1)
        {
            activeReiter = reiter1Active.GetComponent<ReiterButton>();
            reiter1Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat == 2)
        {
            activeReiter = reiter2Active.GetComponent<ReiterButton>();
            reiter2Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat == 3)
        {
            activeReiter = reiter3Active.GetComponent<ReiterButton>();
            reiter3Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat == 4)
        {
            activeReiter = reiter4Active.GetComponent<ReiterButton>();
            reiter4Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat == 5)
        {
            activeReiter = reiter5Active.GetComponent<ReiterButton>();
            reiter5Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat == 6)
        {
            activeReiter = reiter6Active.GetComponent<ReiterButton>();
            reiter6Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat == 7)
        {
            activeReiter = reiter7Active.GetComponent<ReiterButton>();
            reiter7Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat == 8)
        {
            activeReiter = reiter8Active.GetComponent<ReiterButton>();
            reiter8Active.GetComponent<ReiterButton>().Show();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung
        Debug.Log("Schiene Active: " + GetComponent<TriggerSchiene>().schieneActive + ", schieneKulisse: " + this.schieneKulisse+", delay: "+delay+", triggerEinstellungen: "+SceneManager.triggerEinstellungen);
        if (SceneManager.triggerActive == 1 && this.schieneKulisse != 1 && delay != 1) // riggerActive heisst, dass zeiger reiter1 betreten hat. abfrage, ob reiter1 schon aktiv ist, dann rein, ansonsten nach delay fragen
        {
            delay = 1;
            //Debug.Log("delay: " + delay);
        }

        else if (SceneManager.triggerActive == 2 && schieneKulisse != 2 && delay != 2)
        {
            delay = 2;
        }

        else if (SceneManager.triggerActive == 3 && schieneKulisse != 3 && delay != 3)
        {
            delay = 3;
        }

        else if (SceneManager.triggerActive == 4 && schieneKulisse != 4 && delay != 4)
        {
            delay = 4;
        }

        else if (SceneManager.triggerActive == 5 && schieneKulisse != 5 && delay != 5)
        {
            delay = 5;
        }

        else if (SceneManager.triggerActive == 6 && schieneKulisse != 6 && delay != 6)
        {
            delay = 6;
        }

        else if (SceneManager.triggerActive == 7 && schieneKulisse != 7 && delay != 7)
        {
            delay = 7;
        }

        else if (SceneManager.triggerActive == 8 && schieneKulisse != 8 && delay != 8)
        {
            delay = 8;
        }

        else if (this.schieneKulisse != statusReiter && SceneManager.triggerEinstellungen)
        {
            activeReiter.AddKulisse(this);
            this.schieneKulisse = statusReiter;
            //Debug.Log("Schiene hinzugefuegt, da im einstellungsfenster.");
        }

        /*else if (this.schieneKulisse != statusReiter && SceneManager.triggerEinstellungen == false && GetComponent<TriggerSchiene>().schieneActive)
        {
            activeReiter.AddKulisse(this);
            this.schieneKulisse = statusReiter;
            //Debug.Log("Schiene hinzugefuegt, da im einstellungsfenster.");
        }*/

        else if (SceneManager.triggerEinstellungen == false && this.schieneKulisse != 0 && GetComponent<TriggerSchiene>().schieneActive==false)
        {
            Debug.Log("jetzt wird schieneKulisse 0!");
            activeReiter.RemoveKulisse(this);
            this.schieneKulisse = 0;
            delay = 0;
            //Debug.Log("ENTFERNT: Kulisse: " + this.name + ", von Active Reiter: " + activeReiter.name + ", Liste: " + activeReiter.kulissen.Count + ", trigger Einstellungen: " + SceneManager.triggerEinstellungen);
        }

        //Debug.Log("this.schieneKulisse: "+this.schieneKulisse+", aktiver Reiter: "+statusReiter+", Status Kulisse: "+this.activeReiter);
        //Debug.Log("Trigger Einstellungen: "+SceneManager.triggerEinstellungen+", schieneACtive: "+GetComponent<TriggerSchiene>().schieneActive);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //Debug.Log("Status Reiter: " + statusReiter + ", this.SchieneKulisse: " + this.schieneKulisse + ", Count: " + activeReiter.kulissen.Count);
        if (statusReiter == 1)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 1;
                //Debug.Log("List Count: "+activeReiter.kulissen.Count+", active Reiter: "+activeReiter);                
            }
            else if (SceneManager.triggerActive == 1)   // wenn Maus ueber Reiter1 ist
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 1;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
            }
        }

        else if (statusReiter == 2)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 2;
            }
            else if (SceneManager.triggerActive == 2)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 2;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
            }
        }

        else if (statusReiter == 3)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 3;
            }
            else if (SceneManager.triggerActive == 3)
            {
                this.schieneKulisse = 3;
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
            }
        }

        else if (statusReiter == 4)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 4;
            }
            else if (SceneManager.triggerActive == 4)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 4;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
            }
        }

        else if (statusReiter == 5)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 5;
            }
            else if (SceneManager.triggerActive == 5)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 5;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
            }
        }

        else if (statusReiter == 6)
        {
            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = 6;
            }
            else if (SceneManager.triggerActive == 6)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = 6;
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
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
            }
        }

        else
        {
            rectTransform.anchoredPosition = pos;
        }

        if (SceneManager.triggerEinstellungen == true && GetComponent<TriggerSchiene>().schieneActive == false)
        {
            GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
            //Debug.Log("Kulisse: " + this.name + ", Reiter: "+activeReiter.name + "List Count: " + activeReiter.kulissen.Count);
        }


    }

}
