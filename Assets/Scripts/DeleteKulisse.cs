using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteKulisse : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.showSettings = false;
        if(gameObject.name == "DeleteButton")
        {
            gameObject.SetActive(false);
        }
    }

    
}
