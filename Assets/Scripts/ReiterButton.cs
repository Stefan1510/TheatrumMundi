using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReiterButton : MonoBehaviour
{

    public void OnClick()
    {
        DragDrop dragdrop = SceneManager.dragDrop;

        for(int i=0 ; i<this.transform.childCount ; i++)
        {
            dragdrop.setElementInactive(this.transform.GetChild(i).GetComponent<DragDrop>());
        }

        if (dragdrop == null)
        {
            return;
        }

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

    public void OnPointerEnter()
    {
        SceneManager.mouse_over = true;
        //Debug.Log("entering "+gameObject);
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
        //Debug.Log("triggerACtive: " + SceneManager.triggerActive);
        //SceneManager.dragDrop.setReiterActive(SceneManager.dragDrop.statusReiter);
    }

    public void OnPointerExit()
    {
        if (gameObject.name == "Einstellungen")
        {
            SceneManager.triggerEinstellungen = false;
        }
        else
        {
            SceneManager.triggerActive = 0;
            //Debug.Log("TriggerActive = 0");
        }

    }

}