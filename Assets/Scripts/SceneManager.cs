using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneManager
{
    public static DragDrop dragDrop;        // aktuelles dragDrop-Script ("this" in Dragdrop)
    //public static ReiterButton activeReiter;
    public static int triggerActive = new int();
    public static bool triggerEinstellungen = new bool();
    public static bool mouse_over = new bool();
    public static bool deleteButtonPressed = new bool();
    public static bool showSettings = new bool();

    public static int statusReiter = new int();

}
