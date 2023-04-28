using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitySwitchExpertUser : MonoBehaviour
{
    [SerializeField] public bool _isExpert = true;
    ////////////////////////////////////////////////////
    /// Config Men�
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
    [SerializeField] private GameObject _buttonDummy; // leeres dummy

    [SerializeField] private GameObject _buttonMenueDirector, _buttonMenueConfig, _buttonHelp, _buttonHelpPressed, _buttonAboutWebGL, _buttonSend, _buttonMovie;

    [SerializeField] private GameObject _buttonDelete;
    [SerializeField] private GameObject _buttonSave;
    [SerializeField] private GameObject _textSave;
    [SerializeField] private GameObject[] _aInputFieldSave;
    [SerializeField] private GameObject _buttonLoad;
    [SerializeField] private GameObject _scrollViewFileSelect;



    ////////////////////////////////////////////////////
    /// Director Men�
    ////////////////////////////////////////////////////
    [SerializeField] private GameObject _menuLightShelf;
    [SerializeField] private GameObject _buttonLights;

    [SerializeField] private GameObject[] _aTimeSliderPanelControls;
    [SerializeField] private GameObject[] _railImages;
    [SerializeField] private GameObject _imageTimelineRailBg;
    [SerializeField] private GameObject _imageTimelineRailLight;
    [SerializeField] private GameObject _imageTimelineRailMusic;
    //[SerializeField] private GameObject[] _aImageTimeSliderSettings;
    [SerializeField] private GameObject[] counter;
    [SerializeField] private GameObject[] _timesLettersBig;

    private bool _gameControllerStarted = false;

    private void Awake()
    {
        SceneManaging.isExpert = _isExpert;
        _buttonAboutWebGL.gameObject.SetActive(false);
        if (!_isExpert)
        {
            _buttonKulissen.transform.SetParent(_buttonKulissen.transform.parent.parent);
            _buttonKulissen.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 465);
            _buttonKulissen.transform.SetSiblingIndex(5);

            _buttonFigures.transform.SetParent(_buttonFigures.transform.parent.parent);
            _buttonFigures.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 315);
            _buttonFigures.transform.SetSiblingIndex(5);

            _buttonMusic.transform.SetParent(_buttonMusic.transform.parent.parent);
            _buttonMusic.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 165);
            _buttonMusic.transform.SetSiblingIndex(5);

            _buttonLadenSpeichern.SetActive(false);
            // _buttonLadenSpeichern.transform.SetParent(_buttonLadenSpeichern.transform.parent.parent);
            // _buttonLadenSpeichern.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 15);
            // _buttonLadenSpeichern.transform.SetSiblingIndex(5);

            _buttonMenueDirector.transform.GetChild(0).GetComponent<Image>().enabled = false; // switch button ausgrauen
            _buttonMenueDirector.GetComponent<Button>().enabled = false;

            _buttonMenueConfig.SetActive(false);

            _buttonHelp.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonHelp.GetComponent<RectTransform>().anchoredPosition.x, 465);
            _buttonHelpPressed.GetComponent<RectTransform>().anchoredPosition = _buttonHelp.GetComponent<RectTransform>().anchoredPosition;
            _buttonAboutWebGL.gameObject.SetActive(true);
            _buttonAboutWebGL.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonAboutWebGL.GetComponent<RectTransform>().anchoredPosition.x, 315);
            //_buttonMenueDirector.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonMenueDirector.GetComponent<RectTransform>().anchoredPosition.x, 165);
            _buttonMenueDirector.SetActive(false);
            _buttonSend.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonSend.GetComponent<RectTransform>().anchoredPosition.x, 15);
            _buttonSend.transform.GetChild(0).gameObject.SetActive(false);
            //_buttonMovie.transform.GetChild(0).gameObject.SetActive(false);
            //_panelLightAmbient.SetActive(false);
            //_buttonDelete.SetActive(false);
            //_buttonSave.SetActive(false);
            _buttonDummy.SetActive(false);
            //_textSave.SetActive(false);
            //foreach (GameObject inputFieldSave in _aInputFieldSave)
            //{
            //inputFieldSave.SetActive(false);
            //}
            //_buttonLoad.GetComponent<RectTransform>().Translate(0, -300, 0);

            // RectTransform rectScrollView = _scrollViewFileSelect.GetComponent<RectTransform>();
            // rectScrollView.sizeDelta = new Vector2(rectScrollView.sizeDelta.x, rectScrollView.sizeDelta.y + 300);
            // foreach (GameObject panelControl in _aTimeSliderPanelControls)
            // {
            //     panelControl.SetActive(false);
            // }
            //_imageTimelineRailBg.SetActive(false);
            //_imageTimelineRailBg.GetComponent<Image>().color = Color.gray;
            //_imageTimelineRailBg.GetComponent<RailLightManager>().enabled = false;
            //_imageTimelineRailBg.GetComponent<BoxCollider2D>().enabled = false;
            //_imageTimelineRailLight.SetActive(false);
            _imageTimelineRailMusic.transform.SetSiblingIndex(6);
            // foreach (GameObject imageTimeSliderSetting in _aImageTimeSliderSettings)
            // {
            // imageTimeSliderSetting.SetActive(false);
            // }

            // counter ausblenden (figures + music)
            for (int i = 0; i < counter.Length; i++)
            {
                counter[i].SetActive(false);
            }
            // start und endzeit auf timeline ausblenden
            for (int i = 0; i < _timesLettersBig.Length; i++)
            {
                _timesLettersBig[i].SetActive(false);
            }
        }
    }


    void Start()
    {
        _gameControllerStarted = true;
        if (!_isExpert)
        {
            //this.GetComponent<SaveFileController>().LoadSceneFromFile("*Musterszene_leer.json", true);
            //_menueKulissen.SetActive(true);
            _buttonKulissen.transform.GetChild(1).gameObject.SetActive(false);
            // for (int j = 0; j < this.GetComponent<SceneDataController>().objectsLightElements.Length; j++)
            // {
            //     this.GetComponent<SceneDataController>().objectsLightElements[j].goLightElement.GetComponent<Light>().enabled = true;
            // }
        }
    }

    void Update()
    {
        if (_gameControllerStarted)
        {
            DeactivateExpertTools();
            if (!_isExpert)
            {
                _gameControllerStarted = false;
                _menueKulissen.SetActive(true);
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
            // _menuLightShelf.SetActive(false);
            // _buttonLights.SetActive(false);
            _buttonAbout.SetActive(false);
            // for (int j = 0; j < this.GetComponent<SceneDataController>().objectsLightElements.Length; j++)
            // {
            //     this.GetComponent<SceneDataController>().objectsLightElements[j].goLightElement.GetComponent<Light>().enabled = true;
            // }
        }
    }
}
