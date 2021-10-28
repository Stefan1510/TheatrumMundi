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
    public GameObject[] objectsLightElements;
    [HideInInspector] public string sceneFileName;
    [HideInInspector] public string sceneFileAuthor;
    [HideInInspector] public string sceneFileDate;
    [HideInInspector] public string sceneFileComment;
    [HideInInspector] public string recentSceneDataSerialized;
    [HideInInspector] public int countActiveSceneryElements = 0;
    [HideInInspector] public int countActiveLightElements = 0;

    SceneData recentSceneData;

    // Start is called before the first frame update
    void Start()
    {
        sceneFileName = "-";
        sceneFileAuthor = "-";
        sceneFileDate = "-";
        sceneFileComment = "-";
        recentSceneData = new SceneData();
        recentSceneData.railElements = new List<RailElement>();
        recentSceneData.sceneryElements = new List<SceneryElement>();
        recentSceneData.lightElements = new List<LightElement>();
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
        //sceneData.railElements.Clear();
        //sceneData.sceneryElements.Clear();
        //sceneData.lightElements.Clear();
        sceneData.railElements = new List<RailElement>();
        sceneData.sceneryElements = new List<SceneryElement>();
        sceneData.lightElements = new List<LightElement>();

        foreach (GameObject objectRailElement in objectsRailElements)
        {
            RailElement sceneRailElement = new RailElement
            {
                name = objectRailElement.name,
                x = objectRailElement.transform.localPosition.x,
                y = objectRailElement.transform.localPosition.y,
                z = objectRailElement.transform.localPosition.z
            };
            sceneData.railElements.Add(sceneRailElement);
        }

        foreach (GameObject objectSceneryElement in objectsSceneryElements)
        {
            SceneryElement sceneSceneryElement = new SceneryElement
            {
                name = objectSceneryElement.name,
                x = objectSceneryElement.transform.position.x,
                y = objectSceneryElement.transform.position.y,
                z = objectSceneryElement.transform.position.z,
                active = objectSceneryElement.GetComponent<SceneryController>().sceneryActive,
                parent = objectSceneryElement.transform.parent.name
            };
            sceneData.sceneryElements.Add(sceneSceneryElement);
        }

        foreach (GameObject objectLightElement in objectsLightElements)
        {
            LightElement sceneLightElement = new LightElement
            {
                name = objectLightElement.name,
                x = objectLightElement.transform.position.x,
                y = objectLightElement.transform.position.y,
                z = objectLightElement.transform.position.z,
                active = false
            };
            sceneData.lightElements.Add(sceneLightElement);
        }

        recentSceneData = sceneData;
        return sceneData;
    }

    public void CreateScene(SceneData sceneData)
    {
        sceneFileName = sceneData.fileName;
        sceneFileAuthor = sceneData.fileAuthor;
        sceneFileDate = sceneData.fileDate;
        sceneFileComment = sceneData.fileComment;
        SetFileMetaDataToScene();
        countActiveSceneryElements = 0;
        countActiveLightElements = 0;
        Debug.Log(sceneData.railElements[0]);
        foreach (RailElement railElement in sceneData.railElements)
        {
            foreach (GameObject objectRailElement in objectsRailElements)
            {
                if (railElement.name == objectRailElement.name)
                {
                    objectRailElement.transform.localPosition = new Vector3(railElement.x, railElement.y, railElement.z);
                }
            }
        }
        foreach (SceneryElement sceneryElement in sceneData.sceneryElements)
        {
            foreach (GameObject objectSceneryElement in objectsSceneryElements)
            {
                if (sceneryElement.name == objectSceneryElement.name)
                {
                    objectSceneryElement.transform.position = new Vector3(sceneryElement.x, sceneryElement.y, sceneryElement.z);
                    objectSceneryElement.GetComponent<SceneryController>().sceneryActive = sceneryElement.active;
                    objectSceneryElement.SetActive(sceneryElement.active);
                    objectSceneryElement.transform.parent = GameObject.Find(sceneryElement.parent).transform;
                    if (sceneryElement.active)
                    {
                        countActiveSceneryElements++;
                    }
                }
            }
        }
        foreach (LightElement lightElement in sceneData.lightElements)
        {
            foreach (GameObject objectLightElements in objectsLightElements)
            {
                if (lightElement.name == objectLightElements.name)
                {
                    objectLightElements.transform.position = new Vector3(lightElement.x, lightElement.y, lightElement.z);
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

    public SceneData CreateSceneDataFromJSON (string JsonData)
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
