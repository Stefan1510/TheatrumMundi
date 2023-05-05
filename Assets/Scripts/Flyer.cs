using UnityEngine;
using UnityEngine.UI;

public class Flyer : MonoBehaviour
{
    [SerializeField] ObjectShelfAll objShelfAll;
    [SerializeField] GameObject warningPanel;
    [SerializeField] SaveFileController tmpFileController;
    [SerializeField] GameObject[] _flyerSpaces;

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
        StartCoroutine(tmpFileController.LoadFileFromWWW("Musterszene_Kulissen.json", "fromFlyerCreate"));
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
            objShelfAll.ButtonShelf05(false);
            gameObject.SetActive(true);

            // load new scene
            StartCoroutine(tmpFileController.LoadFileFromWWW("*Musterszene_leer.json", "fromFlyerDelete"));

            warningPanel.SetActive(false);
            // destroy children
            for (int i = 0; i < _flyerSpaces.Length; i++)
            {
                if (_flyerSpaces[i].transform.childCount > 1)
                {
                    Destroy(_flyerSpaces[i].transform.GetChild(1).gameObject);
                    // color field
                    _flyerSpaces[i].GetComponent<Image>().color = new Color(.78f, .54f, .44f);
                    SceneManaging.flyerSpace[i] = -1;
                }
            //destroy borders
            _flyerSpaces[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            
        }
    }
}
