using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawCurveBg : MonoBehaviour
{
    private Texture2D _textureCurve;
    [SerializeField] private Slider _valueSlider;
    [SerializeField] private Image _imagePositionKnob;
    [SerializeField] private GameObject _PanelLineDraw;

    private List<Image> _imagePositionKnobCollection;
    private Color32[] _backColors;
    private Color32[] _curveColors;
    private float _maxTime;
    private float _minValue;
    private float _maxValue;
    private bool _gameObjectStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameObjectStarted = true;
        _textureCurve = new Texture2D((int)(_PanelLineDraw.GetComponent<RectTransform>().rect.width), (int)(_PanelLineDraw.GetComponent<RectTransform>().rect.height), TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch automatisch gescaled
        _backColors = new Color32[_textureCurve.width * _textureCurve.height];
        _backColors = UtilitiesTm.ChangeColors(_backColors, new Color32(255, 255, 255, 31));
        //_curveColors = new Color32[_textureCurve.width * _textureCurve.height];
        _curveColors = new Color32[3 * 3];
        _curveColors = UtilitiesTm.ChangeColors(_curveColors, Color.black);
        _maxTime = AnimationTimer.GetMaxTime();
        _minValue = _valueSlider.minValue;
        _maxValue = _valueSlider.maxValue;
        _imagePositionKnob.gameObject.SetActive(false);
        _imagePositionKnobCollection = new List<Image>();
        EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();
        eventTriggerEntry.eventID = EventTriggerType.PointerUp;
        eventTriggerEntry.callback.AddListener((data) => { AddValue((PointerEventData)data); });
        _valueSlider.GetComponent<EventTrigger>().triggers.Add(eventTriggerEntry);
    }

    private void OnEnable()
    {
        if (_gameObjectStarted)
        {
            StaticSceneData.StaticData.backgroundPositions[0] = new BackgroundPosition { moment = 0, yPosition = _valueSlider.value };  // im SceneDataController MUSS ein erstes Element hinzugefügt werden, bevor es hier angesprochen werden kann
            StaticSceneData.StaticData.backgroundPositions.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die railElementSpeeds anhand der Eigenschaft moment
            ChangeCurve();
        }
    }


    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void AddValue(PointerEventData data)      // Aufruf auf Slider, der etwas verändern soll, gelöst über EventTrigger, übergebene data unnötig
    {

        //Debug.LogError(data);
        float valueSliderValue = _valueSlider.value;
        float valueMoment = AnimationTimer.GetTime();
        int momentIndex = StaticSceneData.StaticData.backgroundPositions.FindIndex(bgPos => bgPos.moment == valueMoment);

        BackgroundPosition thisBackgroundPosition = new BackgroundPosition();

        if (momentIndex == -1)
        {
            thisBackgroundPosition = new BackgroundPosition { moment = valueMoment, yPosition = valueSliderValue };
            StaticSceneData.StaticData.backgroundPositions.Add(thisBackgroundPosition);
        }
        else
        {
            StaticSceneData.StaticData.backgroundPositions[momentIndex] = new BackgroundPosition { moment = valueMoment, yPosition = valueSliderValue };
        }
        StaticSceneData.StaticData.backgroundPositions.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die backgroundPositions anhand der Eigenschaft moment

        ChangeCurve();
    }

    public void ChangeCurve()
    {
        _maxTime = AnimationTimer.GetMaxTime();
        _textureCurve.SetPixels32(_backColors);
        //_textureCurve = UtilitiesTm.Bresenham(_textureCurve, 0, _textureCurve.height / 2, _textureCurve.width - 3, _textureCurve.height / 2, _middleLineColors);

        int momentStart = (int)StaticSceneData.StaticData.backgroundPositions[0].moment;
        int momentEnd = (int)StaticSceneData.StaticData.backgroundPositions[0].moment;

        int valueStart = (int)StaticSceneData.StaticData.backgroundPositions[0].yPosition;
        int valueEnd = (int)StaticSceneData.StaticData.backgroundPositions[0].yPosition;

        int listLength = StaticSceneData.StaticData.backgroundPositions.Count;
        for (int valueStates = 0; valueStates < listLength - 1; valueStates++)
        {
            float momentStartF = (int)StaticSceneData.StaticData.backgroundPositions[valueStates].moment; // holen des "früheren" Moments aus der Datenhaltung
            momentStartF = UtilitiesTm.FloatRemap(momentStartF, 0, _maxTime, 0, _textureCurve.width - 3);    // mappen des "früheren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentStart = (int)momentStartF;

            float momentEndF = (int)StaticSceneData.StaticData.backgroundPositions[valueStates + 1].moment; // holen des "späteren" Moments aus der Datenhaltung
            momentEndF = UtilitiesTm.FloatRemap(momentEndF, 0, _maxTime, 0, _textureCurve.width - 3); // mappen des "späteren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentEnd = (int)momentEndF;

            float valueStartF = StaticSceneData.StaticData.backgroundPositions[valueStates].yPosition;  // holen des "früheren" Werts aus der Datenhaltung
            valueStartF = UtilitiesTm.FloatRemap(valueStartF, _minValue, _maxValue, 0, _textureCurve.height - 3);   // mappen des "früheren" Werts von zwischen ValueSlider auf zwischen GraphicHoehe
            valueStart = (int)valueStartF;

            float valueEndF = StaticSceneData.StaticData.backgroundPositions[valueStates + 1].yPosition;    // holen des "späteren" Werts aus der Datenhaltung
            valueEndF = UtilitiesTm.FloatRemap(valueEndF, _minValue, _maxValue, 0, _textureCurve.height - 3);    // mappen des "früheren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            valueEnd = (int)valueEndF;

            _textureCurve = UtilitiesTm.Bresenham(_textureCurve, momentStart, valueStart, momentEnd, valueEnd, _curveColors);

            float deltaMoment = momentEndF - momentStartF;  // deltaX
            float deltaValue = valueEndF - valueStartF;     // deltaY
        }


        _textureCurve.Apply();
        _PanelLineDraw.GetComponent<Image>().sprite = Sprite.Create(_textureCurve, new Rect(0, 0, _textureCurve.width, _textureCurve.height), new Vector2(0.5f, 0.5f));
        UpdateKnobPositions();
    }

    public void UpdateKnobPositions()
    {
        foreach (Image image in _imagePositionKnobCollection)
        {
            Destroy(image.gameObject);
        }
        _imagePositionKnobCollection.Clear();

        float panelWidth = _PanelLineDraw.GetComponent<RectTransform>().rect.width;
        float panelHeight = _PanelLineDraw.GetComponent<RectTransform>().rect.height;

        foreach (BackgroundPosition backgroundPosition in StaticSceneData.StaticData.backgroundPositions)
        {
            Image knobInstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
            knobInstance.gameObject.SetActive(true);
            float knobPosX = UtilitiesTm.FloatRemap(backgroundPosition.moment, 0, _maxTime, 0, panelWidth);
            float knobPosY = UtilitiesTm.FloatRemap(backgroundPosition.yPosition, _minValue, _maxValue, -panelHeight / 2, panelHeight / 2);
            knobInstance.transform.localPosition = new Vector3(knobPosX, knobPosY, knobInstance.transform.localPosition.z);
            _imagePositionKnobCollection.Add(knobInstance);
        }
    }
}
