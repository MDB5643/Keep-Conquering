using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public int redMinionCount = 0;
    public int blueMinionCount = 0;
    public float speed;
    public int startingPoint;
    public Transform[] points;

    private int i;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.name == "Gondola")
        {
            if (redMinionCount > blueMinionCount)
            {
                //move platform to the point position
                transform.position = Vector3.MoveTowards(transform.position, points[1].position, speed * Time.deltaTime);
            }
            else if (redMinionCount < blueMinionCount)
            {
                //move platform to the point position
                transform.position = Vector3.MoveTowards(transform.position, points[0].position, speed * Time.deltaTime);
            }
            else
            {
                
            }
        }

        else
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

            transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.name.Contains("Hook"))
        {
            collision.transform.SetParent(transform);
            collision.rigidbody.interpolation = RigidbodyInterpolation2D.None;
        }
        else
        {
            collision.gameObject.GetComponent<HookBehavior>().latchedObject = transform.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.name.Contains("Hook"))
        {
            collision.transform.SetParent(null);
            collision.rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }
}
