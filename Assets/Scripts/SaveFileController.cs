using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;

public class SaveFileController : MonoBehaviour
{
    #region variables
    private string _jsonString, loadedFiles, tmpCode;
    public GameObject contentFileSelect, panelCodeInput, panelWarningInput, panelWarningInputVisitor, panelOverwrite, menuKulissen, flyer;
    public RailManager contentMenueRails;
    private Button tmpFileButtonScene;
    [SerializeField] Texture2D texMusterszene, texMusterSzeneLeer;
    [SerializeField] private RailMusicManager tmpMusicManager;
    [SerializeField] private CoulissesManager tmpCoulissesManager;
    [SerializeField] private LightAnimationRepresentation lightAnim;
    [SerializeField] private SetLightColors setLightColors;
    [SerializeField] private GameObject _dialogSave, _dialogNewScene, _dialogLoadCode, dialogQuitSave;
    [SerializeField] private RailManager tmpRailmanager;
    [SerializeField] AnimationTimer _animTimer;
    [SerializeField] GameObject _borderWarning, _borderLoad;
    [SerializeField] Text _placeholderTextWarning;
    [SerializeField] GameObject warningPanel;
    [SerializeField] private GameObject _visitorPanelSave;
    [SerializeField] private GameObject codeReminder;
    [SerializeField] private TextMeshProUGUI codeReminderText;
    [SerializeField] private Snapshot tmpSnapshot;
    [SerializeField] private InputField inputFileName, inputAuthor, inputComment;
    [SerializeField] private LightShelf[] tmpLightShelf;
    public InputField inputFieldShowCode, inputFieldShowCodeVisitor;
    [SerializeField] GameObject _showSavedCode;
    public Button fileSelectButton; // prefab
    private List<Button> _buttonsFileList = new List<Button>();
    [SerializeField] private Button _buttonLoad, _buttonDelete;
    public Text textFileMetaData, textFileContentData, textShowCode;
    private SceneData tempSceneData;
    private string _selectedFile, _basepath;
    private bool loadFromAwake;
    private int _loadSaveNew;   // 0 = new, 1 = save, 2 = load
    private string code;
    private Color col_green = new Color32(64, 192, 16, 192);
    private string characters = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
    private bool _biggerSmaller;
    private float _codeTimer;
    private bool pressEnterOnSaveDialog = false;
    private bool pressEnterOnWarning = false;
    #endregion
    private void Start()
    {
        if (!SceneManaging.isExpert)
        {
            _basepath = "./";
            _basepath = "https://lightframefx.de/extras/theatrum-mundi/2023_11_16/";
            StartCoroutine(LoadFileFromServer("Musterszene_leer_Visitor.json", "fromCode"));

            panelWarningInput = panelWarningInputVisitor;
            _loadSaveNew = 0;
        }
        else
        {
            _basepath = Application.persistentDataPath;
            // _basepath += "/theatrum mundi";

            //copy textures
            tmpSnapshot.SaveTexture2DToFile(texMusterszene, _basepath + "/#Musterszene_preview");
            tmpSnapshot.SaveTexture2DToFile(texMusterSzeneLeer, _basepath + "/#NeueSzene_preview");

            if (!File.Exists(_basepath + "/#Musterszene.json"))
            {
                TextAsset jsonTextFile = Resources.Load<TextAsset>("Text/#Musterszene");
                SceneData sceneData = JsonUtility.FromJson<SceneData>(jsonTextFile.text);
                string json = JsonUtility.ToJson(sceneData);

                StreamWriter writer = new StreamWriter(_basepath + "/#Musterszene.json", true);
                writer.Write(json);
                writer.Close();

                jsonTextFile = Resources.Load<TextAsset>("Text/#NeueSzene");
                sceneData = JsonUtility.FromJson<SceneData>(jsonTextFile.text);
                json = JsonUtility.ToJson(sceneData);

                writer = new StreamWriter(_basepath + "/#NeueSzene.json");
                writer.Write(json);

                writer.Close();
            }

            ShowFilesFromDirectory();
        }
        SceneManaging.isPreviewLoaded = true;
        loadFromAwake = true;

        tmpCoulissesManager.Awake();

        // make buttons not interactable
        _buttonLoad.interactable = false;
        _buttonDelete.interactable = false;
    }
    private void Update()
    {
        if (_biggerSmaller && _codeTimer < 0.5f)
        {
            _codeTimer += Time.deltaTime;
            if (_codeTimer <= 0.25f)
                codeReminderText.transform.localScale = new Vector2(codeReminderText.transform.localScale.x + 0.1f, codeReminderText.transform.localScale.y + 0.1f);
            else if (_codeTimer > 0.25f && codeReminderText.transform.localScale.x > 1)
                codeReminderText.transform.localScale = new Vector2(codeReminderText.transform.localScale.x - 0.1f, codeReminderText.transform.localScale.y - 0.1f);
        }
        else if (_biggerSmaller)
        {
            _biggerSmaller = false;
            _codeTimer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (pressEnterOnSaveDialog)
            {
                Debug.Log("1");
                OnClickOKAYOnTabs();

            }
            else if (pressEnterOnWarning)
            {
                Debug.Log("2");
                OnClickWarning(true);
            }
        }
    }
    public void SaveSceneToFile(int overwrite) // 0=save, 1=overwrite, 2=save with number behind
    {
        bool foundName = false;
        string filePath = "";
        SceneData sceneDataSave = GetComponent<SceneDataController>().CreateSceneData(false);

        if (!SceneManaging.isExpert)    // Besucherversion
        {
            StaticSceneData.StaticData.fileName = sceneDataSave.fileName;
            StaticSceneData.StaticData.fileAuthor = sceneDataSave.fileAuthor;
            StaticSceneData.StaticData.fileComment = sceneDataSave.fileComment;
            StaticSceneData.StaticData.fileDate = sceneDataSave.fileDate;
            StaticSceneData.StaticData.tabActive = sceneDataSave.tabActive;
            string sceneDataSaveString = GetComponent<SceneDataController>().CreateJsonFromSceneData(StaticSceneData.StaticData);

            filePath = _basepath+"Saves/"+code + ".json";
            StartCoroutine(WriteToServer(sceneDataSaveString, filePath));
            codeReminder.SetActive(true);
            codeReminderText.text = code;
            ClosePanelShowCode(_visitorPanelSave);
            _biggerSmaller = true;
        }
        else    // expertenversion
        {
            if (sceneDataSave.fileName.Contains("*") || sceneDataSave.fileName.Contains("/") || sceneDataSave.fileName.Contains("#"))
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

                if (SceneManaging.saveQuit)
                    StaticSceneData.StaticData.fileName = "autosave-file_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;

                if (overwrite == 0) // auf speichern geklickt
                {
                    if (_buttonsFileList.Count > 0)
                    {
                        foreach (Button btn in _buttonsFileList)
                        {
                            string[] x = btn.name.Split('.');
                            if (x[0] == StaticSceneData.StaticData.fileName) // Name is already in loaded files -> overwrite?
                            {
                                panelOverwrite.SetActive(true);
                                foundName = true;
                            }
                        }
                    }
                    if (!foundName) // wenn name nicht gefunden wurde
                    {
                        // wenn kein Name eingegeben wurde
                        if (string.IsNullOrEmpty(StaticSceneData.StaticData.fileName.ToString())) // Warnung, dass ein name eingegeben werden muss
                        {
                            _borderWarning.SetActive(true);
                            _placeholderTextWarning.color = new Color(1, 0, 0, 0.27f);
                        }
                        // wenn es namen noch nicht gibt: normal speichern
                        else
                        {
                            filePath = StaticSceneData.StaticData.fileName + ".json";

                            if (sceneDataSaveString.Length != 0)
                            {
                                WriteFileToDirectory(sceneDataSaveString, filePath);
                                GenerateFileButton(filePath, true, true);
                                LoadInformationPreview(filePath);
                            }
                            panelOverwrite.SetActive(false);
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
                    GenerateFileButton(filePath, true, true);
                }

                else    // overwrite
                {
                    filePath = tmpCode + sceneDataSave.fileName + ".json";
                    DeleteFileFromDirectory(filePath);
                    WriteFileToDirectory(sceneDataSaveString, filePath);
                    GenerateFileButton(filePath, true, true);
                    panelOverwrite.SetActive(false);
                }
            }
        }
    }
    public void LoadSceneFromTempToStatic()
    {
        if (tempSceneData != null)
        {
            CoulissesManager tmpCoulMan = menuKulissen.GetComponent<CoulissesManager>();
            // alle counter zurueck
            for (int j = 0; j < contentMenueRails.figCounterCircle.Length; j++)
            {
                contentMenueRails.figCounterCircle[j].text = "0";
            }
            for (int k = 0; k < tmpCoulMan.coulisseCounter.Length; k++)
            {
                tmpCoulMan.coulisseCounter[k].text = "0";
            }
            for (int j = 0; j < tmpMusicManager.figCounterCircle.Length; j++)
            {
                tmpMusicManager.figCounterCircle[j].text = "0";
            }

            if (SceneManaging.isExpert)
            {
                GetComponent<SceneDataController>().CreateScene(tempSceneData);
                SceneManaging.isPreviewLoaded = true;
            }

            for (int i = 0; i < tmpCoulMan.GetComponent<CoulissesManager>().coulisses.Length; i++)
            {
                tmpCoulMan.PlaceInShelf(i);   // alle kulissen zurueck ins shelf
            }

            StaticSceneData.StaticData = tempSceneData;
            GetComponent<UIController>().SceneriesApplyToUI();
            GetComponent<UIController>().LightsApplyToUI();
            //manuell lichter position einstellen
            GetComponent<SceneDataController>().objectsLightElements[4].goLightElement.GetComponent<LightController>().ChangePosition(0.4f);
            GetComponent<SceneDataController>().objectsLightElements[6].goLightElement.GetComponent<LightController>().ChangePosition(-0.2f);
            GetComponent<UIController>().RailsApplyToUI();
            for (int i = 0; i < tmpLightShelf.Length; i++)
                tmpLightShelf[i].ToggleLightBlock(false);
            tmpRailmanager.Update3DFigurePositions();
            lightAnim.ChangeImage();
            setLightColors.ChangeLightColor();
            setLightColors.ChangeLiveColor(1);

            if (!SceneManaging.isExpert)    // besucherversion
            {
                // StartCoroutine(LoadFilesFromServer(false, "", false));
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
        if (_selectedFile != "" && _selectedFile != "#Musterszene.json" && _selectedFile != "#NeueSzene.json")
            DeleteFileFromDirectory(_selectedFile);
    }
    public void LoadSceneFromFile(string fileName, Button fileButtonInstance)
    {
        if (fileName.Substring(0, fileName.Length - 5) != StaticSceneData.StaticData.fileName)
            SceneManaging.isPreviewLoaded = false;
        else
            SceneManaging.isPreviewLoaded = true;
        _selectedFile = fileName;

        LoadInformationPreview(fileName);
        tmpFileButtonScene = fileButtonInstance;
        tmpSnapshot.ShowSnapshot(Application.persistentDataPath + "/" + fileName);

        // make buttons interactable
        _buttonLoad.interactable = true;
        _buttonDelete.interactable = true;
    }
    private void GenerateFileButton(string fileName, bool isPermamentScene, bool highlight)
    {
        // unhighlight all other buttons
        for (int i = 0; i < contentFileSelect.transform.childCount; i++)
        {
            contentFileSelect.transform.GetChild(i).GetComponent<Button>().image.color = new Color32(255, 255, 255, 255);
        }

        // for config menue
        Button fileButtonInstance = Instantiate(fileSelectButton, contentFileSelect.transform);
        fileButtonInstance.gameObject.SetActive(true);
        fileButtonInstance.onClick.AddListener(() => LoadSceneFromFile(fileName, fileButtonInstance));

        _buttonsFileList.Add(fileButtonInstance);

        fileButtonInstance.name = fileName;

        if (highlight)
            fileButtonInstance.GetComponent<Button>().image.color = new Color32(64, 192, 16, 192);

        if (isPermamentScene)
        {
            fileButtonInstance.GetComponentInChildren<Text>().text = fileName.Substring(0, fileName.Length - 5);
        }
        else
        {
            string[] x = fileName.Split('.');
            fileButtonInstance.GetComponentInChildren<Text>().text = x[0].Substring(6);
        }
    }
    public void ShowInputFieldForCode()
    {
        panelCodeInput.SetActive(true);
        inputFieldShowCode.Select();
    }
    public void LoadCodeNow()
    {
        panelCodeInput.SetActive(false);
        StartCoroutine(CheckIfCodeExists(true, inputFieldShowCodeVisitor.text, false));
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
    private IEnumerator CheckIfCodeExists(bool loadFromCode, string code, bool fromAwake)
    {
        WWWForm form = new WWWForm();
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
                            //LoadSceneFromFile(fileEntry, null);
                            StartCoroutine(LoadFileFromServer(fileEntry, "fromCode"));
                            found = true;
                            ClosePanelShowCode(_visitorPanelSave);
                        }
                    }
                }
                if (!found)
                {
                    panelWarningInput.SetActive(true);
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
                        GenerateFileButton(fileEntry, true, !fromAwake);
                    }
                    else if (!string.IsNullOrEmpty(loadedFiles))
                    {
                        string[] x = fileEntry.Split('.');
                        foreach (var word in loadedFiles.Split(separators, System.StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (fileEntry == word)
                            {
                                GenerateFileButton(fileEntry, false, !fromAwake);
                            }
                        }
                    }
                }
            }
        }

