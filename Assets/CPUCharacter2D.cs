using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    public class CPUCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private float m_SecondJumpForce = 300f;                  // Amount of force added when the player double jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private LayerMask m_WhatIsPortal;

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .01f; // Radius of the overlap circle to determine if grounded
        public bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        public double m_DamagePercentage = 0.0f;
        public bool m_FacingRight = true;  // For determining which way the player is currently facing.
        public bool m_Jump;
        private bool m_JumpedOnce = false;
        private bool m_JumpedTwice = false;
        public float m_JumpCooldown = .08f;
        public float m_TimeSinceJump = 0.0f;
        public float m_playerHeight;
        public float m_playerWidth;

        public Text m_DamageDisplay;
        public Text m_InfoDisplay;

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
        private Animator anim;
        private float distance; //store distance between enemy and player
        private bool attackMode;
        private bool isCooling; //check if enemy is cooling after attack
        private float initTimer;

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
        private float hitStunTime;

        private AnimationClip clip;

        public float bufferTime = 0.0f;

        public GameObject impactEffect;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_playerHeight = transform.localScale.y;
            m_playerWidth = transform.localScale.x;

            SelectTarget();
            initTimer = timer; //store initial value of timer
            anim = GetComponent<Animator>();
            currentDamage = minDamage;
        }

        private void Update()
        {
            m_DamageDisplay.text = currentDamage + "%";
            if (!attackMode)
            {
                Move(0f, false);
            }

            if (!InsideofLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("NeutralBasicAttack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("UpTiltAttack"))
            {
                SelectTarget();
            }

            if (inRange || attackMode)
            {
                EnemyLogic();
            }
        }

        public void UpdateAnimClipTimes()
        {
            AnimationClip[] clips = m_Anim.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                switch (clip.name)
                {
                    case "HitStun":
                        hitStunTime = clip.length;
                        break;
                }
            }
        }

        void EnemyLogic()
        {
            distance = Vector2.Distance(transform.position, target.position);
            if (distance > attackDistance)
            {
                StopAttack();
            }
            else if(isCooling)
            {
                anim.SetBool("attack", false);
                isCooling = false;
            }
            else if (attackDistance >= distance && isCooling == false && !anim.GetCurrentAnimatorStateInfo(0).IsName("NeutralBasicAttack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("UpTiltAttack"))
            {
                Attack();
            }

        }

        void Attack()
        {
            timer = initTimer; //reset timer when player enters atttack range 
            attackMode = true; //to check if enemy can still attack

            isCooling = true;
            anim.SetBool("canWalk", false);
            anim.SetTrigger("Attack");

            var playerCollider = GetComponent<BoxCollider2D>();

            Vector2 attackHitboxCenter = jabPoint.position;

            //Detect enemy collision with attack
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackHitboxCenter, jabRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                //Detect impact angle
                var targetclosestPoint = enemy.ClosestPoint(attackHitboxCenter);
                var sourceclosestPoint = playerCollider.attachedRigidbody.ClosestPoint(attackHitboxCenter);

                
                var distance = targetclosestPoint - sourceclosestPoint;

                

                var angle = 0.0f;
                if (transform.rotation.y == 1)
                {
                    if (distance.x > 0)
                    {
                        distance.x = -distance.x;
                    }
                    angle = Vector2.Angle(Vector2.right, distance);
                }
                else
                {
                    if (distance.x < 0)
                    {
                        distance.x = -distance.x;
                    }
                    angle = Vector2.Angle(Vector2.right, distance);
                }
                
                //Impact animation
                Instantiate(impactEffect, targetclosestPoint, transform.rotation);
                //Apply damage
                enemy.GetComponentInParent<PlatformerCharacter2D>().TakeDamage(jabDamage);
                //Apply Knockback
                enemy.GetComponentInParent<PlatformerCharacter2D>().Knockback(jabKB, angle, 0, 2f);
            }
        }

        void StopAttack()
        {
            isCooling = false;
            attackMode = false;
            anim.SetBool("attack", false);

        }



        private void FixedUpdate()
        {

        }

        //Called whenever player collides with something
        void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag("hitbox"))
            {
                m_DamagePercentage += 3.0;
            }

        }

        private void OnCollisionStay2D(Collision2D coll)
        {
            
        }

        private void OnCollisionExit2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag("PortalForeground"))
            {
                m_InfoDisplay.text = "";
            }
        }

        private void CheckOverlaps()
        {
            
        }


        public void Move(float move, bool crouch)
        {

            anim.SetBool("canWalk", true);
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("NeutralBasicAttack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("UpTiltAttack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("InHitStun"))
            {
                Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                m_Anim.SetFloat("Speed", Mathf.Abs(moveSpeed));
            }

            //Tracking jumps
            m_JumpCooldown = .1f;
            if (m_Jump)
            {
                if (!m_JumpedOnce)
                {
                    m_JumpedOnce = true;
                    m_TimeSinceJump = 0.0f;
                }
                else if (m_JumpedOnce && !m_JumpedTwice && m_TimeSinceJump > m_JumpCooldown)
                {
                    // Read the jump input in Update so button presses aren't missed.
                    if (m_Jump)
                    {
                        m_JumpedTwice = true;
                    }
                }
                else
                {
                    m_Jump = false;
                }

            }

            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            //RaycastHit2D groundContact = Physics2D.Raycast(transform.position, -Vector2.up, m_CharDimensions.rect.height / 2);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    if ((colliders[i].gameObject.CompareTag("MidGround") && !transform.CompareTag("PlayerMid")) || (colliders[i].gameObject.CompareTag("Foreground") && !transform.CompareTag("Player")))
                    {
                        Physics2D.IgnoreCollision(colliders[i], gameObject.GetComponent<BoxCollider2D>(), true);
                    }
                    else
                    {
                        if(!(colliders[i].gameObject.name == "left_limit_cpu") && !(colliders[i].gameObject.name == "right_limit_cpu"))
                        {
                            Physics2D.IgnoreCollision(colliders[i], gameObject.GetComponent<BoxCollider2D>(), false);
                            m_Grounded = true;
                        }
                        
                    }
                }

            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);


            if (m_Grounded)
            {
                m_JumpedOnce = false;
                m_JumpedTwice = false;
                m_TimeSinceJump = 0.11f;
            }


            // If the player should jump...
            if (m_JumpedOnce && m_Jump && !m_JumpedTwice)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                Vector2 newVelocity = m_Rigidbody2D.velocity;
                newVelocity.y = 0f;
                m_Rigidbody2D.velocity = newVelocity;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            // If the player should double jump...
            if (!m_Grounded && m_Jump && m_JumpedTwice)
            {
                Vector2 newVelocity = m_Rigidbody2D.velocity;
                newVelocity.y = 0f;
                m_Rigidbody2D.velocity = newVelocity;
                m_Rigidbody2D.AddForce(new Vector2(0f, 15f), ForceMode2D.Impulse);
            }
        }


        public void Flip()
        {
            Vector3 rotation = transform.eulerAngles;
            if (transform.position.x > target.position.x)
            {
                rotation.y = 180f;
            }
            else
            {
                rotation.y = 0f;
            }

            transform.eulerAngles = rotation;
        }
        private bool InsideofLimits()
        {
            return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
        }

        public void SelectTarget()
        {
            float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
            float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

            if (distanceToLeft > distanceToRight)
            {
                target = leftLimit;
            }
            else
            {
                target = rightLimit;
            }

            Flip();
        }


        public void TakeDamage(float damage)
        {
            currentDamage += damage;

            //Play hurt animation
        }



        public void Knockback(float BaseKB, float contactAngle, float modifierx, float modifiery)
        {
            anim.SetTrigger("Hitstun");

            //Make a vector inverse of collision angle
            float radians = contactAngle * Mathf.Deg2Rad;
            Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

            //Calculate knockback force
            KBVector.x = KBVector.x * BaseKB * (currentDamage * 0.75f) + modifierx;
            KBVector.y = KBVector.y * BaseKB * (currentDamage * 0.75f) + modifiery;

            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            e_Rigidbody2D.AddForce(KBVector, ForceMode2D.Impulse);
        }
    }
}
