using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class objectsLightElement
{
    public GameObject goLightElement;
    public int lightStagePosition;   //{ 0- BeforeRail, 1- leftRail, 2- rightRail, 3- BehindRails};
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
}


[System.Serializable]

public class FigureInstanceElement
{
    public float instanceNr;
    public float momentStart;
    public float railStart;
}
[System.Serializable]
public class FigureElement
{
	//figur
    public string name;
    public float x;
    public float y;
    public float z;
    public string parent;
    public bool active;
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
    public int stagePosition;  //{ 0- BeforeRail, 1- leftRail, 2- rightRail};
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

//[System.Serializable]
public class SceneData
{
	//enthält alle objekte
    public string fileName;
    public string fileAuthor;
    public string fileDate;
    public string fileComment;
    public List<RailElement> railElements;
    public List<SceneryElement> sceneryElements;
	public List<FigureElement> figureElements;
    public List<LightElement> lightElements;
    public List<LightingSet> lightingSets;
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

    public static void Everything3D()
    {
        GameObject goGameController = GameObject.Find("GameController");
        goGameController.GetComponent<SceneDataController>().CreateScene(StaticData);
    }
}