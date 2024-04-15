using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAlive : MonoBehaviour
{
    private GameObject audioManager;


    // Start is called before the first frame update
    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
