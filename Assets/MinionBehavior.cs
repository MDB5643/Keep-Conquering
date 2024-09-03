using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinionBehavior : MonoBehaviour
{
    public string teamColor;

    public float m_runSpeed = 3.5f;
    public float m_walkSpeed = 2.0f;
    public float m_jumpForce = 7.5f;
    public float m_dodgeForce = 8.0f;
    public float m_parryKnockbackForce = 4.0f;
    public bool m_noBlood = false;
    public bool m_hideSword = false;
    public float attackDistance = 2f;
    public Text m_DamageDisplay;
    public float minDamage = 0.0f;
    public float currentDamage;
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform target;
    public GameObject firstLauncherTarget;
    public GameObject MovingPlatformTarget;
    public GameObject GondolaTarget;
    public GameObject GondolaEdgeTarget;
    public GameObject TowerPlatformTarget;
    public GameObject TowerPlatformEdgeTarget;
    public GameObject EyeTarget;
    public GameObject KeepEyeTarget;
    public GameObject EyeShotFX;
    public GameObject LightAttackFX;
    private string CurrentJumpTarget = "";
    public bool inRange; //check if player is in range
    public GameObject hotZone;
    public GameObject triggerArea;
    public bool isGrappled;
    public bool m_isInHotZone = false;
    public bool m_isInHitStop;
    bool m_isInKnockback = false;
    float m_timeSinceKnockBack = 3.0f;
    public float m_KnockBackDuration = 0.5f;
    public float m_KnockBackMomentumX = 0.0f;
    public float m_KnockBackMomentumY = 0.0f;
    public bool isInStartUp = false;
    public float m_hitStopDuration;
    public float m_timeSinceHitStop = 0.0f;

    public CombatManager combatManager;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private SpriteRenderer m_SR;
    private Sensor_Prototype m_groundSensor;
    private Sensor_Prototype m_wallSensorR1;
    private Sensor_Prototype m_wallSensorR2;
    private Sensor_Prototype m_wallSensorL1;
    private Sensor_Prototype m_wallSensorL2;

    public bool m_grounded = false;
    private bool m_moving = false;
    public bool m_dead = false;
    private bool m_dodging = false;
    private bool m_wallSlide = false;
    private bool m_ledgeGrab = false;
    private bool m_ledgeClimb = false;
    private bool m_crouching = false;
    public bool m_inHitStun = false;
    public bool m_launched = false;
    public bool m_idle = false;

    private Vector3 m_climbPosition;
    public int m_facingDirection = 1;
    private float m_disableMovementTimer = 0.0f;
    private float m_parryTimer = 0.0f;
    private float m_respawnTimer = 0.0f;
    private Vector3 m_respawnPosition = Vector3.zero;
    private int m_currentAttack = 0;
    public float m_timeSinceAttack = 0.0f;
    private float m_gravity;
    public float m_maxSpeed = 4.5f;
    public LayerMask m_WhatIsPortal;

    private float m_timeSinceStun = 5.0f;
    private float m_timeSinceHitStun = 0.0f;
    private float m_hitStunDuration = 0.0f;

    //Knockback after hitstun
    public float incomingKnockback = 0.0f;
    public float incomingAngle = 0.0f;
    public float incomingXMod = 0.0f;
    public float incomingYMod = 0.0f;

    //Combat
    public Transform jabPoint;
    public Transform upTiltPoint;
    public Transform backPoint;
    public Transform downPoint;
    public LayerMask enemyLayers;

    public float jabRange;
    public float jabDamage = 3.0f;
    public float jabKB = .5f;

    public float upTiltRange = 2f;
    public float upTiltDamage = 3.0f;
    public float upTiltKB = 1.2f;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
        m_gravity = m_body2d.gravityScale;

        combatManager = gameObject.AddComponent<CombatManager>();

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

        if (MenuEvents.gameModeSelect == 1)
        {
            if (teamColor == "Red")
            {
                firstLauncherTarget = GameObject.Find("RedMinionLauncherTarget");
                MovingPlatformTarget = GameObject.Find("RedMinionPlatformTarget");
                GondolaTarget = GameObject.Find("RedMinionGondolaTarget");
                EyeTarget = GameObject.Find("RedMinionEyeTarget");
            }
            else if (teamColor == "Blue")
            {
                firstLauncherTarget = GameObject.Find("BlueMinionLauncherTarget");
                MovingPlatformTarget = GameObject.Find("BlueMinionPlatformTarget");
                GondolaTarget = GameObject.Find("BlueMinionGondolaTarget");
                EyeTarget = GameObject.Find("BlueMinionEyeTarget");
                KeepEyeTarget = GameObject.Find("BlueMinionKeepEyeTarget");
                TowerPlatformTarget = GameObject.Find("Jumppoint5Left");
                GondolaEdgeTarget = GameObject.Find("RightEdge");
            }
            target = firstLauncherTarget.transform;
        }
        if (MenuEvents.gameModeSelect == 3 || MenuEvents.gameModeSelect == 4 || MenuEvents.gameModeSelect == 4)
        {
            if (teamColor == "Red")
            {
                EyeTarget = GameObject.Find("RedMinionEyeTarget");
            }
            else if (teamColor == "Blue")
            {
                EyeTarget = GameObject.Find("BlueMinionEyeTarget");
            }
            if (EyeTarget)
            {
                target = EyeTarget.transform;
            }
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        if (MenuEvents.gameModeSelect == 1)
        {
            if (EyeTarget == null && KeepEyeTarget != null)
            {
                EyeTarget = KeepEyeTarget;
            }

            if (teamColor == "Red" && transform.position.x < TowerPlatformTarget.transform.position.x && EyeTarget != null)
            {
                if (EyeTarget.name != "MinionKeepEyeTarget" && EyeTarget.name != "BlueMinionKeepEyeTarget")
                {
                    target = EyeTarget.transform;
                }
                else if (transform.position.x < -37 && KeepEyeTarget != null)
                {
                    target = EyeTarget.transform;
                }
            }
            if (teamColor == "Blue" && transform.position.x > TowerPlatformTarget.transform.position.x && EyeTarget != null)
            {
                if (EyeTarget.name != "MinionKeepEyeTarget" && EyeTarget.name != "BlueMinionKeepEyeTarget")
                {
                    target = EyeTarget.transform;
                }
                else if (transform.position.x > 120 && KeepEyeTarget != null)
                {
                    target = EyeTarget.transform;
                }
            }

            if (target == null)
            {
                target = TowerPlatformTarget.transform;
                CurrentJumpTarget = "Gondola";
            }
        }

        else if (MenuEvents.gameModeSelect == 3 || MenuEvents.gameModeSelect == 4)
        {
            if (target == null)
            {
                if (EyeTarget != null)
                {
                    target = EyeTarget.transform;
                }
                else
                {
                    target = transform;
                }
            }
        }

        if (isGrappled)
        {
            m_disableMovementTimer = 1.0f;
        }
        // Check for interactable overlapping objects
        CheckOverlaps();

        if (currentDamage >= 15)
        {
            currentDamage = 0.0f;
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            m_respawnTimer = 1f;
            DisableWallSensors();
            m_dead = true;
        }


        // Decrease death respawn timer 
        m_respawnTimer -= Time.deltaTime;

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Decrease timer that checks if we are in parry stance
        m_parryTimer -= Time.deltaTime;

        // Decrease timer that disables input movement. Used when attacking
        m_disableMovementTimer -= Time.deltaTime;

        // Respawn Hero if dead
        if (m_dead && m_respawnTimer < 0.0f)
            DeleteDead();

        if (m_dead)
            return;

        Move();

       
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void Move()
    {
        if (!m_inHitStun && !isGrappled && !m_dead)
        {
            if (!m_isInHitStop)
            {

                //Check if character just landed on the ground
                if (!m_grounded && m_groundSensor.State())
                {
                    m_grounded = true;
                    m_animator.SetBool("Grounded", m_grounded);

                }

                //Check if character just started falling
                if (m_grounded && !m_groundSensor.State())
                {
                    m_grounded = false;
                    m_animator.SetBool("Grounded", m_grounded);
                }

                if (m_grounded)
                {
                    m_launched = false;
                }

                // -- Handle input and movement
                // SlowDownSpeed helps decelerate the characters when stopping
                float SlowDownSpeed = m_moving ? 1.0f : 0.5f;
                float KBSlowDownSpeed = 0.8f;
                // Set movement
                if (!m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_animator.GetBool("isParrying") && m_disableMovementTimer < 0.0f && !m_launched)
                {
                    if (!m_isInKnockback)
                    {
                        m_timeSinceKnockBack = 0.0f;
                        if (m_KnockBackMomentumX <= 1 && m_KnockBackMomentumY <= 1)
                        {
                            if (m_animator.GetBool("isParrying") )
                            {
                                m_body2d.velocity = new Vector2(m_walkSpeed * SlowDownSpeed, m_body2d.velocity.y);
                            }
                            else
                            {
                                if (Mathf.Abs(m_body2d.velocity.x) >= 6.5)
                                {

                                    if (!m_grounded)
                                    {
                                        m_body2d.velocity = new Vector2(m_body2d.velocity.x * .4f, m_body2d.velocity.y);
                                    }
                                    else
                                    {
                                        m_body2d.velocity = new Vector2(m_body2d.velocity.x * .6f, m_body2d.velocity.y);
                                    }

                                }
                                else
                                {
                                    //Default ground/air movement
                                    Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
                                    if (m_grounded)
                                    {
                                        if (target.GetComponentInParent<Conqueror>() != null && Mathf.Abs(targetPosition.x - transform.position.x) <= .5f)
                                        {
                                            //spacing while attacking
                                        }
                                        else
                                        {
                                            transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_runSpeed * Time.deltaTime);
                                        }


                                        m_moving = true;
                                    }
                                    
                                }

                            }

                        }
                        else
                        {
                            m_body2d.velocity = new Vector2(m_KnockBackMomentumX, m_KnockBackMomentumY);
                            
                            m_KnockBackMomentumX = m_KnockBackMomentumX * KBSlowDownSpeed;
                            m_KnockBackMomentumY = m_KnockBackMomentumY * KBSlowDownSpeed;
                        }

                    }
                    else
                    {
                        m_timeSinceKnockBack += Time.deltaTime;
                        if (m_timeSinceKnockBack > m_KnockBackDuration)
                        {
                            m_moving = false;
                            m_isInKnockback = false;
                            m_KnockBackMomentumX = m_body2d.velocity.x;
                            m_KnockBackMomentumY = m_body2d.velocity.y;
                            m_animator.SetBool("Knockback", false);
                        }

                    }
                }


                // Set AirSpeed in animator
                m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

                // Set Animation layer for hiding sword
                int boolInt = m_hideSword ? 1 : 0;
                m_animator.SetLayerWeight(1, boolInt);

                // Check if all sensors are setup properly
                if (m_wallSensorR1 && m_wallSensorR2 && m_wallSensorL1 && m_wallSensorL2)
                {
                    bool prevWallSlide = m_wallSlide;
                    //Wall Slide
                    // True if either both right sensors are colliding and character is facing right
                    // OR if both left sensors are colliding and character is facing left
                    m_wallSlide = (m_wallSensorR1.State() && m_wallSensorR2.State() && m_facingDirection == 1) || (m_wallSensorL1.State() && m_wallSensorL2.State() && m_facingDirection == -1);
                    if (m_grounded)
                        m_wallSlide = false;
                    m_animator.SetBool("WallSlide", m_wallSlide);
                    //Play wall slide sound
                    if (prevWallSlide && !m_wallSlide)
                        AudioManager_PrototypeHero.instance.StopSound("WallSlide");


                    //Grab Ledge
                    // True if either bottom right sensor is colliding and top right sensor is not colliding 
                    // OR if bottom left sensor is colliding and top left sensor is not colliding 
                    bool shouldGrab = !m_ledgeClimb && !m_ledgeGrab && ((m_wallSensorR1.State() && !m_wallSensorR2.State()) || (m_wallSensorL1.State() && !m_wallSensorL2.State()));
                    if (shouldGrab)
                    {
                        Vector3 rayStart;
                        if (m_facingDirection == 1)
                            rayStart = m_wallSensorR2.transform.position + new Vector3(0.2f, 0.0f, 0.0f);
                        else
                            rayStart = m_wallSensorL2.transform.position - new Vector3(0.2f, 0.0f, 0.0f);

                        var hit = Physics2D.Raycast(rayStart, Vector2.down, 1.0f);

                        GrabableLedge ledge = null;
                        if (hit)
                            ledge = hit.transform.GetComponent<GrabableLedge>();

                        if (ledge)
                        {
                            m_ledgeGrab = true;
                            m_body2d.velocity = Vector2.zero;
                            m_body2d.gravityScale = 0;

                            m_climbPosition = ledge.transform.position + new Vector3(ledge.topClimbPosition.x, ledge.topClimbPosition.y, 0);
                            if (m_facingDirection == 1)
                                transform.position = ledge.transform.position + new Vector3(ledge.leftGrabPosition.x, ledge.leftGrabPosition.y, 0);
                            else
                                transform.position = ledge.transform.position + new Vector3(ledge.rightGrabPosition.x, ledge.rightGrabPosition.y, 0);
                        }
                        m_animator.SetBool("LedgeGrab", m_ledgeGrab);
                    }

                }


                // -- Handle Animations --
                //Death
                var distance = Vector2.Distance(transform.position, target.position);

                //Attack

                if (attackDistance >= distance && inRange && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 1f)
                {
                    if (target.GetComponentInParent<MinionBehavior>() && target.GetComponentInParent<MinionBehavior>().m_dead)
                    {
                        //do nothing
                    }
                    else
                    {
                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        m_currentAttack++;

                        // Loop back to one after second attack
                        if (m_currentAttack > 2)
                            m_currentAttack = 1;

                        // Reset Attack combo if time since last attack is too large
                        if (m_timeSinceAttack > 1.0f)
                            m_currentAttack = 1;

                        // Call one of the two attack animations "Attack1" or "Attack2"
                        m_animator.SetTrigger("Attack");

                        //Attack("jab", jabPoint);

                        // Disable movement 
                        m_disableMovementTimer = 0.35f;
                    }

                }


                //Run
                else if (m_moving)
                {
                    m_animator.SetInteger("AnimState", 2);
                    m_maxSpeed = m_runSpeed;
                }

                //Idle
                else
                    m_animator.SetInteger("AnimState", 0);
            }
            else
            {
                m_body2d.velocity = Vector2.zero;
                m_body2d.gravityScale = 0;
                m_timeSinceHitStop += Time.deltaTime;
                m_animator.speed = 0;
                m_moving = false;
                if (m_timeSinceHitStop >= m_hitStopDuration)
                {
                    m_body2d.gravityScale = 1.25f;
                    m_isInHitStop = false;
                    m_animator.speed = 1;
                }
            }
        }
        else
        {
            m_moving = false;
            m_timeSinceHitStun += Time.deltaTime;
            if (m_timeSinceHitStun >= m_hitStunDuration)
            {
                Knockback(incomingKnockback, incomingAngle, incomingXMod, incomingYMod);
                m_isInKnockback = true;
            }
        }
    }

    //Handle collisions w/o sensor
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("PortalForeground"))
        {
            //infoText.text = "Press Tab to Teleport";
            if (Input.GetButtonDown("Submit"))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 15, 25);
                transform.tag = "PlayerMid";
                gameObject.layer = LayerMask.NameToLayer("PlayerMid");
                var currRenderer = gameObject.GetComponent<SpriteRenderer>();
                currRenderer.sortingLayerName = "Background";
            }

        }
        if (coll.gameObject.CompareTag("BlastZone"))
        {
            currentDamage = 0.0f;
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            m_respawnTimer = 2.5f;
            DisableWallSensors();
            m_dead = true;
        }

        if (coll.gameObject.CompareTag("RightLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            e_Rigidbody2D.AddForce(new Vector2(42, 8), ForceMode2D.Impulse);
        }
        //if (coll.gameObject.CompareTag("LeftLauncher"))
        //{
        //    var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        //    m_launched = true;
        //    m_disableMovementTimer = 0.1f;
        //    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
        //    e_Rigidbody2D.AddForce(new Vector2(-46, 8), ForceMode2D.Impulse);
        //    target = MovingPlatformTarget.transform;
        //}
        if ((coll.gameObject.name == "MovingPlatformRight" || coll.gameObject.name == "MovingPlatformLeft") && coll.gameObject.transform.position.y > transform.position.y)
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_disableMovementTimer = 0.1f;
            target = GondolaTarget.transform;
        }

    }

    private void CheckOverlaps()
    {
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useLayerMask = true;
        contactFilter2D.SetLayerMask(m_WhatIsPortal);

        List<Collider2D> collisions = new List<Collider2D>();
        int colCount = collider.OverlapCollider(contactFilter2D, collisions);
        int i = 0;
        bool atPortal = false;
        foreach (Collider2D coll in collisions)
        {
            //Check for portal
            if (coll.gameObject.CompareTag("PortalForeground"))
            {
                atPortal = true;
                //infoText.text = "Press Tab";
                if (CrossPlatformInputManager.GetButtonDown("Submit"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 15, 25);
                    transform.tag = "PlayerMid";
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                    var currRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                    currRenderer.sortingLayerName = "Background";
                }
            }
            if (coll.gameObject.CompareTag("MinionJumpUp"))
            {
                var e_Rigidbody2D = GetComponent<Rigidbody2D>();
                e_Rigidbody2D.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);
                m_animator.SetTrigger("Jump");
            }
            if (coll.gameObject.CompareTag("MinionJumpLeft"))
            {
                var e_Rigidbody2D = GetComponent<Rigidbody2D>();
                e_Rigidbody2D.AddForce(new Vector2(-8, 3), ForceMode2D.Impulse);
                m_animator.SetTrigger("Jump");
            }

        }
        if (atPortal == false)
        {
            //infoText.text = "";
        }
    }


    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform t in obj.transform)
        {
            SetLayerRecursively(t.gameObject, newLayer);
        }
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("PortalForeground"))
        {
            //infoText.text = "";
        }
    }

    // Function used to spawn a dust effect
    // All dust effects spawns on the floor
    // dustXoffset controls how far from the player the effects spawns.
    // Default dustXoffset is zero
    public void SpawnDustEffect(GameObject dust, float dustXOffset = 0, float dustYOffset = 0)
    {
        if (dust != null)
        {
            // Set dust spawn position
            Vector3 dustSpawnPosition = transform.position + new Vector3(dustXOffset * m_facingDirection, dustYOffset, 0.0f);
            GameObject newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity) as GameObject;
            // Turn dust in correct X direction
            newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(m_facingDirection, 1, 1);
        }
    }

    void DisableWallSensors()
    {
        m_ledgeGrab = false;
        m_wallSlide = false;
        m_ledgeClimb = false;
        m_wallSensorR1.Disable(0.8f);
        m_wallSensorR2.Disable(0.8f);
        m_wallSensorL1.Disable(0.8f);
        m_wallSensorL2.Disable(0.8f);
        m_body2d.gravityScale = m_gravity;
        m_animator.SetBool("WallSlide", m_wallSlide);
        m_animator.SetBool("LedgeGrab", m_ledgeGrab);
    }

    // Called in AE_resetDodge in PrototypeHeroAnimEvents
    public void ResetDodging()
    {
        m_dodging = false;
    }

    public void SetPositionToClimbPosition()
    {
        transform.position = m_climbPosition;
        m_body2d.gravityScale = m_gravity;
        m_wallSensorR1.Disable(3.0f / 14.0f);
        m_wallSensorR2.Disable(3.0f / 14.0f);
        m_wallSensorL1.Disable(3.0f / 14.0f);
        m_wallSensorL2.Disable(3.0f / 14.0f);
        m_ledgeGrab = false;
        m_ledgeClimb = false;
    }

    public bool IsWallSliding()
    {
        return m_wallSlide;
    }

    public void DisableMovement(float time = 0.0f)
    {
        m_disableMovementTimer = time;
    }

    public void FindTargets()
    {

    }

    public void SelectTarget()
    {
        if (EyeTarget != null)
        {
            float distanceToEye = Vector2.Distance(transform.position, EyeTarget.transform.position);
        }
        if (MenuEvents.gameModeSelect == 1)
        {
            if (teamColor == "Red")
            {
                if (transform.position.x < firstLauncherTarget.transform.position.x && transform.position.x > MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge")
                {
                    target = MovingPlatformTarget.transform;
                }
                if (transform.position.y >= 0f && transform.position.x < MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge" && CurrentJumpTarget != "EnemyPlatform" && CurrentJumpTarget != "EnemyKeep")
                {
                    m_idle = false;
                    target = GondolaTarget.transform;
                }
            }
            else if (teamColor == "Blue")
            {
                if (transform.position.x > firstLauncherTarget.transform.position.x && transform.position.x < MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge")
                {
                    target = MovingPlatformTarget.transform;
                }
                if (transform.position.y <= 0f && transform.position.x > MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge" && CurrentJumpTarget != "EnemyPlatform" && CurrentJumpTarget != "EnemyKeep")
                {
                    m_idle = false;
                    target = GondolaTarget.transform;
                }
            }
            if (CurrentJumpTarget == "EnemyPlatform")
            {
                target = GondolaEdgeTarget.transform;
            }
        }
        
        else if (MenuEvents.gameModeSelect == 3 || MenuEvents.gameModeSelect == 4)
        {
            target = EyeTarget.transform;
        }
        Flip();
    }

    public void Flip()
    {
        if (target == null)
        {
            if (EyeTarget != null)
            {
                target = EyeTarget.transform;
            }
            else
            {
                target = transform; 
            }
        }

        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x < target.position.x)
        {
            rotation.y = 180f;
            m_facingDirection = -1;
        }
        else
        {
            rotation.y = 0f;
            m_facingDirection = 1;
        }

        transform.eulerAngles = rotation;
    }

    void DeleteDead()
    {
        GameObject pManager = GameObject.Find("PlayerManager");
        
        if (teamColor == "Blue")
        {
            pManager.GetComponent<PlayerManager>().blueMinionCount--;
        }
        else if (teamColor == "Red")
        {
            pManager.GetComponent<PlayerManager>().redMinionCount--;
        }
        Destroy(gameObject);
    }

    public void HitStun(float stunTime)
    {
        if (!m_inHitStun)
        {
            m_timeSinceHitStun = 0.0f;

            m_disableMovementTimer = stunTime;
            m_hitStunDuration = stunTime;

            m_inHitStun = true;
        }
        //m_animator.SetTrigger("HitStun");

    }

    public void TakeDamage(float damage)
    {
        currentDamage += damage;

        //Play hurt animation
        m_animator.SetTrigger("Hurt");
    }

    public void Knockback(float BaseKB, float contactAngle, float modifierx, float modifiery)
    {
        m_inHitStun = false;
        m_disableMovementTimer = 0.1f;

        //Make a vector inverse of collision angle
        float radians = contactAngle * Mathf.Deg2Rad;
        Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        BaseKB *= (currentDamage);

        //Calculate knockback force
        Vector2 KBForce = KBVector * BaseKB;
        KBForce.x += modifierx;
        KBForce.y += modifiery;

        var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
        e_Rigidbody2D.AddForce(KBForce*3, ForceMode2D.Impulse);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "MinionBoardGondola" && m_grounded && (target.name == "RedMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "LeftEdge" || target.name == "BlueMinionGondolaTarget" || target.name == "RightEdge" || target.CompareTag("MinionJumpUp")) && !m_launched)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 17f), ForceMode2D.Impulse);
            m_launched = true;
            target = GondolaEdgeTarget.transform;
            CurrentJumpTarget = "EnemyPlatform";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "targetingHotzone" && m_isInHotZone && collision.GetComponentInParent<TowerEye>().teamColor != teamColor)
        {
            collision.GetComponentInParent<TowerEye>().enemiesInBounds.Remove(transform.gameObject);
            m_isInHotZone = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.GetComponentInParent<ChampJab1>() != null)
        //{
        //    collision.GetComponentInParent<ChampJab1>().Hit(transform);
        //}
        if (collision.gameObject.name == "targetingHotzone" && !m_isInHotZone && collision.GetComponentInParent<TowerEye>().teamColor != teamColor)
        {
            collision.GetComponentInParent<TowerEye>().enemiesInBounds.Add(transform.gameObject);
            m_isInHotZone = true;
        }
        else if (collision.transform.tag == "HotZone")
        {
            //do nothing
        }
        else if (collision.transform.tag == "AttackHitbox" && !m_dead)
        {
            collision.GetComponentInParent<CombatManager>().Hit(transform, collision.GetComponent<CollisionTracker>());
        }
        if (collision.transform.tag == "EyeShot" && collision.GetComponentInParent<TowerEye>().teamColor != teamColor)
        {

            //Detect impact angle
            var targetclosestPoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
            var sourceclosestPoint = new Vector2(transform.position.x, transform.position.y);

            var positionDifference = sourceclosestPoint - targetclosestPoint;

            //Must be done to detect y axis angle
            float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

            // Convert the angle to degrees.
            float attackAngle = angleInRadians * Mathf.Rad2Deg;

            TakeDamage(10f);

            incomingAngle = attackAngle;
            incomingKnockback = .8f;
            incomingXMod = 2f;
            incomingYMod = (2f + (currentDamage / 4));
            HitStun(.2f);

            Instantiate(EyeShotFX, new Vector3((targetclosestPoint.x), targetclosestPoint.y, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            GameObject.Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("LeftLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-27, 13), ForceMode2D.Impulse);
            target = MovingPlatformTarget.transform;
        }
        if (collision.gameObject.CompareTag("LeftTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-7, 21), ForceMode2D.Impulse);
            target = GondolaTarget.transform;
            CurrentJumpTarget = "Gondola";
        }
        if (collision.gameObject.CompareTag("RightLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(27, 13), ForceMode2D.Impulse);
            target = MovingPlatformTarget.transform;
        }
        if (collision.gameObject.CompareTag("RightTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(7, 21), ForceMode2D.Impulse);
            target = GondolaTarget.transform;
            CurrentJumpTarget = "Gondola";
        }
        if (MenuEvents.gameModeSelect == 1)
        {
            if (teamColor == "Red")
            {
                if (collision.transform.tag == "MinionBoardRight" && m_grounded && target.name != "RedMinionPlatformTarget" && !target.name.Contains("EyeTarget"))
                {
                    //set velocity to zero to not carry momentum
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-5.5f, 18f), ForceMode2D.Impulse);
                    //SelectTarget();
                    target = GondolaEdgeTarget.transform;
                    CurrentJumpTarget = "EnemyPlatform";
                    m_grounded = false;
                    m_launched = true;
                    DisableWallSensors();
                    m_groundSensor.Disable(.2f);
                }
                if (collision.transform.tag == "MinionBoardLeft" && m_grounded && target.name != "RedMinionPlatformTarget" && !target.name.Contains("EyeTarget"))
                {
                    //set velocity to zero to not carry momentum
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(5.5f, 18f), ForceMode2D.Impulse);
                    //SelectTarget();
                    target = GondolaEdgeTarget.transform;
                    CurrentJumpTarget = "EnemyPlatform";
                    m_grounded = false;
                    m_launched = true;
                    DisableWallSensors();
                    m_groundSensor.Disable(.2f);
                }
                if (collision.transform.tag == "MinionJumpToMainPlatform" && target.name != "RedMinionGondolaTarget" && m_grounded && CurrentJumpTarget == "MainPlatform")
                {
                    //set velocity to zero to not carry momentum
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-6f, 6f), ForceMode2D.Impulse);
                    m_launched = true;
                    //SelectTarget();
                    target = GondolaTarget.transform;
                    CurrentJumpTarget = "Gondola";
                    m_animator.SetTrigger("Jump");
                }
                if (collision.transform.tag == "MinionJumpLeft")
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-6f, 5), ForceMode2D.Impulse);
                    CurrentJumpTarget = "VerticalPlatform";
                    SelectTarget();
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "MinionJumpUp" && !m_idle && m_grounded && CurrentJumpTarget == "VerticalPlatform")
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
                    m_launched = true;
                    CurrentJumpTarget = "MainPlatform";
                    m_animator.SetTrigger("Jump");
                }
                //else if (collision.transform.tag == "MinionBoardGondola" && m_grounded && (target.name == "RedMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "LeftEdge") && !m_launched)
                //{
                //    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                //    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 17f), ForceMode2D.Impulse);
                //    m_launched = true;
                //    target = GondolaEdgeTarget.transform;
                //    CurrentJumpTarget = "EnemyPlatform";
                //}
                else if (collision.transform.tag == "MinionJumpToLBottomPlat" && m_grounded && (target.name == "RedMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "LeftEdge") && !m_launched)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-2f, 1f), ForceMode2D.Impulse);
                    m_launched = true;
                    target = GondolaEdgeTarget.transform;
                    CurrentJumpTarget = "EnemyPlatform";
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "Gondola" && m_grounded && transform.position.y > collision.transform.position.y)
                {
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0f, .5f), ForceMode2D.Impulse);
                    target = GondolaEdgeTarget.transform;
                    if (EyeTarget != null)
                    {
                        CurrentJumpTarget = "EnemyPlatform";
                    }
                    else
                    {
                        CurrentJumpTarget = "EnemyKeep";
                    }
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "MinionJumpToEnemyPlat" && m_grounded && CurrentJumpTarget == "EnemyPlatform" && EyeTarget != null)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-7f, 7), ForceMode2D.Impulse);
                    m_launched = true;
                    target = EyeTarget.transform;
                    CurrentJumpTarget = "None";
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "MinionJumpToEnemyKeep" && m_grounded && KeepEyeTarget != null)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-3f, 4), ForceMode2D.Impulse);
                    target = KeepEyeTarget.transform;
                    CurrentJumpTarget = "None";
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "MinionJumpSmall" && m_grounded && CurrentJumpTarget == "None" && EyeTarget != null)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-.5f, 0), ForceMode2D.Impulse);
                    target = EyeTarget.transform;
                    CurrentJumpTarget = "None";
                    m_animator.SetTrigger("Jump");
                }
            }
            else if (teamColor == "Blue")
            {
                if (collision.transform.tag == "MinionBoardRight" && m_grounded && target.name != "BlueMinionPlatformTarget" && !target.name.Contains("EyeTarget"))
                {
                    //set velocity to zero to not carry momentum
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-5.5f, 18f), ForceMode2D.Impulse);
                    //SelectTarget();
                    target = GondolaEdgeTarget.transform;
                    CurrentJumpTarget = "EnemyPlatform";
                    m_grounded = false;
                    m_launched = true;
                    DisableWallSensors();
                    m_groundSensor.Disable(.2f);
                    m_animator.SetTrigger("Jump");
                }
                if (collision.transform.tag == "MinionBoardLeft" && m_grounded && target.name != "BlueMinionPlatformTarget" && !target.name.Contains("EyeTarget"))
                {
                    //set velocity to zero to not carry momentum
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(5.5f, 18f), ForceMode2D.Impulse);
                    //SelectTarget();
                    target = GondolaEdgeTarget.transform;
                    CurrentJumpTarget = "EnemyPlatform";
                    m_grounded = false;
                    m_launched = true;
                    DisableWallSensors();
                    m_groundSensor.Disable(.2f);
                    m_animator.SetTrigger("Jump");
                }
                if (collision.transform.tag == "MinionJumpToMainPlatform" && m_grounded && target.name != "BlueMinionGondolaTarget" && CurrentJumpTarget == "MainPlatform")
                {
                    //set velocity to zero to not carry momentum
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(6f, 6f), ForceMode2D.Impulse);
                    m_launched = true;
                    //SelectTarget();
                    target = GondolaTarget.transform;
                    CurrentJumpTarget = "Gondola";
                    m_animator.SetTrigger("Jump");
                }
                if (collision.transform.tag == "MinionJumpRight")
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(6.5f, 5), ForceMode2D.Impulse);
                    CurrentJumpTarget = "VerticalPlatform";
                    SelectTarget();
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "MinionJumpUp" && !m_idle && m_grounded && CurrentJumpTarget == "VerticalPlatform")
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
                    m_launched = true;
                    CurrentJumpTarget = "MainPlatform";
                    m_animator.SetTrigger("Jump");
                }
                //else if (collision.transform.tag == "MinionBoardGondola" && m_grounded && (target.name == "RedMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "LeftEdge") && !m_launched)
                //{
                //    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                //    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 17f), ForceMode2D.Impulse);
                //    m_launched = true;
                //    target = GondolaEdgeTarget.transform;
                //    CurrentJumpTarget = "EnemyPlatform";
                //}
                else if (collision.transform.tag == "MinionJumpToRBottomPlat" && m_grounded && (target.name == "BlueMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "RightEdge") && !m_launched)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-2f, 1f), ForceMode2D.Impulse);
                    m_launched = true;
                    target = GondolaEdgeTarget.transform;
                    CurrentJumpTarget = "EnemyPlatform";
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "Gondola" && m_grounded && transform.position.y > collision.transform.position.y)
                {
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0f, .5f), ForceMode2D.Impulse);
                    target = GondolaEdgeTarget.transform;
                    if (EyeTarget != null)
                    {
                        CurrentJumpTarget = "EnemyPlatform";
                    }
                    else
                    {
                        CurrentJumpTarget = "EnemyKeep";
                    }
                    m_animator.SetTrigger("Jump");

                }
                else if (collision.transform.tag == "MinionJumpToEnemyPlat" && m_grounded && CurrentJumpTarget == "EnemyPlatform" && EyeTarget != null)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(7f, 7), ForceMode2D.Impulse);
                    m_launched = true;
                    target = EyeTarget.transform;
                    CurrentJumpTarget = "None";
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "MinionJumpToEnemyKeep" && m_grounded && KeepEyeTarget != null)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(3f, 4), ForceMode2D.Impulse);
                    target = KeepEyeTarget.transform;
                    CurrentJumpTarget = "None";
                    m_animator.SetTrigger("Jump");
                }
                else if (collision.transform.tag == "MinionJumpSmall" && m_grounded && CurrentJumpTarget == "None" && EyeTarget != null)
                {
                    transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(.5f, 0), ForceMode2D.Impulse);
                    target = EyeTarget.transform;
                    CurrentJumpTarget = "None";
                    m_animator.SetTrigger("Jump");
                }
            }
        }
        
        else if (MenuEvents.gameModeSelect == 3 || MenuEvents.gameModeSelect == 4)
        {
            if (collision.transform.tag == "MinionJumpRight" && teamColor == "Blue" && m_grounded && EyeTarget != null)
            {
                transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(7f, 8), ForceMode2D.Impulse);
                m_launched = true;
                m_animator.SetTrigger("Jump");
                target = EyeTarget.transform;
                CurrentJumpTarget = "None";
            }
            else if (collision.transform.tag == "MinionJumpLeft" && teamColor == "Red" && m_grounded && EyeTarget != null)
            {
                transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
                transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-7f, 8), ForceMode2D.Impulse);
                m_launched = true;
                m_animator.SetTrigger("Jump");
                target = EyeTarget.transform;
                CurrentJumpTarget = "None";
            }
        }


    }

}
