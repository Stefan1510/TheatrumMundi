using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveRailsAbstand12_34 : MonoBehaviour
{
    [SerializeField] private Slider _sliderAbstand12_34;
    [SerializeField] private InputField _inputFieldAbstand12_34;

    private GameObject _objectSchiene3;
    private float _startDistSchiene3;

    // Start is called before the first frame update
    void Start()
    {
        _objectSchiene3 = GameObject.Find("Schien3");
        _startDistSchiene3 = _objectSchiene3.transform.position.z;
        _sliderAbstand12_34.onValueChanged.AddListener(ChangeDistanceSchiene3Slider);
        _inputFieldAbstand12_34.onEndEdit.AddListener(ChangeDistanceSchiene3InputField);
    }

    void ChangeDistanceSchiene3Slider(float val)
    {
        _inputFieldAbstand12_34.text = val.ToString("0.00");
        Vector3 pos = _objectSchiene3.transform.position;
        float baseDist = GameObject.Find("Schien2").transform.position.z;
        _objectSchiene3.transform.position = new Vector3(pos.x, pos.y, _startDistSchiene3 + baseDist + val);
    }

    void ChangeDistanceSchiene3InputField(string str)
    {
        float val;
        if (float.TryParse(str, out val))
        {
            if (val < 0f)
                val = 0f;
            if (val > 1f)
                val = 1f;
            _sliderAbstand12_34.value = val;
            ChangeDistanceSchiene3Slider(val);
        }
        else
        {
            _inputFieldAbstand12_34.text = _sliderAbstand12_34.value.ToString("0.00");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
