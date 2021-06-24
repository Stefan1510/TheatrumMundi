using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadSettings : MonoBehaviour
{
    [SerializeField] private Button _buttonLoad;
    // Start is called before the first frame update
    void Start()
    {
        _buttonLoad.onClick.AddListener(LoadFile);
    }

    void LoadFile()
    {
        string docPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string path = Path.Combine(docPath, "test/test.txt");
        string[] lines = File.ReadAllLines(path);
        float z1 = ValidateFloat(lines[0]);
        float y2 = ValidateFloat(lines[1]);
        float y3 = ValidateFloat(lines[2]);
        float z3 = ValidateFloat(lines[3]);

        Debug.Log("z1: " + z1 + " - y2: " + y2 + " - y3: " + y3 + " - z3: " + z3);

        GameObject.Find("SliderHoeheEins").GetComponent<Slider>().value = z1;
        GameObject.Find("SliderAbstandEinsZwei").GetComponent<Slider>().value = y2;
        GameObject.Find("SliderAbstand12_34").GetComponent<Slider>().value = y3;
        GameObject.Find("SliderDeltaHeohe12_34").GetComponent<Slider>().value = z3;
    }

    // Update is called once per frame

    float ValidateFloat(string str)
    {
        float val;
        if (float.TryParse(str, out val))
        {
            if (val < 0f)
                val = 0f;
            if (val > 1f)
                val = 1f;
        }
        else
        {
            val = 0f;
        }
        return val;
    }

    void Update()
    {
        
    }
}
