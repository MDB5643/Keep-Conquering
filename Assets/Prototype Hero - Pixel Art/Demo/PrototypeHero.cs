using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PrototypeHero : Conqueror {

    
    private GrabableLedge ledge;
    private Rect buttonPos1;
    private Rect buttonPos2;
    public GameObject activeDairHitbox;
    HookBehavior hookShot;

    public GameObject currentHookTarget;
    private float crosshairLingerTime = 0.0f;
    public float maxCrosshairLingerTime = 1.5f;
    public bool hookLatched = false;

    public float m_TimeSinceJab2 = 0.0f;

    // Use this for initialization
    void Start ()
    {
        acrobat = true;
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
        if (m_manaBar)
        {
            m_manaBar.gameObject.SetActive(false);
        }
        if (!isPlayer && !preview)
        {
            CPUBrain.enabled = true;
        }
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
        // Decrease death respawn timer 
        m_respawnTimer -= Time.deltaTime;
        // Respawn Hero if dead
        if (m_dead && m_respawnTimer < 0.0f && !isEliminated)
            RespawnHero();

        if (currentHookTarget != null)
        {
            if (currentHookTarget.GetComponent<MinionBehavior>() && currentHookTarget.GetComponent<MinionBehavior>().m_dead)
            {
                currentHookTarget = null;
                crosshairLingerTime = 0;
            }
            else if (currentHookTarget.GetComponent<Conqueror>() && currentHookTarget.GetComponent<Conqueror>().m_dead)
            {
                currentHookTarget = null;
                crosshairLingerTime = 0;
            }
            crosshairLingerTime += Time.deltaTime;
            if (crosshairLingerTime >= maxCrosshairLingerTime && gameObject != null)
            {
                currentHookTarget = null;
                crosshairLingerTime = 0;
            }
        }

        if (!m_inHitStun && !isGrappled && !m_dead)
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
                if (m_timeSinceSideSpecial > 4.5f && hookActive == true && hookPrefab != null)
                {
                    hookActive = false;
                }
                if (hookShot != null && hookShot.latched == true)
                {
                    m_body2d.velocity = hookShot.pullForce;
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
                //Update stock count
                if (m_StockDisplay)
                {
                    m_StockDisplay.text = "x" + m_StockCount;
                }
                // Check for interactable overlapping objects
                CheckOverlaps();

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

                if (m_StockCount == 0)
                {
                    isEliminated = true;
                }


                if (m_dead)
                {
                    m_freefall = false;
                    if (activeDairHitbox)
                    {
                        GameObject.Destroy(activeDairHitbox);
                    }
                    return;
                }
                    

                //Check if character just landed on the ground
                if (!m_grounded && m_groundSensor.State())
                {
                    m_jumpCount = 0;
                    m_grounded = true;
                    m_animator.SetBool("Grounded", m_grounded);
                    m_launched = false;
                    m_freefall = false;
                    if (activeDairHitbox)
                    {
                        GameObject.Destroy(activeDairHitbox);
                    }
                    if (inAttack)
                    {
                        inAttack = false;
                        m_animator.SetTrigger("Land");
                    }
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

                
                // Set AirSpeed in animator
                m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

                // Set Animation layer for hiding sword
                int boolInt = m_hideSword ? 1 : 0;
                m_animator.SetLayerWeight(1, boolInt);

                // Check if all sensors are setup properly
                if (m_wallSensorR1 && m_wallSensorR2 && m_wallSensorL1 && m_wallSensorL2)
                {
                    //bool prevWallSlide = m_wallSlide;
                    ////Wall Slide
                    //// True if TWO right sensors are colliding and character is facing right
                    //// OR if TWO left sensors are colliding and character is facing left
                    //m_wallSlide = (m_wallSensorR1.State() && m_wallSensorR2.State() && m_facingDirection == 1) || (m_wallSensorR1.State() && m_wallSensorR0.State() && m_facingDirection == 1) || (m_wallSensorL1.State() && m_wallSensorL2.State() && m_facingDirection == -1) || (m_wallSensorL1.State() && m_wallSensorL0.State() && m_facingDirection == -1);
                    //if (m_grounded)
                    //    m_wallSlide = false;
                    //m_animator.SetBool("WallSlide", m_wallSlide);
                    ////Play wall slide sound
                    //if (prevWallSlide && !m_wallSlide)
                    //    AudioManager_PrototypeHero.instance.StopSound("WallSlide");


                    //Grab Ledge
                    // True if either bottom right sensor is colliding and top right sensor is not colliding 
                    // OR if bottom left sensor is colliding and top left sensor is not colliding 
                    if (m_ledgeGrab)
                    {
                        m_climbPosition = ledge.transform.position + new Vector3(ledge.topClimbPosition.x, ledge.topClimbPosition.y, 0);
                    }
                    bool shouldGrab = !m_ledgeClimb && !m_ledgeGrab && (m_wallSensorR1.State() || m_wallSensorL1.State() || m_wallSensorR0.State() || m_wallSensorL0.State() || m_wallSensorR2.State() || m_wallSensorL2.State());
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
                            m_freefall = false;
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
                            m_jumpCount = 0;
                            if (transform.tag == "PlayerMid")
                            {
                                SetLayerRecursively(gameObject, LayerMask.NameToLayer("LedgeGrabMid"));
                            }
                            else if (transform.tag == "Player")
                            {
                                SetLayerRecursively(gameObject, LayerMask.NameToLayer("LedgeGrab"));
                            }
                        }
                        m_animator.SetBool("LedgeGrab", m_ledgeGrab);
                        

                    }

                }

                // -- Handle Animations --

                if (m_timeSinceStun > 4.0f && !preview && !m_isInKnockback && !inAttack)
                {
                    if (m_animator.GetBool("isStunned"))
                    {
                        m_animator.SetBool("isStunned", false);
                    }

                    //Walk
                    else if (m_moving && Mathf.Abs(inputXY.x) < .45)
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
                if (!isPlayer && !(hookShot != null && hookShot.latched == true))
                {
                    CPUBrain.Move();
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
            hookLatched = false;
            if (m_inHitStun)
            {
                bool trembleUp = false;
                bool trembleBack = false;
                m_body2d.velocity = Vector2.zero;
                m_body2d.gravityScale = 0;
                m_timeSinceHitStun += Time.deltaTime;

                m_timeSinceTremble += Time.deltaTime;
                if (m_timeSinceTremble > .06)
                {
                    transform.localPosition += new Vector3(-.04f, 0, 0);
                    trembleBack = true;
                    trembleUp = false;
                    m_timeSinceTremble = 0;
                }
                else if (m_timeSinceTremble > .03)
                {
                    transform.localPosition += new Vector3(.02f, 0, 0);
                    trembleUp = true;
                }
                

                m_animator.speed = 0;
                if (activeDairHitbox)
                {
                    GameObject.Destroy(activeDairHitbox);
                }
                if (m_timeSinceHitStun >= m_hitStunDuration)
                {
                    transform.localPosition = preTremblePosition;
                    m_body2d.gravityScale = 1.25f;
                    Knockback(incomingKnockback, incomingAngle, incomingXMod, incomingYMod);
                    m_isInKnockback = true;
                }
            }
            else if (isGrappled)
            {
                m_animator.speed = 0;
                if (activeDairHitbox)
                {
                    GameObject.Destroy(activeDairHitbox);
                }
                var grabbingPlayerYPos = m_body2d.transform.parent ? m_body2d.transform.parent.position.y : m_body2d.position.y;
                m_body2d.position = new Vector2(m_body2d.position.x, grabbingPlayerYPos);
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
        if (coll.gameObject.CompareTag("BlastZone") || coll.gameObject.CompareTag("BlastZoneMid") && !m_dead)
        {
            currentDamage = 0.0f;
            m_isInKnockback = false;
            isInStartUp = false;
            isGrappled = false;
            transform.parent = null;
            m_inGroundSlam = false;
            m_isInHitStop = false;
            m_isParrying = false;
            m_animator.SetBool("Knockback", false);
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            m_respawnTimer = 2.5f;
            DisableWallSensors();
            m_dead = true;
            m_StockDisplay.text = "x" + (m_StockCount - 1);

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
        if (coll.gameObject.CompareTag("Ground") && m_animator.GetBool("Knockback") && transform.position.y >= coll.transform.position.y && transform.GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            m_animator.SetTrigger("FallToProne");
            m_animator.SetBool("StayDown", true);
            m_fallingdown = true;
            m_isInKnockback = false;
            m_animator.SetBool("Knockback", false);
        }
    }

    private void CheckOverlaps()
    {
        Collider2D collider = gameObject.GetComponentInChildren<Collider2D>();
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
        if (atPortal == false && infoText != null)
        {
            infoText.text = "";
        }
    }

    public void ThrowHook()
    {
        Quaternion rotQuat = new Quaternion();
        
        if (m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }

        hookShot = Instantiate(hookPrefab, launchOffset.position, rotQuat);

        if (hookDirection == "Up")
        {
            hookShot.transform.rotation *= Quaternion.Euler(0, 0, -90);
        }
        else
        {
            hookShot.target = null;
            currentHookTarget = null;
        }
        if (hookDirection == "Up" && currentHookTarget != null)
        {
            hookShot.homing = true;
            hookShot.target = currentHookTarget;
        }

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

    protected override void BasicAction(InputAction.CallbackContext ctx)
    {

        //Up Attack
        if (inputXY.y > 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime && Mathf.Abs(inputXY.x) < Mathf.Abs(inputXY.y)
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {
            m_LagTime = .18f;
            m_animator.SetTrigger("UpAttack");

            // Reset timer
            m_timeSinceAttack = 0.0f;

            //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

            // Disable movement 
            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }

        //Down Tilt Attack
        else if (inputXY.y < 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_grounded && m_timeSinceAttack > m_LagTime && Mathf.Abs(inputXY.x) < Mathf.Abs(inputXY.y)
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {
            m_LagTime = .25f;
            m_animator.SetTrigger("DTiltAttack");

            // Reset timer
            m_timeSinceAttack = 0.0f;

            //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

            // Disable movement 
            m_disableMovementTimer = m_LagTime;
            m_animator.SetBool("Crouching", false);
            inAttack = true;
        }

        //Forward tilt Attack
        else if (Mathf.Abs(inputX) > 0 && Mathf.Abs(inputXY.x) < .45 && !Input.GetKey("s") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime && m_timeSinceNSpec > 1.1f
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !isInStartUp && ctx.phase == InputActionPhase.Started)
        {
            if (m_facingDirection == 1 && inputX < 0)
            {
                m_SR.flipX = true;
                m_facingDirection = -1;
            }
            else if (m_facingDirection == -1 && inputX > 0)
            {
                m_SR.flipX = false;
                m_facingDirection = 1;
            }
            m_LagTime = .22f;
            // Reset timer
            m_timeSinceAttack = 0.0f;

            m_animator.SetTrigger("FTilt");

            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }

        //Dash Attack
        else if (Mathf.Abs(inputXY.x) > 0 && !Input.GetKey("s") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime && m_timeSinceNSpec > 1.1f
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && !isInStartUp && ctx.phase == InputActionPhase.Started)
        {
            m_LagTime = .4f;
            // Reset timer
            m_timeSinceAttack = 0.0f;

            m_currentAttack = 1;

            // Call one of the two attack animations "Attack1" or "Attack2"
            m_animator.SetTrigger("DashAttack");

            m_body2d.velocity = new Vector2(m_facingDirection * m_dodgeForce + m_facingDirection * 3, m_body2d.velocity.y);

            m_disableMovementTimer = 0.4f;
            inAttack = true;
        }

        //Jab Attack
        else if ( inputXY.x == 0 && inputXY.y == 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime && m_TimeSinceJab2 > 0.45f && m_timeSinceNSpec > 1.1f
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {

            m_LagTime = .2f;
            m_currentAttack++;

            if (m_currentAttack == 2)
            {
                m_TimeSinceJab2 = 0.0f;
            }

            // Loop back to one after second attack
            if (m_currentAttack > 2)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > .6f)
                m_currentAttack = 1;

            // Call one of the two attack animations "Attack1" or "Attack2"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }

        //Air Slam Attack
        else if (inputXY.y < 0 && !m_ledgeGrab && !m_ledgeClimb && !m_grounded && ctx.phase == InputActionPhase.Started && Mathf.Abs(inputXY.x) < Mathf.Abs(inputXY.y))
        {
            m_animator.SetTrigger("AttackAirSlam");
            m_body2d.velocity = new Vector2(0.0f, -m_jumpForce);
            m_disableMovementTimer = 0.8f;
            m_timeSinceAttack = 0.0f;
        }

        // Air Attack Up
        else if (inputXY.y > 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > m_LagTime && ctx.phase == InputActionPhase.Started && Mathf.Abs(inputXY.x) < Mathf.Abs(inputXY.y))
        {
            m_LagTime = .45f;
            Debug.Log("Air attack up");
            m_animator.SetTrigger("AirAttackUp");

            m_timeSinceAttack = 0.0f;
            inAttack = true;
        }

        // Air Attack forward
        else if (Mathf.Abs(inputXY.x) > 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && 
            m_timeSinceAttack > m_LagTime && ctx.phase == InputActionPhase.Started)
        {
            if (m_facingDirection == 1 && inputXY.x < 0)
            {
                m_SR.flipX = true;
                m_facingDirection = -1;
            }
            else if (m_facingDirection == -1 && inputXY.x > 0)
            {
                m_SR.flipX = false;
                m_facingDirection = 1;
            }
            m_LagTime = .4f;
            m_animator.SetTrigger("AirAttack");
            m_timeSinceAttack = 0.0f;
            inAttack = true;
        }

        // Air Attack Neutral
        else if (inputXY.x == 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > m_LagTime && ctx.phase == InputActionPhase.Started)
        {
            m_LagTime = .4f;
            m_animator.SetTrigger("Nair");
            m_timeSinceAttack = 0.0f;
            inAttack = true;
        }

        //Ledge Attack
        else if (!m_dodging && m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_timeSinceAttack > m_LagTime
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {
            DisableWallSensors();
            m_ledgeClimb = true;
            m_body2d.gravityScale = 0;
            m_disableMovementTimer = 6.0f / 14.0f;
            m_LagTime = .3f;
            m_animator.SetTrigger("LedgeAttack");
            m_timeSinceAttack = 0.0f;
            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }
    }

    protected override void SpecialAction(InputAction.CallbackContext ctx)
    {


        if (inputXY.y < 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > m_LagTime && m_timeSinceSideSpecial > 2.0f && Mathf.Abs(inputXY.x) < Mathf.Abs(inputXY.y)
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {
            m_LagTime = .45f;
            m_animator.SetTrigger("Slide");
            m_dodging = true;
            m_crouching = false;
            m_animator.SetBool("Crouching", false);
            m_body2d.velocity = new Vector2(m_facingDirection * 10f, m_body2d.velocity.y);
            inAttack = true;
        }
        else if (inputXY.y > 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > m_LagTime && m_timeSinceSideSpecial > 2.0f && Mathf.Abs(inputXY.x) < Mathf.Abs(inputXY.y)
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {
            m_LagTime = .45f;
            hookDirection = "Up";
            m_animator.SetTrigger("UpThrow");
            hookActive = true;
            m_timeSinceSideSpecial = 0.0f;

            ThrowHook();
            // Disable movement 
            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }
        // Throw
        else if (Mathf.Abs(inputXY.x) > 0 && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_timeSinceAttack > m_LagTime && m_timeSinceSideSpecial > 2.0f
            && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {
            if (m_facingDirection == 1 && inputXY.x < 0)
            {
                m_SR.flipX = true;
                m_facingDirection = -1;
            }
            else if (m_facingDirection == -1 && inputXY.x > 0)
            {
                m_SR.flipX = false;
                m_facingDirection = 1;
            }
            m_LagTime = .45f;
            hookDirection = "Side";
            m_animator.SetTrigger("Throw");
            hookActive = true;
            m_timeSinceSideSpecial = 0.0f;

            ThrowHook();
            // Disable movement 
            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }


        else if ( !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime && m_timeSinceSideSpecial > 1.8f
                        && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && ctx.phase == InputActionPhase.Started)
        {
            // Reset timer

            m_LagTime = .35f;
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

            // Disable movement 
            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }
    }

    protected override void ForwardSmashAction(InputAction.CallbackContext ctx)
    {
        //Forward Smash Attack
        if (ctx.phase == InputActionPhase.Started && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded
            && m_timeSinceAttack > m_LagTime && m_timeSinceNSpec > 1.1f && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
        {
            m_LagTime = .15f;
            if (m_fSmashCharging != true)
            {
                m_body2d.velocity = new Vector2(0.0f, 0.0f);
                m_timeSinceChargeStart = 0.0f;
                m_fSmashCharging = true;
            }

            m_animator.SetBool("FSmashCharge", true);


            // Disable movement 
            m_disableMovementTimer = m_LagTime;
        }

        else if (ctx.phase == InputActionPhase.Canceled && m_fSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime)
        {
            m_LagTime = .33f;
            m_animator.SetTrigger("FSmash");
            m_animator.SetBool("FSmashCharge", false);
            // Reset timer
            m_timeSinceAttack = 0.0f;

            m_fSmashCharging = false;
            m_timeSinceChargeStart = 0.0f;
            m_fullCharge = false;

            // Call one of the two attack animations "Attack1" or "Attack2"

            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }
    }

    protected override void UpSmashAction(InputAction.CallbackContext ctx)
    {
        //Up Smash Attack
        if (ctx.phase == InputActionPhase.Started && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded
                        && m_timeSinceAttack > m_LagTime && m_fSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
        {
            m_LagTime = .15f;
            if (m_uSmashCharging != true)
            {
                m_body2d.velocity = new Vector2(0.0f, 0.0f);
                m_timeSinceChargeStart = 0.0f;
                m_uSmashCharging = true;
            }

            m_uSmashCharging = true;
            m_animator.SetBool("USmashCharge", true);

            // Disable movement 
            m_disableMovementTimer = m_LagTime;
        }

        else if (ctx.phase == InputActionPhase.Canceled && m_uSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime
            && m_timeSinceNSpec > 1.1f)
        {
            m_LagTime = .33f;
            m_animator.SetTrigger("USmash");
            m_animator.SetBool("USmashCharge", false);
            // Reset timer
            m_timeSinceAttack = 0.0f;

            m_uSmashCharging = false;
            m_fullCharge = false;

            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }
    }

    protected override void DownSmashAction(InputAction.CallbackContext ctx)
    {
        //Down Smash Attack
        if (ctx.phase == InputActionPhase.Started && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded
                        && m_timeSinceAttack > m_LagTime && m_fSmashCharging == false && m_uSmashCharging == false && m_isParrying == false)
        {
            m_LagTime = .15f;
            if (m_dSmashCharging != true)
            {
                m_body2d.velocity = new Vector2(0.0f, 0.0f);
                m_timeSinceChargeStart = 0.0f;
                m_dSmashCharging = true;
            }

            m_dSmashCharging = true;
            m_animator.SetBool("DSmashCharge", true);

            // Disable movement 
            m_disableMovementTimer = m_LagTime;
        }

        else if (ctx.phase == InputActionPhase.Canceled && m_dSmashCharging == true && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > m_LagTime
            && m_timeSinceNSpec > 1.1f)
        {
            m_LagTime = .33f;
            m_animator.SetTrigger("DSmash");
            m_animator.SetBool("DSmashCharge", false);
            // Reset timer
            m_timeSinceAttack = 0.0f;

            m_dSmashCharging = false;
            m_fullCharge = false;

            m_disableMovementTimer = m_LagTime;
            inAttack = true;
        }
    }

    protected override void ReverseForwardSmashAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void ShieldAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Canceled)
        {
            m_animator.SetBool("isParrying", false);
            m_parryTimer = -1.0f;
            m_isParrying = false;
        }

        // Parry & parry stance
        else if (ctx.phase == InputActionPhase.Started && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false)
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
                m_body2d.velocity = new Vector2(0.0f, 0.0f);
                m_animator.SetTrigger("ParryStance");
                m_animator.SetBool("isParrying", true);
                m_parryTimer = 7.0f / 12.0f;
                m_isParrying = true;
            }

        }

        else if (!m_grounded && ctx.phase == InputActionPhase.Started && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false
            && !m_isInKnockback && !m_inHitStun && !m_isInHitStop)
        {
            m_dodging = true;
            m_isParrying = false;
            m_crouching = false;
            m_animator.SetBool("isParrying", false);
            m_animator.SetBool("Crouching", false);
            m_animator.SetTrigger("AirDodge");
        }
    }

    protected override void DodgeAction(InputAction.CallbackContext ctx)
    {
        m_isParrying = false;
        if (ctx.phase == InputActionPhase.Started && m_animator.GetBool("isParrying") && Mathf.Abs(inputXY.x) > 0)
        {
            m_dodging = true;
            m_crouching = false;
            m_animator.SetBool("isParrying", false);
            m_animator.SetBool("Crouching", false);
            m_animator.SetTrigger("Dodge");
            m_body2d.velocity = new Vector2(m_facingDirection * m_dodgeForce, m_body2d.velocity.y);
            m_SR.flipX = !m_SR.flipX;
        }

        else if (ctx.phase == InputActionPhase.Started && m_animator.GetBool("isParrying"))
        {
            m_dodging = true;
            m_crouching = false;
            m_animator.SetBool("isParrying", false);
            m_animator.SetBool("Crouching", false);
            m_animator.SetTrigger("Dodge");
            m_body2d.velocity = new Vector2(-m_facingDirection * m_dodgeForce, m_body2d.velocity.y);

        }
    }
}
