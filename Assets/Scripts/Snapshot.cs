using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Snapshot : MonoBehaviour
{
    [SerializeField] Image preview;
    Camera snapCam;

    int resWidth = 256;
    int resHeight = 256;

    void Awake()
    {
        snapCam = GetComponent<Camera>();
        if (snapCam.targetTexture == null)
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        preview.enabled = false;
    }
    public void CallTakeSnapshot(string fileName)
    {
        Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        snapCam.Render();
        RenderTexture.active = snapCam.targetTexture;
        snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        byte[] bytes = snapshot.EncodeToJPG();
        fileName += "_preview.jpg";
        File.WriteAllBytes(fileName, bytes);
    }
    public void ShowSnapshot(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;
        filePath = filePath.Substring(0, filePath.Length - 5);
        filePath += "_preview.jpg";

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            preview.enabled = true;
            preview.sprite = mySprite; // = Application.persistentDataPath + "/" + fileName + "_preview.jpg";}
        }
    }
    public void SaveTexture2DToFile(Texture2D tex, string filePath, int jpgQuality = 95)
    {
        File.WriteAllBytes(filePath + ".jpg", tex.EncodeToJPG(jpgQuality));
    }
    public void HidePreview()
    {
        preview.enabled = false;
    }
}
