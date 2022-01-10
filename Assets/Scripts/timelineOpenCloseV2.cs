using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timelineOpenCloseV2 : MonoBehaviour
{
	Image timelineImage;
	public Text timelineText;
	public Image timeSliderImage;
	private BoxCollider2D timeSlider;
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
	//public Button myButton;
	Vector2 objectDefaultPosition;
	Vector2[] objectShelfPosition;
	GameObject[] objectShelfParent;
	Vector2 objectDefaultSize;
	Vector2 objectSceneSize;
	
	public GameObject objectLibrary;
	public GameObject mainMenue;
	
	GameObject[] figureObjects;
	int currentClickedObjectIndex;
	
	public List<GameObject> timelineObjects;
	
	public GameObject gameController;
	private FigureElement ThisFigureElement;	//element to set 3d object
	public GameObject figure1;
	public GameObject figure2;
	double fig1StartPos;
	double fig2StartPos;
	Vector3 rail1StartPos;
	Vector3 rail2StartPos;
	Vector3 rail3StartPos;
	Vector3 rail4StartPos;
	Vector3 rail5StartPos;
	Vector3 rail6StartPos;
	
    // Start is called before the first frame update
    void Start()
    {
        timelineImage=this.GetComponent<Image>();
		//timeSliderImage=GetComponent<Image>();
		//timeSlider=timeSliderImage.GetComponent<BoxCollider2D>();
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
		figureObjects=new GameObject[objectLibrary.transform.childCount]; //instead of a count like "3"
		objectShelfPosition=new Vector2[figureObjects.Length];
		objectShelfParent=new GameObject[figureObjects.Length];
		for(int i=0;i<objectLibrary.transform.childCount;i++)
		{
			//collect objects
			figureObjects[i]=(objectLibrary.transform.GetChild(i).GetChild(1).gameObject);
			//Debug.Log("one figure: "+figureObjects[0]);					//returns the empty
			//Debug.Log("one figure: "+figureObjects[0].GetChild(1));		//returns the childs of this empty e.g. the button
			
			//store default position
			objectShelfPosition[i]=new Vector2(figureObjects[i].transform.position.x,figureObjects[i].transform.position.y);
			//store default parentName
			objectShelfParent[i]=new GameObject(figureObjects[i].transform.parent.gameObject.name);
		}
		Debug.Log("++++++figures loaded: "+figureObjects.Length);
		
		//example for the first element
		objectDefaultPosition=figureObjects[0].transform.position;
		objectDefaultSize.x=figureObjects[0].GetComponent<RectTransform>().rect.width;
		objectDefaultSize.y=figureObjects[0].GetComponent<RectTransform>().rect.height;
		objectSceneSize.x=figureObjects[0].GetComponent<RectTransform>().rect.width;
		objectSceneSize.y=figureObjects[0].GetComponent<RectTransform>().rect.height;
		//Debug.Log("object width,height: "+objectDefaultSize);
		currentClickedObjectIndex=-1;
		
		//change parent to root
		//figureObjects[0].GetChild(1).transform.SetParent(this.transform);	//parent to timeline
		
		//this is the root-parent
		//mainMenue=GameObject.Find("MenueDirectorMain");
		mainMenue=GameObject.Find("timelineArea");
		
		//get figure element
		//gameController = GameObject.Find("GameController");
		//Debug.Log(">>>figure elements existing: "+StaticSceneData.StaticData.figureElements);
		//ThisFigureElement = StaticSceneData.StaticData.figureElements.Find(x => x.name == gameObject.name.Substring(6));
		//Debug.Log(">>>figure element: "+ThisFigureElement);
		//Debug.Log(">>>figure elements existing: "+figure1.name);
		fig1StartPos=-1.75f;
		fig2StartPos=1.88f;
		Vector3 rail1StartPos=new Vector3(0.0f,-0.009f,-2.0f);
		Vector3 rail2StartPos=new Vector3(-0.119f,0.146f,-2.0f);
		Vector3 rail3StartPos=new Vector3(-0.319f,0.146f,-2.0f);
		Vector3 rail4StartPos=new Vector3(-0.439f,0.146f,-2.0f);
		Vector3 rail5StartPos=new Vector3(-0.642f,0.146f,-2.0f);
		Vector3 rail6StartPos=new Vector3(-0.759f,0.146f,-2.0f);
		
		//list of objects in the timeline
		//create an empty list
		List<GameObject> timelineObjects=new List<GameObject>();
    }

	public string identifyClickedObject()
	{
		string objName="";
		if (figureObjects.Length==0)
			return "no object found! [-1]";				//error-handling if no clickable object found
		
		for(int i=0;i<figureObjects.Length;i++)
		{
			//save last correct index
			int oldClickedObject=currentClickedObjectIndex;
			//which object in grid layout is clicked
			if(figureObjects[i].GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(Input.mousePosition))
			{
				//save the index of clicked object
				currentClickedObjectIndex=i;
				//if you click an new object
				if (oldClickedObject!=currentClickedObjectIndex)
				{
					//set objectSceneSize-Variables back to default
					//objectDefaultPosition=figureObjects[currentClickedObjectIndex].transform.position;
					//objectDefaultSize.x=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.width;
					//objectDefaultSize.y=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.height;
					//objectSceneSize.x=objectDefaultSize.x;
					//objectSceneSize.y=objectDefaultSize.y;
					//objectOnTimeline=false;
				}
				return figureObjects[i].GetComponent<BoxCollider2D>().name;
			}
			else
			{
				objName="no object clicked!";
			}
		}
		
		return objName;
	}
	public void openCloseTimelineByClick(bool flag, Image tl)
	{
		float colliderScaleUpFacY=50.0f;
		float colliderScaleDownFacY=20.0f;	//this size is set in editor
		
		if (flag==false)
		{
			//set global flag
			timelineOpen=true;
			//scale up timeline
			tl.rectTransform.localScale = new Vector3(1,5,1);
			//scale up the collider
			tl.GetComponent<BoxCollider2D>().size=new Vector2(tl.GetComponent<BoxCollider2D>().size.x,colliderScaleUpFacY);
			//scale down the text
			tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1,0.2f,1);
			//timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
		}
		else
		{
			//set global flag
			timelineOpen=false;
			//scale down timeline
			tl.rectTransform.localScale = new Vector3(1,1,1);
			//scale down the collider
			tl.GetComponent<BoxCollider2D>().size=new Vector2(tl.GetComponent<BoxCollider2D>().size.x,colliderScaleDownFacY);
			//scale up the text
			tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1,1,1);
		}
	}
    public void openCloseTimelineByDrag(string open, Image tl)
	{
		float colliderScaleUpFacY=50.0f;
		float colliderScaleDownFacY=20.0f;	//this size is set in editor
		
		if (open=="open")
		{
			timelineOpen=true;
			//scale up timeline
			tl.rectTransform.localScale = new Vector3(1,5,1);
			//scale up the collider
			tl.GetComponent<BoxCollider2D>().size=new Vector2(tl.GetComponent<BoxCollider2D>().size.x,colliderScaleUpFacY);
			//scale down the text
			tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1,0.2f,1);
			//timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
		}
		else
		{
			//if open=="close"
			timelineOpen=false;
			//scale down timeline
			tl.rectTransform.localScale = new Vector3(1,1,1);
			//scale down the collider
			tl.GetComponent<BoxCollider2D>().size=new Vector2(tl.GetComponent<BoxCollider2D>().size.x,colliderScaleDownFacY);
			//scale up the text
			tl.transform.GetChild(0).gameObject.GetComponent<Text>().rectTransform.localScale = new Vector3(1,1,1);
		}
	}
	public void saveParent(GameObject obj)
	{
		GameObject oldParent;
		//save old parent
		oldParent=obj.transform.parent.gameObject;		//parent is a transform, to get the gameobject of this parent, use ".gameObject"
		//Debug.Log("old parent "+oldParent);
	}
	public void setParent(GameObject obj, GameObject parentToSet)
	{
		saveParent(obj);
		//set new parent
		//Debug.Log("new parent "+parentToSet);
		obj.transform.SetParent(parentToSet.transform);
	}
	public void updateObjectPosition(GameObject obj,Vector2 mousePos)
	{
		//set parent
		//obj.transform.SetParent(mainMenue.transform);
		setParent(obj,mainMenue);
		//move object
		obj.transform.position=new Vector2(mousePos.x,mousePos.y);
		//set up flags
		
	}
	public bool checkHittingTimeline(GameObject obj, Image tl, Vector2 mousePos)
	{
		bool hit=false;
		/*Debug.Log("tl pos "+tl.transform.position); //global pos are the real position-data
		Debug.Log("tl collider "+tl.GetComponent<BoxCollider2D>().size);
		Debug.Log("mouse "+mousePos);
		Debug.Log("obj collider "+obj.GetComponent<BoxCollider2D>().size);
		*/
		//calculate bounding-box related to the timeline-pos
		Vector2 tlPos=new Vector2(tl.transform.position.x,tl.transform.position.y);
		Vector2 colSize=new Vector2(tl.GetComponent<BoxCollider2D>().size.x,tl.GetComponent<BoxCollider2D>().size.y);
		//if mouse hits the timeline while dragging an object
		//my implementation of object-boundingbox
		if(((mousePos.x <= (tlPos.x+(colSize.x/2.0f))) && (mousePos.x > (tlPos.x-(colSize.x/2.0f)))) &&
		((mousePos.y <= (tlPos.y+(colSize.y/2.0f))) && (mousePos.y > (tlPos.y-(colSize.y/2.0f)))))
		{
			Debug.Log("object hits timeline!");
			hit=true;;
		}
		Debug.Log ("drag and hit "+hit);
		return hit;
	}
	public void openCloseObjectInTimeline(bool timelineOpen, List<GameObject> objects)
	{
		float scaleUp=50.0f;
		float scaleDown=10.0f;
		float length=100.0f;
		Debug.Log("timelineObjects count: "+objects.Count);
		for(int i=0;i<objects.Count;i++)
		{
			//if timeline open scale ALL objects up
			if (timelineOpen)
				scaleObject(objects[i],length,scaleUp);
				//objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,50.0f);
			//new Vector2(animationLength,scaleYUp);
			else
			//otherwise scale ALL objects down
				scaleObject(objects[i],length,scaleDown);
				//objects[i].GetComponent<RectTransform>().sizeDelta=new Vector2(150.0f,10.0f);
		}
	}
	public void scaleObject(GameObject fig,float x,float y)
	{
		fig.GetComponent<RectTransform>().sizeDelta=new Vector2(x,y);
	}
	public void scaleObjectsOfList(List<GameObject> objects, float x, float y)
	{
		for (int i=0;i<objects.Count;i++)
		{
			scaleObject(objects[i],x,y);
		}
	}
	public void setObjectOnTimeline(GameObject fig,float x,float y)
	{
		fig.transform.position=new Vector3(fig.transform.position.x, y,0.0f);
	}
	public void updateObjectList(List<GameObject> objects, GameObject obj)
	{
		//check if the object is already in list
		if (objects.Contains(obj))
		{
			//do nothing
		}
		else
		{
			objects.Add(obj);
		}
	}
	public void set3DObject()
	{
		fig1StartPos=fig1StartPos+0.005f;
		figure1.transform.position=new Vector3(rail1StartPos.x,figure1.transform.position.y,(float)fig1StartPos);		//start-pos on the left side in timeline
		fig2StartPos=fig2StartPos-0.005f;
		figure2.transform.position=new Vector3(rail3StartPos.x,figure2.transform.position.y,(float)fig2StartPos);		//start-pos on the right side in timeline
	}
	public bool isInList(List<GameObject> objects,GameObject obj)
	{
		return objects.Contains(obj);
	}
	public bool moveTimeSlider(Image myTL)
	{
		Vector2 mousePos=Input.mousePosition;
		//Debug.Log("timeslider mouse-pos: "+mousePos);
		//Debug.Log("timeslider-image "+myTL);
		//test if you move the slider or click elsewhere in the timelinearea
		//calculate bounding-box related to the timeline-pos
		Vector2 tlPos=new Vector2(myTL.transform.position.x,myTL.transform.position.y);
		Vector2 colSize=new Vector2(myTL.GetComponent<BoxCollider2D>().size.x,myTL.GetComponent<BoxCollider2D>().size.y);
		//Debug.Log("timeslider colBox: "+myTL.GetComponent<BoxCollider2D>().size);
		//Debug.Log("timeslider pos: "+myTL.transform.position);
		//if mouse hits the timeline while dragging an object
		//my implementation of object-boundingbox
		if(((mousePos.x <= (tlPos.x+(colSize.x/2.0f))) && (mousePos.x > (tlPos.x-(colSize.x/2.0f)))) &&
		((mousePos.y <= (tlPos.y+(colSize.y/2.0f))) && (mousePos.y > (tlPos.y-(colSize.y/2.0f)))))
		{
			Debug.Log("timeslider hit!");
			return true;
		}
		else
			return false;
	}
	// Update is called once per frame
    void Update()
    {
        Vector2 getMousePos=Input.mousePosition;
		
		if (Input.GetMouseButtonDown(0)) //left mouse button clicked down
		{
			//identifiy which gameobject do you clicked
			string objectClicked=identifyClickedObject();	//method fills up the current clicked index
			Debug.Log("you clicked object: "+objectClicked);
			
			//check if you hit the timeslider
			//Debug.Log("timeslider hit? "+moveTimeSlider(timeSliderImage));
			
			//if you click the timeline with the mouse
			if (this.GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			{
				//open or close timeline
				openCloseTimelineByClick(timelineOpen, timelineImage);
				//minimize or maximize objects on timeline
				openCloseObjectInTimeline(timelineOpen, timelineObjects);
				movingOnTimeline=true;		//drag on the timeline
				//clickingTimeline=true;	//may be the same as timelineOpen
			}
			//if you click an object in object shelf with the mouse
			//first check if you have an object clicked
			if (currentClickedObjectIndex!=(-1))
			{
				if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
				{
					//set up some flags
					clickingObject=true;	//do you clicked the object
					movingObject=true;		//want to drag the object
					//save default values
					//objectDefaultPosition=figureObjects[currentClickedObjectIndex].transform.position;
					//objectDefaultSize.x=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.width;
					//objectDefaultSize.y=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.height;
				}
			}
			else
			{
				/*
				movingOnTimeline=false;
				clickingTimeline=false;
				Debug.Log("--->no timeline-click mousepos: "+getMousePos.x +";"+getMousePos.y+" (collider: "+this.GetComponent<BoxCollider2D>()+";"+this.GetComponent<BoxCollider2D>()+")");
				Debug.Log("curr timeline collider-size: "+timelineImage.GetComponent<BoxCollider2D>().size);
				//close timeline if you click somewhere else
				timelineImage.rectTransform.localScale = new Vector3(1,1,1);
				float scaleObjY=11.0f;
				timelineImage.GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,scaleObjY);
				timelineText.rectTransform.localScale = new Vector3(1,1,1);
				timelineOpen=false;
				//if the object is on timeline
				if (objectOnTimeline)
				{
					//minimize the object
					objectSceneSize.y=11.0f;
					//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					//scale down the collider
					figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,objectSceneSize.y);
					//deactive object dragging
					draggingObject=false;
				}*/
			}
			/*
			//if the button is clicked
			//if (myButton.GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			//if (figureObjects[0].transform.GetChild(1).GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			if (figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			{
				//Debug.Log("***button hit");
				//myButton.transform.position=new Vector2(getMousePos.x,getMousePos.y);
				Debug.Log("***object hit: "+figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>().size);
				//figureObjects[0].transform.GetChild(1).transform.position=new Vector2(getMousePos.x,getMousePos.y);
				figureObjects[currentClickedObjectIndex].transform.position=new Vector2(getMousePos.x,getMousePos.y);
				//change parent to root
				//Debug.Log("figureObjects before settingg Parent "+figureObjects[0].transform.GetChild(0));
				//Debug.Log("figureObjects before settingg Parent "+figureObjects[0].transform.GetChild(1));
				//figureObjects[0].transform.GetChild(1).transform.SetParent(mainMenue.transform);	//parent to timeline
				figureObjects[currentClickedObjectIndex].transform.SetParent(mainMenue.transform);
				//figureObjects[0].transform.SetParent(this.transform);
				//Debug.Log("figureObjects after settingg Parent "+figureObjects[0].transform.GetChild(0));
				//Debug.Log("figureObjects after setting Parent "+figureObjects[0].transform.GetChild(1));
				clickingObject=true;
				movingObject=true;
			}
			else
			{
				Debug.Log("´´´no button hit detected. curr collider-size: "+figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>().size);
				movingObject=false;
				clickingObject=false;
			}
			*/
			if (movingOnTimeline)
			{
				//space for dragging
				Debug.Log("ready to drag something on the timeline...");
				draggingOnTimeline=true;
			}
			
			if (movingObject)
			{
				Debug.Log("ready to drag an object...");
				draggingObject=true;
			}
		}
		if (draggingOnTimeline)
		{
			/*
			//if a click is in the timeline
			if (((getMousePos.x<=((maxX*sizeDeltaAsFactor.x))) && (getMousePos.x>=(minX*sizeDeltaAsFactor.x))) &&
			((getMousePos.y<=((maxY*sizeDeltaAsFactor.y))) && (getMousePos.y>=(minY*sizeDeltaAsFactor.y))))
			{
				//Debug.Log("timeline hitted: "+timelineImage.sprite.rect.width);
				
				//open/maximize the timeline
				timelineImage.rectTransform.localScale = new Vector3(1,5,1);
				float scaleObjY=55.0f;
				timelineImage.GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,scaleObjY);
				//Vector2 textPos=new Vector2(timelineText.GetComponent<RectTransform>().rect.position.x,timelineText.GetComponent<RectTransform>().rect.position.y);
				//Debug.Log("textpos "+textPos);
				timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
				//textPos=new Vector2(timelineText.GetComponent<RectTransform>().rect.position.x,timelineText.GetComponent<RectTransform>().rect.position.y);
				//Debug.Log("textpos "+textPos);
				//timelineText.GetComponent<RectTransform>().sizeDelta = new Vector2(textSize.x,textSize.y);
				//scale up the object (old)
				//objectSceneSize.y=objectDefaultSize.y;
				//scale the object to timeline-size
				objectSceneSize.y=55.0f;				
				//this maximize also the object
				//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
				//Debug.Log("object default width,height: "+objectDefaultSize+" | currenSize: "+objectSceneSize);
				figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
				//scale up the collider
				figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,objectSceneSize.y);
				//if object is on timeline -> allow to dragging
			}
			*/
		}
		if (draggingObject)
		{
			//move object
			updateObjectPosition(figureObjects[currentClickedObjectIndex],getMousePos);
			//if you hit the timeline > object snap to timeline and is locked in y-movement-direction
			bool hitTimeline=false;
			hitTimeline=checkHittingTimeline(figureObjects[currentClickedObjectIndex], timelineImage, getMousePos);
			if(hitTimeline)
			{
				//open timeline, if its not open
				Debug.Log(">>>flag timelineOpen: "+timelineOpen);
				openCloseTimelineByDrag("open", timelineImage);
				//scale down object + set animation-length
				float animationLength=100.0f;		//length of objects animation
				float scaleYUp=50.0f;				//size if the timeline is maximized
				//figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(animationLength,scaleYUp);
				//scaleObject(figureObjects[currentClickedObjectIndex], animationLength, scaleYUp);
				scaleObjectsOfList(timelineObjects, animationLength, scaleYUp);
				//lock y-axis
				//figureObjects[currentClickedObjectIndex].transform.position=new Vector3(figureObjects[currentClickedObjectIndex].transform.position.x, this.transform.position.y,0.0f);
				setObjectOnTimeline(figureObjects[currentClickedObjectIndex],figureObjects[currentClickedObjectIndex].transform.position.x,this.transform.position.y);
				//set placed figures to timelineobjects-list
				updateObjectList(timelineObjects,figureObjects[currentClickedObjectIndex]);
				set3DObject();
				//set up flags
			}
		}
		/*
		if (draggingObject)
		{
			//all following commands only accepted by object
			//if you hit the object
			//Debug.Log("--->>>>mousepos during update: "+getMousePos.x +";"+getMousePos.y);
			//object is only free (in x AND y direction) if its not in the timeline
			if (!objectOnTimeline)
			{
				//myButton.transform.position=new Vector2(getMousePos.x,getMousePos.y);
				//figureObjects[0].transform.GetChild(1).transform.position=new Vector2(getMousePos.x,getMousePos.y);
				figureObjects[currentClickedObjectIndex].transform.position=new Vector2(getMousePos.x,getMousePos.y);
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
				float scaleObjY=55.0f;
				timelineImage.GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,scaleObjY);
				//Debug.Log("timeline-collider size: "+timelineImage.GetComponent<BoxCollider2D>().size);
				timelineText.rectTransform.localScale = new Vector3(1,0.2f,1);
				Debug.Log("object on timeline: "+objectOnTimeline);
				Debug.Log("object moveable: "+movingObject);
				//snap the object to the timeline
				//myButton.transform.position=new Vector2(getMousePos.x,this.transform.position.y);
				figureObjects[currentClickedObjectIndex].transform.position=new Vector2(getMousePos.x,this.transform.position.y);
				
				if (currentClickedObjectIndex==0)
				{
					Debug.Log("<<< figure-pos: "+figure1.transform.position);
					Debug.Log("<<< idx= "+currentClickedObjectIndex);
					fig1StartPos=fig1StartPos+0.005f;
					figure1.transform.position=new Vector3(rail1StartPos.x,figure1.transform.position.y,(float)fig1StartPos);		//start-pos on the left side in timeline
					//figure1.transform.localScale=new Vector3(1.0f,1.0f,1.0f);
					//figure1.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
					//figure1.GetComponent<Animator>().speed=1.0f;
					//figure1.GetComponent<Animator>().SetFloat("direction", -1);		//reverse animation
					//figure1.GetComponent<Animator>().enabled=false;		//stop and start animation
				}
				else if (currentClickedObjectIndex==2)
				{
					Debug.Log("<<< figure2-pos: "+figure2.transform.position);
					Debug.Log("<<< idx= "+currentClickedObjectIndex);
					fig2StartPos=fig2StartPos-0.005f;
					figure2.transform.position=new Vector3(rail3StartPos.x,figure2.transform.position.y,(float)fig2StartPos);		//start-pos on the right side in timeline
					//figure2.transform.localScale=new Vector3(1.0f,1.0f,1.0f);
					//figure2.GetComponent<Animator>().SetFloat("direction", 1);
					//figure2.GetComponent<Animator>().enabled=true;		//stop and start animation
				}
				
				//if the object is snapped, you can only move the object horizontal (x-direction) on the timeline
				if (objectOnTimeline && movingObject)
				{
					//now you can only move the object on timeline, deletable only by x-button
					//set the animation length to object-width
					float animationlength=100.0f;
					objectSceneSize.x=animationlength;
					//scale the object to timeline-size
					objectSceneSize.y=55.0f;
					//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
					//scale up the collider
					figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,objectSceneSize.y);
					//object should not able to x-move over timeline start and end -> we need the size
						//double offsetObjectsWidth=myButton.GetComponent<RectTransform>().rect.width;
						//Debug.Log("button width:"+offsetObjectsWidth);
					//set the object position on timeline
					//if the object is on timeline but not hitted by the mouse, then there is no moving
					if (clickingObject)
					{
						Debug.Log("clicking object: "+clickingObject);
						//objectSceneSize.y=objectDefaultSize.y;
						objectSceneSize.y=55.0f;
						//myButton.GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
						figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().sizeDelta=new Vector2(objectSceneSize.x,objectSceneSize.y);
						//myButton.transform.position=new Vector2(getMousePos.x,myButton.transform.position.y);
						figureObjects[currentClickedObjectIndex].transform.position=new Vector2(getMousePos.x,figureObjects[currentClickedObjectIndex].transform.position.y);
						//scale up the collider
						figureObjects[currentClickedObjectIndex].GetComponent<BoxCollider2D>().size=new Vector2(timelineImage.GetComponent<BoxCollider2D>().size.x,objectSceneSize.y);
					}
					else
					{
						Debug.Log("clicking object(else): "+clickingObject);
						Debug.Log("drag the object to move, clicking will be ignored");
					}
				}
			}			
		}
		*/
		/*
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
		*/
		/*
		if (objectOnTimeline==true && draggingOnTimeline==true)
		{
			//edit object which is already on timeline
			//draggingObject=false;
			//Debug.Log("i want to edit object again!");
			//if (draggingObject)
			//{
			//	myButton.transform.position=new Vector2(getMousePos.x,myButton.transform.position.y);
			//}
			//draggingObject=true;
		}	
		*/
		if (Input.GetMouseButtonUp(0)) //left mouse button up
		{
			//Debug.Log("++++++left mouse button up");
			clickingTimeline=false;
			movingOnTimeline=false;
			draggingOnTimeline=false;
			clickingObject=false;
			movingObject=false;
			draggingObject=false;
			/*
			// ------------ Dinge, die f�r die Kulissen im Controler passieren m�ssen

			//ThisSceneryElement.parent = "Schiene" + statusReiter.ToString(); //hier bitte mal genau �berpr�fen wie die parents dann wirklich sind
			Debug.Log("---- Figurename -------- " + ThisFigureElement.name + " --- " + ThisFigureElement.parent + " --- " + ThisFigureElement.x);
			ThisFigureElement.z = GetComponent<RectTransform>().anchoredPosition.x/300;
			ThisFigureElement.y = GetComponent<RectTransform>().anchoredPosition.y/300+0.1f;  //die werte stimmen ungefaehr mit dem liveview ueberein


			// ------------ uebertragen der Daten aus dem Controller auf die 3D-Kulissen
			gameController.GetComponent<SceneDataController>().CreateScene(StaticSceneData.StaticData); // dieses Zeile macht das gleiche und ist glaube besser.
			*/
		}
		/*
		//
		if (Input.GetMouseButtonUp(0) && (!movingObject))
		{
			bool hitTimeline=false;
			if(currentClickedObjectIndex>=0)
				hitTimeline=checkHittingTimeline(figureObjects[currentClickedObjectIndex], timelineImage, getMousePos);
			else
				hitTimeline=false;
			//set object position back to shelf
			if ((!hitTimeline) && (currentClickedObjectIndex>=0) && (!moveTimeSlider(timeSliderImage)))
			{
				//Debug.Log("timeSlider="+moveTimeSlider(timeSliderImage));
				string objectClicked=identifyClickedObject();	//get name of identified object
				bool isPresent=isInList(timelineObjects,figureObjects[currentClickedObjectIndex]);
				if (isPresent)
				{
					Debug.Log("you released object in space "+objectClicked+" bool: "+isPresent+" name: "+figureObjects[currentClickedObjectIndex].name);
					Debug.Log("empty-parent: "+objectShelfParent[currentClickedObjectIndex]);
					Debug.Log("empty-child: "+figureObjects[currentClickedObjectIndex].transform.GetChild(0));
					figureObjects[currentClickedObjectIndex].transform.position=objectShelfPosition[currentClickedObjectIndex];
					scaleObject(figureObjects[currentClickedObjectIndex],250.0f,250.0f);
					//remove object out of objectlist
					timelineObjects.Remove(figureObjects[currentClickedObjectIndex]);
					Debug.Log("timelineObjects count: "+timelineObjects.Count);
				}
				else
				{
					Debug.Log("else released object in space "+objectClicked+" bool: "+isPresent+" name: "+figureObjects[currentClickedObjectIndex].name);
					
				}
			}
		}*/
    }
}