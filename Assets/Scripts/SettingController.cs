using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class SettingController : MonoBehaviour {
    public Slider SliderRail1x;
    public Slider SliderRail1y;
    public Slider SliderRail2x;
    public Slider SliderRail3x;
    public Slider SliderRail3y;
    public Slider SliderRail4x;
    public Slider SliderRail5x;
    public Slider SliderRail5y;
    public Slider SliderRail6x;
    public Slider SliderRail7x;
    public Slider SliderRail7y;
    public Slider SliderRail8x;
    public GameObject[] saveKulissen;

    public Dropdown DropdownFileSelection;
    //public GameObject contentFileSelect;
    //public Button fileSelectButton;
    //public Text fileSelectButtonText;

    private StageElementList myStageElements = new StageElementList();
    private bool sceneLoaded = false;
    private string _jsonString;

    private string _fileName;

    private void Start() {
        //LoadFilesFromFolder();
        StartCoroutine(LoadFilesFromServer());
        StartCoroutine(GetJsonString());
    }

    public void LoadFileFromWWWWrapper(string filename)
    {
        //_fileName = "211015-110409.json";
        _fileName = filename;
        StartCoroutine(LoadFileFromWWW());
    }


    private IEnumerator LoadFilesFromServer()
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post("https://lightframefx.de/extras/theatrum-mundi/LoadFileNames.php", form);
        yield return www;

        string line = www.downloadHandler.text;
        string[] arr = line.Split('?');
        DropdownFileSelection.options.Clear();

        foreach (string str in arr) {
            if (str.Length > 4) {
                DropdownFileSelection.options.Add(new Dropdown.OptionData(str));
            }
        }

    }
    private IEnumerator LoadFileFromWWW()
    {
        string selectedFilename = DropdownFileSelection.options[DropdownFileSelection.value].text;
        UnityWebRequest www = UnityWebRequest.Get("https://lightframefx.de/extras/theatrum-mundi/Saves/" + _fileName);
        yield return www;
        string jsonString = www.downloadHandler.text;

        myStageElements = JsonUtility.FromJson<StageElementList>(jsonString);

        for (int i = 0; i < myStageElements.stageElements.Count; i++) {
            switch (myStageElements.stageElements[i].description) {
                case "Schiene1":
                    SliderRail1x.value = myStageElements.stageElements[i].x;
                    SliderRail1y.value = myStageElements.stageElements[i].y;
                    break;
                case "Schiene2":
                    SliderRail2x.value = myStageElements.stageElements[i].x;
                    break;
                case "Schiene3":
                    SliderRail3x.value = myStageElements.stageElements[i].x;
                    SliderRail3y.value = myStageElements.stageElements[i].y;
                    break;
                case "Schiene4":
                    SliderRail4x.value = myStageElements.stageElements[i].x;
                    break;
                case "Schiene5":
                    SliderRail5x.value = myStageElements.stageElements[i].x;
                    SliderRail5y.value = myStageElements.stageElements[i].y;
                    break;
                case "Schiene6":
                    SliderRail6x.value = myStageElements.stageElements[i].x;
                    break;
                case "Nagelbrett1":
                    SliderRail7x.value = myStageElements.stageElements[i].x;
                    SliderRail7y.value = myStageElements.stageElements[i].y;
                    break;
                case "Nagelbrett2":
                    SliderRail8x.value = myStageElements.stageElements[i].x;
                    break; 
            }

            foreach (GameObject saveKulisse in saveKulissen)
            {
                if (myStageElements.stageElements[i].description == saveKulisse.name)
                {
                    //Debug.Log("halloooo:" + saveKulisse.name);

                    saveKulisse.transform.position = new Vector3(myStageElements.stageElements[i].x, myStageElements.stageElements[i].y, myStageElements.stageElements[i].z);
                    saveKulisse.transform.SetParent(GameObject.Find(myStageElements.stageElements[i].parent).transform);
                    saveKulisse.SetActive(myStageElements.stageElements[i].active);

                }
            }
        }
    }

    private void LoadFilesFromFolder() {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/Resources/JSON/");
        //DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] fileInfo = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
        DropdownFileSelection.options.Clear();
        DropdownFileSelection.options.Add(new Dropdown.OptionData("new"));

        foreach (FileInfo file in fileInfo) {
            Dropdown.OptionData optionData = new Dropdown.OptionData(file.Name);
            DropdownFileSelection.options.Add(optionData);
            DropdownFileSelection.value = 1;
        }
    }

    public void LoadFile() {
        string selectedFilename = DropdownFileSelection.options[DropdownFileSelection.value].text;
        Debug.Log(selectedFilename);

        ///string path = EditorUtility.OpenFilePanel("Load .json", "", "json");
        string path = Application.dataPath + "/Resources/JSON/" + selectedFilename;
        //string path = Application.persistentDataPath + selectedFilename;

        if (path.Length != 0) {
            sceneLoaded = true;

            StreamReader reader = new StreamReader(path);
            string jsonString = reader.ReadToEnd();
            reader.Close();

            myStageElements = JsonUtility.FromJson<StageElementList>(jsonString);

            for (int i = 0; i < myStageElements.stageElements.Count; i++) {
                switch (myStageElements.stageElements[i].description) {
                    case "Schiene1":
                        SliderRail1x.value = myStageElements.stageElements[i].z;
                        break;
                    case "Schiene2":
                        SliderRail2x.value = myStageElements.stageElements[i].y;
                        break;
                    case "Schiene3":
                        SliderRail3x.value = myStageElements.stageElements[i].y;
                        SliderRail3y.value = myStageElements.stageElements[i].z;
                        break;
                }
            }
        }
    }

    private IEnumerator GetJsonString()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://lightframefx.de/extras/theatrum-mundi/Saves/InitScene.json");
        yield return www;
        _jsonString = www.downloadHandler.text;
    }

    public void SaveFile() {
        //if no File was loaded - init myStageElements Class with init .json 
        if (!sceneLoaded) {
            //string tempPath = Resources.Load("JSON/Init.json")
            //string tempPath = Application.dataPath + "/Resources/JSON/InitScene.json";
            //string tempPath = Application.persistentDataPath;
            //StreamReader reader = new StreamReader(tempPath);
            //string jsonString = reader.ReadToEnd();
            //reader.Close();
            StartCoroutine(GetJsonString());
            myStageElements = JsonUtility.FromJson<StageElementList>(_jsonString);
        }

        int index = 0;
        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene1");
        //myStageElements.stageElements[index].x = (float)Math.Round(SliderRail1x.value * 100f) / 100f;
        myStageElements.stageElements[index].x = (SliderRail1x.value);
        myStageElements.stageElements[index].y = (SliderRail1y.value);

        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene2");
        myStageElements.stageElements[index].x = SliderRail2x.value;

        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene3");
        myStageElements.stageElements[index].x = SliderRail3x.value;
        myStageElements.stageElements[index].y = SliderRail3y.value;

        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene4");
        myStageElements.stageElements[index].x = SliderRail4x.value;

        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene5");
        myStageElements.stageElements[index].x = (SliderRail5x.value);
        myStageElements.stageElements[index].y = (SliderRail5y.value);

        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene6");
        myStageElements.stageElements[index].x = SliderRail6x.value;

        index = myStageElements.stageElements.FindIndex(i => i.description == "Nagelbrett1");
        myStageElements.stageElements[index].x = SliderRail7x.value;
        myStageElements.stageElements[index].y = SliderRail7y.value;

        index = myStageElements.stageElements.FindIndex(i => i.description == "Nagelbrett2");
        myStageElements.stageElements[index].x = SliderRail8x.value;

        //for (int j = 0; j < saveKulissen.Length; j++)
        //{ 
        //    index = myStageElements.stageElements.FindIndex(i => i.description == saveKulissen[j].name);
        //    myStageElements.stageElements[index].x = saveKulissen[j].transform.position.x;
        //    myStageElements.stageElements[index].y = saveKulissen[j].transform.position.y;
        //    myStageElements.stageElements[index].z = saveKulissen[j].transform.position.z;
        //    myStageElements.stageElements[index].parent = saveKulissen[j].transform.parent.name;
        //    myStageElements.stageElements[index].active = saveKulissen[j].activeSelf;
        //}

        foreach (GameObject saveKulisse in saveKulissen)
        {
            index = myStageElements.stageElements.FindIndex(i => i.description == saveKulisse.name);
            myStageElements.stageElements[index].x = saveKulisse.transform.position.x;
            myStageElements.stageElements[index].y = saveKulisse.transform.position.y;
            myStageElements.stageElements[index].z = saveKulisse.transform.position.z;
            myStageElements.stageElements[index].parent = saveKulisse.transform.parent.name;
            myStageElements.stageElements[index].active = saveKulisse.activeSelf;
        }

        string json = JsonUtility.ToJson(myStageElements);
        //var path = EditorUtility.SaveFilePanel("Save Settings as JSON", "", ".json", "json");
        var path = Application.dataPath + "/Resources/thisScene.json";
        if (path.Length != 0) {
            //File.WriteAllText(path, json);
            StartCoroutine(WriteToServer(json));
        }
    }

    private IEnumerator WriteToServer(string json) {
        WWWForm form = new WWWForm();
        string filePath = System.DateTime.Now.ToString("yyMMdd-HHmmss") + ".json";
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        UnityWebRequest www = UnityWebRequest.Post("https://lightframefx.de/extras/theatrum-mundi/WriteFile.php", form);
        yield return www;

        Debug.Log("www: " +www.downloadHandler.text);

        yield return StartCoroutine(LoadFilesFromServer());
    }

    public void getInformation()
    {
        GameObject[] kulissen = GameObject.FindGameObjectsWithTag("Kulisse");
        foreach (GameObject kulisse in saveKulissen)
        {
            Debug.Log(kulisse.name + " - " + kulisse.gameObject.transform.parent.name + " - " + kulisse.transform.position + " - " + kulisse.activeSelf);
        }
    }
}
