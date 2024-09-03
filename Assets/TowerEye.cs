using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEye : MonoBehaviour
{
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    public float currentDamage;
    private SpriteRenderer m_SR;
    public PlayerManager playerManager;

    public GameObject EyePop;
    public GameObject eyePartner;
    public GameObject Launcher;
    public GameObject KeepWall;
    public GameObject MinionTarget;
    public GameObject EyeShot;
    public string teamColor;

    public GameObject Gondola;

    public float flashDuration;
    private float timeSinceDamage;
    public float rotateSpeed;
    public float rotationModifier = -180f;
    private float timeSinceTrigger = 0.0f;
    private float timeBetweenShots = 4f;

    public List<GameObject> enemiesInBounds = new List<GameObject>();

    private AudioManager_PrototypeHero m_audioManager;
    private Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        m_SR = transform.GetComponentInChildren<SpriteRenderer>();
        defaultColor = m_SR.color;

        m_audioManager = AudioManager_PrototypeHero.instance;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceDamage += Time.deltaTime;
        if (timeSinceDamage >= flashDuration)
        {
            m_SR.color = defaultColor;
        }
        if (currentDamage > 100)
        {
            Pop();
        }
        RotateTowardEnemy();

        if (timeSinceTrigger > timeBetweenShots)
        {
            Fire();
            timeSinceTrigger = 0.0f;
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
            if (collision.transform.GetComponentInParent<Conqueror>() && collision.transform.GetComponentInParent<Conqueror>().teamColor == teamColor)
            {
                //do nothing
            }
            else
            {
                collision.GetComponentInParent<CombatManager>().Hit(transform, collision.GetComponent<CollisionTracker>());
            }
            
        }
    }

    public void Fire()
    {
        Vector2 direction = (Vector2)enemiesInBounds[0].transform.position - (Vector2)transform.position;
        GameObject eyeShot = Instantiate(EyeShot, transform, false);
        eyeShot.layer = 0;
        eyeShot.GetComponent<EyeShot>().target = enemiesInBounds[0].transform;

        m_audioManager.PlaySound("EyeShot");
        m_SR.color = Color.yellow;
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
            Instantiate(EyePop, new Vector3(dustSpawnPosition.x, dustSpawnPosition.y), Quaternion.identity);
            //Instantiate(Launcher, new Vector3(14.72f, -10.22f, 0), Quaternion.identity).SetActive(true);
        }
        if (transform.name == "FrontRightEye")
        {
            Instantiate(EyePop, new Vector3(dustSpawnPosition.x, dustSpawnPosition.y), Quaternion.identity);
            //Instantiate(Launcher, new Vector3(66.09f, -10.22f, 0), Quaternion.identity).SetActive(true);
        }

        //GameObject newDust = Instantiate(EyePop, dustSpawnPosition, Quaternion.identity) as GameObject;

        //Final tower behavior
        if (transform.CompareTag("RedKeepEye") || transform.CompareTag("RedMidEye"))
        {
            if (eyePartner == null)
            {
                //KeepWall.SetActive(false);
            }
        }
        else if (transform.CompareTag("RedFrontEye"))
        {
            //Move gondola limit
            //Gondola.GetComponent<PlatformMove>().points[0].SetPositionAndRotation(new Vector3(108f, 5.5f), new Quaternion(0f, 0f, 0f, 0f));
        }
        else if (transform.CompareTag("BlueKeepEye") || transform.CompareTag("BlueMidEye"))
        {
            if (eyePartner == null)
            {
                //KeepWall.SetActive(false);
            }
        }
        else if (transform.CompareTag("BlueFrontEye"))
        {
            //Move gondola limit
            //Gondola.GetComponent<PlatformMove>().points[1].SetPositionAndRotation(new Vector3(-25f, 5.5f), new Quaternion(0f, 0f, 0f, 0f));
        }

        if (MenuEvents.gameModeSelect == 3)
        {
            if (teamColor == "Red")
            {
                playerManager.isRedKeepDestroyed = true;
            }
            if (teamColor == "Blue")
            {
                playerManager.isBlueKeepDestroyed = true;
            }
        }
        //ELIMINATE
        Destroy(gameObject);
    }

    public void RotateTowardEnemy()
    {
        if (enemiesInBounds.Count > 0)
        {
            timeSinceTrigger += Time.deltaTime;
            transform.GetComponentInChildren<Animator>().SetBool("EnemyInRange", true);
            Vector2 vectorToTarget = enemiesInBounds[0].transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.GetComponentInChildren<SpriteRenderer>().transform.rotation = q;
        }
        else
        {
            timeSinceTrigger = 0.0f;
            transform.GetComponentInChildren<Animator>().SetBool("EnemyInRange", false);
        }
    }
}
