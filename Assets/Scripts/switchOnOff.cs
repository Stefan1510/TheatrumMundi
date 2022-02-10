using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchOnOff : MonoBehaviour
{
	public Sprite active;
	public Sprite inactive;
	public GameObject speedScale;
	public GameObject speedCtrl;
	
    private void Awake()
    {
        //SceneManager.showSettings = false;
        //gameObject.SetActive(false);
		gameObject.GetComponent<Image>().sprite=inactive;
		speedScale.SetActive(false);
		speedCtrl.SetActive(false);
    }

    public void OnClick()
    {		
		//Debug.Log("activate time ...");
		if (gameObject.GetComponent<Image>().sprite==inactive)
		{
			gameObject.GetComponent<Image>().sprite=active;
			speedScale.SetActive(true);
			speedCtrl.SetActive(true);
		}
		else
		{
			gameObject.GetComponent<Image>().sprite=inactive;
			speedScale.SetActive(false);
			speedCtrl.SetActive(false);
		}
    }
}
