using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeShot : MonoBehaviour
{
    public Transform target;
    private Rigidbody2D rb;

    public float speed = 15f;

    public float rotateSpeed = 1500f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = (Vector2)target.position - rb.position;

        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;

        rb.velocity = transform.up * speed;
    }

    
}
