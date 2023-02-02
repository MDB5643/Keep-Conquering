using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform[] backgrounds;       // Array of all back and foregrounds to be parallaxed
    private float[] parallaxScales;       // proportion of the camera's movement to move the backgrounds by
    public float smoothing = 1f;               // how smooth parallax will be

    private Transform cam;
    private Vector3 previousCamPos; //store precision of camera in previous frame

    //called before start(), great for references
    void Awake()
    {
        //set up reference to camera
        cam = Camera.main.transform;

    }

    // Start is called before the first frame update
    void Start()
    {
        // store previous frame, which had current frame's camera position
        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];

        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z*-1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //for each background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // the parallax is the opposite of the camera movement because the previous frame multiplied by the scale
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            //set a target x position which is the current position + the parallax
            float backgroundTargetPositionX = backgrounds[i].position.x + parallax;

            //create a target position which is the background's current position with its target x position
            Vector3 backgroundTargetPosition = new Vector3(backgroundTargetPositionX, backgrounds[i].position.y, backgrounds[i].position.z);

            // fade between current position and the target position using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPosition, smoothing * Time.deltaTime);
        }

        //set previous cam pos to camera's position at the end of the frame
        previousCamPos = cam.position;
    }
}
