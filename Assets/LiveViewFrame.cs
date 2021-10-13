using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveViewFrame : MonoBehaviour
{
	//public Camera LiveViewCam;
	public GameObject cam1;
	public GameObject cam2;
	
    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("this is camera calling");
		//LiveViewCam.enabled=true;
		cam1.SetActive(true);
		cam2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //LiveViewCam.enabled=true;
		if (Input.GetButtonDown("Switch1"))
		{
		cam1.SetActive(true);
		cam2.SetActive(false);
		}
		if (Input.GetButtonDown("Switch2"))
		{
		cam1.SetActive(false);
		cam2.SetActive(true);
		}
    }
}
