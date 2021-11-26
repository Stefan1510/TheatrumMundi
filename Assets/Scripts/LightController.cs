using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LightActivation(bool onOffSwitch)
    {
        GetComponent<Light>().enabled = onOffSwitch;

        //lightElements müssen erst noch mit der StaticData bekannt gemacht werden --> SceneDataController

        //LightElement thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
        //thisLightElement.active = onOffSwitch;
        
        //Funktion im Controller, die nur die Lichter aktualisiert
        //Für jede Gruppe Schienen, Kulissen, Figuren, Lichter je eine Funktion zum updaten in der Szene.
    }

    public void LightAmbientChange(float intensity)
    {
        RenderSettings.ambientLight = new Color(intensity, intensity, intensity);
    }
}
