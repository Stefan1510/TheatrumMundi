using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightHbController : MonoBehaviour
{
    public Slider SliderHb;
    [HideInInspector] public LightElement thisLightElement;
    // Start is called before the first frame update
    void Start()
    {
        ChangeIntensity(SliderHb.value);
        thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
    }

    public void ChangeIntensity(float intensityValue)
    {
        thisLightElement.intensity = intensityValue;
        GetComponent<Light>().intensity = intensityValue;
        //StaticSceneData.Lights3D();
    }
}
