using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Lightening
{
    public GameObject myLighteningPrefab;

    [Range(0.1f, 5f)]
    public float lighteningFrequency;
    public LayerMask lighteningCollisionLayers;
    public int lighteningOrderInLayer;
    [HideInInspector]
    public ParticleSystem currentLightening;


}
