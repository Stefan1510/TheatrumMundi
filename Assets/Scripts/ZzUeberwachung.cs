using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZzUeberwachung : MonoBehaviour
{
    [SerializeField] private Text _ueberwachungspanelText;
    public GameObject gameController, UICanvas, helpText;
    public float timer;
    //private bool pressed = false;
    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        _ueberwachungspanelText.text = SceneManaging.isPreviewLoaded.ToString();
        // für besuchertool: aller 1 min ohne Aktivität Musterszene laden
        // if (Input.GetMouseButtonDown(0))
        // {
        //     timer = 0.0f;
        // }
        // else
        // {
        //     if (timer < 65)
        //     {
        //         timer += Time.deltaTime;
        //     }
        //     else if (timer >= 65 && SceneManaging.playing == false)
        //     {
        //         // nach 1 min ohne click neue szene laden
        //         gameController.GetComponent<SaveFileController>().LoadSceneFromFile("MusterSzene_11-10.json");
        //         gameController.GetComponent<SaveFileController>().LoadSceneFromTempToStatic();
        //         timer = 0.0f;
        //         UICanvas.GetComponent<ObjectShelfAll>().ButtonShelf02();
        //     }
        // }
        // if (timer < 5)
        // {
        //     timer += Time.deltaTime;
        // }
        // else if (timer >= 5)
        // {
        //     gameController.GetComponent<SaveFileController>().LoadSceneFromFile("MusterSzene_11-10.json", true);
        // }
    }
}
