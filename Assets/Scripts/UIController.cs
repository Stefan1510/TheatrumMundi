using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject goMenueKulissen;
    public GameObject[] goCollection = null;

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
        string buttonName;
        GameObject goButtonScenery;
        goMenueKulissen.SetActive(true);
        foreach (SceneryElement se in StaticSceneData.StaticData.sceneryElements)
        {
            if (se.active)
            {
                buttonName = "Button" + se.name;
                Debug.Log("+++++" + buttonName + "+++++");
                try
                {
                    se.railnumber = se.railnumber = int.Parse(se.parent.Substring(7));
                    goButtonScenery = GameObject.Find(buttonName);
                    goButtonScenery.transform.SetParent(goCollection[se.railnumber-1].transform);
                    goButtonScenery.GetComponent<RectTransform>().anchoredPosition = new Vector2(se.z * 300, (se.y - 0.1f) * 300);
                    //goButtonScenery.GetComponent<DragDrop>().schieneKulisse = se.railnumber;
                    //goButtonScenery.GetComponent<DragDrop>().statusReiter = se.railnumber;
                    //GameObject.Find("Reiter" + se.railnumber + "Active").SetActive(false);
                    Debug.Log("----- Schiene-Kulisse: +++" + se.railnumber+"+++");
                }
                catch
                {
                    Debug.Log("leider nein");
                }
            }
        }
        goMenueKulissen.SetActive(false);
    }


}
