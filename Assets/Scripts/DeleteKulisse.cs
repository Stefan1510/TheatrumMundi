using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteKulisse : MonoBehaviour
{
    bool showDeleteButton;

    private void Awake()
    {
        showDeleteButton = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void OnClick()
    {
        if (GetComponent<DragDrop>().schieneKulisse != 0)
        {
            if (showDeleteButton)
            {
                Debug.Log("hier kommt das x");
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                showDeleteButton = false;
            }
            else if (showDeleteButton == false)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                showDeleteButton = true;
            }
        }
        if(gameObject.name == "DeleteButton")
        {
            //Kulisse removen, schienekulisse =0
            gameObject.transform.GetComponentInParent<DragDrop>().schieneKulisse = 0;
            gameObject.transform.GetComponentInParent<DragDrop>().GetComponent<RectTransform>().anchoredPosition = gameObject.transform.GetComponentInParent<DragDrop>().pos;
        }
    }
}
