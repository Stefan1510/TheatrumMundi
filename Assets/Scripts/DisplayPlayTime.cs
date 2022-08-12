using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayTime : MonoBehaviour
{
    [SerializeField] private Text _textTime;
    [SerializeField] private Text _textMaxTime;
    // Start is called before the first frame update
    void Start()
    {
        _textTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetTime());
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
    }

    // Update is called once per frame
    void Update()
    {
        _textTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetTime());
        _textMaxTime.text = UtilitiesTm.FloaTTimeToString(AnimationTimer.GetMaxTime());
    }
}
