using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public bool latched = false;
    public float speed = 20f;
    public float activeTime = 0.0f;
    public float startingYpos;

    public GameObject explosion;
    public GameObject chargeBall;
    public GameObject splashBox;

    public bool charged;
    public bool reverse;

    public string teamColor;
    // Update is called once per frame
    private void Start()
    {
        startingYpos = transform.position.y;
        if (gameObject.name.StartsWith("RRFSpecHB"))
        {
        }
        if (transform.name.Contains("MagicArrowDown"))
        {
        }
        if (transform.name.Contains("MagicArrowUp"))
        {
        }
    }
    void Update()
    {
        if (gameObject.name.StartsWith("RRFSpecHB"))
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
        else if (transform.name.Contains("MagicArrowDown"))
        {
            transform.position += transform.right * Time.deltaTime * speed / 2;
            transform.position += transform.up * Time.deltaTime * speed / 2;
        }
        else if (transform.name.Contains("MagicArrowUp"))
        {
            transform.position += transform.right * Time.deltaTime * speed / 2;
            transform.position += transform.up * Time.deltaTime * speed / 2;
            activeTime += Time.deltaTime;
            if (activeTime >= .6f)
            {
                Destroy(gameObject);
            }
        }
        else if (transform.name.Contains("MagicArrow"))
        {
            transform.position += transform.right * Time.deltaTime * speed;
            transform.position = new Vector2(transform.position.x, startingYpos);
            if (activeTime >= .8f)
            {
                Destroy(gameObject);
            }
        }
        else if (transform.name.Contains("RRSplash"))
        {
            transform.position += transform.right * Time.deltaTime * speed;
            transform.position = new Vector2(transform.position.x, startingYpos);
            if (activeTime >= .2f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (transform.name.StartsWith("RRFSpecHB"))
        {
            
            GameObject activeHitbox = Instantiate(explosion, new Vector3((transform.position.x), transform.position.y, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), null);
            if (charged)
            {
                activeHitbox.transform.localScale = new Vector3(6, 4, 1);
                activeHitbox.transform.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            activeHitbox.transform.GetComponent<ProjectileBehavior>().teamColor = transform.GetComponent<ProjectileBehavior>().teamColor;
            activeHitbox.layer = transform.gameObject.layer;
            AudioManager_PrototypeHero.instance.PlaySound("RRMagicExplode");
            Destroy(gameObject);
        }
        if (transform.name.StartsWith("MagicArrowDown"))
        {
            if (collision.transform.GetComponent<Conqueror>() && collision.transform.GetComponent<Conqueror>().teamColor == teamColor)
            {
                Physics2D.IgnoreCollision(collision.collider, transform.GetComponent<PolygonCollider2D>());
            }
            else if (!collision.transform.name.Contains("Platform") && !collision.transform.GetComponent<Conqueror>())
            {
                if (charged)
                {
                    Quaternion rotQuat = new Quaternion();
                    float xDisplace = 0.0f;
                    if (!reverse)
                    {
                        rotQuat = new Quaternion(0f, 0f, 0f, 0f);
                        xDisplace = 1f;
                    }
                    else
                    {
                        rotQuat = new Quaternion(0f, 180f, 0f, 0f);
                        xDisplace = -1f;
                    }
                    GameObject activeHitbox = Instantiate(splashBox, new Vector3((transform.position.x), transform.position.y + .3f, transform.position.z),
                rotQuat, null);
                }
                Destroy(gameObject);
            }
            else if (!collision.transform.GetComponent<Conqueror>())
            {
                Physics2D.IgnoreCollision(collision.collider, transform.GetComponent<PolygonCollider2D>());
            }
        }
        if (transform.name == "MagicArrowUp")
        {
            if (collision.transform.GetComponent<Conqueror>() && collision.transform.GetComponent<Conqueror>().teamColor == teamColor)
            {
                Physics2D.IgnoreCollision(collision.collider, transform.GetComponent<PolygonCollider2D>());
            }
            else if (!collision.transform.name.Contains("Platform") && !collision.transform.GetComponent<Conqueror>())
            {
                Destroy(gameObject);
                transform.GetComponentInChildren<RunebornRanger>().upSpecActive = false;
            }
            else if (!collision.transform.GetComponent<Conqueror>())
            {
                Physics2D.IgnoreCollision(collision.collider, transform.GetComponent<PolygonCollider2D>());
            }
        }
        else if (transform.name == "MagicArrow")
        {
            if (!collision.transform.name.Contains("Platform") && !collision.transform.GetComponent<Conqueror>() && !collision.transform.GetComponent<MinionBehavior>())
            {
                Destroy(gameObject);
            }
        }
    }
}
