using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dieses testscript muss dann auf die Buttons des object shelves �berf�hrt werden
public class _testscript : MonoBehaviour
{
    public GameObject[] figureObjects;
    public GameObject gameObjectGameController;
    private FigureElement thisFigureElement;

    //private void Awake()
    //{
    //}

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gooooooood Mooooooooorniiiiiiin TestScriiiiiipt!");
        figureObjects = gameObjectGameController.GetComponent<SceneDataController>().objectsFigureElements;
        Debug.Log(figureObjects[0].name);
        thisFigureElement = StaticSceneData.StaticData.figureElements.Find(x => x.name == figureObjects[0].name);
        Debug.Log(thisFigureElement.width);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void saveSceneTemp()
    {
        gameObjectGameController.GetComponent<SceneDataController>().tempSceneData = gameObjectGameController.GetComponent<SceneDataController>().CreateSceneData();
    }

    public void loadSceneTemp()
    {
        gameObjectGameController.GetComponent<SceneDataController>().CreateScene(gameObjectGameController.GetComponent<SceneDataController>().tempSceneData);
    }

}
