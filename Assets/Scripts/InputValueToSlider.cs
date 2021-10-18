using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputValueToSlider : MonoBehaviour
{
    InputField myInputField;	//the InputField this script is attached to
	public Slider attachedSlider;
	
	// Start is called before the first frame update
    void Start()
    {
        myInputField=this.GetComponent<InputField>();
		//Debug.Log(">>>");
		attachedSlider.value=float.Parse(myInputField.text); //for float
		//Debug.Log("<<<");
    }
	
	public void OnValueChange()
	{
		myInputField=this.GetComponent<InputField>();
		attachedSlider.value=float.Parse(myInputField.text);
		if (float.Parse(myInputField.text)>attachedSlider.maxValue)
		{
			myInputField.text=attachedSlider.maxValue.ToString("0.00");
		}
		if (float.Parse(myInputField.text)<attachedSlider.minValue)
		{
			myInputField.text=attachedSlider.minValue.ToString("0.00");
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
