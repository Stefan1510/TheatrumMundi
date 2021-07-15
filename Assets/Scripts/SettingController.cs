using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
//using UnityEditor;

public class SettingController : MonoBehaviour {
    public Slider sliderHoeheEins;
    public Slider sliderAbstandEinsZwei;
    public Slider sliderAbstand12_34;
    public Slider sliderDeltaHoehe12_34;

    public Dropdown DropdownFileSelection;

    private StageElementList myStageElements = new StageElementList();
    private bool sceneLoaded = false;

    private void Start() {
        //LoadFilesFromFolder();
        StartCoroutine(LoadFilesFromServer());
    }

    public void LoadFileFromWWWWrapper() {
        StartCoroutine(LoadFileFromWW());
    }

    private IEnumerator LoadFilesFromServer() {
        WWWForm form = new WWWForm();
        WWW www = new WWW("https://www2.htw-dresden.de/~struckk/unity/LoadFileNames.php", form);
        yield return www;

        string line = www.text;
        string[] arr = line.Split('?');
        DropdownFileSelection.options.Clear();

        foreach (string str in arr) {
            if (str.Length > 4) {
                DropdownFileSelection.options.Add(new Dropdown.OptionData(str));
            }
        }
    }
    private IEnumerator LoadFileFromWW() {
        string selectedFilename = DropdownFileSelection.options[DropdownFileSelection.value].text;
        WWW www = new WWW("https://www2.htw-dresden.de/~struckk/unity/Saves/" + selectedFilename);
        yield return www;
        string jsonString = www.text;

        myStageElements = JsonUtility.FromJson<StageElementList>(jsonString);

        for (int i = 0; i < myStageElements.stageElements.Count; i++) {
            switch (myStageElements.stageElements[i].description) {
                case "Schiene1":
                    sliderHoeheEins.value = myStageElements.stageElements[i].z;
                    break;
                case "Schiene2":
                    sliderAbstandEinsZwei.value = myStageElements.stageElements[i].y;
                    break;
                case "Schiene3":
                    sliderAbstand12_34.value = myStageElements.stageElements[i].y;
                    sliderDeltaHoehe12_34.value = myStageElements.stageElements[i].z;
                    break;
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
                        sliderHoeheEins.value = myStageElements.stageElements[i].z;
                        break;
                    case "Schiene2":
                        sliderAbstandEinsZwei.value = myStageElements.stageElements[i].y;
                        break;
                    case "Schiene3":
                        sliderAbstand12_34.value = myStageElements.stageElements[i].y;
                        sliderDeltaHoehe12_34.value = myStageElements.stageElements[i].z;
                        break;
                }
            }
        }
    }

    public void SaveFile() {
        //if no File was loaded - init myStageElements Class with init .json 
        if (!sceneLoaded) {
            //string tempPath = Resources.Load("JSON/Init.json")
            string tempPath = Application.dataPath + "/Resources/JSON/InitScene.json";
            //string tempPath = Application.persistentDataPath;
            StreamReader reader = new StreamReader(tempPath);
            string jsonString = reader.ReadToEnd();
            reader.Close();

            myStageElements = JsonUtility.FromJson<StageElementList>(jsonString);
        }

        int index = 0;
        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene1");
        myStageElements.stageElements[index].z = (float)Math.Round(sliderHoeheEins.value * 100f) / 100f;

        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene2");
        myStageElements.stageElements[index].y = sliderAbstandEinsZwei.value;

        index = myStageElements.stageElements.FindIndex(i => i.description == "Schiene3");
        myStageElements.stageElements[index].y = sliderAbstand12_34.value;
        myStageElements.stageElements[index].z = sliderDeltaHoehe12_34.value;

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
        string filePath = System.DateTime.Now.Year + "_" + System.DateTime.Now.Month + "_" 
            + System.DateTime.Now.Day + "_" + System.DateTime.Now.Hour + "_" + System.DateTime.Now.Minute + "_" + System.DateTime.Now.Second + ".json";
        form.AddField("pathFile", filePath);
        form.AddField("text", json);

        WWW www = new WWW("https://www2.htw-dresden.de/~neubert/WriteFile.php", form);
        yield return www;

        Debug.Log("www: " +www.text);

        yield return new WaitForSeconds(2f);
        LoadFileFromWWWWrapper();
    }
}
