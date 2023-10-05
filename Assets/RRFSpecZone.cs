using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRFSpecZone : MonoBehaviour
{
    public float speed = 6f;
    public float activeTime = 0.0f;
    public float startingYpos;
    public bool released = false;

    // Update is called once per frame
    private void Start()
    {
            startingYpos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!released)
        {
            transform.position += (transform.right * Time.deltaTime * speed);
            transform.position = new Vector2(transform.position.x, startingYpos);
        }
        
        activeTime += Time.deltaTime;
        if (activeTime >= 2f)
        {
            Destroy(gameObject);
        }
    }
}

