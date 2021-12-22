using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject goMenueKulissen;
    public GameObject[] goCollection = null;
    public GameObject[] goButtonSceneryElements;
    private objectsLightElement[] objectsLightElements;


    //// Start is called before the first frame update
    void Start()
    {
        objectsLightElements = GetComponent<SceneDataController>().objectsLightElements;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SceneriesApplyToUI()
    {
        //string buttonName;
        //GameObject goButtonScenery;
        
        goMenueKulissen.SetActive(true);
        foreach (SceneryElement se in StaticSceneData.StaticData.sceneryElements)
        {
            foreach (GameObject buttonSe in goButtonSceneryElements)
            {
                if ("Button" + se.name == buttonSe.name)
                {
                    buttonSe.GetComponent<DragDrop>().ThisSceneryElement = StaticSceneData.StaticData.sceneryElements.Find(DataSe => DataSe.name == se.name);
                    if (se.active)
                    {
                        buttonSe.transform.SetParent(goCollection[se.railnumber - 1].transform);
                        buttonSe.GetComponent<RectTransform>().anchoredPosition = new Vector2(se.z * 300, (se.y - 0.1f) * 300);
                        buttonSe.GetComponent<DragDrop>().schieneKulisse = se.railnumber;
                        SceneManager.statusReiter = se.railnumber;
                    }
                    else 
                    {
                        buttonSe.transform.SetParent(buttonSe.GetComponent<DragDrop>().parentStart.transform);
                        buttonSe.GetComponent<RectTransform>().anchoredPosition = buttonSe.GetComponent<DragDrop>().pos;
                        buttonSe.GetComponent<DragDrop>().schieneKulisse = 0;
                        //buttonSe.GetComponent<DragDrop>().statusReiter = 1;
                    }

                    //GameObject.Find("Reiter" + se.railnumber + "Active").SetActive(false);
                    Debug.Log("----- Schiene-Kulisse: +++" + se.railnumber + "+++");
                }
            }
        }

        goMenueKulissen.SetActive(false);
    }

    public void LightsApplyToUI()
    {
        foreach (LightElement le in StaticSceneData.StaticData.lightElements)
        {
            foreach (objectsLightElement objectLightElement in objectsLightElements)
            {
                if (le.name == objectLightElement.goLightElement.name)
                {
                    GameObject gameObjectLe = objectLightElement.goLightElement;
                    switch (objectLightElement.lightStagePosition)
                    {
                        case 0:
                            gameObjectLe.GetComponent<LightBbController>().thisLightElement = StaticSceneData.StaticData.lightElements.Find(DataLe => DataLe.name == le.name);
                            gameObjectLe.GetComponent<LightBbController>().toggleBb.isOn = le.active;
                            break;
                        case 1:
                            gameObjectLe.GetComponent<LightController>().thisLightElement = StaticSceneData.StaticData.lightElements.Find(DataLe => DataLe.name == le.name);
                            gameObjectLe.GetComponent<LightController>().toggleLb.isOn = le.active;
                            gameObjectLe.GetComponent<LightController>().sliderLbIntensity.value = le.intensity;
                            gameObjectLe.GetComponent<LightController>().sliderLbPosition.value = le.z;
                            gameObjectLe.GetComponent<LightController>().sliderLbHeight.value = le.y;
                            gameObjectLe.GetComponent<LightController>().sliderLbHorizontal.value = le.angle_h;
                            gameObjectLe.GetComponent<LightController>().sliderLbVertical.value = le.angle_v;
                            break;
                        case 2:
                            gameObjectLe.GetComponent<LightController>().thisLightElement = StaticSceneData.StaticData.lightElements.Find(DataLe => DataLe.name == le.name);
                            gameObjectLe.GetComponent<LightController>().toggleLb.isOn = le.active;
                            gameObjectLe.GetComponent<LightController>().sliderLbIntensity.value = le.intensity;
                            gameObjectLe.GetComponent<LightController>().sliderLbPosition.value = le.z;
                            gameObjectLe.GetComponent<LightController>().sliderLbHeight.value = le.y;
                            gameObjectLe.GetComponent<LightController>().sliderLbHorizontal.value = le.angle_h;
                            gameObjectLe.GetComponent<LightController>().sliderLbVertical.value = le.angle_v;
                            break;
                    }
                }
            }
        }
    }
}
