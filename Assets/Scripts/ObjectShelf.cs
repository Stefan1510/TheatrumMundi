using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectShelf : MonoBehaviour
{
	//objectShelf is the class for the object-browser (buehne,kulisse,licht)
	//global class variables
	public GameObject MenueShelf01;
	public GameObject MenueShelf02;
	public GameObject MenueShelf03;
	public GameObject MenueShelf04;
	
	public GameObject LiveView;
	public GameObject SchematicView;
	
	public Text HeadlineShelf01;
	public Text HeadlineShelf02;
	public Text HeadlineShelf03;
	public Text HeadlineShelf04;
	
	//private GameObject lvcamera;
	
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("testprint");
		//define the buttons
		//Debug.Log("this is the main menue");
		//mytext.text="this is text";
		MenueShelf01.SetActive(true);
		MenueShelf02.SetActive(false);
		MenueShelf03.SetActive(false);
		MenueShelf04.SetActive(false);
		
		LiveView.SetActive(true);
		
		HeadlineShelf01.gameObject.SetActive(true);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(false);
		HeadlineShelf04.gameObject.SetActive(false);
    }
	
	public void ButtonShelf01()
	{
		Debug.Log("button for shelf01");
		//show menue of buehne
		MenueShelf01.SetActive(true);
		MenueShelf02.SetActive(false);
		MenueShelf03.SetActive(false);
		MenueShelf04.SetActive(false);
		//show headline of buehne
		//HeadlineBuehne.text="bababababa";
		//HeadlineBuehne.color=Color.red;
		HeadlineShelf01.gameObject.SetActive(true);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(false);
		HeadlineShelf04.gameObject.SetActive(false);
		//HeadlineKulisse.show(false);
		//HeadlineLicht.show(false);
		//mytext.text="this is buehne";
		//Debug.Log(mytext);
		//lvcamera=GameObject.Find("LVCam");
		//Debug.Log("lvc");
		//lvcamera.transform.position=new Vector3(8f,4f,5f);
		StaticSceneData.everything3D();
	}
	public void ButtonShelf02()
	{
		Debug.Log("button for shelf02");
		//show menue of buehne
		MenueShelf01.SetActive(false);
		MenueShelf02.SetActive(true);
		MenueShelf03.SetActive(false);
		MenueShelf04.SetActive(false);
		HeadlineShelf01.gameObject.SetActive(false);
		HeadlineShelf02.gameObject.SetActive(true);
		HeadlineShelf03.gameObject.SetActive(false);
		HeadlineShelf04.gameObject.SetActive(false);
		//mytext.text="in kulisse";
		//Debug.Log(mytext);
		//show headline of buehne
		//HeadlineBuehne.SetActive(true);
		//HeadlineKulisse.SetActive(false);
		//HeadlineLicht.SetActive(false);
		StaticSceneData.everything3D();
	}
	public void ButtonShelf03()
	{
		Debug.Log("button for shelf03");
		//show menue of buehne
		MenueShelf01.SetActive(false);
		MenueShelf02.SetActive(false);
		MenueShelf03.SetActive(true);
		MenueShelf04.SetActive(false);
		//show headline of buehne
		HeadlineShelf01.gameObject.SetActive(false);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(true);
		HeadlineShelf04.gameObject.SetActive(false);
		StaticSceneData.everything3D();
	}
	
	public void ButtonShelf04()
	{
		Debug.Log("button for shelf04");
		//show menue of buehne
		MenueShelf01.SetActive(false);
		MenueShelf02.SetActive(false);
		MenueShelf03.SetActive(false);
		MenueShelf04.SetActive(true);
		//show headline of buehne
		HeadlineShelf01.gameObject.SetActive(false);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(false);
		HeadlineShelf04.gameObject.SetActive(true);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
