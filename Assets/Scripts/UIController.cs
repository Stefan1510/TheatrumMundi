using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject goMenueKulissen;
    public GameObject[] goCollection = null;
    public GameObject[] goButtonSceneryElements;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SceneriesApplyToUI()
    {
        //string buttonName;
        //GameObject goButtonScenery;
        
        goMenueKulissen.SetActive(true);
        foreach (SceneryElement se in StaticSceneData.StaticData.sceneryElements)
        {
            foreach (GameObject buttonSe in goButtonSceneryElements)
            {
                if ("Button" + se.name == buttonSe.name)
                {
                    buttonSe.GetComponent<DragDrop>().ThisSceneryElement = StaticSceneData.StaticData.sceneryElements.Find(x => x.name == se.name);
                    if (se.active)
                    {
                        
                        buttonSe.transform.SetParent(goCollection[se.railnumber - 1].transform);
                        buttonSe.GetComponent<RectTransform>().anchoredPosition = new Vector2(se.z * 300, (se.y - 0.1f) * 300);
                        buttonSe.GetComponent<DragDrop>().schieneKulisse = se.railnumber;
                        buttonSe.GetComponent<DragDrop>().statusReiter = se.railnumber;
                    }
                    else 
                    {
                        buttonSe.transform.SetParent(buttonSe.GetComponent<DragDrop>().parentStart.transform);
                        buttonSe.GetComponent<RectTransform>().anchoredPosition = buttonSe.GetComponent<DragDrop>().pos;
                        buttonSe.GetComponent<DragDrop>().schieneKulisse = 0;
                        //buttonSe.GetComponent<DragDrop>().statusReiter = 1;
                    }

                    //GameObject.Find("Reiter" + se.railnumber + "Active").SetActive(false);
                    Debug.Log("----- Schiene-Kulisse: +++" + se.railnumber + "+++");
                }
            }
            /*if (se.active)
            {
                buttonName = "Button" + se.name;
                Debug.Log("+++++" + buttonName + "+++++");
                try
                {
                    se.railnumber = se.railnumber = int.Parse(se.parent.Substring(7));
                    goButtonScenery = GameObject.Find(buttonName);
                    goButtonScenery.GetComponent<DragDrop>().ThisSceneryElement = StaticSceneData.StaticData.sceneryElements.Find(x => x.name == se.name);
                    goButtonScenery.transform.SetParent(goCollection[se.railnumber - 1].transform);
                    goButtonScenery.GetComponent<RectTransform>().anchoredPosition = new Vector2(se.z * 300, (se.y - 0.1f) * 300);
                    goButtonScenery.GetComponent<DragDrop>().schieneKulisse = se.railnumber;
                    goButtonScenery.GetComponent<DragDrop>().statusReiter = se.railnumber;

                    //GameObject.Find("Reiter" + se.railnumber + "Active").SetActive(false);
                    Debug.Log("----- Schiene-Kulisse: +++" + se.railnumber + "+++");
                }
                catch
                {
                    Debug.Log("leider nein");
                }
            }*/
        }

        goMenueKulissen.SetActive(false);
    }


}
