using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveRailsAbstandEinsZwei : MonoBehaviour
{
    [SerializeField] private Slider _sliderAbstandEinsZwei;
    [SerializeField] private InputField _inputFieldAbstandEinsZwei;

    private GameObject _objectSchiene2;
    private float _startDistSchiene2;

    // Start is called before the first frame update
    void Start()
    {
        _objectSchiene2 = GameObject.Find("Schiene2");
        _startDistSchiene2 = _objectSchiene2.transform.position.z;
        _sliderAbstandEinsZwei.onValueChanged.AddListener(ChangeDistanceSchiene2Slider);
        _inputFieldAbstandEinsZwei.onEndEdit.AddListener(ChangeDistanceSchiene2InputField);
    }

    void ChangeDistanceSchiene2Slider(float val)
    {
        _inputFieldAbstandEinsZwei.text = val.ToString("0.00");
        Vector3 pos = _objectSchiene2.transform.position;
        _objectSchiene2.transform.position = new Vector3(pos.x, pos.y, _startDistSchiene2 + val);
    }

    void ChangeDistanceSchiene2InputField (string str)
    {
        float val;
        if (float.TryParse(str, out val))
        {
            if (val < 0f)
                val = 0f;
            if (val > 1f)
                val = 1f;
            _sliderAbstandEinsZwei.value = val;
            ChangeDistanceSchiene2Slider(val);
        }
        else
        {
            _inputFieldAbstandEinsZwei.text = _sliderAbstandEinsZwei.value.ToString("0.00");
        }
    }
}
