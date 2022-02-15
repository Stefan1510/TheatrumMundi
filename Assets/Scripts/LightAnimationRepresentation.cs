using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightAnimationRepresentation : MonoBehaviour
{
    private Texture2D _textureLightRepresentation;
    //[SerializeField] private Slider _sliderTime;
    [SerializeField] private Slider _sliderIntensity;
    private Color32[] _lightColors;
    private Color32[] _lightColorsColumn;
    private float _maxTime;
    private float _maxIntensity;
    // Start is called before the first frame update
    void Start()
    {
        //_maxTime = (int)_sliderTime.maxValue;
        _maxTime = 60*10;
        _maxIntensity = _sliderIntensity.maxValue;
        Debug.Log(_maxTime);
        _textureLightRepresentation = new Texture2D((int)_maxTime * 10, (int)_maxTime * 1, TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch stretch automatisch gescaled
        //_textureLightRepresentation.filterMode = FilterMode.Trilinear;
        _lightColors = new Color32[_textureLightRepresentation.width * _textureLightRepresentation.height];
        _lightColorsColumn = new Color32[_textureLightRepresentation.height];
        ChangeImage();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void ChangeImage()
    {
        _lightColors = ChangeColors(_lightColors, new Color32(255, 255, 255, 255));
        _textureLightRepresentation.SetPixels32(_lightColors);
        Color32 colorStart = new Color(StaticSceneData.StaticData.lightingSets[0].r, StaticSceneData.StaticData.lightingSets[0].g, StaticSceneData.StaticData.lightingSets[0].b);
        Color32 colorEnd = new Color(StaticSceneData.StaticData.lightingSets[0].r, StaticSceneData.StaticData.lightingSets[0].g, StaticSceneData.StaticData.lightingSets[0].b);
        float intensityStart;
        float intensityEnd;
        float intensityGradient = StaticSceneData.StaticData.lightingSets[0].intensity;
        intensityGradient = UtilitiesTm.FloatRemap(intensityGradient, 0, _maxIntensity, 0, _textureLightRepresentation.height);
        int momentStart = (int)StaticSceneData.StaticData.lightingSets[0].moment;
        int momentEnd = (int)StaticSceneData.StaticData.lightingSets[0].moment;
        int secondCount = 0;
        int listLength = StaticSceneData.StaticData.lightingSets.Count;

        Color32 colorGradient;
        for (int lightStates = 0; lightStates < listLength - 1; lightStates++)
        {
            //Debug.Log("bluip, in der Schelife");
            colorStart = new Color(StaticSceneData.StaticData.lightingSets[lightStates].r, StaticSceneData.StaticData.lightingSets[lightStates].g, StaticSceneData.StaticData.lightingSets[lightStates].b);   // holen der "früheren" Farbe aus der Datenhaltung
            colorEnd = new Color(StaticSceneData.StaticData.lightingSets[lightStates + 1].r, StaticSceneData.StaticData.lightingSets[lightStates + 1].g, StaticSceneData.StaticData.lightingSets[lightStates + 1].b); // holen der "späteren" Farbe aus der Datenhaltung
            float momentStartF = StaticSceneData.StaticData.lightingSets[lightStates].moment; // holen des "früheren" Moments aus der Datenhaltung
            momentStart = (int)UtilitiesTm.FloatRemap(momentStartF, 0, _maxTime, 0, _textureLightRepresentation.width);    // mappen des "früheren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            float momentEndF = StaticSceneData.StaticData.lightingSets[lightStates + 1].moment; // holen des "späteren" Moments aus der Datenhaltung
            momentEnd = (int)UtilitiesTm.FloatRemap(momentEndF, 0, _maxTime, 0, _textureLightRepresentation.width); // mappen des "späteren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            intensityStart = StaticSceneData.StaticData.lightingSets[lightStates].intensity;  // holen der "früheren" Lichtstärke aus der Datenhaltung
            intensityEnd = StaticSceneData.StaticData.lightingSets[lightStates + 1].intensity;    // holen der "späteren" Lichtstärke aus der Datenhaltung
            //Debug.Log("momentStart: " + momentStart + " - momentEnd: " + momentEnd + " - intensityStart: " + intensityStart + " - intensityEnd: " + intensityEnd);
            for (secondCount = momentStart; secondCount <= momentEnd; secondCount++)
            {
                float _interpolationStep = UtilitiesTm.FloatRemap((float)secondCount, (float)momentStart, (float)momentEnd, 0f, 1f);    //mappen der aktuellen Sekunde zwischen des "früheren" und "späteren" Moments auf einen Wert zwischen 0 und 1 --> Lerp braucht so einen Wert
                colorGradient = Color32.Lerp(colorStart, colorEnd, _interpolationStep);
                float intensityGradientF = Mathf.Lerp(intensityStart, intensityEnd, _interpolationStep);
                intensityGradient = UtilitiesTm.FloatRemap(intensityGradientF, 0, _maxIntensity, 0, _textureLightRepresentation.height);
                _lightColorsColumn = ChangeColors(_lightColorsColumn, colorGradient);
                _textureLightRepresentation.SetPixels32(secondCount, 0, 1, (int)intensityGradient, _lightColorsColumn);
            }

        }
        colorEnd = new Color(StaticSceneData.StaticData.lightingSets[listLength - 1].r, StaticSceneData.StaticData.lightingSets[listLength - 1].g, StaticSceneData.StaticData.lightingSets[listLength - 1].b);
        Debug.Log(listLength + " - r  " + colorEnd.r + " - g  " + colorEnd.g + " - b  " + colorEnd.b + " - intensityGradient " + intensityGradient);
        Color32[] lastColorBlock = new Color32[(_textureLightRepresentation.width - momentEnd - 1) * (int)intensityGradient];
        lastColorBlock = ChangeColors(lastColorBlock, colorEnd);
        _textureLightRepresentation.SetPixels32(secondCount, 0, (_textureLightRepresentation.width - momentEnd - 1), (int)intensityGradient, lastColorBlock);
        Debug.Log("_________________________________________________________");
        _textureLightRepresentation.Apply();
        GetComponent<Image>().sprite = Sprite.Create(_textureLightRepresentation, new Rect(0, 0, _textureLightRepresentation.width, _textureLightRepresentation.height), new Vector2(0.5f, 0.5f));
    }


    Color32[] ChangeColors(Color32[] colorArray, Color32 changeToColor)
    {
        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = changeToColor;
        }
        return colorArray;
    }
}
