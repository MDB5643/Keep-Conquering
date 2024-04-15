using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcSlam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.GetComponentInParent<Conqueror>().m_grounded)
        {
            Destroy(gameObject);
        }
    }

}