        if (fromAwake && !SceneManaging.isExpert)
        {
            StartCoroutine(LoadFileFromServer("Musterszene_leer_Visitor.json", "fromCode"));
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
                GenerateFileButton(fileEntry.Name, true, false);
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

        Debug.Log("path: "+filePath+", json : "+json);

        // UnityWebRequest uwr = UnityWebRequest.Post(_basepath + "WriteFile.php", form);
        // yield return uwr;

        WWW www = new WWW(_basepath + "WriteFile.php", form);
        yield return www;
        Debug.Log("www: "+www.text);
        //yield return StartCoroutine(LoadFilesFromServer(false, "", false));

        _placeholderTextWarning.text = "";
        _loadSaveNew = 3;
    }
    private void WriteFileToDirectory(string json, string filePath)
    {
        string path = _basepath + "\\" + filePath;
        tmpSnapshot.CallTakeSnapshot(path.Substring(0, path.Length - 5));
        Debug.LogWarning(path);
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(json);
        writer.Close();
    }
    public IEnumerator LoadFileFromServer(string fileName, string status)
    {
        SceneDataController tmpSceneDataController = GetComponent<SceneDataController>();
        WWW www = new WWW(_basepath + "Saves/" + fileName);
        yield return www;
        _jsonString = www.text;
        Debug.Log("json: " + _jsonString);
        Debug.Log("path: " + _basepath + "/Saves/" + fileName);
        tempSceneData = tmpSceneDataController.CreateSceneDataFromJSON(_jsonString);
        tmpSceneDataController.CreateScene(tempSceneData);

        if (status == "fromCode" && !SceneManaging.isExpert)
        {
            LoadSceneFromTempToStatic();
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
            //contentMenueRails.currentRailIndex = 0;
            contentMenueRails.Update3DFigurePositions();
            contentMenueRails.OpenCloseObjectInTimeline(contentMenueRails.railList[0].myObjects, 0, false);
            contentMenueRails.PublicUpdate();

        }
        else if (status == "fromFlyerDelete")
        {
            LoadSceneFromTempToStatic();
        }
    }
    private void LoadInformationPreview(string fileName)
    {
        string path = _basepath + "\\" + fileName;
        StreamReader reader = new StreamReader(path);
        _jsonString = reader.ReadToEnd();
        reader.Close();
        tempSceneData = GetComponent<SceneDataController>().CreateSceneDataFromJSON(_jsonString);

        // inputfields get names of clicked scene
        inputFileName.text = tempSceneData.fileName;
        inputAuthor.text = tempSceneData.fileAuthor;
        inputComment.text = tempSceneData.fileComment;

        //GetComponent<SceneDataController>().CreateScene(tempSceneData);
        string sceneMetaData = "";
        sceneMetaData += "Name: " + tempSceneData.fileName + "\n\n";
        sceneMetaData += "Datum: " + tempSceneData.fileDate + "\n\n";
        sceneMetaData += "Autor/in: " + tempSceneData.fileAuthor + "\n\n";
        sceneMetaData += "Kommentar:\n" + tempSceneData.fileComment;
        textFileMetaData.text = sceneMetaData;

        string sceneContentData = "";
        sceneContentData += "Kulissen: " + CountActiveSceneries().ToString() + "\n\n"; //GetComponent<SceneDataController>().countActiveSceneryElements.ToString() + "\n\n";
        sceneContentData += "Figuren: " + CountActiveFigures().ToString() + "\n\n"; // GetComponent<SceneDataController>().countActiveFigureElements.ToString() + "\n\n";
        sceneContentData += "Länge: " + tempSceneData.pieceLength / 60 + " min\n\n";
        sceneContentData += "Lichter: " + CountLights().ToString() + "\n\n"; //GetComponent<SceneDataController>().countActiveLightElements.ToString() + "\n\n";
        sceneContentData += "Musik: " + CountMusic().ToString() + "\n\n"; //GetComponent<SceneDataController>().countActiveMusicClips.ToString() + "\n\n";
        textFileContentData.text = sceneContentData;
    }
    private int CountActiveFigures()
    {
        int count = 0;
        foreach (FigureElement fE in tempSceneData.figureElements)
        {
            count += fE.figureInstanceElements.Count;
        }
        return count;
    }
    private int CountMusic()
    {
        int count = 0;
        foreach (MusicClipElement mE in tempSceneData.musicClipElements)
        {
            count += mE.musicClipElementInstances.Count;
        }
        return count;
    }
    private int CountActiveSceneries()
    {
        int count = 0;
        foreach (SceneryElement sE in tempSceneData.sceneryElements)
        {
            if (sE.active)
                count++;
        }
        return count;
    }
    private int CountLights()
    {
        int count = 0;
        foreach (LightElement lE in tempSceneData.lightElements)
        {
            if (lE.active)
                count++;
        }
        return count;
    }
    private void DeleteFileFromDirectory(string FileName)
    {
        string path = _basepath + "\\" + FileName;
        string pathImage = _basepath + "\\" + FileName.Substring(0, FileName.Length - 5) + "_preview.jpg";
        File.Delete(path);
        File.Delete(pathImage);
        ShowFilesFromDirectory();
    }
    public void SaveVisitorVersion()
    {
        if (!SceneManaging.tutorialActive && !SceneManaging.aboutActive && !SceneManaging.railLengthDialogActive && !SceneManaging.dialogActive)
        {
            _visitorPanelSave.SetActive(true);
            _loadSaveNew = 0;
            if (!SceneManaging.saveDialogActive)
            {
                pressEnterOnSaveDialog = true;
            }
            SceneManaging.saveDialogActive = true;
        }
    }
    public void OnClickNewScene()
    {
        StartCoroutine(LoadFileFromServer("Musterszene_leer_Visitor.json", "fromCode"));
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
                pressEnterOnWarning = true;
                break;
            case 1:
                LoadCodeNow();
                break;
            case 2:
                SaveSceneToFile(0);
                //ClosePanelShowCode(_visitorPanelSave);
                break;
            case 3:
                ClosePanelShowCode(_visitorPanelSave);
                break;
        }
        pressEnterOnSaveDialog = false;
    }
    public void OnClickWarning(bool ok)
    {
        if (ok)
        {
            OnClickNewScene();
        }
        warningPanel.SetActive(false);
        pressEnterOnWarning = false;
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
    public void UnhighlightButton()
    {
        // unhighlight all buttons
        for (int i = 0; i < contentFileSelect.transform.childCount; i++)
        {
            contentFileSelect.transform.GetChild(i).GetComponent<Button>().image.color = new Color32(255, 255, 255, 255);
        }

        inputFileName.text = "";
        inputAuthor.text = "";
        inputComment.text = "";

        //GetComponent<SceneDataController>().CreateScene(tempSceneData);
        string sceneMetaData = "";
        sceneMetaData += "Name: " + "\n\n";
        sceneMetaData += "Datum: " + "\n\n";
        sceneMetaData += "Autor/in: " + "\n\n";
        sceneMetaData += "Kommentar:\n";
        textFileMetaData.text = sceneMetaData;

        string sceneContentData = "";
        sceneContentData += "Kulissen: " + "\n\n";
        sceneContentData += "Figuren: " + "\n\n";
        sceneContentData += "Länge: " + " min\n\n";
        sceneContentData += "Lichter: " + "\n\n";
        sceneContentData += "Musik: " + "\n\n";
        textFileContentData.text = sceneContentData;

        tmpSnapshot.HidePreview();

        // make buttons not interactable
        _buttonLoad.interactable = false;
        _buttonDelete.interactable = false;
    }
}