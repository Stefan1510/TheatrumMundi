using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

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
    private string _basepath;
    private bool _isWebGl;
    //// Start is called before the first frame update

    private void Awake()
    {
        #if UNITY_WEBGL
            _isWebGl = true;
        #else
            //Debug.LogWarning("any other");
        #endif

        if (_isWebGl)
        {
            //Debug.LogError("WEBGL!!!");
            if (Application.absoluteURL == "tm.skd.museum")
            {
                _basepath = "http://tm.skd.museum/";
            }
            else
            {
                _basepath = "https://lightframefx.de/extras/theatrum-mundi/";
            }
        }
        else
        {
            _basepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            _basepath += "\\theatrum mundi";
        }
        //Debug.LogError(Application.absoluteURL);
        
    }

    void Start()
    {
        _directorySaves = "Saves";
        if (_isWebGl)
        {
            StartCoroutine(LoadFilesFromServer());
        }
        else
        {
            ShowFilesFromDirectory();
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SaveSceneToFile()
    {
        SceneData sceneDataSave = this.GetComponent<SceneDataController>().CreateSceneData();
        //string sceneDataSaveString = this.GetComponent<SceneDataController>().CreateJsonFromSceneData(sceneDataSave);
        StaticSceneData.StaticData.fileName = sceneDataSave.fileName;
        StaticSceneData.StaticData.fileAuthor = sceneDataSave.fileAuthor;
        StaticSceneData.StaticData.fileComment = sceneDataSave.fileComment;
        StaticSceneData.StaticData.fileDate = sceneDataSave.fileDate;
        string sceneDataSaveString = this.GetComponent<SceneDataController>().CreateJsonFromSceneData(StaticSceneData.StaticData);
        //var path = EditorUtility.SaveFilePanel("Save Settings as JSON", "", ".json", "json");

        string filePath = sceneDataSave.fileName + ".json";
        if (sceneDataSaveString.Length != 0)
        {
            //File.WriteAllText(path, json);

            if (_isWebGl)
            {
                StartCoroutine(WriteToServer(sceneDataSaveString, filePath));
            }
            else
            {
                WriteFileToDirectory(sceneDataSaveString, filePath);
            }
            //GenerateFileButton(filePath);
        }
    }

    public void LoadSceneFromTempToStatic()
    {
        StaticSceneData.StaticData = tempSceneData;
        //UIController.SceneriesApplyToUI();
        GetComponent<UIController>().SceneriesApplyToUI();
        GetComponent<UIController>().LightsApplyToUI();
        GetComponent<UIController>().RailsApplyToUI();
        GetComponent<SceneDataController>().SetFileMetaDataToScene();
        if (_isWebGl)
        {
            StartCoroutine(LoadFilesFromServer());
        }
        else
        {
            ShowFilesFromDirectory();
        }
        AnimationTimer.SetTime(0);
        GetComponent<UIController>().Rails[0].GetComponent<RailManager>().PublicUpdate();
    }

    public void DeleteFile()
    {
        if (_selectedFile != "")
        {
            if (_isWebGl)
            {
                StartCoroutine(DeleteFileFromServer(_selectedFile));
            }
            else
            {
                DeleteFileFromDirectory(_selectedFile);
            }
        }

        if (_isWebGl)
        {
            StartCoroutine(LoadFilesFromServer());
        }
        else
        {
            ShowFilesFromDirectory();
        }
    }

    public void LoadSceneFromFile(string fileName)
    {
        _selectedFile = fileName;
        if (_isWebGl)
        {
            StartCoroutine(LoadFileFromWWW(fileName));
        }
        else
        {            
            LoadFileFromDirectory(fileName);
        }
        //Debug.Log("_____________ selected File: " + _selectedFile);
    }

    private void GenerateFileButton(string fileName)
    {
        Button fileButtonInstance = Instantiate(fileSelectButton, contentFileSelect.transform);
        //Debug.LogError(fileName.Substring(0, fileName.Length - 5));
        fileButtonInstance.name = fileName;
        if (fileName.Substring(0,fileName.Length-5) == StaticSceneData.StaticData.fileName)
        {
            //Debug.LogError("gleicher Filename !!!! einesse  q344l   1243keaskr");
            fileButtonInstance.GetComponent<Button>().image.color = new Color32(64, 192, 16, 192);
        }
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
        WWW www = new WWW(_basepath + "LoadFileNames.php", form);
        yield return www;


        string line = www.text;
        //print("www-text: " + line);
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

    private void ShowFilesFromDirectory()
    {
        ClearFileButtons();
        fileSelectButton.gameObject.SetActive(true);
        //Debug.LogError(_basepath);
        if (Directory.Exists(_basepath))
        {
            DirectoryInfo d = new DirectoryInfo(_basepath);
            foreach (var fileEntry in d.GetFiles("*.json"))
            {
                //Debug.LogWarning(fileEntry.Name);
                GenerateFileButton(fileEntry.Name);
            }
        }
        else
        {
            Directory.CreateDirectory(_basepath);
            return;
        }
        fileSelectButton.gameObject.SetActive(false);
    }

    private IEnumerator WriteToServer(string json, string filePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        WWW www = new WWW(_basepath + "WriteFile.php", form);

        yield return www;

        Debug.Log("www: " + www.text);
        yield return StartCoroutine(LoadFilesFromServer());
    }

    private void WriteFileToDirectory(string json, string filePath)
    {
        string path = _basepath + "\\" + filePath;
        Debug.LogWarning(path);
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(json);
        writer.Close();
        ShowFilesFromDirectory();
    }

    private IEnumerator LoadFileFromWWW(string fileName)
    {
        WWW www = new WWW(_basepath + "Saves/" + fileName);
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
        sceneContentData += "Figuren: " + this.GetComponent<SceneDataController>().countActiveFigureElements.ToString() + "\n\n";
        sceneContentData += "Länge: " + "\n\n";
        sceneContentData += "Lichter: " + this.GetComponent<SceneDataController>().countActiveLightElements.ToString() + "\n\n";
        sceneContentData += "Musik: " + this.GetComponent<SceneDataController>().countActiveMusicClips.ToString() + "\n\n";
        textFileContentData.text = sceneContentData;
    }

    private void LoadFileFromDirectory(string fileName)
    {
        string path = _basepath + "\\" + fileName;
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        _jsonString = reader.ReadToEnd();
        reader.Close();
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
        sceneContentData += "Figuren: " + this.GetComponent<SceneDataController>().countActiveFigureElements.ToString() + "\n\n";
        sceneContentData += "Länge: " + "\n\n";
        sceneContentData += "Lichter: " + this.GetComponent<SceneDataController>().countActiveLightElements.ToString() + "\n\n";
        sceneContentData += "Musik: " + this.GetComponent<SceneDataController>().countActiveMusicClips.ToString() + "\n\n";
        textFileContentData.text = sceneContentData;
    }

    private IEnumerator DeleteFileFromServer(string FileName)
    {
        WWWForm form = new WWWForm();

        form.AddField("pathDirectory", _directorySaves);
        form.AddField("pathFile", FileName);

        UnityWebRequest www = UnityWebRequest.Post(_basepath + "DeleteFile.php", form);
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

    private void DeleteFileFromDirectory(string FileName)
    {
        string path = _basepath + "\\" + FileName;
        File.Delete(path);
        ShowFilesFromDirectory();
    }


}