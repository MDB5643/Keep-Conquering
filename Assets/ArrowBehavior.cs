using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public bool latched = false;
    public float speed = 20f;
    public float activeTime = 0.0f;
    public float startingYpos;
    // Update is called once per frame
    private void Start()
    {
        startingYpos = transform.position.y;
    }
    void Update()
    {
        
        if (!transform.name.Contains("MagicArrowUp"))
        {
            transform.position += transform.right * Time.deltaTime * speed;
            transform.position = new Vector2(transform.position.x, startingYpos);
        }
        else
        {
            transform.position += (transform.right * Time.deltaTime * speed)/2;
            transform.position += (transform.up * Time.deltaTime * speed)/2;
        }
            
        activeTime += Time.deltaTime;
        if (activeTime >= .5f && transform.name.Contains("MagicArrowUp"))
        {
            transform.GetComponentInParent<RunebornRanger>().upSpecActive = false;
            Destroy(gameObject);
        }
        if (activeTime >= .8f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (transform.name == "MagicArrowUp")
        {
            transform.GetComponentInChildren<RunebornRanger>().upSpecActive = false;
        }
        Destroy(gameObject);
    }
}
