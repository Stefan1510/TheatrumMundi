using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject schieneBild;
    private ReiterButton activeReiter;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 pos;
    
    

    public GameObject reiter1, reiter2, reiter3, reiter4, reiter5, reiter6; 
    public GameObject reiter1Active, reiter2Active, reiter3Active, reiter4Active, reiter5Active, reiter6Active;

    public GameObject feineinstellungen;
    int statusReiter;
    int schieneKulisse;



    float dist1, dist2, dist3, dist4, dist5, dist6;

    private void Awake() {
        reiter1 = GameObject.Find("Reiter1");
        reiter2 = GameObject.Find("Reiter2");
        reiter3 = GameObject.Find("Reiter3");
        reiter4 = GameObject.Find("Reiter4");
        reiter5 = GameObject.Find("Reiter5");
        reiter6 = GameObject.Find("Reiter6");
        reiter1Active = GameObject.Find("Reiter1Active");
        reiter2Active = GameObject.Find("Reiter2Active");
        reiter3Active = GameObject.Find("Reiter3Active");
        reiter4Active = GameObject.Find("Reiter4Active");
        reiter5Active = GameObject.Find("Reiter5Active");
        reiter6Active = GameObject.Find("Reiter6Active");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        pos = rectTransform.anchoredPosition;
        
        feineinstellungen = GameObject.Find("Einstellungen");
        statusReiter = 1;           // statusReiter ist der aktuell geoeffnete Reiter
        schieneKulisse = 0;         // schieneKulisse ist 0, wenn sie im Shelf liegt, also keinem Reiter angehoert!
        
        SceneManager.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
    }

    public void Start() 
    {
        setReiterActive(statusReiter);  // die funktion darf erst nach Awake ausgefuehrt werden, weil sonst die erste Schleife 
                                        // alles auf false setzt und die weiteren Reiter nicht mehr gefunden werden! 
                                        // also erst Awake fuer alle und dann aktiven Reiter setzen
    }


    public void setReiterActive(int stat) // stat ist statusReiter
    {
        reiter1Active.GetComponent<ReiterButton>().Hide();
        reiter2Active.GetComponent<ReiterButton>().Hide();
        reiter3Active.GetComponent<ReiterButton>().Hide();
        reiter4Active.GetComponent<ReiterButton>().Hide();
        reiter5Active.GetComponent<ReiterButton>().Hide();
        reiter6Active.GetComponent<ReiterButton>().Hide();

        if (stat==1) {
            statusReiter = 1;
            activeReiter = reiter1Active.GetComponent<ReiterButton>();
            reiter1Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat==2) { 
            statusReiter = 2;          
            activeReiter = reiter2Active.GetComponent<ReiterButton>();
            reiter2Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat==3) {
            statusReiter = 3;
            activeReiter = reiter3Active.GetComponent<ReiterButton>();
            reiter3Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat==4) {
            statusReiter = 4;
            activeReiter = reiter4Active.GetComponent<ReiterButton>();
            reiter4Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat==5) {
            statusReiter = 5;
            activeReiter = reiter5Active.GetComponent<ReiterButton>();
            reiter5Active.GetComponent<ReiterButton>().Show();
        }
        else if (stat==6) {
            statusReiter = 6;
            activeReiter = reiter6Active.GetComponent<ReiterButton>();
            reiter6Active.GetComponent<ReiterButton>().Show();
        }
    } 

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung
        dist1 = Vector2.Distance(reiter1.GetComponent<RectTransform>().anchoredPosition, GetComponent<RectTransform>().anchoredPosition);
        dist2 = Vector2.Distance(reiter2.GetComponent<RectTransform>().anchoredPosition, GetComponent<RectTransform>().anchoredPosition);
        dist3 = Vector2.Distance(reiter3.GetComponent<RectTransform>().anchoredPosition, GetComponent<RectTransform>().anchoredPosition);
        dist4 = Vector2.Distance(reiter4.GetComponent<RectTransform>().anchoredPosition, GetComponent<RectTransform>().anchoredPosition);
        dist5 = Vector2.Distance(reiter5.GetComponent<RectTransform>().anchoredPosition, GetComponent<RectTransform>().anchoredPosition);
        dist6 = Vector2.Distance(reiter6.GetComponent<RectTransform>().anchoredPosition, GetComponent<RectTransform>().anchoredPosition);
        
        //Debug.Log("Reiter: "+activeReiter.name + ", List: "+activeReiter.kulissen.Count + ", this hinzugefuegt: "+this.name);

        if (dist1 <= 40f) setReiterActive(1);

        else if (dist2 <= 40f) setReiterActive(2);

        else if (dist3 <= 40f) setReiterActive(3);

        else if (dist4 <= 40f) setReiterActive(4);

        else if (dist5 <= 40f) setReiterActive(5);

        else if (dist6 <= 40f) setReiterActive(6);
     
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        if (statusReiter == 1) {
            if (schieneBild.GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 1) {
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 1;
                }
                else {  
           // Debug.Log("Schiene Kulisse ist schon 1, die Schiene bleibt aber trotzdem hier.");
                }
                
            } 
            else if (dist1 < 40f) 
            {
                if (schieneKulisse != 1) {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 1;
                }
                else {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                activeReiter.RemoveKulisse(this);
            }
        }

        else if (statusReiter == 2)
        {
            if (schieneBild.GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 2) {
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 2;
                }
                else {  
                }
                
            } 
            else if (dist2 < 40f) 
            {
                if (schieneKulisse != 2) {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 2;
                }
                else {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                activeReiter.RemoveKulisse(this);
            }
        }

        else if (statusReiter == 3)
        {
            if (schieneBild.GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 3) {
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 3;
                }
                else {  
                }
                
            } 
            else if (dist3 < 40f) 
            {
                if (schieneKulisse != 3) {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 3;
                }
                else {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                activeReiter.RemoveKulisse(this);
            }
        }

        else if (statusReiter == 4)
        {
            if (schieneBild.GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 4) {
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 4;
                }
                else {  
                }
                
            } 
            else if (dist4 < 40f) 
            {
                if (schieneKulisse != 4) {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 4;
                }
                else {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                activeReiter.RemoveKulisse(this);
            }
        }

        else if (statusReiter == 5)
        {
            if (schieneBild.GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 5) {
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 5;
                }
                else {  
                }
                
            } 
            else if (dist5 < 40f) 
            {
                if (schieneKulisse != 5) {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 5;
                }
                else {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                activeReiter.RemoveKulisse(this);
            }
        }

        else if (statusReiter == 6)
        {
            if (schieneBild.GetComponent<TriggerSchiene>().schieneActive)
            {
                if (schieneKulisse != 6) {
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 6;
                }
                else {  

                }
                
            } 
            else if (dist6 < 40f) 
            {
                if (schieneKulisse != 6) {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                    activeReiter.AddKulisse(this);
                    this.schieneKulisse = 6;
                }
                else {
                    GetComponent<RectTransform>().anchoredPosition = feineinstellungen.GetComponent<RectTransform>().anchoredPosition;
                    
                }
            }

            else
            {
                rectTransform.anchoredPosition = pos;
                this.schieneKulisse = 0;
                activeReiter.RemoveKulisse(this);
            }
        }
        

        else
        {
            rectTransform.anchoredPosition = pos;
        }
    }

}
