using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneManaging
{
    public static int triggerActive = new int();
    public static int objectInIndexTab;
    public static bool triggerEinstellungen = new bool();
    public static bool mouse_over = new bool();
    public static bool deleteButtonPressed = new bool();
    public static bool showSettings = new bool();

    public static int statusReiter = new int();
    public static bool anyTimelineOpen;				//timeline open/close
    public static bool playing;
    public static bool updateMusic;
    public static int timelineHit;
    public static bool highlighted;                 //global variable since only one object can be highlighted - everything else is unhighlighted

    public static int objectsTimeline;
    public static bool openUp;
    public static bool hittingSomething;
    public static int hitObject;
    public static int configMenueActive;
    public static int directorMenueActive;
    public static int mainMenuActive;

    public static bool isPreviewLoaded;    // Variable zur Abfrage, ob die Szene in der Vorschau auch geladen wurde.
}
