using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBar : MonoBehaviour
{
    public float shieldHealth = 56.0f;
    public Sprite highHealth1;
    public Sprite highHealth2;
    public Sprite highHealth3;
    public Sprite medHealth1;
    public Sprite medHealth2;
    public Sprite medHealth3;
    public Sprite lowHealth1;
    public Sprite lowHealth2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shieldHealth >= 48)
        {
            transform.GetComponent<SpriteRenderer>().sprite = highHealth1;
        }
        else if (shieldHealth < 48 && shieldHealth >= 40)
        {
            transform.GetComponent<SpriteRenderer>().sprite = highHealth2;
        }
        else if (shieldHealth < 40 && shieldHealth >= 32)
        {
            transform.GetComponent<SpriteRenderer>().sprite = highHealth3;
        }
        else if (shieldHealth < 32 && shieldHealth >= 24)
        {
            transform.GetComponent<SpriteRenderer>().sprite = medHealth1;
        }
        else if (shieldHealth < 24 && shieldHealth >= 16)
        {
            transform.GetComponent<SpriteRenderer>().sprite = medHealth2;
        }
        else if (shieldHealth < 16 && shieldHealth >= 8)
        {
            transform.GetComponent<SpriteRenderer>().sprite = medHealth3;
        }
        else if (shieldHealth < 8)
        {
            transform.GetComponent<SpriteRenderer>().sprite = lowHealth1;
        }
    }
}
