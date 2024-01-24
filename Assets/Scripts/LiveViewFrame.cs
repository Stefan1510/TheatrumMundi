using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveViewFrame : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;

    void Start()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Switch1"))
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
        }
        if (Input.GetButtonDown("Switch2"))
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
        }
    }
}
