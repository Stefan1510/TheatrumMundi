using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReiterButton : MonoBehaviour
{
    public void OnClick()
    {
        DragDrop dragdrop = SceneManaging.dragDrop;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            dragdrop.setElementInactive(this.transform.GetChild(i).GetComponent<DragDrop>());
        }

        if (dragdrop == null)
        {
            return;
        }

        if (gameObject.name == "Reiter1")
        {
            SceneManaging.statusReiter = 1;
            dragdrop.setReiterActive(1);
        }
        else if (gameObject.name == "Reiter2")
        {
            SceneManaging.statusReiter = 2;
            dragdrop.setReiterActive(2);
        }
        else if (gameObject.name == "Reiter3")
        {
            SceneManaging.statusReiter = 3;
            dragdrop.setReiterActive(3);
        }
        else if (gameObject.name == "Reiter4")
        {
            dragdrop.setReiterActive(4);
            SceneManaging.statusReiter = 4;
        }
        else if (gameObject.name == "Reiter5")
        {
            dragdrop.setReiterActive(5);
            SceneManaging.statusReiter = 5;
        }
        else if (gameObject.name == "Reiter6")
        {
            dragdrop.setReiterActive(6);
            SceneManaging.statusReiter = 6;
        }
        else if (gameObject.name == "Reiter7")
        {
            dragdrop.setReiterActive(7);
            SceneManaging.statusReiter = 7;
        }
        else if (gameObject.name == "Reiter8")
        {
            dragdrop.setReiterActive(8);
            SceneManaging.statusReiter = 8;
        }
    }

    public void OnPointerEnter()
    {
        //SceneManaging.mouse_over = true;
        //Debug.Log("entering "+gameObject);
        if (gameObject.name == "Reiter1") SceneManaging.triggerActive = 1;
        else if (gameObject.name == "Reiter2") SceneManaging.triggerActive = 2;
        else if (gameObject.name == "Reiter3") SceneManaging.triggerActive = 3;
        else if (gameObject.name == "Reiter4") SceneManaging.triggerActive = 4;
        else if (gameObject.name == "Reiter5") SceneManaging.triggerActive = 5;
        else if (gameObject.name == "Reiter6") SceneManaging.triggerActive = 6;
        else if (gameObject.name == "Reiter7") SceneManaging.triggerActive = 7;
        else if (gameObject.name == "Reiter8") SceneManaging.triggerActive = 8;
        else if (gameObject.name == "Einstellungen") SceneManaging.triggerEinstellungen = true;
        else SceneManaging.triggerActive = 0;
        //Debug.Log("triggerACtive: " + SceneManaging.triggerActive+", triggerEinstellung: "+SceneManaging.triggerEinstellungen);
        //SceneManaging.dragDrop.setReiterActive(SceneManaging.dragDrop.statusReiter);
    }

    public void OnPointerExit()
    {
        if (gameObject.name == "Einstellungen")
        {
            SceneManaging.triggerEinstellungen = false;
        }
        else
        {
            SceneManaging.triggerActive = 0;
            //Debug.Log("TriggerActive = 0");
        }

    }

}