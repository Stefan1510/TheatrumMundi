using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timelineOpenClose : MonoBehaviour
{
	Image timelineImage;
	public Text timelineText;
	Vector2 textSize;
	bool timelineOpen;				//timeline open/close
	bool clickingTimeline;			//if timeline is clicked
	bool movingOnTimeline;			//is the mouse over the timeline
	bool draggingOnTimeline;		//dragging the mouse over the timeline
	bool clickingObject;			//do you only click the object
	bool movingObject;				//is the mouse over the object
	bool draggingObject;			//dragging an object with mouse
	bool objectOnTimeline;			//is object on timeline
	double minX;
	double maxX;
	double minY;
	double maxY;
	Vector2 sizeDeltaAsFactor;
	public Button myButton;
	Vector2 objectDefaultPosition;
	Vector2 objectDefaultSize;
	Vector2 objectSceneSize;
	
	public GameObject objectLibrary;
	public GameObject mainMenue;
	
	GameObject[] figureObjects;
	
    // Start is called before the first frame update
    void Start()
    {
        timelineImage=GetComponent<Image>();
		timelineOpen=false;
		clickingTimeline=false;
		movingOnTimeline=false;
		draggingOnTimeline=false;
		clickingObject=false;
		movingObject=false;
		draggingObject=false;
		objectOnTimeline=false;
		minX=301.0f;
		maxX=1623.0f;
		minY=428.0f;
		maxY=438.0f;
		sizeDeltaAsFactor=new Vector2(1.0f,1.0f);
		Debug.Log("++++++timeline calling");
		
		/*
		objectDefaultPosition=myButton.transform.position;
		objectDefaultSize.x=myButton.GetComponent<RectTransform>().rect.width;
		objectDefaultSize.y=myButton.GetComponent<RectTransform>().rect.height;
		objectSceneSize.x=myButton.GetComponent<RectTransform>().rect.width;
		objectSceneSize.y=myButton.GetComponent<RectTransform>().rect.height;
		*/
		
		textSize=new Vector2(timelineText.GetComponent<RectTransform>().sizeDelta.x, timelineText.GetComponent<RectTransform>().sizeDelta.y);
		
		//load all objects given in the figuresShelf
		//Debug.Log("objectscount: "+objectLibrary.transform.childCount);
		figureObjects=new GameObject[3];
		for(int i=0;i<objectLibrary.transform.childCount;i++)
		{
			figureObjects[i]=(objectLibrary.transform.GetChild(i).GetChild(1).gameObject);
			//Debug.Log("one figure: "+figureObjects[0]);					//returns the empty
			//Debug.Log("one figure: "+figureObjects[0].GetChild(1));		//returns the childs of this empty e.g. the button
		}
		//Debug.Log("figureObjectscount: "+figureObjects.GetLength(0));	//the int is the dimension: 0 for array like [o1,o2,...]
		//Debug.Log("one figure: "+figureObjects[0]);
		//Debug.Log("one figure-pos: "+figureObjects[0].transform.position);
		
		//example for the first element
		objectDefaultPosition=figureObjects[0].transform.position;
		objectDefaultSize.x=figureObjects[0].GetComponent<RectTransform>().rect.width;
		objectDefaultSize.y=figureObjects[0].GetComponent<RectTransform>().rect.height;
		objectSceneSize.x=figureObjects[0].GetComponent<RectTransform>().rect.width;
		objectSceneSize.y=figureObjects[0].GetComponent<RectTransform>().rect.height;
		
		//change parent to root
		//figureObjects[0].GetChild(1).transform.SetParent(this.transform);	//parent to timeline
		
		//this is the root-parent
		//mainMenue=GameObject.Find("MenueDirectorMain");
		mainMenue=GameObject.Find("timelineArea");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 getMousePos=Input.mousePosition;
		
		if (Input.GetMouseButton(0))	//left mouse button is held down
		{
			//Debug.Log("mouse btn is held down");
		}
		if (Input.GetMouseButtonDown(0)) //left mouse button clicked down
		{
			//Debug.Log("++++++left mouse button down");
			//Debug.Log("++++++xpos mouse : "+getMousePos);
			
			//if you touch the timeline with the mouse
			if (this.GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			{
				movingOnTimeline=true;
				clickingTimeline=true;
				Debug.Log("--->>>>click the timeline mousepos: "+getMousePos.x +";"+getMousePos.y+" (collider: "+this.GetComponent<BoxCollider2D>()+";"+this.GetComponent<BoxCollider2D>()+")");
				timelineOpen=true;
			}
			else
			{
				movingOnTimeline=false;
				clickingTimeline=false;
				Debug.Log("--->no timeline-click mousepos: "+getMousePos.x +";"+getMousePos.y+" (collider: "+this.GetComponent<BoxCollider2D>()+";"+this.GetComponent<BoxCollider2D>()+")");
				//close timeline if you click somewhere else
				timelineImage.rectTransform.localScale = new Vector3(1,1,1);
				timelineText.rectTransform.localScale = new Vector3(1,1,1);
				timelineOpen=false;
				//if the object is on timeline
				if (objectOnTimeline)
				{
					//minimize the object
					objectSceneSize.y=11.0f;
					//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					figureObjects[0].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					//deactive object dragging
					draggingObject=false;
				}
			}
			//if the button is clicked
			//if (myButton.GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			//if (figureObjects[0].transform.GetChild(1).GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			if (figureObjects[0].GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			{
				//Debug.Log("***button hit");
				//myButton.transform.position=new Vector2(getMousePos.x,getMousePos.y);
				Debug.Log("***object hit");
				//figureObjects[0].transform.GetChild(1).transform.position=new Vector2(getMousePos.x,getMousePos.y);
				figureObjects[0].transform.position=new Vector2(getMousePos.x,getMousePos.y);
				//change parent to root
				//Debug.Log("figureObjects before settingg Parent "+figureObjects[0].transform.GetChild(0));
				//Debug.Log("figureObjects before settingg Parent "+figureObjects[0].transform.GetChild(1));
				//figureObjects[0].transform.GetChild(1).transform.SetParent(mainMenue.transform);	//parent to timeline
				figureObjects[0].transform.SetParent(mainMenue.transform);
				//figureObjects[0].transform.SetParent(this.transform);
				//Debug.Log("figureObjects after settingg Parent "+figureObjects[0].transform.GetChild(0));
				//Debug.Log("figureObjects after setting Parent "+figureObjects[0].transform.GetChild(1));
				clickingObject=true;
				movingObject=true;
			}
			else
			{
				Debug.Log("´´´no button hit detected");
				movingObject=false;
				clickingObject=false;
			}
			if (movingOnTimeline)
			{
				//space for dragging
				draggingOnTimeline=true;
			}
			if (movingObject)
			{
				draggingObject=true;
			}
		}
		if (draggingOnTimeline)
		{
			//if a click is in the timeline
			if (((getMousePos.x<=((maxX*sizeDeltaAsFactor.x))) && (getMousePos.x>=(minX*sizeDeltaAsFactor.x))) &&
			((getMousePos.y<=((maxY*sizeDeltaAsFactor.y))) && (getMousePos.y>=(minY*sizeDeltaAsFactor.y))))
			{
				//Debug.Log("timeline hitted: "+timelineImage.sprite.rect.width);
				
				//open/maximize the timeline
				timelineImage.rectTransform.localScale = new Vector3(1,5,1);
				//Vector2 textPos=new Vector2(timelineText.GetComponent<RectTransform>().rect.position.x,timelineText.GetComponent<RectTransform>().rect.position.y);
				//Debug.Log("textpos "+textPos);
				timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
				//textPos=new Vector2(timelineText.GetComponent<RectTransform>().rect.position.x,timelineText.GetComponent<RectTransform>().rect.position.y);
				//Debug.Log("textpos "+textPos);
				//timelineText.GetComponent<RectTransform>().sizeDelta = new Vector2(textSize.x,textSize.y);
				//scale up the object
				objectSceneSize.y=objectDefaultSize.y;				
				//this maximize also the object
				//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
				figureObjects[0].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
				//if object is on timeline -> allow to dragging
			}
		}
		else
		{
			//if you click outside the timeline -> close timeline
			//Debug.Log("rail not hitted: "+timelineImage.sprite.rect.width);
		}
		if (draggingObject)
		{
			//all following commands only accepted by object
			//if you hit the object
			//Debug.Log("--->>>>mousepos during update: "+getMousePos.x +";"+getMousePos.y);
			//object is only free (in x AND y direction) moving if its not in the timeline
			if (!objectOnTimeline)
			{
				//myButton.transform.position=new Vector2(getMousePos.x,getMousePos.y);
				//figureObjects[0].transform.GetChild(1).transform.position=new Vector2(getMousePos.x,getMousePos.y);
				figureObjects[0].transform.position=new Vector2(getMousePos.x,getMousePos.y);
			}
			//hit the timeline while you are dragging the object
			if (((getMousePos.x<=((maxX*sizeDeltaAsFactor.x))) && (getMousePos.x>=(minX*sizeDeltaAsFactor.x))) &&
			((getMousePos.y<=((maxY*sizeDeltaAsFactor.y))) && (getMousePos.y>=(minY*sizeDeltaAsFactor.y))))
			{
				//Debug.Log("you hit the timeline with the object");
				//save that the object is now only movable on timeline
				objectOnTimeline=true;
				//"open" the timeline
				timelineImage.rectTransform.localScale = new Vector3(1,5,1);
				timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
				Debug.Log("object on timeline: "+objectOnTimeline);
				Debug.Log("object moveable: "+movingObject);
				//snap the object to the timeline
				//myButton.transform.position=new Vector2(getMousePos.x,this.transform.position.y);
				figureObjects[0].transform.position=new Vector2(getMousePos.x,this.transform.position.y);
				//if the object is snapped, you can only move the object horizontal (x-direction) on the timeline
				if (objectOnTimeline && movingObject)
				{
					//now you can only move the object on timeline, deletable only by x-button
					//set the animation length to object-width
					float animationlength=100.0f;
					objectSceneSize.x=animationlength;
					//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					figureObjects[0].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					//object should not able to x-move over timeline start and end -> we need the size
						//double offsetObjectsWidth=myButton.GetComponent<RectTransform>().rect.width;
						//Debug.Log("button width:"+offsetObjectsWidth);
					//set the object position on timeline
					//if the object is on timeline but not hitted by the mouse, then there is no moving
					if (clickingObject)
					{
						Debug.Log("clicking object: "+clickingObject);
						objectSceneSize.y=objectDefaultSize.y;
						//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
						figureObjects[0].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
						//myButton.transform.position=new Vector2(getMousePos.x,myButton.transform.position.y);
						figureObjects[0].transform.position=new Vector2(getMousePos.x,figureObjects[0].transform.position.y);
					}
					else
					{
						Debug.Log("clicking object(else): "+clickingObject);
						Debug.Log("drag the object to move, clicking will be ignored");
					}
				}
			}			
		}
		else if(draggingObject==false && draggingOnTimeline==false)
		{
			//if you do not dragging an object and you dont drag your mouse on timeline
			//close timeline
			//timelineImage.rectTransform.localScale = new Vector3(1,1,1);
			if (objectOnTimeline)
			{
				//scale object down if its on timeline
				//objectSceneSize.y=11.0f;
				//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
			}
		}
		if (objectOnTimeline==true && draggingOnTimeline==true)
		{
			//edit object which is already on timeline
			//draggingObject=false;
			//Debug.Log("i want to edit object again!");
			/*if (draggingObject)
			{
				myButton.transform.position=new Vector2(getMousePos.x,myButton.transform.position.y);
			}*/
			//draggingObject=true;
		}
		if (Input.GetMouseButtonUp(0)) //left mouse button up
		{
			//Debug.Log("++++++left mouse button up");
			clickingTimeline=false;
			movingOnTimeline=false;
			draggingOnTimeline=false;
			clickingObject=false;
			movingObject=false;
			draggingObject=false;
		}
    }
}
