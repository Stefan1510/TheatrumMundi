using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSchiene : MonoBehaviour
{
    public bool schieneActive = false;

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.name == "SchieneBild") {
            schieneActive = true;
            //Debug.Log("Schiene: "+ schieneActive);
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if(collider.name == "SchieneBild") {
            schieneActive = false;
            //Debug.Log("Schiene: "+ schieneActive);
        }
    }
}
