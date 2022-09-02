using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZzUeberwachung : MonoBehaviour
{
    [SerializeField] private Text _ueberwachungspanelText;
    public GameObject gameController;
    public GameObject UICanvas;
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("mainemnu: " + SceneManaging.mainMenuActive + ", config: " + SceneManaging.configMenueActive + ", director: " + SceneManaging.mainMenuActive);
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        _ueberwachungspanelText.text = SceneManaging.isPreviewLoaded.ToString();
        if (Input.GetMouseButtonDown(0))
        {
            timer = 0.0f;
        }
        else
        {
            if (timer < 10)
            {
                timer += Time.deltaTime;
                //                Debug.Log("timer: " + timer);
            }
            else if (timer >= 10 && SceneManaging.playing == false)
            {
                // nach 1 min ohne click neue szene laden
                Debug.Log("mainemnu: " + SceneManaging.mainMenuActive + ", config: " + SceneManaging.configMenueActive);

                gameController.GetComponent<SaveFileController>().LoadSceneFromFile("Lotte3008.json");
                gameController.GetComponent<SaveFileController>().LoadSceneFromTempToStatic();
                timer = 0.0f;
                // if (SceneManaging.mainMenuActive == 1 && SceneManaging.configMenueActive == 2)
                // {
                Debug.Log("button kulisse ist aktiv");
                UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf02();
                // }

            }
        }
    }
}
