using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressPlus : MonoBehaviour
{
    
    public void OnClick()
    {
        DragDrop dragdrop = SceneManager.dragDrop;
        if (dragdrop == null)
        {
            return;
        }
        else if(dragdrop.ThisSceneryElement.zPos <2)
        {
        dragdrop.ThisSceneryElement.zPos += 1;
        dragdrop.ThisSceneryElement.x += .01f;
        dragdrop.transform.SetSiblingIndex(dragdrop.ThisSceneryElement.zPos);
        //Debug.Log("dragdrop: "+dragdrop.name+", kulisse pos x: "+dragdrop.ThisSceneryElement.x+", Index Sibling: "+dragdrop.ThisSceneryElement.zPos);
        //Debug.Log("sibling index : " + dragdrop.transform.GetSiblingIndex());
        dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
        }
        
    }
}