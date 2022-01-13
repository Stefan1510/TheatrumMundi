using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightRepresentationController : MonoBehaviour
{
    public Image UiSetting_LB;
    public Image UiSetting_LB_Light;
    public Sprite UiSetting_LB_on_Sprite;
    public Sprite UiSetting_LB_off_Sprite;
    public Sprite UiSetting_LB_active_Sprite;
    public Sprite UiSetting_LB_Light_on_Sprite;
    public Sprite UiSetting_LB_Light_off_Sprite;
    public Sprite UiSetting_LB_Light_active_Sprite;
    // Start is called before the first frame update
    void Start()
    {
        UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_off_Sprite;
        UiSetting_LB_Light.GetComponent<Image>().sprite = UiSetting_LB_Light_active_Sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setActive(bool trueFalse)
    {
        UiSetting_LB.GetComponent<Image>().sprite = UiSetting_LB_active_Sprite;
    }
}
