using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

	public GameObject MenueButton01;
	public GameObject MenueButton02;
	public GameObject MenueButton03;
	public GameObject MenueButton04;

	//private GameObject lvcamera;

	// Start is called before the first frame update
	void Start()
    {
        //Debug.Log("testprint");
        //define the buttons
        //Debug.Log("this is the main menue");
        //mytext.text="this is text";

        // MenueShelf01.SetActive(true);
        // MenueShelf02.SetActive(false);
        // MenueShelf03.SetActive(false);
        // MenueShelf04.SetActive(false);

        // LiveView.SetActive(true);

        // HeadlineShelf01.gameObject.SetActive(true);
        // HeadlineShelf02.gameObject.SetActive(false);
        // HeadlineShelf03.gameObject.SetActive(false);
        // HeadlineShelf04.gameObject.SetActive(false);

        // MenueButton01.SetActive(false);
        // MenueButton02.SetActive(true);
        // MenueButton03.SetActive(true);
        // MenueButton04.SetActive(true);
        MenueShelf01.SetActive(true);
        MenueShelf02.SetActive(false);
        MenueShelf03.SetActive(false);
        try
        {
            MenueShelf04.SetActive(false);
        }
        catch (Exception ex)
        {
            if (ex is NullReferenceException || ex is UnassignedReferenceException)
            {
                return;
            }
            throw;
		}

		HeadlineShelf01.gameObject.SetActive(true);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(false);
		try
		{
			HeadlineShelf04.gameObject.SetActive(false);
		}
		catch (Exception ex)
		{
			if (ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}

		MenueButton01.SetActive(false);
		MenueButton02.SetActive(true);
		MenueButton03.SetActive(true);
		try
		{
			MenueButton04.SetActive(true);
		}
		catch (Exception ex)
		{
			if (ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
	}

    public void ButtonShelf01()
	{
		Debug.Log("button for shelf01");
		//show menue of buehne
		MenueShelf01.SetActive(true);
		MenueShelf02.SetActive(false);
		MenueShelf03.SetActive(false);
		try {
			MenueShelf04.SetActive(false);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
		
		//show headline of buehne
		//HeadlineBuehne.text="bababababa";
		//HeadlineBuehne.color=Color.red;
		HeadlineShelf01.gameObject.SetActive(true);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(false);
		try {
			HeadlineShelf04.gameObject.SetActive(false);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
		//HeadlineKulisse.show(false);
		//HeadlineLicht.show(false);
		//mytext.text="this is buehne";
		//Debug.Log(mytext);
		//lvcamera=GameObject.Find("LVCam");
		//Debug.Log("lvc");
		//lvcamera.transform.position=new Vector3(8f,4f,5f);

		MenueButton01.SetActive(false);
		MenueButton02.SetActive(true);
		MenueButton03.SetActive(true);
		try {
			MenueButton04.SetActive(true);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}

		StaticSceneData.Everything3D();
	}
	public void ButtonShelf02()
	{
		Debug.Log("button for shelf02");
		//show menue of buehne
		MenueShelf01.SetActive(false);
		MenueShelf02.SetActive(true);
		MenueShelf03.SetActive(false);
		try {
			MenueShelf04.SetActive(false);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
		HeadlineShelf01.gameObject.SetActive(false);
		HeadlineShelf02.gameObject.SetActive(true);
		HeadlineShelf03.gameObject.SetActive(false);
		try {
			HeadlineShelf04.gameObject.SetActive(false);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
		//mytext.text="in kulisse";
		//Debug.Log(mytext);
		//show headline of buehne
		//HeadlineBuehne.SetActive(true);
		//HeadlineKulisse.SetActive(false);
		//HeadlineLicht.SetActive(false););

		MenueButton01.SetActive(true);
		MenueButton02.SetActive(false);
		MenueButton03.SetActive(true);
		try {
			MenueButton04.SetActive(true);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}

		StaticSceneData.Everything3D();
	}
	public void ButtonShelf03()
	{
		Debug.Log("button for shelf03");
		//show menue of buehne
		MenueShelf01.SetActive(false);
		MenueShelf02.SetActive(false);
		MenueShelf03.SetActive(true);
		try {
			MenueShelf04.SetActive(false);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
		//show headline of buehne
		HeadlineShelf01.gameObject.SetActive(false);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(true);
		try {
			HeadlineShelf04.gameObject.SetActive(false);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}

		MenueButton01.SetActive(true);
		MenueButton02.SetActive(true);
		MenueButton03.SetActive(false);
		try {
			MenueButton04.SetActive(true);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}

		StaticSceneData.Everything3D();
	}
	
	public void ButtonShelf04()
	{
		Debug.Log("button for shelf04");
		//show menue of buehne
		MenueShelf01.SetActive(false);
		MenueShelf02.SetActive(false);
		MenueShelf03.SetActive(false);
		try {
			MenueShelf04.SetActive(true);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
		//show headline of buehne
		HeadlineShelf01.gameObject.SetActive(false);
		HeadlineShelf02.gameObject.SetActive(false);
		HeadlineShelf03.gameObject.SetActive(false);
		try {
			HeadlineShelf04.gameObject.SetActive(true);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}

		MenueButton01.SetActive(true);
		MenueButton02.SetActive(true);
		MenueButton03.SetActive(true);
        try {
			MenueButton04.SetActive(false);
		} catch (Exception ex) 
		{
			if(ex is NullReferenceException || ex is UnassignedReferenceException)
			{
				return;
			}
			throw;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
