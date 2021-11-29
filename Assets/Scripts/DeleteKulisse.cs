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

        dragdrop.transform.GetChild(0).gameObject.SetActive(false);
        dragdrop.ThisSceneryElement.active = false;
        dragdrop.transform.SetParent(dragdrop.parentStart.transform);
        dragdrop.ThisSceneryElement.parent = "Schiene1";
        dragdrop.GetComponent<RectTransform>().anchoredPosition = dragdrop.pos;
        dragdrop.schieneKulisse = 0;
        dragdrop.activeReiter.RemoveKulisse(dragdrop);
        dragdrop.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        dragdrop.menuExtra.SetActive(false);
        dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
    }
}
