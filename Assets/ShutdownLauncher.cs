using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutdownLauncher : MonoBehaviour
{
    public GameObject Launcher;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("MinionBoardGondola"))
        {
            Launcher.SetActive(false);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("MinionBoardGondola"))
        {
            Launcher.SetActive(true);
        }
    }
}
