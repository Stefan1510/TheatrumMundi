using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneManaging
{
    public static DragDrop dragDrop;                // aktuelles dragDrop-Script ("this" in Dragdrop)
    public static int triggerActive = new int();
    public static bool triggerEinstellungen = new bool();
    public static bool mouse_over = new bool();
    public static bool deleteButtonPressed = new bool();
    public static bool showSettings = new bool();

    public static int statusReiter = new int();
    public static bool anyTimelineOpen;				//timeline open/close
    public static bool playing;
    public static bool updateMusic;
    public static int timelineHit;
    public static bool highlighted = false;                 //global variable since only one object can be highlighted - everything else is unhighlighted


}
