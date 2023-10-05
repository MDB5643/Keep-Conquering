using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualHBHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.gameObject.name == "NSpecFire")
        {
            if (transform.GetChild(0) != null)
            {
                transform.GetChild(0).gameObject.layer = transform.gameObject.layer;
                transform.GetChild(1).gameObject.layer = transform.gameObject.layer;
                transform.GetChild(2).gameObject.layer = transform.gameObject.layer;
                transform.GetChild(3).gameObject.layer = transform.gameObject.layer;
                transform.GetChild(4).gameObject.layer = transform.gameObject.layer;
                transform.GetChild(5).gameObject.layer = transform.gameObject.layer;
                transform.GetChild(6).gameObject.layer = transform.gameObject.layer;
            }
        }
    }
}
