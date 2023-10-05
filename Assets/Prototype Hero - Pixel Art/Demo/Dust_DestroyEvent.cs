using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust_DestroyEvent : MonoBehaviour
{
    public void destroyEvent()
    {
        if (gameObject.transform.parent != null && !gameObject.transform.parent.GetComponent<Conqueror>())
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
