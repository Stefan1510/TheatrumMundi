using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{


    public void togglePanel()
    {
        if (!gameObject.activeSelf)
        {
            foreach(GameObject panel in GameObject.FindGameObjectsWithTag("UIPanel"))
            {
                panel.SetActive(false);
            }
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }

}
