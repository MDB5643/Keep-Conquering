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

    private TheChampion m_player;
    public CombatManager c_Manager;
    public GameObject jab1Hitbox;
    public GameObject jab2Hitbox;
    public GameObject sideSpecialHitbox;
    public GameObject sideSpecialExplode;

    private GameObject activeHitbox;
    [SerializeField]
    private float lingerDeltaTime;
    [SerializeField]
    private float lingerDurationSeconds;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GetComponentInParent<TheChampion>();
        c_Manager = GetComponentInParent<CombatManager>();
    }

    // Animation Events
    // These functions are called inside the animation files
    void AE_resetDodge()
    {
        m_player.ResetDodging();
        float dustXOffset = 0.6f;
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_RunStopDust, dustXOffset, dustYOffset);
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
    }

    void AE_Jump()
    {

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
    }

    void AE_AttackAirLanding()
    {
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_AirSlamDust, 0.0f, dustYOffset);
        m_player.DisableMovement(0.5f);
    }

    void AE_Hurt()
    {
    }

    void AE_Death()
    {
    }

    void AE_SwordAttack1()
    {
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
        activeHitbox = Instantiate(jab1Hitbox, new Vector2((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .74f),
            rotQuat, m_player.transform);
        StartCoroutine(Linger(0f));
        c_Manager.hitEnemy = "None";
    }
    void EndAttack()
    {
        //GameObject.Destroy(activeHitbox);
    }

    void AE_SwordAttack2()
    {
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 2.7f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -2.7f;
        }
        activeHitbox = Instantiate(jab2Hitbox, new Vector2((m_player.transform.position.x + xDisplace), m_player.transform.position.y),
            rotQuat, m_player.transform);
        StartCoroutine(Linger(0f));
        c_Manager.hitEnemy = "None";
    }

    void AE_SideSpecialRelease()
    {
        Quaternion rotQuat = new Quaternion();
        float xDisplace = .7f;
        float xDisplaceExplode = .9f;
        float pushForce = 10f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -.7f;
            pushForce = -pushForce;
            xDisplaceExplode = -xDisplaceExplode;
        }
        activeHitbox = Instantiate(sideSpecialHitbox, new Vector2((m_player.transform.position.x + xDisplace), m_player.transform.position.y),
            rotQuat, m_player.transform);
        m_player.GetComponent<Rigidbody2D>().AddForce(new Vector2(pushForce, 0.0f), ForceMode2D.Impulse);
        StartCoroutine(Linger(0.5f));
        c_Manager.hitEnemy = "None";
    }

    void SideSpecialExplode()
    {
        Quaternion rotQuat = new Quaternion();
        float xDisplaceExplode = .9f;
        float pushForce = 10f;
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
        activeHitbox = Instantiate(sideSpecialExplode, new Vector2((m_player.transform.position.x + xDisplaceExplode), m_player.transform.position.y),
            rotQuat, m_player.transform);
        m_player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-pushForce, 0.0f), ForceMode2D.Impulse);
        StartCoroutine(Linger(0.3f));
        c_Manager.hitEnemy = "None";
    }


    void AE_SheathSword()
    {
    }

    void AE_Dodge()
    {
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_DodgeDust, 0.0f, dustYOffset);
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
    }

    void AE_LedgeClimb()
    {
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
        GameObject.Destroy(activeHitbox);

        if (followUp == true)
        {
            SideSpecialExplode();
        }
    }
}
