using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawCurveBg : MonoBehaviour
{
    #region variables
    private Texture2D _textureCurve, _textureRest;
    [SerializeField] private Slider _valueSlider;
    [SerializeField] private Image _imagePositionKnob;
    [SerializeField] private GameObject _PanelLineDraw;
    [SerializeField] private GameObject _backgroundHimmel;

    private List<Image> _imagePositionKnobCollection;
    private Color32[] _backColors;
    private Color32[] _curveColors;
    private float _maxTime;
    private float _minValue;
    private float _maxValue;
    private int _rectWidth;
    #endregion
    void Start()
    {
        _rectWidth = (int)GetComponent<RectTransform>().rect.width;
        _textureCurve = new Texture2D(_rectWidth, (int)(_PanelLineDraw.GetComponent<RectTransform>().rect.height), TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch automatisch gescaled

        _backColors = new Color32[_textureCurve.width * _textureCurve.height];
        _backColors = UtilitiesTm.ChangeColors(_backColors, new Color32(255, 255, 255, 31));
        _curveColors = new Color32[3 * 3];
        _curveColors = UtilitiesTm.ChangeColors(_curveColors, new Color32(180, 180, 180, 150));

        _maxTime = AnimationTimer.GetMaxTime();
        _minValue = _valueSlider.minValue;
        _maxValue = _valueSlider.maxValue;
        _imagePositionKnob.gameObject.SetActive(false);
        _imagePositionKnobCollection = new List<Image>();

        // EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();
        // eventTriggerEntry.eventID = EventTriggerType.PointerUp;
        // eventTriggerEntry.callback.AddListener((data) => { AddValue(); });
        // _valueSlider.onValueChanged.AddListener((float value) => ChangeLiveBackground(value));
        // _valueSlider.GetComponent<EventTrigger>().triggers.Add(eventTriggerEntry);

        StaticSceneData.StaticData.backgroundPositions[0] = new BackgroundPosition { moment = 0, yPosition = _valueSlider.value };  // im SceneDataController MUSS ein erstes Element hinzugef�gt werden, bevor es hier angesprochen werden kann
        StaticSceneData.StaticData.backgroundPositions.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die railElementSpeeds anhand der Eigenschaft moment
        //ChangeCurve();
        //ChangeBackgroundPosition();
        Debug.Log("start fertig");
    }

    void Update()
    {
        ChangeBackgroundPosition();
    }

    public void AddValue()      // Aufruf auf Slider, der etwas veraendern soll, geloest ueber EventTrigger, uebergebene data unnoetig
    {
        Debug.Log("addvalue");
        float valueSliderValue = _valueSlider.value;
        float valueMoment = AnimationTimer.GetTime();
        int momentIndex = StaticSceneData.StaticData.backgroundPositions.FindIndex(bgPos => bgPos.moment == valueMoment);

        BackgroundPosition thisBackgroundPosition = new BackgroundPosition();

        if (momentIndex == -1)  // wenn es schon einen punkt an der stelle gibt
        {
            thisBackgroundPosition = new BackgroundPosition { moment = valueMoment, yPosition = valueSliderValue };
            StaticSceneData.StaticData.backgroundPositions.Add(thisBackgroundPosition);
            Debug.Log("count: "+StaticSceneData.StaticData.backgroundPositions.Count+", value: " + valueMoment);
        }
        else
        {
            Debug.Log("else");
            StaticSceneData.StaticData.backgroundPositions[momentIndex] = new BackgroundPosition { moment = valueMoment, yPosition = valueSliderValue };
        }
        StaticSceneData.StaticData.backgroundPositions.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die backgroundPositions anhand der Eigenschaft moment

        ChangeCurve();
        ChangeBackgroundPosition();
    }

    public void ChangeCurve()
    {
        _maxTime = AnimationTimer.GetMaxTime();
        _textureCurve.SetPixels32(_backColors);

        int momentStart = (int)StaticSceneData.StaticData.backgroundPositions[0].moment;
        int momentEnd = (int)StaticSceneData.StaticData.backgroundPositions[0].moment;
        Debug.Log("momentStart: "+momentStart+", momentend: " + momentEnd);

        int valueStart = (int)StaticSceneData.StaticData.backgroundPositions[0].yPosition;
        int valueEnd = (int)StaticSceneData.StaticData.backgroundPositions[0].yPosition;

        int listLength = StaticSceneData.StaticData.backgroundPositions.Count;
        if(listLength > 1)
        {
        for (int valueStates = 0; valueStates < listLength - 1; valueStates++)
        {
            float momentStartF = (int)StaticSceneData.StaticData.backgroundPositions[valueStates].moment; // holen des "frueheren" Moments aus der Datenhaltung
            momentStartF = UtilitiesTm.FloatRemap(momentStartF, 0, _maxTime, 0, _textureCurve.width - 3);    // mappen des "frueheren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentStart = (int)momentStartF;

            float momentEndF = (int)StaticSceneData.StaticData.backgroundPositions[valueStates + 1].moment; // holen des "spaeteren" Moments aus der Datenhaltung

            momentEndF = UtilitiesTm.FloatRemap(momentEndF, 0, _maxTime, 0, _textureCurve.width - 3); // mappen des "spaeteren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentEnd = (int)momentEndF;

            float valueStartF = StaticSceneData.StaticData.backgroundPositions[valueStates].yPosition;  // holen des "fr�heren" Werts aus der Datenhaltung
            valueStartF = UtilitiesTm.FloatRemap(valueStartF, _minValue, _maxValue, 0, _textureCurve.height - 3);   // mappen des "fr�heren" Werts von zwischen ValueSlider auf zwischen GraphicHoehe
            valueStart = (int)valueStartF;

            float valueEndF = StaticSceneData.StaticData.backgroundPositions[valueStates + 1].yPosition;    // holen des "sp�teren" Werts aus der Datenhaltung
            valueEndF = UtilitiesTm.FloatRemap(valueEndF, _minValue, _maxValue, 0, _textureCurve.height - 3);    // mappen des "fr�heren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            valueEnd = (int)valueEndF;
            //Debug.Log("valueEnd: "+valueEnd);

            _textureCurve = UtilitiesTm.Bresenham(_textureCurve, momentStart, valueStart, momentEnd, valueEnd, _curveColors);

            float deltaMoment = momentEndF - momentStartF;  // deltaX
            float deltaValue = valueEndF - valueStartF;     // deltaY
        }
        }
        else
        {
            float valueEndF = StaticSceneData.StaticData.backgroundPositions[0].yPosition;    // holen des "sp�teren" Werts aus der Datenhaltung
            valueEndF = UtilitiesTm.FloatRemap(valueEndF, _minValue, _maxValue, 0, _textureCurve.height - 3);    // mappen des "fr�heren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            valueEnd = (int)valueEndF;
            Debug.Log("valueEnd: "+valueEnd);
        }

        _textureRest = new Texture2D(_rectWidth, 80, TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch automatisch gescaled
        Debug.Log("bg: momentend: " + momentEnd + ", valueEnd: " + valueEnd + ", valueStart: " + valueStart + ", _texturewidth: " + (_textureCurve.width - 3));
        _textureRest = UtilitiesTm.Bresenham(_textureCurve, momentEnd, valueEnd, _textureCurve.width - 3, valueEnd, _curveColors);

        _textureCurve.Apply();
        _PanelLineDraw.GetComponent<Image>().sprite = Sprite.Create(_textureCurve, new Rect(0, 0, _textureCurve.width, _textureCurve.height), new Vector2(0.5f, 0.5f));
        if (SceneManaging.isExpert)
            UpdateKnobPositions();
    }

    public void UpdateKnobPositions()
    {
        foreach (Image image in _imagePositionKnobCollection)
        {
            Destroy(image.gameObject);
        }
        _imagePositionKnobCollection.Clear();

        float panelHeight = _PanelLineDraw.GetComponent<RectTransform>().rect.height;

        foreach (BackgroundPosition backgroundPosition in StaticSceneData.StaticData.backgroundPositions)
        {
            Image knobInstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
            knobInstance.gameObject.SetActive(true);
            float knobPosX = UtilitiesTm.FloatRemap(backgroundPosition.moment, 0, _maxTime, 0, _rectWidth);
            float knobPosY = UtilitiesTm.FloatRemap(backgroundPosition.yPosition, _minValue, _maxValue, -panelHeight / 2, panelHeight / 2);
            knobInstance.transform.localPosition = new Vector3(knobPosX, knobPosY, knobInstance.transform.localPosition.z);
            _imagePositionKnobCollection.Add(knobInstance);
        }
    }

    void ChangeBackgroundPosition()
    {
        float TimeNow = AnimationTimer.GetTime();
        float _backgroundHimmelYPos = _backgroundHimmel.transform.localPosition.y;
        int _listLength = StaticSceneData.StaticData.backgroundPositions.Count;

        for (int i = 0; i < _listLength - 1; i++)
        {
            if ((StaticSceneData.StaticData.backgroundPositions[i].moment <= TimeNow) && (TimeNow <= StaticSceneData.StaticData.backgroundPositions[i + 1].moment))
            {
                _backgroundHimmelYPos = UtilitiesTm.FloatRemap(TimeNow, StaticSceneData.StaticData.backgroundPositions[i].moment, StaticSceneData.StaticData.backgroundPositions[i + 1].moment, StaticSceneData.StaticData.backgroundPositions[i].yPosition, StaticSceneData.StaticData.backgroundPositions[i + 1].yPosition);
                _backgroundHimmel.transform.localPosition = new Vector3(_backgroundHimmel.transform.localPosition.x, _backgroundHimmelYPos, _backgroundHimmel.transform.localPosition.z);
            }
        }
    }
    public void ChangeLiveBackground()
    {
        _backgroundHimmel.transform.localPosition = new Vector3(_backgroundHimmel.transform.localPosition.x, _valueSlider.value, _backgroundHimmel.transform.localPosition.z);
    }
}
