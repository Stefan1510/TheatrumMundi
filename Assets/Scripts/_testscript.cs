using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dieses testscript muss dann auf die Buttons des object shelves überführt werden
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
        Debug.LogWarning("please place Object Figure Element in SceneDataController (Script) on GameController");
        figureObjects = gameObjectGameController.GetComponent<SceneDataController>().objectsFigureElements; // get all figure objects from public array at Script on GameController
        Debug.Log(figureObjects[0].name);
        thisFigureElement = StaticSceneData.StaticData.figureElements.Find(x => x.name == figureObjects[0].name); // connect first scenedata.figurelement with first figureobject from array above // could be put in a loop
                                                                                                                  // when changing something like position, you now can change position on figureelement in scenedata as well
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
