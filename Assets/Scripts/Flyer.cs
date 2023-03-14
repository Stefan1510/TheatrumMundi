using UnityEngine;
using UnityEngine.UI;

public class Flyer : MonoBehaviour
{
    [SerializeField] ObjectShelfAll objShelfAll;
    [SerializeField] GameObject warningPanel;
    [SerializeField] SaveFileController tmpFileController;

    void Start()
    {
        for (int i = 0; i < SceneManaging.flyerSpace.Length; i++)
        {
            SceneManaging.flyerSpace[i] = -1;
        }
    }

    public void OnClickStartFlyer()
    {
        // Warning current scene will be deleted
        warningPanel.SetActive(true);

    }
    public void ClickCancel()
    {
        SceneManaging.flyerActive = false;
        gameObject.SetActive(false);
    }
    public void OnClickFillScene()
    {

    }
    public void OnClickWarning(bool cancel)
    {
        //back to normal mode
        if (cancel)
        {
            warningPanel.SetActive(false);
        }

        // open flyer
        else
        {
            // disable 'figures in schiene ziehen' in Rail Manager
            SceneManaging.flyerActive = true;

            // load new scene
            gameObject.SetActive(true);
            objShelfAll.ButtonShelf05(false);
            warningPanel.SetActive(false);

            StartCoroutine(tmpFileController.LoadFileFromWWW("*Musterszene_leer.json", false));
        }
    }
}
