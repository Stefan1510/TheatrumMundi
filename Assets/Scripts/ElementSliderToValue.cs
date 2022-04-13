using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSliderToValue : MonoBehaviour
{
    [SerializeField] GameObject xPos;
    public void ChangeElementPosition()
    {
        DragDrop dragdrop = SceneManaging.dragDrop;
        
        //2D-Kulisse
        //Debug.Log("value: "+ xPos.GetComponent<InputField>().text);
        xPos.GetComponent<InputField>().text = GetComponent<Slider>().value.ToString();
        dragdrop.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.GetComponent<Slider>().value*200, dragdrop.GetComponent<RectTransform>().anchoredPosition.y);

        //3D-Kulisse
        dragdrop.ThisSceneryElement.z = GetComponent<RectTransform>().GetComponent<Slider>().value / 1.5f;
        StaticSceneData.Sceneries3D();
    }
}
