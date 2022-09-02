using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ObjectShelfAll : MonoBehaviour
{
    public GameObject MenueShelf01;
    public GameObject MenueShelf02;
    public GameObject MenueShelf03;
    public GameObject MenueShelf04;
    public GameObject MenueShelf05;
    public GameObject MenueShelf06;
    public GameObject MenueShelf07;

    public GameObject MenueButton01;    //ButtonBuehne          -- only expert
    public GameObject MenueButton02;    //ButtonKulissen
    public GameObject MenueButton03;    //ButtonLicht           -- only expert
    public GameObject MenueButton04;    //ButtonLadenSpeichern
    public GameObject MenueButton05;    //ButtonObjects
    public GameObject MenueButton06;    //ButtonLights          -- only expert
    public GameObject MenueButton07;    //ButtonMusic

    public GameObject ObjectMenueDirectorMain;
    public GameObject ObjectMenueConfigMain;
    public GameObject ButtonMenueConfig;    // Der Button in ObjectMenueDirectorMain der zum ConfigMenu leitet
    public GameObject ButtonMenueDirector;  // Der Button in ObjectMenueConfigMain der zum DirectorMenu leitet

    public GameObject panelPreviewNotLoaded;

    public GameObject gameController;


    private void Awake()
    {
        MenueButton01.GetComponent<Button>().onClick.AddListener(() => ButtonShelf01());
        MenueButton02.GetComponent<Button>().onClick.AddListener(() => ButtonShelf02());
        MenueButton03.GetComponent<Button>().onClick.AddListener(() => ButtonShelf03());
        MenueButton04.GetComponent<Button>().onClick.AddListener(() => ButtonShelf04());
        MenueButton05.GetComponent<Button>().onClick.AddListener(() => ButtonShelf05(false));
        MenueButton06.GetComponent<Button>().onClick.AddListener(() => ButtonShelf06());
        MenueButton07.GetComponent<Button>().onClick.AddListener(() => ButtonShelf07(false));
        ButtonMenueDirector.GetComponent<Button>().onClick.AddListener(() => SwitchToMenueDirector());
        ButtonMenueConfig.GetComponent<Button>().onClick.AddListener(() => SwitchToMenueConfig());
    }

    // Start is called before the first frame update
    void Start()
    {
        /////////////////////////////////// for VISITOR-Tool
        SceneManaging.mainMenuActive = 1;
        SceneManaging.configMenueActive = 2;
        //////////////////////////////////

        //gameController.GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
        //Debug.Log("testprint");
        //define the buttons
        //Debug.Log("this is the main menue");
        //mytext.text="this is text";

        MenueShelf01.SetActive(false);
        MenueShelf02.SetActive(true);
        MenueShelf03.SetActive(false);
        MenueShelf04.SetActive(false);
        MenueShelf05.SetActive(false);
        MenueShelf06.SetActive(false);
        MenueShelf07.SetActive(false);

        MenueButton01.SetActive(false);
        MenueButton02.SetActive(false);
        MenueButton03.SetActive(false);
        MenueButton04.SetActive(true);
        MenueButton05.SetActive(true);
        MenueButton06.SetActive(false);
        MenueButton07.SetActive(true);

        if (SceneManaging.isExpert)
        {
            MenueShelf01.SetActive(true);
            MenueShelf02.SetActive(false);

            MenueButton01.SetActive(false);
            MenueButton02.SetActive(true);
            MenueButton03.SetActive(true);
            MenueButton06.SetActive(true);

            SceneManaging.mainMenuActive = 1;
            SceneManaging.configMenueActive = 1;
        }
        ObjectMenueConfigMain.SetActive(true);
        ObjectMenueDirectorMain.SetActive(false);
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

        //if (SceneManaging.mainMenuActive == 1 && SceneManaging.menueActive == 4 && !SceneManaging.isPreviewLoaded)
        //{
        //    StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
        //    StartCoroutine(ButtonShelfI(4));
        //    //Debug.LogWarning("please load");
        //}
        //else
        //{
        //    StartCoroutine(ButtonShelfI(4));
        //}

        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4)
        {
        }
        else
        {
            StartCoroutine(ButtonShelfI(4));
        }
    }

    public void ButtonShelf05(bool fromRail)
    {

        //show menue of buehne or figuren

        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(5));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(5));
            if (fromRail == false)
            {
                Debug.Log("fromrail = false");
                gameController.GetComponent<UIController>().Rails[0].openTimelineByClick(false, gameController.GetComponent<UIController>().Rails[0].timelineImage, true);
            }
        }

    }
    public void ButtonShelf06()
    {
        //Debug.Log("button for shelf02");
        // gameController.GetComponent<UIController>().RailLightBG[0].openCloseTimelineByClick(false,gameController.GetComponent<UIController>().RailLightBG[0].timel);
        //Debug.Log("+++timeline: "+gameController.GetComponent<UIController>().RailLightBG[0].timeSliderImage);
        //show menue of kulissen or licht
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(6));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(6));
        }
    }
    public void ButtonShelf07(bool fromRail)
    {

        //show menue of lichtkonfiguration or musik
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(7));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(7));
            if (fromRail == false)
            {
                Debug.Log("fromrail = false");
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().openTimelineByClick(false, true);
            }
        }
    }
    public void SwitchToMenueDirector()
    {
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(8));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(8));
        }
    }
    public void SwitchToMenueConfig()
    {
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            StartCoroutine(panelPreviewNotLoaded.GetComponent<WarningPanelLoad>().WaitForButtonClick());
            StartCoroutine(ButtonShelfI(9));
            //Debug.LogWarning("please load");
        }
        else
        {
            StartCoroutine(ButtonShelfI(9));
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
        MenueShelf04.SetActive(false);
        MenueShelf05.SetActive(false);
        MenueShelf06.SetActive(false);
        MenueShelf07.SetActive(false);

        MenueButton01.SetActive(false);
        MenueButton02.SetActive(true);
        MenueButton03.SetActive(false);
        MenueButton04.SetActive(true);
        MenueButton05.SetActive(true);
        MenueButton06.SetActive(false);
        MenueButton07.SetActive(true);

        if (SceneManaging.isExpert)
        {
            MenueShelf01.SetActive(true);
            MenueShelf02.SetActive(false);

            MenueButton01.SetActive(false);
            MenueButton02.SetActive(true);
            MenueButton03.SetActive(true);
            MenueButton06.SetActive(true);
        }

        switch (shelfNumber)
        {
            case 1:         // MenueBuehne
                MenueShelf01.SetActive(true);
                MenueButton01.SetActive(false);
                SceneManaging.configMenueActive = 1;
                SceneManaging.mainMenuActive = 1;
                Debug.Log("shelf 1");
                break;
            case 2:         // MenueKulissen
                ObjectMenueConfigMain.SetActive(true);
                MenueShelf02.SetActive(true);
                MenueButton02.SetActive(false);
                ObjectMenueDirectorMain.SetActive(false);
                //gameController.GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
                SceneManaging.mainMenuActive = 1;
                SceneManaging.configMenueActive = 2;
                Debug.Log("shelf 2");
                break;
            case 3:         // MenueLicht
                MenueShelf03.SetActive(true);
                MenueButton03.SetActive(false);
                SceneManaging.mainMenuActive = 1;
                SceneManaging.configMenueActive = 3;
                Debug.Log("shelf 3");
                break;
            case 4:         // MenueLadenSpeichern
                ObjectMenueConfigMain.SetActive(true);
                MenueShelf04.SetActive(true);
                MenueButton04.SetActive(false);
                ObjectMenueDirectorMain.SetActive(false);
                //gameController.GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
                SceneManaging.mainMenuActive = 1;
                SceneManaging.configMenueActive = 4;
                Debug.Log("shelf 4");
                break;
            case 5:         // MenueObjectsShelf
                ObjectMenueDirectorMain.SetActive(true);
                MenueShelf05.SetActive(true);
                MenueButton05.SetActive(false);
                ObjectMenueConfigMain.SetActive(false);
                //gameController.GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
                SceneManaging.mainMenuActive = 2;
                SceneManaging.directorMenueActive = 1;
                Debug.Log("shelf 5");
                break;
            case 6:         // MenueLightShelf
                MenueShelf06.SetActive(true);
                MenueButton06.SetActive(false);
                SceneManaging.mainMenuActive = 2;
                SceneManaging.directorMenueActive = 2;
                Debug.Log("shelf 6");
                break;
            case 7:         // MenueMusicShelf
                ObjectMenueDirectorMain.SetActive(true);
                MenueShelf07.SetActive(true);
                MenueButton07.SetActive(false);
                ObjectMenueConfigMain.SetActive(false);
                //gameController.GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
                SceneManaging.mainMenuActive = 2;
                SceneManaging.directorMenueActive = 3;
                Debug.Log("shelf 7");
                break;
            case 8:         // MenueDirectorMain
                ObjectMenueDirectorMain.SetActive(true);

                /////////////////////////////////// for VISITOR-Tool
                SceneManaging.mainMenuActive = 2;
                //////////////////////////////////

                MenueShelf05.SetActive(true);
                MenueButton05.SetActive(false);
                SceneManaging.directorMenueActive = 1;

                ObjectMenueConfigMain.SetActive(false);
                //gameController.GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
                break;
            case 9:         // MenueConfigMain
                ObjectMenueConfigMain.SetActive(true);
                /////////////////////////////////// for VISITOR-Tool
                SceneManaging.mainMenuActive = 1;
                //////////////////////////////////

                MenueShelf02.SetActive(true);
                MenueButton02.SetActive(false);
                SceneManaging.configMenueActive = 2;

                ObjectMenueDirectorMain.SetActive(false);

                //gameController.GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
                break;
        }
        StaticSceneData.Everything3D();
        gameController.GetComponent<UIController>().Rails[0].GetComponent<RailManager>().PublicUpdate();
        SceneManaging.isPreviewLoaded = true;

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
