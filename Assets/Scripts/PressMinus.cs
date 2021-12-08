using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressMinus : MonoBehaviour
{
    public void OnClick()
    {
        DragDrop dragdrop = SceneManager.dragDrop;
        
        if (dragdrop == null)
        {
            return;
        }
        else if(dragdrop.transform.GetSiblingIndex() >0)
        {
            //dragdrop.ThisSceneryElement.x -= .01f;
            dragdrop.transform.SetSiblingIndex(dragdrop.transform.GetSiblingIndex()-1);
            dragdrop.ThisSceneryElement.zPos = dragdrop.transform.GetSiblingIndex();
            Debug.Log("Childcount: "+dragdrop.transform.parent.transform.childCount + ", IndexSibling: "+dragdrop.transform.GetSiblingIndex()+"zPos: "+dragdrop.ThisSceneryElement.zPos);
            //Debug.Log("Index Sibling: "+dragdrop.ThisSceneryElement.zPos);
            dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
        }
    }
}
