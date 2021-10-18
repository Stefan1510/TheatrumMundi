using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkboxCheck : MonoBehaviour
{
	Toggle thisToggle;
	public GameObject object3D;
	
    // Start is called before the first frame update
    void Start()
    {
		thisToggle=this.GetComponent<Toggle>();
		//Debug.Log("ckbox is set to: "+thisToggle.isOn);
    }
	
	public void OnStatusChange()
	{
		//Debug.Log("status changed to: "+thisToggle.isOn);
		if (thisToggle.isOn)
		{
			object3D.SetActive(true);
		}
		else
		{
			object3D.SetActive(false);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
