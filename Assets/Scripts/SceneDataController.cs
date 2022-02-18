using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SceneDataController : MonoBehaviour
{
    public InputField inputFieldFileName;
    public InputField inputFieldFileAuthor;
    public InputField inputFieldFileComment;
    public GameObject[] objectsRailElements;
    public GameObject[] objectsSceneryElements;
    public GameObject[] objectsFigureElements;
    //public GameObject[] objectsLightElements;
    public objectsLightElement[] objectsLightElements;
    [HideInInspector] public string sceneFileName;
    [HideInInspector] public string sceneFileAuthor;
    [HideInInspector] public string sceneFileDate;
    [HideInInspector] public string sceneFileComment;
    [HideInInspector] public string recentSceneDataSerialized;
    [HideInInspector] public int countActiveSceneryElements = 0;
    [HideInInspector] public int countActiveLightElements = 0;
    [HideInInspector] public int countActiveFigureElements = 0;
    [HideInInspector] public int currentTime;

    [HideInInspector] public SceneData recentSceneData;
    [HideInInspector] public SceneData tempSceneData;

    // Start is called before the first frame updates
    void Awake()
    {
        Debug.Log("***************************** Puffpaff");
        sceneFileName = "-";
        sceneFileAuthor = "-";
        sceneFileDate = "-";
        sceneFileComment = "-";
        //recentSceneData = new SceneData();
        //recentSceneData.railElements = new List<RailElement>();
        //recentSceneData.sceneryElements = new List<SceneryElement>();
        //recentSceneData.figureElements = new List<FigureElement>();
        //recentSceneData.lightElements = new List<LightElement>();
        foreach (GameObject re in objectsRailElements)
        {
            //Debug.Log("-----+++++" + re.name);
        }
        SceneManager.statusReiter = 1;
        foreach (GameObject objectSceneryElement in objectsSceneryElements)
        {
            //Debug.Log("SceneDataController Name: " + objectSceneryElement.name);
            objectSceneryElement.transform.SetParent(GameObject.Find("Schiene1").transform);
            objectSceneryElement.SetActive(false);
        }
        Debug.Log("------- staticSceneDataJSON");

        StaticSceneData.StaticData = CreateSceneData();
    }

    private void Start()
    {

        Debug.Log("------- staticSceneDataJSON" + StaticSceneData.StaticData.ToString());
        Debug.Log("------- staticSceneDataJSON" + CreateJsonFromSceneData(StaticSceneData.StaticData));

    }

    private void GetFileMetaDataFromScene()
    {
        sceneFileName = inputFieldFileName.text;
        sceneFileAuthor = inputFieldFileAuthor.text;
        sceneFileComment = inputFieldFileComment.text;
        sceneFileDate = DateTime.Now.ToString();
    }

    private void SetFileMetaDataToScene()
    {
        inputFieldFileName.text = sceneFileName;
        inputFieldFileAuthor.text = sceneFileAuthor;
        inputFieldFileComment.text = sceneFileComment;
    }

    public SceneData CreateSceneData()
    {
        SceneData sceneData = new SceneData();
        GetFileMetaDataFromScene();
        sceneData.fileName = sceneFileName;
        sceneData.fileAuthor = sceneFileAuthor;
        sceneData.fileComment = sceneFileComment;
        sceneData.fileDate = sceneFileDate;
        Debug.Log(CreateJsonFromSceneData(sceneData));
        //sceneData.railElements.Clear();
        //sceneData.sceneryElements.Clear();
        //sceneData.lightElements.Clear();
        sceneData.railElements = new List<RailElement>();
        sceneData.sceneryElements = new List<SceneryElement>();
        sceneData.figureElements = new List<FigureElement>();
        sceneData.lightElements = new List<LightElement>();
        sceneData.lightingSets = new List<LightingSet>();

        foreach (GameObject objectRailElement in objectsRailElements)
        {
            RailElement sceneRailElement = new RailElement
            {
                name = objectRailElement.name,
                x = objectRailElement.transform.localPosition.x,
                y = objectRailElement.transform.localPosition.y,
                z = objectRailElement.transform.localPosition.z,
                //width=objectRailElement.GetComponent<SceneryController>().bounds.Length,
                //width=objectRailElement.GetComponent<Renderer>().bounds.size.x,
                //height=objectRailElement.GetComponent<Renderer>().bounds.size.y,
                velocity = 1.0f,
                direction = "toRight"
            };
            sceneData.railElements.Add(sceneRailElement);
        }

        //scenery aka kulissen
        foreach (GameObject objectSceneryElement in objectsSceneryElements)
        {
            SceneryElement sceneSceneryElement = new SceneryElement
            {
                name = objectSceneryElement.name,
                //description = objectSceneryElement.transform.parent.GetChild(0).GetComponent<Text>().text,
                x = objectSceneryElement.transform.position.x,
                y = objectSceneryElement.transform.position.y,
                z = objectSceneryElement.transform.position.z,
                active = objectSceneryElement.GetComponent<SceneryController>().sceneryActive,
                parent = objectSceneryElement.transform.parent.name,
                zPos = 0,
                mirrored = false
            };
            sceneData.sceneryElements.Add(sceneSceneryElement);
        }

        //figures
        foreach (GameObject objectFigureElement in objectsFigureElements)
        {
            FigureElement sceneFigureElement = new FigureElement
            {
                name = objectFigureElement.name,
                x = objectFigureElement.transform.position.x,
                y = objectFigureElement.transform.position.y,
                z = objectFigureElement.transform.position.z,
                //active = objectFigureElement.GetComponent<SceneryController>().sceneryActive,	//check, if this command can be set up on a better way ;)
                //parent = objectFigureElement.transform.parent.name,
                mirrored = false,
                //width = objectFigureElement.GetComponent<Renderer>().bounds.size.x,
                //height = objectFigureElement.GetComponent<Renderer>().bounds.size.y,
                width = 0.27f,
                height = 0.16f,
                velocity = 0.0f,
                wheelsize = 1.0f,
                railnumber = 1,
                direction = "toRight"
            };
            sceneData.figureElements.Add(sceneFigureElement);
        }

        //light
        foreach (objectsLightElement objectLightElement in objectsLightElements)
        {
            GameObject gameObjectLe = objectLightElement.goLightElement;
            LightElement sceneLightElement = new LightElement
            {
                name = gameObjectLe.name,
                x = gameObjectLe.transform.localPosition.x,
                y = gameObjectLe.transform.localPosition.y,
                z = gameObjectLe.transform.localPosition.z,
                active = gameObjectLe.GetComponent<Light>().isActiveAndEnabled,
                railnumber = 1,
                r = gameObjectLe.GetComponent<Light>().color.r,
                g = gameObjectLe.GetComponent<Light>().color.g,
                b = gameObjectLe.GetComponent<Light>().color.b,
                intensity = gameObjectLe.GetComponent<Light>().intensity,
                angle_h = 256,
                angle_v = 256,
                stagePosition = objectLightElement.lightStagePosition
            };

            switch (objectLightElement.lightStagePosition)
            {
                case 0:
                    sceneLightElement.y = gameObjectLe.transform.localPosition.y;
                    sceneLightElement.z = gameObjectLe.transform.localPosition.z;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

            sceneData.lightElements.Add(sceneLightElement);
        }

        sceneData.lightingSets.Add(new LightingSet());

        recentSceneData = sceneData;
        return sceneData;
    }

    public void CreateScene(SceneData sceneData)
    {
        sceneFileName = sceneData.fileName;
        sceneFileAuthor = sceneData.fileAuthor;
        sceneFileDate = sceneData.fileDate;
        sceneFileComment = sceneData.fileComment;
        //SetFileMetaDataToScene();
        //Debug.Log(sceneData.railElements[0]);

        //rail elements
        RailsApplyToScene(sceneData.railElements);

        //scenery elements (kulissen)
        SceneriesApplyToScene(sceneData.sceneryElements);

        //light elements
        LightsApplyToScene(sceneData.lightElements);

        //figure elements (Figuren)
        FiguresApplyToScene(sceneData.figureElements);
    }

    public void RailsApplyToScene(List<RailElement> railElements)
    {
        foreach (RailElement re in railElements)
        {
            foreach (GameObject goRailElement in objectsRailElements)
            {
                if (re.name == goRailElement.name)
                {
                    goRailElement.transform.localPosition = new Vector3(re.x, re.y, re.z);
                }
            }
        }
    }

    public void SceneriesApplyToScene(List<SceneryElement> sceneryElements)
    {
        countActiveSceneryElements = 0;
        foreach (SceneryElement se in sceneryElements)
        {
            foreach (GameObject goSceneryElement in objectsSceneryElements)
            {
                if (se.name == goSceneryElement.name)
                {
                    se.railnumber = int.Parse(se.parent.Substring(7));
                    //Debug.Log("-----" + se.railnumber + "-----");
                    goSceneryElement.transform.parent = GameObject.Find(se.parent).transform;
                    goSceneryElement.transform.localPosition = new Vector3(se.x+(se.zPos*0.01f), se.y, se.z);
                    //goSceneryElement.GetComponent<SceneryController>().sceneryActive = se.active;
                    goSceneryElement.SetActive(se.active);
                    if (se.mirrored)
                    {
                        if (goSceneryElement.transform.localScale.x > 0)
                        {
                            goSceneryElement.transform.localScale = new Vector3(-goSceneryElement.transform.localScale.x, goSceneryElement.transform.localScale.y, goSceneryElement.transform.localScale.z);
                        }
                    }
                    else
                    {
                        if (goSceneryElement.transform.localScale.x < 0)
                        {
                            goSceneryElement.transform.localScale = new Vector3(-goSceneryElement.transform.localScale.x, goSceneryElement.transform.localScale.y, goSceneryElement.transform.localScale.z);
                        }
                    }
                    //Debug.Log("Objekt: "+goSceneryElement+", Schiene: "+goSceneryElement.transform.parent+", Railnumber: "+se.railnumber+", active: "+goSceneryElement.active+", se.active: "+se.active);
                    if (se.active)
                    {
                        countActiveSceneryElements++;
                    }

                }
            }
        }
    }

    public void LightsApplyToScene(List<LightElement> lightElements)
    {
        countActiveLightElements = 0;
        foreach (LightElement le in lightElements)
        {
            foreach (objectsLightElement objectLightElement in objectsLightElements)
            {
                if (le.name == objectLightElement.goLightElement.name)
                {
                    GameObject gameObjectLe = objectLightElement.goLightElement;
                    gameObjectLe.transform.localPosition = new Vector3(le.x, le.y, le.z);
                    gameObjectLe.GetComponent<Light>().enabled = le.active;
                    if (le.active)
                    {
                        countActiveLightElements++;
                    }
                    gameObjectLe.GetComponent<Light>().intensity = le.intensity;
                    switch (objectLightElement.lightStagePosition)
                    {
                        case 0:
                            break;
                        case 1:
                            Debug.Log("lights3D " + gameObjectLe.name);
                            //gameObjectLe.GetComponent<LightController>().Sayhi();
                            gameObjectLe.GetComponent<LightController>().ChangeHorizontal(le.angle_h);
                            gameObjectLe.GetComponent<LightController>().ChangeVertical(le.angle_v);
                            gameObjectLe.GetComponent<LightController>().ChangePosition(le.z);
                            gameObjectLe.GetComponent<LightController>().ChangeHeight(le.y);
                            break;
                        case 2:
                            gameObjectLe.GetComponent<LightController>().ChangeHorizontal(le.angle_h);
                            gameObjectLe.GetComponent<LightController>().ChangeVerticalLeft(le.angle_v);
                            gameObjectLe.GetComponent<LightController>().ChangePosition(le.z);
                            gameObjectLe.GetComponent<LightController>().ChangeHeight(le.y);
                            break;
                        case 3:
                            break;
                    }
                }
            }
        }
    }

    public void FiguresApplyToScene(List<FigureElement> figureElements)
    {
        foreach (FigureElement fe in figureElements)
        {
            foreach (GameObject goFigureElement in objectsFigureElements)
            {
                if (fe.name == goFigureElement.name)
                {
                    //            objectFigureElement.transform.position = new Vector3(figureElement.x, figureElement.y, figureElement.z);
                    //            objectFigureElement.GetComponent<SceneryController>().sceneryActive = figureElement.active;
                    //            objectFigureElement.SetActive(figureElement.active);
                    //            objectFigureElement.transform.parent = GameObject.Find(figureElement.parent).transform;
                }
            }
        }
    }

    public string CreateJsonFromSceneData(SceneData sceneData)
    {
        String JsonData = JsonUtility.ToJson(sceneData, true);
        Debug.LogFormat("Data serialized to:\n{0}", JsonData);
        return JsonData;
    }

    public SceneData CreateSceneDataFromJSON(string JsonData)
    {
        SceneData sceneData = JsonUtility.FromJson<SceneData>(JsonData);
        return sceneData;
    }


    public void TestThisStuff()
    {
        //recentSceneData = CreateSceneData();
        CreateScene(recentSceneData);
    }
    // Update is called once per frame
    //void Update()
    //{

    //}
}
