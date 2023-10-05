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

    public string teamColor;
    // Update is called once per frame
    private void Start()
    {
        startingYpos = transform.position.y;
    }
    void Update()
    {
        if (gameObject.name.StartsWith("RRFSpecHB"))
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
        if (transform.name.Contains("MagicArrowDown"))
        {
            transform.position += transform.right * Time.deltaTime * speed / 2;
            transform.position += transform.up * Time.deltaTime * speed / 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (transform.name.StartsWith("RRFSpecHB"))
        {
            GameObject activeHitbox = Instantiate(explosion, new Vector3((transform.position.x), transform.position.y, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), null);
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
            else if (!collision.transform.name.Contains("Platform"))
            {
                Destroy(gameObject);
            }
            else
            {
                Physics2D.IgnoreCollision(collision.collider, transform.GetComponent<PolygonCollider2D>());
            }
        }
    }
}
