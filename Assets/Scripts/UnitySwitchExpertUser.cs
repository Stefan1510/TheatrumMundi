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
    
    
    [SerializeField] private GameObject _buttonFigures;
    [SerializeField] private GameObject _buttonMusic;
    [SerializeField] private GameObject _buttonAbout;
    [SerializeField] private GameObject _buttonDummy;


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

    private void Awake()
    {
        if (!_isExpert)
        {
            _buttonKulissen.GetComponent<RectTransform>().Translate(0, 150, 0);
            _buttonKulissen.transform.SetParent(_buttonKulissen.transform.parent.parent);
            _buttonKulissen.transform.SetSiblingIndex(6);

            _buttonFigures.GetComponent<RectTransform>().Translate(0, -150, 0);
            _buttonFigures.transform.SetParent(_buttonFigures.transform.parent.parent);
            _buttonFigures.transform.SetSiblingIndex(6);

            _buttonMusic.GetComponent<RectTransform>().Translate(0, -150, 0);
            _buttonMusic.transform.SetParent(_buttonMusic.transform.parent.parent); ;
            _buttonMusic.transform.SetSiblingIndex(6);

            _buttonLadenSpeichern.GetComponent<RectTransform>().Translate(0, 0 * 150, 0);
            _buttonLadenSpeichern.transform.SetParent(_buttonLadenSpeichern.transform.parent.parent); ;
            _buttonLadenSpeichern.transform.SetSiblingIndex(6);

            _panelLightAmbient.SetActive(false);
            _buttonDelete.SetActive(false);
            _buttonSave.SetActive(false);
            _buttonDummy.SetActive(false);
            _textSave.SetActive(false);
            foreach(GameObject inputFieldSave in _aInputFieldSave)
            {
                inputFieldSave.SetActive(false);
            }
            _buttonLoad.GetComponent<RectTransform>().Translate(0, -300, 0);
            RectTransform rectScrollView = _scrollViewFileSelect.GetComponent<RectTransform>();
            rectScrollView.sizeDelta = new Vector2(rectScrollView.sizeDelta.x, rectScrollView.sizeDelta.y + 300);
            foreach (GameObject panelControl in _aTimeSliderPanelControls)
            {
                panelControl.SetActive(false);
            }
        }        
    }


    // Start is called before the first frame update
    void Start()
    {
        _gameControllerStarted = true;
        if (!_isExpert)
        {
            _menueKulissen.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameControllerStarted)
        {
            DeactivateExpertTools();
            if (!_isExpert)
            {
                _menueKulissen.SetActive(true);
                _gameControllerStarted = false;
            }
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
            _buttonAbout.SetActive(false);
        }
    }
}
