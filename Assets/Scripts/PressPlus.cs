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
        else if(dragdrop.transform.GetSiblingIndex() < dragdrop.transform.parent.transform.childCount-1)
        {
            //dragdrop.ThisSceneryElement.x += .01f;
            dragdrop.transform.SetSiblingIndex(dragdrop.transform.GetSiblingIndex()+1);
            dragdrop.ThisSceneryElement.zPos = dragdrop.transform.GetSiblingIndex();
            Debug.Log("Childcount: "+dragdrop.transform.parent.transform.childCount + ", IndexSibling: "+dragdrop.transform.GetSiblingIndex());
            //Debug.Log("Index Sibling: "+dragdrop.ThisSceneryElement.zPos);
            dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
        }
    }
}