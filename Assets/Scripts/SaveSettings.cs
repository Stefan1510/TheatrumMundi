using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveSettings : MonoBehaviour
{
    [SerializeField] private Button _buttonSave;
    // Start is called before the first frame update
    void Start()
    {
        _buttonSave.onClick.AddListener(SaveFile);
    }

    void SaveFile()
    {
        float z1 = GameObject.Find("SliderHoeheEins").GetComponent<Slider>().value;
        float y2 = GameObject.Find("SliderAbstandEinsZwei").GetComponent<Slider>().value;
        float y3 = GameObject.Find("SliderAbstand12_34").GetComponent<Slider>().value;
        float z3 = GameObject.Find("SliderDeltaHeohe12_34").GetComponent<Slider>().value;
        Debug.Log("z1: " + z1 + " - y2: " + y2 + " - y3: " + y3 + " - z3: " + z3);
        string docPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string path = Path.Combine(docPath, "test/test.txt");
        string[] lines = { z1.ToString(), y2.ToString(), z3.ToString(), y3.ToString() };
        File.WriteAllLines(path, lines);
    }
}
