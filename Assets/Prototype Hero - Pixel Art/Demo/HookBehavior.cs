using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    public bool latched = false;
    public float speed = 20f;
    public float activeTime = 0.0f;
    public bool homing = false;
    public GameObject target;
    public GameObject latchedObject;
    public Rigidbody2D rb;
    public float rotateSpeed = 1500f;
    public Vector2 pullForce;
    Conqueror m_Character;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_Character = transform.GetComponentInParent<Conqueror>();
    }

    // Update is called once per frame
    void Update()
    {

        if (latched == false)
        {
            if (transform.GetComponentInParent<Conqueror>().hookDirection == "Up" && target != null)
            {
                Vector2 direction = (Vector2)target.transform.position - rb.position;

                direction.Normalize();

                float rotateAmount = Vector3.Cross(direction, -transform.right).z;

                rb.angularVelocity = -rotateAmount * rotateSpeed;

                rb.velocity = -transform.right * speed;
            }
            else if (transform.GetComponentInParent<Conqueror>().hookDirection == "Up" )
            {
                transform.position += -transform.right * Time.deltaTime * speed;
            }
            else
            {
                transform.position += -transform.right * Time.deltaTime * speed;
            }
            if (activeTime >= .8f)
            {
                m_Character.homingIn = false;
                Destroy(gameObject);
            }
        }
        else
        {
            if (latchedObject != null)
            {
                transform.position = latchedObject.transform.position;
            }
            
            var targetclosestPoint = transform.position;
            var sourceclosestPoint = m_Character.transform.position;

            var positionDifference = targetclosestPoint - sourceclosestPoint;
            float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);
            float radians = angleInRadians * Mathf.Deg2Rad;
            Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            pullForce = KBVector * positionDifference * 7;
            if (transform.GetComponentInParent<Conqueror>().hookDirection == "Up")
            {
                pullForce.y += 15;
            }

            pullForce.y += 5;
            if (Mathf.Abs(positionDifference.x) < 1.8f && Mathf.Abs(positionDifference.y) < 1.8f)
            {
                target = null;
                latchedObject = null;
                m_Character.homingIn = false;
                if (transform.GetComponentInParent<Conqueror>().hookDirection == "Side")
                {
                    m_Character.m_body2d.AddForce(pullForce * .7f);
                }
                Destroy(gameObject);
            }

            else if (transform.GetComponentInParent<Conqueror>().hookDirection == "Up" && m_Character.transform.position.y > transform.position.y)
            {
                target = null;
                latchedObject = null;
                m_Character.homingIn = false;
                Destroy(gameObject);
            }
            else if (transform.GetComponentInParent<Conqueror>().hookDirection == "Side" && Mathf.Abs(positionDifference.x) < .8f)
            {
                target = null;
                latchedObject = null;
                m_Character.homingIn = false;
                m_Character.m_body2d.AddForce(pullForce*.7f);
                Destroy(gameObject);
            }
        }
        activeTime += Time.deltaTime;
        if (activeTime >= 4.5f )
        {
            target = null;
            latchedObject = null;
            m_Character.homingIn = false;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_Character == null)
        {
            Destroy(gameObject);
        }
        else if ((homing == false || (homing == true && target != null && target.name == collision.gameObject.name)) && collision.gameObject != transform.parent.gameObject && !latched)
        {
            if (collision.gameObject.GetComponent<Conqueror>() || collision.gameObject.GetComponent<MinionBehavior>() || collision.gameObject.GetComponent<PlatformMove>())
            {
                latchedObject = collision.gameObject;
            }
            m_Character.homingIn = true;
            latched = true;
            m_Character.m_jumpCount++;
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
            pullForce = KBVector * positionDifference * 2;
            if (transform.GetComponentInParent<Conqueror>().hookDirection == "Up")
            {
                pullForce.y += 10;
            }

            pullForce.y += 5;
            //Destroy(gameObject);
        }

    }
}
