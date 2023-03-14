using UnityEngine;
using UnityEngine.UI;

public class Flyer : MonoBehaviour
{
    [SerializeField] ObjectShelfAll objShelfAll;
    [SerializeField] GameObject warningPanel;
    [SerializeField] SaveFileController tmpFileController;
    [SerializeField] RailManager railManager;

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
        StartCoroutine(tmpFileController.LoadFileFromWWW("Musterszene_Kulissen.json", false));
        if (SceneManaging.flyerSpace[0] != -1)
        {
            Debug.Log("1");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[0], 0, 0, 0, true);
        }
        if (SceneManaging.flyerSpace[1] != -1)
        {
            Debug.Log("2");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[1], 30, 1, 0, true);
        }
        if (SceneManaging.flyerSpace[2] != -1)
        {
            Debug.Log("3");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[2], 40, 3, 0, true);
        }
        if (SceneManaging.flyerSpace[3] != -1)
        {
            Debug.Log("4");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[3], 70, 2, 0, true);
        }
        if (SceneManaging.flyerSpace[4] != -1)
        {
            Debug.Log("5");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[4], 85, 1, 0, true);
        }
        if (SceneManaging.flyerSpace[5] != -1)
        {
            Debug.Log("6");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[5], 90, 3, 0, true);
        }
        if (SceneManaging.flyerSpace[6] != -1)
        {
            Debug.Log("7");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[6], 100, 3, 0, true);
        }
        if (SceneManaging.flyerSpace[7] != -1)
        {
            Debug.Log("8");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[7], 120, 4, 0, true);
        }
        if (SceneManaging.flyerSpace[8] != -1)
        {
            Debug.Log("9");
            railManager.CreateNew2DInstance(SceneManaging.flyerSpace[8], 130, 5, 0, true);
        }
        //         if (SceneManaging.flyerSpace[9] != -1)
        // {
        //     railManager.CreateNew2DInstance(SceneManaging.flyerSpace[9], 170, 0, 5, true);
        // }
        gameObject.SetActive(false);
        SceneManaging.flyerActive = false;
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
