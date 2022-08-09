using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ObjectShelf : MonoBehaviour
{
    //objectShelf is the class for the object-browser (buehne,kulisse,licht)
    //global class variables
    public GameObject MenueShelf01;
    public GameObject MenueShelf02;
    public GameObject MenueShelf03;
    public GameObject MenueShelf04;
    //public GameObject gameController;

    public GameObject LiveView;
    public GameObject SchematicView;

    public Text HeadlineShelf01;
    public Text HeadlineShelf02;
    public Text HeadlineShelf03;
    public Text HeadlineShelf04;

    public GameObject MenueButton01;
    public GameObject MenueButton02;
    public GameObject MenueButton03;
    public GameObject MenueButton04;

    public GameObject panelPreviewNotLoaded;

    GameObject gameController;

    //private GameObject lvcamera;

    private void Awake()
    {
        gameController = GameObject.Find("GameController");
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManaging.mainMenuActive = 1;
        SceneManaging.configMenueActive = 1;
        //Debug.Log("testprint");
        //define the buttons
        //Debug.Log("this is the main menue");
        //mytext.text="this is text";

        MenueShelf01.SetActive(true);
        MenueShelf02.SetActive(false);
        MenueShelf03.SetActive(false);
        MenueShelf04.SetActive(false);

        //LiveView.SetActive(true);

        HeadlineShelf01.gameObject.SetActive(true);
        HeadlineShelf02.gameObject.SetActive(false);
        HeadlineShelf03.gameObject.SetActive(false);
        HeadlineShelf04.gameObject.SetActive(false);

        MenueButton01.SetActive(false);
        MenueButton02.SetActive(true);
        MenueButton03.SetActive(true);
        MenueButton04.SetActive(true);
        //ButtonShelf01();


    }

    public void ButtonShelf01()
    {
        //Debug.Log("button for shelf01");
        //gameController.GetComponent<UIController>().Rails[0].openCloseTimelineByClick(false,gameController.GetComponent<UIController>().Rails[0].timelineImage,false);
        //show menue of buehne or figuren

        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(1));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(1));
        }

 
    }
    public void ButtonShelf02()
    {
        //Debug.Log("button for shelf02");
        // gameController.GetComponent<UIController>().RailLightBG[0].openCloseTimelineByClick(false,gameController.GetComponent<UIController>().RailLightBG[0].timel);
        //Debug.Log("+++timeline: "+gameController.GetComponent<UIController>().RailLightBG[0].timeSliderImage);
        //show menue of kulissen or licht
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(2));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(2));
        }
    }
    public void ButtonShelf03()
    {
        //Debug.Log("button for shelf03");
        //gameController.GetComponent<UIController>().RailMusic.openCloseTimelineByClick(false,gameController.GetComponent<UIController>().RailMusic.timelineImage,false);
        //show menue of lichtkonfiguration or musik
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(3));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(3));
        }
    }

    public void ButtonShelf04()
    {
        //Debug.Log("button for shelf04");
        //show menue of ladenspeichern
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(4));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(4));
        }
    }

    IEnumerator ButtonShelfI(int shelfNumber)
    {
        while (panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().buttonClicked == null)
        {
            yield return panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick();
        }
        if (panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().buttonClicked == "ignore")
        {
            //Debug.LogWarning("_buttonClicked == ignore");
        }
        else if (panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().buttonClicked == "back")
        {
            //Debug.LogWarning("_buttonClicked == back");
            shelfNumber = 4;
        }
        else
        {
            //Debug.LogWarning("_buttonClicked == DDD:");
        }

        MenueShelf01.SetActive(false);
        MenueShelf02.SetActive(false);
        MenueShelf03.SetActive(false); 
        try
        {
            MenueShelf04.SetActive(false);
        }
        catch (Exception ex)
        {
            if (ex is NullReferenceException || ex is UnassignedReferenceException)
            {
                yield break;
            }
            throw;
        }
        MenueShelf04.SetActive(false);

        HeadlineShelf01.gameObject.SetActive(false);
        HeadlineShelf02.gameObject.SetActive(false);
        HeadlineShelf03.gameObject.SetActive(false);
        try
        {
            HeadlineShelf04.gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            if (ex is NullReferenceException || ex is UnassignedReferenceException)
            {
                yield break;
            }
            throw;
        }

        MenueButton01.SetActive(true);
        MenueButton02.SetActive(true);
        MenueButton03.SetActive(true);
        try
        {
            MenueButton04.SetActive(true);
        }
        catch (Exception ex)
        {
            if (ex is NullReferenceException || ex is UnassignedReferenceException)
            {
                yield break;
            }
            throw;
        }


        switch (shelfNumber)
        {
            case 1:
                MenueShelf01.SetActive(true);
                HeadlineShelf01.gameObject.SetActive(true); 
                //SceneManaging.menueActive = 1;
                break;
            case 2:
                MenueShelf02.SetActive(true);
                HeadlineShelf02.gameObject.SetActive(true);
                //SceneManaging.menueActive = 2;
                break;
            case 3:
                MenueShelf03.SetActive(true);
                HeadlineShelf03.gameObject.SetActive(true);
                //SceneManaging.menueActive = 3;
                break;
            case 4:
                try
                {
                    MenueShelf04.SetActive(true);
                    HeadlineShelf04.gameObject.SetActive(true);
                    //SceneManaging.menueActive = 4;
                }
                catch (Exception ex)
                {
                    if (ex is NullReferenceException || ex is UnassignedReferenceException)
                    {
                        yield break;
                    }
                    throw;
                }
                break;
        }

        StaticSceneData.Everything3D();
        gameController.GetComponent<UIController>().Rails[0].GetComponent<RailManager>().PublicUpdate();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
