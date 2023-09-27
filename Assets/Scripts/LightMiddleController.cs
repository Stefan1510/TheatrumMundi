using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightMiddleController : MonoBehaviour
{
    [SerializeField] private Slider sliderIntensity;
    private Light spotLight;
    public LightElement thisLightElement;
    void Awake()
    {
        LightActivation(false);
        spotLight = gameObject.GetComponent<Light>();
    }
    void Start()
    {
        thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
    }
    public void ChangeIntensity()
    {
        spotLight.intensity = sliderIntensity.value / 3;
        thisLightElement.intensity = spotLight.intensity;
    }
    public void LightActivation(bool on)
    {
        thisLightElement.active = on;
        GetComponent<Light>().enabled = on;
    }
}
