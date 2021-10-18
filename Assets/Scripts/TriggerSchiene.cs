using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSchiene : MonoBehaviour
{
    public bool schieneActive = false;

    void OnTriggerEnter2D(Collider2D collider) {
        schieneActive = true;
        //Debug.Log("Schiene: "+ schieneActive);
    }

    void OnTriggerExit2D(Collider2D collider) {
        schieneActive = false;
        //Debug.Log("Schiene: "+ schieneActive);
    }
}
