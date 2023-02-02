using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;

    private int i;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        //check distance between the platform and the point
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++; 
            if (i == points.Length)
            {
                i = 0;
            }
        }

        //move platform to the point position
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y > transform.position.y)
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
    }
}
