using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CPUBehavior : MonoBehaviour
{
    private GrabableLedge ledge;
    private Rect buttonPos1;
    private Rect buttonPos2;
    public float attackDistance = 2.5f;
    public float minDamage = 0.0f;
    public float smashAttackInterval = 15f;
    public float timeSinceSmashAttack = 0.0f;
    public float reactionTime = .3f;
    public float m_timeSinceDecision = .3f;

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
    HookBehavior hookShot;

    public float m_TimeSinceJab2 = 0.0f;

    public string routine = "Lane";

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponentInParent<Conqueror>().isPlayer = false;
        gameObject.GetComponentInParent<Conqueror>().teamColor = "Red";

        hotZone.SetActive(true);
        triggerArea.SetActive(true);

        if (gameObject.GetComponentInParent<Conqueror>().teamColor == "Red")
        {
            firstLauncherTarget = GameObject.Find("RedMinionLauncherTarget");
            MovingPlatformTarget = GameObject.Find("RedMinionPlatformTarget");
            GondolaTarget = GameObject.Find("RedMinionGondolaTarget");
            EyeTarget = GameObject.Find("RedMinionEyeTarget");
            TowerPlatformTarget = GameObject.Find("Jumppoint5");
        }
        if (leftLimit == null)
        {
            leftLimit = GameObject.Find("CPULimitL").transform;
        }
        if (rightLimit == null)
        {
            rightLimit = GameObject.Find("CPULimitR").transform;
        }

        if (MenuEvents.gameModeSelect == 2)
        {
            routine = "HoldGround";
        }

        if (routine == "HoldGround")
        {
            target = leftLimit;
        }
        //

        SelectTarget();
    }


    private void CheckOverlaps()
    {
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useLayerMask = true;
        contactFilter2D.SetLayerMask(gameObject.GetComponentInParent<Conqueror>().m_WhatIsPortal);

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
                    gameObject.GetComponentInParent<Conqueror>().SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                    //gameObject.GetComponentInParent<Conqueror>().m_audioManager.PlaySound("Teleport");
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
                    gameObject.GetComponentInParent<Conqueror>().SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                    //gameObject.GetComponentInParent<Conqueror>().m_audioManager.PlaySound("Teleport");
                }
            }
        }
        if (atPortal == false)
        {
        }
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }


    public void Move()
    {
        if (!gameObject.GetComponentInParent<Conqueror>().preview)
        {
            if (leftLimit == null)
            {
                leftLimit = GameObject.Find("CPULimitL").transform;
            }
            if (rightLimit == null)
            {
                rightLimit = GameObject.Find("CPULimitR").transform;
            }
            if (EyeTarget == null && KeepEyeTarget != null)
            {
                EyeTarget = KeepEyeTarget;
            }
            if (TowerPlatformTarget != null)
            {
                if (gameObject.GetComponentInParent<Conqueror>().teamColor == "Red" && routine == "Lane" && transform.position.x < TowerPlatformTarget.transform.position.x && EyeTarget != null)
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
            }

            if (gameObject.GetComponentInParent<Conqueror>().teamColor == "Blue" && routine == "Lane" && transform.position.x > TowerPlatformTarget.transform.position.x && EyeTarget != null)
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
                if (TowerPlatformTarget != null)
                {
                    target = TowerPlatformTarget.transform;
                }
                CurrentJumpTarget = "Gondola";
            }

            if (!InsideofLimits() && !inRange && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > 0.2f)
            {
                SelectTarget();
            }

            // Increase timer that controls attack combo
            m_timeSinceDecision += Time.deltaTime;
            m_TimeSinceJab2 += Time.deltaTime;
            
            //Sense if ground is anywhere below them
            Vector3 rayStart = gameObject.GetComponentInParent<Conqueror>().m_groundSensor.transform.position;

            var hit = Physics2D.Raycast(rayStart, Vector2.down, 4.0f);

            // -- Handle Animations --
            //Attack
            if (m_timeSinceDecision > reactionTime)
            {

                if (gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && m_timeSinceDecision > 1.0f)
                {
                    gameObject.GetComponentInParent<Conqueror>().DisableWallSensors();
                    gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb = true;
                    gameObject.GetComponentInParent<Conqueror>().m_body2d.gravityScale = 0;
                    gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 6.0f / 14.0f;
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("LedgeClimb");

                    if (transform.gameObject.layer == 27)
                    {
                        gameObject.GetComponentInParent<Conqueror>().SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                    }
                    else if (transform.gameObject.layer == 26)
                    {
                        gameObject.GetComponentInParent<Conqueror>().SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                    }
                }

                else if (gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == true && !gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging && gameObject.GetComponentInParent<Conqueror>().m_fullCharge && gameObject.GetComponentInParent<Conqueror>().m_grounded)
                {
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("USmash");
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("USmashCharge", false);
                    // Reset timer
                    gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
                    timeSinceSmashAttack = 0.0f;

                    gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging = false;
                    gameObject.GetComponentInParent<Conqueror>().m_fullCharge = false;

                    gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
                    m_timeSinceDecision = 0.0f;
                }
                else if (gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == true && !gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging && gameObject.GetComponentInParent<Conqueror>().m_fullCharge && gameObject.GetComponentInParent<Conqueror>().m_grounded)
                {
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("FSmash");
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("FSmashCharge", false);
                    // Reset timer
                    gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
                    timeSinceSmashAttack = 0.0f;

                    gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging = false;
                    gameObject.GetComponentInParent<Conqueror>().m_fullCharge = false;

                    gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
                    m_timeSinceDecision = 0.0f;
                }

                else if (inRange)
                {
                    if (gameObject.GetComponentInParent<Conqueror>().m_grounded)
                    {
                        GroundCombat();
                    }
                    else
                    {
                        //AirCombat();

                    }
                }



                //jump to recover
                if (transform.position.y < -2 && gameObject.GetComponentInParent<Conqueror>().m_jumpCount < 2 && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > 0.3f && gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial > 2.0f
                        && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false && MenuEvents.gameModeSelect != 4)
                {

                    if (transform.position.x > rightLimit.position.x)
                    {
                        gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(-1, gameObject.GetComponentInParent<Conqueror>().m_jumpForce);
                        gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Jump");
                        gameObject.GetComponentInParent<Conqueror>().m_grounded = false;
                        gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Grounded", gameObject.GetComponentInParent<Conqueror>().m_grounded);
                        gameObject.GetComponentInParent<Conqueror>().m_groundSensor.Disable(0.2f);
                        gameObject.GetComponentInParent<Conqueror>().m_jumpCount++;
                    }
                    else if (transform.position.x < leftLimit.position.x)
                    {
                        gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(-1, gameObject.GetComponentInParent<Conqueror>().m_jumpForce);
                        gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Jump");
                        gameObject.GetComponentInParent<Conqueror>().m_grounded = false;
                        gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Grounded", gameObject.GetComponentInParent<Conqueror>().m_grounded);
                        gameObject.GetComponentInParent<Conqueror>().m_groundSensor.Disable(0.2f);
                        gameObject.GetComponentInParent<Conqueror>().m_jumpCount++;
                    }


                }

                if (transform.name.Contains("Prototype"))
                {
                    if (transform.position.x > rightLimit.position.x && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > 0.3f && gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial > 2.0f
                        && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false && gameObject.GetComponentInParent<Conqueror>().m_jumpCount >= 1)
                    {
                        gameObject.GetComponentInParent<Conqueror>().hookDirection = "Side";
                        gameObject.GetComponentInParent<PrototypeHero>().ThrowHook();
                        gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Throw");
                        gameObject.GetComponentInParent<Conqueror>().hookActive = true;
                        gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial = 0.0f;
                        gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 1.0f;
                        m_timeSinceDecision = 0.0f;
                    }
                    else if (transform.position.x < leftLimit.position.x && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > 0.3f && gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial > 2.0f
                                    && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false && gameObject.GetComponentInParent<Conqueror>().m_jumpCount >= 1)
                    {
                        gameObject.GetComponentInParent<Conqueror>().hookDirection = "Side";
                        gameObject.GetComponentInParent<PrototypeHero>().ThrowHook();
                        gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Throw");
                        gameObject.GetComponentInParent<Conqueror>().hookActive = true;
                        gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial = 0.0f;
                        m_timeSinceDecision = 0.0f;
                        gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 1.0f;
                    }
                }

                //Run
                if (gameObject.GetComponentInParent<Conqueror>().m_moving && gameObject.GetComponentInParent<Conqueror>().m_grounded)
                {
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetInteger("AnimState", 1);
                    gameObject.GetComponentInParent<Conqueror>().m_maxSpeed = gameObject.GetComponentInParent<Conqueror>().m_runSpeed;
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("StayDown", false);
                }

                //Idle
                else if (gameObject.GetComponentInParent<Conqueror>().m_grounded)
                {
                    gameObject.GetComponentInParent<Conqueror>().m_animator.SetInteger("AnimState", 0);
                }
            }
            float SlowDownSpeed = gameObject.GetComponentInParent<Conqueror>().m_moving ? 1.0f : 0.5f;
            float KBSlowDownSpeed = 0.8f;
            
            if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching
                && (!gameObject.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying") || gameObject.GetComponentInParent<Conqueror>().heavy) && gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer < 0.0f && !gameObject.GetComponentInParent<Conqueror>().m_launched
                && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false
                && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && !gameObject.GetComponentInParent<Conqueror>().preview
                && !gameObject.GetComponentInParent<Conqueror>().m_inHitStun && !gameObject.GetComponentInParent<Conqueror>().m_isInHitStop && !gameObject.GetComponentInParent<Conqueror>().homingIn)
            {
                if (!gameObject.GetComponentInParent<Conqueror>().m_isInKnockback)
                {
                    gameObject.GetComponentInParent<Conqueror>().m_timeSinceKnockBack = 0.0f;
                    if (gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumX <= 1 && gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumY <= 1)
                    {
                        if (gameObject.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying") && gameObject.GetComponentInParent<Conqueror>().heavy)
                        {
                            gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(gameObject.GetComponentInParent<Conqueror>().m_walkSpeed * SlowDownSpeed, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);
                        }
                        else
                        {
                            if (Mathf.Abs(gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.x) >= 6.5)
                            {

                                if (!gameObject.GetComponentInParent<Conqueror>().m_grounded)
                                {
                                    gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.x * .4f, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);
                                }
                                else
                                {
                                    gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.x * .6f, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);
                                }

                            }
                            else if (gameObject.GetComponentInParent<Conqueror>().m_grounded)
                            {
                                //Default groundmovement
                                if (target != null && m_timeSinceDecision > reactionTime)
                                {
                                    Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
                                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, (gameObject.GetComponentInParent<Conqueror>().m_runSpeed * Time.deltaTime)/1.5f);
                                    gameObject.GetComponentInParent<Conqueror>().m_moving = true;
                                }

                            }

                        }

                    }
                    else
                    {
                        gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumX, gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumY);


                        gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumX = gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumX * KBSlowDownSpeed;
                        gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumY = gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumY * KBSlowDownSpeed;
                    }

                }
                else
                {
                    gameObject.GetComponentInParent<Conqueror>().m_timeSinceKnockBack += Time.deltaTime;
                    if (gameObject.GetComponentInParent<Conqueror>().m_timeSinceKnockBack > gameObject.GetComponentInParent<Conqueror>().m_KnockBackDuration)
                    {
                        gameObject.GetComponentInParent<Conqueror>().m_isInKnockback = false;
                        gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumX = gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.x;
                        gameObject.GetComponentInParent<Conqueror>().m_KnockBackMomentumY = gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y;
                        gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Knockback", false);
                    }

                }
            }

        }
    }


    void GroundCombat()
    {
        bool enemyAbove = false;
        bool enemyBelow = false;
        bool enemyBehind = false;

        if (target != null)
        {
            var distance = Vector2.Distance(transform.position, target.position);

            if ((gameObject.GetComponentInParent<Conqueror>().m_wallSensorR2.State() || gameObject.GetComponentInParent<Conqueror>().m_wallSensorL2.State())
                && !gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging && !gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging && !gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging)
            {
                enemyAbove = true;

            }
            else if ((gameObject.GetComponentInParent<Conqueror>().m_wallSensorR0.State() || gameObject.GetComponentInParent<Conqueror>().m_wallSensorL0.State()) && !gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging && !gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging && !gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging)
            {
                enemyBelow = true;
            }
            else if (gameObject.GetComponentInParent<Conqueror>().m_wallSensorL1.State())
            {
                enemyBehind = true;
            }
            if (Mathf.Abs(distance) <= attackDistance)
            {
                Basic(enemyAbove, enemyBehind, enemyBelow);
            }
        }
        
    }
    void AirCombat()
    {
        float rng = Random.Range(1, 10);
        if (gameObject.GetComponentInParent<Conqueror>().m_wallSensorR2.State() || gameObject.GetComponentInParent<Conqueror>().m_wallSensorL2.State() && !gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging && !gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging)
        {

            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("AirAttackUp");
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .65f;
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            m_timeSinceDecision = 0.0f;
        }
        else
        {
            if (rng >= 5.5)
            {
                gameObject.GetComponentInParent<Conqueror>().m_LagTime = .65f;
                gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Nair");
                m_timeSinceDecision = 0.0f;
                gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            }
            else
            {
                gameObject.GetComponentInParent<Conqueror>().m_LagTime = .6f;
                gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("AirAttack");

                gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
                m_timeSinceDecision = 0.0f;
            }

        }
    }
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
            if (gameObject.GetComponentInParent<Conqueror>().teamColor == "Red")
            {
                if (MenuEvents.gameModeSelect == 3)
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
                else
                {
                    if (target == null)
                    {
                        target = MovingPlatformTarget.transform;
                    }
                    else if (transform.position.x < firstLauncherTarget.transform.position.x && transform.position.x > MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge" && routine == "Lane")
                    {
                        target = MovingPlatformTarget.transform;
                    }
                    else if (transform.position.y >= 0f && transform.position.x < MovingPlatformTarget.transform.position.x && target.name != "LeftEdge" && target.name != "RightEdge" && CurrentJumpTarget != "EnemyPlatform" && CurrentJumpTarget != "EnemyKeep" && routine == "Lane")
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
                
            }
            else if (gameObject.GetComponentInParent<Conqueror>().teamColor == "Blue")
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
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                rotation.y = 180f;
                gameObject.GetComponentInParent<Conqueror>().m_facingDirection = -1;
            }
            else
            {
                rotation.y = 0f;
                gameObject.GetComponentInParent<Conqueror>().m_facingDirection = 1;
            }
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
        else if (collision.transform.tag == "AttackHitbox" && !gameObject.GetComponentInParent<Conqueror>().m_dead)
        {
            var targetclosestPoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
            var sourceclosestPoint = new Vector2(transform.position.x, transform.position.y);

            var midPointX = (targetclosestPoint.x + sourceclosestPoint.x) / 2f;
            var midPointY = (targetclosestPoint.y + sourceclosestPoint.y) / 2f;

            if (collision.transform.name.Contains("Smash") || collision.transform.name.Contains("Jab1Hitbox"))
            {
                Instantiate(gameObject.GetComponentInParent<Conqueror>().HeavyAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            }
            else
            {
                Instantiate(gameObject.GetComponentInParent<Conqueror>().LightAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            }
            if (collision.transform.name.Contains("ChargeBall"))
            {
                //GameObject.Destroy(collision.transform.parent.gameObject);
            }
            if (collision.transform.name.Contains("MagicArrow") && collision.GetComponentInParent<ProjectileBehavior>().teamColor != gameObject.GetComponentInParent<Conqueror>().teamColor)
            {
                //GameObject.Destroy(collision.transform.gameObject);
            }
            if (collision.GetComponentInParent<CombatManager>())
            {
                collision.GetComponentInParent<CombatManager>().Hit(transform, collision.GetComponent<CollisionTracker>());
            }
            else if (collision.GetComponent<CombatManager>())
            {
                collision.GetComponent<CombatManager>().Hit(transform, collision.GetComponent<CollisionTracker>());
            }
        }
        if (collision.gameObject.CompareTag("RightLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(27, 13), ForceMode2D.Impulse);
        }
        if (collision.gameObject.CompareTag("LeftLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-27, 13), ForceMode2D.Impulse);
        }
        if (collision.gameObject.CompareTag("LeftTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-7, 21), ForceMode2D.Impulse);
        }
        if (collision.gameObject.CompareTag("RightTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(7, 21), ForceMode2D.Impulse);
        }
        else if (collision.transform.tag == "MinionJumpUp" && gameObject.GetComponentInParent<Conqueror>().m_grounded && CurrentJumpTarget == "VerticalPlatform")
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            CurrentJumpTarget = "MainPlatform";
        }
        if (collision.transform.tag == "MinionJumpToMainPlatform" && target.name != "RedMinionGondolaTarget" && gameObject.GetComponentInParent<Conqueror>().m_grounded && CurrentJumpTarget == "MainPlatform")
        {
            //set velocity to zero to not carry momentum
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-6f, 6f), ForceMode2D.Impulse);
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            //SelectTarget();
            target = GondolaTarget.transform;
            CurrentJumpTarget = "Gondola";
        }
        else if (collision.transform.tag == "MinionJumpToLBottomPlat" && gameObject.GetComponentInParent<Conqueror>().m_grounded && (target.name == "RedMinionGondolaTarget" || target.name == "MinionAfterEyeTarget" || target.name == "LeftEdge") && !gameObject.GetComponentInParent<Conqueror>().m_launched)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-2f, 1f), ForceMode2D.Impulse);
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            target = GondolaEdgeTarget.transform;
            CurrentJumpTarget = "EnemyPlatform";
        }
        else if (collision.transform.tag == "Gondola" && gameObject.GetComponentInParent<Conqueror>().m_grounded && transform.position.y > collision.transform.position.y)
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
        else if (collision.transform.tag == "MinionJumpToEnemyPlat" && gameObject.GetComponentInParent<Conqueror>().m_grounded && CurrentJumpTarget == "EnemyPlatform" && EyeTarget != null)
        {
            rightLimit = GameObject.Find("CPULimitR2").transform;
            leftLimit = GameObject.Find("CPULimitL2").transform;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-7f, 7), ForceMode2D.Impulse);
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            target = EyeTarget.transform;
            CurrentJumpTarget = "None";
        }
        else if (collision.transform.tag == "MinionJumpToEnemyKeep" && gameObject.GetComponentInParent<Conqueror>().m_grounded && KeepEyeTarget != null)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-3f, 4), ForceMode2D.Impulse);
            target = KeepEyeTarget.transform;
            CurrentJumpTarget = "None";
        }
        else if (collision.transform.tag == "MinionJumpSmall" && gameObject.GetComponentInParent<Conqueror>().m_grounded && CurrentJumpTarget == "None" && EyeTarget != null)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-.5f, 0), ForceMode2D.Impulse);
            target = EyeTarget.transform;
            CurrentJumpTarget = "None";
        }
        if (collision.transform.tag == "MinionJumpLeft" && routine == "Lane")
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(-10f, 7), ForceMode2D.Impulse);
            CurrentJumpTarget = "VerticalPlatform";
            SelectTarget();
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
        }
        if (collision.transform.tag == "EyeShot" && collision.GetComponentInParent<TowerEye>().teamColor != gameObject.GetComponentInParent<Conqueror>().teamColor)
        {

            //Detect impact angle
            var targetclosestPoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
            var sourceclosestPoint = new Vector2(transform.position.x, transform.position.y);

            var positionDifference = sourceclosestPoint - targetclosestPoint;

            //Must be done to detect y axis angle
            float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

            // Convert the angle to degrees.
            float attackAngle = angleInRadians * Mathf.Rad2Deg;

            gameObject.GetComponentInParent<Conqueror>().TakeDamage(10f, false);

            gameObject.GetComponentInParent<Conqueror>().incomingAngle = attackAngle;
            gameObject.GetComponentInParent<Conqueror>().incomingKnockback = .8f;
            gameObject.GetComponentInParent<Conqueror>().incomingXMod = 2f;
            gameObject.GetComponentInParent<Conqueror>().incomingYMod = (2f + (gameObject.GetComponentInParent<Conqueror>().currentDamage / 4));
            gameObject.GetComponentInParent<Conqueror>().HitStun(.2f);

            Instantiate(gameObject.GetComponentInParent<Conqueror>().EyeShotFX, new Vector3((targetclosestPoint.x), targetclosestPoint.y, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            GameObject.Destroy(collision.gameObject);
        }
        if (collision.gameObject.name == "targetingHotzone" && !gameObject.GetComponentInParent<Conqueror>().m_isInHotZone && collision.GetComponentInParent<TowerEye>().teamColor != gameObject.GetComponentInParent<Conqueror>().teamColor)
        {
            collision.GetComponentInParent<TowerEye>().enemiesInBounds.Add(transform.gameObject);
            gameObject.GetComponentInParent<Conqueror>().m_isInHotZone = true;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "MinionBoardGondola" && !gameObject.GetComponentInParent<Conqueror>().m_launched)
        {
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(0, 17f), ForceMode2D.Impulse);
            gameObject.GetComponentInParent<Conqueror>().m_launched = true;
            target = GondolaEdgeTarget.transform;
            CurrentJumpTarget = "EnemyPlatform";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "targetingHotzone" && gameObject.GetComponentInParent<Conqueror>().m_isInHotZone && collision.GetComponentInParent<TowerEye>().teamColor != gameObject.GetComponentInParent<Conqueror>().teamColor)
        {
            collision.GetComponentInParent<TowerEye>().enemiesInBounds.Remove(transform.gameObject);
            gameObject.GetComponentInParent<Conqueror>().m_isInHotZone = false;
        }
    }

    void Basic(bool enemyAbove, bool enemyBehind, bool enemyBelow)
    {

        //Up Attack
        if (enemyAbove && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime 
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .3f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("UpAttack");

            // Reset timer
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            m_timeSinceDecision = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
        }

        //Down Tilt Attack
        else if (enemyBelow && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime 
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .35f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("DTiltAttack");

            // Reset timer
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            m_timeSinceDecision = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.45f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Crouching", false);
        }


        //Dash Attack
        else if (Mathf.Abs(gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.x) > 1f && !enemyAbove && !enemyBehind && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_timeSinceNSpec > 1.1f
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false && !gameObject.GetComponentInParent<Conqueror>().isInStartUp)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .45f;
            // Reset timer
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;

            gameObject.GetComponentInParent<Conqueror>().m_currentAttack = 1;

            // Call one of the two attack animations "Attack1" or "Attack2"
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("DashAttack");

            gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(gameObject.GetComponentInParent<Conqueror>().m_facingDirection * gameObject.GetComponentInParent<Conqueror>().m_dodgeForce + gameObject.GetComponentInParent<Conqueror>().m_facingDirection * 3, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);
            m_timeSinceDecision = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.45f;
        }

        //Jab Attack
        else if (gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.x < 1 && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && m_TimeSinceJab2 > 0.45f 
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            if (gameObject.GetComponentInParent<Conqueror>().acrobat)
            {
                gameObject.GetComponentInParent<Conqueror>().m_LagTime = .24f;
                gameObject.GetComponentInParent<Conqueror>().m_currentAttack++;

                if (gameObject.GetComponentInParent<Conqueror>().m_currentAttack == 2)
                {
                    m_TimeSinceJab2 = 0.0f;
                }

                // Loop back to one after second attack
                if (gameObject.GetComponentInParent<Conqueror>().m_currentAttack > 2)
                    gameObject.GetComponentInParent<Conqueror>().m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > .6f)
                    gameObject.GetComponentInParent<Conqueror>().m_currentAttack = 1;
            }
            else
            {
                gameObject.GetComponentInParent<Conqueror>().m_currentAttack = 2;
            }
            
            // Call one of the two attack animations "Attack1" or "Attack2"
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Attack" + gameObject.GetComponentInParent<Conqueror>().m_currentAttack);

            // Reset timer
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.45f;
            m_timeSinceDecision = 0.0f;
        }

        //Air Slam Attack
        else if (!gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_grounded && enemyBelow)
        {
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("AttackAirSlam");
            gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(0.0f, -gameObject.GetComponentInParent<Conqueror>().m_jumpForce);
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.8f;
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            m_timeSinceDecision = 0.0f;
        }

        // Air Attack Up
        else if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && !gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime  && enemyAbove)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .65f;
            Debug.Log("Air attack up");
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("AirAttackUp");
            m_timeSinceDecision = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
        }

        // Air Attack forward
        else if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && !gameObject.GetComponentInParent<Conqueror>().m_grounded &&
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime)
        {
            if (gameObject.GetComponentInParent<Conqueror>().m_facingDirection == 1)
            {
                gameObject.GetComponentInParent<Conqueror>().m_SR.flipX = true;
                gameObject.GetComponentInParent<Conqueror>().m_facingDirection = -1;
            }
            else if (gameObject.GetComponentInParent<Conqueror>().m_facingDirection == -1)
            {
                gameObject.GetComponentInParent<Conqueror>().m_SR.flipX = false;
                gameObject.GetComponentInParent<Conqueror>().m_facingDirection = 1;
            }
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .6f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("AirAttack");
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            m_timeSinceDecision = 0.0f;
        }

        // Air Attack Neutral
        else if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && !gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && enemyBehind)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .65f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Nair");
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            m_timeSinceDecision = 0.0f;
        }

        //Ledge Attack
        else if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().DisableWallSensors();
            gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb = true;
            gameObject.GetComponentInParent<Conqueror>().m_body2d.gravityScale = 0;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 6.0f / 14.0f;
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .3f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("LedgeAttack");
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
            m_timeSinceDecision = 0.0f;
        }
    }

    void Special(bool enemyAbove, bool enemyBehind)
    {


        if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial > 2.0f 
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .45f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Slide");
            gameObject.GetComponentInParent<Conqueror>().m_dodging = true;
            gameObject.GetComponentInParent<Conqueror>().m_crouching = false;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Crouching", false);
            gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(gameObject.GetComponentInParent<Conqueror>().m_facingDirection * 10f, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);
        }
        else if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial > 2.0f 
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .65f;
            gameObject.GetComponentInParent<Conqueror>().hookDirection = "Up";
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("UpThrow");
            gameObject.GetComponentInParent<Conqueror>().hookActive = true;
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial = 0.0f;

            gameObject.GetComponentInParent<PrototypeHero>().ThrowHook();
            // Disable movement 
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = .85f;
        }
        // Throw
        else if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial > 2.0f
            && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            if (gameObject.GetComponentInParent<Conqueror>().m_facingDirection == 1 )
            {
                gameObject.GetComponentInParent<Conqueror>().m_SR.flipX = true;
                gameObject.GetComponentInParent<Conqueror>().m_facingDirection = -1;
            }
            else if (gameObject.GetComponentInParent<Conqueror>().m_facingDirection == -1 )
            {
                gameObject.GetComponentInParent<Conqueror>().m_SR.flipX = false;
                gameObject.GetComponentInParent<Conqueror>().m_facingDirection = 1;
            }
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .65f;
            gameObject.GetComponentInParent<Conqueror>().hookDirection = "Side";
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Throw");
            gameObject.GetComponentInParent<Conqueror>().hookActive = true;
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial = 0.0f;

            gameObject.GetComponentInParent<PrototypeHero>().ThrowHook();
            // Disable movement 
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = .85f;
        }


        else if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_timeSinceSideSpecial > 1.8f
                        && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            // Reset timer

            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .45f;
            gameObject.GetComponentInParent<Conqueror>().m_currentSpecial++;

            // Loop back to one after second attack
            if (gameObject.GetComponentInParent<Conqueror>().m_currentSpecial > 2)
                gameObject.GetComponentInParent<Conqueror>().m_currentSpecial = 1;

            // Reset Attack combo if time since last attack is too large
            if (gameObject.GetComponentInParent<Conqueror>().m_timeSinceNSpec > 1.1f)
                gameObject.GetComponentInParent<Conqueror>().m_currentSpecial = 1;

            // Call one of the two attack animations "Attack1" or "Attack2"
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("SideSpec" + gameObject.GetComponentInParent<Conqueror>().m_currentSpecial);
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceNSpec = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;

            // Disable movement 
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.5f;
        }
    }

    void ForwardSmash(bool enemyAbove, bool enemyBehind)
    {
        //Forward Smash Attack
        if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded
            && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_timeSinceNSpec > 1.1f && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .2f;
            if (gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging != true)
            {
                gameObject.GetComponentInParent<Conqueror>().m_timeSinceChargeStart = 0.0f;
                gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging = true;
            }

            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("FSmashCharge", true);


            // Disable movement 
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
        }

        else if ( gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == true && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .4f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("FSmash");
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("FSmashCharge", false);
            // Reset timer
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;

            gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging = false;
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceChargeStart = 0.0f;
            gameObject.GetComponentInParent<Conqueror>().m_fullCharge = false;

            // Call one of the two attack animations "Attack1" or "Attack2"

            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
        }
    }

    void UpSmash(bool enemyAbove, bool enemyBehind)
    {
        //Up Smash Attack
        if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded
                        && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .25f;
            if (gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging != true)
            {
                gameObject.GetComponentInParent<Conqueror>().m_timeSinceChargeStart = 0.0f;
                gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging = true;
            }

            gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging = true;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("USmashCharge", true);

            // Disable movement 
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
        }

        else if ( gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == true && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime
            && gameObject.GetComponentInParent<Conqueror>().m_timeSinceNSpec > 1.1f)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .4f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("USmash");
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("USmashCharge", false);
            // Reset timer
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;

            gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging = false;
            gameObject.GetComponentInParent<Conqueror>().m_fullCharge = false;

            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
        }
    }

    void DownSmash(bool enemyAbove, bool enemyBehind)
    {
        //Down Smash Attack
        if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded
                        && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_isParrying == false)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .25f;
            if (gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging != true)
            {
                gameObject.GetComponentInParent<Conqueror>().m_timeSinceChargeStart = 0.0f;
                gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging = true;
            }

            gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging = true;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("DSmashCharge", true);

            // Disable movement 
            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
        }

        else if (gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == true && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > gameObject.GetComponentInParent<Conqueror>().m_LagTime
            && gameObject.GetComponentInParent<Conqueror>().m_timeSinceNSpec > 1.1f)
        {
            gameObject.GetComponentInParent<Conqueror>().m_LagTime = .4f;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("DSmash");
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("DSmashCharge", false);
            // Reset timer
            gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack = 0.0f;

            gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging = false;
            gameObject.GetComponentInParent<Conqueror>().m_fullCharge = false;

            gameObject.GetComponentInParent<Conqueror>().m_disableMovementTimer = 0.35f;
        }
    }

    void ReverseForwardSmash(bool enemyAbove, bool enemyBehind)
    {
        throw new System.NotImplementedException();
    }

    void Shield(bool enemyAbove, bool enemyBehind)
    {

        // Parry & parry stance
        if (!gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && !gameObject.GetComponentInParent<Conqueror>().m_crouching && gameObject.GetComponentInParent<Conqueror>().m_grounded && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false)
        {
            // Parry
            // Used when you are in parry stance and something hits you
            //if (gameObject.GetComponentInParent<Conqueror>().m_parryTimer > 0.0f)
            //{
            //    gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Parry");
            //    //gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(-gameObject.GetComponentInParent<Conqueror>().m_facingDirection * gameObject.GetComponentInParent<Conqueror>().m_parryKnockbackForce, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);
            //}

            // Parry Stance
            // Ready to parry in case something hits you
            if (!gameObject.GetComponentInParent<Conqueror>().m_animator.GetCurrentAnimatorStateInfo(0).IsName("ParryStance"))
            {
                gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("ParryStance");
                gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("isParrying", true);
                gameObject.GetComponentInParent<Conqueror>().m_parryTimer = 7.0f / 12.0f;
                gameObject.GetComponentInParent<Conqueror>().m_isParrying = true;
            }

        }

        else if (!gameObject.GetComponentInParent<Conqueror>().m_grounded && !gameObject.GetComponentInParent<Conqueror>().m_dodging && !gameObject.GetComponentInParent<Conqueror>().m_ledgeGrab && !gameObject.GetComponentInParent<Conqueror>().m_ledgeClimb && gameObject.GetComponentInParent<Conqueror>().m_fSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_uSmashCharging == false && gameObject.GetComponentInParent<Conqueror>().m_dSmashCharging == false
            && !gameObject.GetComponentInParent<Conqueror>().m_isInKnockback && !gameObject.GetComponentInParent<Conqueror>().m_inHitStun && !gameObject.GetComponentInParent<Conqueror>().m_isInHitStop)
        {
            gameObject.GetComponentInParent<Conqueror>().m_dodging = true;
            gameObject.GetComponentInParent<Conqueror>().m_crouching = false;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("isParrying", false);
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Crouching", false);
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("AirDodge");
        }
    }

    void Dodge(bool enemyAbove, bool enemyBehind)
    {
        gameObject.GetComponentInParent<Conqueror>().m_isParrying = false;
        if ( gameObject.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying"))
        {
            gameObject.GetComponentInParent<Conqueror>().m_dodging = true;
            gameObject.GetComponentInParent<Conqueror>().m_crouching = false;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("isParrying", false);
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Crouching", false);
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Dodge");
            gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(gameObject.GetComponentInParent<Conqueror>().m_facingDirection * gameObject.GetComponentInParent<Conqueror>().m_dodgeForce, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);
            gameObject.GetComponentInParent<Conqueror>().m_SR.flipX = !gameObject.GetComponentInParent<Conqueror>().m_SR.flipX;
        }

        else if ( gameObject.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying"))
        {
            gameObject.GetComponentInParent<Conqueror>().m_dodging = true;
            gameObject.GetComponentInParent<Conqueror>().m_crouching = false;
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("isParrying", false);
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetBool("Crouching", false);
            gameObject.GetComponentInParent<Conqueror>().m_animator.SetTrigger("Dodge");
            gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(-gameObject.GetComponentInParent<Conqueror>().m_facingDirection * gameObject.GetComponentInParent<Conqueror>().m_dodgeForce, gameObject.GetComponentInParent<Conqueror>().m_body2d.velocity.y);

        }
    }
}
