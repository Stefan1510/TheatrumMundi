using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkboxCheck : MonoBehaviour
{
	Toggle thisToggle;
	public GameObject object3D;
	
    void Start()
    {
		thisToggle=this.GetComponent<Toggle>();
    }
	
	public void OnStatusChange()
	{
		if (thisToggle.isOn)
		{
			object3D.SetActive(true);
		}
		else
		{
			object3D.SetActive(false);
		}
	}
}
