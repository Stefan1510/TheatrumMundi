using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBbController : MonoBehaviour
{
    public Toggle toggleBb;
    [HideInInspector] public LightElement thisLightElement;
    [HideInInspector] public Image PanelBbImage;
    void Start()
    {
        thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
        //if (SceneManaging.isExpert) LightAmbientChange(0.0f);
        PanelBbImage = toggleBb.transform.parent.parent.gameObject.GetComponent<Image>();
        PanelBbImage.color = new Color(171f / 255f, 171f / 255f, 171f / 255f, 160f / 255f);
        if (SceneManaging.isExpert)
            LightActivation(true);
    }

    public void LightActivation(bool onOffSwitch)
    {
        thisLightElement.active = onOffSwitch;
        GetComponent<Light>().enabled = onOffSwitch;
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
