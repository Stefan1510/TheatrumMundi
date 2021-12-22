using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBbController : MonoBehaviour
{
    public Toggle toggleBb;
    [HideInInspector] public LightElement thisLightElement;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(name);
        thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
        //Debug.Log(thisLightElement.name);
        //LightActivation(toggleBb.isOn);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void LightActivation(bool onOffSwitch)
    {
        thisLightElement.active = onOffSwitch;
        //gameController.GetComponent<SceneDataController>().LightsApplyToScene(StaticSceneData.StaticData.lightElements);
        StaticSceneData.Lights3D();
    }

    public void LightAmbientChange(float intensity)
    {
        RenderSettings.ambientLight = new Color(intensity, intensity, intensity);
    }
}
