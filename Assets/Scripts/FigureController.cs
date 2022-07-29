using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void removeObject()
    {
        if (gameObject.transform.parent.parent.name[0] == 'F')
        {
            //Debug.Log("Object: "+gameObject.transform.parent.transform.parent.name + " soll geloescht werden. Von timeline: "+gameObject.transform.parent.transform.parent.transform.parent.name);
            gameObject.transform.parent.transform.parent.transform.parent.GetComponent<RailManager>().removeObjectFromTimeline2D(gameObject.transform.parent.transform.parent.gameObject);
        }
        else
        {
            gameObject.transform.parent.parent.parent.GetComponent<RailMusicManager>().removeObjectFromTimeline(gameObject.transform.parent.parent.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
