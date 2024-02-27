using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTJ.FrameCapturer;

public class RenderFilm : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    public void ClickOkay()
    {
        gameObject.SetActive(false);
        playerControls.ButtonRender(true);
    }
    public void ClickCancel()
    {
        gameObject.SetActive(false);
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
