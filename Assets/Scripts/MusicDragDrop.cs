using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicDragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public Vector2 pos;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject railMusic;
    private RectTransform rectTransform;
    
    [HideInInspector] public GameObject scenerysettings;
    private CanvasGroup canvasGroup;
    [HideInInspector] public GameObject parentStart;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        pos = rectTransform.anchoredPosition;
        parentStart = gameObject.transform.parent.gameObject;
        scenerysettings = GameObject.Find("MenueDirectorMain");

        SceneManager.dragDropMusic = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        gameObject.transform.SetParent(scenerysettings.transform);
        GetComponent<Image>().color = new Color(.5f, 1f, 0.3f, 1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung
        Debug.Log("Dragging");
        //Debug.Log("Status Reiter: "+SceneManager.statusReiter+ ", this.Schiene: "+this.schieneKulisse+"schieneActive: "+GetComponent<TriggerSchiene>().schieneActive);

        /*if (SceneManager.triggerActive != 0 && this.schieneKulisse != SceneManager.triggerActive)
        {
            SceneManager.statusReiter = SceneManager.triggerActive;
            this.schieneKulisse = SceneManager.triggerActive;
            setReiterActive(SceneManager.triggerActive);
        }
        if(GetComponent<TriggerSchiene>().schieneActive)
        {
            this.schieneKulisse = SceneManager.statusReiter;
        }*/

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        /*if (SceneManager.statusReiter != 0)
        {
            gameObject.transform.SetParent(collection[(SceneManager.statusReiter - 1)].transform);   // collection geht von 0-7, deswegen 'statusReiter-1'

            if (GetComponent<TriggerSchiene>().schieneActive)
            {
                this.schieneKulisse = SceneManager.statusReiter;
                ThisSceneryElement.active = true;
                ThisSceneryElement.parent = "Schiene" + SceneManager.statusReiter.ToString();
                ThisSceneryElement.railnumber = SceneManager.statusReiter;
            }
            else if (SceneManager.triggerActive == SceneManager.statusReiter)
            {
                GetComponent<RectTransform>().anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
                this.schieneKulisse = SceneManager.statusReiter;
                ThisSceneryElement.active = true;
                ThisSceneryElement.parent = "Schiene" + SceneManager.statusReiter.ToString();
                ThisSceneryElement.railnumber = SceneManager.statusReiter;
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

        // ------------ Dinge, die fuer die Kulissen im Controler passieren muessen
        
        ThisSceneryElement.z = GetComponent<RectTransform>().anchoredPosition.x / 300;
        ThisSceneryElement.y = GetComponent<RectTransform>().anchoredPosition.y / 300;  //die werte stimmen ungefaehr mit dem liveview ueberein
        ThisSceneryElement.x = 0.062f;
        Debug.Log("Scenery Element: "+ThisSceneryElement.name+", ThisSceneryElement.active: "+ThisSceneryElement.active+", Posz: "+ThisSceneryElement.z+", Parent: "+ThisSceneryElement.parent);
        // ------------ uebertragen der Daten aus dem Controller auf die 3D-Kulissen

        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements

        //Debug.Log("parent: "+gameObject.transform.parent);
        //Debug.Log("Trigger Active: "+SceneManager.triggerActive+ ", schieneActive: "+GetComponent<TriggerSchiene>().schieneActive);*/
    }

}
