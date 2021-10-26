using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RailElement
{
    public string name;
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class SceneryElement
{
    public string name;
    public float x;
    public float y;
    public float z;
    public string parent;
    public bool active;
}

[System.Serializable]
public class LightElement
{
    public string name;
    public float x;
    public float y;
    public float z;
    public bool active;
    //have some more parameters
}

public class SceneData
{
    public string fileName;
    public string fileAuthor;
    public string fileDate;
    public string fileComment;
    public List<RailElement> railElements;
    public List<SceneryElement> sceneryElements;
    public List<LightElement> lightElements;
}
