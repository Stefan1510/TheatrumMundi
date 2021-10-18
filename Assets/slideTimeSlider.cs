using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class slideTimeSlider : MonoBehaviour
{
	Collider2D timeSlider;
	public Text timeSliderText;
	
	bool moving;
	bool dragging;
	//rail-length
	private double minX;
	private double maxX;
	double minSec;
	double maxSec;
	string length;

    // Start is called before the first frame update
    void Start()
    {
        timeSlider=GetComponent<Collider2D>();
		timeSliderText.text="00:00";
		moving=false;
		dragging=false;
		//startpoint rail + width of rail
		minX=170.0; //this is 00:00min
		maxX=930.0; //this is 10:14min
		minSec=0.0;
		length="10:14";
		maxSec=convertMinutesToSeconds(length);
		Debug.Log("seconds: " +convertMinutesToSeconds(length));
		Debug.Log("seconds by pos: " +getTimeByXpos(minX,maxX,313.0,614));
		Debug.Log("minutes: " +convertSecondsToMinutes(614));
		//timeSliderText.text=convertSecondsToMinutes(getTimeByXpos(minX,maxX,this.transform.position.x,maxSec));
		Debug.Log("initialization of collider");
		//Debug.Log("maxSeconds: "+maxSec);
    }

	public int convertMinutesToSeconds(string strMinutes)
	{
		int sec=0;		
		string [] splitArray = strMinutes.Split(':');
		int minutes=int.Parse(splitArray[0]);
		int seconds=int.Parse(splitArray[1]);
		sec=minutes*60+seconds;
		return sec;
	}
	public string convertSecondsToMinutes(int seconds)
	{
		string minutes="";
		int mins=0;
		int secs=0;
		mins=seconds/60;
		secs=seconds%60;
		minutes=mins.ToString()+":"+secs.ToString();
		return minutes;
	}
	
	public double getTimeByXpos(double startX, double endX,double currX,int lengthSeconds)
	{
		double currTimeInSeconds=0;
		//shift to x=0 by -startX
		currTimeInSeconds=(((currX-startX)*lengthSeconds)/(endX-startX)); //in seconds
		return currTimeInSeconds;
	}
    // Update is called once per frame
    void Update()
    {
        //Vector2 getMousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 getMousePos=Input.mousePosition;
		//getMousePos.y=0.0f;
		//Debug.Log("mouse position: "+getMousePos);
		
		if (Input.GetMouseButtonDown(0)) //left mouse button down
		{
			Debug.Log("left mouse button down");
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
			if ((getMousePos.x<=maxX) && (getMousePos.x>=minX))
			this.transform.position=new Vector2(getMousePos.x,this.transform.position.y);
		Debug.Log("xpos mouse : "+getMousePos);
		Debug.Log("xpos slider: "+this.transform.position);
		}
		if (Input.GetMouseButtonUp(0)) //left mouse button up
		{
			Debug.Log("left mouse button up");
			moving=false;
			dragging=false;
		}
    }
}
