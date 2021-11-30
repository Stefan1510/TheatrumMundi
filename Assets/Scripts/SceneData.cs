using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public bool active;
	public int zPos;//0=backgound, 1=mid-pos, 2=front
	public bool mirrored;
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
	public int r;
	public int g;
	public int b;
	public float intensity;
	public int angle_h;
	public int angle_v;
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
}