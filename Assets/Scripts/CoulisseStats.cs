using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoulisseStats : MonoBehaviour
{
    public float CoulisseWidth;
    public float CoulisseHeight;
    public string description;
    public int typeOfCoulisse;       // 0: kleine blende, 1: grosse Blende, 2: seitenkulisse    (Blende = Schienenabdeckung -> die muessen weiter runter, damit sie nicht die kulissen verdecken)
}
