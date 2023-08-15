using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ConfigLight : MonoBehaviour
{
    [SerializeField] private Image imgBb;
    [SerializeField] private Sprite spriteGrey, spriteYellow;
    [SerializeField] private Image imgBbRight;
    [SerializeField] private Sprite spriteGreyRight, spriteYellowRight;
    public void ToggleBb()
    {
        if (this.GetComponent<Toggle>().isOn)
        {
            imgBb.sprite = spriteYellow;
            imgBbRight.sprite = spriteYellowRight;
            imgBbRight.color = new Color(1, 1, 1, 1f);
        }
        else
        {
            imgBb.sprite = spriteGrey;
            imgBbRight.sprite = spriteGreyRight;
            imgBbRight.color = new Color(1, 1, 1, 0.5f);
        }
    }
}
