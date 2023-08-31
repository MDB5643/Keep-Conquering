using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GondolaHotzone : MonoBehaviour
{
    public PlatformMove parentPlatform;
    // Start is called before the first frame update
    void Start()
    {
        parentPlatform = transform.GetComponentInParent<PlatformMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("Hook1"))
        {
            if (collision.gameObject.GetComponent<Conqueror>() )
            {
                //add tags for red vs blue team and update later
                parentPlatform.blueMinionCount++;
            }
            if (collision.gameObject.GetComponent<MinionBehavior>())
            {
                parentPlatform.redMinionCount++;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("Hook1"))
        {
            if (collision.gameObject.GetComponent<Conqueror>() && !collision.gameObject.name.Contains("Hook1"))
            {
                //add tags for red vs blue team and update later
                if (parentPlatform.blueMinionCount > 0)
                {
                    parentPlatform.blueMinionCount--;
                }

            }
            if (collision.gameObject.GetComponent<MinionBehavior>())
            {
                if (parentPlatform.redMinionCount > 0)
                {
                    parentPlatform.redMinionCount--;
                }

            }
        }
    }
}
