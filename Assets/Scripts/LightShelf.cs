using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightShelf : MonoBehaviour
{
    [SerializeField] private GameObject _panelElementToDeactivate;
    [SerializeField] private Graphic _graphicToggleLbBackground;
    
    private float _startHeight;
    public Image UiSetting_LB_Image;
    // Start is called before the first frame update
    void Start()
    {
        ToggleLightBlock(false);
        GetComponent<Toggle>().isOn = false;
    }
    public void ToggleLightBlock(bool toggleOnOff)
    {
        _panelElementToDeactivate.SetActive(toggleOnOff);
        _graphicToggleLbBackground.enabled = !toggleOnOff;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_panelElementToDeactivate.transform.parent.GetComponentInParent<RectTransform>());
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().SetSelected(toggleOnOff);
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().ToggleLightCone(toggleOnOff);
    }
}
