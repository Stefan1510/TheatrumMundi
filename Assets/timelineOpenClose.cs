using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timelineOpenClose : MonoBehaviour
{
	Image timelineImage;
	bool moving;
	bool dragging;
	double minX;
	double maxX;
	double minY;
	double maxY;
	Vector2 sizeDeltaAsFactor;
	
    // Start is called before the first frame update
    void Start()
    {
        timelineImage=GetComponent<Image>();
		moving=false;
		dragging=false;
		minX=301.0f;
		maxX=1623.0f;
		minY=428.0f;
		maxY=438.0f;
		sizeDeltaAsFactor=new Vector2(1.0f,1.0f);
		Debug.Log("++++++timeline calling");
    }

	public void OnMouseClick()
	{
		Debug.Log("++++++timeline is clicked");
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 getMousePos=Input.mousePosition;
		
		if (Input.GetMouseButtonDown(0)) //left mouse button down
		{
			Debug.Log("++++++left mouse button down");
			//Debug.Log("++++++xpos mouse : "+getMousePos);
			if (GetComponent<Collider>()==Physics2D.OverlapPoint(getMousePos))	//if you touch the slider with the mouse
			{
				moving=true;
			}
			else
			{
				moving=false;
			}
			if (moving)
			{
				//space for dragging
				dragging=true;
			}
		}
		if (dragging)
		{
			if (((getMousePos.x<=((maxX*sizeDeltaAsFactor.x))) && (getMousePos.x>=(minX*sizeDeltaAsFactor.x))) &&
			((getMousePos.y<=((maxY*sizeDeltaAsFactor.y))) && (getMousePos.y>=(minY*sizeDeltaAsFactor.y))))
			{
				//this.transform.position=new Vector2(getMousePos.x,this.transform.position.y);
				//Debug.Log("xpos mouse : "+getMousePos);
				Debug.Log("++++++timeline hitted at pos: "+this.transform.position);
				Debug.Log("rail hitted: "+timelineImage.sprite.rect.width);
				//currPos=timeSlider.transform.position.x;
				//int maxSec=convertMinutesToSeconds(length);
				//int timeByPos=getTimeByXpos(minX,maxX,(currPos/sizeDeltaAsFactor.x),maxSec);
				//string minStr=convertSecondsToMinutes(timeByPos);
				//Debug.Log("seconds: " +maxSec);
				//Debug.Log("mouspos.x: " +currPos);
				//Debug.Log("seconds by pos: " +timeByPos);
				//Debug.Log("minutes: " +minStr);
				//timeSliderText.text=minStr;
			}
		}
		if (Input.GetMouseButtonUp(0)) //left mouse button up
		{
			//Debug.Log("++++++left mouse button up");
			moving=false;
			dragging=false;
		}
    }
}
