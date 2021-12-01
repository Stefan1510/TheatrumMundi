using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMenues : MonoBehaviour
{
    public GameObject ObjectMenueConfigMain;
    public GameObject ObjectMenueDirectorMain;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SwitchToMenueDirector()
    {
        ObjectMenueConfigMain.SetActive(false);
        ObjectMenueDirectorMain.SetActive(true);
        StaticSceneData.everything3D();
    }
    public void SwitchToMenueConfig()
    {
        ObjectMenueDirectorMain.SetActive(false);
        ObjectMenueConfigMain.SetActive(true);
    }
}
