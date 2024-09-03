using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DistinctTimeShade
{
    [Range(0f, 23f)]
    public int hour;
    [Range(0f, 59f)]
    public int minute;
    public Gradient myGradient;

}
