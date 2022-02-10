using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timesliderSettingsCtrl : MonoBehaviour
{
	GameObject speedScale;
	bool movingCtrl;

    void Start()
    {		
		Debug.Log("speed ctrl ...");
		speedScale=this.transform.parent.gameObject;
		movingCtrl=false;
    }
	void Update()
	{
		Vector2 getMousePos=Input.mousePosition;
		float maxY=speedScale.transform.position.y+(speedScale.GetComponent<RectTransform>().sizeDelta.y/3.9f);
		float minY=speedScale.transform.position.y-(speedScale.GetComponent<RectTransform>().sizeDelta.y/8.2f);
		
		if (Input.GetMouseButtonDown(0)) //left mouse button clicked down
		{
			Debug.Log("speed-ctrl-collider: "+this.GetComponent<BoxCollider2D>());
			if(this.GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			{
				movingCtrl=true;
				/*if ((this.transform.position.y >=minY) && (this.transform.position.y <=maxY))
				{
					this.transform.position=new Vector2(this.transform.position.x,getMousePos.y);
				}*/
			}
		}
		
		if (movingCtrl)
		{
			//Debug.Log("minY: "+minY+" maxY: "+maxY);
			if ((this.transform.position.y>=minY) && (this.transform.position.y<=maxY))
			{
				this.transform.position=new Vector2(this.transform.position.x,getMousePos.y);
			}
		}
		
		if (Input.GetMouseButtonUp(0)) //left mouse button up
		{
			movingCtrl=false;
		}
	}
}
