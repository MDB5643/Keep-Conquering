using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform jabPoint;
    public Transform upTiltPoint;
    public LayerMask enemyLayers;

    public float jabRange = 0.5f;
    public float jabDamage = 3.5f;
    public float jabKB = 1.0f;

    public float upTiltRange = 0.6f;
    public float upTiltDamage = 3.0f;
    public float upTiltKB = 1.2f;

    public float attackTime = 0.0f;
    public float damageTime;
    public float deathTime;
    public float idleTime;

    private Animator anim;
    private AnimationClip clip;

    public float bufferTime = 0.0f;

    public GameObject impactEffect;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.Log("Error: Did not find anim!");
        }
        else
        {
            //Debug.Log("Got anim");
        }

        UpdateAnimClipTimes();
    }
    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Attack1":
                    attackTime = clip.length;
                    break;
                case "AttackUp":
                    attackTime = clip.length;
                    break;
                case "NeutralSpecial":
                    damageTime = clip.length;
                    break;
                case "Dead":
                    deathTime = clip.length;
                    break;
                case "Idle":
                    idleTime = clip.length;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bufferTime += Time.deltaTime;

        if (bufferTime >= attackTime && !anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerDeath"))
        {
            if (CrossPlatformInputManager.GetButton("BasicAttack"))
            {
                bufferTime = 0;

                if (CrossPlatformInputManager.GetButton("DirectionalUp"))
                {
                    UpTilt();
                }
                else
                {
                    Jab();
                }

            }
            if (CrossPlatformInputManager.GetButton("NeutralSpecial"))
            {
                bufferTime = 0;

                NeutralSpecial();
            }
        }
        
    }

    //public void Knockback(float BaseKB, float contactAngle, float modifierx, float modifiery)
    //{
    //    anim.SetBool("tookDamage", true);
    //
    //    //Make a vector inverse of collision angle
    //    float radians = contactAngle * Mathf.Deg2Rad;
    //    Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    //
    //    //Calculate knockback force
    //    KBVector.x = KBVector.x * BaseKB * (currentDamage * 0.75f) + modifierx;
    //    KBVector.y = KBVector.y * BaseKB * (currentDamage * 0.75f) + modifiery;
    //
    //    var e_Rigidbody2D = GetComponent<Rigidbody2D>();
    //    e_Rigidbody2D.AddForce(KBVector, ForceMode2D.Impulse);
    //}

    void Jab()
    {
        //Play attack animation
        animator.SetTrigger("Attack");
        float damageModifier = 0.0f;

        var playerCollider = GetComponent<BoxCollider2D>();

        Vector2 attackHitboxCenter = jabPoint.position;

        //Detect enemy collision with attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackHitboxCenter, jabRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            //Detect impact angle
            var targetclosestPoint = enemy.ClosestPoint(attackHitboxCenter);
            var sourceclosestPoint = playerCollider.attachedRigidbody.ClosestPoint(attackHitboxCenter);

            var distance = targetclosestPoint - sourceclosestPoint;

            var angle = Vector2.Angle(Vector2.right, distance);

            //Apply damage
            if (enemy.GetComponentInParent<EnemyBehavior>() != null)
            {
                enemy.GetComponentInParent<EnemyBehavior>().TakeDamage(jabDamage);
                //Apply Knockback
                enemy.GetComponentInParent<EnemyBehavior>().Knockback(jabKB, angle, 0, 2);
                //Impact animation
                Instantiate(impactEffect, targetclosestPoint, transform.rotation);
            }
            if (enemy.GetComponentInParent<CPUCharacter2D>() != null)
            {
                enemy.GetComponentInParent<CPUCharacter2D>().TakeDamage(jabDamage);
                //Apply Knockback
                enemy.GetComponentInParent<CPUCharacter2D>().Knockback(jabKB, angle, 0, 2);
                //Impact animation
                Instantiate(impactEffect, targetclosestPoint, transform.rotation);
            }
        }

        //Apply damage
    }

    void UpTilt()
    {
        //Play attack animation
        animator.SetTrigger("UpTilt");
        float damageModifier = 0.0f;

        var playerCollider = GetComponent<BoxCollider2D>();

        Vector2 attackHitboxCenter = upTiltPoint.position;

        //Detect enemy collision with attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(upTiltPoint.position, upTiltRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            var closestPoint = enemy.ClosestPoint(attackHitboxCenter);
            var distance = closestPoint - playerCollider.attachedRigidbody.ClosestPoint(attackHitboxCenter);
            var angle = Vector2.Angle(Vector2.right, distance);
            //if (angle < 135 && angle > 45)
            //{
            //    //underneath
            //}
            //The rest of sides by angle
            //return CollisionSide.None;
            //Impact animation
            Instantiate(impactEffect, closestPoint, transform.rotation);
            //Apply damage
            if(enemy.GetComponentInParent<EnemyBehavior>() != null)
            {
                enemy.GetComponentInParent<EnemyBehavior>().TakeDamage(upTiltDamage);
                //Apply Knockback
                enemy.GetComponentInParent<EnemyBehavior>().Knockback(upTiltKB, angle, 0, 0);
            }
            if (enemy.GetComponentInParent<CPUCharacter2D>() != null)
            {
                enemy.GetComponentInParent<CPUCharacter2D>().TakeDamage(upTiltDamage);
                //Apply Knockback
                enemy.GetComponentInParent<CPUCharacter2D>().Knockback(upTiltKB, angle, 0, 0);
            }
        }

        //Apply damage
    }

    void NeutralSpecial()
    {
        //Play attack animation
        animator.SetTrigger("NeutralSpecial");
        float damageModifier = 0.0f;
 

        //var playerCollider = GetComponent<BoxCollider2D>();
        //
        //Vector2 attackHitboxCenter = upTiltPoint.position;
        //
        ////Detect enemy collision with attack
        //Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(upTiltPoint.position, upTiltRange, enemyLayers);
        //
        //foreach (Collider2D enemy in hitEnemies)
        //{
        //    var closestPoint = enemy.ClosestPoint(attackHitboxCenter);
        //    var distance = closestPoint - playerCollider.attachedRigidbody.ClosestPoint(attackHitboxCenter);
        //    var angle = Vector2.Angle(Vector2.right, distance);
        //    //if (angle < 135 && angle > 45)
        //    //{
        //    //    //underneath
        //    //}
        //    //The rest of sides by angle
        //    //return CollisionSide.None;
        //    //Impact animation
        //    Instantiate(impactEffect, closestPoint, transform.rotation);
        //    //Apply damage
        //    enemy.GetComponentInParent<EnemyBehavior>().TakeDamage(upTiltDamage);
        //    //Apply Knockback
        //    enemy.GetComponentInParent<EnemyBehavior>().Knockback(upTiltKB, angle, 0, 0);
        //}

        //Apply damage
    }

    private void OnDrawGizmosSelected()
    {
        if (jabPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(jabPoint.position, jabRange);
    }
}
