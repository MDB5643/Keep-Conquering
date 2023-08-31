using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEye : MonoBehaviour
{
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    public float currentDamage;
    private SpriteRenderer m_SR;

    public GameObject EyePop;
    public GameObject eyePartner;
    public GameObject Launcher;
    public GameObject KeepWall;
    public GameObject MinionTarget;

    public GameObject Gondola;

    public float flashDuration;
    private float timeSinceDamage;

    private Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        m_SR = transform.GetComponentInChildren<SpriteRenderer>();
        defaultColor = m_SR.color;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceDamage += Time.deltaTime;
        if (timeSinceDamage >= flashDuration)
        {
            m_SR.color = defaultColor;
        }
        if (currentDamage > 70)
        {
            Pop();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.GetComponentInParent<ChampJab1>() != null)
        //{
        //    collision.GetComponentInParent<ChampJab1>().Hit(transform);
        //}
        if (collision.transform.tag == "HotZone")
        {
            //do nothing
        }
        else if (collision.transform.tag == "AttackHitbox")
        {
            collision.GetComponentInParent<CombatManager>().Hit(transform, collision.transform.name);
        }
    }

    public void TakeDamage(float damage)
    {
        currentDamage += damage;
        timeSinceDamage = 0.0f;

        m_SR.color = Color.red;

        //Play hurt animation
        //m_animator.SetTrigger("Hurt");
    }

    public void Pop()
    {
        // Set dust spawn position
        Vector2 dustSpawnPosition = transform.position;

        if (transform.name == "FrontLeftEye")
        {
            Instantiate(EyePop, new Vector3(dustSpawnPosition.x + 3.2f, dustSpawnPosition.y + 1.85f), Quaternion.identity);
            Instantiate(Launcher, new Vector3(14.72f, -10.22f, 0), Quaternion.identity).SetActive(true);
        }
        if (transform.name == "FrontRightEye")
        {
            Instantiate(EyePop, new Vector3(dustSpawnPosition.x + 3.2f, dustSpawnPosition.y + 1.85f), Quaternion.identity);
            Instantiate(Launcher, new Vector3(66.09f, -10.22f, 0), Quaternion.identity).SetActive(true);
        }

        GameObject newDust = Instantiate(EyePop, dustSpawnPosition, Quaternion.identity) as GameObject;

        //Final tower behavior
        if (transform.CompareTag("RedKeepEye") || transform.CompareTag("RedMidEye"))
        {
            if (eyePartner == null)
            {
                KeepWall.SetActive(false);
            }
        }
        else if (transform.CompareTag("RedFrontEye"))
        {
            //Move gondola limit
            Gondola.GetComponent<PlatformMove>().points[0].SetPositionAndRotation(new Vector3(108f, 5.5f), new Quaternion(0f, 0f, 0f, 0f));
        }
        else if (transform.CompareTag("BlueKeepEye") || transform.CompareTag("BlueMidEye"))
        {
            if (eyePartner == null)
            {
                KeepWall.SetActive(false);
            }
        }
        else if (transform.CompareTag("BlueFrontEye"))
        {
            //Move gondola limit
            Gondola.GetComponent<PlatformMove>().points[1].SetPositionAndRotation(new Vector3(-25f, 5.5f), new Quaternion(0f, 0f, 0f, 0f));
        }


        //ELIMINATE
        Destroy(gameObject);
    }
}