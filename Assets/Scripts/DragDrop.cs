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
    private bool dragging;
    [HideInInspector] public Vector2 pos;

    [HideInInspector] public float shelfSizeWidth, shelfSizeHeight;
    public SceneryElement ThisSceneryElement;
    public float CoulisseWidth;
    public float CoulisseHeight;
    private Material[] matCoulisses;

    [HideInInspector] public GameObject parentStart;
    public GameObject silhouette;
    public GameObject scenerysettings;
    public GameObject gameController;

    //[HideInInspector] public int statusReiter;
    [HideInInspector] public int schieneKulisse;
    [HideInInspector] public bool isWide;
    private bool highlighted;
    private Color colHighlighted, colCoulisse, colSilhouetteActive, colSilhouette;

    public bool OpenUp { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        //statusReiter = 1;           // statusReiter ist der aktuell geoeffnete Reiter
        schieneKulisse = 0;         // schieneKulisse ist 0, wenn die Kulisse im Shelf liegt, also keinem Reiter angehoert!

        SceneManaging.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManager die aktuelle Kulisse uebergeben!
        menuExtra.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        pos = rectTransform.anchoredPosition;
        parentStart = gameObject.transform.parent.gameObject;
        dragging = false;
    }

    public void Start()
    {
        setReiterActive(SceneManaging.statusReiter);
        ThisSceneryElement = StaticSceneData.StaticData.sceneryElements.Find(x => x.name == gameObject.name.Substring(6));
        //ThisSceneryElement.emission = false;
        colHighlighted = new Color(1f, .45f, 0.33f, 1f);
        colCoulisse = new Color(1f, 1f, 1f, 1f);
        colSilhouette = new Color(0f, 0f, 0f, 0.4f);
        colSilhouetteActive = new Color(0.6f, 0f, 0f, 0.4f);

        if (CoulisseWidth < CoulisseHeight)
        {
            isWide = false;
            shelfSizeWidth = 200 / CoulisseHeight * CoulisseWidth;
            rectTransform.sizeDelta = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
            silhouette.GetComponent<RectTransform>().sizeDelta = new Vector2(shelfSizeWidth, 200);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(shelfSizeWidth, 200);
            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 100);
        }
        else
        {
            isWide = true;
            shelfSizeHeight = 200 / CoulisseWidth * CoulisseHeight;
            rectTransform.sizeDelta = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
            silhouette.GetComponent<RectTransform>().sizeDelta = new Vector2(200, shelfSizeHeight);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(200, shelfSizeHeight);
            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, shelfSizeHeight / 2);
        }
    }


    public void RemoveCoulisse()
    {
        if (ThisSceneryElement.mirrored)
        {
            ThisSceneryElement.mirrored = false;
            rectTransform.localScale = new Vector3(1, 1.0f, 1.0f);
        }
        transform.GetChild(1).gameObject.SetActive(false);
        ThisSceneryElement.active = false;
        transform.SetParent(parentStart.transform);
        ThisSceneryElement.parent = "Schiene1";
        rectTransform.anchoredPosition = pos;
        schieneKulisse = 0;
        if (isWide)
        {
            rectTransform.sizeDelta = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
            GetComponent<BoxCollider2D>().size = new Vector2(200, 200 / CoulisseWidth * CoulisseHeight);
            GetComponent<BoxCollider2D>().offset = new Vector2(0, shelfSizeHeight / 2);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
            GetComponent<BoxCollider2D>().size = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
            GetComponent<BoxCollider2D>().offset = new Vector2(0, 100);
        }
        GetComponent<Image>().color = colCoulisse;
        menuExtra.SetActive(false);
        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
    }

    public void setReiterActive(int stat) // stat = statusReiter
    {
        for (int i = 0; i <= 7; i++)
        {
            gameController.GetComponent<UIController>().goReiterActive[i].SetActive(false);
            gameController.GetComponent<UIController>().goIndexTabs[i].SetActive(false);
        }

        if (stat != 0)
        {
            gameController.GetComponent<UIController>().goReiterActive[stat - 1].SetActive(true);
            gameController.GetComponent<UIController>().goIndexTabs[stat - 1].SetActive(true);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SceneManaging.dragDrop = this;

        for (int j = 0; j < gameController.GetComponent<UIController>().goIndexTabs.Length; j++)
        {
            for (int i = 0; i < gameController.GetComponent<UIController>().goIndexTabs[j].transform.childCount; i++)  // set inactive all coulisses --> in reiterbutton script: on click/change unhighlight ALL coulisses!
            {
                setElementInactive(gameController.GetComponent<UIController>().goIndexTabs[j].transform.GetChild(i).GetComponent<DragDrop>());
            }
        }
        GetComponent<Image>().color = colHighlighted;
        silhouette.GetComponent<Image>().color = colSilhouetteActive;

        canvasGroup.blocksRaycasts = false;
        gameObject.transform.SetParent(scenerysettings.transform);
        //Debug.Log("Schiene Width: " + schieneBild.rectTransform.rect.width + ", Kulisse Width: " + gameObject.rectTransform.rect.width + ", kulisse soll width: " + schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseWidth);
        float valueScale = schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseWidth / rectTransform.rect.width;
        rectTransform.sizeDelta = new Vector2(schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseWidth, schieneBild.GetComponent<RectTransform>().rect.width / 410 * CoulisseHeight);
        gameObject.GetComponent<BoxCollider2D>().size = rectTransform.sizeDelta;
        GetComponent<BoxCollider2D>().offset = new Vector2(0, rectTransform.rect.height / 2);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SceneManaging.dragDrop = this;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // durch Skalierung d. Canvas teilen, sonst Bewegung d. Objekts nicht gleich der Mausbewegung


        //Debug.Log("Status Reiter: "+SceneManaging.statusReiter+ ", this.Schiene: "+this.schieneKulisse+"schieneActive: "+GetComponent<TriggerSchiene>().schieneActive);
        //Debug.Log("Triggereinstellungen: " + SceneManaging.triggerEinstellungen + ", triggerSChiene: " + GetComponent<TriggerSchiene>().schieneActive + ", statusReiter: " + SceneManaging.statusReiter);

        if (SceneManaging.triggerActive != 0 && this.schieneKulisse != SceneManaging.triggerActive)
        {
            dragging = true;

        }
        if (GetComponent<TriggerSchiene>().schieneActive)
        {
            this.schieneKulisse = SceneManaging.statusReiter;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        GetComponent<Image>().color = colCoulisse;
        silhouette.GetComponent<Image>().color = colSilhouette;
        canvasGroup.blocksRaycasts = true;
        gameObject.transform.SetParent(gameController.GetComponent<UIController>().goIndexTabs[(SceneManaging.statusReiter - 1)].transform);   // collection geht von 0-7, deswegen 'statusReiter-1'
        sliderX.GetComponent<Slider>().value = rectTransform.anchoredPosition.x / 200;
        sliderY.GetComponent<Slider>().value = (rectTransform.anchoredPosition.y + 150) / 100;
        if (GetComponent<TriggerSchiene>().schieneActive)                                                            // if coulisse touches rail image
        {
            this.schieneKulisse = SceneManaging.statusReiter;
            ThisSceneryElement.active = true;
            ThisSceneryElement.parent = "Schiene" + SceneManaging.statusReiter.ToString();
            ThisSceneryElement.railnumber = SceneManaging.statusReiter;
            setElementActive(this);
        }
        else if (SceneManaging.triggerActive == SceneManaging.statusReiter)                                          // if mouse pos is on indexTab
        {
            rectTransform.anchoredPosition = schieneBild.GetComponent<RectTransform>().anchoredPosition;
            this.schieneKulisse = SceneManaging.statusReiter;
            ThisSceneryElement.active = true;
            ThisSceneryElement.parent = "Schiene" + SceneManaging.statusReiter.ToString();
            ThisSceneryElement.railnumber = SceneManaging.statusReiter;
            setElementActive(this);
        }
        else if (SceneManaging.triggerEinstellungen && GetComponent<TriggerSchiene>().schieneActive == false)       // if coulisse is not on rail image but in schematic window
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, schieneBild.GetComponent<RectTransform>().anchoredPosition.y);
            ThisSceneryElement.active = true;
            ThisSceneryElement.parent = "Schiene" + SceneManaging.statusReiter.ToString();
            ThisSceneryElement.railnumber = SceneManaging.statusReiter;
            setElementActive(this);
        }
        else                                                                                                         // if coulisse is dropped somewhere else
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
                GetComponent<BoxCollider2D>().offset = new Vector2(0, rectTransform.rect.height / 2);
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(200 / CoulisseHeight * CoulisseWidth, 200);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, rectTransform.rect.height / 2);
            }
            this.schieneKulisse = 0;
        }
        // sliderX.GetComponent<Slider>().value = rectTransform.anchoredPosition.x / 200;
        // sliderY.GetComponent<Slider>().value = (rectTransform.anchoredPosition.y + 150) / 100;
        ThisSceneryElement.z = rectTransform.anchoredPosition.x / 300;
        ThisSceneryElement.y = (rectTransform.anchoredPosition.y + 150) / 200;
        ThisSceneryElement.x = 0.062f;
        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
    }

    public void setElementActive(DragDrop dragdrop)
    {
        dragdrop.GetComponent<Image>().color = colHighlighted;
        SceneManaging.showSettings = true;
        dragdrop.menuExtra.SetActive(true);
        dragdrop.transform.GetChild(0).gameObject.SetActive(true);
        dragdrop.transform.GetChild(1).gameObject.SetActive(true);
        ElementName.GetComponent<Text>().text = gameObject.transform.GetChild(0).GetComponent<Text>().text;
        dragdrop.highlighted = true;
        // highlight LV Element 
        dragdrop.ThisSceneryElement.emission = true;
        dragdrop.silhouette.GetComponent<Image>().color = colSilhouetteActive;
    }

    public void setElementInactive(DragDrop dragdrop)
    {
        dragdrop.GetComponent<Image>().color = colCoulisse;
        dragdrop.transform.GetChild(0).gameObject.SetActive(false);
        dragdrop.transform.GetChild(1).gameObject.SetActive(false);
        SceneManaging.showSettings = false;
        dragdrop.menuExtra.SetActive(false);
        dragdrop.highlighted = false;
        dragdrop.ThisSceneryElement.emission = false;
        dragdrop.silhouette.GetComponent<Image>().color = colSilhouette;
    }

    public void OnClick()
    {
        SceneManaging.dragDrop = this; // hier wird dem dragDrop-Objekt im SceneManaging die aktuelle Kulisse uebergeben
        sliderX.GetComponent<Slider>().value = rectTransform.anchoredPosition.x / 200;
        sliderY.GetComponent<Slider>().value = (rectTransform.anchoredPosition.y + 150) / 100;
        //Debug.Log("X: pos 3d: " + ThisSceneryElement.z + ", pos slider: " + sliderX.GetComponent<Slider>().value + ", pos 2D: " + GetComponent<RectTransform>().anchoredPosition.x / 200);

        if (schieneKulisse != 0) // if object is not in shelf
        {
            if (SceneManaging.showSettings && this.highlighted) // this coulisse is highlighted
            {
                setElementInactive(this);
            }
            else if (SceneManaging.showSettings && this.highlighted == false) // different coulisse is highlighted
            {
                for (int j = 0; j < gameController.GetComponent<UIController>().goIndexTabs.Length; j++)
                {
                    for (int i = 0; i < gameController.GetComponent<UIController>().goIndexTabs[j].transform.childCount; i++)  // set inactive all coulisses of this collection --> in reiterbutton script: on click/change unhighlight ALL coulisses!
                    {
                        setElementInactive(gameController.GetComponent<UIController>().goIndexTabs[j].transform.GetChild(i).GetComponent<DragDrop>());
                    }
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
            if (SceneManaging.showSettings)
            {
                for (int j = 0; j < gameController.GetComponent<UIController>().goIndexTabs.Length; j++)
                {
                    for (int i = 0; i < gameController.GetComponent<UIController>().goIndexTabs[j].transform.childCount; i++)  // set inactive all coulisses of this collection --> in reiterbutton script: on click/change unhighlight ALL coulisses!
                    {
                        setElementInactive(gameController.GetComponent<UIController>().goIndexTabs[j].transform.GetChild(i).GetComponent<DragDrop>());
                    }
                }
            }
            GetComponent<Image>().color = colHighlighted;
        }
        ThisSceneryElement.z = rectTransform.anchoredPosition.x / 300;
        ThisSceneryElement.y = (rectTransform.anchoredPosition.y + 150) / 200;
        ThisSceneryElement.x = 0.062f;
        StaticSceneData.Sceneries3D(); //CreateScene der SceneryElements
    }


    public void Update()
    {
        if (dragging && SceneManaging.openUp)
        {
            SceneManaging.statusReiter = SceneManaging.triggerActive;
            this.schieneKulisse = SceneManaging.triggerActive;
            setReiterActive(SceneManaging.triggerActive);
        }
    }
}