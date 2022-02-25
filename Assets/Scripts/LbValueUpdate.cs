using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LbValueUpdate : MonoBehaviour
{
    public Slider lbSlider;
    public InputField lbInputField;
    // Start is called before the first frame update
    void Start()
    {
        lbInputField.onEndEdit.AddListener(OnInputFieldChanged);
        lbSlider.onValueChanged.AddListener(OnSliderChanged);
        OnSliderChanged(lbSlider.value);
    }

    private void OnInputFieldChanged(string newText)
    {
        if (float.TryParse(newText, out var value))
        {
            value = Mathf.Clamp(value, lbSlider.minValue, lbSlider.maxValue);

            lbInputField.text = value.ToString("F");
            lbSlider.value = value;
        }
        else
        {
            Debug.LogWarning("Input Format Error!", this);
            lbSlider.value = Mathf.Clamp(0, lbSlider.minValue, lbSlider.maxValue);
            lbInputField.text = lbSlider.value.ToString("F");
        }
    }

    private void OnSliderChanged(float newValue)
    {
        lbInputField.text = newValue.ToString("F");
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
