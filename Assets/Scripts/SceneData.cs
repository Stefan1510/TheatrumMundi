using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class objectsLightElement
{
    public GameObject goLightElement;
    public int lightStagePosition;   //{ 0- BeforeRail, 1- leftRail, 2- rightRail, 3- BehindRails, 4- middleLights};
}
[System.Serializable]
public class RailElementSpeed
{
    public float moment;
    public float speed;
}
[System.Serializable]
public class RailElement
{
    //schiene
    public string name;
    public float x;
    public float y;
    public float z;
    public float width;
    public float height;
    public float velocity;
    public string direction;	//"toRight"=band moves from left to right, "toLeft"=band moves from left to right
    public List<RailElementSpeed> railElementSpeeds;
}
[System.Serializable]
public class SceneryElement
{
    //kulisse
    public string name;
    public string description;
    public float x;
    public float y;
    public float z;
    public string parent;
    public int railnumber;
    public bool active;
    public int zPos;//0=backgound, 1=mid-pos, 2=front
    public bool mirrored;
    public bool outline;
}
[System.Serializable]
public class FigureInstanceElement
{
    public string name;
    public int instanceNr;
    public float moment;
    public int railStart;         // empty auf der es sein soll
    public int layer;
}
[System.Serializable]
public class FigureElement
{
    //figur
    public string name;
    public float x;
    public float y;
    public float z;
    public bool mirrored;
    public float width;
    public float height;
    public float velocity;
    public float wheelsize;
    public int railnumber;
    public string direction;
    public List<FigureInstanceElement> figureInstanceElements;
}
[System.Serializable]
public class LightElement
{
    //licht
    public string name;
    public float x;
    public float y;
    public float z;
    public bool active;
    //have some more parameters
    public int railnumber;
    public float r;
    public float g;
    public float b;
    public float intensity;
    public int angle_h;
    public int angle_v;
    public int stagePosition;  //{ 0- BeforeRail, 1- leftRail, 2- rightRail, 3- behind Rail, 4- middle light};
}
[System.Serializable]
public class LightingSet
{
    public float moment;
    public byte r;
    public byte g;
    public byte b;
    public float intensity;
}
[System.Serializable]
public class BackgroundPosition
{
    public float moment;
    public float yPosition;
}
[System.Serializable]
public class MusicClipElementInstance
{
    public string name;
    public int instanceNr;
    public float moment;
    public float layer;
}
[System.Serializable]
public class MusicClipElement
{
    public string name;
    public List<MusicClipElementInstance> musicClipElementInstances;
}

public class SceneData
{
    //enthält alle objekte
    public string fileName;
    public string fileAuthor;
    public string fileDate;
    public string fileComment;
    public int pieceLength;     // in sekunden
    public List<MusicClipElement> musicClipElements;
    public List<RailElement> railElements;
    public List<SceneryElement> sceneryElements;
    public List<FigureElement> figureElements;
    public List<LightElement> lightElements;
    public List<LightingSet> lightingSets;
    public List<BackgroundPosition> backgroundPositions;
}

public class StaticSceneData
{
    private StaticSceneData() { }
    private static SceneData staticData = new SceneData();
    public static SceneData StaticData
    {
        get
        {
            return staticData;
        }
        set
        {
            staticData = value;
        }
    }

    public static void Rails3D()
    {
        GameObject goGameController = GameObject.Find("GameController");
        goGameController.GetComponent<SceneDataController>().RailsApplyToScene(StaticData.railElements);
    }
    public static void Sceneries3D()
    {
        GameObject goGameController = GameObject.Find("GameController");
        goGameController.GetComponent<SceneDataController>().SceneriesApplyToScene(StaticData.sceneryElements);
    }
    public static void Lights3D()
    {
        GameObject goGameController = GameObject.Find("GameController");
        goGameController.GetComponent<SceneDataController>().LightsApplyToScene(StaticData.lightElements);
    }
    public static void Figures3D()
    {
        GameObject goGameController = GameObject.Find("GameController");
        goGameController.GetComponent<SceneDataController>().FiguresApplyToScene(StaticData.figureElements);
    }
    public static void Music()
    {
        GameObject goGameController = GameObject.Find("GameController");
        goGameController.GetComponent<SceneDataController>().MusicApplyToScene(StaticData.musicClipElements);
    }

    public static void Everything3D()
    {
        GameObject goGameController = GameObject.Find("GameController");
        goGameController.GetComponent<SceneDataController>().CreateScene(StaticData);
    }
}