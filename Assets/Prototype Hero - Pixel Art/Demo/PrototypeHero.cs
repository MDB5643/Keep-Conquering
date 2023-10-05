using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrototypeHero : Conqueror {

    
    private GrabableLedge ledge;
    private Rect buttonPos1;
    private Rect buttonPos2;

    public float m_TimeSinceJab2 = 0.0f;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();


        m_gravity = m_body2d.gravityScale;

        buttonPos1 = new Rect(10.0f, 10.0f, 200.0f, 30.0f);
        buttonPos2 = new Rect(10.0f, 50.0f, 200.0f, 30.0f);

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorR0 = transform.Find("WallSensor_R0").GetComponent<Sensor_Prototype>();
        m_wallSensorL0 = transform.Find("WallSensor_L0").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();
    }

   

    // Update is called once per frame
    void Update ()
    {
        

        if (!m_inHitStun )
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
                m_timeSinceNSpec += Time.deltaTime;
                m_TimeSinceJab2 += Time.deltaTime;
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

                if (m_disableMovementTimer < 0.0f && !preview)
                    inputX = Input.GetAxis("Horizontal");

                // GetAxisRaw returns either -1, 0 or 1
                float inputRaw = Input.GetAxisRaw("Horizontal");

                // Check if character is currently moving
                if (Mathf.Abs(inputRaw) > Mathf.Epsilon && Mathf.Sign(inputRaw) == m_facingDirection)
                    m_moving = true;
                else
                    m_moving = false;

                // Swap direction of sprite depending on move direction
                if (inputRaw > 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb && !preview)
                {
                    if (m_SR.flipX == true)
                    {
                        launchOffset.position = new Vector3(transform.position.x + .6f, transform.position.y + .13f, transform.position.z + 0.0f);
                    }
                    m_SR.flipX = false;
                    m_facingDirection = 1;
                }

                else if (inputRaw < 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb && !preview)
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
                        //    m_body2d.velocity = new Vector2(m_body2d.velocity.x * KBSlowDownSpeed, m_body2d.velocity.y * KBSlowDownSpeed);
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

                    // Parry & parry stance
                    else if (Input.GetKeyDown("left shift") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false)
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
                        && m_timeSinceAttack > 0.2f && m_fSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
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

                    else if (Input.GetMouseButtonUp(0) && m_uSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.2f
                        && m_timeSinceNSpec > 1.1f)
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
                        && m_timeSinceAttack > 0.2f && m_fSmashCharging == false && m_uSmashCharging == false && m_isParrying == false)
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
                        && m_timeSinceNSpec > 1.1f)
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
                        && m_timeSinceAttack > 0.2f && m_timeSinceNSpec > 1.1f && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
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

                    else if (Input.GetMouseButtonUp(0) && m_fSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.2f)
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
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("w") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.2f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                    {
                        m_animator.SetTrigger("UpAttack");

                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

                        // Disable movement 
                        m_disableMovementTimer = 0.35f;
                    }

                    //Jab Attack
                    else if (Input.GetMouseButtonDown(0) && !Input.GetKey("w") && !Input.GetKey("s") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.15f && m_TimeSinceJab2 > 0.45f && m_timeSinceNSpec > 1.1f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
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

                    //Air Slam Attack
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("s") && !m_ledgeGrab && !m_ledgeClimb && !m_grounded)
                    {
                        m_animator.SetTrigger("AttackAirSlam");
                        m_body2d.velocity = new Vector2(0.0f, -m_jumpForce);
                        m_disableMovementTimer = 0.8f;

                        // Reset timer
                        m_timeSinceAttack = 0.0f;
                    }

                    //Down Tilt Attack
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("s") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_grounded && m_timeSinceAttack > 0.3f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                    {
                        m_animator.SetTrigger("DTiltAttack");

                        // Reset timer
                        m_timeSinceAttack = 0.0f;

                        //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

                        // Disable movement 
                        m_disableMovementTimer = 0.45f;
                        m_animator.SetBool("Crouching", false);
                    }

                    // Air Attack Up
                    else if (Input.GetMouseButtonDown(0) && Input.GetKey("w") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.2f)
                    {
                        Debug.Log("Air attack up");
                        m_animator.SetTrigger("AirAttackUp");

                        // Reset timer
                        m_timeSinceAttack = 0.0f;
                    }

                    // Air Attack forward
                    else if (Input.GetMouseButtonDown(0) && (Input.GetKey("d") || Input.GetKey("a")) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.65f)
                    {
                        m_animator.SetTrigger("AirAttack");

                        // Reset timer
                        m_timeSinceAttack = 0.0f;
                    }

                    // Air Attack Neutral
                    else if (Input.GetMouseButtonDown(0) && !(Input.GetKey("d") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("w")) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.65f)
                    {

                        m_animator.SetTrigger("Nair");
                        //// Reset timer
                        m_timeSinceAttack = 0.0f;
                        m_disableMovementTimer = .65f;
                    }

                    // Throw
                    else if ((Input.GetKey("d") || Input.GetKey("a")) && Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                    {
                        hookDirection = "Side";
                        m_animator.SetTrigger("Throw");
                        hookActive = true;
                        m_timeSinceSideSpecial = 0.0f;

                        ThrowHook();
                        // Disable movement 
                        m_disableMovementTimer = 1.0f;
                    }

                    else if ((Input.GetKey("s")) && Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                    {
                        m_animator.SetTrigger("Slide");
                        m_dodging = true;
                        m_crouching = false;
                        m_animator.SetBool("Crouching", false);
                        m_body2d.velocity = new Vector2(m_facingDirection * 10f, m_body2d.velocity.y);
                    }
                    else if ((Input.GetKey("w")) && Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > 0.3f && m_timeSinceSideSpecial > 2.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                    {
                        hookDirection = "Up";
                        m_animator.SetTrigger("Throw");
                        hookActive = true;
                        m_timeSinceSideSpecial = 0.0f;

                        ThrowHook();
                        // Disable movement 
                        m_disableMovementTimer = 1.0f;
                    }

                    else if (Input.GetMouseButtonDown(1) && m_animator.GetBool("isParrying"))
                    {
                        m_dodging = true;
                        m_crouching = false;
                        m_animator.SetBool("isParrying", false);
                        m_animator.SetBool("Crouching", false);
                        m_animator.SetTrigger("Dodge");
                        m_body2d.velocity = new Vector2(-m_facingDirection * m_dodgeForce, m_body2d.velocity.y);
                    }

                    else if (Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.5f && m_timeSinceSideSpecial > 2.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                    {
                        // Reset timer


                        m_currentSpecial++;

                        // Loop back to one after second attack
                        if (m_currentSpecial > 2)
                            m_currentSpecial = 1;

                        // Reset Attack combo if time since last attack is too large
                        if (m_timeSinceNSpec > 1.1f)
                            m_currentSpecial = 1;

                        // Call one of the two attack animations "Attack1" or "Attack2"
                        m_animator.SetTrigger("SideSpec" + m_currentSpecial);
                        m_timeSinceNSpec = 0.0f;
                        m_timeSinceAttack = 0.0f;
                        //if (m_facingDirection == -1)
                        //{
                        //    Attack(jabDamage, jabKB, jabRange, 0, 2, backPoint);
                        //}
                        //else
                        //{
                        //    Attack(jabDamage, jabKB, jabRange, 0, 2, jabPoint);
                        //}


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
                    else if (Input.GetButtonDown("Jump") && (m_grounded || m_wallSlide || m_jumpCount <= 1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_disableMovementTimer < 0.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && m_launched == false)
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
                    else if (Input.GetKeyDown("s") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_disableMovementTimer < 0.0f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
                    {
                        m_crouching = true;
                        m_animator.SetBool("Crouching", true);
                        m_body2d.velocity = new Vector2(m_body2d.velocity.x / 2.0f, m_body2d.velocity.y);
                    }
                    else if (Input.GetKeyUp("s") && m_crouching)
                    {
                        m_crouching = false;
                        m_animator.SetBool("Crouching", false);
                    }
                    //Walk
                    else if (m_moving && Input.GetKey(KeyCode.LeftControl))
                    {
                        m_animator.SetInteger("AnimState", 2);
                        m_maxSpeed = m_walkSpeed;
                    }

                    //Run
                    else if (m_moving && m_launched == false && m_timeSinceSideSpecial > 1.0f)
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
            infoText.text = "Press Tab to Teleport";
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
                infoText.text = "Press Tab";
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
        if (atPortal == false)
        {
            infoText.text = "";
        }
    }

    public void ThrowHook()
    {
        Quaternion rotQuat = new Quaternion();
        if (hookDirection == "Up")
        {
            rotQuat = new Quaternion(90f, 0f, 0f, 0f);
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
}
