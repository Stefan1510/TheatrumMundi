using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSliderToValueY : MonoBehaviour
{
    [SerializeField] GameObject yPos;
    public void ChangeElementPosition()
    {
        DragDrop dragdrop = SceneManager.dragDrop;
        
        //2D-Kulisse
        Debug.Log("value: "+ yPos.GetComponent<InputField>().text);
        yPos.GetComponent<InputField>().text = GetComponent<Slider>().value.ToString();
        dragdrop.GetComponent<RectTransform>().anchoredPosition = new Vector2(dragdrop.GetComponent<RectTransform>().anchoredPosition.x, this.GetComponent<Slider>().value*100);
        
        //3D-Kulisse
        //dragdrop.ThisSceneryElement.y = GetComponent<RectTransform>().anchoredPosition.y / 300;
        //dragdrop.gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData);
    }
}
