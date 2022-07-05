using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitySwitchExpertUser : MonoBehaviour
{
    [SerializeField] private bool _isExpert =true;
    [SerializeField] private GameObject _panelLightAmbient;
    [SerializeField] private GameObject _buttonBuehne;
    [SerializeField] private GameObject _menueBuehne;
    [SerializeField] private GameObject _buttonLicht;
    [SerializeField] private GameObject _menueLicht;

    [SerializeField] private GameObject _buttonKulissen;
    [SerializeField] private GameObject _menueKulissen;
    [SerializeField] private GameObject _buttonLadenSpeichern;


    private bool _gameControllerStarted = false;         

    // Start is called before the first frame update
    void Start()
    {
        _gameControllerStarted = true;
        if (!_isExpert)
        {
            _buttonKulissen.GetComponent<RectTransform>().Translate(0, 137, 0);
            _buttonLadenSpeichern.GetComponent<RectTransform>().Translate(0, 2 * 137, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameControllerStarted)
        {
            DeactivateExpertTools();
        }
    }

    public void DeactivateExpertTools()
    {
        if (!_isExpert)
        {
            _panelLightAmbient.SetActive(false);
            _buttonBuehne.SetActive(false);
            _menueBuehne.SetActive(false);
            _buttonLicht.SetActive(false);
            _menueLicht.SetActive(false);
            _menueKulissen.SetActive(true);
        }
    }
}
