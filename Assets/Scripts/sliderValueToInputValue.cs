using UnityEngine;
using UnityEngine.UI;
using System;

public class sliderValueToInputValue : MonoBehaviour
{
	Slider slider;  //the slider this script is attached to
	public InputField attachedInputValue;
	public GameObject attachedRailSide;
	public Image attachedRailTop;
	public GameObject rail3D;
	//-----for sideview
	Vector3 v = new Vector3(0.0f, 0.0f, 0.0f);
	float currSliderVal = 0.0f;
	//-----for topview
	Vector3 v2 = new Vector3(0.0f, 0.0f, 0.0f);
	float currSliderVal2 = 0.0f;
	//-----for 3Dview
	Vector3 v3 = new Vector3(0.0f, 0.0f, 0.0f);
	float currSliderVal3 = 0.0f;

	float yLight;

	float shiftScaleFacVert = 100.0f;
	float shiftScaleFacHor = 40.0f;
	float shiftScaleFac3d = 0.5f;
	RailElement thisRailElement;

	[SerializeField] private GameObject imgLight;

	void Start()
	{
		slider = GetComponent<Slider>();
		currSliderVal = slider.value;
		currSliderVal2 = slider.value;
		currSliderVal3 = slider.value;
		try
		{
			yLight = imgLight.transform.localPosition.y;
		}
		catch (UnassignedReferenceException)
		{ }
		try
		{
			v.x = attachedRailSide.transform.localPosition.x;
			v.y = attachedRailSide.transform.localPosition.y;
			v.z = attachedRailSide.transform.localPosition.z;
			v2.x = attachedRailTop.transform.localPosition.x;
			v2.y = attachedRailTop.transform.localPosition.y;
			v2.z = attachedRailTop.transform.localPosition.z;
			v3.x = rail3D.transform.localPosition.x;
			v3.y = rail3D.transform.localPosition.y;
			v3.z = rail3D.transform.localPosition.z;
		}
		catch (NullReferenceException)
		{
			Debug.Log("there are scene objects with some inputs which are not completly set");
		}
		//Debug.Log("===Rail: " +v.x.ToString());
		//myInputfield=GameObject.Find("InputField").GetComponent<InputField>();
		try
		{
			//Debug.Log("+++START: " +attachedInputValue.text);
			attachedInputValue.text = slider.value.ToString("0.00");
			//Debug.Log("+++++++START: " +attachedInputValue.text);

		}
		catch (NullReferenceException)
		{
			Debug.Log("attachedInputValue was not set in the inspector");
		}
	}
	public void OnSliderChange()
	{
		attachedInputValue.text = slider.value.ToString("0.00");
		//------transform rails in schematic view		

		//seperate the height-slider from horizontal slider by name
		string[] returnedArray = slider.ToString().Split(' ');
		if ((returnedArray[0] == "SliderHeight12to34") || (returnedArray[0] == "SliderHeight34to56") || (returnedArray[0] == "SliderHeight56to78") || (returnedArray[0] == "SliderStageHeight"))
		{
			if (currSliderVal < slider.value)
			{
				yLight += (slider.value - currSliderVal3) * shiftScaleFacVert;
				v.y = v.y + ((slider.value - currSliderVal) * shiftScaleFacVert);
				//3d
				v3.y = v3.y + ((slider.value - currSliderVal3) * shiftScaleFac3d);
			}
			else
			{
				yLight -= (currSliderVal - slider.value) * shiftScaleFacVert;
				v.y = v.y - ((currSliderVal - slider.value) * shiftScaleFacVert);
				//3d
				v3.y = v3.y - ((currSliderVal3 - slider.value) * shiftScaleFac3d);
			}
			//write back last x-value
			v.x = attachedRailSide.transform.localPosition.x;
			v3.x = rail3D.transform.localPosition.x;
		}
		else
		{
			if (currSliderVal < slider.value)
			{
				//sideview
				v.x = v.x + ((slider.value - currSliderVal) * shiftScaleFacHor);
				v.y = attachedRailSide.transform.localPosition.y;
				//topview
				v2.y = v2.y + ((slider.value - currSliderVal2) * shiftScaleFacHor);
				//3d, switch direction because of the coordinate-system in 3d instead of 2d
				v3.x = v3.x + ((slider.value - currSliderVal3) * shiftScaleFac3d * -1.0f);
				v3.y = rail3D.transform.localPosition.y;
			}
			else
			{
				//sideview
				v.x = v.x - ((currSliderVal - slider.value) * shiftScaleFacHor);
				v.y = attachedRailSide.transform.localPosition.y;
				//topview
				v2.y = v2.y - ((currSliderVal2 - slider.value) * shiftScaleFacHor);
				//3d, switch direction because of the coordinate-system in 3d instead of 2d
				v3.x = v3.x - ((currSliderVal3 - slider.value) * shiftScaleFac3d * -1.0f);
				v3.y = rail3D.transform.localPosition.y;
			}
		}
		currSliderVal = slider.value;
		currSliderVal2 = slider.value;
		currSliderVal3 = slider.value;

		//update the objects in scene/write back values
		try
		{
			attachedRailSide.transform.localPosition = v;
		}
		catch (NullReferenceException)
		{
			Debug.Log("attachedRailSide was not set in the inspector");
		}
		try
		{
			imgLight.transform.localPosition = new Vector2(imgLight.transform.localPosition.x, yLight);
		}
		catch (UnassignedReferenceException)
		{ }
		try
		{
			attachedRailTop.transform.localPosition = v2;
		}
		catch (NullReferenceException)
		{
			Debug.Log("attachedRailTop was not set in the inspector");
		}
		try
		{
			//rail3D.transform.localPosition = v3;
			thisRailElement = StaticSceneData.StaticData.railElements.Find(re => re.name == rail3D.name);
			thisRailElement.x = v3.x;
			thisRailElement.y = v3.y;
			thisRailElement.z = v3.z;
			StaticSceneData.Rails3D();
		}
		catch (NullReferenceException)
		{
			Debug.Log("rail3D was not set in the inspector");
		}
	}
}
