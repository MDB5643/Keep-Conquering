using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTracker : MonoBehaviour
{
    public float baseDamage;
    public float baseKB;
    public float hitStun;
    public float damageModifier;
    public float KBModifier;
    public float modifierY;
    public float modifierX;
    public float multModY;
    public float multModX;
    public float shakeIntensity;
    public bool shakeScreen;
    public bool smash;
    public bool grab;
    public bool continuous;
    public bool groundPound;
    public bool noHitStop;
    public string soundName;
    

    public List<string> hitEnemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
