using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    [SerializeField] GameObject _dialogSaveBeforeExit;
    [SerializeField] SaveFileController tmpSave;
    RectTransform trans;

    void Start()
    {
        trans = GetComponent<RectTransform>();
    }
    void Update()
    {
        if (Input.mousePosition.y > Screen.height * 0.95f)
        {
            trans.position = new Vector2(trans.position.x, Screen.height - Screen.height * 0.02f);
            trans.sizeDelta = new Vector2(190, trans.sizeDelta.y);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            trans.position = new Vector2(trans.position.x, Screen.height * 1.01f);
            trans.sizeDelta = new Vector2(52, trans.sizeDelta.y);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    public void OnClickExitButton()
    {
        _dialogSaveBeforeExit.SetActive(true);
    }
    public void OnExit()
    {
        Application.Quit();
    }
    public void OnCancelExit()
    {
        _dialogSaveBeforeExit.SetActive(false);
    }
    public void OnSave()
    {
        SceneManaging.saveQuit = true;
        tmpSave.SaveSceneToFile(0);
        Application.Quit();
    }
}
