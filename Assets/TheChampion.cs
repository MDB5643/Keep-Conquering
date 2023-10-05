using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class TheChampion : Conqueror
{

    private GrabableLedge ledge;
    public bool isInUpSpecial;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
        m_gravity = m_body2d.gravityScale;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorR0 = transform.Find("WallSensor_R0").GetComponent<Sensor_Prototype>();
        m_wallSensorL0 = transform.Find("WallSensor_L0").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_inHitStun)
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

                //Smash charge
                if ((m_fSmashCharging || m_uSmashCharging || m_dSmashCharging) && m_timeSinceChargeStart <= m_maxSmashChargeTime)
                {
                    m_chargeModifier += .01f;
                }
                else if ((m_fSmashCharging) && m_timeSinceChargeStart > m_maxSmashChargeTime && m_fullCharge == false)
                {
                    Instantiate(ChargeFlashFX, new Vector3(transform.position.x + (1.3f * -m_facingDirection), transform.position.y - .1f, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    m_fullCharge = true;
                }
                else if ((m_uSmashCharging) && m_timeSinceChargeStart > m_maxSmashChargeTime && m_fullCharge == false)
                {
                    Instantiate(ChargeFlashFX, new Vector3(transform.position.x + (.5f * m_facingDirection), transform.position.y + .45f, transform.position.z),
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
                if (m_DamageDisplay)
                {
                    m_DamageDisplay.text = currentDamage + "%";
                }
                
                // Check for interactable overlapping objects
                CheckOverlaps();
                // Decrease death respawn timer 
                m_respawnTimer -= Time.deltaTime;

                // Increase timer that controls attack combo
                m_timeSinceAttack += Time.deltaTime;
                m_timeSinceSideSpecial += Time.deltaTime;
                m_timeSinceStun += Time.deltaTime;

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
                    if (!isInUpSpecial)
                    {
                        m_animator.SetTrigger("Land");
                    }
                    else
                    {
                        m_animator.SetTrigger("UpSpecLand");
                        isInUpSpecial = false;
                    }
                    m_launched = false;
                    m_isInKnockback = false;
                    m_SR.color = Color.white;
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

                if (m_disableMovementTimer < 0.0f && !preview)
                    inputX = Input.GetAxis("Horizontal");

                // GetAxisRaw returns either -1, 0 or 1
                float inputRaw = Input.GetAxisRaw("Horizontal");

                // Check if character is currently moving
                if (Mathf.Abs(inputRaw) > Mathf.Epsilon && Mathf.Sign(inputRaw) == m_facingDirection && !preview)
                    m_moving = true;
                else
                    m_moving = false;

                // Swap direction of sprite depending on move direction
                if (inputRaw > 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb && m_disableMovementTimer < 0 && !preview)
                {
                    if (m_SR.flipX == true)
                    {
                        launchOffset.position = new Vector3(transform.position.x + .6f, transform.position.y + .13f, transform.position.z + 0.0f);
                    }
                    m_SR.flipX = false;
                    m_facingDirection = 1;
                }

                else if (inputRaw < 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb && m_disableMovementTimer < 0 && !preview)
                {
                    if (m_SR.flipX == false)
                    {
                        launchOffset.position = new Vector3(transform.position.x - .6f, transform.position.y + .13f, transform.position.z + 0.0f);
                    }
                    m_SR.flipX = true;
                    m_facingDirection = -1;
                }

                // SlowDownSpeed helps decelerate the characters when stopping
                float SlowDownSpeed = m_moving ? 1.0f : 0.5f;
                float KBSlowDownSpeed = 0.8f;
                // Set movement
                if (!m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_animator.GetBool("isParrying") && m_disableMovementTimer < 0.0f && !m_launched && m_fSmashCharging == false
                    && m_uSmashCharging == false && m_dSmashCharging == false && !preview)
                {
                    if (!m_isInKnockback)
                    {
                        m_timeSinceKnockBack = 0.0f;
                        if (m_KnockBackMomentumX <= 1 && m_KnockBackMomentumY <= 1)
                        {
                            m_body2d.velocity = new Vector2(inputX * m_maxSpeed * SlowDownSpeed, m_body2d.velocity.y);
                            
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
                        //
                        //    m_body2d.velocity = new Vector2(m_body2d.velocity.x * KBSlowDownSpeed, 0);
                            m_isInKnockback = false;
                            m_KnockBackMomentumX = m_body2d.velocity.x;
                            m_KnockBackMomentumY = m_body2d.velocity.y;
                        //
                        }
                        //else
                        //{
                        //  m_body2d.velocity = new Vector2(m_body2d.velocity.x * KBSlowDownSpeed, m_body2d.velocity.y * KBSlowDownSpeed);
                        //}

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
                    // True if TWO right sensors are colliding and character is facing right
                    // OR if TWO left sensors are colliding and character is facing left
                    m_wallSlide = (m_wallSensorR1.State() && m_wallSensorR2.State() && m_facingDirection == 1) || (m_wallSensorR1.State() && m_wallSensorR0.State() && m_facingDirection == 1) || (m_wallSensorL1.State() && m_wallSensorL2.State() && m_facingDirection == -1) || (m_wallSensorL1.State() && m_wallSensorL0.State() && m_facingDirection == -1);
                    if (m_grounded)
                        m_wallSlide = false;
                    //m_animator.SetBool("WallSlide", m_wallSlide);
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
                            m_animator.SetTrigger("LedgeGrab");
                        }
                        
                    }

                }


                // -- Handle Animations --

                if (m_timeSinceStun > 4.0f && !preview)
                {
                    if (m_animator.GetBool("isStunned"))
                    {
                        m_animator.SetBool("isStunned", false);
                    }
                    if (Input.GetKeyUp("left shift"))
                    {
                        m_animator.SetBool("isParrying", false);
                        m_parryTimer = -1.0f;
                    }

                    if (m_animator.GetBool("SideSpecialHold"))
                    {
                        if (Input.GetMouseButtonUp(1))
                        {
                            m_animator.SetBool("SideSpecialHold", false);
                            m_animator.SetTrigger("SideSpecial");
                        }

                        m_timeSinceSideSpecial = 0.0f;
                        //
                        //SideSpecial();
                        //// Disable movement 
                        m_disableMovementTimer = 1.0f;
                    }
                    if (m_animator.GetBool("DSpec"))
                    {
                        if (Input.GetMouseButtonUp(1))
                        {
                            m_animator.SetBool("DSpec", false);
                        }

                        m_timeSinceSideSpecial = 0.0f;
                        //
                        //SideSpecial();
                        //// Disable movement 
                        m_disableMovementTimer = 1.0f;
                    }

                    // Parry & parry stance
                    else if (Input.GetKeyDown("left shift") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && !isInStartUp && !isInUpSpecial)
                    {
                        // Parry
                        // Used when you are in parry stance and something hits you
                        //if (m_parryTimer > 0.0f)
                        //{
                        //    m_animator.SetTrigger("Parry");
                        //    //m_body2d.velocity = new Vector2(-m_facingDirection * m_parryKnockbackForce, m_body2d.velocity.y);
                        //}

                        // Parry Stance
                        // Ready to parry in case something hits you
                        if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("ParryStance"))
                        {
                            m_animator.SetTrigger("ParryStance");
                            m_animator.SetBool("isParrying", true);
                            m_parryTimer = 7.0f / 12.0f;
                            m_isParrying = true;
                        }

                    }

                    //Up Smash Attack
                    else if (Input.GetMouseButton(0) && ((Input.GetKey("w") && Input.GetKey("left alt")) || Input.GetKey("2")) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded
                        && m_timeSinceAttack > 0.2f && m_fSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !isInStartUp && !isInUpSpecial)
                    {

                        if (m_uSmashCharging != true)
                        {
                            m_timeSinceChargeStart = 0.0f;
                            m_uSmashCharging = true;
                        }

                        m_uSmashCharging = true;
                        m_animator.SetBool("USmashCharge", true);

                        // Disable movement 
                        m_disableMovementTimer = 0.35f;
                    }

                    else if (Input.GetMouseButtonUp(0) && m_uSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.2f)
                    {
                        m_animator.SetTrigger("USmash");
                        m_animator.SetBool("USmashCharge", false);
                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        m_uSmashCharging = false;
                        m_fullCharge = false;

                        m_disableMovementTimer = 0.35f;
                    }

                    //Down Smash Attack
                    else if (Input.GetMouseButton(0) && ((Input.GetKey("s") && Input.GetKey("left alt")) || Input.GetKey("x")) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded
                        && m_timeSinceAttack > 0.55f && m_fSmashCharging == false && m_uSmashCharging == false && m_isParrying == false && !isInStartUp && !isInUpSpecial)
                    {

                        if (m_dSmashCharging != true)
                        {
                            m_timeSinceChargeStart = 0.0f;
                            m_dSmashCharging = true;
                        }

                        m_dSmashCharging = true;
                        m_animator.SetBool("DSmashCharge", true);

                        // Disable movement 
                        m_disableMovementTimer = 0.35f;
                    }

                    else if (Input.GetMouseButtonUp(0) && m_dSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.2f
                        && m_fSmashCharging == false && m_uSmashCharging == false)
                    {
                        m_animator.SetTrigger("DSmash");
                        m_animator.SetBool("DSmashCharge", false);
                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        m_dSmashCharging = false;
                        m_fullCharge = false;

                        m_disableMovementTimer = 0.35f;
                    }

                    //Forward Smash Attack
                    else if (Input.GetMouseButton(0) && (Input.GetKey("left alt") || Input.GetKey("f")) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded
                        && m_timeSinceAttack > 0.2f && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !isInStartUp && !isInUpSpecial)
                    {
                        if (m_fSmashCharging != true)
                        {
                            m_timeSinceChargeStart = 0.0f;
                            m_fSmashCharging = true;
                        }

                        m_animator.SetBool("FSmashCharge", true);


                        // Disable movement 
                        m_disableMovementTimer = 0.35f;
                    }

                    else if (Input.GetMouseButtonUp(0) && m_fSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.2f
                        && m_uSmashCharging == false && m_dSmashCharging == false)
                    {
                        m_animator.SetTrigger("FSmash");
                        m_animator.SetBool("FSmashCharge", false);
                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        m_fSmashCharging = false;
                        m_timeSinceChargeStart = 0.0f;
                        m_fullCharge = false;

                        // Call one of the two attack animations "Attack1" or "Attack2"

                        m_disableMovementTimer = 0.35f;
                    }

                    //Up Attack
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("w") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.4f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !isInStartUp && !isInUpSpecial)
                    {
                        m_animator.SetTrigger("UpAttack");

                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

                        // Disable movement 
                        m_disableMovementTimer = 0.45f;
                    }

                    //Attack
                    else if (Input.GetMouseButtonDown(0) && !Input.GetKey("w") && !Input.GetKey("s") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.32f && !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && !isInStartUp && !isInUpSpecial)
                    {
                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        // Call one of the two attack animations "Attack1" or "Attack2"
                        m_animator.SetTrigger("Attack2");

                        // Disable movement 
                        m_disableMovementTimer = 0.32f;
                    }

                    //down air Attack
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("s") && !m_ledgeGrab && !m_ledgeClimb && !m_grounded && m_timeSinceAttack > 0.2f && !isInStartUp && !isInUpSpecial)
                    {
                        m_animator.SetTrigger("Dair");
                        //m_body2d.velocity = new Vector2(0.0f, -m_jumpForce);
                        //m_disableMovementTimer = 0.8f;
                        //
                        //// Reset timer
                        m_timeSinceAttack = 0.0f;
                    }

                    // Air Attack Up
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("w") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.2f && !isInStartUp && !isInUpSpecial)
                    {
                        //Debug.Log("Air attack up");
                        if (m_facingDirection == 1)
                        {
                            m_animator.SetTrigger("UAir");
                        }
                        else
                        {
                            m_animator.SetTrigger("UAirBack");
                        }
                        //
                        //// Reset timer
                        m_timeSinceAttack = 0.0f;
                    }

                    // Air Attack Forward
                    else if (Input.GetMouseButtonDown(0) && (Input.GetKey("d") || Input.GetKey("a")) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.8f && !isInStartUp && !isInUpSpecial)
                    {
                        //Debug.Log("Air attack up");
                        if (m_facingDirection == 1)
                        {
                            m_animator.SetTrigger("Fair");
                        }
                        else
                        {
                            m_animator.SetTrigger("FairBack");
                        }
                        //
                        //// Reset timer
                        m_timeSinceAttack = 0.0f;
                        m_disableMovementTimer = .8f;
                    }
                    // Air Attack Neutral
                    else if (Input.GetMouseButtonDown(0) && !(Input.GetKey("d") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("w")) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.65f && !isInStartUp && !isInUpSpecial)
                    {

                        m_animator.SetTrigger("Nair");
                        //// Reset timer
                        m_timeSinceAttack = 0.0f;
                        m_disableMovementTimer = .65f;
                    }

                    //Down Tilt Attack
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("s") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_grounded && m_timeSinceAttack > 0.3f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !isInStartUp && !isInUpSpecial)
                    {
                        m_animator.SetTrigger("DTilt");

                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

                        // Disable movement 
                        m_disableMovementTimer = 0.45f;
                        m_animator.SetBool("Crouching", false);
                    }

                    // Dodge
                    //else if (Input.GetKeyDown("left shift") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb)
                    //{
                    //    m_dodging = true;
                    //    m_crouching = false;
                    //    m_animator.SetBool("Crouching", false);
                    //    m_animator.SetTrigger("Dodge");
                    //    m_body2d.velocity = new Vector2(m_facingDirection * m_dodgeForce, m_body2d.velocity.y);
                    //}

                    else if (Input.GetMouseButtonDown(1) && m_animator.GetBool("isParrying"))
                    {
                        m_dodging = true;
                        m_crouching = false;
                        m_animator.SetBool("isParrying", false);
                        m_animator.SetBool("Crouching", false);
                        m_animator.SetTrigger("Dodge");
                        m_body2d.velocity = new Vector2(-m_facingDirection * m_dodgeForce, m_body2d.velocity.y);
                    }

                    else if ((Input.GetKey("d") || Input.GetKey("a")) && Input.GetMouseButton(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f && !m_animator.GetBool("isParrying") && !isInStartUp && !isInUpSpecial)
                    {
                        m_animator.SetBool("SideSpecialHold", true);
                        //hookActive = true;
                        m_timeSinceSideSpecial = 0.0f;
                        //
                        //SideSpecial();
                        //// Disable movement 
                        m_disableMovementTimer = 1.0f;
                    }

                    else if ((Input.GetKey("s")) && Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f && !m_animator.GetBool("isParrying") && !isInStartUp && !isInUpSpecial)
                    {
                        m_animator.SetTrigger("DSpecTrigger");
                        m_animator.SetBool("DSpec", true);
                        //m_dodging = true;
                        //m_crouching = false;
                        //m_animator.SetBool("Crouching", false);
                        //m_body2d.velocity = new Vector2(m_facingDirection * 10f, m_body2d.velocity.y);
                    }

                    else if ((Input.GetKey("w")) && Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_launched && m_timeSinceAttack > 0.3f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !m_launched && !isInStartUp && !isInUpSpecial)
                    {
                        hookDirection = "Up";
                        m_animator.SetTrigger("UpSpecial");
                        m_SR.color = Color.red;
                        m_launched = true;
                        m_disableMovementTimer = 1.0f;

                        isInStartUp = true;
                    }

                    else if (Input.GetMouseButtonDown(1) && m_animator.GetBool("isParrying"))
                    {
                        //m_dodging = true;
                        //m_crouching = false;
                        //m_animator.SetBool("isParrying", false);
                        //m_animator.SetBool("Crouching", false);
                        //m_animator.SetTrigger("Dodge");
                        //m_body2d.velocity = new Vector2(-m_facingDirection * m_dodgeForce, m_body2d.velocity.y);
                    }

                    else if (Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.5f 
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !isInStartUp && !isInUpSpecial)
                    {
                        // Reset timer

                        // Call one of the two attack animations "Attack1" or "Attack2"
                        m_animator.SetTrigger("NSpec");
                        m_timeSinceNSpec = 0.0f;
                        m_timeSinceAttack = 0.0f;

                        // Disable movement 
                        m_disableMovementTimer = 0.5f;
                    }

                    // Ledge Climb
                    else if (Input.GetKeyDown("w") && m_ledgeGrab)
                    {
                        DisableWallSensors();
                        m_ledgeClimb = true;
                        m_body2d.gravityScale = 0;
                        m_disableMovementTimer = 6.0f / 14.0f;
                        m_animator.SetTrigger("LedgeClimb");
                    }

                    // Ledge Drop
                    else if (Input.GetKeyDown("s") && m_ledgeGrab)
                    {
                        DisableWallSensors();
                    }

                    //Jump
                    else if (Input.GetButtonDown("Jump") && (m_grounded || m_wallSlide || m_jumpCount <= 1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_disableMovementTimer < 0.0f && !m_launched && !isInStartUp && !isInUpSpecial)
                    {
                        m_jumpCount++;
                        // Check if it's a normal jump or a wall jump
                        if (!m_wallSlide)
                            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                        else
                        {
                            m_body2d.velocity = new Vector2(-m_facingDirection * m_jumpForce / 2.0f, m_jumpForce);
                            m_facingDirection = -m_facingDirection;
                            m_SR.flipX = !m_SR.flipX;
                        }

                        m_animator.SetTrigger("Jump");
                        m_grounded = false;
                        m_animator.SetBool("Grounded", m_grounded);
                        m_groundSensor.Disable(0.2f);
                    }

                    //Crouch / Stand up
                    else if (Input.GetKeyDown("s") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_animator.GetBool("isParrying") && !isInStartUp && !isInUpSpecial)
                    {
                        //m_crouching = true;
                        //m_animator.SetBool("Crouching", true);
                        //m_body2d.velocity = new Vector2(m_body2d.velocity.x / 2.0f, m_body2d.velocity.y);
                    }
                    else if (Input.GetKeyUp("s") && m_crouching)
                    {
                        //m_crouching = false;
                        //m_animator.SetBool("Crouching", false);
                    }
                    //Walk
                    else if (m_moving && Input.GetKey(KeyCode.LeftControl) && !isInStartUp && !isInUpSpecial)
                    {
                        m_animator.SetInteger("AnimState", 2);
                        m_maxSpeed = m_walkSpeed;
                    }

                    //Run
                    else if (m_moving && m_disableMovementTimer < 0.0f && !isInStartUp && !isInUpSpecial)
                    {
                        m_animator.SetInteger("AnimState", 1);
                        m_maxSpeed = m_runSpeed;
                    }

                    //Idle
                    else
                        m_animator.SetInteger("AnimState", 0);
                }
            }
            else
            {
                m_body2d.velocity = Vector2.zero;
                m_body2d.gravityScale = 0;
                m_timeSinceHitStop += Time.deltaTime;
                m_animator.speed = 0;
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
            if (m_inHitStun)
            {
                m_body2d.velocity = Vector2.zero;
                m_body2d.gravityScale = 0;
                
                m_timeSinceHitStun += Time.deltaTime;
                m_animator.speed = 0;
                if (m_timeSinceHitStun >= m_hitStunDuration)
                {
                    m_body2d.gravityScale = 1.25f;
                    isInUpSpecial = false;
                    m_animator.SetBool("SideSpecialHold", false);
                    isInStartUp = false;
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

    //Handle collisions w/o sensor
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("PortalForeground"))
        {
            infoText.text = "Press Tab to Teleport";
            if (Input.GetButtonDown("Submit"))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 15, 20);
                transform.tag = "PlayerMid";
                gameObject.layer = LayerMask.NameToLayer("PlayerMid");
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
            }

        }
        if (coll.gameObject.CompareTag("BlastZone") || coll.gameObject.CompareTag("BlastZoneMid"))
        {
            currentDamage = 0.0f;
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            Quaternion rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            GameObject.Instantiate(KnockoutFX, new Vector2(transform.position.x, transform.position.y + 7f), rotQuat, null);
            m_respawnTimer = 2.5f;
            DisableWallSensors();
            m_dead = true;
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
                infoText.text = "Press Tab";
                if (CrossPlatformInputManager.GetButtonDown("Submit") )
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 15, 20);
                    transform.tag = "PlayerMid";
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                }
            }
            //Check for portal
            else if (coll.gameObject.CompareTag("PortalMidground"))
            {
                atPortal = true;
                infoText.text = "Press Tab";
                if (CrossPlatformInputManager.GetButtonDown("Submit"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 10, 0);
                    transform.tag = "Player";
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                    //m_audioManager.PlaySound("Teleport");
                }
            }
        }
        if (atPortal == false && infoText)
        {
            infoText.text = "";
        }
    }

    public void Attack(float baseDamage, float baseKnockBack, float baseRange, float KBModX, float KBmodY, Transform attackPoint)
    {
        float damageModifier = 0.0f;
        baseDamage = jabDamage;

        var playerCollider = GetComponent<BoxCollider2D>();

        Vector2 jab1Hitbox = new Vector2(attackPoint.position.x - .3f, attackPoint.position.y + 1.0f);
        Vector2 attackHitboxCenter = attackPoint.position;
        if (m_currentAttack == 1)
        {
            attackHitboxCenter = jab1Hitbox;
        }

        //Detect enemy collision with attack
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackHitboxCenter, new Vector2(1.5f, .3f), 0.0f, enemyLayers) ;
        if (m_currentAttack == 1)
        {
            hitEnemies = Physics2D.OverlapBoxAll(attackHitboxCenter, new Vector2(1.5f, 1.2f), 0.0f, enemyLayers);
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponentInParent<CPUBehavior>() != null && enemy.tag.StartsWith("Player"))
            {
                var above = false;

                //Detect impact angle
                var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                var sourceclosestPoint = new Vector2(playerCollider.transform.position.x, playerCollider.transform.position.y);
                if (sourceclosestPoint.y > targetclosestPoint.y)
                {
                    above = true;
                }

                var positionDifference = targetclosestPoint - sourceclosestPoint;

                //Must be done to detect y axis angle
                float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

                // Convert the angle to degrees.
                float attackAngle = angleInRadians * Mathf.Rad2Deg;

                //Apply damage
                enemy.GetComponentInParent<CPUBehavior>().TakeDamage(baseDamage);
                //Apply Knockback
                enemy.GetComponentInParent<CPUBehavior>().incomingAngle = attackAngle;
                enemy.GetComponentInParent<CPUBehavior>().incomingKnockback = baseKnockBack;
                enemy.GetComponentInParent<CPUBehavior>().incomingXMod = 0f;
                enemy.GetComponentInParent<CPUBehavior>().incomingYMod = 2f;
                enemy.GetComponentInParent<CPUBehavior>().HitStun(.15f);
                //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

            }
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
            infoText.text = "";
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


    public void TakeDamage(float damage)
    {
        currentDamage += damage;

        //Play hurt animation
    }



    public void SideSpecial()
    {
        //Quaternion rotQuat = new Quaternion();
        //if (m_facingDirection == 1)
        //{
        //    rotQuat = new Quaternion(0f, 180f, 0f, 0f);
        //}
        //else
        //{
        //    rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        //}
        //
        //HookBehavior hookShot = Instantiate(hookPrefab, launchOffset.position, rotQuat);
        //var pointArr = new Transform[2];
        //pointArr[0] = transform;
        //pointArr[1] = hookShot.transform;
        //
        //hookShot.transform.parent = transform;
        //hookChain.SetUpLine(pointArr);
    }
}
