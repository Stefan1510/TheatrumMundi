using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class slideTimeSlider : MonoBehaviour
{
	//BoxCollider2D timeSlider;
	Image timeSliderImage;
	private BoxCollider2D timeSlider;
	public Text timeSliderText;
	public RectTransform timelineArea;
	public Image timeline;
	
	bool moving;
	bool dragging;
	//rail-length
	private double minX;
	private double maxX;
	double minSec;
	double maxSec;
	double currPos;
	string length;
	Vector2 stageSize; //native app-resolution
	Vector2 screenSize; //current screen size, thats what the user set up
	Vector2 sizeDeltaAsFactor;	//difference between stageSize and screenSize
	Vector2 anchorPoint;

    // Start is called before the first frame update
    void Start()
    {
		//Output the current screen window width in the console
        //Debug.Log("Screen Width : " + Screen.width);
		timeSliderImage=GetComponent<Image>();
		timeSlider=timeSliderImage.GetComponent<BoxCollider2D>();
        //timeSlider=GetComponent<BoxCollider2D>();
		stageSize=new Vector2(1920.0f,1080.0f);	//native resolution of app
		screenSize=new Vector2(Screen.width, Screen.height);
		sizeDeltaAsFactor=new Vector2(screenSize.x/stageSize.x,screenSize.y/stageSize.y);
		Debug.Log("Native Size : " + stageSize);
		Debug.Log("Screen Size : " + screenSize);
		Debug.Log("Delta : " + sizeDeltaAsFactor);
		
		timeSliderText.text="00:00";
		moving=false;
		dragging=false;
		//sceen-size 1920x1080
		anchorPoint=new Vector2(stageSize.x/2.0f,stageSize.y/2.0f);	//anchorPoint of app-resolution -> this is (0;0) in unity-editor
		//double xPos=getScreenXPosOfObject(stageSize, timelineArea.transform.localPosition, new Vector2(timelineArea.rect.width,timelineArea.rect.height)).x;
		//Debug.Log("--->timeline-absolutePos: "+xPos);
		//Debug.Log("--->timeline-percentagePos: "+getScreenXPosOfObjectAsPercentage(stageSize.x,xPos)+" %");
		//xPos=getScreenXPosOfObject(stageSize, timeline.transform.localPosition, new Vector2(timeline.rectTransform.sizeDelta.x,timeline.rectTransform.sizeDelta.y)).x;
		//Debug.Log("--->timelineRail-absolutePos: "+xPos);
		//Debug.Log("--->timelineRail-percentagePos: "+getScreenXPosOfObjectAsPercentage(stageSize.x,xPos)+" %");
		
		/*Debug.Log("area (local): "+timelineArea.transform.localPosition);
		Debug.Log("area: "+timelineArea.transform.position);
		Debug.Log("area-width: "+timelineArea.rect.width+" -height: "+timelineArea.rect.height);
		Debug.Log("timeline (local): "+timeline.transform.localPosition);
		Debug.Log("timeline: "+timeline.transform.position);
		Debug.Log("timeline-real-width: "+timeline.sprite.rect.width);
		Debug.Log("timeline-real-height: "+timeline.sprite.rect.height);
		Debug.Log("timeline-scene-width: "+timeline.rectTransform.sizeDelta.x);
		Debug.Log("timeline-scene-height: "+timeline.rectTransform.sizeDelta.y);
		Debug.Log("sliderCollider-scene-width: "+timeSlider.size.x);
		Debug.Log("sliderCollider-scene-width: "+timeSliderImage.sprite.rect.width);*/
		//startpoint rail + width of rail
		//double xPosPerc=getScreenXPosOfObjectAsPercentage(stageSize.x,xPos);
		//minX=convertScreenPercentageToPixel(stageSize.x,xPosPerc);
		//minX=convertScreenPercentageToPixel(stageSize.x,xPos);
		//Debug.Log("^^^^xPos: "+xPos+" XPosPerc: "+xPosPerc+" newXPos: "+minX);
		//timeline.transform.localPosition.x; //this is 00:00min
		
		//minX=timeline.transform.localPosition.x-115.0; //this is 00:00min
		//minX=141.0f+129.0f+(38.0f/2.0f); //=timelineArea.x+timeline.x+slider-width/2
		minX=Mathf.Abs(timelineArea.transform.localPosition.x)+timeline.transform.localPosition.x+(timeSliderImage.sprite.rect.width/2.0f);
		//scale the position belong to the screen size (if its not 1920x1080)
		//minX=minX*sizeDeltaAsFactor.x;
		//maxX=minX+timeline.sprite.rect.width; //this is 10:14min, 
		maxX=minX+timeline.rectTransform.sizeDelta.x; //->use sizeDelta, not real png-size, because sizeDelta represent the screen-size of element
		//maxX=maxX*sizeDeltaAsFactor.x;

		minSec=0.0;
		length="10:14";
		//currPos=Input.mousePosition.x;
		currPos=timeSlider.transform.position.x;
		//Debug.Log("timesliderPos: "+timeSlider.transform.position);
		int maxSec=convertMinutesToSeconds(length);
		int timeByPos=getTimeByXpos(minX,maxX,(currPos/sizeDeltaAsFactor.x),maxSec);
		string minStr=convertSecondsToMinutes(timeByPos);
		/*Debug.Log("seconds: " +maxSec);
		Debug.Log("mouspos.x: " +currPos);
		Debug.Log("seconds by pos: " +timeByPos);
		Debug.Log("minutes: " +minStr);*/
		timeSliderText.text=minStr;
		//timeSliderText.text=convertSecondsToMinutes(getTimeByXpos(minX,maxX,this.transform.position.x,maxSec));
		//Debug.Log("initialization of collider");
		//Debug.Log("maxSeconds: "+maxSec);
    }
	
	//not used
	public Vector2 getScreenXPosOfObject(Vector2 actStageSize,Vector2 objectPos, Vector2 objectSize)
	{
		//Debug.Log("----getScreenXPosOfObject----");
		Vector2 minPos=new Vector2(0.0f,0.0f);
		//stagesize 1920x1080
		//half stagesize 960x540
		double halfStageXSize=actStageSize.x/2.0f;
		//Debug.Log("----halfStageXSize: "+halfStageXSize);
		//anchorPoint of stage is (0;0) at size x: 1920/2
		//object x: e.g. -140px -> make absolute
		double absObjXPos=Mathf.Abs(objectPos.x);
		//Debug.Log("----absObjXPos: "+absObjXPos);
		//object size x 1638
		//Debug.Log("----ObjectSize: "+objectSize.x);
		//object halfsize x 819
		double halfObjXSize=objectSize.x/2.0f;
		//Debug.Log("----halfObjXSize: "+halfObjXSize);
		//object |x| + halfsize x = |-140|+819=959
		double absoluteHalfObjectXSize=absObjXPos+halfObjXSize;
		//Debug.Log("----absoluteHalfObjectXSize: "+absoluteHalfObjectXSize);
		//object - stage = 960-959=1 -> this is the screenXPos of Object
		minPos.x=(float)(halfStageXSize-absoluteHalfObjectXSize);
		if(minPos.x<0)
		{
			//Debug.Log("pos x is lower than 0.0 !!");
		}
		//Debug.Log("----minPos: "+minPos.x);
		return minPos;
	}
	//not used
	public double getScreenXPosOfObjectAsPercentage(double StageXSize,double ObjectXPos )
	{
		double percentage=0.0;
		//1920=StageXSize
		//1px=ObjectXPos
		percentage=ObjectXPos/StageXSize*100.0;
		return percentage;
	}
	//not used
	public double convertScreenPercentageToPixel(double StageXSize,double ObjectXPercentage)
	{
		double xpos=0.0f;
		xpos=StageXSize*ObjectXPercentage/100.0f;
		return xpos;
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
		if (mins<10)
		{
			if (secs<10)
			{
				minutes="0"+mins.ToString()+":"+"0"+secs.ToString();
			}
			else
			{
				minutes="0"+mins.ToString()+":"+secs.ToString();
			}
		}
		else
		{
			if (secs<10)
			{
				minutes=mins.ToString()+":"+"0"+secs.ToString();
			}
			else
			{
				minutes=mins.ToString()+":"+secs.ToString();
			}
		}
		return minutes;
	}
	
	public int getTimeByXpos(double startX, double endX,double currX,int lengthSeconds)
	{
		int currTimeInSeconds=0;
		//shift to x=0 by -startX
		double tmp=(((currX-startX)*(double)lengthSeconds)/(endX-startX)); //in seconds
		//Debug.Log("full timeByPos: "+tmp);
		currTimeInSeconds=(int)(Mathf.Round((float)tmp));
		//Debug.Log("full timeByPos-round: "+currTimeInSeconds);
		return currTimeInSeconds;
	}
    // Update is called once per frame
    void Update()
    {
		//Output the current screen window width in the console
		screenSize.x=Screen.width;
		screenSize.y=Screen.height;
        //Debug.Log("Screen Size : " + screenSize);
		sizeDeltaAsFactor.x=(screenSize.x/stageSize.x);
		sizeDeltaAsFactor.y=(screenSize.y/stageSize.y);
		//Debug.Log("Delta Size : " + sizeDeltaAsFactor);
		
        //Vector2 getMousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 getMousePos=Input.mousePosition;
		//getMousePos.y=0.0f;
		//Debug.Log("mouse position: "+getMousePos);
		
		if (Input.GetMouseButtonDown(0)) //left mouse button down
		{
			//Debug.Log("left mouse button down");
			//Debug.Log("xpos mouse : "+getMousePos);
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
			if ((getMousePos.x<=((maxX*sizeDeltaAsFactor.x))) && (getMousePos.x>=(minX*sizeDeltaAsFactor.x)))
			{
				this.transform.position=new Vector2(getMousePos.x,this.transform.position.y);
				//Debug.Log("xpos mouse : "+getMousePos);
				//Debug.Log("xpos slider: "+this.transform.position);
				currPos=timeSlider.transform.position.x;
				int maxSec=convertMinutesToSeconds(length);
				int timeByPos=getTimeByXpos(minX,maxX,(currPos/sizeDeltaAsFactor.x),maxSec);
				string minStr=convertSecondsToMinutes(timeByPos);
				//Debug.Log("seconds: " +maxSec);
				//Debug.Log("mouspos.x: " +currPos);
				//Debug.Log("seconds by pos: " +timeByPos);
				//Debug.Log("minutes: " +minStr);
				timeSliderText.text=minStr;
			}
		}
		if (Input.GetMouseButtonUp(0)) //left mouse button up
		{
			//Debug.Log("left mouse button up");
			moving=false;
			dragging=false;
		}
    }
}
