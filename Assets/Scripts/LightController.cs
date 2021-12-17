using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LightController : MonoBehaviour
{
    public Toggle toggleLb;
    public Slider sliderLbBrightness;
    public Slider sliderLbPosition;
    public Slider sliderLbHeight;
    public Slider sliderLbHorizontal;
    public Slider sliderLbVertical;
    private float _startPosition;
    private float _startHeight;

    private Texture2D _lbCookie;
    private int _lbCookieWidth;
    private int _lbCookieHeight;
    private Color[] _lbColors_on;
    private Color[] _lbColors_off;
    private Color[] _lbColors_smooth;
    private int _lbAngleHorizontal;
    private int _lbAngleVertical;
    //private void Awake()
    //{

    //}

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.localPosition.z;
        _startHeight = transform.localPosition.y;
        _lbCookie = (Texture2D)GetComponent<Light>().cookie;
        _lbCookieWidth = _lbCookie.width;
        _lbCookieHeight = _lbCookie.height;
        _lbColors_on = new Color[_lbCookieWidth * _lbCookieHeight];
        _lbColors_off = new Color[_lbCookieWidth * _lbCookieHeight];
        _lbColors_smooth = new Color[_lbCookieWidth];

        _lbColors_on = changeColors(_lbColors_on, new Color(1f, 1f, 1f, 1f));
        _lbColors_off = changeColors(_lbColors_off, new Color(0f, 0f, 0f, 0f));
        ChangeHorizontal(256f);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void LightActivation(bool onOffSwitch)
    {

        //lightElements müssen erst noch mit der StaticData bekannt gemacht werden --> SceneDataController

        LightElement thisLightElement = StaticSceneData.StaticData.lightElements.Find(le => le.name == gameObject.name);
        thisLightElement.active = onOffSwitch;
        //gameController.GetComponent<SceneDataController>().LightsApplyToScene(StaticSceneData.StaticData.lightElements);
        StaticSceneData.Lights3D();
    }

    public void ChangeBrightness(float brightnessValue)
    {
        GetComponent<Light>().intensity = brightnessValue;
    }

    public void ChangePosition(float PositionValue)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _startPosition + PositionValue);
    }

    public void ChangeHeight(float HeightValue)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, _startHeight + HeightValue, transform.localPosition.z);
    }

    public void ChangeHorizontal(float HorizontalValue)
    {
        float grayScale = 0.0f;
        int attenuation = 256;
        HorizontalValue = _lbCookieWidth / 2 - HorizontalValue; //wie weit geht das Licht zu, hier noch auf beiden Seiten
        _lbAngleHorizontal = Mathf.Max(1, (int)(HorizontalValue) - attenuation);
        ChangeCookie();

        _lbCookie.Apply();
        GetComponent<Light>().cookie = _lbCookie;
    }

    public void ChangeVertical(float VerticalValue)
    {
        int attenuation = 256;
        VerticalValue = _lbCookieWidth / 2 - VerticalValue; //wie weit geht das Licht zu, hier noch auf beiden Seiten
        _lbAngleVertical = Mathf.Max(1, (int)(VerticalValue) - attenuation);
        ChangeCookie();

        _lbCookie.Apply();
        GetComponent<Light>().cookie = _lbCookie;
    }

    private void ChangeCookie()
    {

        float grayScale = 0.0f;
        int attenuation = 256;
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
}
