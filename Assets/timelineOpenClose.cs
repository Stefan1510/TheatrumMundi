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
	int currentClickedObjectIndex;
	
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
		figureObjects=new GameObject[objectLibrary.transform.childCount]; //instead of a count like "3"
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
		Debug.Log("object width,height: "+objectDefaultSize);
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
		Debug.Log(">>>figure elements existing: "+figure1.name);
		fig1StartPos=-1.75f;
		fig2StartPos=1.88f;
		Vector3 rail1StartPos=new Vector3(0.0f,-0.009f,-2.0f);
		Vector3 rail2StartPos=new Vector3(-0.119f,0.146f,-2.0f);
		Vector3 rail3StartPos=new Vector3(-0.319f,0.146f,-2.0f);
		Vector3 rail4StartPos=new Vector3(-0.439f,0.146f,-2.0f);
		Vector3 rail5StartPos=new Vector3(-0.642f,0.146f,-2.0f);
		Vector3 rail6StartPos=new Vector3(-0.759f,0.146f,-2.0f);
	
    }

	public string getClickedObject()
	{
		string objName="";
		
		Vector3 mousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePos2D=new Vector2(mousePos.x,mousePos.y);
		//Debug.Log("mousepos of ray: "+mousePos);
		RaycastHit2D myHit = Physics2D.Raycast(mousePos2D,Vector2.zero);
		if(myHit.collider!=null)
		{
			objName=myHit.collider.gameObject.name;
		}
		else
		{
			objName="no object found!";
		}
		//if the ray-way fails
		for(int i=0;i<figureObjects.Length;i++)
		{
			//save last correct index
			int oldClickedObject=currentClickedObjectIndex;
			//Debug.Log("idx of old element: "+oldClickedObject);
			//which object in grid layout is clicked
			if(figureObjects[i].GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(Input.mousePosition))
			{
				//save the index of clicked object
				currentClickedObjectIndex=i;
				//Debug.Log("idx of element: "+currentClickedObjectIndex);
				//if you click an new object
				if (oldClickedObject!=currentClickedObjectIndex)
				{
					//set objectSceneSize-Variables back to default
					objectDefaultPosition=figureObjects[currentClickedObjectIndex].transform.position;
					//objectDefaultSize.x=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.width;
					//objectDefaultSize.y=figureObjects[currentClickedObjectIndex].GetComponent<RectTransform>().rect.height;
					objectSceneSize.x=objectDefaultSize.x;
					objectSceneSize.y=objectDefaultSize.y;
					objectOnTimeline=false;
				}
				return figureObjects[i].GetComponent<BoxCollider2D>().name;
			}
			//error-handling if no clicked object found
			if (currentClickedObjectIndex==-1)
				return "no object found! [-1]";
		}
		
		return objName;
	}
    // Update is called once per frame
    void Update()
    {
        Vector2 getMousePos=Input.mousePosition;
		
		if (Input.GetMouseButton(0))	//left mouse button is held down
		{
			//Debug.Log("mouse btn is held down");
			//identifiy which gameobject do you clicked
			
		}
		if (Input.GetMouseButtonDown(0)) //left mouse button clicked down
		{
			//identifiy which gameobject do you clicked
			Debug.Log("you clicked object: "+getClickedObject());
			//Debug.Log("++++++left mouse button down");
			//Debug.Log("++++++xpos mouse : "+getMousePos);
			
			//if you touch the timeline with the mouse
			if (this.GetComponent<BoxCollider2D>()==Physics2D.OverlapPoint(getMousePos))
			{
				movingOnTimeline=true;
				clickingTimeline=true;
				Debug.Log("--->>>>click the timeline mousepos: "+getMousePos.x +";"+getMousePos.y+" (collider: "+this.GetComponent<BoxCollider2D>()+";"+this.GetComponent<BoxCollider2D>()+")");
				Debug.Log("curr timeline collider-size: "+timelineImage.GetComponent<BoxCollider2D>().size);
				timelineOpen=true;
			}
			else
			{
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
				}
			}
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
    }
}
