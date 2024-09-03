using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{

    public Vector2 cameraDirectionAndMagnitude;
    // S

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + cameraDirectionAndMagnitude.x, transform.position.y + cameraDirectionAndMagnitude.y, transform.position.z);
    }
}
