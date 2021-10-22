using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFileController : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void saveJSONFile()
    {
        SceneData sceneDataSave = this.GetComponent<SceneDataController>().CreateRecentSceneData();
        string sceneDataSaveString = this.GetComponent<SceneDataController>().createJSONfromSceneData(sceneDataSave);
        //var path = EditorUtility.SaveFilePanel("Save Settings as JSON", "", ".json", "json");
        if (sceneDataSaveString.Length != 0)
        {
            //File.WriteAllText(path, json);
            StartCoroutine(WriteToServer(sceneDataSaveString));
        }
    }

    private IEnumerator WriteToServer(string json)
    {
        WWWForm form = new WWWForm();
        string filePath = "__" + System.DateTime.Now.ToString("yyMMdd-HHmmss") + ".json";
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        WWW www = new WWW("https://lightframefx.de/extras/theatrum-mundi/WriteFile.php", form);

        yield return www;

        Debug.Log("www: " + www.text);

    }
}
