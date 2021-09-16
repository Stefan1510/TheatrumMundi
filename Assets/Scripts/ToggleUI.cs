using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    private GameObject _mainCamera;

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

        _mainCamera = GameObject.Find("Main Camera");
        if (gameObject.name == "PanelMeasures" && gameObject.activeSelf)
        {
            Debug.Log("changecam");
            _mainCamera.transform.position = new Vector3(8f, 4f, 5f);
            _mainCamera.transform.rotation = Quaternion.Euler(18f, -120f, 0f);
        }
        else
        {
            _mainCamera.transform.position = new Vector3(5f, 1.6f, 0f);
            _mainCamera.transform.rotation = Quaternion.Euler(8.5f, -90f, 0f);
            Debug.Log("standardcam");
        }
        
    }

}
