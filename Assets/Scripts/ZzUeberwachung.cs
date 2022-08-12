using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZzUeberwachung : MonoBehaviour
{
    [SerializeField] private Text _ueberwachungspanelText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _ueberwachungspanelText.text = SceneManaging.isPreviewLoaded.ToString();
    }
}
