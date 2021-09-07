using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageElement {
    public string description;
    public float x;
    public float y;
    public float z;
    public string parent;
    public bool active;
}

public class StageElementList {
    public List<StageElement> stageElements;
}

