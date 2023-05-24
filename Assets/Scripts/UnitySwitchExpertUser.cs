using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitySwitchExpertUser : MonoBehaviour
{
    public bool _isExpert = true;
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

    [SerializeField] private GameObject _buttonMenueDirector, _buttonMenueConfig, _buttonHelp, _buttonHelpPressed, _buttonAboutWebGL, _buttonFlyer, _buttonMovie;
    [SerializeField] private GameObject bgBrownImage;
    #endregion

    #region Director Menue
    [SerializeField] private GameObject _imageTimelineRailMusic;
    [SerializeField] private BoxCollider2D _imageTimelineBg;
    [SerializeField] private GameObject[] counter;
    [SerializeField] private GameObject[] _timesLettersBig;
    #endregion
    private bool _gameControllerStarted = false;

    private void Awake()
    {
        SceneManaging.isExpert = _isExpert;
        if (!_isExpert)
        {
            _buttonTimelineLength.SetActive(false);
            _buttonAboutWebGL.gameObject.SetActive(false);

            _buttonKulissen.transform.SetParent(_buttonKulissen.transform.parent.parent);
            _buttonKulissen.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 465);
            _buttonKulissen.transform.SetSiblingIndex(3);

            _buttonFigures.transform.SetParent(_buttonFigures.transform.parent.parent);
            _buttonFigures.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 315);
            _buttonFigures.transform.SetSiblingIndex(3);

            _buttonMusic.transform.SetParent(_buttonMusic.transform.parent.parent);
            _buttonMusic.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonKulissen.GetComponent<RectTransform>().anchoredPosition.x, 165);
            _buttonMusic.transform.SetSiblingIndex(3);

            _buttonLadenSpeichern.SetActive(false);

            _buttonMenueDirector.transform.GetChild(0).GetComponent<Image>().enabled = false; // switch button ausgrauen
            _buttonMenueDirector.GetComponent<Button>().enabled = false;

            _buttonMenueConfig.SetActive(false);

            _buttonHelp.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonHelp.GetComponent<RectTransform>().anchoredPosition.x, 465);
            _buttonHelpPressed.GetComponent<RectTransform>().anchoredPosition = _buttonHelp.GetComponent<RectTransform>().anchoredPosition;
            _buttonAboutWebGL.gameObject.SetActive(true);
            _buttonAboutWebGL.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonAboutWebGL.GetComponent<RectTransform>().anchoredPosition.x, 315);
            _buttonAboutWebGL.transform.GetChild(0).gameObject.SetActive(true);
            _buttonMenueDirector.SetActive(false);
            _buttonFlyer.SetActive(true);
            _buttonFlyer.GetComponent<RectTransform>().anchoredPosition = new Vector2(_buttonFlyer.GetComponent<RectTransform>().anchoredPosition.x, 14);

            _buttonMovie.SetActive(true);
            _buttonDummy.SetActive(false);
            bgBrownImage.SetActive(true);
            _imageTimelineBg.enabled = false;
            
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
            _buttonAbout.SetActive(false);
        }
    }
}
