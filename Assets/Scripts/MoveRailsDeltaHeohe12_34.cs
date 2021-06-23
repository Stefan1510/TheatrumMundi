using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveRailsDeltaHeohe12_34 : MonoBehaviour
{
    [SerializeField] private Slider _sliderDeltaHeohe12_34;
    [SerializeField] private InputField _inputFieldDeltaHeohe12_34;

    private GameObject _objectSchiene3;

    // Start is called before the first frame update
    void Start()
    {
        _objectSchiene3 = GameObject.Find("Schiene3");
        _sliderDeltaHeohe12_34.onValueChanged.AddListener(ChangeHeightSchiene3Slider);
        _inputFieldDeltaHeohe12_34.onEndEdit.AddListener(ChangeHeightSchiene3InputField);
    }

    void ChangeHeightSchiene3Slider(float val)
    {
        _inputFieldDeltaHeohe12_34.text = val.ToString("0.00");
        Vector3 pos = _objectSchiene3.transform.position;
        float baseheight = GameObject.Find("Schiene1").transform.position.y;
        _objectSchiene3.transform.position = new Vector3(pos.x, baseheight + val, pos.z);
    }

    void ChangeHeightSchiene3InputField(string str)
    {

        float val;
        if (float.TryParse(str, out val))
        {
            if (val < 0f)
                val = 0f;
            if (val > 1f)
                val = 1f;
            _sliderDeltaHeohe12_34.value = val;
            ChangeHeightSchiene3Slider(val);

        }
        else
        {
            _inputFieldDeltaHeohe12_34.text = _sliderDeltaHeohe12_34.value.ToString("0.00");
        }
    }

}
