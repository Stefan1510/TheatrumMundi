using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMenues : MonoBehaviour
{
    public GameObject ObjectMenueConfigMain;
    public GameObject ObjectMenueDirectorMain;
    public GameObject _canvas;

    void Start()
    {
        SwitchToMenueConfig();
    }

    public void SwitchToMenueDirector()
    {
        ObjectMenueConfigMain.SetActive(false);
        ObjectMenueDirectorMain.SetActive(true);
        StaticSceneData.Everything3D();
        GetComponent<UIController>().menueFiguresContent.GetComponent<RailManager>().PublicUpdate();

        /////////////////////////////////// for VISITOR-Tool
        SceneManaging.directorMenueActive = 1;
        SceneManaging.mainMenuActive = 2;
        _canvas.GetComponent<ObjectShelfAll>().ButtonShelf05(false);
        //////////////////////////////////
    }
    public void SwitchToMenueConfig()
    {
        /////////////////////////////////// for VISITOR-Tool
        //SceneManaging.configMenueActive = 2;
        SceneManaging.mainMenuActive = 1;
        //////////////////////////////////

        ObjectMenueDirectorMain.SetActive(false);
        ObjectMenueConfigMain.SetActive(true);
        _canvas.GetComponent<ObjectShelfAll>().ButtonShelf01();
        StaticSceneData.Everything3D();
        //GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
    }
}
