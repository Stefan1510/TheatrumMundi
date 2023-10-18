using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTJ.FrameCapturer;

public class RenderFilm : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    public void ClickOkay()
    {
        playerControls.ButtonRender(true);
        gameObject.SetActive(false);
    }
    public void ClickCancel()
    {
        gameObject.SetActive(false);
    }
    public void setActive()
    {
        gameObject.SetActive(true);
    }
}
