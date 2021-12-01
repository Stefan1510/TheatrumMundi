using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementValueToSlider : MonoBehaviour
{
    [SerializeField] GameObject slider;

    public void OnEndEdit()
    {
        //Debug.Log("EndEdit: "+ float.Parse(GetComponent<InputField>().text));
        slider.GetComponent<Slider>().value = float.Parse(GetComponent<InputField>().text);
    }
}
