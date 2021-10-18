using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectShelf : MonoBehaviour
{
	//objectShelf is the class for the object-browser (buehne,kulisse,licht)
	//global class variables
	public GameObject MenueBuehne;
	public GameObject MenueKulisse;
	public GameObject MenueLicht;
	
	public GameObject LiveView;
	public GameObject SchematicView;
	
	public Text HeadlineBuehne;
	public Text HeadlineKulisse;
	public Text HeadlineLicht;
	
	//private GameObject lvcamera;
	
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("testprint");
		//define the buttons
		Debug.Log("this is the main menue");
		//mytext.text="this is text";
		MenueBuehne.SetActive(false);
		MenueKulisse.SetActive(true);
		MenueLicht.SetActive(false);
		
		LiveView.SetActive(true);
		
		HeadlineBuehne.gameObject.SetActive(true);
		HeadlineKulisse.gameObject.SetActive(false);
		HeadlineLicht.gameObject.SetActive(false);
    }
	
	public void ButtonBuehne()
	{
		Debug.Log("button for buehne");
		//show menue of buehne
		MenueBuehne.SetActive(true);
		MenueKulisse.SetActive(false);
		MenueLicht.SetActive(false);
		//show headline of buehne
		//HeadlineBuehne.text="bababababa";
		//HeadlineBuehne.color=Color.red;
		HeadlineBuehne.gameObject.SetActive(true);
		HeadlineKulisse.gameObject.SetActive(false);
		HeadlineLicht.gameObject.SetActive(false);
		//HeadlineKulisse.show(false);
		//HeadlineLicht.show(false);
		//mytext.text="this is buehne";
		//Debug.Log(mytext);
		//lvcamera=GameObject.Find("LVCam");
		//Debug.Log("lvc");
		//lvcamera.transform.position=new Vector3(8f,4f,5f);
	}
	public void ButtonKulisse()
	{
		Debug.Log("button for kulisse");
		//show menue of buehne
		MenueBuehne.SetActive(false);
		MenueKulisse.SetActive(true);
		MenueLicht.SetActive(false);
		HeadlineBuehne.gameObject.SetActive(false);
		HeadlineKulisse.gameObject.SetActive(true);
		HeadlineLicht.gameObject.SetActive(false);
		//mytext.text="in kulisse";
		//Debug.Log(mytext);
		//show headline of buehne
		//HeadlineBuehne.SetActive(true);
		//HeadlineKulisse.SetActive(false);
		//HeadlineLicht.SetActive(false);
	}
	public void ButtonLicht()
	{
		Debug.Log("button for Licht");
		//show menue of buehne
		MenueBuehne.SetActive(false);
		MenueKulisse.SetActive(false);
		MenueLicht.SetActive(true);
		//show headline of buehne
		HeadlineBuehne.gameObject.SetActive(false);
		HeadlineKulisse.gameObject.SetActive(false);
		HeadlineLicht.gameObject.SetActive(true);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
