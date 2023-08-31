using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class CPUBehavior : Conqueror
{
    private GrabableLedge ledge;
    private Rect buttonPos1;
    private Rect buttonPos2;
    public float attackDistance = 2f;
    public float minDamage = 0.0f;
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform target;
    public bool inRange; //check if player is in range
    public GameObject hotZone;
    public GameObject triggerArea;
    
    private AudioManager_PrototypeHero m_audioManager;

    public float m_TimeSinceJab2 = 0.0f;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
        m_audioManager = AudioManager_PrototypeHero.instance;
        m_gravity = m_body2d.gravityScale;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

        leftLimit = GameObject.Find("CPULimitL").transform;
        rightLimit = GameObject.Find("CPULimitR").transform;
        m_DamageDisplay = GameObject.Find("DmgDisplayP2").GetComponentInChildren<Text>();

        SelectTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrappled)
        {
            m_disableMovementTimer = 1.0f;
        }
        // Check for interactable overlapping objects
        CheckOverlaps();

        m_DamageDisplay.text = currentDamage + "%";

        if (!InsideofLimits() && !inRange && m_timeSinceAttack > 0.2f)
        {
            SelectTarget();
        }

        // Decrease death respawn timer 
        m_respawnTimer -= Time.deltaTime;

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;
        m_TimeSinceJab2 += Time.deltaTime;

        // Decrease timer that checks if we are in parry stance
        m_parryTimer -= Time.deltaTime;

        // Decrease timer that disables input movement. Used when attacking
        m_disableMovementTimer -= Time.deltaTime;

        // Respawn Hero if dead
        if (m_dead && m_respawnTimer < 0.0f)
            RespawnHero();

        if (m_dead)
            return;

        if (!m_inHitStun && !isGrappled)
        {
            if (!m_isInHitStop)
            {
                m_animator.speed = 1;
                if (m_animator.GetBool("isParrying"))
                {
                    if (shieldBar.shieldHealth > 0.0f)
                    {
                        shieldBar.shieldHealth -= .02f;
                        shieldBar.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    else
                    {
                        m_animator.SetBool("isParrying", false);
                        m_isParrying = false;
                        m_animator.SetTrigger("ShieldBreak");
                        m_animator.SetBool("isStunned", true);
                        m_timeSinceStun = 0.0f;
                        m_disableMovementTimer = 4.0f;
                        shieldBar.shieldHealth = 1.0f;
                        shieldBar.GetComponent<SpriteRenderer>().enabled = false;
                    }
                    if (m_facingDirection == 1)
                    {
                        shieldBar.transform.position = new Vector3(transform.position.x + 0.64f, transform.position.y + 0.15f, transform.position.z + 0.0f);
                        shieldBar.GetComponent<SpriteRenderer>().flipX = false;
                    }
                    else
                    {
                        shieldBar.transform.position = new Vector3(transform.position.x - 0.64f, transform.position.y + 0.15f, transform.position.z + 0.0f);
                        shieldBar.GetComponent<SpriteRenderer>().flipX = true;
                    }

                }
                else
                {
                    shieldBar.GetComponent<SpriteRenderer>().enabled = false;
                    if (shieldBar.shieldHealth < 56)
                    {
                        shieldBar.shieldHealth += .02f;
                    }
                    //shieldBarFrame.enabled = false;
                }
                if (m_timeSinceSideSpecial > .08f && hookActive == true && hookPrefab != null)
                {
                    hookActive = false;
                }
                if (hookPrefab != null)
                {
                    //Actions while hook is out
                    if (hookPrefab.latched == true)
                    {

                    }

                }
                if ((m_fSmashCharging || m_uSmashCharging || m_dSmashCharging) && m_timeSinceChargeStart <= m_maxSmashChargeTime)
                {
                    m_chargeModifier += .01f;
                }
                else if ((m_fSmashCharging) && m_timeSinceChargeStart > m_maxSmashChargeTime && m_fullCharge == false)
                {
                    Instantiate(ChargeFlashFX, new Vector3(transform.position.x + (.6f * -m_facingDirection), transform.position.y - .3f, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    m_fullCharge = true;
                }
                else if ((m_uSmashCharging) && m_timeSinceChargeStart > m_maxSmashChargeTime && m_fullCharge == false)
                {
                    Instantiate(ChargeFlashFX, new Vector3(transform.position.x + (.5f * m_facingDirection), transform.position.y - .2f, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    m_fullCharge = true;
                }
                else if ((m_dSmashCharging) && m_timeSinceChargeStart > m_maxSmashChargeTime && m_fullCharge == false)
                {
                    Instantiate(ChargeFlashFX, new Vector3(transform.position.x + (.65f * -m_facingDirection), transform.position.y - .23f, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    m_fullCharge = true;
                }
                //Update damage
                m_DamageDisplay.text = currentDamage + "%";
                // Check for interactable overlapping objects
                CheckOverlaps();
                // Decrease death respawn timer 
                m_respawnTimer -= Time.deltaTime;

                // Increase timer that controls attack combo
                m_timeSinceAttack += Time.deltaTime;
                m_timeSinceSideSpecial += Time.deltaTime;
                m_timeSinceStun += Time.deltaTime;
                m_timeSinceNSpec += Time.deltaTime;

                m_timeSinceChargeStart += Time.deltaTime;

                // Decrease timer that checks if we are in parry stance
                //m_parryTimer -= Time.deltaTime;

                // Decrease timer that disables input movement. Used when attacking
                m_disableMovementTimer -= Time.deltaTime;

                // Respawn Hero if dead
                if (m_dead && m_respawnTimer < 0.0f)
                    RespawnHero();

                if (m_dead)
                    return;

                //Check if character just landed on the ground
                if (!m_grounded && m_groundSensor.State())
                {
                    m_jumpCount = 0;
                    m_grounded = true;
                    m_animator.SetBool("Grounded", m_grounded);
                    m_launched = false;
                    //m_SpriteShaper.DrawDebug();
                }

                //Check if character just started falling
                if (m_grounded && !m_groundSensor.State())
                {
                    m_grounded = false;
                    m_animator.SetBool("Grounded", m_grounded);
                    m_fSmashCharging = false;
                    m_animator.SetBool("FSmashCharge", false);
                    m_uSmashCharging = false;
                    m_animator.SetBool("USmashCharge", false);
                    m_crouching = false;
                    m_dSmashCharging = false;
                    m_animator.SetBool("DSmashCharge", false);
                    m_animator.SetBool("Crouching", false);
                    m_animator.SetBool("isParrying", false);
                    m_parryTimer = -1.0f;
                    m_isParrying = false;
                }

                // -- Handle input and movement --
                float inputX = 0.0f;

                // Check if character is currently moving

                // SlowDownSpeed helps decelerate the characters when stopping
                float SlowDownSpeed = m_moving ? 1.0f : 0.5f;
                float KBSlowDownSpeed = 0.5f;
                // Set movement

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
                    // True if TWO right sensors are colliding and character is facing right
                    // OR if TWO left sensors are colliding and character is facing left
                    m_wallSlide = (m_wallSensorR1.State() && m_wallSensorR2.State() && m_facingDirection == 1) || (m_wallSensorR1.State() && m_wallSensorR0.State() && m_facingDirection == 1) || (m_wallSensorL1.State() && m_wallSensorL2.State() && m_facingDirection == -1) || (m_wallSensorL1.State() && m_wallSensorL0.State() && m_facingDirection == -1);
                    if (m_grounded)
                        m_wallSlide = false;
                    m_animator.SetBool("WallSlide", m_wallSlide);
                    //Play wall slide sound
                    if (prevWallSlide && !m_wallSlide)
                        AudioManager_PrototypeHero.instance.StopSound("WallSlide");


                    //Grab Ledge
                    // True if either bottom right sensor is colliding and top right sensor is not colliding 
                    // OR if bottom left sensor is colliding and top left sensor is not colliding 
                    if (m_ledgeGrab)
                    {
                        m_climbPosition = ledge.transform.position + new Vector3(ledge.topClimbPosition.x, ledge.topClimbPosition.y, 0);
                    }
                    bool shouldGrab = !m_ledgeClimb && !m_ledgeGrab && ((m_wallSensorR1.State() && !m_wallSensorR2.State()) || (m_wallSensorL1.State() && !m_wallSensorL2.State()) || (m_wallSensorR0.State() && !m_wallSensorR1.State()) || (m_wallSensorL0.State() && !m_wallSensorL1.State()));
                    if (shouldGrab)
                    {
                        Vector3 rayStart;
                        if (m_facingDirection == 1)
                            rayStart = m_wallSensorR2.transform.position + new Vector3(0.2f, 0.0f, 0.0f);
                        else
                            rayStart = m_wallSensorL2.transform.position - new Vector3(0.2f, 0.0f, 0.0f);

                        var hit = Physics2D.Raycast(rayStart, Vector2.down, 1.0f);

                        ledge = null;
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

                            if (hit.transform.tag == "MovingLedge")
                            {
                                transform.SetParent(hit.transform.parent);
                            }
                        }
                        m_animator.SetBool("LedgeGrab", m_ledgeGrab);
                    }

                }


                // -- Handle Animations --

                if (m_timeSinceStun > 4.0f)
                {
                    if (m_animator.GetBool("isStunned"))
                    {
                        m_animator.SetBool("isStunned", false);
                    }
                    if (Input.GetKeyUp("left shift"))
                    {
                        m_animator.SetBool("isParrying", false);
                        m_parryTimer = -1.0f;
                        m_isParrying = false;
                    }
                    //Self destruct
                    //if (Input.GetKeyDown("e") && !m_dodging)
                    //{
                    //    m_animator.SetBool("noBlood", m_noBlood);
                    //    m_animator.SetTrigger("Death");
                    //    m_respawnTimer = 2.5f;
                    //    DisableWallSensors();
                    //    m_dead = true;
                    //}

                    var distance = Vector2.Distance(transform.position, target.position);

                    //Attack

                    if (attackDistance >= distance && inRange && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.5f && m_TimeSinceJab2 > 0.9f)
                    {
                        if (m_wallSensorR2.State() || m_wallSensorL2.State())
                        {
                            m_animator.SetTrigger("UpAttack");

                            // Reset timer
                            m_timeSinceAttack = 0.0f;

                            //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

                            // Disable movement 
                            m_disableMovementTimer = 0.35f;
                        }
                        else
                        {
                            // Reset timer
                            m_timeSinceAttack = 0.0f;

                            m_currentAttack++;

                            if (m_currentAttack == 2)
                            {
                                m_TimeSinceJab2 = 0.0f;
                            }

                            // Loop back to one after second attack
                            if (m_currentAttack > 2)
                                m_currentAttack = 1;

                            // Reset Attack combo if time since last attack is too large
                            if (m_timeSinceAttack > .8f)
                                m_currentAttack = 1;

                            // Call one of the two attack animations "Attack1" or "Attack2"
                            m_animator.SetTrigger("Attack" + m_currentAttack);

                            m_disableMovementTimer = 0.45f;
                        }
                    }
                }

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

                // -- Handle input and movement --
                if (m_disableMovementTimer < 0.0f && m_grounded)
                {
                    Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_runSpeed * Time.deltaTime);

                    m_moving = true;
                }


                // Set Animation layer for hiding sword
                //int boolInt = m_hideSword ? 1 : 0;
                //m_animator.SetLayerWeight(1, boolInt);


                //Run
                if (m_moving)
                {
                    m_animator.SetInteger("AnimState", 1);
                    m_maxSpeed = m_runSpeed;
                }

                //Idle
                else
                {
                    m_animator.SetInteger("AnimState", 0);
                }
                
                //jump to recover
                if (transform.position.y < -2 && m_jumpCount < 2 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                {
                    
                    if (transform.position.x > rightLimit.position.x)
                    {
                        m_body2d.velocity = new Vector2(-1, m_jumpForce);
                        m_animator.SetTrigger("Jump");
                        m_grounded = false;
                        m_animator.SetBool("Grounded", m_grounded);
                        m_groundSensor.Disable(0.2f);
                        m_jumpCount++;
                    }
                    else if (transform.position.x < leftLimit.position.x)
                    {
                        m_body2d.velocity = new Vector2(-1, m_jumpForce);
                        m_animator.SetTrigger("Jump");
                        m_grounded = false;
                        m_animator.SetBool("Grounded", m_grounded);
                        m_groundSensor.Disable(0.2f);
                        m_jumpCount++;
                    }

                    
                }

                if (transform.position.x > rightLimit.position.x && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && m_jumpCount >= 1)
                {
                    ThrowHook();
                    m_animator.SetTrigger("Throw");
                    hookActive = true;
                    m_timeSinceSideSpecial = 0.0f;

                    m_disableMovementTimer = 1.0f;
                }
                else if (transform.position.x < leftLimit.position.x && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f
                                && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && m_jumpCount >= 1)
                {
                    ThrowHook();
                    m_animator.SetTrigger("Throw");
                    hookActive = true;
                    m_timeSinceSideSpecial = 0.0f;

                    m_disableMovementTimer = 1.0f;
                }
            }
            else
            {
                m_timeSinceHitStop += Time.deltaTime;
                m_animator.speed = 0;
                if (m_timeSinceHitStop >= m_hitStopDuration)
                {
                    m_isInHitStop = false;
                    m_animator.speed = 1;
                }
            }
        }
        else
        {
            if (m_inHitStun)
            {
                m_body2d.gravityScale = 0;
                m_timeSinceHitStun += Time.deltaTime;
                m_animator.speed = 0;
                if (m_timeSinceHitStun >= m_hitStunDuration)
                {
                    m_body2d.gravityScale = 1.25f;
                    Knockback(incomingKnockback, incomingAngle, incomingXMod, incomingYMod);
                    m_isInKnockback = true;
                }
            }
            else if (isGrappled)
            {
                m_animator.speed = 0;
            }
        }
    }
    //
    //Handle collisions w/o sensor
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("PortalForeground"))
        {
            if (Input.GetButtonDown("Submit"))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 15, 20);
                transform.tag = "PlayerMid";
                gameObject.layer = LayerMask.NameToLayer("PlayerMid");
                var currRenderer = gameObject.GetComponent<SpriteRenderer>();
            }

        }
        if (coll.gameObject.CompareTag("BlastZone") || coll.gameObject.CompareTag("BlastZoneMid"))
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
        if (coll.gameObject.CompareTag("LeftLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            e_Rigidbody2D.AddForce(new Vector2(-42, 8), ForceMode2D.Impulse);
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
                if (CrossPlatformInputManager.GetButtonDown("Submit"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 15, 20);
                    transform.tag = "PlayerMid";
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                    //m_audioManager.PlaySound("Teleport");
                }
            }
            //Check for portal
            else if (coll.gameObject.CompareTag("PortalMidground"))
            {
                atPortal = true;
                if (CrossPlatformInputManager.GetButtonDown("Submit"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 10, 0);
                    transform.tag = "Player";
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                    //m_audioManager.PlaySound("Teleport");
                }
            }
        }
        if (atPortal == false)
        {
        }
    }

    public void ThrowHook()
    {
        Quaternion rotQuat = new Quaternion();
        if (hookDirection == "Up")
        {
            rotQuat = new Quaternion(180f, 0f, 0f, 0f);
        }
        else if (m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }

        HookBehavior hookShot = Instantiate(hookPrefab, launchOffset.position, rotQuat);

        if (transform.CompareTag("PlayerMid"))
        {
            hookShot.gameObject.layer = 10;
        }

        var pointArr = new Transform[2];
        pointArr[0] = transform;
        pointArr[1] = hookShot.transform;

        hookShot.transform.parent = transform;
        hookChain.SetUpLine(pointArr);
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }


    public void Move()
    {
        if (!m_inHitStun)
        {
            
                

        }
        else
        {
            m_timeSinceHitStun += Time.deltaTime;
            if (m_timeSinceHitStun >= m_hitStunDuration)
            {
                Knockback(incomingKnockback, incomingAngle, incomingXMod, incomingYMod);
            }
        }
    }


    void Attack(string attackType, Transform attackPoint)
    {
        float damageModifier = 0.0f;

        var playerCollider = GetComponent<BoxCollider2D>();

        Vector2 attackHitboxCenter = attackPoint.position;

        //Detect enemy collision with attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackHitboxCenter, jabRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponentInParent<PrototypeHero>() != null && enemy.tag == "Player")
            {
                //Detect impact angle
                var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);

                var sourceclosestPoint = new Vector2(playerCollider.transform.position.x, playerCollider.transform.position.y);

                var positionDifference = targetclosestPoint - sourceclosestPoint;

                //Must be done to detect y axis angle
                float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

                // Convert the angle to degrees.
                float attackAngle = angleInRadians * Mathf.Rad2Deg;

                //Block check
                if (enemy.GetComponentInParent<PrototypeHero>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<PrototypeHero>().m_facingDirection == 1 && attackAngle > 90 && attackAngle < 225)
                {
                    enemy.GetComponentInParent<PrototypeHero>().Block(jabDamage);
                    Instantiate(BlockFX, new Vector2((targetclosestPoint.x), targetclosestPoint.y),
            new Quaternion(0f, 0f, 0f, 0f), playerCollider.transform);
                    m_audioManager.PlaySound("Parry");
                }
                else if (enemy.GetComponentInParent<PrototypeHero>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<PrototypeHero>().m_facingDirection == -1 && (attackAngle < 90 || attackAngle > 315))
                {
                    enemy.GetComponentInParent<PrototypeHero>().Block(jabDamage);
                    Instantiate(BlockFX, new Vector2((targetclosestPoint.x), targetclosestPoint.y),
            new Quaternion(0f, 0f, 0f, 0f), playerCollider.transform);
                    m_audioManager.PlaySound("Parry");
                }
                else
                {
                    //Apply damage
                    enemy.GetComponentInParent<PrototypeHero>().TakeDamage(jabDamage);
                    //Apply Knockback
                    enemy.GetComponentInParent<PrototypeHero>().incomingAngle = attackAngle;
                    enemy.GetComponentInParent<PrototypeHero>().incomingKnockback = jabKB;
                    enemy.GetComponentInParent<PrototypeHero>().incomingXMod = 0f;
                    enemy.GetComponentInParent<PrototypeHero>().incomingYMod = 2f;
                    enemy.GetComponentInParent<PrototypeHero>().HitStun(.15f);
                    //enemy.GetComponentInParent<PrototypeHero>().Knockback(jabKB, attackAngle, 0, 2);
                }
                

            }
            if (enemy.GetComponentInParent<TheChampion>() != null && enemy.tag == "Player")
            {
                //Detect impact angle
                var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);

                var sourceclosestPoint = new Vector2(playerCollider.transform.position.x, playerCollider.transform.position.y);

                var positionDifference = targetclosestPoint - sourceclosestPoint;

                //Must be done to detect y axis angle
                float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

                // Convert the angle to degrees.
                float attackAngle = angleInRadians * Mathf.Rad2Deg;

                //Block check
                if (enemy.GetComponentInParent<TheChampion>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<TheChampion>().m_facingDirection == 1 && attackAngle > 90 && attackAngle < 225)
                {
                    enemy.GetComponentInParent<TheChampion>().Block(jabDamage);
                }
                else if (enemy.GetComponentInParent<TheChampion>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<TheChampion>().m_facingDirection == -1 && (attackAngle < 90 || attackAngle > 315))
                {
                    enemy.GetComponentInParent<TheChampion>().Block(jabDamage);
                }
                else
                {
                    //Apply damage
                    enemy.GetComponentInParent<TheChampion>().TakeDamage(jabDamage);
                    //Apply Knockback
                    enemy.GetComponentInParent<TheChampion>().incomingAngle = attackAngle;
                    enemy.GetComponentInParent<TheChampion>().incomingKnockback = jabKB;
                    enemy.GetComponentInParent<TheChampion>().incomingXMod = 0f;
                    enemy.GetComponentInParent<TheChampion>().incomingYMod = 2f;
                    enemy.GetComponentInParent<TheChampion>().HitStun(.15f);
                    //enemy.GetComponentInParent<PrototypeHero>().Knockback(jabKB, attackAngle, 0, 2);
                }


            }
        }
    }


    // Function used to spawn a dust effect
    // All dust effects spawns on the floor
    // dustXoffset controls how far from the player the effects spawns.
    // Default dustXoffset is zero
    

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

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
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

}
