using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySampleMusic : MonoBehaviour
{
    public GameObject railMusic;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playSample()
    {
        int i = int.Parse(gameObject.name.Substring(6));
        railMusic.GetComponent<RailMusicManager>().PlaySample(i);
    }
}
