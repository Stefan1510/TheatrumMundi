using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightAnimationRepresentation : MonoBehaviour
{
    private Texture2D _textureLightRepresentation;
    //[SerializeField] private Slider _sliderTime;
    [SerializeField] private Slider _sliderIntensity;
    [SerializeField] private Image _imagePositionKnob;
    [SerializeField] private GameObject _representationPanel;
    private List<Image> _imagePositionKnobCollection;
    private Color32[] _lightColors;
    private Color32[] _lightColorsColumn;
    private float _maxTime;
    private float _maxIntensity;
    private bool _gameObjectStarted = false;


    private void Awake()
    {
        _maxIntensity = _sliderIntensity.maxValue;
        _maxTime = AnimationTimer.GetMaxTime();
        _textureLightRepresentation = new Texture2D((int)_maxTime * 10, (int)_maxTime * 1, TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch automatisch gescaled
        _lightColors = new Color32[_textureLightRepresentation.width * _textureLightRepresentation.height];
        _lightColorsColumn = new Color32[_textureLightRepresentation.height];
        _imagePositionKnob.gameObject.SetActive(false);
        _imagePositionKnobCollection = new List<Image>();
    }

    void Start()
    {
        _gameObjectStarted = true;
        ChangeImage();
    }

    private void OnEnable()
    {
        if (_gameObjectStarted)
        {
            ChangeImage();
        }
    }

    public void ChangeImage()
    {
        //Debug.Log("change");
        _maxTime = AnimationTimer.GetMaxTime();
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

        //Debug.Log("listLength " + listLength);
        Color32 colorGradient;
        for (int lightStates = 0; lightStates < listLength - 1; lightStates++)
        {
            //Debug.Log("bluip, in der Schelife");
            colorStart = new Color32(StaticSceneData.StaticData.lightingSets[lightStates].r, StaticSceneData.StaticData.lightingSets[lightStates].g, StaticSceneData.StaticData.lightingSets[lightStates].b, 255);   // holen der "fr�heren" Farbe aus der Datenhaltung
            colorEnd = new Color32(StaticSceneData.StaticData.lightingSets[lightStates + 1].r, StaticSceneData.StaticData.lightingSets[lightStates + 1].g, StaticSceneData.StaticData.lightingSets[lightStates + 1].b, 255); // holen der "sp�teren" Farbe aus der Datenhaltung
            float momentStartF = StaticSceneData.StaticData.lightingSets[lightStates].moment; // holen des "fr�heren" Moments aus der Datenhaltung
            momentStart = (int)UtilitiesTm.FloatRemap(momentStartF, 0, _maxTime, 0, _textureLightRepresentation.width);    // mappen des "fr�heren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            float momentEndF = StaticSceneData.StaticData.lightingSets[lightStates + 1].moment; // holen des "sp�teren" Moments aus der Datenhaltung
            momentEnd = (int)UtilitiesTm.FloatRemap(momentEndF, 0, _maxTime, 0, _textureLightRepresentation.width); // mappen des "sp�teren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            intensityStart = StaticSceneData.StaticData.lightingSets[lightStates].intensity;  // holen der "fr�heren" Lichtst�rke aus der Datenhaltung
            intensityEnd = StaticSceneData.StaticData.lightingSets[lightStates + 1].intensity;    // holen der "sp�teren" Lichtst�rke aus der Datenhaltung
            //Debug.Log("momentStart: " + momentStart + " - momentEnd: " + momentEnd + " - intensityStart: " + intensityStart + " - intensityEnd: " + intensityEnd);
            for (secondCount = momentStart; secondCount <= momentEnd; secondCount++)
            {
                float _interpolationStep = UtilitiesTm.FloatRemap((float)secondCount, (float)momentStart, (float)momentEnd, 0f, 1f);    //mappen der aktuellen Sekunde zwischen des "fr�heren" und "sp�teren" Moments auf einen Wert zwischen 0 und 1 --> Lerp braucht so einen Wert
                colorGradient = Color32.Lerp(colorStart, colorEnd, _interpolationStep);
                float intensityGradientF = Mathf.Lerp(intensityStart, intensityEnd, _interpolationStep);
                intensityGradient = UtilitiesTm.FloatRemap(intensityGradientF, 0, _maxIntensity, 0, _textureLightRepresentation.height);
                _lightColorsColumn = ChangeColors(_lightColorsColumn, colorGradient);
                //Debug.Log("secondCount " + secondCount + " -intensityGradient " + intensityGradient + " -_lightColorsColumn " + _lightColorsColumn);
                _textureLightRepresentation.SetPixels32(secondCount, 0, 1, (int)intensityGradient, _lightColorsColumn);
            }
        }
        colorEnd = new Color32(StaticSceneData.StaticData.lightingSets[listLength - 1].r, StaticSceneData.StaticData.lightingSets[listLength - 1].g, StaticSceneData.StaticData.lightingSets[listLength - 1].b, 255);
        Color32[] lastColorBlock = new Color32[(_textureLightRepresentation.width - momentEnd - 1) * (int)intensityGradient];
        lastColorBlock = ChangeColors(lastColorBlock, colorEnd);
        _textureLightRepresentation.SetPixels32(secondCount, 0, (_textureLightRepresentation.width - momentEnd - 1), (int)intensityGradient, lastColorBlock);
        _textureLightRepresentation.Apply();
        _representationPanel.GetComponent<Image>().sprite = Sprite.Create(_textureLightRepresentation, new Rect(0, 0, _textureLightRepresentation.width, _textureLightRepresentation.height), new Vector2(0.5f, 0.5f));
        UpdateKnobPositions();
    }

    public void UpdateKnobPositions()
    {
        foreach (Image image in _imagePositionKnobCollection)
        {
            Destroy(image.gameObject);
        }
        _imagePositionKnobCollection.Clear();
        float panelWidth = _representationPanel.GetComponent<RectTransform>().rect.width;
        float maxTime = AnimationTimer.GetMaxTime();
        float panelHeight = _representationPanel.GetComponent<RectTransform>().rect.height;
        float maxIntensity = _sliderIntensity.maxValue;
        foreach (LightingSet lightingSet in StaticSceneData.StaticData.lightingSets)
        {
            Image knobInstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
            knobInstance.gameObject.SetActive(true);
            float knobPosX = UtilitiesTm.FloatRemap(lightingSet.moment, 0, maxTime, 0, panelWidth);
            float knobPosY = UtilitiesTm.FloatRemap(lightingSet.intensity, 0, maxIntensity, -panelHeight / 2, panelHeight / 2);
            knobInstance.transform.localPosition = new Vector3(knobPosX, knobPosY, knobInstance.transform.localPosition.z);
            knobInstance.GetComponent<Image>().color = new Color32(lightingSet.r, lightingSet.g, lightingSet.b, 255);
            _imagePositionKnobCollection.Add(knobInstance);
        }
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
