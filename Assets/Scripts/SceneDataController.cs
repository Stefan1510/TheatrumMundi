using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SceneDataController : MonoBehaviour
{
    public GameObject[] objectsRailElements;
    public GameObject[] objectsSceneryElements;
    public GameObject[] objectsLightElements;
    [HideInInspector] public string sceneFileAuthor;
    [HideInInspector] public string sceneFileDate;
    [HideInInspector] public string sceneFileComment;
    [HideInInspector] public string recentSceneDataSerialized;

    SceneData recentSceneData;

    // Start is called before the first frame update
    void Start()
    {
        sceneFileAuthor = "-";
        sceneFileDate = "-";
        sceneFileComment = "-";
        recentSceneData = new SceneData();
        recentSceneData.railElements = new List<RailElement>();
        recentSceneData.sceneryElements = new List<SceneryElement>();
        recentSceneData.lightElements = new List<LightElement>();
    }

    public SceneData CreateRecentSceneData()
    {
        recentSceneData.fileAuthor = sceneFileAuthor;
        recentSceneData.fileComment = sceneFileComment;
        recentSceneData.fileDate = DateTime.Now.ToString();
        recentSceneData.railElements.Clear();
        recentSceneData.sceneryElements.Clear();
        recentSceneData.lightElements.Clear();

        foreach (GameObject objectRailElement in objectsRailElements)
        {
            RailElement sceneRailElement = new RailElement
            {
                description = objectRailElement.name,
                x = objectRailElement.transform.localPosition.x,
                y = objectRailElement.transform.localPosition.y,
                z = objectRailElement.transform.localPosition.z
            };
            recentSceneData.railElements.Add(sceneRailElement);
        }

        foreach (GameObject objectSceneryElement in objectsSceneryElements)
        {
            SceneryElement sceneSceneryElement = new SceneryElement
            {
                description = objectSceneryElement.name,
                x = objectSceneryElement.transform.position.x,
                y = objectSceneryElement.transform.position.y,
                z = objectSceneryElement.transform.position.z,
                active = objectSceneryElement.GetComponent<SceneryController>().sceneryActive,
                parent = objectSceneryElement.transform.parent.name
            };
            recentSceneData.sceneryElements.Add(sceneSceneryElement);
        }

        foreach (GameObject objectLightElement in objectsLightElements)
        {
            LightElement sceneLightElement = new LightElement
            {
                description = objectLightElement.name,
                x = objectLightElement.transform.position.x,
                y = objectLightElement.transform.position.y,
                z = objectLightElement.transform.position.z,
                active = false
            };
            recentSceneData.lightElements.Add(sceneLightElement);
        }

        return recentSceneData;
    }

    public string createJSONfromSceneData(SceneData sceneData)
    {
        String JSONData = JsonUtility.ToJson(sceneData, true);
        Debug.LogFormat("Data serialized to:\n{0}", JSONData);
        return JSONData;
    }



    // Update is called once per frame
    //void Update()
    //{

    //}
}
