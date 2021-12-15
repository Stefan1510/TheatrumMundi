using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightShelf : MonoBehaviour
{
    [SerializeField] private GameObject _panelElementToCollapse;
    [SerializeField] private GameObject _panelElementToDeactivate;
    [SerializeField] private Graphic _graphicToggleLbBackground;
    [SerializeField] float _collapsedHeight = 80f;
    private float _startHeight;
    // Start is called before the first frame update
    void Start()
    {
        _startHeight = _panelElementToCollapse.GetComponent<RectTransform>().rect.height;
        ToggleLightBlock(false);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void ToggleLightBlock(bool toggleOnOff)
    {
        RectTransform rt = _panelElementToCollapse.GetComponent<RectTransform>();
        _panelElementToDeactivate.SetActive(toggleOnOff);
        _graphicToggleLbBackground.enabled = !toggleOnOff;
        if (toggleOnOff)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _startHeight);
        }
        else
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _collapsedHeight);
        }
    }
}
