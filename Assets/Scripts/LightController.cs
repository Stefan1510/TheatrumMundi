using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LightController : MonoBehaviour
{
    public Toggle toggleLb;
    public Slider sliderLbIntensity;
    public Slider sliderLbPosition;
    public Slider sliderLbHeight;
    public Slider sliderLbHorizontal;
    public Slider sliderLbVertical;
    private float _startPosition;
    private float _startYAngle;
    private float _startHeight;
    private float _startXAngle;

    private Texture2D _lbCookie;
    private Texture2D _lbCookieOriginal;
    private Color[] _lbCookieColors;
    private int _lbCookieWidth;
    private int _lbCookieHeight;
    private int _lbCookieAttenuation;
    private Color[] _lbColors_on;
    private Color[] _lbColors_off;
    private Color[] _lbColors_smooth;
    private int _lbAngleHorizontal;
    private int _lbAngleVertical;

    public Image UiSetting_LB_Image;

    [HideInInspector] public Image PanelLbImage;

    [HideInInspector] public LightElement thisLightElement;

    private void Awake()
    {
        _startPosition = transform.localPosition.z;
        _startYAngle = transform.localEulerAngles.y;
        _startHeight = transform.localPosition.y;
        _lbCookieOriginal = (Texture2D)GetComponent<Light>().cookie;
        _lbCookieWidth = 1024;
        _lbCookieHeight = 1024;
        _lbCookieAttenuation = 128;
        //_lbCookie = _lbCookieOriginal;
        _lbCookie = new Texture2D(_lbCookieWidth, _lbCookieHeight);
        _lbCookie.wrapModeU = TextureWrapMode.Clamp;
        _lbCookie.SetPixels(_lbCookie.GetPixels());
        _lbCookieColors = new Color[_lbCookieWidth * _lbCookieHeight];
        _lbColors_on = new Color[_lbCookieWidth * _lbCookieHeight];
        _lbColors_off = new Color[_lbCookieWidth * _lbCookieHeight];
        _lbColors_smooth = new Color[_lbCookieWidth];

        _lbColors_on = changeColors(_lbColors_on, new Color(1f, 1f, 1f, 1f));
        _lbColors_off = changeColors(_lbColors_off, new Color(0f, 0f, 0f, 0f));
        ChangeHorizontal(256);
        ChangeIntensity(sliderLbIntensity.value);
        // ChangePosition(sliderLbPosition.value);
        ChangeHeight(sliderLbHeight.value);
        PanelLbImage = toggleLb.transform.parent.parent.GetComponent<Image>();
        PanelLbImage.color = new Color(171f / 255f, 171f / 255f, 171f / 255f, 160f / 255f);

    }
    void Start()
    {
        thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
        //Debug.Log(PanelLbImage);
        LightActivation(false);
        ChangePosition(sliderLbPosition.value);
        //Debug.Log("pos: " + sliderLbPosition.value);
    }
    public void LightActivation(bool onOffSwitch)
    {
        //lightElements muessen erst noch mit der StaticData bekannt gemacht werden --> SceneDataController

        thisLightElement.active = onOffSwitch;
        GetComponent<Light>().enabled = onOffSwitch;
        //StaticSceneData.Lights3D();
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().SetActive(onOffSwitch);
        if (onOffSwitch)
        {
            PanelLbImage.color = new Color(205f / 255f, 176f / 255f, 42f / 255f, 160f / 255f);
        }
        else
        {
            PanelLbImage.color = new Color(171f / 255f, 171f / 255f, 171f / 255f, 160f / 255f);
        }
        //ChangePosition(sliderLbPosition.value);
    }
    public void ChangeIntensity(float intensityValue)
    {
        thisLightElement.intensity = intensityValue;
        //Debug.Log("intensity: "+thisLightElement.intensity+", thislightEl: "+thisLightElement.name);
        GetComponent<Light>().intensity = intensityValue;
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().setBrightness_UiSetting_LB_Light_angle(intensityValue);
    }
    public void ChangePosition(float PositionValue)
    {
        //Debug.Log(name + " YAngle: " + _startYAngle);
        thisLightElement.z = PositionValue;
        float angleYValue = PositionValue * 10;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _startPosition + PositionValue);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, _startYAngle - angleYValue, transform.localEulerAngles.z);
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().setPosition_UiSetting_LB_Light(PositionValue);
    }
    public void ChangeHeight(float HeightValue)
    {
        thisLightElement.y = HeightValue;
        float angleXValue = HeightValue * 10;
        transform.localPosition = new Vector3(transform.localPosition.x, _startHeight + HeightValue, transform.localPosition.z);
        transform.localEulerAngles = new Vector3(_startXAngle + angleXValue, transform.localEulerAngles.y, transform.localEulerAngles.z);
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().setHeight_UiSetting_LB_side_Light(HeightValue);
    }
    public void ChangeHorizontalValue(float HorizontalValue)
    {
        //ChangeVertical(VerticalValue);
        thisLightElement.angle_v = (int)HorizontalValue;

        if (thisLightElement.stagePosition == 1)
        {
            ChangeVertical(thisLightElement.angle_v);
            Debug.Log("pos: "+thisLightElement.stagePosition+", this: "+thisLightElement.name+thisLightElement.angle_v);
        }
        else
        {
            ChangeVerticalLeft(thisLightElement.angle_v);
            Debug.Log("pos: "+thisLightElement.stagePosition+", this: "+thisLightElement.name);
        }
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().RotateVertical(HorizontalValue);
    }

    public void ChangeVerticalValue(float VerticalValue)
    {
        //ChangeHorizontal(HorizontalValue);
        thisLightElement.angle_h = (int)VerticalValue;

        if (thisLightElement.stagePosition == 1)
            ChangeHorizontal(thisLightElement.angle_h);
        else
            ChangeHorizontalLeft(thisLightElement.angle_h);

        //ChangeHorizontal(thisLightElement.angle_h);
        UiSetting_LB_Image.GetComponent<LightRepresentationController>().RotateHorizontal(VerticalValue);
    }

    public void ChangeHorizontal(int HorizontalValue)
    {
        //Debug.Log("PING ChangeHorizontal");
        int attenuation = _lbCookieAttenuation;
        HorizontalValue = _lbCookieWidth / 2 - HorizontalValue; //wie weit geht das Licht zu, hier noch auf beiden Seiten
        _lbAngleHorizontal = Mathf.Max(1, (int)(HorizontalValue) - attenuation);
        ChangeCookie();

        _lbCookie.Apply();
        GetComponent<Light>().cookie = _lbCookie;
    }

    public void ChangeHorizontalLeft(int HorizontalValue)
    {
        //Debug.Log("PING ChangeHorizontal");
        int attenuation = _lbCookieAttenuation;
        HorizontalValue = _lbCookieWidth / 2 - HorizontalValue; //wie weit geht das Licht zu, hier noch auf beiden Seiten
        _lbAngleHorizontal = Mathf.Max(1, (int)(HorizontalValue) - attenuation);
        ChangeCookie();

        FlipTexture(ref _lbCookie);

        _lbCookie.Apply();
        GetComponent<Light>().cookie = _lbCookie;
    }

    public void ChangeVertical(int VerticalValue)
    {
        int attenuation = _lbCookieAttenuation;
        VerticalValue = _lbCookieWidth / 2 - VerticalValue; //wie weit geht das Licht zu, hier noch auf beiden Seiten
        _lbAngleVertical = Mathf.Max(1, (int)(VerticalValue) - attenuation);
        ChangeCookie();

        _lbCookie.Apply();
        GetComponent<Light>().cookie = _lbCookie;
    }

    public void ChangeVerticalLeft(int VerticalValue)
    {
        int attenuation = _lbCookieAttenuation;
        VerticalValue = _lbCookieWidth / 2 - VerticalValue; //wie weit geht das Licht zu, hier noch auf beiden Seiten
        _lbAngleVertical = Mathf.Max(1, (int)(VerticalValue) - attenuation);
        ChangeCookie();

        FlipTexture(ref _lbCookie);

        _lbCookie.Apply();
        GetComponent<Light>().cookie = _lbCookie;
    }

    private void ChangeCookie()
    {

        float grayScale = 0.0f;
        int attenuation = _lbCookieAttenuation;
        _lbCookie.SetPixels(0, 0, _lbCookieWidth, _lbCookieHeight, _lbColors_on);

        _lbCookie.SetPixels(0, 0, 1, _lbCookieHeight, _lbColors_off);
        _lbCookie.SetPixels(_lbCookieWidth - _lbAngleVertical, 0, _lbAngleVertical, _lbCookieHeight, _lbColors_off);

        for (int i = 1; i < attenuation; i++)
        {
            grayScale = (float)i / (float)attenuation;
            _lbColors_smooth = changeColors(_lbColors_smooth, new Color(grayScale, grayScale, grayScale, grayScale));
            _lbCookie.SetPixels(i, 0, 1, _lbCookieHeight, _lbColors_smooth);
            _lbCookie.SetPixels(_lbCookieWidth - _lbAngleVertical - i, 0, 1, _lbCookieHeight, _lbColors_smooth);
        }

        _lbCookie.SetPixels(0, 0, _lbCookieWidth, 1, _lbColors_off);
        _lbCookie.SetPixels(0, _lbCookieHeight - _lbAngleHorizontal, _lbCookieWidth, _lbAngleHorizontal, _lbColors_off);
        for (int i = 0; i < attenuation; i++)
        {
            grayScale = (float)i / (float)attenuation;
            _lbColors_smooth = changeColors(_lbColors_smooth, new Color(grayScale, grayScale, grayScale, grayScale));
            _lbCookie.SetPixels(i, i, _lbCookieWidth - 2 * i - _lbAngleVertical, 1, _lbColors_smooth);
            _lbCookie.SetPixels(i, _lbCookieHeight - _lbAngleHorizontal - i, _lbCookieWidth - 2 * i - _lbAngleVertical, 1, _lbColors_smooth);
        }
    }

    Color[] changeColors(Color[] colorArray, Color changeToColor)
    {
        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = changeToColor;
        }
        return colorArray;
    }

    public static void FlipTexture(ref Texture2D texture)
    {
        int textureWidth = texture.width;
        int textureHeight = texture.height;

        Color32[] pixels = texture.GetPixels32();

        for (int y = 0; y < textureHeight; y++)
        {
            int yo = y * textureWidth;
            for (int il = yo, ir = yo + textureWidth - 1; il < ir; il++, ir--)
            {
                Color32 col = pixels[il];
                pixels[il] = pixels[ir];
                pixels[ir] = col;
            }
        }
        texture.SetPixels32(pixels);
        texture.Apply();
    }
}
