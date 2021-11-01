using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReiterButton : MonoBehaviour
{
    public List<DragDrop> kulissen;
    //private bool mouse_over = false;
    //float currentTime;

    private void Start()
    {
        kulissen = new List<DragDrop>();
        //currentTime = 0f;
    }

    /*void Update()
    {
        if (mouse_over)
        {
            if(currentTime > .5f)
            {
                
            }
            //Debug.Log("currentTime: "+currentTime+", Time.deltaTime: "+ Time.deltaTime+", MouseOver: "+mouse_over);
            else if(currentTime <= .5f) 
            {
                currentTime += Time.deltaTime;
                Debug.Log("currentTime: "+currentTime +", Time.deltatime: "+Time.deltaTime);
            }
            else {}
            
         }
     }*/

    public void OnPointerEnter()
    {
        SceneManager.mouse_over = true;

        if (gameObject.name == "Reiter1") SceneManager.triggerActive = 1;
        else if (gameObject.name == "Reiter2") SceneManager.triggerActive = 2;
        else if (gameObject.name == "Reiter3") SceneManager.triggerActive = 3;
        else if (gameObject.name == "Reiter4") SceneManager.triggerActive = 4;
        else if (gameObject.name == "Reiter5") SceneManager.triggerActive = 5;
        else if (gameObject.name == "Reiter6") SceneManager.triggerActive = 6;
        else if (gameObject.name == "Reiter7") SceneManager.triggerActive = 7;
        else if (gameObject.name == "Reiter8") SceneManager.triggerActive = 8;
        else if (gameObject.name == "Einstellungen") SceneManager.triggerEinstellungen = true;
        else SceneManager.triggerActive = 0;
        //Debug.Log("entering "+gameObject+", triggerActive: "+SceneManager.triggerActive);
        //SceneManager.dragDrop.setReiterActive(SceneManager.dragDrop.statusReiter);
        //add kulisse an liste! SceneManager.dragDrop.setReiterActive()
    }

    public void OnPointerExit()
    {
        //Debug.Log("exiting: "+gameObject.name);
        SceneManager.mouse_over = false;
        //currentTime = 0f;
        //if(SceneManager.triggerEinstellungen == true) {}
        if (gameObject.name == "Einstellungen")
        {
            SceneManager.triggerEinstellungen = false;
        }
        else
        {
            SceneManager.triggerActive = 0;

        }


    }
    public void RemoveKulisse(DragDrop kulisse)
    {
        kulissen.Remove(kulisse);
    }

    public void AddKulisse(DragDrop kulisse)
    {
        kulissen.Add(kulisse);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        foreach (DragDrop kulisse in kulissen)
        {
            kulisse.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        //Debug.Log(gameObject.name);
        gameObject.SetActive(true);

        foreach (DragDrop kulisse in kulissen)
        {
            kulisse.gameObject.SetActive(true);
        }
    }
    public void OnClick()
    {
        DragDrop dragdrop = SceneManager.dragDrop;

        /*if (dragdrop == null)
        {
            return;
        }*/

        if (gameObject.name == "Reiter1")
        {
            dragdrop.setReiterActive(1);
        }
        else if (gameObject.name == "Reiter2")
        {
            dragdrop.setReiterActive(2);
        }
        else if (gameObject.name == "Reiter3")
        {
            dragdrop.setReiterActive(3);
        }
        else if (gameObject.name == "Reiter4")
        {
            dragdrop.setReiterActive(4);
        }
        else if (gameObject.name == "Reiter5")
        {
            dragdrop.setReiterActive(5);
        }
        else if (gameObject.name == "Reiter6")
        {
            dragdrop.setReiterActive(6);
        }
        else if (gameObject.name == "Reiter7")
        {
            dragdrop.setReiterActive(7);
        }
        else if (gameObject.name == "Reiter8")
        {
            dragdrop.setReiterActive(8);
        }
    }
}
