using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTJ.FrameCapturer;

public class UnitySwitchExpertUser : MonoBehaviour
{
    public bool _isExpert;
    [SerializeField] private GameObject _buttonTimelineLength;

    #region Config Menue
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
    [SerializeField] private GameObject _buttonClose;
    [SerializeField] private Light backgroundLight;
    [SerializeField] private GameObject titleCoulisses, titleFigures, titleMusic;

    [SerializeField] private GameObject _buttonMenueDirector, _buttonMenueConfig, _buttonHelp, _buttonHelpPressed, _buttonFlyer, _buttonMovie;
    [SerializeField] private GameObject flyerButtonTop;
    [SerializeField] private GameObject bgBrownImage;
    [SerializeField] private MovieRecorder mov;
    #endregion

    #region Director Menue
    [SerializeField] private GameObject _imageTimelineRailMusic;
    [SerializeField] private BoxCollider2D _imageTimelineBg;
    [SerializeField] private GameObject[] counter;
    [SerializeField] private GameObject[] _timesLettersBig;
    [SerializeField] private GameObject _buttonLadenSpeichernDir;
    #endregion
    private bool _gameControllerStarted = false;

    private void Awake()
    {
        SceneManaging.isExpert = _isExpert;
        if (!_isExpert)
        {
            titleCoulisses.SetActive(false);
            titleFigures.SetActive(false);
            titleMusic.SetActive(false);
            // backgroundLight.transform.eulerAngles = new Vector3(25, backgroundLight.transform.eulerAngles.y, backgroundLight.transform.eulerAngles.z);
            //RenderSettings.ambientLight = new Color(1.86f,1.8f,1.28f);
            // RenderSettings.ambientIntensity = 1;
            // RenderSettings.am

            _buttonClose.SetActive(false);
            mov.enabled = false;

            _buttonTimelineLength.SetActive(false);

            _buttonKulissen.transform.SetParent(_buttonKulissen.transform.parent.parent);
            _buttonKulissen.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 465);
            _buttonKulissen.transform.SetSiblingIndex(2);

            _buttonFigures.transform.SetParent(_buttonFigures.transform.parent.parent);
            _buttonFigures.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 315);
            _buttonFigures.transform.SetSiblingIndex(2);

            _buttonMusic.transform.SetParent(_buttonMusic.transform.parent.parent);
            _buttonMusic.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 165);
            _buttonMusic.transform.SetSiblingIndex(2);

            _buttonLadenSpeichern.SetActive(false);
            _buttonLadenSpeichernDir.SetActive(false);

            _buttonMenueDirector.transform.GetChild(0).GetComponent<Image>().enabled = false; // switch button ausgrauen
            _buttonMenueDirector.GetComponent<Button>().enabled = false;

            _buttonMenueConfig.SetActive(false);

            _buttonHelp.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonHelp.GetComponent<RectTransform>().anchoredPosition.x, 465);
            _buttonHelpPressed.GetComponent<RectTransform>().anchoredPosition = _buttonHelp.GetComponent<RectTransform>().anchoredPosition;
            _buttonAbout.gameObject.SetActive(true);
            _buttonAbout.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonAbout.GetComponent<RectTransform>().anchoredPosition.x, 315);
            _buttonAbout.transform.GetChild(0).gameObject.SetActive(true);
            _buttonMenueDirector.SetActive(false);
            _buttonFlyer.SetActive(true);
            _buttonFlyer.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonFlyer.GetComponent<RectTransform>().anchoredPosition.x, 14);
            flyerButtonTop.SetActive(true);

            _buttonMovie.SetActive(true);
            _buttonDummy.SetActive(false);
            bgBrownImage.SetActive(true);
            _imageTimelineBg.enabled = false;

            SceneManaging.configMenueActive = 2;

            _imageTimelineRailMusic.transform.SetSiblingIndex(6);

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
        else
        {
            _buttonHelp.GetComponent<PressHelp>().ClickOkayOnStartOverlay();
        }
    }
    void Start()
    {
        _gameControllerStarted = true;
        if (!_isExpert)
        {
            _buttonKulissen.transform.GetChild(1).gameObject.SetActive(false);
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
        }
    }
}
