using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanelLoad : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Button _buttonIgnore;
    [SerializeField] private Button _buttonBack;
    private bool _ignore;
    private string _buttonClicked;

    private void Awake()
    {
        _buttonIgnore.onClick.AddListener(() => clickedIgnore());
        _buttonBack.onClick.AddListener(() => clickedBack());
        _ignore = false;
        _buttonClicked = null;
    }

    IEnumerator waitForButtonClick()
    {
        _buttonClicked = null;
        while (_buttonClicked==null)
        {
            yield return null;
        }
    }

    private void clickedIgnore()
    {
        gameObject.SetActive(false);
        _buttonClicked = "ignore";
    }

    private void clickedBack()
    {
        gameObject.SetActive(false);
        _buttonClicked = "back";
    }

    public IEnumerator GetIgnoreIEnum()
    {
        gameObject.SetActive(true);
        _ignore = false;
        yield return waitForButtonClick();
        Debug.LogWarning(_buttonClicked);
        if (_buttonClicked == "ignore")
        {
            Debug.LogWarning("_buttonClicked == ignore");
        }
        else if (_buttonClicked == "back")
        {
            Debug.LogWarning("_buttonClicked == back");
        }
        else
        {
            Debug.LogWarning("_buttonClicked == DDD:");
        }
    }

    public void GetIgnore()
    {
        StartCoroutine(GetIgnoreIEnum());
    }

    //public void SetIgnore(bool val)
    //{
    //    _ignore = val;
    //}

    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
