using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public abstract class Conqueror : MonoBehaviour
{
    public PlayerInput playerInput;
    //public KeepConqueringP1 playerActions;

    public List<Conqueror> grabbedPlayers;
    public List<MinionBehavior> grabbedMinions;

    public string teamColor = "Blue";
    public int playerNumber = 1;
    public bool isPlayer = true;

    public float mana = 100f;
    public float mps = 2f;

    public Slider m_manaBar;
    public Text m_DamageDisplay;
    public Text m_StockDisplay;
    public float currentDamage = 0.0f;
    public float m_StockCount = 3;
    public bool isEliminated;

    public float m_runSpeed = 4.5f;
    public float m_walkSpeed = 2.0f;
    public float m_jumpForce = 7.5f;
    public float m_dodgeForce = 7.0f;
    public float m_parryKnockbackForce = 4.0f;
    public bool m_noBlood = false;
    public bool m_hideSword = false;
    public Text infoText;

    public bool inAttack = false;

    public Animator m_animator;
    public Rigidbody2D m_body2d;
    public SpriteRenderer m_SR;
    public Sensor_Prototype m_groundSensor;
    public Sensor_Prototype m_wallSensorR1;
    public Sensor_Prototype m_wallSensorR0;
    public Sensor_Prototype m_wallSensorR2;
    public Sensor_Prototype m_wallSensorL1;
    public Sensor_Prototype m_wallSensorL2;
    public Sensor_Prototype m_wallSensorL0;

    public Vector2 velocityState;

    public bool heavy = false;
    public bool striker = false;
    public bool caster = false;
    public bool acrobat = false;
    public bool sharpshooter = false;
    public bool support = false;

    public bool m_grounded = false;
    public bool m_fallingdown = false;
    public bool m_down = false;
    public bool m_moving = false;
    public bool m_dead = false;
    public bool m_dodging = false;
    public bool m_wallSlide = false;
    public bool m_ledgeGrab = false;
    public bool m_ledgeClimb = false;
    public bool m_crouching = false;
    public bool m_inHitStun = false;
    public bool m_inGroundSlam = false;
    public bool m_launched = false;
    public bool m_fSmashCharging = false;
    public bool m_uSmashCharging = false;
    public bool m_dSmashCharging = false;
    public bool m_fullCharge = false;
    public bool m_isParrying = false;
    public bool m_isInKnockback = false;
    public bool m_isInHotZone = false;
    public bool m_cameraShake;
    public bool m_inJumpSquat;
    public bool m_freefall = false;

    public bool preview = false;

    public float m_shakeIntensity = .1f;

    public float m_maxSmashChargeTime = 2f;
    public float m_chargeModifier = 0.0f;
    public float m_timeSinceChargeStart = 0.0f;

    public Vector3 m_climbPosition;
    public int m_facingDirection = 1;

    public float m_disableMovementTimer = 0.0f;
    public float m_parryTimer = 0.0f;
    public float m_respawnTimer = 0.0f;

    public Vector3 m_respawnPosition = Vector3.zero;

    public int m_currentAttack = 0;
    public int m_currentSpecial = 0;
    public int m_jumpCount = 0;
    public int upSpecCount = 0;

    public float m_timeSinceAttack = 0.0f;
    public float m_LagTime = 0.0f;
    public float m_timeSinceNSpec = 0.0f;
    public float m_timeSinceSideSpecial = 0.0f;
    public float m_timeSinceStun = 5.0f;
    public float m_timeSinceHitStun = 0.0f;
    public float m_timeSinceTremble = 0.0f;
    public Vector3 preTremblePosition;
    public float m_timeSinceKnockBack = 0.0f;

    public float m_KnockBackDuration = 0.3f;
    public float m_KnockBackMomentumX = 0.0f;
    public float m_KnockBackMomentumY = 0.0f;

    public float m_hitStunDuration = 0.0f;
    public float m_AirSpeed = 2.5f;
    public float m_gravity;
    public float m_maxSpeed = 4.5f;

    public bool m_isInHitStop;
    public bool isInStartUp = false;
    public float m_hitStopDuration;
    public float m_timeSinceHitStop = 0.0f;

    public float animSpeed;

    public float inputX;
    public float inputY;
    public Vector2 inputXY;

    public bool jump = false;
    public bool attack = false;
    public bool special = false;
    public bool shield = false;
    public bool smash = false;
    public bool submit = false;

    public bool homingIn = false;

    //Knockback after hitstun
    public float incomingKnockback = 0.0f;
    public float incomingAngle = 0.0f;
    public float incomingXMod = 0.0f;
    public float incomingYMod = 0.0f;

    //float BaseKB, float contactAngle, float modifierx, float modifiery

    public LayerMask m_WhatIsPortal;
    public GrabableLedge ledge;

    //Combat
    public Transform jabPoint;
    public Transform upTiltPoint;
    public Transform backPoint;
    public Transform downPoint;
    
    public LayerMask enemyLayers;

    public GameObject KnockoutFX;
    public GameObject LightAttackFX;
    public GameObject baseAttackFX;
    public GameObject baseAttackFX2;
    public GameObject HeavyAttackFX;
    public GameObject ChargeFlashFX;
    public GameObject BlockFX;
    public GameObject EyeShotFX;

    public ShieldBar shieldBar;
    public HookBehavior hookPrefab;
    public LineController hookChain;
    public Transform launchOffset;
    public SpriteShapeDemo m_SpriteShaper;
    
    public string hookDirection = "";
    public bool hookActive = false;

    public bool isGrappled;

    public float jabRange;
    public float jabDamage = 3.0f;
    public float jabKB = .5f;

    public float upTiltRange = 2f;
    public float upTiltDamage = 3.0f;
    public float upTiltKB = 1.2f;

    public float downAirRange = 3f;
    public float downAirDamage = 5.0f;
    public float downAirKB = 1.4f;

    public bool FSmashInput = false;
    public bool ReverseFSmashInput = false;
    public bool DSmashInput = false;
    public bool USmashInput = false;

    public CapsuleCollider2D NormalCollider;
    public CapsuleCollider2D ProneCollider;
    public CapsuleCollider2D CrouchCollider;
    public CapsuleCollider2D KnockbackCollider;

    public GameObject RepelBoxNormal;
    public GameObject RepelBoxProne;
    public GameObject RepelBoxCrouch;

    public AudioManager_PrototypeHero m_audioManager;

    Vector2 _movement;
    Rigidbody2D _rb;

    public CPUBehavior CPUBrain;

    private InputAction jumpAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Use this for initialization
    void Start()
    {
        
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        if (!m_body2d)
        {
            m_body2d = GetComponentInChildren<Rigidbody2D>();
        }
        m_SR = GetComponentInChildren<SpriteRenderer>();
        m_gravity = m_body2d.gravityScale;
        m_SpriteShaper = GetComponentInChildren<SpriteShapeDemo>();
        m_cameraShake = false;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorR0 = transform.Find("WallSensor_R0").GetComponent<Sensor_Prototype>();
        m_wallSensorL0 = transform.Find("WallSensor_L0").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

        m_audioManager = AudioManager_PrototypeHero.instance;

        
    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    //Fixed update runs 50 TIMES PER SECOND
    private void FixedUpdate()
    {
        if (m_StockCount == 0)
        {
            isEliminated = true;
        }
        if (caster || support)
        {
            if (mana < 100)
            {
                mana += Time.deltaTime * mps;
            }
            if (m_manaBar)
            {
                m_manaBar.value = mana;
            }
        }
        if (m_disableMovementTimer < 0.0f && !preview && isPlayer)
        {

            // -- Handle input and movement --

            // GetAxisRaw returns either -1, 0 or 1
            float inputRaw = Input.GetAxisRaw("Horizontal");

            // check if character is currently moving
            if (Mathf.Abs(inputXY.x) > 0 && Mathf.Sign(inputXY.x) == m_facingDirection)
            {
                m_moving = true;
                m_animator.SetBool("StayDown", false);
            }

            else
                m_moving = false;


            // Swap direction of sprite depending on move direction
            if (inputXY.x > 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb && m_grounded && m_fSmashCharging == false
                && m_uSmashCharging == false && m_dSmashCharging == false && !preview && !m_inHitStun && !m_isInHitStop && !inAttack)
            {
                if (m_SR.flipX == true)
                {
                    launchOffset.position = new Vector3(transform.position.x + .6f, transform.position.y + .13f, transform.position.z + 0.0f);
                }
                m_SR.flipX = false;
                m_facingDirection = 1;
            }

            else if (inputXY.x < 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb && m_grounded && m_fSmashCharging == false
                && m_uSmashCharging == false && m_dSmashCharging == false && !preview && !m_inHitStun && !m_isInHitStop && !inAttack)
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
            float KBSlowDownSpeed = 0.82f;
            // Set movement
            if (!m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && (!m_animator.GetBool("isParrying") || heavy) && m_disableMovementTimer < 0.0f && !m_launched && m_fSmashCharging == false
                && m_uSmashCharging == false && m_dSmashCharging == false && !preview && !m_inHitStun && !m_isInHitStop && !homingIn && !inAttack)
            {
                if (!m_isInKnockback)
                {
                    m_timeSinceKnockBack = 0.0f;
                    if (m_KnockBackMomentumX <= 1 && m_KnockBackMomentumY <= 1)
                    {
                        if (m_animator.GetBool("isParrying") && heavy)
                        {
                            m_body2d.velocity = new Vector2(inputXY.x * m_walkSpeed * SlowDownSpeed, m_body2d.velocity.y);
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
                                
                                // Fast Fall
                                if (inputY < 0 && m_body2d.velocity.y < 0 && m_body2d.velocity.y > -6.5 && !m_grounded && !m_launched && !m_ledgeGrab && !m_ledgeClimb && m_disableMovementTimer < 0.0f && !m_inHitStun && !m_isInHitStop && !m_isInKnockback)
                                {
                                    m_body2d.velocity = new Vector2(m_body2d.velocity.x * .5f, m_body2d.velocity.y * 1.2f);
                                }
                                else
                                {
                                    //Default ground/air movement
                                    m_body2d.velocity = new Vector2(inputXY.x * m_runSpeed * SlowDownSpeed, m_body2d.velocity.y);
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
                        m_isInKnockback = false;
                        m_KnockBackMomentumX = m_body2d.velocity.x;
                        m_KnockBackMomentumY = m_body2d.velocity.y;
                        m_animator.SetBool("Knockback", false);
                    }

                }
            }
            // Ledge Climb
            else if (inputY > 0 && m_ledgeGrab)
            {
                DisableWallSensors();
                m_ledgeClimb = true;
                m_body2d.gravityScale = 0;
                m_disableMovementTimer = 6.0f / 14.0f;
                m_animator.SetTrigger("LedgeClimb");

                if (transform.gameObject.layer == 27)
                {
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                }
                else if (transform.gameObject.layer == 26)
                {
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                }
            }

            // Ledge Drop
            else if (inputY < 0 && m_ledgeGrab)
            {
                DisableWallSensors();
                if (transform.gameObject.layer == 27)
                {
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                }
                else if (transform.gameObject.layer == 26)
                {
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                }
            }

            // Fast Fall
            else if (inputY < 0 && m_body2d.velocity.y < 0 && !m_grounded && !m_launched && !m_ledgeGrab && !m_ledgeClimb && m_disableMovementTimer < 0.0f && !m_inHitStun && !m_isInHitStop && !m_isInKnockback)
            {
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_body2d.velocity.y * 1.2f);
            }

            //Crouch / Stand up
            if (inputY < 0 && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_disableMovementTimer < 0.0f && !m_inHitStun && !m_isInHitStop && !inAttack && !m_isInKnockback
                && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
            {
                m_crouching = true;
                m_animator.SetBool("Crouching", true);
                m_body2d.velocity = new Vector2(m_body2d.velocity.x / 2.0f, m_body2d.velocity.y);
                //NormalCollider.enabled = false;
                RepelBoxNormal.SetActive(false);
                RepelBoxCrouch.SetActive(true);
            }
            else if (inputY >= 0 && m_crouching && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_disableMovementTimer < 0.0f && !m_inHitStun && !m_isInHitStop && !inAttack && !m_isInKnockback
                && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false)
            {
                m_crouching = false;
                m_animator.SetBool("Crouching", false);
                RepelBoxCrouch.SetActive(false);
            }
            if (!m_crouching )
            {
                //NormalCollider.enabled = true;
                RepelBoxNormal.SetActive(true);
                RepelBoxProne.SetActive(false);
                RepelBoxCrouch.SetActive(false);
            }
        }
        else if (!isPlayer)
        {

        }
    }

    public void Interrupt()
    {
        upSpecCount = 0;
        m_ledgeGrab = false;
        m_ledgeClimb = false;
        m_inGroundSlam = false;
        m_inJumpSquat = false;
        m_LagTime += .01f;
        isInStartUp = false;
        m_freefall = false;
        inAttack = false;
        m_body2d.bodyType = RigidbodyType2D.Dynamic;
        RepelBoxProne.SetActive(false);
        RepelBoxCrouch.SetActive(false);
        RepelBoxNormal.SetActive(true);
        foreach (var grabbedEnemy in grabbedPlayers)
        {
            if (grabbedEnemy.transform.tag == "PlayerMid")
            {
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
            }
            else if (grabbedEnemy.transform.tag == "Player")
            {
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
            }
        }
        grabbedPlayers.Clear();
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "RepelBox"  && !m_inGroundSlam && GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            Rigidbody2D thisBody = GetComponent<Rigidbody2D>();
            m_groundSensor.Disable(.1f);
            
            Rigidbody2D thatBody = coll.GetComponentInParent<Rigidbody2D>();
            var rel = new Vector2(thisBody.position.x, 0) - new Vector2(thatBody.position.x, 0);

            rel.Normalize();
            thisBody.AddForce(rel * 250f);
        }
        if (coll.gameObject.tag == "WindBox")
        {
            Rigidbody2D thisBody = GetComponent<Rigidbody2D>();

            Rigidbody2D thatBody = coll.GetComponentInParent<Conqueror>().m_body2d;
            var rel = new Vector2(thisBody.position.x, 0) - new Vector2(thatBody.position.x, 0);

            rel.Normalize();
            thisBody.AddForce(rel * 50f);
        }
    }



    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "RepelBox" && !m_inGroundSlam && GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            Rigidbody2D thisBody = GetComponent<Rigidbody2D>();
            velocityState = thisBody.velocity;
            //m_groundSensor.Disable(.1f);
        }
        if (coll.gameObject.tag == "RepelBox" && !m_inGroundSlam)
        {
            Rigidbody2D thisBody = GetComponent<Rigidbody2D>();
            velocityState = thisBody.velocity;
        }
        if (coll.gameObject.CompareTag("RightLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(27, 13), ForceMode2D.Impulse);
        }
        if (coll.gameObject.CompareTag("LeftLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-27, 13), ForceMode2D.Impulse);
        }
        if (coll.gameObject.CompareTag("LeftTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(-7, 21), ForceMode2D.Impulse);
        }
        if (coll.gameObject.CompareTag("RightTowerLauncher"))
        {
            var e_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_launched = true;
            m_disableMovementTimer = 0.1f;
            transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
            e_Rigidbody2D.AddForce(new Vector2(7, 21), ForceMode2D.Impulse);
        }
        if (coll.transform.tag == "AttackHitbox")
        {
            string incomingAttackSource;
            if (coll.GetComponentInParent<MinionBehavior>())
            {
                incomingAttackSource = coll.GetComponentInParent<MinionBehavior>().teamColor;
            }
            else if (coll.GetComponentInParent<ProjectileBehavior>())
            {
                incomingAttackSource = coll.GetComponentInParent<ProjectileBehavior>().teamColor;
            }
            else
            {
                incomingAttackSource = coll.GetComponentInParent<Conqueror>().teamColor;
            }
            if (incomingAttackSource != teamColor)
            {
                Interrupt();
                float rng = Random.Range(1, 10);
                preTremblePosition = transform.localPosition;
                if (transform.GetComponent<PrototypeHero>())
                {
                    GameObject.Destroy(transform.GetComponentInChildren<PrototypeHeroAnimEvents>().activeHitbox);
                }
                var targetclosestPoint = new Vector2(coll.transform.position.x, coll.transform.position.y);
                var sourceclosestPoint = new Vector2(transform.position.x, transform.position.y);

                var midPointX = (targetclosestPoint.x + sourceclosestPoint.x) / 2f;
                var midPointY = (targetclosestPoint.y + sourceclosestPoint.y) / 2f;

                if (coll.transform.name.Contains("Smash") || coll.transform.name.Contains("Jab1Hitbox"))
                {
                    Instantiate(HeavyAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    if (rng > 5.5)
                    {
                        Instantiate(baseAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    }
                    else
                    {
                        Instantiate(baseAttackFX2, new Vector3(midPointX, midPointY, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    }
                    
                }
                else
                {
                    Instantiate(LightAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    if (rng > 5.5)
                    {
                        Instantiate(baseAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    }
                    else
                    {
                        Instantiate(baseAttackFX2, new Vector3(midPointX, midPointY, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);
                    }
                }
                //if (coll.transform.name.Contains("ChargeBall"))
                //{
                //    GameObject.Destroy(coll.transform.parent.gameObject);
                //}
                //if (coll.transform.name.Contains("MagicArrow"))
                //{
                //    GameObject.Destroy(coll.transform.gameObject);
                //}
                RepelBoxProne.SetActive(false);
                RepelBoxCrouch.SetActive(false);
                RepelBoxNormal.SetActive(true);
                //NormalCollider.enabled = true;
                coll.GetComponentInParent<CombatManager>().Hit(transform, coll.GetComponent<CollisionTracker>());

            }
            
        }
        if (coll.gameObject.name == "targetingHotzone" && !m_isInHotZone && coll.GetComponentInParent<TowerEye>().teamColor != teamColor)
        {
            coll.GetComponentInParent<TowerEye>().enemiesInBounds.Add(transform.gameObject);
            m_isInHotZone = true;
        }
        if (coll.transform.tag == "EyeShot" && coll.GetComponentInParent<TowerEye>().teamColor != teamColor)
        {

            //Detect impact angle
            var targetclosestPoint = new Vector2(coll.transform.position.x, coll.transform.position.y);
            var sourceclosestPoint = new Vector2(transform.position.x, transform.position.y);

            var positionDifference = sourceclosestPoint - targetclosestPoint;

            //Must be done to detect y axis angle
            float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

            // Convert the angle to degrees.
            float attackAngle = angleInRadians * Mathf.Rad2Deg;

            TakeDamage(10f, false);

            incomingAngle = attackAngle;
            incomingKnockback = .8f;
            incomingXMod = 2f;
            incomingYMod = (2f + (currentDamage / 4));
            HitStun(.2f);

            m_audioManager = AudioManager_PrototypeHero.instance;
            m_audioManager.PlaySound("EnergyHit");
            Instantiate(HeavyAttackFX, new Vector3((coll.transform.position.x), coll.transform.position.y, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            Instantiate(EyeShotFX, new Vector3((targetclosestPoint.x), targetclosestPoint.y, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            GameObject.Destroy(coll.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.name == "targetingHotzone" && m_isInHotZone && coll.GetComponentInParent<TowerEye>().teamColor != teamColor)
        {
            coll.GetComponentInParent<TowerEye>().enemiesInBounds.Remove(transform.gameObject);
            m_isInHotZone = false;
        }
        if (coll.gameObject.tag == "RepelBox" && !m_inGroundSlam && GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            Rigidbody2D thisBody = GetComponent<Rigidbody2D>();
            thisBody.velocity = velocityState;
        }
    }

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
            }

        }
        if ((coll.gameObject.CompareTag("BlastZone") || coll.gameObject.CompareTag("BlastZoneMid")) && !m_dead)
        {
            Interrupt();
            m_dead = true;
            currentDamage = 0.0f;
            m_isInKnockback = false;
            isInStartUp = false;
            isGrappled = false;
            transform.parent = null;
            m_inGroundSlam = false;
            m_isInHitStop = false;
            m_isParrying = false;
            m_animator.SetBool("Knockback", false);
            m_animator.SetTrigger("Death");
            m_respawnTimer = 2.5f;
            DisableWallSensors();
            m_StockCount--;
            gameObject.SetActive(false);
        }
        if (coll.gameObject.CompareTag("Ground") && m_animator.GetBool("Knockback") && transform.position.y >= coll.transform.position.y && transform.GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            m_animator.SetTrigger("FallToProne");
            m_animator.SetBool("StayDown", true);
            m_fallingdown = true;
            m_isInKnockback = false;
            m_animator.SetBool("Knockback", false);
            RepelBoxProne.SetActive(true);
        }
    }

    private void OnCollisionStay2D(Collision2D coll)
    {
        
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
                    transform.position = new Vector3(transform.position.x, transform.position.y + 20, 20);
                    transform.tag = "PlayerMid";
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                    m_audioManager.PlaySound("Teleport");
                }
            }
            //Check for portal
            else if (coll.gameObject.CompareTag("PortalMidground"))
            {
                atPortal = true;
                infoText.text = "Press Tab";
                if (CrossPlatformInputManager.GetButtonDown("Submit"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 25, 0);
                    transform.tag = "Player";
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                    m_audioManager.PlaySound("Teleport");
                }
            }
        }
        if (atPortal == false)
        {
            infoText.text = "";
        }
    }


    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (!(obj.tag == "HotZone") && !(obj.tag == "RepelBox") && !(obj.tag == "HurtBox"))
        {
            obj.layer = newLayer;
        }
            
        foreach (Transform t in obj.transform)
        {
            
            SetLayerRecursively(t.gameObject, newLayer);
        }
    }

    public void OnCollisionExit2D(Collision2D coll)
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

    public void DisableWallSensors()
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
        if (transform.position.z > 10f)
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
        }
        else
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
        }
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

    public void RespawnHero()
    {
        m_StockCount--;
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
        if (playerNumber == 1)
        {
            Transform spawnPoint = GameObject.Find("P1RespawnPoint").transform;
            
            transform.position = spawnPoint.position;
        }
        else if (playerNumber == 2)
        {
            Transform spawnPoint = GameObject.Find("P2RespawnPoint").transform;
            
            transform.position = spawnPoint.position;
        }
        else if (playerNumber == 3)
        {
            Transform spawnPoint = GameObject.Find("P3RespawnPoint").transform;

            transform.position = spawnPoint.position;
        }
        else
        {
            Transform spawnPoint = GameObject.Find("P4RespawnPoint").transform;

            transform.position = spawnPoint.position;
        }

        gameObject.SetActive(true);
        transform.tag = "Player";
        m_dead = false;
        m_launched = false;
        m_animator.Rebind();
        isInStartUp = false;
        ResetDodging();
    }

    public void TakeDamage(float damage, bool noFlinch)
    {
        currentDamage += damage;
        if (!noFlinch)
        {
            m_launched = false;
        }
        
        //Play hurt animation
        m_animator.SetTrigger("Hurt");
    }

    public void HitStun(float stunTime)
    {
        inAttack = false;
        m_inGroundSlam = false;
        m_fSmashCharging = false;
        m_animator.SetBool("FSmashCharge", false);
        m_uSmashCharging = false;
        m_animator.SetBool("USmashCharge", false);
        m_dSmashCharging = false;
        m_animator.SetBool("DSmashCharge", false);
        m_crouching = false;
        m_animator.SetBool("Crouching", false);

        m_animator.SetBool("isParrying", false);
        m_parryTimer = -1.0f;
        m_isParrying = false;
        homingIn = false;

        if (transform.gameObject.layer == 27)
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
        }
        else if (transform.gameObject.layer == 26)
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
        }

        if (!m_inHitStun)
        {
            ResetDodging();
            m_timeSinceHitStun = 0.0f;

            m_disableMovementTimer = stunTime;
            m_hitStunDuration = stunTime;
            if (stunTime > 0)
            {
                m_inHitStun = true;
            }
        }
        //m_animator.SetTrigger("HitStun");

    }

    public void Knockback(float BaseKB, float contactAngle, float modifierx, float modifiery)
    {
        homingIn = false;
        m_animator.SetBool("inJumpSquat", false);
        m_inHitStun = false;
        m_disableMovementTimer = 0.1f;

        //Make a vector inverse of collision angle
        float radians = contactAngle * Mathf.Deg2Rad;
        Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        BaseKB *= (currentDamage * 0.7f);

        if (BaseKB > 2)
        {
            m_animator.SetBool("Knockback", true);
        }

        //Calculate knockback force
        Vector2 KBForce = KBVector * BaseKB;
        KBForce.x += modifierx;
        KBForce.y += modifiery;

        var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
        e_Rigidbody2D.AddForce(KBForce, ForceMode2D.Impulse);
    }

    public void Block(float shieldDamage)
    {
        shieldBar.shieldHealth -= shieldDamage * 0.65f;
        var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        transform.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);
        if (m_facingDirection == 1)
        {
            e_Rigidbody2D.AddForce(new Vector2(-shieldDamage * 0.2f, 0), ForceMode2D.Force);
        }
        if (m_facingDirection == -1)
        {
            e_Rigidbody2D.AddForce(new Vector2(shieldDamage * 0.2f, 0), ForceMode2D.Force);
        }

    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (!preview && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false)
        {
            inputXY = ctx.ReadValue<Vector2>();
        }
    }

    public void MoveX(InputAction.CallbackContext ctx)
    {
        if (!preview && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false)
        {
            inputX = ctx.ReadValue<float>();
        }
    }
    public void MoveY(InputAction.CallbackContext ctx)
    {
        if (!preview && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false)
        {
            inputY = ctx.ReadValue<float>();
        }
    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        //Jump
        if ((m_grounded || m_wallSlide || m_jumpCount <= 1) && !m_dodging && !m_ledgeClimb && !m_crouching && m_disableMovementTimer < 0.0f && !m_isInHitStop && !m_isInKnockback && !m_inHitStun
             && m_fSmashCharging == false && m_uSmashCharging == false && m_dSmashCharging == false && m_isParrying == false && m_launched == false && m_timeSinceAttack > m_LagTime 
             && ctx.phase == InputActionPhase.Started && !m_freefall)
        {
            m_LagTime = .08f;
            m_jumpCount++;

            if (m_ledgeGrab)
            {
                DisableWallSensors();
                m_wallSensorR0.Disable(0.8f);
                m_wallSensorL0.Disable(0.8f);
                if (transform.gameObject.layer == 27)
                {
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
                }
                else if (transform.gameObject.layer == 26)
                {
                    SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
                }
            }

            // Check if it's a normal jump or a wall jump
            if (!m_wallSlide)
            {
                m_animator.SetBool("inJumpSquat", true);
                m_animator.SetTrigger("Jump");
            }
                
            
            else
            {
                m_body2d.velocity = new Vector2(-m_facingDirection * m_jumpForce / 2.0f, m_jumpForce);
                m_facingDirection = -m_facingDirection;
                m_SR.flipX = !m_SR.flipX;
                m_animator.SetBool("inJumpSquat", true);
                m_animator.SetTrigger("Jump");
            }

            
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_groundSensor.Disable(0.25f);
        }
    }

    protected abstract void BasicAction(InputAction.CallbackContext ctx);
    protected abstract void SpecialAction(InputAction.CallbackContext ctx);

    protected abstract void ForwardSmashAction(InputAction.CallbackContext ctx);

    protected abstract void UpSmashAction(InputAction.CallbackContext ctx);

    protected abstract void DownSmashAction(InputAction.CallbackContext ctx);

    protected abstract void ReverseForwardSmashAction(InputAction.CallbackContext ctx);

    protected abstract void ShieldAction(InputAction.CallbackContext ctx);

    protected abstract void DodgeAction(InputAction.CallbackContext ctx);

    public void Basic(InputAction.CallbackContext ctx)
    {
        if (!m_isInHitStop && !m_isInKnockback && !m_inHitStun && !inAttack && !m_freefall && !m_animator.GetBool("inJumpSquat"))
        {
            m_animator.SetBool("Crouching", false);
            if (transform.gameObject.layer == 27)
            {
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("PlayerMid"));
            }
            else if (transform.gameObject.layer == 26)
            {
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
            }
            if (FSmashInput || m_fSmashCharging)
            {
                ForwardSmashAction(ctx);
            }
            else if (USmashInput || m_uSmashCharging)
            {
                UpSmashAction(ctx);
            }
            else if (DSmashInput || m_dSmashCharging)
            {
                DownSmashAction(ctx);
            }
            else
            {
                BasicAction(ctx);
            }
        }
        
    }

    public void Special(InputAction.CallbackContext ctx)
    {
        if (!m_isInHitStop && !m_isInKnockback && !m_inHitStun && !inAttack && !m_freefall && !m_animator.GetBool("inJumpSquat"))
        {

            if (m_isParrying)
            {
                DodgeAction(ctx);
            }
            else
            {
                SpecialAction(ctx);
            }
        }
        
    }

    public void ForwardSmash(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (FSmashInput)
            {
                FSmashInput = false;
            }
            else
            {
                FSmashInput = true;
            }
            
        }
    }

    public void UpSmash(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (USmashInput)
            {
                USmashInput = false;
            }
            else
            {
                USmashInput = true;
            }

        }
    }

    public void DownSmash(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed )
        {
            if (DSmashInput)
            {
                DSmashInput = false;
            }
            else
            {
                DSmashInput = true;
            }

        }
    }

    public void InstantForwardSmash(InputAction.CallbackContext ctx)
    {
        if ((!m_moving || striker) && !inAttack && !m_isInHitStop && !m_isInKnockback && !m_inHitStun && m_timeSinceAttack > m_LagTime && !m_animator.GetBool("inJumpSquat"))
        {
            if (m_facingDirection == -1 && !m_fSmashCharging)
            {
                m_facingDirection = -m_facingDirection;
                m_SR.flipX = !m_SR.flipX;
            }
            ForwardSmashAction(ctx);
        }
        
    }

    public void InstantUpSmash(InputAction.CallbackContext ctx)
    {
        if ((!m_moving || striker) && !inAttack && !m_isInHitStop && !m_isInKnockback && !m_inHitStun && m_timeSinceAttack > m_LagTime && !m_animator.GetBool("inJumpSquat"))
        {
            UpSmashAction(ctx);
        }
    }

    public void InstantDownSmash(InputAction.CallbackContext ctx)
    {
        if ((!m_moving || striker) && !inAttack && !m_isInHitStop && !m_isInKnockback && !m_inHitStun && m_timeSinceAttack > m_LagTime && !m_animator.GetBool("inJumpSquat"))
        {
            DownSmashAction(ctx);
        }
    }

    public void ReverseForwardSmash(InputAction.CallbackContext ctx)
    {
        if ((!m_moving || striker) && !m_isInHitStop && !m_isInKnockback && !m_inHitStun && m_timeSinceAttack > m_LagTime && !m_animator.GetBool("inJumpSquat"))
        {
            if (m_facingDirection == 1 && !m_fSmashCharging)
            {
                m_facingDirection = -m_facingDirection;
                m_SR.flipX = !m_SR.flipX;
            }

            ForwardSmashAction(ctx);
        }
    }

    public void Shield(InputAction.CallbackContext ctx)
    {
        if (!m_isInHitStop && !m_isInKnockback && !inAttack && !m_inHitStun && !m_freefall && !m_animator.GetBool("inJumpSquat"))
        {
            ShieldAction(ctx);
        }
    }

    public void Dodge(InputAction.CallbackContext ctx)
    {
        if (!m_dodging && !m_isInHitStop && !inAttack && !m_isInKnockback && !m_inHitStun && !m_freefall && !m_animator.GetBool("inJumpSquat"))
        {
            DodgeAction(ctx);
        }
        
    }
}