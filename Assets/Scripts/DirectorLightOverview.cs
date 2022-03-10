 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectorLightOverview : MonoBehaviour
{
    [SerializeField] private string _thisLight;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        showLightField();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void showLightField()
    {
        //Debug.Log(StaticSceneData.StaticData.lightElements[_thisLight].name);
        int lightIndex = StaticSceneData.StaticData.lightElements.FindIndex(le => le.name == _thisLight);
        bool lightElementsActive = StaticSceneData.StaticData.lightElements[lightIndex].active;
        if (lightElementsActive)
        {
            GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
    }
}
