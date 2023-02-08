using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class SaveFileController : MonoBehaviour
{
    #region variables
    //private string[] _fileList;
    private string _jsonString, loadedFiles, tmpCode;
    public GameObject contentFileSelect, panelCodeInput, panelSaveShowCode, panelWarningInput, panelOverwrite, menuKulissen, contentRailsMenue;
    public InputField inputFieldShowCode;
    public Button fileSelectButton;
    private List<Button> _buttonsFileList = new List<Button>();
    public Text textFileMetaData, textFileContentData, textShowCode;
    private SceneData tempSceneData;
    private string _selectedFile, _directorySaves, _basepath;
    private bool _isWebGl, loadFromAwake, _pressOk;
    private string characters = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
    #endregion
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
            // _basepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            // _basepath += "\\theatrum mundi";
            _basepath = "https://lightframefx.de/extras/theatrum-mundi/";
        }
        //Debug.LogError(Application.absoluteURL);
        SceneManaging.isPreviewLoaded = true;
        loadFromAwake = true;

        _directorySaves = "Saves";
        if (_isWebGl)
        {

            StartCoroutine(LoadFilesFromServer("", true));
        }
        else
        {
            StartCoroutine(LoadFilesFromServer("", true));
            //ShowFilesFromDirectory();
        }
        //menuKulissen.SetActive(true);
    }
    private void Update()
    {
        if (_pressOk)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("hello");
                LoadCodeNow();
                _pressOk = false;
            }
        }
        if (SceneManaging.sceneChanged)
        {

        }
    }
    public void SaveSceneToFile(int overwrite) // 0=save, 1=overwrite, 2=save with new code
    {

        Debug.Log("hier");
        bool foundName = false;
        string code = "";
        string filePath = "";
        SceneData sceneDataSave = this.GetComponent<SceneDataController>().CreateSceneData();

        Debug.Log(sceneDataSave.fileName);
        if (sceneDataSave.fileName.Contains("*")||sceneDataSave.fileName.Contains("/"))
        {
            panelWarningInput.SetActive(true);
            panelWarningInput.transform.GetChild(1).GetComponent<Text>().text = "Bitte verwende keine Sonderzeichen im Namen.";
        }
        else
        {
            StaticSceneData.StaticData.fileName = sceneDataSave.fileName;
            StaticSceneData.StaticData.fileAuthor = sceneDataSave.fileAuthor;
            StaticSceneData.StaticData.fileComment = sceneDataSave.fileComment;
            StaticSceneData.StaticData.fileDate = sceneDataSave.fileDate;
            string sceneDataSaveString = this.GetComponent<SceneDataController>().CreateJsonFromSceneData(StaticSceneData.StaticData);

            if (overwrite != 1)
            {
                if (overwrite == 0)
                {
                    if (!string.IsNullOrEmpty(loadedFiles))
                    {
                        string[] separators = new string[] { "," };
                        foreach (var word in loadedFiles.Split(separators, System.StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] x = word.Split('.');
                            if (x[0].Substring(6) == sceneDataSave.fileName) // Name is already in loaded files -> overwrite
                            {
                                panelOverwrite.SetActive(true);
                                foundName = true;
                                tmpCode = x[0].Substring(0, 6);
                            }
                        }
                    }
                }

                if (!foundName)
                {
                    if (string.IsNullOrEmpty(sceneDataSave.fileName.ToString())) // Warnung, dass ein name eingegeben werden muss
                    {
                        panelWarningInput.SetActive(true);
                        panelWarningInput.transform.GetChild(1).GetComponent<Text>().text = "Bitte gib erst einen Namen ein!";
                    }
                    else
                    {
                        // create code if not expert
                        if (!SceneManaging.isExpert)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                int a = UnityEngine.Random.Range(0, characters.Length);
                                code += characters[a].ToString();
                            }
                            filePath = code + sceneDataSave.fileName + ".json";
                            panelSaveShowCode.SetActive(true);
                            textShowCode.text = code;
                            loadedFiles += filePath + ",";
                            Debug.Log("files: " + loadedFiles);
                        }

                        else
                        {
                            filePath = sceneDataSave.fileName + ".json";
                            Debug.Log("filepath: " + filePath);
                        }

                        if (sceneDataSaveString.Length != 0)
                        {
                            if (_isWebGl)
                            {
                                StartCoroutine(WriteToServer(sceneDataSaveString, filePath, true));
                            }
                            else
                            {
                                StartCoroutine(WriteToServer(sceneDataSaveString, filePath, true));
                                // WriteFileToDirectory(sceneDataSaveString, filePath);
                            }
                            GenerateFileButton(filePath, true);
                        }
                        panelOverwrite.SetActive(false);
                    }
                }
            }
            else    // overwrite
            {
                if (_isWebGl)
                {
                    filePath = tmpCode + sceneDataSave.fileName + ".json";
                    StartCoroutine(WriteToServer(sceneDataSaveString, filePath, true));
                    panelOverwrite.SetActive(false);
                }
                else
                {
                    StartCoroutine(WriteToServer(sceneDataSaveString, filePath, true));
                    // WriteFileToDirectory(sceneDataSaveString, filePath);
                }
                GenerateFileButton(filePath, true);
            }
        }
    }
    public void LoadSceneFromTempToStatic()
    {
        // for (int i = 0; i < contentRailsMenue.GetComponent<RailManager>().railList.Length; i++)
        // {
        //     for (int j = 0; j < contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects.Count; j++)
        //     {
        //         Debug.Log("before: element: " + contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects[j] + ", size: " + contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects[j].GetComponent<RectTransform>().sizeDelta);
        //     }
        // }

        if (tempSceneData != null)
        {
            for (int i = 0; i < this.GetComponent<UIController>().goButtonSceneryElements.Length; i++)
            {
                menuKulissen.GetComponent<CoulissesManager>().placeInShelf(i);   // alle kulissen zurueck ins shelf
            }
            // Todo: alle counter zurueck
            for (int j = 0; j < contentRailsMenue.GetComponent<RailManager>().figCounterCircle.Length; j++)
            {
                contentRailsMenue.GetComponent<RailManager>().figCounterCircle[j].transform.GetChild(0).GetComponent<Text>().text = "0";
            }

            StaticSceneData.StaticData = tempSceneData;
            GetComponent<UIController>().SceneriesApplyToUI();
            GetComponent<UIController>().LightsApplyToUI();
            GetComponent<UIController>().RailsApplyToUI();
            //GetComponent<SceneDataController>().SetFileMetaDataToScene();
            if (_isWebGl)
            {
                StartCoroutine(LoadFilesFromServer("", false));
            }
            else
            {
                StartCoroutine(LoadFilesFromServer("", false));
                // ShowFilesFromDirectory();
            }
            //AnimationTimer.SetTime(0);
            //contentRailsMenue.GetComponent<RailManager>().openCloseObjectInTimeline(false,contentRailsMenue.GetComponent<RailManager>().railList[0].timelineInstanceObjects,0);
            //contentRailsMenue.GetComponent<RailManager>().openTimelineByClick(false,0,false);

            // when scene is truly loaded then buttons shouldnt be green anymore and dateiinforamtionen ist leer
            for (int i = 0; i < contentFileSelect.transform.childCount; i++)
            {
                contentFileSelect.transform.GetChild(i).GetComponent<Button>().image.color = new Color32(255, 255, 255, 255);
                //Debug.Log("child: " + contentFileSelect.transform.GetChild(i));
            }
            textFileContentData.text = "";
            textFileMetaData.text = "";

            // if loaded from Awake coulisses-menue should be loaded
            if (loadFromAwake)
            {
                menuKulissen.SetActive(true);
                StaticSceneData.StaticData.fileName = "neu";
                loadFromAwake = false;
            }

            for (int i = 0; i < contentRailsMenue.GetComponent<RailManager>().railList.Length; i++)
            {
                for (int j = 0; j < contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects.Count; j++)
                {
                    Debug.Log("after: element: " + contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects[j] + ", size: " + contentRailsMenue.GetComponent<RailManager>().railList[i].timelineInstanceObjects[j].GetComponent<RectTransform>().sizeDelta);
                }
            }
        }
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
            StartCoroutine(LoadFilesFromServer("", false));
        }
        else
        {
            // ShowFilesFromDirectory();
            StartCoroutine(LoadFilesFromServer("", false));
        }
    }
    public void LoadSceneFromFile(string fileName, bool fromCode)
    {
        SceneManaging.isPreviewLoaded = false;
        if (fileName.Substring(0, fileName.Length - 5) != StaticSceneData.StaticData.fileName)
        {
            SceneManaging.isPreviewLoaded = false;
        }
        else
        {
            SceneManaging.isPreviewLoaded = true;
        }
        _selectedFile = fileName;

        if (_isWebGl)
        {
            if (fromCode) StartCoroutine(LoadFileFromWWW(fileName, true));
            else StartCoroutine(LoadFileFromWWW(fileName, false));
        }
        else
        {
            if (fromCode) StartCoroutine(LoadFileFromWWW(fileName, true));            // LoadFileFromDirectory(fileName);
            else StartCoroutine(LoadFileFromWWW(fileName, false));

        }
    }
    private void GenerateFileButton(string fileName, bool isPermamentScene)
    {
        Button fileButtonInstance = Instantiate(fileSelectButton, contentFileSelect.transform);
        //Debug.Log(fileName);
        fileButtonInstance.name = fileName;
        if (fileName.Substring(0, fileName.Length - 5) == StaticSceneData.StaticData.fileName)
        {
            fileButtonInstance.GetComponent<Button>().image.color = new Color32(64, 192, 16, 192);
            //Debug.Log("color");
            SceneManaging.isPreviewLoaded = true;
        }
        else if (fileName.Length > 11)
        {
            if (fileName.Substring(6, fileName.Length - 11) == StaticSceneData.StaticData.fileName)
            {
                fileButtonInstance.GetComponent<Button>().image.color = new Color32(64, 192, 16, 192);
                //Debug.Log("color");
                SceneManaging.isPreviewLoaded = true;
            }
        }
        if (isPermamentScene) fileButtonInstance.GetComponentInChildren<Text>().text = fileName.Substring(0, fileName.Length - 5);
        else
        {
            string[] x = fileName.Split('.');

            fileButtonInstance.GetComponentInChildren<Text>().text = x[0].Substring(6);
        }
        fileButtonInstance.gameObject.SetActive(true);
        fileButtonInstance.onClick.AddListener(() => LoadSceneFromFile(fileName, false));
        _buttonsFileList.Add(fileButtonInstance);
    }
    public void ShowInputFieldForCode()
    {
        panelCodeInput.SetActive(true);
        inputFieldShowCode.Select();
        _pressOk = true;
    }
    public void LoadCodeNow()
    {
        panelCodeInput.SetActive(false);
        StartCoroutine(LoadFilesFromServer(inputFieldShowCode.text, false));
        inputFieldShowCode.text = "";
        _pressOk = false;
    }
    public void ClosePanelShowCode(GameObject panel)
    {
        panel.SetActive(false);
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
    private IEnumerator LoadFilesFromServer(string code, bool fromAwake)
    {
        WWWForm form = new WWWForm();
        WWW www = new WWW(_basepath + "LoadFileNames.php", form);
        yield return www;

        string line = www.text;
        string[] arr = line.Split('?');

        if (!string.IsNullOrEmpty(code))    // code eingegeben
        {
            foreach (string fileEntry in arr)
                if (fileEntry.ToLower().Substring(0,6) == code.ToLower())
                {
                    if (!string.IsNullOrEmpty(loadedFiles))
                    {
                        if (loadedFiles.ToLower().Contains(code.ToLower()))
                        {
                            panelWarningInput.SetActive(true);
                            panelWarningInput.transform.GetChild(1).GetComponent<Text>().text = "Du hast diese Szene bereits geladen.";
                        }
                        else
                        {
                            ClearFileButtons();
                            LoadSceneFromFile(fileEntry, true);
                            loadedFiles += fileEntry + ",";
                        }
                    }
                    else
                    {
                        ClearFileButtons();
                        LoadSceneFromFile(fileEntry, true);
                        loadedFiles += fileEntry + ",";
                    }
                }
        }
        else
        {
            ClearFileButtons();
            string[] separators = new string[] { "," };

            foreach (string fileEntry in arr)
            {
                if (fileEntry.Length > 4)
                {
                    if (fileEntry.Substring(0, 1) == "*") GenerateFileButton(fileEntry, true);
                    else if (!string.IsNullOrEmpty(loadedFiles))
                    {
                        string[] x = fileEntry.Split('.');
                        foreach (var word in loadedFiles.Split(separators, System.StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (fileEntry == word)
                            {
                                GenerateFileButton(fileEntry, false);
                            }
                        }
                    }
                }

            }
        }
        if (fromAwake)
        {
            StartCoroutine(LoadFileFromWWW("*Musterszene_leer.json", true));
        }
    }

    public void SaveVisitorVersion()
    {
        Debug.Log("dialog oeffne dich!");
    }

    // private void ShowFilesFromDirectory()
    // {
    //     ClearFileButtons();
    //     fileSelectButton.gameObject.SetActive(true);
    //     //Debug.LogError(_basepath);
    //     if (Directory.Exists(_basepath))
    //     {
    //         DirectoryInfo d = new DirectoryInfo(_basepath);
    //         foreach (var fileEntry in d.GetFiles("*.json"))
    //         {
    //             //Debug.LogWarning(fileEntry.Name);
    //             GenerateFileButton(fileEntry.Name);
    //         }
    //     }
    //     else
    //     {
    //         Directory.CreateDirectory(_basepath);
    //         return;
    //     }
    //     fileSelectButton.gameObject.SetActive(false);
    // }

    private IEnumerator WriteToServer(string json, string filePath, bool save)
    {
        WWWForm form = new WWWForm();
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        WWW www = new WWW(_basepath + "WriteFile.php", form);
        yield return www;
        yield return StartCoroutine(LoadFilesFromServer("", false));
    }
    /* private void WriteFileToDirectory(string json, string filePath)
     {
         string path = _basepath + "\\" + filePath;
         Debug.LogWarning(path);
         StreamWriter writer = new StreamWriter(path, true);
         writer.Write(json);
         writer.Close();
         // ShowFilesFromDirectory();
         StartCoroutine(LoadFilesFromServer(false));
     }*/
    private IEnumerator LoadFileFromWWW(string fileName, bool fromCode)
    {
        //Debug.Log("klappt?");
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
        if (fromCode)
        {
            LoadSceneFromTempToStatic();
        }

    }
    // private void LoadFileFromDirectory(string fileName)
    // {
    //     string path = _basepath + "\\" + fileName;
    //     Debug.Log("path: " + path);
    //     //Read the text directly from the test.txt file
    //     StreamReader reader = new StreamReader(path);
    //     _jsonString = reader.ReadToEnd();
    //     Debug.Log("jsonstring: " + _jsonString);
    //     reader.Close();
    //     tempSceneData = this.GetComponent<SceneDataController>().CreateSceneDataFromJSON(_jsonString);
    //     Debug.Log("tmp scene date: " + tempSceneData);
    //     this.GetComponent<SceneDataController>().CreateScene(tempSceneData);
    //     string sceneMetaData = "";
    //     sceneMetaData += tempSceneData.fileName + "\n\n";
    //     sceneMetaData += "erstellt: " + tempSceneData.fileDate + "\n\n";
    //     sceneMetaData += "Ersteller: " + tempSceneData.fileAuthor + "\n\n";
    //     sceneMetaData += "Kommentar:\n" + tempSceneData.fileComment;
    //     textFileMetaData.text = sceneMetaData;
    //     string sceneContentData = "";
    //     sceneContentData += "Dateiinformationen:\n\n";
    //     sceneContentData += "Kulissen: " + this.GetComponent<SceneDataController>().countActiveSceneryElements.ToString() + "\n\n";
    //     sceneContentData += "Figuren: " + this.GetComponent<SceneDataController>().countActiveFigureElements.ToString() + "\n\n";
    //     sceneContentData += "Länge: " + "\n\n";
    //     sceneContentData += "Lichter: " + this.GetComponent<SceneDataController>().countActiveLightElements.ToString() + "\n\n";
    //     sceneContentData += "Musik: " + this.GetComponent<SceneDataController>().countActiveMusicClips.ToString() + "\n\n";
    //     textFileContentData.text = sceneContentData;
    // }
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
        // ShowFilesFromDirectory();
        StartCoroutine(LoadFilesFromServer("", false));
    }
}