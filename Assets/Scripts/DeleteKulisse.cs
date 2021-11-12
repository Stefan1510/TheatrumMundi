using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteKulisse : MonoBehaviour
{
    bool showDeleteButton;
    //public DragDrop activeKulisse;
    private void Awake()
    {
        showDeleteButton = false;
        if(gameObject.name == "DeleteButton")
        {
            gameObject.SetActive(false);
        }
    }

    /*public void OnClick()
    {
        if(gameObject.name != "DeleteButton") 
        {
            //activeKulisse = this.transform.parent.gameObject.GetComponent<DragDrop>();
            if (GetComponent<DragDrop>().schieneKulisse != 0)
            {
                if (showDeleteButton)
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    showDeleteButton = false;
                }
                else if (showDeleteButton == false)
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    showDeleteButton = true;
                }
            }
            else 
            {
                showDeleteButton = false;
                //GetComponent<DragDrop>().activeKulisse = null;
            }
        }
        else if(gameObject.name == "DeleteButton")
        {
            SceneManager.deleteButtonPressed = true;
            
            //gameObject.transform.GetComponentInParent<DragDrop>().schieneKulisse = 0;
            //gameObject.transform.GetComponentInParent<DragDrop>().GetComponent<RectTransform>().anchoredPosition = gameObject.transform.GetComponentInParent<DragDrop>().pos;
        }
        else SceneManager.deleteButtonPressed = false;
    }*/
}
