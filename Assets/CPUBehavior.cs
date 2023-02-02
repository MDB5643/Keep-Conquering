using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class CPUBehavior : MonoBehaviour
{

    public float m_runSpeed = 4.5f;
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
    public bool inRange; //check if player is in range
    public GameObject hotZone;
    public GameObject triggerArea;
    public bool isGrappled;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private SpriteRenderer m_SR;
    private Sensor_Prototype m_groundSensor;
    private Sensor_Prototype m_wallSensorR1;
    private Sensor_Prototype m_wallSensorR2;
    private Sensor_Prototype m_wallSensorL1;
    private Sensor_Prototype m_wallSensorL2;
    private bool m_grounded = false;
    private bool m_moving = false;
    private bool m_dead = false;
    private bool m_dodging = false;
    private bool m_wallSlide = false;
    private bool m_ledgeGrab = false;
    private bool m_ledgeClimb = false;
    private bool m_crouching = false;
    private Vector3 m_climbPosition;
    private int m_facingDirection = 1;
    private float m_disableMovementTimer = 0.0f;
    private float m_parryTimer = 0.0f;
    private float m_respawnTimer = 0.0f;
    private Vector3 m_respawnPosition = Vector3.zero;
    private int m_currentAttack = 0;
    public float m_timeSinceAttack = 0.0f;
    private float m_gravity;
    public float m_maxSpeed = 4.5f;
    public LayerMask m_WhatIsPortal;

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

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

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

        // Decrease timer that checks if we are in parry stance
        m_parryTimer -= Time.deltaTime;

        // Decrease timer that disables input movement. Used when attacking
        m_disableMovementTimer -= Time.deltaTime;

        // Respawn Hero if dead
        if (m_dead && m_respawnTimer < 0.0f)
            RespawnHero();

        if (m_dead)
            return;

        Move();

       
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }


    void Attack()
    {
        //timer = initTimer; //reset timer when player enters atttack range 
        //attackMode = true; //to check if enemy can still attack
        //
        //isCooling = true;
        //anim.SetBool("canWalk", false);
        //anim.SetTrigger("Attack");

        //var playerCollider = GetComponent<BoxCollider2D>();
        //
        //Vector2 attackHitboxCenter = jabPoint.position;
        //
        ////Detect enemy collision with attack
        //Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackHitboxCenter, jabRange, enemyLayers);
        //
        //foreach (Collider2D enemy in hitEnemies)
        //{
        //    //Detect impact angle
        //    var targetclosestPoint = enemy.ClosestPoint(attackHitboxCenter);
        //    var sourceclosestPoint = playerCollider.attachedRigidbody.ClosestPoint(attackHitboxCenter);
        //
        //
        //    var distance = targetclosestPoint - sourceclosestPoint;
        //
        //
        //
        //    var angle = 0.0f;
        //    if (transform.rotation.y == 1)
        //    {
        //        if (distance.x > 0)
        //        {
        //            distance.x = -distance.x;
        //        }
        //        angle = Vector2.Angle(Vector2.right, distance);
        //    }
        //    else
        //    {
        //        if (distance.x < 0)
        //        {
        //            distance.x = -distance.x;
        //        }
        //        angle = Vector2.Angle(Vector2.right, distance);
        //    }
        //
        //    //Impact animation
        //    Instantiate(impactEffect, targetclosestPoint, transform.rotation);
        //    //Apply damage
        //    enemy.GetComponentInParent<PlatformerCharacter2D>().TakeDamage(jabDamage);
        //    //Apply Knockback
        //    enemy.GetComponentInParent<PlatformerCharacter2D>().Knockback(jabKB, angle, 0, 2f);
        //}
    }

    public void Move()
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

        // -- Handle input and movement --
        if (m_disableMovementTimer < 0.0f)
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_runSpeed * Time.deltaTime);

            m_moving = true;
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
        //if (Input.GetKeyDown("e") && !m_dodging)
        //{
        //    m_animator.SetBool("noBlood", m_noBlood);
        //    m_animator.SetTrigger("Death");
        //    m_respawnTimer = 2.5f;
        //    DisableWallSensors();
        //    m_dead = true;
        //}
        //
        ////Hurt
        //else if (Input.GetKeyDown("q") && !m_dodging)
        //{
        //    m_animator.SetTrigger("Hurt");
        //    // Disable movement 
        //    m_disableMovementTimer = 0.1f;
        //    DisableWallSensors();
        //}
        //
        //// Parry & parry stance
        //else if (Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded)
        //{
        //    // Parry
        //    // Used when you are in parry stance and something hits you
        //    if (m_parryTimer > 0.0f)
        //    {
        //        m_animator.SetTrigger("Parry");
        //        m_body2d.velocity = new Vector2(-m_facingDirection * m_parryKnockbackForce, m_body2d.velocity.y);
        //    }
        //
        //    // Parry Stance
        //    // Ready to parry in case something hits you
        //    else
        //    {
        //        m_animator.SetTrigger("ParryStance");
        //        m_parryTimer = 7.0f / 12.0f;
        //    }
        //}
        //
        ////Up Attack
        //else if (Input.GetMouseButtonDown(0) && Input.GetKey("w") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.2f)
        //{
        //    m_animator.SetTrigger("UpAttack");
        //
        //    // Reset timer
        //    m_timeSinceAttack = 0.0f;
        //
        //    // Disable movement 
        //    m_disableMovementTimer = 0.35f;
        //}

        //Attack
        
        if (attackDistance >= distance && inRange && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded && m_timeSinceAttack > 0.5f)
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
            m_animator.SetTrigger("Attack" + m_currentAttack);

            Attack("jab", jabPoint);

            // Disable movement 
            m_disableMovementTimer = 0.35f;
        }

        //Air Slam Attack
        //else if (Input.GetMouseButtonDown(0) && Input.GetKey("s") && !m_ledgeGrab && !m_ledgeClimb && !m_grounded)
        //{
        //    m_animator.SetTrigger("AttackAirSlam");
        //    m_body2d.velocity = new Vector2(0.0f, -m_jumpForce);
        //    m_disableMovementTimer = 0.8f;
        //
        //    // Reset timer
        //    m_timeSinceAttack = 0.0f;
        //}
        //
        //// Air Attack Up
        //else if (Input.GetMouseButtonDown(0) && Input.GetKey("w") && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.2f)
        //{
        //    Debug.Log("Air attack up");
        //    m_animator.SetTrigger("AirAttackUp");
        //
        //    // Reset timer
        //    m_timeSinceAttack = 0.0f;
        //}
        //
        //// Air Attack
        //else if (Input.GetMouseButtonDown(0) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && !m_grounded && m_timeSinceAttack > 0.2f)
        //{
        //    m_animator.SetTrigger("AirAttack");
        //
        //    // Reset timer
        //    m_timeSinceAttack = 0.0f;
        //}
        //
        //// Dodge
        //else if (Input.GetKeyDown("left shift") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb)
        //{
        //    m_dodging = true;
        //    m_crouching = false;
        //    m_animator.SetBool("Crouching", false);
        //    m_animator.SetTrigger("Dodge");
        //    m_body2d.velocity = new Vector2(m_facingDirection * m_dodgeForce, m_body2d.velocity.y);
        //}

        // Throw
        //else if (Input.GetKeyDown("f") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb)
        //{
        //    m_animator.SetTrigger("Throw");
        //
        //    // Disable movement 
        //    m_disableMovementTimer = 0.20f;
        //}
        //
        //// Ledge Climb
        //else if (Input.GetKeyDown("w") && m_ledgeGrab)
        //{
        //    DisableWallSensors();
        //    m_ledgeClimb = true;
        //    m_body2d.gravityScale = 0;
        //    m_disableMovementTimer = 6.0f / 14.0f;
        //    m_animator.SetTrigger("LedgeClimb");
        //}
        //
        //// Ledge Drop
        //else if (Input.GetKeyDown("s") && m_ledgeGrab)
        //{
        //    DisableWallSensors();
        //}

        ////Jump
        //else if (Input.GetButtonDown("Jump") && (m_grounded || m_wallSlide) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_disableMovementTimer < 0.0f)
        //{
        //    // Check if it's a normal jump or a wall jump
        //    if (!m_wallSlide)
        //        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        //    else
        //    {
        //        m_body2d.velocity = new Vector2(-m_facingDirection * m_jumpForce / 2.0f, m_jumpForce);
        //        m_facingDirection = -m_facingDirection;
        //        m_SR.flipX = !m_SR.flipX;
        //    }
        //
        //    m_animator.SetTrigger("Jump");
        //    m_grounded = false;
        //    m_animator.SetBool("Grounded", m_grounded);
        //    m_groundSensor.Disable(0.2f);
        //}
        //
        ////Crouch / Stand up
        //else if (Input.GetKeyDown("s") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_parryTimer < 0.0f)
        //{
        //    m_crouching = true;
        //    m_animator.SetBool("Crouching", true);
        //    m_body2d.velocity = new Vector2(m_body2d.velocity.x / 2.0f, m_body2d.velocity.y);
        //}
        //else if (Input.GetKeyUp("s") && m_crouching)
        //{
        //    m_crouching = false;
        //    m_animator.SetBool("Crouching", false);
        //}
        ////Walk
        //else if (m_moving && Input.GetKey(KeyCode.LeftControl))
        //{
        //    m_animator.SetInteger("AnimState", 2);
        //    m_maxSpeed = m_walkSpeed;
        //}

        //Run
        else if (m_moving)
        {
            m_animator.SetInteger("AnimState", 1);
            m_maxSpeed = m_runSpeed;
        }

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
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
        }
        if (atPortal == false)
        {
            //infoText.text = "";
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
                }
                else if (enemy.GetComponentInParent<PrototypeHero>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<PrototypeHero>().m_facingDirection == -1 && (attackAngle < 90 || attackAngle > 315))
                {
                    enemy.GetComponentInParent<PrototypeHero>().Block(jabDamage);
                }
                else
                {
                    //Apply damage
                    enemy.GetComponentInParent<PrototypeHero>().TakeDamage(jabDamage);
                    //Apply Knockback
                    enemy.GetComponentInParent<PrototypeHero>().Knockback(jabKB, attackAngle, 0, 2);
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
                    enemy.GetComponentInParent<TheChampion>().Knockback(jabKB, attackAngle, 0, 2);
                }


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

    void RespawnHero()
    {
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
        transform.position = new Vector2(81, 3);
        m_dead = false;
        m_animator.Rebind();
    }

    public void TakeDamage(float damage)
    {
        currentDamage += damage;

        //Play hurt animation
    }

    public void Knockback(float BaseKB, float contactAngle, float modifierx, float modifiery, bool fromAbove)
    {
        m_animator.SetTrigger("Hurt");
        m_disableMovementTimer = 0.1f;

        //Make a vector inverse of collision angle
        float radians = contactAngle * Mathf.Deg2Rad;
        Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        BaseKB *= (currentDamage * 0.75f);
        //if (fromAbove)
        //{
        //    KBVector.y = -KBVector.y;
        //}
        //
        //if (KBVector.x < 0)
        //{
        //    modifierx = -modifierx;
        //}
        //if (KBVector.y < 0)
        //{
        //    modifiery = -modifiery;
        //}
        ////Calculate knockback force
        //KBVector.x = KBVector.x * BaseKB * (currentDamage * 0.75f) + modifierx;
        //KBVector.y = KBVector.y * BaseKB * (currentDamage * 0.75f) + modifiery;

        Vector2 KBForce = KBVector * BaseKB;
        KBForce.x += modifierx;
        KBForce.y += modifiery;

        var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        e_Rigidbody2D.AddForce(KBForce, ForceMode2D.Impulse);
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
