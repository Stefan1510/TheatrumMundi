using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject goMenueKulissen;

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
        GameObject goButtonScenery = null;
        string buttonName;
        goMenueKulissen.SetActive(true);
        foreach (SceneryElement se in StaticSceneData.StaticData.sceneryElements)
        {
            if (se.active)
            {
                buttonName = "Button" + se.name;
                Debug.Log("+++++" + buttonName + "+++++");
                try
                {
                    goButtonScenery = GameObject.Find(buttonName);
                    se.railnumber = se.railnumber = int.Parse(se.parent.Substring(7));
                    goButtonScenery.transform.SetParent(GameObject.Find("Collection" + se.railnumber).transform);
                    goButtonScenery.GetComponent<RectTransform>().anchoredPosition = new Vector2(se.x * 300, (se.y - 0.1f) * 300);
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
