using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileController : MonoBehaviour
{
    //private string[] _fileList;
    private string _jsonString;
    public GameObject contentFileSelect;
    public Button fileSelectButton;
    private List<Button> _buttonsFileList = new List<Button>();
    public Text textFileMetaData;
    public Text textFileContentData;

    //// Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadFilesFromServer());
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SaveSceneToFile()
    {
        SceneData sceneDataSave = this.GetComponent<SceneDataController>().CreateSceneData();
        string sceneDataSaveString = this.GetComponent<SceneDataController>().CreateJsonFromSceneData(sceneDataSave);
        //var path = EditorUtility.SaveFilePanel("Save Settings as JSON", "", ".json", "json");

        string filePath = sceneDataSave.fileName + ".json";
        if (sceneDataSaveString.Length != 0)
        {
            //File.WriteAllText(path, json);

            StartCoroutine(WriteToServer(sceneDataSaveString, filePath));
            //GenerateFileButton(filePath);
        }
        StartCoroutine(LoadFilesFromServer());

    }

    public void LoadSceneFromFile(string fileName)
    {
        StartCoroutine(LoadFileFromWWW(fileName));
    }

    private void GenerateFileButton(string fileName)
    {
        Button fileButtonInstance = Instantiate(fileSelectButton, contentFileSelect.transform);
        fileButtonInstance.name = fileName;
        fileButtonInstance.GetComponentInChildren<Text>().text = fileName;
        fileButtonInstance.gameObject.SetActive(true);
        fileButtonInstance.onClick.AddListener(() => LoadSceneFromFile(fileName));
        _buttonsFileList.Add(fileButtonInstance);
    }

    private void ClearFileButtons()
    {
        foreach (Button fileButton in _buttonsFileList)
        {
            Destroy(fileButton.gameObject);
        }
        _buttonsFileList.Clear();
    }

    //public void GenerateFileButtonList()
    //{
    //    StartCoroutine(LoadFilesFromServer());

    //    print("_fileList: " + _fileList);
    //    foreach (string fileEntry in _fileList)
    //    {
    //        GenerateFileButton(fileEntry);
    //    }
    //}

    private IEnumerator LoadFilesFromServer()
    {
        ClearFileButtons();
        fileSelectButton.gameObject.SetActive(true);
        WWWForm form = new WWWForm();
        WWW www = new WWW("https://lightframefx.de/extras/theatrum-mundi/LoadFileNames.php", form);
        yield return www;


        string line = www.text;
        print("www-text: " + line);
        string[] arr = line.Split('?');
        foreach (string fileEntry in arr)
        {
            if (fileEntry.Length > 4)
            {
                GenerateFileButton(fileEntry);
            }
        }
        fileSelectButton.gameObject.SetActive(false);
    }

    private IEnumerator WriteToServer(string json, string filePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        WWW www = new WWW("https://lightframefx.de/extras/theatrum-mundi/WriteFile.php", form);

        yield return www;

        Debug.Log("www: " + www.text);
    }

    private IEnumerator LoadFileFromWWW(string fileName)
    {
        WWW www = new WWW("https://lightframefx.de/extras/theatrum-mundi/Saves/" + fileName);
        yield return www;
        _jsonString = www.text;
        SceneData sceneData = this.GetComponent<SceneDataController>().CreateSceneDataFromJSON(_jsonString);
        this.GetComponent<SceneDataController>().CreateScene(sceneData);
        string sceneMetaData = "";
        sceneMetaData += sceneData.fileName + "\n\n";
        sceneMetaData += "erstellt: " + sceneData.fileDate + "\n\n";
        sceneMetaData += "Ersteller: " + sceneData.fileAuthor + "\n\n";
        sceneMetaData += "Kommentar:\n" + sceneData.fileComment;
        textFileMetaData.text = sceneMetaData;
        string sceneContentData = "";
        sceneContentData += "Dateiinformationen:\n\n";
        sceneContentData += "Kulissen: " + this.GetComponent<SceneDataController>().countActiveSceneryElements.ToString() + "\n\n";
        sceneContentData += "Fuguren: " + "\n\n";
        sceneContentData += "Länge: " + "\n\n";
        sceneContentData += "Lichter: " + "\n\n";
        sceneContentData += "Musik: ";
        textFileContentData.text = sceneContentData;
    }

}