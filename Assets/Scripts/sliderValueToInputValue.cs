using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class sliderValueToInputValue : MonoBehaviour
{
	Slider slider;	//the slider this script is attached to
	public InputField attachedInputValue;
	public Image attachedRailSide;
	public Image attachedRailTop;
	public GameObject rail3D;
	//-----for sideview
	Vector3 v=new Vector3(0.0f,0.0f,0.0f);
	float currSliderVal=0.0f;
	//-----for topview
	Vector3 v2=new Vector3(0.0f,0.0f,0.0f);
	float currSliderVal2=0.0f;
	//-----for 3Dview
	Vector3 v3=new Vector3(0.0f,0.0f,0.0f);
	float currSliderVal3=0.0f;
	
	float shiftScaleFac=50.0f;
	float shiftScaleFac3d=0.5f;
	
    // Start is called before the first frame update
    void Start()
    {		
		slider=this.GetComponent<Slider>();
		//Debug.Log("+++Name of Slider: " +slider.ToString());
		//Debug.Log("===START: " +slider.value);
		currSliderVal=slider.value;
		currSliderVal2=slider.value;
		currSliderVal3=slider.value;
		try
		{
			v.x=attachedRailSide.transform.localPosition.x;
			v.y=attachedRailSide.transform.localPosition.y;
			v.z=attachedRailSide.transform.localPosition.z;
			v2.x=attachedRailTop.transform.localPosition.x;
			v2.y=attachedRailTop.transform.localPosition.y;
			v2.z=attachedRailTop.transform.localPosition.z;
			v3.x=rail3D.transform.localPosition.x;
			v3.y=rail3D.transform.localPosition.y;
			v3.z=rail3D.transform.localPosition.z;
		}
		catch (NullReferenceException ex)
		{
			Debug.Log("there are scene objects with some inputs which are not completly set");
		}
		//Debug.Log("===Rail: " +v.x.ToString());
		//myInputfield=GameObject.Find("InputField").GetComponent<InputField>();
		try
		{
			//Debug.Log("+++START: " +attachedInputValue.text);
			attachedInputValue.text=slider.value.ToString("0.00");
			//Debug.Log("+++++++START: " +attachedInputValue.text);
			
		}
		catch (NullReferenceException ex)
		{
			Debug.Log("attachedInputValue was not set in the inspector");
		}
    }
	
	public void OnSliderChange()
	{
		attachedInputValue.text=slider.value.ToString("0.00");
		//------transform rails in schematic view		
		//Debug.Log("+++Name of Slider: " +slider.ToString());
		//Debug.Log(">>>curr RailPos.x: "+attachedRailSide.transform.localPosition.x.ToString());
		//Debug.Log(">>>curr SlideVal: "+currSliderVal.ToString());
		//Debug.Log("+++Name of Slider: " +slider.ToString());
		//Debug.Log("---before sidePosObj: "+attachedRailSide.transform.localPosition);
			Debug.Log("---before sidePosVec: "+v);
		//seperate the height-slider from horizontal slider by name
		string [] returnedArray = slider.ToString().Split(' ');
		if ((returnedArray[0]=="SliderHeight12to34") || (returnedArray[0]=="SliderHeight34to56") || (returnedArray[0]=="SliderHeight56to78") || (returnedArray[0]=="SliderStageHeight"))
		{
			//Debug.Log("----->its an height slider<-------");
			//Debug.Log("---if height sidePosObj: "+attachedRailSide.transform.localPosition);
			//Debug.Log("---if height sidePosVec: "+v);
			if ((currSliderVal)<(slider.value))
			{
				v.y=v.y+((slider.value-currSliderVal)*shiftScaleFac);
				//3d
				v3.y=v3.y+((slider.value-currSliderVal3)*shiftScaleFac3d);
			}
			else
			{
				v.y=v.y-((currSliderVal-slider.value)*shiftScaleFac);
				//3d
				v3.y=v3.y-((currSliderVal3-slider.value)*shiftScaleFac3d);
			}
			//Debug.Log("---if height sidePosObj: "+attachedRailSide.transform.localPosition);
			//Debug.Log("---if height sidePosVec: "+v);
			//write back last x-value
			v.x=attachedRailSide.transform.localPosition.x;
			v3.x=rail3D.transform.localPosition.x;
		}
		else
		{
			//Debug.Log("---else sidePosObj: "+attachedRailSide.transform.localPosition);
			//Debug.Log("---else sidePosVec: "+v);
			//Debug.Log("----->its an horizontal slider<-------");
			if ((currSliderVal)<(slider.value))
			{
				//sideview
				v.x=v.x+((slider.value-currSliderVal)*shiftScaleFac);
				v.y=attachedRailSide.transform.localPosition.y;
				//topview
				v2.y=v2.y+((slider.value-currSliderVal2)*shiftScaleFac);
				//3d, switch direction because of the coordinate-system in 3d instead of 2d
				v3.x=v3.x+((slider.value-currSliderVal3)*shiftScaleFac3d*-1.0f);
				v3.y=rail3D.transform.localPosition.y;
			}
			else
			{
				//sideview
				v.x=v.x-((currSliderVal-slider.value)*shiftScaleFac);
				v.y=attachedRailSide.transform.localPosition.y;
				//topview
				v2.y=v2.y-((currSliderVal2-slider.value)*shiftScaleFac);
				//3d, switch direction because of the coordinate-system in 3d instead of 2d
				v3.x=v3.x-((currSliderVal3-slider.value)*shiftScaleFac3d*-1.0f);
				v3.y=rail3D.transform.localPosition.y;
			}
			//Debug.Log("---else sidePosObj: "+attachedRailSide.transform.localPosition);
			//Debug.Log("---else sidePosVec: "+v);
			//write back the value and save the current value for x
			//attachedRailSide.transform.localPosition=new Vector3(v.x,attachedRailSide.transform.localPosition.y,v.z);
		}
		//attachedRail.transform.position.x=new Vector3(v.x,v.z,v.z);
		currSliderVal=slider.value;
		currSliderVal2=slider.value;
		currSliderVal3=slider.value;
		//Debug.Log(">>>new SlideVal: "+currSliderVal.ToString());
		//Debug.Log(">>>new RailPos.x: "+v.x.ToString());
		//Debug.Log("+++Name of Slider: " +slider.ToString());
		//Debug.Log("---update sidePosObj: "+attachedRailSide.transform.localPosition);
		//Debug.Log("---update sidePosVec: "+v);
		//Debug.Log("---update 3dPosObj: "+rail3D.transform.localPosition);
		//Debug.Log("---update 3dPosVec: "+v3);
		//update the objects in scene/write back values
		try
		{
			attachedRailSide.transform.localPosition=v;
		}
		catch (NullReferenceException ex)
		{
			Debug.Log("attachedRailSide was not set in the inspector");
		}
		try
		{
			attachedRailTop.transform.localPosition=v2;
		}
		catch (NullReferenceException ex)
		{
			Debug.Log("attachedRailTop was not set in the inspector");
		}
		try
		{
			rail3D.transform.localPosition=v3;
		}
		catch (NullReferenceException ex)
		{
			Debug.Log("rail3D was not set in the inspector");
		}
		
	}

    // Update is called once per frame
    void Update()
    {
		//attachedInputValue.text=slider.value.ToString;
		//Debug.Log(">>>updateRail.x: " +v.x.ToString());
		//attachedRailSide.transform.localPosition=v;
		try
		{
			//attachedRailTop.transform.localPosition=v2;
		}
		catch (NullReferenceException ex)
		{
			
		}
    }
}
