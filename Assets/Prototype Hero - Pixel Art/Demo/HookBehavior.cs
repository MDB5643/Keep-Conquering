using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    public bool latched = false;
    public float speed = 20f;
    public float activeTime = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (latched == false)
        {

            if(transform.GetComponentInParent<Conqueror>().hookDirection == "Up")
            {
                transform.position += -transform.up * Time.deltaTime * speed;
            }
            else
            {
                transform.position += -transform.right * Time.deltaTime * speed;
            }
            
        }
        activeTime += Time.deltaTime;
        if(activeTime >= .8f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Conqueror m_Character = transform.GetComponentInParent<Conqueror>();
        latched = true;

        m_Character.m_jumpCount++;
        m_Character.m_animator.SetTrigger("Jump");
        m_Character.m_grounded = false;
        m_Character.m_animator.SetBool("Grounded", m_Character.m_grounded);
        m_Character.m_groundSensor.Disable(0.2f);

        //Detect impact angle
        var targetclosestPoint = transform.position;
        var sourceclosestPoint = m_Character.transform.position;

        var positionDifference = targetclosestPoint - sourceclosestPoint;

        //Must be done to detect y axis angle
        float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);
        float radians = angleInRadians * Mathf.Deg2Rad;
        Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 pullForce = KBVector * positionDifference * 2;
        if (transform.GetComponentInParent<Conqueror>().hookDirection == "Up")
        {
            pullForce.y += 10;
        }
            
        pullForce.y += 5;
        // Convert the angle to degrees.
        float attackAngle = angleInRadians * Mathf.Rad2Deg;
        m_Character.GetComponent<Rigidbody2D>().AddForce(pullForce, ForceMode2D.Impulse);
        Destroy(gameObject);
    }
}
