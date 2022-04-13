using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoulisseController : MonoBehaviour
{
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManaging.triggerActive != 0)
        {
            if (timer <= 1.0f)
            {
                timer += Time.deltaTime;
            }
            if (timer >= 1.0f) SceneManaging.openUp = true;
        }
        else
        {
            timer = 0.0f;
            SceneManaging.openUp = false;
        }

    }
}
