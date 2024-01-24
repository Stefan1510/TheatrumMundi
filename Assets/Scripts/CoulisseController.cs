using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoulisseController : MonoBehaviour
{
    private bool setBack = false;
    public float timer;
    private int currentTab;
    void Start()
    {
        currentTab = 0;
        timer = 0.0f;
    }
    void FixedUpdate()
    {
        if (SceneManaging.objectInIndexTab != -1)
        {
            if (setBack)
            {
                timer = 0.0f;
                SceneManaging.openUp = false;
                setBack = false;
            }
            if (currentTab != SceneManaging.objectInIndexTab)
            {
                timer = 0.0f;
                currentTab = SceneManaging.objectInIndexTab;
            }
            else if (timer <= 1.0f)
            {
                timer += Time.deltaTime;
            }
            else if (timer > 1.0f)
            {
                setBack = true;
                SceneManaging.openUp = true;
            }
        }
        else
        {
            timer = 0.0f;
            SceneManaging.openUp = false;
        }
    }
}
