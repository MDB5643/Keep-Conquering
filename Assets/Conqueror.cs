using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections.Generic;

public class Conqueror : MonoBehaviour
{
    public string teamColor = "Blue";
    public int playerNumber = 1;

    public float m_runSpeed = 4.5f;
    public float m_walkSpeed = 2.0f;
    public float m_jumpForce = 7.5f;
    public float m_dodgeForce = 6.0f;
    public float m_parryKnockbackForce = 4.0f;
    public bool m_noBlood = false;
    public bool m_hideSword = false;
    public Text infoText;
    public Text m_DamageDisplay;
    public float currentDamage = 0.0f;
    public string hookDirection = "";

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

    public bool m_grounded = false;
    public bool m_moving = false;
    public bool m_dead = false;
    public bool m_dodging = false;
    public bool m_wallSlide = false;
    public bool m_ledgeGrab = false;
    public bool m_ledgeClimb = false;
    public bool m_crouching = false;
    public bool m_inHitStun = false;
    public bool m_launched = false;
    public bool m_fSmashCharging = false;
    public bool m_uSmashCharging = false;
    public bool m_dSmashCharging = false;
    public bool m_fullCharge = false;
    public bool m_isParrying = false;
    public bool m_isInKnockback = false;
    public bool m_isInHotZone = false;
    public bool m_cameraShake = false;
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
    public float m_timeSinceAttack = 0.0f;
    public float m_timeSinceNSpec = 0.0f;
    public float m_timeSinceSideSpecial = 0.0f;
    public float m_timeSinceStun = 5.0f;
    public float m_timeSinceHitStun = 0.0f;
    public float m_timeSinceKnockBack = 0.0f;
    public float m_KnockBackDuration = 0.2f;
    public float m_KnockBackMomentumX = 0.0f;
    public float m_KnockBackMomentumY = 0.0f;
    public float m_hitStunDuration = 0.0f;
    public float m_gravity;
    public float m_maxSpeed = 4.5f;
    public bool m_isInHitStop;
    public bool isInStartUp = false;
    public float m_hitStopDuration;
    public float m_timeSinceHitStop = 0.0f;
    public float animSpeed;

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
    public GameObject HeavyAttackFX;
    public GameObject ChargeFlashFX;
    public GameObject BlockFX;
    public GameObject EyeShotFX;

    public ShieldBar shieldBar;
    public HookBehavior hookPrefab;
    public LineController hookChain;
    public Transform launchOffset;
    public SpriteShapeDemo m_SpriteShaper;
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

    public AudioManager_PrototypeHero m_audioManager;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
        m_gravity = m_body2d.gravityScale;
        m_SpriteShaper = GetComponentInChildren<SpriteShapeDemo>();

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorR0 = transform.Find("WallSensor_R0").GetComponent<Sensor_Prototype>();
        m_wallSensorL0 = transform.Find("WallSensor_L0").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

        m_audioManager = AudioManager_PrototypeHero.instance;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
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
            }
            else
            {
                Instantiate(LightAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), transform);
            }
            if (coll.transform.name.Contains("ChargeBall"))
            {
                GameObject.Destroy(coll.transform.parent.gameObject);
            }
            coll.GetComponentInParent<CombatManager>().Hit(transform, coll.transform.name);
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

            TakeDamage(10f);

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
        if (coll.gameObject.CompareTag("BlastZone") || coll.gameObject.CompareTag("BlastZoneMid"))
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

    public void Attack(float baseDamage, float baseKnockBack, float baseRange, float KBModX, float KBmodY, Transform attackPoint)
    {
        float damageModifier = 0.0f;

        var playerCollider = GetComponent<BoxCollider2D>();

        Vector2 attackHitboxCenter = attackPoint.position;

        //Detect enemy collision with attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackHitboxCenter, baseRange, enemyLayers);

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
                enemy.GetComponentInParent<CPUBehavior>().incomingKnockback = jabKB;
                enemy.GetComponentInParent<CPUBehavior>().incomingXMod = 0f;
                enemy.GetComponentInParent<CPUBehavior>().incomingYMod = 2f;
                enemy.GetComponentInParent<CPUBehavior>().HitStun(.15f);
                //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKnockBack, attackAngle, KBModX, KBmodY, above);

            }
        }
    }

    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

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
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
        if (playerNumber == 1)
        {
            Transform spawnPoint = GameObject.Find("P1RespawnPoint").transform;
            transform.position = spawnPoint.position;
        }
        else if (playerNumber == 3)
        {
            Transform spawnPoint = GameObject.Find("P2RespawnPoint").transform;
            transform.position = spawnPoint.position;
        }
        
        
        transform.tag = "Player";
        m_dead = false;
        m_launched = false;
        m_animator.Rebind();
        ResetDodging();
    }

    public void TakeDamage(float damage)
    {
        currentDamage += damage;
        m_launched = false;

        //Play hurt animation
        m_animator.SetTrigger("Hurt");
    }

    public void HitStun(float stunTime)
    {
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

        if (!m_inHitStun)
        {
            ResetDodging();
            m_timeSinceHitStun = 0.0f;

            m_disableMovementTimer = stunTime;
            m_hitStunDuration = stunTime;

            m_inHitStun = true;
        }
        //m_animator.SetTrigger("HitStun");

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

    public void Block(float shieldDamage)
    {
        shieldBar.shieldHealth -= shieldDamage * 0.75f;
        var e_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_facingDirection == 1)
        {
            e_Rigidbody2D.AddForce(new Vector2(-shieldDamage * 0.3f, 0), ForceMode2D.Impulse);
        }
        if (m_facingDirection == -1)
        {
            e_Rigidbody2D.AddForce(new Vector2(shieldDamage * 0.3f, 0), ForceMode2D.Impulse);
        }

    }

}