using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using TMPro;

public class SaveFileController : MonoBehaviour
{
    #region variables
    //private string[] _fileList;
    private string _jsonString, loadedFiles, tmpCode;
    public GameObject contentFileSelect, panelCodeInput, panelWarningInput, panelWarningInputVisitor, panelOverwrite, menuKulissen, flyer;
    public RailManager contentMenueRails;
    private Button tmpFileButtonScene;
    [SerializeField] private RailMusicManager tmpMusicManager;
    //[SerializeField] private SceneDataController tmpSceneDataController;
    //[SerializeField] private LightAnimationRepresentation tmpLightAnimRep;
    //[SerializeField] private SetLightColors tmpSetLightCol;
    [SerializeField] private GameObject _dialogSave, _dialogNewScene, _dialogLoadCode;
    [SerializeField] AnimationTimer _animTimer;
    [SerializeField] GameObject _borderWarning, _borderLoad;
    [SerializeField] Text _placeholderTextWarning;
    [SerializeField] GameObject warningPanel;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _visitorPanelSave;
    [SerializeField] private GameObject codeReminder;
    [SerializeField] private TextMeshProUGUI codeReminderText;
    public InputField inputFieldShowCode, inputFieldShowCodeVisitor;
    [SerializeField] GameObject _showSavedCode;
    public Button fileSelectButton; // prefab
    private List<Button> _buttonsFileList = new List<Button>();
    public Text textFileMetaData, textFileContentData, textShowCode;
    private SceneData tempSceneData;
    private string _selectedFile, _directorySaves, _basepath;
    private bool _isWebGl, loadFromAwake, _pressOk;
    private int _loadSaveNew;   // 0 = new, 1 = save, 2 = load
    private string code;
    private Color col_green = new Color32(64, 192, 16, 192);

