using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheChampionAnimEvents : MonoBehaviour
{
    // References to effect prefabs. These are set in the inspector
    [Header("Effects")]
    public GameObject m_RunStopDust;
    public GameObject m_JumpDust;
    public GameObject m_LandingDust;
    public GameObject m_DodgeDust;
    public GameObject m_WallSlideDust;
    public GameObject m_WallJumpDust;
    public GameObject m_AirSlamDust;
    public GameObject m_ParryEffect;

    private Conqueror m_player;
    private AudioManager_PrototypeHero m_audioManager;

    public CombatManager c_Manager;
    public GameObject jab1Hitbox;
    public GameObject jab2Hitbox;
    public GameObject sideSpecialHitbox;
    public GameObject sideSpecialExplode;
    public GameObject upTiltHitbox;
    public GameObject dTiltHitbox;
    public GameObject upSpecExplosionHitbox;
    public GameObject upSpecExplosionHitbox2;
    public GameObject upSmashHitbox;
    public GameObject dSmashHitbox;
    public GameObject dSmashHitboxBack;
    public GameObject downAirHitbox;
    public GameObject upAirHitbox;
    public GameObject fAirHitbox;
    public GameObject nAirHitbox;
    public GameObject nSpecHitbox;
    public GameObject dashAttackHitbox;

    public GameObject dSpecExplosionHitbox;
    public GameObject dSpecExplosionHitbox2;

    private GameObject activeHitbox;
    private GameObject activeHitbox2;
    [SerializeField]
    private float lingerDeltaTime;
    [SerializeField]
    private float lingerDurationSeconds;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GetComponentInParent<Conqueror>();
        c_Manager = GetComponentInParent<CombatManager>();
        m_audioManager = AudioManager_PrototypeHero.instance;
    }

    // Animation Events
    // These functions are called inside the animation files
    

    void AE_resetDodge()
    {
        m_player.ResetDodging();
        float dustXOffset = 0.6f;
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_RunStopDust, dustXOffset, dustYOffset);
        m_player.m_SR.color = Color.white;

        if (m_player.transform.position.z > 10f)
        {
            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("PlayerMid"));
        }
        else
        {
            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("Player"));
        }
    }

    void AE_setPositionToClimbPosition()
    {
        m_player.SetPositionToClimbPosition();
    }

    void AE_runStop()
    {
        float dustXOffset = 0.6f;
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_RunStopDust, dustXOffset, dustYOffset);
    }

    void AE_footstep()
    {
        m_audioManager.PlaySound("BBStep");
    }

    void AE_Jump()
    {
        m_audioManager.PlaySound("Jump");
        m_player.m_body2d.velocity = new Vector2(m_player.m_body2d.velocity.x, m_player.m_jumpForce);
        if (!m_player.IsWallSliding())
        {
            float dustYOffset = 0.078125f;
            m_player.SpawnDustEffect(m_JumpDust, 0.0f, dustYOffset);
        }
        else
        {
            m_player.SpawnDustEffect(m_WallJumpDust);
        }
    }

    void AE_Landing()
    {
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_LandingDust, 0.0f, dustYOffset);
    }

    void AE_Throw()
    {
    }

    void AE_dashAttack()
    {
        m_audioManager.PlaySound("ShieldBash");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = .8f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -.8f;
        }
        activeHitbox = Instantiate(dashAttackHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y , m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_FellDown()
    {
        m_player.m_fallingdown = false;
    }

    void AE_Parry()
    {
        float xOffset = 0.1875f;
        float yOffset = 0.25f;
        m_player.SpawnDustEffect(m_ParryEffect, xOffset, yOffset);
        m_player.DisableMovement(0.5f);
    }

    void AE_ParryStance()
    {
    }

    void AE_AttackAirSlam()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = -0.55f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0.55f;
        }
        activeHitbox = Instantiate(downAirHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - 1.3f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
                if (activeHitbox.transform.GetChild(0) != null)
                {
                    activeHitbox.transform.GetChild(0).gameObject.layer = 19;
                }
            }
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_UpAir()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.1f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = -0.7f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0.7f;
        }
        activeHitbox = Instantiate(upAirHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .5f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
                if (activeHitbox.transform.GetChild(0) != null)
                {
                    activeHitbox.transform.GetChild(0).gameObject.layer = 19;
                }
            }
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_ForwardAir()
    {
        m_audioManager.PlaySound("BBHeavy");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.1f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 2.4f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -2.4f;
        }
        activeHitbox = Instantiate(fAirHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
                if (activeHitbox.transform.GetChild(0) != null)
                {
                    activeHitbox.transform.GetChild(0).gameObject.layer = 19;
                }
            }
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }
    void AE_NeutralAir()
    {
        m_audioManager.PlaySound("BBFire");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.1f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0f;
        }
        activeHitbox = Instantiate(nAirHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .5f, m_player.transform.position.z - .1f),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
            }
        }
        c_Manager.hitEnemy = "None";
    }

    void AE_DSmash()
    {
        m_audioManager.PlaySound("BBFire");
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.1f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 1.5f;
            activeHitbox = Instantiate(dSmashHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .3f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -1.5f;
            activeHitbox = Instantiate(dSmashHitboxBack, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .3f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        }
        
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
            }
        }
        c_Manager.hitEnemy = "None";
    }

    void AE_Flip()
    {
        m_player.m_facingDirection = -m_player.m_facingDirection;
        if (m_player.m_SR.flipX == true)
        {
            m_player.m_SR.flipX = false;
        }
        else
        {
            m_player.m_SR.flipX = true;
        }
    }

    void AE_AttackAirLanding()
    {
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_AirSlamDust, 0.0f, dustYOffset);
        m_player.DisableMovement(0.5f);
    }

    void AE_Hurt()
    {
        m_audioManager.PlaySound("Hurt");
    }

    void AE_Death()
    {
        m_audioManager.PlaySound("Death");
        Quaternion rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        GameObject.Instantiate(m_player.KnockoutFX, new Vector2(m_player.transform.position.x, m_player.transform.position.y), rotQuat, null);
    }

    void AE_UpTilt()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.5f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0.5f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.5f;
        }
        activeHitbox = Instantiate(upTiltHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - 1f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
                if (activeHitbox.transform.GetChild(0) != null)
                {
                    activeHitbox.transform.GetChild(0).gameObject.layer = 19;
                }
            }
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }
    void AE_UpSpec()
    {
        m_audioManager.PlaySound("BBExplode");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        float pushForce = 5;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0f;

        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0f;
            pushForce = -pushForce;
        }
        activeHitbox = Instantiate(upSpecExplosionHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .5f, m_player.transform.position.z -.01f),
            rotQuat, m_player.transform);

        m_player.m_cameraShake = true;
        m_player.m_shakeIntensity = .22f;

        m_player.transform.GetComponentInChildren<Rigidbody2D>().velocity = new Vector2(0, 0);
        m_player.m_disableMovementTimer = 1.0f;
        m_player.transform.GetComponentInChildren<Rigidbody2D>().AddForce(new Vector2(pushForce, 18), ForceMode2D.Impulse);
        m_player.m_launched = true;
        m_player.m_SR.color = Color.white;
        m_player.GetComponent<TheChampion>().isInUpSpecial = true;
        m_player.isInStartUp = false;
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
            }
        }
        c_Manager.hitEnemy = "None";
    }

    void AE_DSpec()
    {
        m_audioManager.PlaySound("BBFire");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        float pushForce = 5;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0f;

        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0f;
            pushForce = -pushForce;
        }
        activeHitbox = Instantiate(dSpecExplosionHitbox2, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .7f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
            }
        }
        c_Manager.hitEnemy = "None";
    }

    void AE_DSpec2()
    {
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        float pushForce = 5;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0f;

        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0f;
            pushForce = -pushForce;
        }
        activeHitbox = Instantiate(dSpecExplosionHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y -.7f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
            }
        }
        c_Manager.hitEnemy = "None";
    }

    void AE_UpSpecLand()
    {
        m_audioManager.PlaySound("BBExplode");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0f;

        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0f;
        }
        activeHitbox = Instantiate(upSpecExplosionHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .5f, m_player.transform.position.z -.01f),
            rotQuat, m_player.transform);
        //activeHitbox2 = Instantiate(upSpecExplosionHitbox2, new Vector2(-(m_player.transform.position.x + xDisplace), m_player.transform.position.y),
        //    rotQuat);
        m_player.m_cameraShake = true;
        m_player.m_shakeIntensity = .22f;
        m_player.m_disableMovementTimer = 1.0f;
        m_player.m_SR.color = Color.white;
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
            //activeHitbox2.layer = 19;
        }
        c_Manager.hitEnemy = "None";
    }

    void AE_SwordAttack1()
    {
        m_audioManager.PlaySound("BBHeavy");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 3.0f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -3.0f;
        }
        activeHitbox = Instantiate(jab1Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .74f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox.layer = 19;
            }
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_NSpec()
    {
        m_audioManager.PlaySound("BBFire");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 0f;
        }
        if (m_player.m_grounded)
        {
            if (m_player.tag == "PlayerMid")
            {
                Instantiate(nSpecHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .41f, m_player.transform.position.z -.01f),
            rotQuat, m_player.transform).layer = 19;
            }
            else
            {
                Instantiate(nSpecHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .41f, m_player.transform.position.z - .01f),
            rotQuat, m_player.transform);
            }
            
            
            c_Manager.hitEnemy = "None";
        }
        
    }

    void AE_USmashHitbox()
    {
        m_audioManager.PlaySound("BBFire");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0.05f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.05f;
        }
        activeHitbox = Instantiate(upSmashHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - 1f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
            if (activeHitbox.transform.GetChild(0) != null)
            {
                activeHitbox.transform.GetChild(0).gameObject.layer = 19;
            }
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }
    void EndAttack()
    {
        //GameObject.Destroy(activeHitbox);
    }

    void AE_SwordAttack2()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 1.9f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -1.9f;
        }
        activeHitbox = Instantiate(jab2Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_DTilt()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 1.3f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -1.3f;
        }
        activeHitbox = Instantiate(dTiltHitbox, new Vector2((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .7f),
            rotQuat, m_player.transform);
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_SideSpecialRelease()
    {
        m_audioManager.PlaySound("BBFireSword");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0f;
        float xDisplaceExplode = .9f;
        float pushForce = 10f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0f;
            pushForce = -pushForce;
            xDisplaceExplode = -xDisplaceExplode;
        }
        activeHitbox = Instantiate(sideSpecialHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        m_player.GetComponent<Rigidbody2D>().AddForce(new Vector2(pushForce, 0.0f), ForceMode2D.Impulse);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        StartCoroutine(Linger(0.5f));
        //c_Manager.hitEnemy = "None";
    }

    void SideSpecialExplode()
    {
        m_audioManager.PlaySound("BBExplode2");
        Quaternion rotQuat = new Quaternion();
        float xDisplaceExplode = 1.5f;
        float pushForce = 5f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            pushForce = -pushForce;
            xDisplaceExplode = -xDisplaceExplode;
        }

        m_player.m_disableMovementTimer = .5f;
        m_player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        m_player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-pushForce, 0.0f), ForceMode2D.Impulse);
        foreach (var c in m_player.GetComponentInParent<Conqueror>().grabbedPlayers)
        {
            c.transform.parent = null;
            c.isGrappled = false;
        }
        foreach (var c in m_player.GetComponentInParent<Conqueror>().grabbedMinions)
        {
            c.transform.parent = null;
            c.isGrappled = false;
        }
        m_player.GetComponentInParent<Conqueror>().grabbedPlayers.Clear();
        m_player.GetComponentInParent<Conqueror>().grabbedMinions.Clear();
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            Instantiate(sideSpecialExplode, new Vector3((m_player.transform.position.x + xDisplaceExplode), m_player.transform.position.y, m_player.transform.position.z - .01f),
            rotQuat, m_player.transform).layer = 19;
        }
        else
        {
            Instantiate(sideSpecialExplode, new Vector3((m_player.transform.position.x + xDisplaceExplode), m_player.transform.position.y, m_player.transform.position.z - .01f),
            rotQuat, m_player.transform);
        }
        m_player.m_cameraShake = true;
        m_player.m_shakeIntensity = .22f;
        c_Manager.hitEnemy = "None";
    }


    void AE_SheathSword()
    {
    }

    void AE_Dodge()
    {
        m_audioManager.PlaySound("Dodge");
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_DodgeDust, 0.0f, dustYOffset);
        Color tmp = m_player.m_SR.color;
        tmp.a = .6f;
        m_player.m_SR.color = tmp;
        if (m_player.transform.position.z == 20)
        {
            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("iFrameMid"));
        }
        else
        {

            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("iFrame"));
        }
    }

    void AE_WallSlide()
    {
        //m_audioManager.GetComponent<AudioSource>().loop = true;
        float dustXOffset = 0.25f;
        float dustYOffset = 0.25f;
        m_player.SpawnDustEffect(m_WallSlideDust, dustXOffset, dustYOffset);
    }

    void AE_LedgeGrab()
    {
        m_audioManager.PlaySound("LedgeGrab");
    }

    void AE_LedgeClimb()
    {
        m_audioManager.PlaySound("RunStop");
    }

    private IEnumerator Linger(float extraTime)
    {
        bool followUp = false;
            // TODO: add any logic we want here
        if (activeHitbox.name.StartsWith("ChampSideSpecHB"))
        {
            followUp = true;
        }
        yield return new WaitForSeconds(lingerDeltaTime + extraTime);
        if (activeHitbox2 != null)
        {
            GameObject.Destroy(activeHitbox2);
        }
        if (activeHitbox != null)
        {
            GameObject.Destroy(activeHitbox);
        }
        

        if (followUp == true)
        {
            SideSpecialExplode();
        }
    }
}
