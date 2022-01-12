using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHbController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeIntensity(0.5f);
    }

    public void ChangeIntensity(float intensityValue)
    {
        //thisLightElement.intensity = intensityValue;
        GetComponent<Light>().intensity = intensityValue;
        //StaticSceneData.Lights3D();
    }
}
