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
    //private Color32[] _lightColors;
    //private Color32[] _lightColorsColumn;
    private float _maxTime;
    private float _maxIntensity;
    private bool _gameObjectStarted = false;
    // Start is called before the first frame update

    // Start is called before the first frame update
    void Start()
    {
        _gameObjectStarted = true;
        _imagePositionKnob.gameObject.SetActive(false);
        _imagePositionKnobCollection = new List<Image>();
    }

    private void OnEnable()
    {
        if (_gameObjectStarted)
        {
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
        float maxTime = AnimationTimer.GetMaxTime();
        float panelHeight = GetComponent<RectTransform>().rect.height;
        float minIntensity = _valueSlider.minValue;
        float maxIntensity = _valueSlider.maxValue;
        foreach(RailElementSpeed railElementSpeed in StaticSceneData.StaticData.railElements[3].railElementSpeeds)
        {
            Image knobInstance = Instantiate(_imagePositionKnob, _imagePositionKnob.transform.parent);
            knobInstance.gameObject.SetActive(true);
            float knobPosX = UtilitiesTm.FloatRemap(railElementSpeed.moment, 0, maxTime, 0, panelWidth);
            float knobPosY = UtilitiesTm.FloatRemap(railElementSpeed.speed, minIntensity, maxIntensity, -panelHeight / 2, panelHeight / 2);
            knobInstance.transform.localPosition = new Vector3(knobPosX, knobPosY, knobInstance.transform.localPosition.z);
            _imagePositionKnobCollection.Add(knobInstance);
        }


    }
}
