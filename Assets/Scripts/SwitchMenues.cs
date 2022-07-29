using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMenues : MonoBehaviour
{
    public GameObject ObjectMenueConfigMain;
    public GameObject ObjectMenueDirectorMain;

    // Start is called before the first frame update
    void Start()
    {
        SwitchToMenueConfig();
    }

    public void SwitchToMenueDirector()
    {
        ObjectMenueConfigMain.SetActive(false);
        ObjectMenueDirectorMain.SetActive(true);
        StaticSceneData.Everything3D();
        GetComponent<UIController>().Rails[0].GetComponent<RailManager>().PublicUpdate();

        /////////////////////////////////// for VISITOR-Tool
        SceneManaging.directorMenueActive = 1;
        SceneManaging.mainMenuActive = 2;
        ObjectMenueDirectorMain.GetComponent<ObjectShelf>().ButtonShelf01();
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
        StaticSceneData.Everything3D();
        GetComponent<UnitySwitchExpertUser>().DeactivateExpertTools();
    }
}
