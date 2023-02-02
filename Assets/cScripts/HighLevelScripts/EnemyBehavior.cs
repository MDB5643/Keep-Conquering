using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    #region Public Variables
    public float attackDistance; //minimum distance for attack
    public float moveSpeed;
    public float timer; //for cooldown between attacks
    public Transform leftLimit;
    public Transform rightLimit;
    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange; //check if player is in range
    public GameObject hotZone;
    public GameObject triggerArea;
    #endregion
    public float minDamage = 0.0f;
    public float currentDamage;
    public Text m_DamageDisplayP2;
    #region Private Variables
    private Animator anim;
    private float distance; //store distance between enemy and player
    private bool attackMode;
    private bool isCooling; //check if enemy is cooling after attack
    private float initTimer;
    #endregion
    
    void Awake()
    {
        SelectTarget();
        initTimer = timer; //store initial value of timer
        anim = GetComponent<Animator>();
        currentDamage = minDamage;
    }

    // Update is called once per frame
    void Update()
    {
            m_DamageDisplayP2.text = currentDamage + "%";

            if (!attackMode)
            {
                Move();
            }

            if (!InsideofLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("skels_attack"))
            {
                SelectTarget();
            }

            if (inRange)
            {
                EnemyLogic();
            }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);
        if(distance > attackDistance)
        {
            StopAttack();
        }
        else if(attackDistance >= distance && isCooling == false)
        {
            Attack();
        }

        if (isCooling)
        {
            anim.SetBool("attack", false);
            
        }
    }

    void Move()
    {
        anim.SetBool("canWalk", true);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("skels_attack"))
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    void Attack()
    {
        timer = initTimer; //reset timer when player enters atttack range 
        attackMode = true; //to check if enemy can still attack

        anim.SetBool("canWalk", false);
        anim.SetBool("attack", true);
    }

    void StopAttack()
    {
        isCooling = false;
        attackMode = false;
        anim.SetBool("attack", false);

    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if(distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }

        Flip();
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if(transform.position.x > target.position.x)
        {
            rotation.y = 180f;
        }
        else
        {
            rotation.y = 0f;
        }

        transform.eulerAngles = rotation;
    }

    public void TakeDamage(float damage)
    {
        currentDamage += damage;

        //Play hurt animation
    }



    public void Knockback(float BaseKB, float contactAngle, float modifierx, float modifiery)
    {
        anim.SetTrigger("tookDamage");

        //Make a vector inverse of collision angle
        float radians = contactAngle * Mathf.Deg2Rad;
        Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        //Calculate knockback force
        KBVector.x = KBVector.x * BaseKB * (currentDamage*0.75f) + modifierx;
        KBVector.y = KBVector.y * BaseKB * (currentDamage * 0.75f) + modifiery;

        var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        e_Rigidbody2D.AddForce(KBVector, ForceMode2D.Impulse);
    }
}
