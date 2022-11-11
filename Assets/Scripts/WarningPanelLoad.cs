using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanelLoad : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Button _buttonIgnore;
    [SerializeField] private Button _buttonBack;
    [HideInInspector] public string buttonClicked;
    [HideInInspector] public bool isButtonClicked;

    private void Awake()
    {
        _buttonIgnore.onClick.AddListener(() => ClickedIgnore());
        _buttonBack.onClick.AddListener(() => ClickedBack());
        buttonClicked = "notnull";
    }

    public IEnumerator WaitForButtonClick()
    {
        buttonClicked = null;
        gameObject.SetActive(true);
        while (buttonClicked == null)
        {
            Debug.Log("was ist das?");
            yield return null;
        }
        isButtonClicked = true;
    }

    private void ClickedIgnore()
    {
        gameObject.SetActive(false);
        buttonClicked = "ignore";
    }

    private void ClickedBack()
    {
        gameObject.SetActive(false);
        buttonClicked = "back";
    }

    public IEnumerator GetIgnoreIEnum()
    {
        yield return WaitForButtonClick();
        Debug.LogWarning(buttonClicked);
        //if (buttonClicked == "ignore")
        //{
        //    Debug.LogWarning("_buttonClicked == ignore");
        //}
        //else if (buttonClicked == "back")
        //{
        //    Debug.LogWarning("_buttonClicked == back");
        //}
        //else
        //{
        //    Debug.LogWarning("_buttonClicked == DDD:");
        //}
    }

    public void GetIgnore()
    {
        gameObject.SetActive(true);
        StartCoroutine(GetIgnoreIEnum());
        //while (_buttonClicked==null)
        //{

        //}
        Debug.LogWarning("============== Boink ==============");
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
    //void Update()
    //{

    //}
}
