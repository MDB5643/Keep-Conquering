using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinionBehavior : MonoBehaviour
{

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
    private string CurrentJumpTarget = "";
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

    public bool m_grounded = false;
    private bool m_moving = false;
    private bool m_dead = false;
    private bool m_dodging = false;
    private bool m_wallSlide = false;
    private bool m_ledgeGrab = false;
    private bool m_ledgeClimb = false;
    private bool m_crouching = false;
    public bool m_inHitStun = false;
    public bool m_launched = false;
    public bool m_idle = false;

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

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

        firstLauncherTarget = GameObject.Find("RedMinionLauncherTarget");
        MovingPlatformTarget = GameObject.Find("RedMinionPlatformTarget");
        GondolaTarget = GameObject.Find("RedMinionGondolaTarget");
        EyeTarget = GameObject.Find("RedMinionEyeTarget");

        target = firstLauncherTarget.transform;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (EyeTarget == null && KeepEyeTarget != null)
        {
            EyeTarget = KeepEyeTarget;
        }

        if (transform.position.x < TowerPlatformTarget.transform.position.x && EyeTarget != null)
        {
            if (EyeTarget.name != "MinionKeepEyeTarget")
            {
                target = EyeTarget.transform;
            }
            else if (transform.position.x < -37 && KeepEyeTarget != null)
            {
                target = EyeTarget.transform;
            }
        }

        if (target == null)
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

        if (currentDamage >= 15)
        {
            currentDamage = 0.0f;
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            m_respawnTimer = 1f;
            DisableWallSensors();
            m_dead = true;
        }

        //m_DamageDisplay.text = currentDamage + "%";

        //if (!InsideofLimits() && !inRange && m_timeSinceAttack > 0.2f)
        //{
        //    SelectTarget();
        //}

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
        if (!m_inHitStun && !m_dead)
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
            if (m_disableMovementTimer < 0.0f && !m_launched && !m_idle && m_grounded)
            {
                Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

                if(target.GetComponentInParent<Conqueror>() != null && Mathf.Abs(targetPosition.x - transform.position.x) <= .5f)
                {
                    //spacing while attacking
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_runSpeed * Time.deltaTime);
                }
                

                m_moving = true;
            }
            else
            {
                m_moving = false;
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
                m_animator.SetTrigger("Attack");

                Attack("jab", jabPoint);

                // Disable movement 
                m_disableMovementTimer = 0.35f;
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
            m_timeSinceHitStun += Time.deltaTime;
            if (m_timeSinceHitStun >= m_hitStunDuration)
            {
                Knockback(incomingKnockback, incomingAngle, incomingXMod, incomingYMod);
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
        if (coll.gameObject.name == "MovingPlatformRight" && coll.gameObject.transform.position.y > transform.position.y)
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
            }
            if (coll.gameObject.CompareTag("MinionJumpLeft"))
            {
                var e_Rigidbody2D = GetComponent<Rigidbody2D>();
                e_Rigidbody2D.AddForce(new Vector2(-8, 3), ForceMode2D.Impulse);
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

        var playerCollider = GetComponent<CapsuleCollider2D>();

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
                    enemy.GetComponentInParent<PrototypeHero>().incomingAngle = attackAngle;
                    enemy.GetComponentInParent<PrototypeHero>().incomingKnockback = 0;
                    enemy.GetComponentInParent<PrototypeHero>().incomingXMod = 0f;
                    enemy.GetComponentInParent<PrototypeHero>().incomingYMod = 2f;
                    enemy.GetComponentInParent<PrototypeHero>().HitStun(.1f);
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
                    enemy.GetComponentInParent<TheChampion>().incomingKnockback = 0;
                    enemy.GetComponentInParent<TheChampion>().incomingXMod = 0f;
                    enemy.GetComponentInParent<TheChampion>().incomingYMod = 2f;
                    enemy.GetComponentInParent<TheChampion>().HitStun(.1f);
                }


            }
            if (enemy.GetComponent<MinionTowerTarget>() != null)
            {
                enemy.GetComponentInParent<TowerEye>().TakeDamage(jabDamage);
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

    public void FindTargets()
    {

    }

    public void SelectTarget()
    {
        float distanceToLaunch = Vector2.Distance(transform.position, firstLauncherTarget.transform.position);
        float distanceToPlatform = Vector2.Distance(transform.position, MovingPlatformTarget.transform.position);
        float distanceToGondola = Vector2.Distance(transform.position, GondolaTarget.transform.position);
        if (EyeTarget != null)
        {
            float distanceToEye = Vector2.Distance(transform.position, EyeTarget.transform.position);
        }

        if (transform.position.x < firstLauncherTarget.transform.position.x && transform.position.x > MovingPlatformTarget.transform.position.x && target.name != "LeftEdge")
        {
            target = MovingPlatformTarget.transform;
        }
        if (transform.position.y >= 0f && transform.position.x < MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && CurrentJumpTarget != "EnemyPlatform" && CurrentJumpTarget != "EnemyKeep")
        {
            m_idle = false;
            target = GondolaTarget.transform;
        }
        if (CurrentJumpTarget == "EnemyPlatform")
        {
            target = GondolaEdgeTarget.transform;
        }

        Flip();
    }

    public void Flip()
    {
        if (target == null)
        {
            target = TowerPlatformTarget.transform;
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
        BaseKB *= (currentDamage * 0.75f);

        //Calculate knockback force
        Vector2 KBForce = KBVector * BaseKB;
        KBForce.x += modifierx;
        KBForce.y += modifiery;

        var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
        e_Rigidbody2D.AddForce(KBForce, ForceMode2D.Impulse);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "MinionBoardGondola" && m_grounded && (target.name == "RedMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "LeftEdge") && !m_launched)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 17f), ForceMode2D.Impulse);
            m_launched = true;
            target = GondolaEdgeTarget.transform;
            CurrentJumpTarget = "EnemyPlatform";
        }
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
        else if (collision.transform.tag == "AttackHitbox" && !m_dead)
        {
            collision.GetComponentInParent<CombatManager>().Hit(transform, collision.transform.name);
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
        if (collision.transform.tag == "MinionBoardRight" && m_grounded && target.name != "RedMinionPlatformTarget")
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
        if (collision.transform.tag == "MinionBoardLeft" && m_grounded && target.name != "RedMinionPlatformTarget")
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
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-2f, 1f), ForceMode2D.Impulse);
            //SelectTarget();
            target = GondolaTarget.transform;
            CurrentJumpTarget = "Gondola";
        }
        if (collision.transform.tag == "MinionJumpLeft")
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-6f, 5), ForceMode2D.Impulse);
            CurrentJumpTarget = "VerticalPlatform";
            SelectTarget();
        }
        else if (collision.transform.tag == "MinionJumpUp" && !m_idle && m_grounded && CurrentJumpTarget == "VerticalPlatform")
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
            m_launched = true;
            CurrentJumpTarget = "MainPlatform";
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
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-3f, 4), ForceMode2D.Impulse);
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
    }

}
