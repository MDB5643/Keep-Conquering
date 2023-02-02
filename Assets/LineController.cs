using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;
    public bool active = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] points)
    {
        lr.enabled = true;
        lr.positionCount = points.Length;
        this.points = points;
        active = true;
    }

    private void Update()
    {
        if (points != null)
        {
            if (points[0] != null && points[1] != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    lr.SetPosition(i, points[i].position);
                }
            }
            else
            {
                lr.enabled = false;
            }
        }
        else
        {
            lr.enabled = false;
        }
        
    }
}
