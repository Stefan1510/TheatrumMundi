using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveRailsHoeheEins : MonoBehaviour
{
    [SerializeField] private Slider _sliderHoeheEins;
    [SerializeField] private InputField _inputFieldHoeheEins;

    private float _startHeightSchiene1;

    // Start is called before the first frame update
    void Start()
    {
        MyLibrary myLibrary = new MyLibrary();
        myLibrary.Test();

        _startHeightSchiene1 = transform.position.y;
    }
        
    public void ChangeHeightSchiene1Slider(float val)
    {
        _inputFieldHoeheEins.text = val.ToString("0.00");
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, _startHeightSchiene1 + val, pos.z);
    }

    public void ChangeHeightSchiene1InputField(string str)
    {
        float val;
        if (float.TryParse(str, out val))
        {
            if (val < 0f)
                val = 0f;
            if (val > 1f)
                val = 1f;
            _sliderHoeheEins.value = val;
            ChangeHeightSchiene1Slider(val);

        }
        else
        {
            _inputFieldHoeheEins.text = _sliderHoeheEins.value.ToString("0.00");
        }
    }
}
