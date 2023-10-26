using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutDialog : MonoBehaviour
{
    [SerializeField] private Texture2D _textureHand;
    public void OpenLink(string str)
    {
        if (str == "skd")
            Application.OpenURL("https://skd-online-collection.skd.museum/Kollektion?id=1544");
        else if (str == "br")
            Application.OpenURL("https://www.bundesregierung.de/breg-de/bundesregierung/bundeskanzleramt/staatsministerin-fuer-kultur-und-medien");
        else if (str == "nsk")
            Application.OpenURL("https://www.bundesregierung.de/breg-de/bundesregierung/bundeskanzleramt/staatsministerin-fuer-kultur-und-medien/neustart-kultur-startet-1767056");
        else if (str == "m40")
            Application.OpenURL("https://www.museum4punkt0.de/");
        else if (str == "htw")
            Application.OpenURL("https://www.htw-dresden.de/");
        else if (str == "dre")
            Application.OpenURL("http://www.drematrix.de/");
        else if (str == "light")
            Application.OpenURL("https://lightframefx.de/");
    }
    public void ChangeMouseIcon(bool hoverOver)
    {
        if(hoverOver)
        Cursor.SetCursor(_textureHand,new Vector2(_textureHand.width/2,0),CursorMode.Auto);
        else
        Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
    }
}
