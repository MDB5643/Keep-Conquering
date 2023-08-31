using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionJumpPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<MinionBehavior>() != null)
        {
            other.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-1, 2), ForceMode2D.Impulse);
        }
            
    }
}
