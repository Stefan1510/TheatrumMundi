using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public CoulissesManager goMenueKulissen;
    public GameObject[] goCollection = null;
    public GameObject[] goButtonSceneryElements;
    public GameObject[] goReiterActive;
    public GameObject[] goIndexTabs;
    public GameObject[] goRailDistSliders;
    public GameObject[] goRailHeightSliders;
    public RailManager[] Rails;
    public GameObject menueFiguresContent;
    public RailLightManager[] RailLightBG;
    public RailMusicManager RailMusic;
    //public CoulissesManager CoulissesMan;
    private FigureInstanceElement[] figureInstanceElements;
    private objectsLightElement[] objectsLightElements;
    private GameObject[] goButtonFigureObjects;
    private int currenScreenWidth;

    private void Awake()
    {
        currenScreenWidth = Screen.width;
        objectsLightElements = GetComponent<SceneDataController>().objectsLightElements;
    }
    // Update is called once per frame
    void Update()
    {
        if (Screen.width != currenScreenWidth)
        {
            // for(int i=0;i<Rails.Length;i++)
            // {
            //     Rails[i].ResetScreenSize();   
            // // }
            RailMusic.ResetScreenSize();
            goMenueKulissen.ResetScreenSize();
            // RailLightBG[0].ResetScreenSize();
            // RailLightBG[1].ResetScreenSize();
            currenScreenWidth = Screen.width;
        }
    }

    public void SceneriesApplyToUI()
    {
        int[] count = new int[8];
        goMenueKulissen.gameObject.SetActive(true);
        for (int i = 0; i < StaticSceneData.StaticData.sceneryElements.Count; i++)
        {
            if (StaticSceneData.StaticData.sceneryElements[i].active)
            {
                goButtonSceneryElements[i].transform.SetParent(goCollection[StaticSceneData.StaticData.sceneryElements[i].railnumber - 1].transform);
                goButtonSceneryElements[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(StaticSceneData.StaticData.sceneryElements[i].z * 270, (StaticSceneData.StaticData.sceneryElements[i].y - 0.515f) * 260);
                //SceneManaging.statusReiter = StaticSceneData.StaticData.sceneryElements[i].railnumber;
                //Spiegelung
                if (StaticSceneData.StaticData.sceneryElements[i].mirrored) goButtonSceneryElements[i].GetComponent<RectTransform>().localScale = new Vector2(-1, 1);
                // Größe der Kulissen
                goButtonSceneryElements[i].GetComponent<RectTransform>().sizeDelta = goButtonSceneryElements[i].GetComponent<BoxCollider2D>().size = new Vector2(goMenueKulissen.GetComponent<CoulissesManager>().railWidth / 410 * goButtonSceneryElements[i].GetComponent<CoulisseStats>().CoulisseWidth / goButtonSceneryElements[i].transform.lossyScale.x, goMenueKulissen.GetComponent<CoulissesManager>().railWidth / 410 * goButtonSceneryElements[i].GetComponent<CoulisseStats>().CoulisseHeight / goButtonSceneryElements[i].transform.lossyScale.y);
                goButtonSceneryElements[i].GetComponent<BoxCollider2D>().offset = new Vector2(0, goButtonSceneryElements[i].GetComponent<RectTransform>().rect.height / 2);

                // add active elements to "coulissesOnRails"
                goMenueKulissen.coulissesOnRails.Add(goButtonSceneryElements[i]);
                count[StaticSceneData.StaticData.sceneryElements[i].railnumber - 1]++;
            }
            else
            {
                goButtonSceneryElements[i].transform.SetParent(goMenueKulissen.parentStart[i].transform);
            }
        }
        for (int i = 0; i < goMenueKulissen.coulisseCounter.Length; i++)
        {
            goMenueKulissen.coulisseCounter[i].text = count[i].ToString();
        }
        goMenueKulissen.gameObject.SetActive(false);
    }

    public void LightsApplyToUI()
    {
        // if (SceneManaging.isExpert)
        // {
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
        // }
    }

    public void RailsApplyToUI()
    {
        if (SceneManaging.isExpert)
        {
            for (int i = 0; i < goRailDistSliders.Length; i++)
            {
                //if (i == 0)
                //{
                //    //Debug.LogWarning("odd index // railname: " + goRailDistSliders[i].name + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
                //    //goRailDistSliders[i].GetComponent<Slider>().value = -(StaticSceneData.StaticData.railElements[i + 1].x * 2);
                //    goRailDistSliders[i].GetComponent<Slider>().value = -(StaticSceneData.StaticData.railElements[i + 1].x + 0.12f) * 2;
                //    //Debug.LogWarning("railname: " + goRailDistSliders[i].name + " slider: " + goRailDistSliders[i].GetComponent<Slider>().value + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
                //}
                //else if (i == 1)
                //{
                //    //Debug.LogWarning("odd index // railname: " + goRailDistSliders[i].name + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
                //    //goRailDistSliders[i].GetComponent<Slider>().value = -(StaticSceneData.StaticData.railElements[i + 1].x + 0.44f) * 2;
                //    goRailDistSliders[i].GetComponent<Slider>().value = -(StaticSceneData.StaticData.railElements[i + 1].x + 0.2f) * 2;
                //    //Debug.LogWarning("railname: " + goRailDistSliders[i].name + " slider: " + goRailDistSliders[i].GetComponent<Slider>().value + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
                //}
                //else if (i%2==0)
                if (i % 2 == 0)
                {
                    //Debug.LogWarning("even index // railname: " + goRailDistSliders[i].name + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
                    goRailDistSliders[i].GetComponent<Slider>().value = -(StaticSceneData.StaticData.railElements[i + 1].x + 0.12f) * 2;
                    //Debug.LogWarning("railname: " + goRailSliders[i].name + " slider: " + goRailSliders[i].GetComponent<Slider>().value + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
                }
                else
                {
                    //Debug.LogWarning("odd index // railname: " + goRailDistSliders[i].name + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
                    goRailDistSliders[i].GetComponent<Slider>().value = -(StaticSceneData.StaticData.railElements[i + 1].x + 0.2f) * 2;
                }
                goRailDistSliders[i].GetComponent<sliderValueToInputValue>().OnSliderChange();
                //Debug.LogWarning("railname: " + goRailDistSliders[i].name + " slider: " + goRailDistSliders[i].GetComponent<Slider>().value + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
            }

            for (int i = 0; i < goRailHeightSliders.Length; i++)
            {
                //Debug.LogWarning(" index // railname: " + goRailHeightSliders[i].name + " static: " + StaticSceneData.StaticData.railElements[i * 2].name + " " + StaticSceneData.StaticData.railElements[i * 2].y);
                if (i == 0)
                {
                    goRailHeightSliders[i].GetComponent<Slider>().value = (StaticSceneData.StaticData.railElements[i * 2].y - 0.06f) * 2;
                }
                else
                {
                    goRailHeightSliders[i].GetComponent<Slider>().value = (StaticSceneData.StaticData.railElements[i * 2].y * 2);
                }
                goRailHeightSliders[i].GetComponent<sliderValueToInputValue>().OnSliderChange();
                //Debug.LogWarning("railname: " + goRailHeightSliders[i].name + " slider: " + goRailHeightSliders[i].GetComponent<Slider>().value + " static: " + StaticSceneData.StaticData.railElements[i + 1].x);
            }
        }
    }

    public void FiguresApplyToUI()
    {

    }
}
