using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectShelfAll : MonoBehaviour
{
    #region variables
    public GameObject MenueShelf01;
    public GameObject MenueShelf02;
    public GameObject MenueShelf03;
    public GameObject MenueShelf04;
    public GameObject MenueShelf05;
    public GameObject MenueShelf06;
    public GameObject MenueShelf07;
    public GameObject MenueShelf08;

    public GameObject MenueButton01;    //ButtonBuehne          -- only expert
    public GameObject MenueButton02;    //ButtonKulissen
    public GameObject MenueButton03;    //ButtonLicht           -- only expert
    public GameObject MenueButton04;    //ButtonLadenSpeichern
    public GameObject MenueButton05;    //ButtonObjects
    public GameObject MenueButton06;    //ButtonLights          -- only expert
    public GameObject MenueButton07;    //ButtonMusic
    public GameObject MenueButton08;    //ButtonSaveDirector    -- only expert

    public GameObject ObjectMenueDirectorMain;
    public GameObject ObjectMenueConfigMain;
    public RailManager MenueContentRails;
    public GameObject ButtonMenueConfig;    // Der Button in ObjectMenueDirectorMain der zum ConfigMenu leitet
    public GameObject ButtonMenueDirector;  // Der Button in ObjectMenueConfigMain der zum DirectorMenu leitet

    [SerializeField] private GameObject viewportSaveDir, viewportSaveConfig, contentSave;

    public GameObject panelPreviewNotLoaded;
    [SerializeField] private UTJ.FrameCapturer.PlayerControls playerCtrls;

    public GameObject gameController;//, overlayWaiting;
    private int tmpShelfNumber;
    [SerializeField] private TimeSliderController tmpSlider;

    #endregion
    void Start()
    {
        /////////////////////////////////// for VISITOR-Tool
        SceneManaging.mainMenuActive = 1;
        SceneManaging.configMenueActive = 2;
        //////////////////////////////////

        MenueShelf01.SetActive(false);
        MenueShelf02.SetActive(true);
        MenueShelf03.SetActive(false);
        MenueShelf04.SetActive(false);
        MenueShelf05.SetActive(false);
        MenueShelf06.SetActive(false);
        MenueShelf07.SetActive(false);
        MenueShelf08.SetActive(false);

        MenueButton01.SetActive(false);
        MenueButton02.SetActive(false);
        MenueButton03.SetActive(false);
        MenueButton04.SetActive(true);
        MenueButton05.SetActive(true);
        MenueButton06.SetActive(false);
        MenueButton07.SetActive(true);
        MenueButton08.SetActive(true);

        if (SceneManaging.isExpert)
        {
            MenueShelf01.SetActive(false);
            MenueShelf02.SetActive(false);

            MenueButton01.SetActive(true);
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
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive && !SceneManaging.dialogActive)
        {
            //overlayWaiting.SetActive(true);
            if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
            {
                panelPreviewNotLoaded.SetActive(true);
                tmpShelfNumber = 1;
            }
            else
            {
                ButtonShelfI(1);
            }
        }
    }
    public void ButtonShelf02()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive && !SceneManaging.dialogActive)
        {
            if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
            {
                panelPreviewNotLoaded.SetActive(true);
                tmpShelfNumber = 2;
            }
            else ButtonShelfI(2);
        }
    }
    public void ButtonShelf03()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive && !SceneManaging.dialogActive)
        {
            //show menue of lichtkonfiguration or musik
            if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
            {
                panelPreviewNotLoaded.SetActive(true);
                tmpShelfNumber = 3;
            }
            else ButtonShelfI(3);
        }
    }
    public void ButtonShelf04()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive && !SceneManaging.dialogActive)
        {
            ButtonShelfI(4);
        }
    }
    public void ButtonShelf05(bool fromRail)
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive && !SceneManaging.dialogActive)
        {
            ButtonShelfI(5);
            if (fromRail == false)
            {
                MenueContentRails.OpenTimelineByClick(false, 0, true);
            }
        }
    }
    public void ButtonShelf06()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive && !SceneManaging.dialogActive)
        {
            ButtonShelfI(6);
        }
    }
    public void ButtonShelf07(bool fromRail)
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive && !SceneManaging.dialogActive)
        {
            ButtonShelfI(7);
            if (fromRail == false)
            {
                gameController.GetComponent<UIController>().RailMusic.GetComponent<RailMusicManager>().OpenTimelineByClick(false, true);
            }
        }
    }
    public void ButtonShelf08()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.railLengthDialogActive && !SceneManaging.dialogActive)
        {
            ButtonShelfI(8);
        }
    }
    public void SwitchToMenueDirector()
    {
        // overlayWaiting.SetActive(true);
        if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 4 && !SceneManaging.isPreviewLoaded)
        {
            panelPreviewNotLoaded.SetActive(true);
            tmpShelfNumber = 10;
        }
        else
        {
            ButtonShelfI(10);
        }
    }
    public void SwitchToMenueConfig()
    {
        ButtonShelfI(9);
    }

    private void ButtonShelfI(int shelfNumber)
    {
        MenueShelf02.GetComponent<CoulissesManager>().HighlightRail(false);

        MenueShelf01.SetActive(false);
        MenueShelf02.SetActive(false);
        MenueShelf03.SetActive(false);
        MenueShelf04.SetActive(false);
        MenueShelf05.SetActive(false);
        MenueShelf06.SetActive(false);
        MenueShelf07.SetActive(false);
        MenueShelf08.SetActive(false);

        MenueButton01.SetActive(false);
        MenueButton02.SetActive(true);
        MenueButton03.SetActive(false);
        MenueButton04.SetActive(true);
        MenueButton05.SetActive(true);
        MenueButton06.SetActive(false);
        MenueButton07.SetActive(true);
        MenueButton08.SetActive(false);

        if (SceneManaging.isExpert)
        {
            MenueShelf01.SetActive(false);
            MenueShelf02.SetActive(false);

            MenueButton01.SetActive(true);
            MenueButton02.SetActive(true);
            MenueButton03.SetActive(true);
            MenueButton06.SetActive(true);
            MenueButton08.SetActive(true);
        }

        switch (shelfNumber)
        {
            case 1:         // MenueBuehne
                MenueShelf01.SetActive(true);
                MenueButton01.SetActive(false);
                SceneManaging.configMenueActive = 1;
                SceneManaging.mainMenuActive = 1;
                break;
            case 2:         // MenueKulissen
                ObjectMenueConfigMain.SetActive(true);
                MenueShelf02.SetActive(true);
                MenueButton02.SetActive(false);
                ObjectMenueDirectorMain.SetActive(false);
                SceneManaging.mainMenuActive = 1;
                SceneManaging.configMenueActive = 2;
                if (AnimationTimer.GetTimerState() == AnimationTimer.TimerState.playing)
                {
                    playerCtrls.ButtonPlay();
                }
                MenueShelf02.GetComponent<CoulissesManager>().HighlightRail(true);
                break;
            case 3:         // MenueLichtConfig
                MenueShelf03.SetActive(true);
                MenueButton03.SetActive(false);
                SceneManaging.mainMenuActive = 1;
                SceneManaging.configMenueActive = 3;
                break;
            case 4:         // MenueLadenSpeichern
                ObjectMenueConfigMain.SetActive(true);
                MenueShelf04.SetActive(true);
                MenueButton04.SetActive(false);
                ObjectMenueDirectorMain.SetActive(false);
                SceneManaging.mainMenuActive = 1;
                SceneManaging.configMenueActive = 4;
                contentSave.transform.SetParent(viewportSaveConfig.transform);
                break;
            case 5:         // MenueObjectsShelf
                ObjectMenueDirectorMain.SetActive(true);
                MenueShelf05.SetActive(true);
                MenueButton05.SetActive(false);
                ObjectMenueConfigMain.SetActive(false);
                SceneManaging.mainMenuActive = 2;
                SceneManaging.directorMenueActive = 1;
                break;
            case 7:         // MenueMusicShelf
                ObjectMenueDirectorMain.SetActive(true);
                MenueShelf07.SetActive(true);
                MenueButton07.SetActive(false);
                ObjectMenueConfigMain.SetActive(false);
                SceneManaging.mainMenuActive = 2;
                SceneManaging.directorMenueActive = 3;
                break;
            case 8:         // MenueLadenSpeichern Director
                ObjectMenueConfigMain.SetActive(false);
                MenueShelf08.SetActive(true);
                MenueButton08.SetActive(false);
                ObjectMenueDirectorMain.SetActive(true);
                SceneManaging.mainMenuActive = 2;
                SceneManaging.directorMenueActive = 3;
                contentSave.transform.SetParent(viewportSaveDir.transform);
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
                if (AnimationTimer.GetTimerState() == AnimationTimer.TimerState.playing)
                {
                    playerCtrls.ButtonPlay();
                }
                break;
            case 10:         // MenueDirectorMain
                ObjectMenueDirectorMain.SetActive(true);

                /////////////////////////////////// for VISITOR-Tool
                SceneManaging.mainMenuActive = 2;
                //////////////////////////////////

                MenueShelf05.SetActive(true);
                MenueButton05.SetActive(false);
                SceneManaging.directorMenueActive = 1;

                ObjectMenueConfigMain.SetActive(false);
                break;
        }
        MenueContentRails.PublicUpdate();
        SceneManaging.isPreviewLoaded = true;
    }

    public void OnClickIgnore()
    {
        ButtonShelfI(tmpShelfNumber);
        panelPreviewNotLoaded.SetActive(false);
        StaticSceneData.Everything3D();
    }
    public void OnClickBack()
    {
        panelPreviewNotLoaded.SetActive(false);
    }
}
