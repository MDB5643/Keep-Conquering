using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    private float lingerTime = 0.0f;
    public float maxLingerTime = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lingerTime += Time.deltaTime;
        if (lingerTime >= maxLingerTime && gameObject != null)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
