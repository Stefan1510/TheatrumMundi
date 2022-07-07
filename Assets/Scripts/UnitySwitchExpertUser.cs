using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitySwitchExpertUser : MonoBehaviour
{
    [SerializeField] private bool _isExpert =true;
    ////////////////////////////////////////////////////
    /// Config Menü
    ////////////////////////////////////////////////////
    [SerializeField] private GameObject _panelLightAmbient;
    [SerializeField] private GameObject _buttonBuehne;
    [SerializeField] private GameObject _menueBuehne;
    [SerializeField] private GameObject _buttonLicht;
    [SerializeField] private GameObject _menueLicht;

    [SerializeField] private GameObject _buttonKulissen;
    [SerializeField] private GameObject _menueKulissen;
    [SerializeField] private GameObject _buttonLadenSpeichern;

    [SerializeField] private GameObject _buttonDelete;
    [SerializeField] private GameObject _buttonSave;
    [SerializeField] private GameObject _textSave;
    [SerializeField] private GameObject[] _aInputFieldSave;
    [SerializeField] private GameObject _buttonLoad;
    [SerializeField] private GameObject _scrollViewFileSelect;


    ////////////////////////////////////////////////////
    /// Director Menü
    ////////////////////////////////////////////////////
    [SerializeField] private GameObject _menuLightShelf;
    [SerializeField] private GameObject _buttonLights;

    [SerializeField] private GameObject[] _aTimeSliderPanelControls;


    private bool _gameControllerStarted = false;         

    // Start is called before the first frame update
    void Start()
    {
        _gameControllerStarted = true;
        if (!_isExpert)
        {
            _buttonKulissen.GetComponent<RectTransform>().Translate(0, 137, 0);
            _buttonLadenSpeichern.GetComponent<RectTransform>().Translate(0, 2 * 137, 0);
            _panelLightAmbient.SetActive(false);
            _buttonDelete.SetActive(false);
            _buttonSave.SetActive(false);
            _textSave.SetActive(false);
            foreach(GameObject inputFieldSave in _aInputFieldSave)
            {
                inputFieldSave.SetActive(false);
            }
            _menueKulissen.SetActive(true);
            _buttonLoad.GetComponent<RectTransform>().Translate(0, -300, 0);
            RectTransform rectScrollView = _scrollViewFileSelect.GetComponent<RectTransform>();
            rectScrollView.sizeDelta = new Vector2(rectScrollView.sizeDelta.x, rectScrollView.sizeDelta.y + 300);
            foreach (GameObject panelControl in _aTimeSliderPanelControls)
            {
                panelControl.SetActive(false);
            }
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
            _buttonBuehne.SetActive(false);
            _menueBuehne.SetActive(false);
            _buttonLicht.SetActive(false);
            _menueLicht.SetActive(false);
            _menuLightShelf.SetActive(false);
            _buttonLights.SetActive(false);
        }
    }
}
