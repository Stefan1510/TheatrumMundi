using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightRepresentationController : MonoBehaviour
{
    public Image UiSetting_LB;
    public Image UiSetting_LB_Light;
    public Image UiSetting_LB_side;
    public Image UiSetting_LB_side_Light;
    public Sprite UiSetting_LB_on_Sprite;
    public Sprite UiSetting_LB_off_Sprite;
    public Sprite UiSetting_LB_active_Sprite;
    public Sprite UiSetting_LB_side_on_Sprite;
    public Sprite UiSetting_LB_side_off_Sprite;
    public Sprite UiSetting_LB_side_active_Sprite;
    public Sprite UiSetting_LB_Light_on_Sprite;
    public Sprite UiSetting_LB_Light_off_Sprite;
    public Sprite UiSetting_LB_Light_active_Sprite;
    private bool _isSelected;
    private bool _isActive;
    // Start is called before the first frame update
    void Start()
    {
        UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_off_Sprite;
        UiSetting_LB_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_off_Sprite;
        UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_off_Sprite;
        UiSetting_LB_side_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_off_Sprite;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        ChangeStates();
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        ChangeStates();
    }
    
    void ChangeStates()
    {
        if (_isActive)
        {
            UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_on_Sprite;
            UiSetting_LB_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_on_Sprite;
            UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_on_Sprite;
            UiSetting_LB_side_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_on_Sprite;
            if (_isSelected)
            {
                UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_active_Sprite;
                UiSetting_LB_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_active_Sprite;
                UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_active_Sprite;
                UiSetting_LB_side_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_active_Sprite;
            }
        }
        else
        {
            UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_off_Sprite;
            UiSetting_LB_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_off_Sprite;
            UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_off_Sprite;
            UiSetting_LB_side_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_off_Sprite;
            if (_isSelected)
            {
                UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_active_Sprite;
                UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_active_Sprite;
            }
        }
    }

    public void setPosition_UiSetting_LB_Light(float xPosition)
    {
        UiSetting_LB_Light.rectTransform.localPosition = new Vector3(xPosition * 25, UiSetting_LB_Light.rectTransform.localPosition.y, UiSetting_LB_Light.rectTransform.localPosition.z);
    }

    public void setHeight_UiSetting_LB_side_Light(float yPosition)
    {
        UiSetting_LB_side_Light.rectTransform.localPosition = new Vector3(UiSetting_LB_side_Light.rectTransform.localPosition.x, yPosition * 300, UiSetting_LB_side_Light.rectTransform.localPosition.z);
    }

    void RotateLight()
    {
        UiSetting_LB.GetComponent<Image>().rectTransform.Rotate(0, 0, 45);
    }
}
