using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReiterButton : MonoBehaviour
{
    public GameObject gameController;
    public void OnClick()
    {
        DragDrop dragdrop = SceneManaging.dragDrop;

        // set everything inactive
        for (int j = 0; j < gameController.GetComponent<UIController>().goIndexTabs.Length; j++)
        {
            for (int i = 0; i < gameController.GetComponent<UIController>().goIndexTabs[j].transform.childCount; i++)
            {
                dragdrop.setElementInactive(gameController.GetComponent<UIController>().goIndexTabs[j].transform.GetChild(i).GetComponent<DragDrop>());
            }
        }
        StaticSceneData.Sceneries3D();

        if (dragdrop == null)
        {
            return;
        }

        if (gameObject.name == "RailButton1")
        {
            SceneManaging.statusReiter = 1;
            dragdrop.setReiterActive(1);
        }
        else if (gameObject.name == "RailButton2")
        {
            SceneManaging.statusReiter = 2;
            dragdrop.setReiterActive(2);
        }
        else if (gameObject.name == "RailButton3")
        {
            SceneManaging.statusReiter = 3;
            dragdrop.setReiterActive(3);
        }
        else if (gameObject.name == "RailButton4")
        {
            dragdrop.setReiterActive(4);
            SceneManaging.statusReiter = 4;
        }
        else if (gameObject.name == "RailButton5")
        {
            dragdrop.setReiterActive(5);
            SceneManaging.statusReiter = 5;
        }
        else if (gameObject.name == "RailButton6")
        {
            dragdrop.setReiterActive(6);
            SceneManaging.statusReiter = 6;
        }
        else if (gameObject.name == "RailButton7")
        {
            dragdrop.setReiterActive(7);
            SceneManaging.statusReiter = 7;
        }
        else if (gameObject.name == "RailButton8")
        {
            dragdrop.setReiterActive(8);
            SceneManaging.statusReiter = 8;
        }

    }

    public void OnPointerEnter()
    {
        //SceneManaging.mouse_over = true;
        //Debug.Log("entering "+gameObject);
        if (gameObject.name == "RailButton1") SceneManaging.triggerActive = 1;
        else if (gameObject.name == "RailButton2") SceneManaging.triggerActive = 2;
        else if (gameObject.name == "RailButton3") SceneManaging.triggerActive = 3;
        else if (gameObject.name == "RailButton4") SceneManaging.triggerActive = 4;
        else if (gameObject.name == "RailButton5") SceneManaging.triggerActive = 5;
        else if (gameObject.name == "RailButton6") SceneManaging.triggerActive = 6;
        else if (gameObject.name == "RailButton7") SceneManaging.triggerActive = 7;
        else if (gameObject.name == "RailButton8") SceneManaging.triggerActive = 8;
        else if (gameObject.name == "SettingsWindow") SceneManaging.triggerEinstellungen = true;
        else SceneManaging.triggerActive = 0;
        //Debug.Log("triggerACtive: " + SceneManaging.triggerActive+", triggerEinstellung: "+SceneManaging.triggerEinstellungen);
        //SceneManaging.dragDrop.setReiterActive(SceneManaging.dragDrop.statusReiter);
    }

    public void OnPointerExit()
    {
        if (gameObject.name == "SettingsWindow")
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