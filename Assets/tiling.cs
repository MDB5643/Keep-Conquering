using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]

public class tiling : MonoBehaviour
{
    public int offsetX = 2;
    public bool hasARightBuddy = false;
    public bool hasALeftBuddy = false;

    public bool reverseScale = false;

    private float spriteWidth = 0f;
    private Camera cam;
    private Transform myTransform;

    void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasALeftBuddy == false || hasARightBuddy == false)
        {
            //calculate the camera's extent - half the width of what the camera can see in world coordinates
            float camHorizontalExtent = cam.orthographicSize * Screen.width / Screen.height;

            //calculate x position where the camera can see the edge of the sprite
            float edgeVisiblePosRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtent;
            float edgeVisiblePosLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtent;

            if (cam.transform.position.x >= edgeVisiblePosRight - offsetX && hasARightBuddy == false)
            {
                MakeNewBuddy(1);
                hasARightBuddy = true;
            }
            else if(cam.transform.position.x <= edgeVisiblePosLeft + offsetX && hasALeftBuddy == false)
            {
                MakeNewBuddy(-1);
                hasALeftBuddy = true;
            }
        }
    }


    //function that creates a buddy on the side required
    void MakeNewBuddy(int rightOrLeft)
    {
        //calculating new position for new buddy
        Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);
        //Instantiate(myTransform, newPosition, myTransform.rotation);
        //instantiating new buddy and storing in variable
        Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation) as Transform;
        
        //if not tilable revers the x size of our object to get rid of ugly seams
        if (reverseScale == true)
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
        }

        newBuddy.parent = myTransform;
        if (rightOrLeft > 0)
        {
            newBuddy.GetComponent<tiling>().hasALeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<tiling>().hasARightBuddy = true;
        }
    }
}
