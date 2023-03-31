using UnityEngine;

public class FigureController : MonoBehaviour
{
    private void Start()
    {
        
    }
    public void removeObject()
    {
        Debug.Log("hi");
        if (gameObject.transform.parent.parent.name[0] == 'F')
        {
            Debug.Log("Object: "+gameObject.transform.parent.parent.parent.parent.name + " soll geloescht werden. Von timeline: "+gameObject.transform.parent.transform.parent.transform.parent.name);
            //gameObject.transform.parent.parent.parent.parent.GetComponent<RailManager>().removeObjectFromTimeline2D(gameObject.transform.parent.transform.parent.gameObject, false);
        }
        else
        {
            gameObject.transform.parent.parent.parent.GetComponent<RailMusicManager>().removeObjectFromTimeline(gameObject.transform.parent.parent.gameObject);
        }
    }
    public void sinnlos()
    {
        Debug.Log("sinnos");
    }
}
