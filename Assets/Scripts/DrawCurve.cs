using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCurve : MonoBehaviour
{
    private Texture2D _textureCurve;
    [SerializeField] private Slider _valueSlider;
    [SerializeField] private Image _imagePositionKnob;
    //[SerializeField] private GameObject _representationPanel;
    private List<Image> _imagePositionKnobCollection;
    private Color32[] _backColors;
    private Color32[] _curveColors;
    private float _maxTime;
    private float _minValue;
    private float _maxValue;
    private bool _gameObjectStarted = false;
    // Start is called before the first frame update

    // Start is called before the first frame update
    void Start()
    {
        _gameObjectStarted = true;
        _textureCurve = new Texture2D((int)(GetComponent<RectTransform>().rect.width), (int)(GetComponent<RectTransform>().rect.height), TextureFormat.RGBA32, false); // wird durch Panel RectTransform stretch automatisch gescaled
        _backColors = new Color32[_textureCurve.width * _textureCurve.height];
        _backColors = UtilitiesTm.ChangeColors(_backColors, new Color32(255, 255, 255, 255));
        _curveColors = new Color32[_textureCurve.width * _textureCurve.height];
        _curveColors = UtilitiesTm.ChangeColors(_curveColors, Color.black);
        _maxTime = AnimationTimer.GetMaxTime();
        _minValue = _valueSlider.minValue;
        _maxValue = _valueSlider.maxValue;
        _imagePositionKnob.gameObject.SetActive(false);
        _imagePositionKnobCollection = new List<Image>();
    }

    private void OnEnable()
    {
        if (_gameObjectStarted)
        {
            StaticSceneData.StaticData.railElements[3].railElementSpeeds[0] = new RailElementSpeed { moment = 0, speed = _valueSlider.value };  // im SceneDataController MUSS ein erstes Element hinzugefügt werden, bevor es hier angesprochen werden kann
            StaticSceneData.StaticData.railElements[3].railElementSpeeds.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die railElementSpeeds anhand der Eigenschaft moment
            ChangeCurve();
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void AddValue()      // Aufruf auf Slider, der etwas verändern soll, !ACHTUNG! neue Komponente zum Slider --> EventTrigger --> Pointer Up, darauf dann diese Funktion, das Skript dazu liegt auf dem Panel
    {
        float valueSliderValue = _valueSlider.value;
        float valueMoment = AnimationTimer.GetTime();
        int momentIndex = StaticSceneData.StaticData.railElements[3].railElementSpeeds.FindIndex(mom => mom.moment == valueMoment);     //hier spezifisches RailElement, das kann dann entsprechend geändert werden

        RailElementSpeed thisRailElementSpeed = new RailElementSpeed();
        Debug.Log(StaticSceneData.StaticData.railElements[3].name);

        if (momentIndex == -1)
        {
            thisRailElementSpeed = new RailElementSpeed { moment = valueMoment, speed = valueSliderValue };
            StaticSceneData.StaticData.railElements[3].railElementSpeeds.Add(thisRailElementSpeed);     //hier spezifisches RailElement, das kann dann entsprechend geändert werden
        }
        else
        {
            StaticSceneData.StaticData.railElements[3].railElementSpeeds[momentIndex] = new RailElementSpeed { moment = valueMoment, speed = valueSliderValue };
        }
        StaticSceneData.StaticData.railElements[3].railElementSpeeds.Sort((x, y) => x.moment.CompareTo(y.moment));   // sortiert die railElementSpeeds anhand der Eigenschaft moment

        ChangeCurve();
    }

    public void ChangeCurve()
    {
        _maxTime = AnimationTimer.GetMaxTime();
        _textureCurve.SetPixels32(_backColors);

        int momentStart = (int)StaticSceneData.StaticData.railElements[3].railElementSpeeds[0].moment;
        int momentEnd = (int)StaticSceneData.StaticData.railElements[3].railElementSpeeds[0].moment;

        int valueStart = (int)StaticSceneData.StaticData.railElements[3].railElementSpeeds[0].speed;
        int valueEnd = (int)StaticSceneData.StaticData.railElements[3].railElementSpeeds[0].speed;

        int listLength = StaticSceneData.StaticData.railElements[3].railElementSpeeds.Count;
        for (int valueStates = 0; valueStates < listLength - 1; valueStates++)
        {
            float momentStartF = (int)StaticSceneData.StaticData.railElements[3].railElementSpeeds[valueStates].moment; // holen des "früheren" Moments aus der Datenhaltung
            momentStartF = UtilitiesTm.FloatRemap(momentStartF, 0, _maxTime, 0, _textureCurve.width-3);    // mappen des "früheren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentStart = (int)momentStartF;

            float momentEndF = (int)StaticSceneData.StaticData.railElements[3].railElementSpeeds[valueStates + 1].moment; // holen des "späteren" Moments aus der Datenhaltung
            momentEndF = UtilitiesTm.FloatRemap(momentEndF, 0, _maxTime, 0, _textureCurve.width-3); // mappen des "späteren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            momentEnd = (int)momentEndF;

            float valueStartF = StaticSceneData.StaticData.railElements[3].railElementSpeeds[valueStates].speed;  // holen des "früheren" Werts aus der Datenhaltung
            valueStartF = UtilitiesTm.FloatRemap(valueStartF, _minValue, _maxValue, 0, _textureCurve.height-3);   // mappen des "früheren" Werts von zwischen ValueSlider auf zwischen GraphicHoehe
            valueStart = (int)valueStartF;

            float valueEndF = StaticSceneData.StaticData.railElements[3].railElementSpeeds[valueStates + 1].speed;    // holen des "späteren" Werts aus der Datenhaltung
            valueEndF = UtilitiesTm.FloatRemap(valueEndF, _minValue, _maxValue, 0, _textureCurve.height-3);    // mappen des "früheren" Moments von zwischen TimeSlider auf zwischen PanelWeite
            valueEnd = (int)valueEndF;

            _textureCurve = UtilitiesTm.Bresenham(_textureCurve, momentStart, valueStart, momentEnd, valueEnd, _curveColors);

            float deltaMoment = momentEndF - momentStartF;  // deltaX
            float deltaValue = valueEndF - valueStartF;     // deltaY
            Debug.Log("Deltas: x: " + deltaMoment + " // y: " + deltaValue);
            //if (Mathf.Abs(deltaMoment)>=Mathf.Abs(deltaValue))  // y ist kleinergleich x
            //{

            //}
            //else    // x ist kleiner als y
            //{
            //    int step = (int)(Mathf.Abs(deltaValue) / Mathf.Abs(deltaMoment));
            //    for (int i = 0; i < deltaMoment; i++)
            //    {
            //        Debug.Log("ACHTUNG ACHTUNG HIER IST EIN i" + i + "und ein Wert: " + deltaValue / deltaMoment);
            //        _textureCurve.SetPixels32((int)(momentStartF + i), (int)(valueStartF + i * (Mathf.Abs(deltaValue) / Mathf.Abs(deltaMoment))), 3, (int)(Mathf.Abs(deltaValue) / Mathf.Abs(deltaMoment))+1, _curveColors);
            //    }
            //}
        }


        _textureCurve.Apply();
        GetComponent<Image>().sprite = Sprite.Create(_textureCurve, new Rect(0, 0, _textureCurve.width, _textureCurve.height), new Vector2(0.5f, 0.5f));
        UpdateKnobPositions();
    }

    public void UpdateKnobPositions()
    {

        foreach (Image image in _imagePositionKnobCollection)
        {
            Destroy(image.gameObject);
        }
        _imagePositionKnobCollection.Clear();

        float panelWidth = GetComponent<RectTransform>().rect.width;
        float panelHeight = GetComponent<RectTransform>().rect.height;

        foreach(RailElementSpeed railElementSpeed in StaticSceneData.StaticData.railElements[3].railElementSpeeds)
        {
            Image knobInstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
            knobInstance.gameObject.SetActive(true);
            float knobPosX = UtilitiesTm.FloatRemap(railElementSpeed.moment, 0, _maxTime, 0, panelWidth);
            float knobPosY = UtilitiesTm.FloatRemap(railElementSpeed.speed, _minValue, _maxValue, -panelHeight / 2, panelHeight / 2);
            knobInstance.transform.localPosition = new Vector3(knobPosX, knobPosY, knobInstance.transform.localPosition.z);
            _imagePositionKnobCollection.Add(knobInstance);
        }


    }
}
