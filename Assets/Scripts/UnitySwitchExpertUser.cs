using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitySwitchExpertUser : MonoBehaviour
{
    [SerializeField] private bool _isExpert = true;
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
    [SerializeField] private GameObject _buttonDummy;

    [SerializeField] private GameObject _buttonMenueDirector;

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

    [SerializeField] private GameObject _imageTimelineRailBg;
    [SerializeField] private GameObject _imageTimelineRailLight;
    [SerializeField] private GameObject[] _aImageTimeSliderSettings;


    private bool _gameControllerStarted = false;

    private void Awake()
    {
        SceneManaging.isExpert = _isExpert;
        if (!_isExpert)
        {
            _buttonKulissen.GetComponent<RectTransform>().Translate(0, 150, 0);
            _buttonKulissen.transform.SetParent(_buttonKulissen.transform.parent.parent);
            _buttonKulissen.transform.SetSiblingIndex(5);

            _buttonFigures.GetComponent<RectTransform>().Translate(0, -150, 0);
            _buttonFigures.transform.SetParent(_buttonFigures.transform.parent.parent);
            _buttonFigures.transform.SetSiblingIndex(5);

            _buttonMusic.GetComponent<RectTransform>().Translate(0, -150, 0);
            _buttonMusic.transform.SetParent(_buttonMusic.transform.parent.parent); ;
            _buttonMusic.transform.SetSiblingIndex(5);

            _buttonLadenSpeichern.GetComponent<RectTransform>().Translate(0, 0 * 150, 0);
            _buttonLadenSpeichern.transform.SetParent(_buttonLadenSpeichern.transform.parent.parent); ;
            _buttonLadenSpeichern.transform.SetSiblingIndex(5);

            _buttonMenueDirector.transform.GetChild(0).GetComponent<Image>().enabled = false; // switch button ausgrauen
            _buttonMenueDirector.GetComponent<Button>().enabled = false;

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
            foreach (GameObject panelControl in _aTimeSliderPanelControls)
            {
                panelControl.SetActive(false);
            }
            //_imageTimelineRailBg.SetActive(false);
            _imageTimelineRailBg.GetComponent<Image>().color = Color.gray;
            //_imageTimelineRailBg.GetComponent<RailLightManager>().enabled = false;
            _imageTimelineRailBg.GetComponent<BoxCollider2D>().enabled = false;
            // _imageTimelineRailLight.SetActive(false);
            // foreach (GameObject imageTimeSliderSetting in _aImageTimeSliderSettings)
            // {
            //     imageTimeSliderSetting.SetActive(false);
            // }

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _gameControllerStarted = true;
        if (!_isExpert)
        {
            //this.GetComponent<SaveFileController>().LoadSceneFromFile("*Musterszene_leer.json", true);
            _menueKulissen.SetActive(true);
            _buttonKulissen.transform.GetChild(1).gameObject.SetActive(false);
            // for (int j = 0; j < this.GetComponent<SceneDataController>().objectsLightElements.Length; j++)
            // {
            //     this.GetComponent<SceneDataController>().objectsLightElements[j].goLightElement.GetComponent<Light>().enabled = true;
            // }
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