    private Color col_grey = new Color(.4f, .4f, .4f, 1);
    private Color col_white = new Color(0.843f, 0.843f, 0.843f, 1);
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
            _basepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            _basepath += "\\theatrum mundi";
            //_basepath = "https://lightframefx.de/extras/theatrum-mundi/";
        }
        //Debug.LogError(Application.absoluteURL);
        SceneManaging.isPreviewLoaded = true;
        loadFromAwake = true;

        _directorySaves = "Saves";
        if (_isWebGl)   // bescucher / web version
        {
            panelWarningInput = panelWarningInputVisitor;
            _loadSaveNew = 0;
            StartCoroutine(LoadFileFromWWW("*Musterszene_leer_Visitor.json", "fromCode"));
        }
        else    // expertenversion
        {
            //StartCoroutine(LoadFilesFromServer(false, "", true));
            ShowFilesFromDirectory();
        }
    }
    private void Update()
    {
        if (_pressOk)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                LoadCodeNow();
                _pressOk = false;
            }
        }
        // if (SceneManaging.sceneChanged)
        // {

        // }
    }
    public void SaveSceneToFile(int overwrite) // 0=save, 1=overwrite, 2=save with number behind
    {
        bool foundName = false;
        string filePath = "";
        SceneData sceneDataSave = this.GetComponent<SceneDataController>().CreateSceneData();
        bool isExpert = this.GetComponent<UnitySwitchExpertUser>()._isExpert;

        if (!isExpert)
        {
            StaticSceneData.StaticData.fileName = sceneDataSave.fileName;
            StaticSceneData.StaticData.fileAuthor = sceneDataSave.fileAuthor;
            StaticSceneData.StaticData.fileComment = sceneDataSave.fileComment;
            StaticSceneData.StaticData.fileDate = sceneDataSave.fileDate;
            string sceneDataSaveString = this.GetComponent<SceneDataController>().CreateJsonFromSceneData(StaticSceneData.StaticData);

            filePath = code + ".json";

            // if (_isWebGl)
            // {
            StartCoroutine(WriteToServer(sceneDataSaveString, filePath, isExpert));
            codeReminder.SetActive(true);
            codeReminderText.text = "Zuletzt abgespeicherter Code: " + code;
            // }
            // else
            // {
            //     Debug.Log("kommich hierher?");
            //     WriteFileToDirectory(sceneDataSaveString, filePath);
            // }

        }
        else    // expertenversion
        {
            //Debug.Log(sceneDataSave.fileName);
            if (sceneDataSave.fileName.Contains("*") || sceneDataSave.fileName.Contains("/"))
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

                if (overwrite == 0) // auf speichern geklickt
                {
                    if (_buttonsFileList.Count > 0)
                    {
                        string[] separators = new string[] { "," };
                        foreach (Button btn in _buttonsFileList)
                        {
                            string[] x = btn.name.Split('.');
                            if (x[0] == sceneDataSave.fileName) // Name is already in loaded files -> overwrite?
                            {
                                panelOverwrite.SetActive(true);
                                foundName = true;
                            }
                        }
                        if (!foundName) // wenn name nicht gefunden wurde
                        {
                            // wenn kein Name eingegeben wurde
                            if (string.IsNullOrEmpty(sceneDataSave.fileName.ToString())) // Warnung, dass ein name eingegeben werden muss
                            {
                                _borderWarning.SetActive(true);
                                _placeholderTextWarning.color = new Color(1, 0, 0, 0.27f);
                            }
                            // wenn es namen noch nicht gibt: normal speichern
                            else
                            {
                                filePath = sceneDataSave.fileName + ".json";
                                ClosePanelShowCode(_visitorPanelSave);

                                if (sceneDataSaveString.Length != 0)
                                {
                                    //StartCoroutine(WriteToServer(sceneDataSaveString, filePath, isExpert));
                                    WriteFileToDirectory(sceneDataSaveString, filePath);

                                    GenerateFileButton(filePath, true);
                                }
                                panelOverwrite.SetActive(false);

                            }
                        }
                    }
                }
                else if (overwrite == 2) // speichern mit counter
                {
                    int tmpCounter;
                    foundName = true;

                    int tmpVal = 2;
                    while (foundName)
                    {
                        tmpCounter = 0;
                        filePath = sceneDataSave.fileName + "_" + tmpVal + ".json";

                        foreach (Button btn in _buttonsFileList)
                        {
                            Debug.Log("btn: " + btn.name + ", filepath: " + filePath);
                            // string[] x = btn.name.Split('.');
                            if (btn.name != filePath) // Name is already in loaded files -> overwrite?
                            {
                                tmpCounter++;
                                // Debug.Log("tmpcounter: "+tmpCounter);
                            }
                        }
                        tmpVal++;

                        Debug.Log("tmpcounter: " + tmpCounter + ", filelist: " + _buttonsFileList.Count);
                        if (tmpCounter == _buttonsFileList.Count)
                        {
                            foundName = false;
                            Debug.Log("namen gibt es nicht");
                        }
                    }
                    panelOverwrite.SetActive(false);
                    WriteFileToDirectory(sceneDataSaveString, filePath);
                    GenerateFileButton(filePath, true);
                }


                else    // overwrite
                {
                    // if (_isWebGl)
                    // {
                    filePath = tmpCode + sceneDataSave.fileName + ".json";
                    DeleteFileFromDirectory(filePath);
                    //     StartCoroutine(WriteToServer(sceneDataSaveString, filePath, true));
                    // }
                    // else
                    // {
                    //StartCoroutine(WriteToServer(sceneDataSaveString, filePath, true));
                    WriteFileToDirectory(sceneDataSaveString, filePath);
                    // }
                    GenerateFileButton(filePath, true);
                    panelOverwrite.SetActive(false);
                }
            }
        }
    }
    public void LoadSceneFromTempToStatic()
    {
        if (tempSceneData != null)
        {
            //Debug.Log("tempsenedata: " + tempSceneData.lightElements);
            CoulissesManager tmpCoulMan = menuKulissen.GetComponent<CoulissesManager>();

            for (int i = 0; i < this.GetComponent<UIController>().goButtonSceneryElements.Length; i++)
            {
                tmpCoulMan.placeInShelf(i);   // alle kulissen zurueck ins shelf
            }
            // alle counter zurueck
            for (int j = 0; j < contentMenueRails.figCounterCircle.Length; j++)
            {
                contentMenueRails.figCounterCircle[j].text = "0";
            }
            for (int k = 0; k < tmpCoulMan.coulisseCounter.Length; k++)
            {
                tmpCoulMan.coulisseCounter[k].text = "0";
            }

            StaticSceneData.StaticData = tempSceneData;
            GetComponent<UIController>().SceneriesApplyToUI();
            GetComponent<UIController>().LightsApplyToUI();
            GetComponent<UIController>().RailsApplyToUI();
            //GetComponent<SceneDataController>().SetFileMetaDataToScene();
            if (_isWebGl)
            {
                // if (GetComponent<UnitySwitchExpertUser>()._isExpert)
                // {
                StartCoroutine(LoadFilesFromServer(false, "", false));
                // }
            }
            else
            {
                if (tmpFileButtonScene != null)
                {
                    for (int i = 0; i < contentFileSelect.transform.childCount; i++)
                    {
                        contentFileSelect.transform.GetChild(i).GetComponent<Button>().image.color = new Color32(255, 255, 255, 255);
                    }
                    tmpFileButtonScene.GetComponent<Button>().image.color = col_green;
                }
                else
                {
                    Debug.Log("kein file selected");
                }
                // if (GetComponent<UnitySwitchExpertUser>()._isExpert)
                // {
                //     StartCoroutine(LoadFilesFromServer(false, "", false));
                // }
                //ShowFilesFromDirectory();
            }

            // when scene is truly loaded then buttons shouldnt be green anymore and dateiinforamtionen ist leer

            textFileContentData.text = "";
            textFileMetaData.text = "";

            // if loaded from Awake coulisses-menue should be loaded
            if (loadFromAwake && !SceneManaging.isExpert)
            {
                menuKulissen.SetActive(true);
                StaticSceneData.StaticData.fileName = "Besucher";
                loadFromAwake = false;
            }
            //Debug.Log("hi2");

            // for (int i = 0; i < contentRailsMenue.railList.Length; i++)
            // {
            //     for (int j = 0; j < contentRailsMenue.railList[i].timelineInstanceObjects.Count; j++)
            //     {
            //         Debug.Log("after: element: " + contentRailsMenue.railList[i].timelineInstanceObjects[j] + ", size: " + contentRailsMenue.railList[i].timelineInstanceObjects[j].GetComponent<RectTransform>().sizeDelta);
            //     }
            // }
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
            StartCoroutine(LoadFilesFromServer(false, "", false));
        }
        else
        {
            ShowFilesFromDirectory();
            //StartCoroutine(LoadFilesFromServer(false, "", false));
        }
    }
    public void LoadSceneFromFile(string fileName, bool fromCode, Button fileButtonInstance)
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
            if (fromCode)
                StartCoroutine(LoadFileFromWWW(fileName, "fromCode"));
            else
            {
                StartCoroutine(LoadFileFromWWW(fileName, ""));
            }
        }
        else
        {
            // if (fromCode)
            //StartCoroutine(LoadFileFromWWW(fileName, "fromCode"));        
            LoadFileFromDirectory(fileName);
            tmpFileButtonScene = fileButtonInstance;
            // else
            // {
            //     StartCoroutine(LoadFileFromWWW(fileName, ""));
            //     if (fileButtonInstance != null)
            //     {
            //         foreach (Button btn in _buttonsFileList)
            //         {
            //             btn.GetComponent<Button>().image.color = Color.white;
            //         }
            //         fileButtonInstance.GetComponent<Button>().image.color = col_green;
            //     }
            // }

        }
    }
    private void GenerateFileButton(string fileName, bool isPermamentScene)
    {
        Button fileButtonInstance = Instantiate(fileSelectButton, contentFileSelect.transform);
        fileButtonInstance.name = fileName;
        if (fileName.Substring(0, fileName.Length - 5) == StaticSceneData.StaticData.fileName)
        {
            fileButtonInstance.GetComponent<Button>().image.color = new Color32(64, 192, 16, 192);
            SceneManaging.isPreviewLoaded = true;
        }
        else if (fileName.Length > 11)
        {
            if (fileName.Substring(6, fileName.Length - 11) == StaticSceneData.StaticData.fileName)
            {
                fileButtonInstance.GetComponent<Button>().image.color = new Color32(64, 192, 16, 192);
                SceneManaging.isPreviewLoaded = true;
            }
        }
        if (isPermamentScene)
            fileButtonInstance.GetComponentInChildren<Text>().text = fileName.Substring(0, fileName.Length - 5);
        else
        {
            string[] x = fileName.Split('.');

            fileButtonInstance.GetComponentInChildren<Text>().text = x[0].Substring(6);
        }
        fileButtonInstance.gameObject.SetActive(true);
        fileButtonInstance.onClick.AddListener(() => LoadSceneFromFile(fileName, false, fileButtonInstance));
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
        if (GetComponent<UnitySwitchExpertUser>()._isExpert)
        {
            StartCoroutine(LoadFilesFromServer(true, inputFieldShowCode.text, false));
            inputFieldShowCode.text = "";
        }
        else
        {
            StartCoroutine(LoadFilesFromServer(true, inputFieldShowCodeVisitor.text, false));
            inputFieldShowCodeVisitor.text = "";
        }

        _pressOk = false;
    }
    public void ClosePanelShowCode(GameObject panel)
    {
        panel.SetActive(false);
        ResetTabs(0);
        SceneManaging.saveDialogActive = false;
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
    private IEnumerator LoadFilesFromServer(bool loadFromCode, string code, bool fromAwake)
    {
        WWWForm form = new WWWForm();
        // UnityWebRequest uwr = UnityWebRequest.Post(_basepath + "LoadFileNames.php", form);
        // yield return uwr;
        WWW www = new WWW(_basepath + "LoadFileNames.php", form);
        yield return www;

        string line = www.text;
        string[] arr = line.Split('?');
        bool found = false;

        //TODO: fuer besucherversion: wenn eintrag gleich eingegebener name

        // code laden
        if (loadFromCode)
        {
            // string ist leer
            if (string.IsNullOrEmpty(code))
            {
                panelWarningInput.SetActive(true);
                panelWarningInput.GetComponent<Text>().text = "Bitte gib einen Code ein.";
                //_placeholderTextLoadWarning.color = new Color(1, 0, 0, 0.27f);
                _borderLoad.SetActive(true);
            }
            // string eingegeben
            else
            {
                foreach (string fileEntry in arr)
                {
                    if (fileEntry.Length > 6)
                    {
                        // wenn die ersten 6 zeichen mit denen des eingegebenen codes uebereinstimmen
                        if (fileEntry.ToLower().Substring(0, 6) == code.ToLower())
                        {
                            if (this.GetComponent<UnitySwitchExpertUser>()._isExpert)
                            {
                                // wenn schon szenen geladen wurden
                                if (!string.IsNullOrEmpty(loadedFiles))
                                {
                                    if (loadedFiles.ToLower().Contains(code.ToLower()))
                                    {
                                        panelWarningInput.SetActive(true);
                                        panelWarningInput.transform.GetChild(1).GetComponent<Text>().text = "Du hast diese Szene bereits geladen.";
                                    }
                                    // szene wurde noch nicht geladen
                                    else
                                    {
                                        ClearFileButtons();
                                        LoadSceneFromFile(fileEntry, true, null);
                                        loadedFiles += fileEntry + ",";
                                    }
                                    found = true;
                                    ClosePanelShowCode(_visitorPanelSave);
                                }
                                // wenn noch keine szenen geladen wurden
                                else
                                {
                                    ClearFileButtons();
                                    LoadSceneFromFile(fileEntry, true, null);
                                    loadedFiles += fileEntry + ",";
                                    found = true;
                                    ClosePanelShowCode(_visitorPanelSave);
                                }
                            }
                            else
                            {
                                ClearFileButtons();
                                LoadSceneFromFile(fileEntry, true, null);
                                found = true;
                                ClosePanelShowCode(_visitorPanelSave);
                            }
                        }
                    }
                }
                if (!found)
                {
                    panelWarningInput.SetActive(true);
                    //panelWarningInput.transform.GetChild(1).GetComponent<Text>().text = "Die Szene wurde nicht gefunden.";
                    panelWarningInput.GetComponent<Text>().text = "Die Szene wurde nicht gefunden.";
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
                    if (fileEntry.Substring(0, 1) == "*")
                    {
                        GenerateFileButton(fileEntry, true);
                    }
                    else if (!string.IsNullOrEmpty(loadedFiles))
                    {
                        string[] x = fileEntry.Split('.');
                        foreach (var word in loadedFiles.Split(separators, System.StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (fileEntry == word)
                            {
                                //Debug.Log("fileentry: " + fileEntry);
                                GenerateFileButton(fileEntry, false);
                            }
                        }
                    }
                }

            }
        }

        if (fromAwake && !SceneManaging.isExpert)
        {
            StartCoroutine(LoadFileFromWWW("*Musterszene_leer_Visitor.json", "fromCode"));
        }

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
                GenerateFileButton(fileEntry.Name, true);
            }
        }
        else
        {
            Directory.CreateDirectory(_basepath);
            return;
        }
        fileSelectButton.gameObject.SetActive(false);
    }
    private IEnumerator WriteToServer(string json, string filePath, bool isExpert)
    {
        WWWForm form = new WWWForm();
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        // UnityWebRequest uwr = UnityWebRequest.Post(_basepath + "WriteFile.php", form);
        // yield return uwr;

        WWW www = new WWW(_basepath + "WriteFile.php", form);
        yield return www;
        yield return StartCoroutine(LoadFilesFromServer(false, "", false));
        if (!isExpert)
        {
            //_textSaveInputName.text = "Bitte schreibe dir deinen Code auf: ";
            //_inputFieldSaveName.GetComponent<InputField>().enabled = false;
            //_inputFieldSaveName.GetComponent<Image>().color = col_grey;
            // _showSavedCode.transform.GetChild(0).GetComponent<Text>().text = filePath.Substring(0, 6);
            _placeholderTextWarning.text = "";
            _loadSaveNew = 3;
        }
    }
    private void WriteFileToDirectory(string json, string filePath)
    {
        string path = _basepath + "\\" + filePath;
        Debug.LogWarning(path);
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(json);
        writer.Close();
        //ShowFilesFromDirectory();
        //StartCoroutine(LoadFilesFromServer(false));
    }
    public IEnumerator LoadFileFromWWW(string fileName, string status) //bool fromCode, bool fromFlyer)
    {
        SceneDataController tmpSceneDataController = this.GetComponent<SceneDataController>();
        // UnityWebRequest uwr = UnityWebRequest.Get(_basepath + "Saves/" + fileName);
        // yield return uwr;
        // _jsonString = uwr.downloadHandler.text;
        WWW www = new WWW(_basepath + "Saves/" + fileName);
        yield return www;
        _jsonString = www.text;
        tempSceneData = tmpSceneDataController.CreateSceneDataFromJSON(_jsonString);
        //Debug.Log("json: "+_jsonString);

        this.GetComponent<SceneDataController>().CreateScene(tempSceneData);
        string sceneMetaData = "";
        sceneMetaData += tempSceneData.fileName + "\n\n";
        sceneMetaData += "erstellt: " + tempSceneData.fileDate + "\n\n";
        sceneMetaData += "Ersteller: " + tempSceneData.fileAuthor + "\n\n";
        sceneMetaData += "Kommentar:\n" + tempSceneData.fileComment;
        textFileMetaData.text = sceneMetaData;
        string sceneContentData = "";
        sceneContentData += "Dateiinformationen:\n\n";
        sceneContentData += "Kulissen: " + tmpSceneDataController.countActiveSceneryElements.ToString() + "\n\n";
        sceneContentData += "Figuren: " + tmpSceneDataController.countActiveFigureElements.ToString() + "\n\n";
        sceneContentData += "Länge: " + "\n\n";
        sceneContentData += "Lichter: " + tmpSceneDataController.countActiveLightElements.ToString() + "\n\n";
        sceneContentData += "Musik: " + tmpSceneDataController.countActiveMusicClips.ToString() + "\n\n";

        textFileContentData.text = sceneContentData;
        _animTimer.SetMaxTime(tmpSceneDataController.pieceLength);

        if (status == "fromCode" && !SceneManaging.isExpert)
        {
            LoadSceneFromTempToStatic();
            _canvas.GetComponent<ObjectShelfAll>().ButtonShelf02();
        }

        else if (status == "fromFlyerCreate")
        {
            LoadSceneFromTempToStatic();

            if (SceneManaging.flyerSpace[0] != -1)  // schlange
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[0], 10, 0, 0, true);
            }
            if (SceneManaging.flyerSpace[1] != -1)  // tier 1
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[1], 30, 1, 0, true);
            }
            if (SceneManaging.flyerSpace[2] != -1)  // tier 2
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[2], 35, 3, 0, true);
            }
            if (SceneManaging.flyerSpace[3] != -1)  // tier 3
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[3], 40, 5, 0, true);
            }
            if (SceneManaging.flyerSpace[4] != -1)  // elefant
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[4], 55, 4, 0, true);
            }
            if (SceneManaging.flyerSpace[5] != -1)  // voruebergehende(r)
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[5], 65, 0, 0, true);
            }
            if (SceneManaging.flyerSpace[6] != -1)  // andere
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[6], 75, 1, 0, true);
            }
            if (SceneManaging.flyerSpace[7] != -1)  // tabakpfeife
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[7], 85, 5, 0, true);
            }
            if (SceneManaging.flyerSpace[8] != -1)  // trauerzug 1
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[8], 97, 3, 1, true);
                //Debug.Log("8: ");
            }
            if (SceneManaging.flyerSpace[9] != -1)  // trauerzug 2
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[9], 118, 3, 2, true);
            }
            if (SceneManaging.flyerSpace[10] != -1) // trauerzug 3
            {
                contentMenueRails.CreateNew2DInstance(SceneManaging.flyerSpace[10], 138, 3, 1, true);
            }
            // boxcollider ausschalten, damit musikstuecke und figuren nicht gedruekct weren koennen wenn schiene klein
            for (int i = 0; i < tmpMusicManager.myObjects.Count; i++)
            {
                tmpMusicManager.myObjects[i].musicPiece.GetComponent<BoxCollider2D>().enabled = false;
            }
            for (int i = 0; i < contentMenueRails.railList[contentMenueRails.currentRailIndex].myObjects.Count; i++)
            {
                contentMenueRails.railList[contentMenueRails.currentRailIndex].myObjects[i].figure.GetComponent<BoxCollider2D>().enabled = false;
            }

            flyer.SetActive(false);
            SceneManaging.flyerActive = false;
            contentMenueRails.currentRailIndex = 0;
            contentMenueRails.Update3DFigurePositions();
            contentMenueRails.openCloseObjectInTimeline(contentMenueRails.railList[0].myObjects, 0, false);

            this.GetComponent<SceneDataController>().CreateScene(tempSceneData);
        }
        else if (status == "fromFlyerDelete")
        {
            LoadSceneFromTempToStatic();
            //_canvas.GetComponent<ObjectShelfAll>().ButtonShelf02();
            // _canvas.GetComponent<ObjectShelfAll>().ButtonShelf05(false);
        }
    }
    private void LoadFileFromDirectory(string fileName)
    {
        string path = _basepath + "\\" + fileName;
        //Debug.Log("path: " + path);
        //Read the text directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        _jsonString = reader.ReadToEnd();
        //Debug.Log("jsonstring: " + _jsonString);
        reader.Close();
        tempSceneData = this.GetComponent<SceneDataController>().CreateSceneDataFromJSON(_jsonString);
        //Debug.Log("tmp scene date: " + tempSceneData);
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
        //StartCoroutine(LoadFilesFromServer(false, "", false));
    }
    public void SaveVisitorVersion()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.aboutActive && !SceneManaging.railLengthDialogActive && !SceneManaging.dialogActive)
        {
            _visitorPanelSave.SetActive(true);
            _loadSaveNew = 0;
            SceneManaging.saveDialogActive = true;
        }
    }
    public void OnClickNewScene()
    {
        StartCoroutine(LoadFileFromWWW("*Musterszene_leer_Visitor.json", "fromCode"));
        ClosePanelShowCode(_visitorPanelSave);
    }
    public void OnClickSaveTabs(int loadSaveNew)
    {
        switch (loadSaveNew)
        {
            case 0: // New Scene
                _loadSaveNew = 0;
                break;
            case 1: // Load via Code
                _loadSaveNew = 1;
                break;
            case 2: // Save Scene
                code = "";
                for (int i = 0; i < 6; i++)
                {
                    int a = UnityEngine.Random.Range(0, characters.Length);
                    code += characters[a].ToString();
                }
                _showSavedCode.transform.GetChild(0).GetComponent<Text>().text = code;//filePath.Substring(0, 6);
                _loadSaveNew = 2;
                break;
        }
        ResetTabs(_loadSaveNew);
    }
    public void OnClickOKAYOnTabs()
    {
        switch (_loadSaveNew)
        {
            case 0: // click auf neue szene - ok
                warningPanel.SetActive(true);
                break;
            case 1:
                LoadCodeNow();
                break;
            case 2:
                SaveSceneToFile(0);
                ClosePanelShowCode(_visitorPanelSave);
                break;
            case 3:
                ClosePanelShowCode(_visitorPanelSave);
                break;
        }
    }
    public void OnClickWarning(bool ok)
    {
        if (ok)
        {
            OnClickNewScene();
        }
        warningPanel.SetActive(false);
    }
    private void ResetTabs(int tab)
    {
        _borderWarning.SetActive(false);
        _borderLoad.SetActive(false);
        panelWarningInput.SetActive(false);
        _placeholderTextWarning.color = new Color(.2f, .2f, .2f, 0.27f);

        _dialogNewScene.SetActive(false);
        _dialogLoadCode.SetActive(false);
        _dialogSave.SetActive(false);

        if (tab == 0)
        {
            _dialogNewScene.SetActive(true);
        }
        else if (tab == 1)
        {
            _dialogLoadCode.SetActive(true);
        }
        else
        {
            _dialogSave.SetActive(true);
        }
    }
    public void OnClickCloseCodeReminder()
    {
        codeReminder.SetActive(false);
    }
}