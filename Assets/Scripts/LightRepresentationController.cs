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

    public GameObject UiSetting_LB_Light_angle;
    public Image UiSetting_LB_Light_angle_left;
    public Image UiSetting_LB_Light_angle_fill;
    public Image UiSetting_LB_Light_angle_right;
    public GameObject UiSetting_LB_side_Light_angle;
    public Image UiSetting_LB_side_Light_angle_top;
    public Image UiSetting_LB_side_Light_angle_fill;
    public Image UiSetting_LB_side_Light_angle_down;

    public Text UiSetting_LB_Text;
    public Text UiSetting_LB_side_Text;
    
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
        if (selected)
        {
            UiSetting_LB_Text.color = new Color(1f, 237f / 255f, 157f / 255f);
            UiSetting_LB_side_Text.color = new Color(1f, 237f / 255f, 157f / 255f);
        }
        else
        {
            UiSetting_LB_Text.color = Color.black;
            UiSetting_LB_side_Text.color = Color.black;
        }
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
            UiSetting_LB_side.transform.SetAsFirstSibling();
            if (_isSelected)
            {
                UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_active_Sprite;
                UiSetting_LB_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_active_Sprite;
                UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_active_Sprite;
                UiSetting_LB_side_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_active_Sprite;
                UiSetting_LB_side.transform.SetAsLastSibling();
            }
        }
        else
        {
            UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_off_Sprite;
            UiSetting_LB_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_off_Sprite;
            UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_off_Sprite;
            UiSetting_LB_side_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_off_Sprite;
            UiSetting_LB_side.transform.SetAsFirstSibling();
            if (_isSelected)
            {
                UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_active_Sprite;
                UiSetting_LB_side.GetComponent<Image>().sprite = UiSetting_LB_side_active_Sprite;
                UiSetting_LB_side.transform.SetAsLastSibling();
            }
        }
    }

    public void setPosition_UiSetting_LB_Light(float xPosition)
    {
        UiSetting_LB_Light.rectTransform.localPosition = new Vector3(xPosition * 25, UiSetting_LB_Light.rectTransform.localPosition.y, UiSetting_LB_Light.rectTransform.localPosition.z);
        UiSetting_LB_Light_angle.transform.localPosition = new Vector3(xPosition * 25, UiSetting_LB_Light.rectTransform.localPosition.y, UiSetting_LB_Light.rectTransform.localPosition.z);
        UiSetting_LB_side.transform.SetAsLastSibling();
    }

    public void setHeight_UiSetting_LB_side_Light(float yPosition)
    {
        UiSetting_LB_side_Light.rectTransform.localPosition = new Vector3(UiSetting_LB_side_Light.rectTransform.localPosition.x, yPosition * 300, UiSetting_LB_side_Light.rectTransform.localPosition.z);
        UiSetting_LB_side_Light_angle.transform.localPosition = new Vector3(UiSetting_LB_side_Light.rectTransform.localPosition.x, yPosition * 300, UiSetting_LB_side_Light.rectTransform.localPosition.z);
        UiSetting_LB_side.transform.SetAsLastSibling();
    }

    public void RotateVertical(float VerticalValue)
    {
        Vector3 vector3FillScale = UiSetting_LB_Light_angle_fill.transform.localScale;
        Vector3 vector3FillRotate = UiSetting_LB_Light_angle_fill.transform.localEulerAngles;
        Vector3 vector3RightRotate = UiSetting_LB_Light_angle_right.transform.localEulerAngles;
        float scale = Remap(VerticalValue, -256, 256, 0.5f, 1.0f);
        float rotate = Remap(VerticalValue, -256, 256, 30, 0);
        UiSetting_LB_Light_angle_fill.transform.localScale = new Vector3(vector3FillScale.x, scale, vector3FillScale.z);
        UiSetting_LB_Light_angle_fill.transform.localEulerAngles = new Vector3(vector3FillRotate.x, vector3FillRotate.y, rotate/2);
        UiSetting_LB_Light_angle_right.transform.localEulerAngles = new Vector3(vector3RightRotate.x, vector3RightRotate.y, rotate);
        UiSetting_LB_side.transform.SetAsLastSibling();
    }

    public void RotateHorizontal(float HorizontalValue)
    {
        Vector3 vector3FillScale = UiSetting_LB_side_Light_angle_fill.transform.localScale;
        Vector3 vector3FillRotate = UiSetting_LB_side_Light_angle_fill.transform.localEulerAngles;
        Vector3 vector3RightRotate = UiSetting_LB_side_Light_angle_top.transform.localEulerAngles;
        float scale = Remap(HorizontalValue, -256, 256, 0.5f, 1.0f);
        float rotate = Remap(HorizontalValue, -256, 256, -30, 0);
        UiSetting_LB_side_Light_angle_fill.transform.localScale = new Vector3(vector3FillScale.x, scale, vector3FillScale.z);
        UiSetting_LB_side_Light_angle_fill.transform.localEulerAngles = new Vector3(vector3FillRotate.x, vector3FillRotate.y, rotate/2);
        UiSetting_LB_side_Light_angle_top.transform.localEulerAngles = new Vector3(vector3RightRotate.x, vector3RightRotate.y, rotate);
        UiSetting_LB_side.transform.SetAsLastSibling();
    }

    public void ToggleLightCone(bool toggleOnOff)
    {
        UiSetting_LB_Light_angle.SetActive(toggleOnOff);
        UiSetting_LB_side_Light_angle.SetActive(toggleOnOff);
    }

    public static float Remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

}
