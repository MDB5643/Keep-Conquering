using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Stars
{
    public GameObject starsPrefab;
    public int starsOrderInLayer;
    [Range(0, 23)]
    public int starHourStart;
    [Range(0, 59)]
    public int starMinuteStart;
    [Range(0, 23)]
    public int starHourFinish;
    [Range(0, 59)]
    public int starMinuteFinish;
    [Range(0, 7200)]
    public int starFadeInAndOutInMinutes;
    [Range(0f, 1f)]
    public float parallaxScale;

    [HideInInspector]
    public GameObject spawnedStars;
    [HideInInspector]
    public Material myMaterial;

    [HideInInspector]
    public int startTimeInSeconds, endTimeInSeconds, fadeTimeInSeconds;
}
