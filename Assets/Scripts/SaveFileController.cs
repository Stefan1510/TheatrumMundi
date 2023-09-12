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
    private string _jsonString, loadedFiles, tmpCode;
    public GameObject contentFileSelect, panelCodeInput, panelWarningInput, panelWarningInputVisitor, panelOverwrite, menuKulissen, flyer;
    public RailManager contentMenueRails;
    private Button tmpFileButtonScene;
    [SerializeField] private RailMusicManager tmpMusicManager;
    [SerializeField] private CoulissesManager tmpCoulissesManager;
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
    private bool loadFromAwake;
    private int _loadSaveNew;   // 0 = new, 1 = save, 2 = load
    private string code;
    private Color col_green = new Color32(64, 192, 16, 192);
    private string characters = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
    #endregion
    private void Start()
    {
        if (!SceneManaging.isExpert)
        {
            if (Application.absoluteURL == "tm.skd.museum")
            {
                _basepath = "http://tm.skd.museum/";
            }
            else
            {
                _basepath = "https://lightframefx.de/extras/theatrum-mundi/";
            }

            panelWarningInput = panelWarningInputVisitor;
            _loadSaveNew = 0;
            StartCoroutine(LoadFileFromWWW("*Musterszene_leer_Visitor.json", "fromCode"));
        }
        else
        {
            _basepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            _basepath += "\\theatrum mundi";

            ShowFilesFromDirectory();
        }
        SceneManaging.isPreviewLoaded = true;
        loadFromAwake = true;
        _directorySaves = "Saves";

        tmpCoulissesManager.Awake();
    }
    public void SaveSceneToFile(int overwrite) // 0=save, 1=overwrite, 2=save with number behind
    {
        bool foundName = false;
        string filePath = "";
        SceneData sceneDataSave = GetComponent<SceneDataController>().CreateSceneData(false);

        if (!SceneManaging.isExpert)
        {
            StaticSceneData.StaticData.fileName = sceneDataSave.fileName;
            StaticSceneData.StaticData.fileAuthor = sceneDataSave.fileAuthor;
            StaticSceneData.StaticData.fileComment = sceneDataSave.fileComment;
            StaticSceneData.StaticData.fileDate = sceneDataSave.fileDate;
            StaticSceneData.StaticData.tabActive = sceneDataSave.tabActive;
            string sceneDataSaveString = GetComponent<SceneDataController>().CreateJsonFromSceneData(StaticSceneData.StaticData);

            filePath = code + ".json";
            StartCoroutine(WriteToServer(sceneDataSaveString, filePath));
            codeReminder.SetActive(true);
            codeReminderText.text = "Zuletzt abgespeicherter Code: " + code;
        }
        else    // expertenversion
        {
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
                StaticSceneData.StaticData.pieceLength = sceneDataSave.pieceLength;
                string sceneDataSaveString = GetComponent<SceneDataController>().CreateJsonFromSceneData(StaticSceneData.StaticData);

                if (overwrite == 0) // auf speichern geklickt
                {
                    if (_buttonsFileList.Count > 0)
                    {
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
                            if (btn.name != filePath) // Name is already in loaded files -> overwrite?
                                tmpCounter++;
                        }
                        tmpVal++;

                        if (tmpCounter == _buttonsFileList.Count)
                            foundName = false;
                    }
                    panelOverwrite.SetActive(false);
                    WriteFileToDirectory(sceneDataSaveString, filePath);
                    GenerateFileButton(filePath, true);
                }


                else    // overwrite
                {
                    filePath = tmpCode + sceneDataSave.fileName + ".json";
                    DeleteFileFromDirectory(filePath);
                    WriteFileToDirectory(sceneDataSaveString, filePath);
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
            SceneManaging.isPreviewLoaded = true;
            CoulissesManager tmpCoulMan = menuKulissen.GetComponent<CoulissesManager>();

            for (int i = 0; i < tmpCoulMan.GetComponent<CoulissesManager>().coulisses.Length; i++)
            {
                tmpCoulMan.PlaceInShelf(i);   // alle kulissen zurueck ins shelf
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

            if (!SceneManaging.isExpert)
            {
                StartCoroutine(LoadFilesFromServer(false, "", false));
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
        }
    }
    public void DeleteFile()
    {
        if (_selectedFile != "")
            DeleteFileFromDirectory(_selectedFile);
    }
    public void LoadSceneFromFile(string fileName, bool fromCode, Button fileButtonInstance)
    {
        if (fileName.Substring(0, fileName.Length - 5) != StaticSceneData.StaticData.fileName)
            SceneManaging.isPreviewLoaded = false;
        else
            SceneManaging.isPreviewLoaded = true;
        _selectedFile = fileName;

        if (!SceneManaging.isExpert)
        {
            StartCoroutine(LoadFileFromWWW(fileName, ""));
        }
        else
        {
            LoadFileFromDirectory(fileName);
            tmpFileButtonScene = fileButtonInstance;
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
    }
    public void LoadCodeNow()
    {
        panelCodeInput.SetActive(false);
        StartCoroutine(LoadFilesFromServer(true, inputFieldShowCodeVisitor.text, false));
        inputFieldShowCodeVisitor.text = "";
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

        // code laden
        if (loadFromCode)
        {
            // string ist leer
            if (string.IsNullOrEmpty(code))
            {
                panelWarningInput.SetActive(true);
                panelWarningInput.GetComponent<Text>().text = "Bitte gib einen Code ein.";
                _borderLoad.SetActive(true);
            }
            // string eingegeben
            else
            {
                foreach (string fileEntry in arr)
                {
                    if (fileEntry.Length == 11)
                    {
                        // wenn die ersten 6 zeichen mit denen des eingegebenen codes uebereinstimmen
                        if (fileEntry.ToLower().Substring(0, 6) == code.ToLower())
                        {
                            // if (SceneManaging.isExpert)
                            // {
                            //     // wenn schon szenen geladen wurden
                            //     if (!string.IsNullOrEmpty(loadedFiles))
                            //     {
                            //         if (loadedFiles.ToLower().Contains(code.ToLower()))
                            //         {
                            //             panelWarningInput.SetActive(true);
                            //             panelWarningInput.transform.GetChild(1).GetComponent<Text>().text = "Du hast diese Szene bereits geladen.";
                            //         }
                            //         // szene wurde noch nicht geladen
                            //         else
                            //         {
                            //             ClearFileButtons();
                            //             LoadSceneFromFile(fileEntry, true, null);
                            //             loadedFiles += fileEntry + ",";
                            //         }
                            //         found = true;
                            //         ClosePanelShowCode(_visitorPanelSave);
                            //     }
                            //     // wenn noch keine szenen geladen wurden
                            //     else
                            //     {
                            //         ClearFileButtons();
                            //         LoadSceneFromFile(fileEntry, true, null);
                            //         loadedFiles += fileEntry + ",";
                            //         found = true;
                            //         ClosePanelShowCode(_visitorPanelSave);
                            //     }
                            // }
                            // else
                            // {
                            ClearFileButtons();
                            LoadSceneFromFile(fileEntry, true, null);
                            found = true;
                            ClosePanelShowCode(_visitorPanelSave);
                            // }
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
            string[] separators = { "," };

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
        if (Directory.Exists(_basepath))
        {
            DirectoryInfo d = new DirectoryInfo(_basepath);
            foreach (var fileEntry in d.GetFiles("*.json"))
            {
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
    private IEnumerator WriteToServer(string json, string filePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        // UnityWebRequest uwr = UnityWebRequest.Post(_basepath + "WriteFile.php", form);
        // yield return uwr;

        WWW www = new WWW(_basepath + "WriteFile.php", form);
        yield return www;
        //yield return StartCoroutine(LoadFilesFromServer(false, "", false));

        _placeholderTextWarning.text = "";
        _loadSaveNew = 3;
    }
    private void WriteFileToDirectory(string json, string filePath)
    {
        string path = _basepath + "\\" + filePath;
        Debug.LogWarning(path);
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(json);
        writer.Close();
    }
    public IEnumerator LoadFileFromWWW(string fileName, string status) //bool fromCode, bool fromFlyer)
    {
        SceneDataController tmpSceneDataController = GetComponent<SceneDataController>();
        // UnityWebRequest uwr = UnityWebRequest.Get(_basepath + "Saves/" + fileName);
        // yield return uwr;
        // _jsonString = uwr.downloadHandler.text;
        WWW www = new WWW(_basepath + "Saves/" + fileName);
        yield return www;
        _jsonString = www.text;
        tempSceneData = tmpSceneDataController.CreateSceneDataFromJSON(_jsonString);
        tmpSceneDataController.CreateScene(tempSceneData);

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
        sceneContentData += "Länge: " + "3 min\n\n";
        sceneContentData += "Lichter: " + tmpSceneDataController.countActiveLightElements.ToString() + "\n\n";
        sceneContentData += "Musik: " + tmpSceneDataController.countActiveMusicClips.ToString() + "\n\n";

        textFileContentData.text = sceneContentData;
        _animTimer.SetMaxTime(tmpSceneDataController.scenePieceLength);

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
            contentMenueRails.OpenCloseObjectInTimeline(contentMenueRails.railList[0].myObjects, 0, false);

            this.GetComponent<SceneDataController>().CreateScene(tempSceneData);
        }
        else if (status == "fromFlyerDelete")
        {
            LoadSceneFromTempToStatic();
        }
    }
    private void LoadFileFromDirectory(string fileName)
    {
        string path = _basepath + "\\" + fileName;
        StreamReader reader = new StreamReader(path);
        _jsonString = reader.ReadToEnd();
        reader.Close();
        tempSceneData = GetComponent<SceneDataController>().CreateSceneDataFromJSON(_jsonString);
        GetComponent<SceneDataController>().CreateScene(tempSceneData);
        string sceneMetaData = "";
        sceneMetaData += tempSceneData.fileName + "\n\n";
        sceneMetaData += "erstellt: " + tempSceneData.fileDate + "\n\n";
        sceneMetaData += "Ersteller: " + tempSceneData.fileAuthor + "\n\n";
        sceneMetaData += "Kommentar:\n" + tempSceneData.fileComment;
        textFileMetaData.text = sceneMetaData;
        string sceneContentData = "";
        sceneContentData += "Dateiinformationen:\n\n";
        sceneContentData += "Kulissen: " + GetComponent<SceneDataController>().countActiveSceneryElements.ToString() + "\n\n";
        sceneContentData += "Figuren: " + GetComponent<SceneDataController>().countActiveFigureElements.ToString() + "\n\n";
        sceneContentData += "Länge: " + tempSceneData.pieceLength / 60 + " min\n\n";
        sceneContentData += "Lichter: " + GetComponent<SceneDataController>().countActiveLightElements.ToString() + "\n\n";
        sceneContentData += "Musik: " + GetComponent<SceneDataController>().countActiveMusicClips.ToString() + "\n\n";
        textFileContentData.text = sceneContentData;
    }
    private void DeleteFileFromDirectory(string FileName)
    {
        string path = _basepath + "\\" + FileName;
        File.Delete(path);
        ShowFilesFromDirectory();
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
                    int a = Random.Range(0, characters.Length);
                    code += characters[a].ToString();
                }
                _showSavedCode.transform.GetChild(1).GetComponent<Text>().text = code;
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
    public void ResetTabs(int tab)
    {
        _borderWarning.SetActive(false);
        _borderLoad.SetActive(false);
        panelWarningInput.SetActive(false);
        _placeholderTextWarning.color = new Color(.2f, .2f, .2f, 0.27f);

        _dialogNewScene.SetActive(false);
        _dialogLoadCode.SetActive(false);
        _dialogSave.SetActive(false);

        textShowCode.text = "";

        if (tab == 0)
        {
            _dialogNewScene.SetActive(true);
        }
        else if (tab == 1)
        {
            _dialogLoadCode.SetActive(true);
            inputFieldShowCodeVisitor.ActivateInputField();
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