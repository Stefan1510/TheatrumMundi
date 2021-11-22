using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteKulisse : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.showDeleteButton = false;
        if(gameObject.name == "DeleteButton")
        {
            gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if(gameObject.name != "DeleteButton") 
        {
            //activeKulisse = this.transform.parent.gameObject.GetComponent<DragDrop>();
            if (this.GetComponent<DragDrop>().schieneKulisse != 0)
            {
                if (SceneManager.showDeleteButton)
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    SceneManager.showDeleteButton = false;
                }
                else if (SceneManager.showDeleteButton == false)
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    SceneManager.showDeleteButton = true;
                    GetComponent<DragDrop>().menuExtra.SetActive(true);
                }
            }
            else 
            {
                SceneManager.showDeleteButton = false;
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
    }
}
