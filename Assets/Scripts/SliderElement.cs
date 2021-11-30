using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderElement : MonoBehaviour
{
    //[SerializeField] GameObject xPos;
    public void ChangeElementPosition()
    {
        DragDrop dragdrop = SceneManager.dragDrop;
        
        //Debug.Log("number: "+ this.GetComponent<Slider>().value.ToString()+", xPos.text: "+xPos.GetComponent<InputField>().textComponent.text);
        //xPos.GetComponent<InputField>().textComponent.text = GetComponent<Slider>().value.ToString();
        dragdrop.GetComponent<RectTransform>().anchoredPosition += new Vector2(this.GetComponent<Slider>().value, 0);
    }
}
