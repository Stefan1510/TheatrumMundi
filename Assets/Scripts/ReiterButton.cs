using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReiterButton : MonoBehaviour
{
    public List<DragDrop> kulissen;

    private void Start()
    {
        kulissen = new List<DragDrop>();
    }

    public void OnTriggerEnter2D(Collider2D collider) 
    {
        Debug.Log("entering"+gameObject);
        //add kulisse an liste! SceneManager.dragDrop.setReiterActive()
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        Debug.Log("exiting");//remove kulisse von liste
    }
    public void RemoveKulisse (DragDrop kulisse)
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

        foreach(DragDrop kulisse in kulissen)
        {
            kulisse.gameObject.SetActive(false);
        }
    }

    public void Show() 
    {
        //Debug.Log(gameObject.name);
        gameObject.SetActive(true);

        foreach(DragDrop kulisse in kulissen)
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
    }
}
