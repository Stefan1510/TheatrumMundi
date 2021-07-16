using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveRailsY : MonoBehaviour
{

    [SerializeField] private InputField _inputField;
    [SerializeField] private Slider _slider;
    private Vector3 _startPosSchiene;
    private MyLibrary _myLibrary = new MyLibrary();

    // Start is called before the first frame update
    void Start()
    {
        _myLibrary.Test();

        _startPosSchiene = transform.localPosition;

    }

    public void ChangeValueSliderX(float val)
    {
        _inputField.text = val.ToString("0.00");
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(_startPosSchiene.x - val, pos.y, pos.z);

    }

    public void ChangeValueInputFieldX(string str)
    {
        float val = _myLibrary.ChangeSelectedValue(str, _slider, _inputField);
        ChangeValueSliderX(val);
    }

    public void ChangeValueSliderY(float val)
    {
        _inputField.text = val.ToString("0.00");
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, _startPosSchiene.y + val, pos.z);
    }

    public void ChangeValueInputFieldY(string str)
    {
        float val = _myLibrary.ChangeSelectedValue(str, _slider, _inputField);
        ChangeValueSliderY(val);
    }
}
