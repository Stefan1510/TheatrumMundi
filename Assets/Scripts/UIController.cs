using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject goMenueKulissen;
    public GameObject[] goCollection = null;
    public GameObject[] goButtonSceneryElements;
    public GameObject[] goButtonFigureObjects;
    public GameObject[] goReiterActive;
    public GameObject[] goIndexTabs;
    private objectsLightElement[] objectsLightElements;
    private FigureInstanceElement[] figureInstanceElements;
    public RailManager[] Rails;

    public RailLightManager[] RailLightBG;
    public RailMusicManager RailMusic;

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
        for (int i = 0; i < StaticSceneData.StaticData.sceneryElements.Count; i++)
        {
            if (StaticSceneData.StaticData.sceneryElements[i].active)
            {
                goButtonSceneryElements[i].transform.SetParent(goCollection[StaticSceneData.StaticData.sceneryElements[i].railnumber - 1].transform);
                goButtonSceneryElements[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(StaticSceneData.StaticData.sceneryElements[i].z * 270, (StaticSceneData.StaticData.sceneryElements[i].y - 0.02f) * 260);
                SceneManaging.statusReiter = StaticSceneData.StaticData.sceneryElements[i].railnumber;
                //Spiegelung
                if(StaticSceneData.StaticData.sceneryElements[i].mirrored) goButtonSceneryElements[i].GetComponent<RectTransform>().localScale = new Vector2(-1,1);
                // Größe der Kulissen
                goButtonSceneryElements[i].GetComponent<RectTransform>().sizeDelta = new Vector2(goMenueKulissen.GetComponent<CoulissesManager>().schieneBild.GetComponent<RectTransform>().rect.width / 410 * goMenueKulissen.GetComponent<CoulissesManager>().coulisses[i].GetComponent<CoulisseStats>().CoulisseWidth, goMenueKulissen.GetComponent<CoulissesManager>().schieneBild.GetComponent<RectTransform>().rect.width / 410 * goMenueKulissen.GetComponent<CoulissesManager>().coulisses[i].GetComponent<CoulisseStats>().CoulisseHeight);
                goButtonSceneryElements[i].GetComponent<BoxCollider2D>().size = goMenueKulissen.GetComponent<CoulissesManager>().coulisses[i].GetComponent<RectTransform>().sizeDelta;
            }
            else
            {
                goButtonSceneryElements[i].transform.SetParent(goMenueKulissen.GetComponent<CoulissesManager>().parentStart[i].transform);
                goButtonSceneryElements[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                //goButtonSceneryElements[i].GetComponent<DragDrop>().schieneKulisse = 0;
            }

            // Debug.Log("----- Schiene-Kulisse: +++" + StaticSceneData.StaticData.sceneryElements[i].railnumber + "+++");
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
                        case 3:
                            gameObjectLe.GetComponent<LightHbController>().thisLightElement = StaticSceneData.StaticData.lightElements.Find(DataLe => DataLe.name == le.name);
                            gameObjectLe.GetComponent<LightHbController>().SliderHb.value = le.intensity;
                            break;
                    }
                }
            }
        }
    }

    public void FiguresApplyToUI()
    {

    }
}
