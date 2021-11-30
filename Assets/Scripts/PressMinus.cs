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
        else if(dragdrop.ThisSceneryElement.zPos >0)
        {
            dragdrop.ThisSceneryElement.zPos -= 1;
            dragdrop.ThisSceneryElement.x -= .01f;
            dragdrop.transform.SetSiblingIndex(dragdrop.ThisSceneryElement.zPos);
            Debug.Log("Index Sibling: "+dragdrop.ThisSceneryElement.zPos);
            dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
        }
    }
}
