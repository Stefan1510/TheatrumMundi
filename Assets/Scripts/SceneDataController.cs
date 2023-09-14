using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SceneDataController : MonoBehaviour
{
    #region variables
    public InputField inputFieldFileNameExpert;
    public InputField inputFieldFileAuthor;
    public InputField inputFieldFileComment;
    public GameObject[] objectsRailElements;
    public GameObject[] objectsSceneryElements;
    public GameObject[] objectsFigureElements;
    public GameObject ContentRailMenue;
    public AudioClip[] objectsMusicClips;
    public GameObject imageTimelineRailMusic;
    public objectsLightElement[] objectsLightElements;
    [HideInInspector] public string sceneFileName;
    [HideInInspector] public string sceneFileAuthor;
    [HideInInspector] public string sceneFileDate;
    [HideInInspector] public string sceneFileComment;
    [HideInInspector] public int countActiveSceneryElements = 0;
    [HideInInspector] public int countActiveLightElements = 0;
    [HideInInspector] public int countActiveFigureElements = 0;
    [HideInInspector] public int countActiveMusicClips = 0;
    [HideInInspector] public int scenePieceLength, sceneTabActive;
    [HideInInspector] public List<GameObject> objects3dFigureInstances;
    [HideInInspector] public List<GameObject> objects2dFigureInstances;
    [HideInInspector] public SceneData recentSceneData;
    [HideInInspector] public SceneData tempSceneData;
    [SerializeField] private Slider sliderMaxLength;
    [SerializeField] private TimeSliderController timeSliderController;
    [SerializeField] private ObjectShelfAll tmpobjShelfAll;
    #endregion
    void Awake()
    {
        StaticSceneData.StaticData = CreateSceneData(true);
        foreach (GameObject objectSceneryElement in objectsSceneryElements)
        {
            objectSceneryElement.transform.SetParent(GameObject.Find("Schiene1").transform);
            objectSceneryElement.SetActive(false);

            objectSceneryElement.GetComponent<cakeslice.Outline>().enabled = false;
        }

    }
    private void GetFileMetaDataFromScene(bool fromAwake)
    {
        if (GetComponent<UnitySwitchExpertUser>()._isExpert)
        {
            sceneFileName = inputFieldFileNameExpert.text;
            sceneFileAuthor = inputFieldFileAuthor.text;
            sceneFileComment = inputFieldFileComment.text;
        }

        sceneFileDate = DateTime.Now.ToString();
        if (fromAwake)
        {
            scenePieceLength = 10;
            sceneTabActive = 2;
        }
        else
        {
            scenePieceLength = AnimationTimer._maxTime;
            if (tmpobjShelfAll.MenueShelf02.activeSelf)
                sceneTabActive = 2;
            else if (tmpobjShelfAll.MenueShelf05.activeSelf)
                sceneTabActive = 5;
            else if (tmpobjShelfAll.MenueShelf07.activeSelf)
                sceneTabActive = 7;
        }
    }
    public void SetFileMetaDataToScene()
    {
        inputFieldFileNameExpert.text = sceneFileName;
        inputFieldFileAuthor.text = sceneFileAuthor;
        inputFieldFileComment.text = sceneFileComment;
    }
    public SceneData CreateSceneData(bool fromAwake)  // only in awake
    {
        SceneData sceneData = new SceneData
        {
            railElements = new List<RailElement>()
        };
        foreach (GameObject objectRailElement in objectsRailElements)
        {
            RailElement sceneRailElement = new RailElement
            {
                name = objectRailElement.name,
                x = objectRailElement.transform.localPosition.x,
                y = objectRailElement.transform.localPosition.y,
                z = objectRailElement.transform.localPosition.z,
                velocity = 1.0f,
                direction = "toRight",
                railElementSpeeds = new List<RailElementSpeed>()
            };
            sceneRailElement.railElementSpeeds.Add(new RailElementSpeed());
            sceneData.railElements.Add(sceneRailElement);
        }

        GetFileMetaDataFromScene(fromAwake);
        sceneData.fileName = sceneFileName;
        sceneData.fileAuthor = sceneFileAuthor;
        sceneData.fileComment = sceneFileComment;
        sceneData.fileDate = sceneFileDate;
        sceneData.pieceLength = scenePieceLength;
        sceneData.tabActive = sceneTabActive;

        sceneData.sceneryElements = new List<SceneryElement>();
        sceneData.figureElements = new List<FigureElement>();
        sceneData.lightElements = new List<LightElement>();
        sceneData.lightingSets = new List<LightingSet>();
        sceneData.backgroundPositions = new List<BackgroundPosition>();
        sceneData.musicClipElements = new List<MusicClipElement>();

        //scenery aka kulissen
        foreach (GameObject objectSceneryElement in objectsSceneryElements)
        {
            SceneryElement sceneSceneryElement = new SceneryElement
            {
                name = objectSceneryElement.name,
                x = objectSceneryElement.transform.position.x,
                y = objectSceneryElement.transform.position.y,
                z = objectSceneryElement.transform.position.z,
                parent = objectSceneryElement.transform.parent.name,
                zPos = 0,
                mirrored = false,
                outline = false
            };
            sceneData.sceneryElements.Add(sceneSceneryElement);
        }

        //figures
        foreach (GameObject objectFigureElement in objectsFigureElements)
        {
            FigureElement sceneFigureElement = new FigureElement
            {
                name = objectFigureElement.name,
                x = objectFigureElement.transform.position.x,
                y = objectFigureElement.transform.position.y,
                z = objectFigureElement.transform.position.z,
                mirrored = false,
                width = 0.27f,
                height = 0.16f,
                railnumber = 1,
                direction = "toRight",
                figureInstanceElements = new List<FigureInstanceElement>()
            };
            sceneData.figureElements.Add(sceneFigureElement);
        }

        //light
        foreach (objectsLightElement objectLightElement in objectsLightElements)
        {
            GameObject gameObjectLe = objectLightElement.goLightElement;
            LightElement sceneLightElement = new LightElement
            {
                name = gameObjectLe.name,
                x = gameObjectLe.transform.localPosition.x,
                y = gameObjectLe.transform.localPosition.y,
                z = gameObjectLe.transform.localPosition.z,
                active = gameObjectLe.GetComponent<Light>().isActiveAndEnabled,
                railnumber = 1,
                r = gameObjectLe.GetComponent<Light>().color.r,
                g = gameObjectLe.GetComponent<Light>().color.g,
                b = gameObjectLe.GetComponent<Light>().color.b,
                intensity = gameObjectLe.GetComponent<Light>().intensity,
                angle_h = 256,
                angle_v = 256,
                stagePosition = objectLightElement.lightStagePosition
            };

            switch (objectLightElement.lightStagePosition)
            {
                case 0:
                    sceneLightElement.y = gameObjectLe.transform.localPosition.y;
                    sceneLightElement.z = gameObjectLe.transform.localPosition.z;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

            sceneData.lightElements.Add(sceneLightElement);
        }

        //lightAnimation
        sceneData.lightingSets.Add(new LightingSet());

        //backgroundAnimation
        sceneData.backgroundPositions.Add(new BackgroundPosition());

        //music
        foreach (AudioClip clip in objectsMusicClips)
        {
            MusicClipElement sceneMusicClipElement = new MusicClipElement
            {
                name = clip.name,
                musicClipElementInstances = new List<MusicClipElementInstance>()
            };
            sceneData.musicClipElements.Add(sceneMusicClipElement);
        }
        recentSceneData = sceneData;
        return sceneData;
    }
    public void CreateScene(SceneData sceneData)
    {
        sceneFileName = sceneData.fileName;
        sceneFileAuthor = sceneData.fileAuthor;
        sceneFileDate = sceneData.fileDate;
        sceneFileComment = sceneData.fileComment;
        scenePieceLength = sceneData.pieceLength;
        sceneTabActive = sceneData.tabActive;

        if (SceneManaging.isExpert)
            SetPieceLength(scenePieceLength);

        //rail elements
        RailsApplyToScene(sceneData.railElements);

        //scenery elements (kulissen)
        SceneriesApplyToScene(sceneData.sceneryElements);

        //light elements
        LightsApplyToScene(sceneData.lightElements);

        //figure elements (Figuren)
        FiguresApplyToScene(sceneData.figureElements);

        //music title elements (Musik)
        MusicApplyToScene(sceneData.musicClipElements);

        //set right tab
        if (!SceneManaging.isExpert)
            SetTabActive(sceneData.tabActive);
    }

    public void SetPieceLength(int pieceLength)
    {
        sliderMaxLength.value = pieceLength / 60;
        timeSliderController.ChangeMaxLength();
    }
    public void RailsApplyToScene(List<RailElement> railElements)
    {
        foreach (RailElement re in railElements)
        {
            foreach (GameObject goRailElement in objectsRailElements)
            {
                if (re.name == goRailElement.name)
                    goRailElement.transform.localPosition = new Vector3(re.x, re.y, re.z);
            }
            if (SceneManaging.isExpert)
            {
                foreach (RailElementSpeed res in re.railElementSpeeds)
                {
                    timeSliderController.GetComponent<JumpToKeyframe>()._railPanelsLineDraw[timeSliderController.GetComponent<JumpToKeyframe>()._railSelection].GetComponent<DrawCurve>().ChangeCurve();
                }
            }
        }
    }
    public void SceneriesApplyToScene(List<SceneryElement> sceneryElements)
    {
        countActiveSceneryElements = 0;
        foreach (SceneryElement se in sceneryElements)
        {
            foreach (GameObject goSceneryElement in objectsSceneryElements) // 3D-Objekte
            {
                if (se.name == goSceneryElement.name)
                {
                    se.railnumber = int.Parse(se.parent.Substring(7));
                    goSceneryElement.transform.parent = GameObject.Find(se.parent).transform;
                    goSceneryElement.transform.localPosition = new Vector3(se.x + (se.zPos * 0.01f), se.y, se.z);
                    goSceneryElement.SetActive(se.active);
                    if (se.mirrored)
                    {
                        if (goSceneryElement.transform.localScale.x > 0)
                            goSceneryElement.transform.localScale = new Vector3(-goSceneryElement.transform.localScale.x, goSceneryElement.transform.localScale.y, goSceneryElement.transform.localScale.z);
                    }
                    else
                    {
                        if (goSceneryElement.transform.localScale.x < 0)
                            goSceneryElement.transform.localScale = new Vector3(-goSceneryElement.transform.localScale.x, goSceneryElement.transform.localScale.y, goSceneryElement.transform.localScale.z);
                    }
                    if (se.active)
                        countActiveSceneryElements++;
                    if (se.outline == false)
                        goSceneryElement.GetComponent<cakeslice.Outline>().enabled = false;
                    else
                        goSceneryElement.GetComponent<cakeslice.Outline>().enabled = true;
                }
            }
        }
    }
    public void LightsApplyToScene(List<LightElement> lightElements)
    {
        countActiveLightElements = 0;
        foreach (LightElement le in lightElements)
        {
            foreach (objectsLightElement objectLightElement in objectsLightElements)
            {
                if (le.name == objectLightElement.goLightElement.name)
                {
                    GameObject gameObjectLe = objectLightElement.goLightElement;
                    gameObjectLe.transform.localPosition = new Vector3(le.x, le.y, le.z);
                    gameObjectLe.GetComponent<Light>().enabled = le.active;
                    if (le.active)
                    {
                        countActiveLightElements++;
                    }
                    gameObjectLe.GetComponent<Light>().intensity = le.intensity;
                    switch (objectLightElement.lightStagePosition)
                    {
                        case 0:
                            break;
                        case 1: // left
                            gameObjectLe.GetComponent<LightController>().ChangeHorizontal(le.angle_h);
                            gameObjectLe.GetComponent<LightController>().ChangeVertical(le.angle_v);
                            gameObjectLe.GetComponent<LightController>().ChangePosition(le.z);
                            gameObjectLe.GetComponent<LightController>().ChangeHeight(le.y);
                            gameObjectLe.GetComponent<LightController>().ChangeIntensity(le.intensity);
                            gameObjectLe.GetComponent<LightController>().sliderLbIntensity.value = le.intensity;
                            break;
                        case 2: // right
                            gameObjectLe.GetComponent<LightController>().ChangeHorizontal(le.angle_h);
                            gameObjectLe.GetComponent<LightController>().ChangeVerticalLeft(le.angle_v);
                            gameObjectLe.GetComponent<LightController>().ChangePosition(le.z);
                            gameObjectLe.GetComponent<LightController>().ChangeHeight(le.y);
                            gameObjectLe.GetComponent<LightController>().ChangeIntensity(le.intensity);
                            gameObjectLe.GetComponent<LightController>().sliderLbIntensity.value = le.intensity;
                            break;
                        case 3:
                            countActiveLightElements--;
                            break;
                        case 4:
                            gameObjectLe.GetComponent<LightMiddleController>().ChangeIntensity(le.intensity);
                            break;
                    }
                }
            }
        }
    }
    public void FiguresApplyToScene(List<FigureElement> figureElements)
    {
        countActiveFigureElements = 0;

        foreach (RailManager.Rail rail in ContentRailMenue.GetComponent<RailManager>().railList)
        {
            foreach (RailManager.Figure obj in rail.myObjects)
            {
                Destroy(obj.figure);
                Destroy(obj.figure3D);
            }
            rail.myObjects.Clear();
            rail.sizeLayering = 1;
        }

        for (int i = 0; i < figureElements.Count; i++)
        {
            foreach (FigureInstanceElement feInstance in figureElements[i].figureInstanceElements)
            {
                countActiveFigureElements++;
                GameObject curr3DObject = ContentRailMenue.GetComponent<RailManager>().CreateNew2DInstance(i, feInstance.moment, feInstance.railStart, feInstance.layer, false);
                curr3DObject.transform.localPosition = new Vector3(curr3DObject.transform.localPosition.x, curr3DObject.transform.localPosition.y, objectsRailElements[feInstance.railStart].transform.GetChild(0).GetComponent<RailSpeedController>().GetDistanceAtTime(feInstance.moment));
                objects3dFigureInstances.Add(curr3DObject);
            }
        }
    }
    public void MusicApplyToScene(List<MusicClipElement> musicClipElements)
    {
        countActiveMusicClips = 0;
        foreach (RailMusicManager.MusicPiece obj in imageTimelineRailMusic.GetComponent<RailMusicManager>().myObjects)
        {
            Destroy(obj.musicPiece);
        }
        imageTimelineRailMusic.GetComponent<RailMusicManager>().myObjects.Clear();
        imageTimelineRailMusic.GetComponent<RailMusicManager>().sizeLayering = 1;

        for (int i = 0; i < musicClipElements.Count; i++)
        {
            foreach (MusicClipElementInstance mceInstance in musicClipElements[i].musicClipElementInstances)
            {
                countActiveMusicClips++;
                imageTimelineRailMusic.GetComponent<RailMusicManager>().CreateNew2DInstance(i, mceInstance.moment, mceInstance.layer);
            }
        }
    }
    public void SetTabActive(int tab)
    {
        switch (tab)
        {
            case 2:
                tmpobjShelfAll.ButtonShelf02();
                break;
            case 5:
                tmpobjShelfAll.ButtonShelf05(false);
                Debug.Log("button 5");
                break;
            case 7:
                tmpobjShelfAll.ButtonShelf07(false);
                break;
        }
    }
    public string CreateJsonFromSceneData(SceneData sceneData)
    {
        string JsonData = JsonUtility.ToJson(sceneData, true);
        return JsonData;
    }
    public SceneData CreateSceneDataFromJSON(string JsonData)
    {
        SceneData sceneData = JsonUtility.FromJson<SceneData>(JsonData);
        return sceneData;
    }
}
