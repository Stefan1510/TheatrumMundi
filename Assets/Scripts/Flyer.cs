using UnityEngine;
using UnityEngine.UI;

public class Flyer : MonoBehaviour
{
    [SerializeField] ObjectShelfAll objShelfAll;
    [SerializeField] GameObject warningPanel;
    [SerializeField] SaveFileController tmpFileController;
    [SerializeField] GameObject[] _flyerSpaces;
    [SerializeField] private UTJ.FrameCapturer.PlayerControls playerCtrls;
    void Awake()
    {
        gameObject.SetActive(false);
    }
    void Start()
    {
        for (int i = 0; i < SceneManaging.flyerSpace.Length; i++)
        {
            SceneManaging.flyerSpace[i] = -1;
        }
    }
    public void OnClickStartFlyer()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.saveDialogActive && !SceneManaging.railLengthDialogActive && !SceneManaging.flyerActive)
        {
            // Warning current scene will be deleted
            warningPanel.SetActive(true);
            SceneManaging.dialogActive = true;
        }
    }
    public void ClickCancel()
    {
        SceneManaging.flyerActive = false;
        gameObject.SetActive(false);
    }
    public void OnClickFillScene()
    {
        //Debug.Log("hier");
        StartCoroutine(tmpFileController.LoadFileFromServer("Musterszene_Kulissen.json", "fromFlyerCreate"));
        // StartCoroutine(tmpFileController.LoadFileFromWWW("Musterszene_Kulissen.json", "fromFlyerCreate"));
        SceneManaging.flyerActive = false;
    }
    public void OnClickWarning(bool cancel)
    {
        //back to normal mode
        if (cancel)
        {
            SceneManaging.dialogActive = false;
            warningPanel.SetActive(false);
        }
        // open flyer
        else
        {
            // slider soll ganz am anfang sein
            playerCtrls.ButtonStop();
            SceneManaging.dialogActive = false;

            // disable 'figures in schiene ziehen' in Rail Manager
            objShelfAll.ButtonShelf05(false);
            gameObject.SetActive(true);
            SceneManaging.flyerActive = true;

            // load new scene
            StartCoroutine(tmpFileController.LoadFileFromServer("*Musterszene_leer_Visitor.json", "fromFlyerDelete"));

            warningPanel.SetActive(false);
            // destroy children
            for (int i = 0; i < _flyerSpaces.Length; i++)
            {
                if (_flyerSpaces[i].transform.childCount > 1)
                {
                    Destroy(_flyerSpaces[i].transform.GetChild(1).gameObject);
                    // color field
                    _flyerSpaces[i].GetComponent<Image>().color = new Color(.78f, .54f, .44f, 0f);
                    SceneManaging.flyerSpace[i] = -1;
                }
                //destroy borders
                _flyerSpaces[i].transform.GetChild(0).gameObject.SetActive(false);
            }

        }
    }
}
