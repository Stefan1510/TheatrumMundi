using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorElement : MonoBehaviour
{
    public void OnMirror()
    {
        DragDrop dragdrop = SceneManaging.dragDrop;


        if(dragdrop.ThisSceneryElement.mirrored)
        {
            //dragdrop.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(axis.x, 250f);
            dragdrop.GetComponent<RectTransform>().localScale = new Vector3(1, 1.0f, 1.0f);
            dragdrop.ThisSceneryElement.mirrored = false;
        }
        else if(dragdrop.ThisSceneryElement.mirrored == false)
        {
            dragdrop.ThisSceneryElement.mirrored = true;
            dragdrop.GetComponent<RectTransform>().localScale = new Vector3(-1, 1.0f, 1.0f);
        }
        dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
        
        Debug.Log("mirrored: "+dragdrop.ThisSceneryElement.mirrored);
    }
    
}
