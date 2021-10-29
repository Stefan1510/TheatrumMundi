using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dieses testscript muss dann auf die Buttons des object shelves überführt werden
public class _testscript : MonoBehaviour
{
    public GameObject gameObjectGameController;
    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

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
