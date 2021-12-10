using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SaveFileController : MonoBehaviour
{
    //private string[] _fileList;
    private string _jsonString;
    public GameObject contentFileSelect;
    public Button fileSelectButton;
    private List<Button> _buttonsFileList = new List<Button>();
    public Text textFileMetaData;
    public Text textFileContentData;
    private SceneData tempSceneData;
    private string _selectedFile;
    private string _directorySaves;
    //// Start is called before the first frame update
    void Start()
    {
        _directorySaves = "Saves";
        StartCoroutine(LoadFilesFromServer());
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SaveSceneToFile()
    {
        SceneData sceneDataSave = this.GetComponent<SceneDataController>().CreateSceneData();
        //string sceneDataSaveString = this.GetComponent<SceneDataController>().CreateJsonFromSceneData(sceneDataSave);
        string sceneDataSaveString = this.GetComponent<SceneDataController>().CreateJsonFromSceneData(StaticSceneData.StaticData);
        //var path = EditorUtility.SaveFilePanel("Save Settings as JSON", "", ".json", "json");

        string filePath = sceneDataSave.fileName + ".json";
        if (sceneDataSaveString.Length != 0)
        {
            //File.WriteAllText(path, json);

            StartCoroutine(WriteToServer(sceneDataSaveString, filePath));
            //GenerateFileButton(filePath);
        }
    }

    public void LoadSceneFromTempToStatic()
    {
        StaticSceneData.StaticData = tempSceneData;
        //UIController.SceneriesApplyToUI();
        GetComponent<UIController>().SceneriesApplyToUI();
    }

    public void DeleteFile()
    {
        if (_selectedFile != "")
        {
            StartCoroutine(DeleteFileFromServer(_selectedFile));
        }
        StartCoroutine(LoadFilesFromServer());
    }

    public void LoadSceneFromFile(string fileName)
    {
        _selectedFile = fileName;
        StartCoroutine(LoadFileFromWWW(fileName));
        Debug.Log("_____________ selected File: " + _selectedFile);
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
        yield return StartCoroutine(LoadFilesFromServer());
    }

    private IEnumerator LoadFileFromWWW(string fileName)
    {
        WWW www = new WWW("https://lightframefx.de/extras/theatrum-mundi/Saves/" + fileName);
        yield return www;
        _jsonString = www.text;
        tempSceneData = this.GetComponent<SceneDataController>().CreateSceneDataFromJSON(_jsonString);
        this.GetComponent<SceneDataController>().CreateScene(tempSceneData);
        string sceneMetaData = "";
        sceneMetaData += tempSceneData.fileName + "\n\n";
        sceneMetaData += "erstellt: " + tempSceneData.fileDate + "\n\n";
        sceneMetaData += "Ersteller: " + tempSceneData.fileAuthor + "\n\n";
        sceneMetaData += "Kommentar:\n" + tempSceneData.fileComment;
        textFileMetaData.text = sceneMetaData;
        string sceneContentData = "";
        sceneContentData += "Dateiinformationen:\n\n";
        sceneContentData += "Kulissen: " + this.GetComponent<SceneDataController>().countActiveSceneryElements.ToString() + "\n\n";
        sceneContentData += "Fuguren: " + "\n\n";
        sceneContentData += "L�nge: " + "\n\n";
        sceneContentData += "Lichter: " + "\n\n";
        sceneContentData += "Musik: ";
        textFileContentData.text = sceneContentData;
    }
    private IEnumerator DeleteFileFromServer(string FileName)
    {
        WWWForm form = new WWWForm();

        form.AddField("pathDirectory", _directorySaves);
        form.AddField("pathFile", FileName);

        UnityWebRequest www = UnityWebRequest.Post("https://lightframefx.de/extras/theatrum-mundi/DeleteFile.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Script Not Successfull");
        }
        else
        {
            Debug.Log("Script Successfull");
        }
    }


}