using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReiterActiveButton : MonoBehaviour
{
    public List<DragDrop> kulissen;

    private void Start()
    {
        kulissen = new List<DragDrop>();
        /*foreach (DragDrop kulisse in kulissen)
        {
            kulisse.setElementInactive(kulisse);
        }*/
    }

    // Update is called once per frame
    public void RemoveKulisse(DragDrop kulisse)
    {
        kulissen.Remove(kulisse);
    }

    public void AddKulisse(DragDrop kulisse)
    {
        kulissen.Add(kulisse);
        //Debug.Log("Kulisse: "+kulisse+" added."+kulissen.Count);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        foreach (DragDrop kulisse in kulissen)
        {
            kulisse.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        //Debug.Log(gameObject.name);
        gameObject.SetActive(true);

        foreach (DragDrop kulisse in kulissen)
        {
            kulisse.gameObject.SetActive(true);

        }
    }
}
