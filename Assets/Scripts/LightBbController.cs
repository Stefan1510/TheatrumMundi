using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBbController : MonoBehaviour
{
    public Toggle toggleBb;
    [HideInInspector] public LightElement thisLightElement;
    [HideInInspector] public Image PanelBbImage;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(name);
        thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
        LightAmbientChange(0.0f);
        //Debug.Log(thisLightElement.name);
        //LightActivation(toggleBb.isOn);
        PanelBbImage = toggleBb.transform.parent.parent.gameObject.GetComponent<Image>();
        PanelBbImage.color = new Color(171f / 255f, 171f / 255f, 171f / 255f, 160f / 255f);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void LightActivation(bool onOffSwitch)
    {
        thisLightElement.active = onOffSwitch;
        GetComponent<Light>().enabled = onOffSwitch;
        //gameController.GetComponent<SceneDataController>().LightsApplyToScene(StaticSceneData.StaticData.lightElements);
        //StaticSceneData.Lights3D();
        if (onOffSwitch)
        {
            PanelBbImage.color = new Color(205f / 255f, 176f / 255f, 42f / 255f, 160f / 255f);
        }
        else
        {
            PanelBbImage.color = new Color(171f / 255f, 171f / 255f, 171f / 255f, 160f / 255f);
        }
    }

    public void LightAmbientChange(float intensity)
    {
        RenderSettings.ambientLight = new Color(intensity, intensity, intensity);
    }
}
