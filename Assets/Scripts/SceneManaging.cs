using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneManaging
{
    public static int objectInIndexTab;
    public static bool playing;
    public static bool dragging;
    public static bool updateMusic = false;
    public static bool highlighted;                 //global variable since only one object can be highlighted - everything else is unhighlighted
    //public static int objectsTimeline;
    public static bool openUp;
    public static bool sceneChanged;
    public static int configMenueActive;
    public static int directorMenueActive;
    public static int mainMenuActive;
    public static bool isPreviewLoaded;    // Variable zur Abfrage, ob die Szene in der Vorschau auch geladen wurde.
    public static bool isExpert;
    
    // flyer
    public static bool flyerActive;
    public static int[] flyerSpace = new int[9];
}
