using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LightController : MonoBehaviour
{
    public Toggle toggleLb;
    public Slider sliderLbBrightness;
    public Slider sliderLbPosition;
    public Slider sliderLbHeight;
    public Slider sliderLbHorizontal;
    public Slider sliderLbVertival;
    //private void Awake()
    //{

    //}

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void LightActivation(bool onOffSwitch)
    {

        //lightElements müssen erst noch mit der StaticData bekannt gemacht werden --> SceneDataController

        LightElement thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
        thisLightElement.active = onOffSwitch;
        //gameController.GetComponent<SceneDataController>().LightsApplyToScene(StaticSceneData.StaticData.lightElements);
        StaticSceneData.Lights3D();
    }

    public void ChangeBrightness(float brightnessValue)
    {
        GetComponent<Light>().intensity = brightnessValue;
    }

    public void LightAmbientChange(float intensity)
    {
        RenderSettings.ambientLight = new Color(intensity, intensity, intensity);
    }
}
