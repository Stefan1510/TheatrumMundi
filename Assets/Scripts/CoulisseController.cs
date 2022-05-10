using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoulisseController : MonoBehaviour
{
    private bool setBack = false;
    public float timer;
    private int currentTab;

    // Start is called before the first frame update
    void Start()
    {
        currentTab = 0;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManaging.objectInIndexTab != -1)
        {
            if (setBack)
            {
                timer = 0.0f;
                SceneManaging.openUp = false;
                setBack = false;
            }
            // Debug.Log("openUp: "+SceneManaging.openUp);
            if (currentTab != SceneManaging.objectInIndexTab)
            {
                timer = 0.0f;
                // Debug.Log("Timer neu!!!!");
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
