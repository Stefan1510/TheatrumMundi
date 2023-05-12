using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawCurve : MonoBehaviour
{
    #region variables
    private Texture2D _textureCurve, _textureRest;
    [SerializeField] private UnitySwitchExpertUser gameController;
    [SerializeField] private Slider _valueSlider;
    [SerializeField] private Image _imagePositionKnob;
    [SerializeField] private int _railIndex;
    private List<Image> _imagePositionKnobCollection;
    private Color32[] _backColors;
    private Color32[] _curveColors;
    private Color32[] _middleLineColors;
    private float _maxTime;
    private float _minValue;
    private float _maxValue;
    private int _rectWidth;
    #endregion
    void Start()
    {
        if (gameController._isExpert)
        {
            _rectWidth = (int)GetComponent<RectTransform>().rect.width;
            _textureCurve = new Texture2D(_rectWidth, 80, TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch automatisch gescaled

            _backColors = new Color32[_textureCurve.width * _textureCurve.height];
            _backColors = UtilitiesTm.ChangeColors(_backColors, new Color32(255, 255, 255, 31));
            _curveColors = new Color32[3 * 3];
            _curveColors = UtilitiesTm.ChangeColors(_curveColors, new Color32(180, 180, 180, 150));
            
            _middleLineColors = new Color32[_textureCurve.width * 2];
            _middleLineColors = UtilitiesTm.ChangeColors(_middleLineColors, new Color32(224, 224, 224, 224));
            
            _maxTime = AnimationTimer.GetMaxTime();
            _minValue = _valueSlider.minValue;
            _maxValue = _valueSlider.maxValue;
            _imagePositionKnob.gameObject.SetActive(false);
            _imagePositionKnobCollection = new List<Image>();
            
            EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();
            eventTriggerEntry.eventID = EventTriggerType.PointerUp;
            eventTriggerEntry.callback.AddListener((data) => { AddValue(); });
            _valueSlider.GetComponent<EventTrigger>().triggers.Add(eventTriggerEntry);
Debug.Log("val: "+_valueSlider.value);
            StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[0] = new RailElementSpeed { moment = 0, speed = _valueSlider.value };  // im SceneDataController MUSS ein erstes Element hinzugef�gt werden, bevor es hier angesprochen werden kann
            Debug.Log("speed: "+StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[0].speed);
            StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die railElementSpeeds anhand der Eigenschaft moment
            ChangeCurve();
        }
        else
        {
            this.gameObject.SetActive(false);
            StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[0] = new RailElementSpeed { moment = 0, speed = _valueSlider.value };
        }
    }
    public void AddValue()      // Aufruf auf Slider, der etwas ver�ndern soll, gel�st �ber EventTrigger, �bergebene data unn�tig
    {
        if (_railIndex == ImageTimelineSelection.GetRailNumber())
        {
            //Debug.LogError(data);
            float valueSliderValue = _valueSlider.value;
            float valueMoment = AnimationTimer.GetTime();
            int momentIndex = StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds.FindIndex(mom => mom.moment == valueMoment);     //hier spezifisches RailElement, das kann dann entsprechend ge�ndert werden

            RailElementSpeed thisRailElementSpeed = new RailElementSpeed();
            //Debug.Log(StaticSceneData.StaticData.railElements[_railIndex].name);

            if (momentIndex == -1)
            {
                thisRailElementSpeed = new RailElementSpeed { moment = valueMoment, speed = valueSliderValue };
                StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds.Add(thisRailElementSpeed);     //hier spezifisches RailElement, das kann dann entsprechend ge�ndert werden
            }
            else
            {
                StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[momentIndex] = new RailElementSpeed { moment = valueMoment, speed = valueSliderValue };
            }
            StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die railElementSpeeds anhand der Eigenschaft moment

            ChangeCurve();
        }
    }
    public void ChangeCurve()
    {
        _maxTime = AnimationTimer.GetMaxTime();
        _textureCurve.SetPixels32(_backColors);
        //_textureCurve = UtilitiesTm.Bresenham(_textureCurve, 0, _textureCurve.height / 2, _textureCurve.width - 3, _textureCurve.height / 2, _middleLineColors);
        _textureCurve.SetPixels32(0, _textureCurve.height / 2, _textureCurve.width, 2, _middleLineColors);

        int momentStart = (int)StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[0].moment;
        int momentEnd = (int)StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[0].moment;

        int valueStart = (int)StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[0].speed;
        int valueEnd = 39;
        Debug.Log("rail ind: "+_railIndex+", value: "+StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[0].speed);

        int listLength = StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds.Count;
        for (int valueStates = 0; valueStates < listLength - 1; valueStates++)
        {
            float momentStartF = (int)StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[valueStates].moment; // holen des "frueheren" Moments aus der Datenhaltung
            momentStartF = UtilitiesTm.FloatRemap(momentStartF, 0, _maxTime, 0, _textureCurve.width - 3);    // mappen des "frueheren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentStart = (int)momentStartF;

            float momentEndF = (int)StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[valueStates + 1].moment; // holen des "spaeteren" Moments aus der Datenhaltung
            momentEndF = UtilitiesTm.FloatRemap(momentEndF, 0, _maxTime, 0, _textureCurve.width - 3); // mappen des "sp�teren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentEnd = (int)momentEndF;

            float valueStartF = StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[valueStates].speed;  // holen des "fr�heren" Werts aus der Datenhaltung
            valueStartF = UtilitiesTm.FloatRemap(valueStartF, _minValue, _maxValue, 0, _textureCurve.height - 3);   // mappen des "fr�heren" Werts von zwischen ValueSlider auf zwischen GraphicHoehe
            valueStart = (int)valueStartF;

            float valueEndF = StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds[valueStates + 1].speed;    // holen des "sp�teren" Werts aus der Datenhaltung
            valueEndF = UtilitiesTm.FloatRemap(valueEndF, _minValue, _maxValue, 0, _textureCurve.height - 3);    // mappen des "fr�heren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            valueEnd = (int)valueEndF;

            _textureCurve = UtilitiesTm.Bresenham(_textureCurve, momentStart, valueStart, momentEnd, valueEnd, _curveColors);

            float deltaMoment = momentEndF - momentStartF;  // deltaX
            float deltaValue = valueEndF - valueStartF;     // deltaY

        }

        _textureRest = new Texture2D(_rectWidth, 80, TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch automatisch gescaled
        Debug.Log("momentend: "+momentEnd+", valueEnd: "+valueEnd+", _texturewidth: "+(_textureCurve.width-3));
        _textureRest = UtilitiesTm.Bresenham(_textureCurve, momentEnd, valueEnd, _textureCurve.width - 3, valueEnd, _curveColors);

        _textureCurve.Apply();
        GetComponent<Image>().sprite = Sprite.Create(_textureCurve, new Rect(0, 0, _textureCurve.width, _textureCurve.height), new Vector2(0.5f, 0.5f));
        UpdateKnobPositions();
        //Debug.Log("ich: " + this.name);
    }
    public void UpdateKnobPositions()
    {

        foreach (Image image in _imagePositionKnobCollection)
        {
            Destroy(image.gameObject);
        }
        _imagePositionKnobCollection.Clear();

        //float panelWidth = GetComponent<RectTransform>().rect.width;
        float panelHeight = GetComponent<RectTransform>().rect.height;

        foreach (RailElementSpeed railElementSpeed in StaticSceneData.StaticData.railElements[_railIndex].railElementSpeeds)
        {
            Image knobInstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
            knobInstance.gameObject.SetActive(true);
            float knobPosX = UtilitiesTm.FloatRemap(railElementSpeed.moment, 0, _maxTime, 0, _rectWidth);
            float knobPosY = UtilitiesTm.FloatRemap(railElementSpeed.speed, _minValue, _maxValue, -panelHeight / 2, panelHeight / 2);
            knobInstance.transform.localPosition = new Vector3(knobPosX, knobPosY, knobInstance.transform.localPosition.z);
            _imagePositionKnobCollection.Add(knobInstance);
        }


    }
}
