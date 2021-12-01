using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteKulisse : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.showSettings = false;
        gameObject.SetActive(false);
    }

    void OnClick()
    {
        DragDrop dragdrop = SceneManager.dragDrop;

        if(dragdrop.ThisSceneryElement.mirrored) 
        {
            dragdrop.ThisSceneryElement.mirrored=false;
            dragdrop.GetComponent<RectTransform>().localScale = new Vector3(1, 1.0f, 1.0f);
        }
        dragdrop.transform.GetChild(1).gameObject.SetActive(false);
        dragdrop.ThisSceneryElement.active = false;
        dragdrop.transform.SetParent(dragdrop.parentStart.transform);
        dragdrop.ThisSceneryElement.parent = "Schiene1";
        dragdrop.GetComponent<RectTransform>().anchoredPosition = dragdrop.pos;
        dragdrop.schieneKulisse = 0;
        dragdrop.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        dragdrop.menuExtra.SetActive(false);
        dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
    }
}
