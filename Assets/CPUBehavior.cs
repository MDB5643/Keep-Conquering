using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CPUBehavior : Conqueror
{
    private GrabableLedge ledge;
    private Rect buttonPos1;
    private Rect buttonPos2;
    public float attackDistance = 2f;
    public float minDamage = 0.0f;
    public float smashAttackInterval = 15f;
    public float timeSinceSmashAttack = 0.0f;
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform target;
    public bool inRange; //check if player is in range
    public GameObject hotZone;
    public GameObject triggerArea;

    //Pathfinding
    public GameObject firstLauncherTarget;
    public GameObject MovingPlatformTarget;
    public GameObject GondolaTarget;
    public GameObject GondolaEdgeTarget;
    public GameObject TowerPlatformTarget;
    public GameObject TowerPlatformEdgeTarget;
    public GameObject EyeTarget;
    public GameObject KeepEyeTarget;
    private string CurrentJumpTarget = "";

    private AudioManager_PrototypeHero m_audioManager;

    public float m_TimeSinceJab2 = 0.0f;

    public string routine = "Lane";

    // Use this for initialization
    void Start()
    {
        isPlayer = false;
        playerNumber = 3;
        if (playerNumber == 4)
        {
            routine = "Lane";
        }
        if (playerNumber == 3)
        {
            routine = "Lane";
        }
        teamColor = "Red";
        if (teamColor == "Red")
        {
            firstLauncherTarget = GameObject.Find("RedMinionLauncherTarget");
            MovingPlatformTarget = GameObject.Find("RedMinionPlatformTarget");
            GondolaTarget = GameObject.Find("RedMinionGondolaTarget");
            EyeTarget = GameObject.Find("RedMinionEyeTarget");
            TowerPlatformTarget = GameObject.Find("Jumppoint5");
        }
        if (GondolaTarget == null)
        {
            routine = "HoldGround";
        }
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
        if (routine == "Lane")
        {
            m_DamageDisplay = GameObject.Find("DmgDisplayP2").GetComponentInChildren<Text>();
        }
        //

        SelectTarget();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!m_fallingdown)
        {
            if (EyeTarget == null && KeepEyeTarget != null)
            {
                EyeTarget = KeepEyeTarget;
            }

            if (teamColor == "Red" && routine == "Lane" && transform.position.x < TowerPlatformTarget.transform.position.x && EyeTarget != null)
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
            if (teamColor == "Blue" && routine == "Lane" && transform.position.x > TowerPlatformTarget.transform.position.x && EyeTarget != null)
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

            if (target == null && routine == "Lane")
            {
                target = TowerPlatformTarget.transform;
                CurrentJumpTarget = "Gondola";
            }

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
            m_timeSinceSideSpecial += Time.deltaTime;
            m_timeSinceStun += Time.deltaTime;
            m_timeSinceNSpec += Time.deltaTime;
            timeSinceSmashAttack += Time.deltaTime;
            m_timeSinceChargeStart += Time.deltaTime;

            // Decrease timer that checks if we are in parry stance
            m_parryTimer -= Time.deltaTime;

            // Decrease timer that disables input movement. Used when attacking
            m_disableMovementTimer -= Time.deltaTime;


            if (!m_inHitStun && !isGrappled && !m_fallingdown)
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

                    //Update stock count
                    if (m_StockDisplay)
                    {
                        m_StockDisplay.text = "x" + m_StockCount;
                    }
                    // Check for interactable overlapping objects
                    CheckOverlaps();
                    // Decrease death respawn timer 
                    m_respawnTimer -= Time.deltaTime;



                    // Decrease timer that checks if we are in parry stance
                    //m_parryTimer -= Time.deltaTime;

                    // Decrease timer that disables input movement. Used when attacking
                    m_disableMovementTimer -= Time.deltaTime;

                    if (m_StockCount == 0)
                    {
                        isEliminated = true;
                    }

                    // Respawn Hero if dead
                    if (m_dead && m_respawnTimer < 0.0f && !isEliminated)
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
                    if (!m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_animator.GetBool("isParrying") && m_disableMovementTimer < 0.0f && !m_launched && m_fSmashCharging == false
                        && m_uSmashCharging == false && m_dSmashCharging == false && !preview)
                    {
                        if (!m_isInKnockback)
                        {
                            m_timeSinceKnockBack = 0.0f;
                            if (m_KnockBackMomentumX <= 1 && m_KnockBackMomentumY <= 1)
                            {
                                //m_body2d.velocity = new Vector2(m_maxSpeed * SlowDownSpeed, m_body2d.velocity.y);

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
                                m_animator.SetBool("Knockback", false);
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

                    if (m_timeSinceStun > 4.0f && !m_isInKnockback)
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
                        if (m_uSmashCharging == true && !m_fSmashCharging && m_timeSinceChargeStart > m_maxSmashChargeTime)
                        {
                            m_animator.SetTrigger("USmash");
                            m_animator.SetBool("USmashCharge", false);
                            // Reset timer
                            m_timeSinceAttack = 0.0f;
                            timeSinceSmashAttack = 0.0f;

                            m_uSmashCharging = false;
                            m_fullCharge = false;

                            m_disableMovementTimer = 0.35f;
                        }
                        else if (m_fSmashCharging == true && !m_uSmashCharging && m_timeSinceChargeStart > m_maxSmashChargeTime)
                        {
                            m_animator.SetTrigger("FSmash");
                            m_animator.SetBool("FSmashCharge", false);
                            // Reset timer
                            m_timeSinceAttack = 0.0f;
                            timeSinceSmashAttack = 0.0f;

                            m_fSmashCharging = false;
                            m_fullCharge = false;

                            m_disableMovementTimer = 0.35f;
                        }

                        if (attackDistance >= distance && inRange && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.15f && m_TimeSinceJab2 > 0.45f)
                        {
                            if (m_uSmashCharging == true && !m_fSmashCharging && m_timeSinceChargeStart > 2.0f)
                            {
                                m_animator.SetTrigger("USmash");
                                m_animator.SetBool("USmashCharge", false);
                                // Reset timer
                                m_timeSinceAttack = 0.0f;
                                timeSinceSmashAttack = 0.0f;

                                m_uSmashCharging = false;
                                m_fullCharge = false;

                                m_disableMovementTimer = 0.35f;
                            }
                            else if (m_fSmashCharging == true && !m_uSmashCharging && m_timeSinceChargeStart > 2.0f)
                            {
                                m_animator.SetTrigger("FSmash");
                                m_animator.SetBool("FSmashCharge", false);
                                // Reset timer
                                m_timeSinceAttack = 0.0f;
                                timeSinceSmashAttack = 0.0f;

                                m_fSmashCharging = false;
                                m_fullCharge = false;

                                m_disableMovementTimer = 0.35f;
                            }
                            else if (m_wallSensorR2.State() || m_wallSensorL2.State() && !m_fSmashCharging && !m_uSmashCharging)
                            {
                                if (timeSinceSmashAttack > smashAttackInterval)
                                {
                                    if (m_fSmashCharging != true && !m_uSmashCharging)
                                    {
                                        m_timeSinceChargeStart = 0.0f;
                                        m_uSmashCharging = true;
                                        m_uSmashCharging = true;
                                        m_animator.SetBool("USmashCharge", true);


                                    }
                                    // Disable movement 
                                    m_disableMovementTimer = 0.35f;
                                }
                                else
                                {
                                    m_animator.SetTrigger("UpAttack");

                                    // Reset timer
                                    m_timeSinceAttack = 0.0f;

                                    //Attack(upTiltDamage, upTiltKB, upTiltRange, 0, 2, upTiltPoint);

                                    // Disable movement 
                                    m_disableMovementTimer = 0.35f;
                                }

                            }
                            else
                            {
                                if (timeSinceSmashAttack > smashAttackInterval)
                                {
                                    if (m_fSmashCharging != true && !m_uSmashCharging)
                                    {
                                        m_timeSinceChargeStart = 0.0f;
                                        m_fSmashCharging = true;
                                        m_animator.SetBool("FSmashCharge", true);

                                    }
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
                    if (m_disableMovementTimer < 0.0f && m_grounded && !m_fSmashCharging && !m_uSmashCharging)
                    {
                        Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

                        transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_runSpeed * Time.deltaTime);

                        m_moving = true;
                    }


                    //Run
                    if (m_moving)
                    {
                        m_animator.SetInteger("AnimState", 1);
                        m_maxSpeed = m_runSpeed;
                        m_animator.SetBool("StayDown", false);
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
                    if (transform.name.Contains("Prototype"))
                    {
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
        else
        {
            m_animator.speed = 1;
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
        if ((coll.gameObject.CompareTag("BlastZone") || coll.gameObject.CompareTag("BlastZoneMid")) && !m_dead)
        {
            currentDamage = 0.0f;
            m_animator.SetBool("Knockback", false);
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            m_respawnTimer = 2.5f;
            DisableWallSensors();
            m_dead = true;
            m_StockCount--;
            //gameObject.SetActive(false);
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

        if (routine == "Lane")
        {
            if (EyeTarget != null)
            {
                float distanceToEye = Vector2.Distance(transform.position, EyeTarget.transform.position);
            }
            if (teamColor == "Red")
            {
                if (target == null)
                {
                    target = MovingPlatformTarget.transform;
                }
                else if (transform.position.x < firstLauncherTarget.transform.position.x && transform.position.x > MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge")
                {
                    target = MovingPlatformTarget.transform;
                }
                else if (transform.position.y >= 0f && transform.position.x < MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge" && CurrentJumpTarget != "EnemyPlatform" && CurrentJumpTarget != "EnemyKeep")
                {
                    target = GondolaTarget.transform;
                }
                else
                {
                    if (distanceToLeft > distanceToRight)
                    {
                        target = leftLimit;
                    }
                    else
                    {
                        target = rightLimit;
                    }
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
                    target = GondolaTarget.transform;
                }
            }
            if (CurrentJumpTarget == "EnemyPlatform")
            {
                target = GondolaEdgeTarget.transform;
            }
        }
        
        else if (routine == "HoldGround")
        {
            if (distanceToLeft > distanceToRight)
            {
                target = leftLimit;
            }
            else
            {
                target = rightLimit;
            }
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
            var targetclosestPoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
            var sourceclosestPoint = new Vector2(transform.position.x, transform.position.y);

            var midPointX = (targetclosestPoint.x + sourceclosestPoint.x) / 2f;
            var midPointY = (targetclosestPoint.y + sourceclosestPoint.y) / 2f;

            if (collision.transform.name.Contains("Smash") || collision.transform.name.Contains("Jab1Hitbox"))
            {
                Instantiate(HeavyAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            }
            else
            {
                Instantiate(LightAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            }
            if (collision.transform.name.Contains("ChargeBall"))
            {
                GameObject.Destroy(collision.transform.parent.gameObject);
            }
            if (collision.transform.name.Contains("MagicArrow") && collision.GetComponentInParent<ProjectileBehavior>().teamColor != teamColor)
            {
                GameObject.Destroy(collision.transform.gameObject);
            }
            if (collision.GetComponentInParent<CombatManager>())
            {
                collision.GetComponentInParent<CombatManager>().Hit(transform, collision.transform.name);
            }
            else if (collision.GetComponent<CombatManager>())
            {
                collision.GetComponent<CombatManager>().Hit(transform, collision.transform.name);
            }
        }
        if (collision.gameObject.CompareTag("RightLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(27, 13), ForceMode2D.Impulse);
        }
        if (collision.gameObject.CompareTag("LeftLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-27, 13), ForceMode2D.Impulse);
        }
        if (collision.gameObject.CompareTag("LeftTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-7, 21), ForceMode2D.Impulse);
        }
        if (collision.gameObject.CompareTag("RightTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(7, 21), ForceMode2D.Impulse);
        }
        else if (collision.transform.tag == "MinionJumpUp" && m_grounded && CurrentJumpTarget == "VerticalPlatform")
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
            m_launched = true;
            CurrentJumpTarget = "MainPlatform";
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
        }
        else if (collision.transform.tag == "MinionJumpToLBottomPlat" && m_grounded && (target.name == "RedMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "LeftEdge") && !m_launched)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-2f, 1f), ForceMode2D.Impulse);
            m_launched = true;
            target = GondolaEdgeTarget.transform;
            CurrentJumpTarget = "EnemyPlatform";
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

        }
        else if (collision.transform.tag == "MinionJumpToEnemyPlat" && m_grounded && CurrentJumpTarget == "EnemyPlatform" && EyeTarget != null)
        {
            rightLimit = GameObject.Find("CPULimitR2").transform;
            leftLimit = GameObject.Find("CPULimitL2").transform;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-7f, 7), ForceMode2D.Impulse);
            m_launched = true;
            target = EyeTarget.transform;
            CurrentJumpTarget = "None";
        }
        else if (collision.transform.tag == "MinionJumpToEnemyKeep" && m_grounded && KeepEyeTarget != null)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-3f, 4), ForceMode2D.Impulse);
            target = KeepEyeTarget.transform;
            CurrentJumpTarget = "None";
        }
        else if (collision.transform.tag == "MinionJumpSmall" && m_grounded && CurrentJumpTarget == "None" && EyeTarget != null)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-.5f, 0), ForceMode2D.Impulse);
            target = EyeTarget.transform;
            CurrentJumpTarget = "None";
        }
        if (collision.transform.tag == "MinionJumpLeft")
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-6f, 5), ForceMode2D.Impulse);
            CurrentJumpTarget = "VerticalPlatform";
            SelectTarget();
            m_launched = true;
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
        if (collision.gameObject.name == "targetingHotzone" && !m_isInHotZone && collision.GetComponentInParent<TowerEye>().teamColor != teamColor)
        {
            collision.GetComponentInParent<TowerEye>().enemiesInBounds.Add(transform.gameObject);
            m_isInHotZone = true;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "MinionBoardGondola" && !m_launched)
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

    protected override void BasicAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void SpecialAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void ForwardSmashAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void UpSmashAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void DownSmashAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void ReverseForwardSmashAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void ShieldAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    protected override void DodgeAction(InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }
}
