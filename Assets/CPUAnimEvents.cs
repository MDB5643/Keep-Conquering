using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUAnimEvents : MonoBehaviour
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

    public GameObject attack1HB;
    public GameObject attack2HB;

    public GameObject m_GolemFlick;

    private MinionBehavior m_player;
    private GolemBehavior m_Golem;
    public CombatManager c_Manager;
    private AudioManager_PrototypeHero m_audioManager;
    private GameObject activeHitbox;

    [SerializeField]
    private float lingerDeltaTime;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GetComponentInParent<MinionBehavior>();
        m_Golem = GetComponentInParent<GolemBehavior>();
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
    }
    void AE_setPositionToClimbPosition()
    {
        m_player.SetPositionToClimbPosition();
    }

    void AE_runStop()
    {
        m_audioManager.PlaySound("RunStop");
        float dustXOffset = 0.6f;
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_RunStopDust, dustXOffset, dustYOffset);
    }

    void AE_footstep()
    {
        m_audioManager.PlaySound("Footstep");
    }

    void AE_Jump()
    {
        m_audioManager.PlaySound("Jump");

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
        m_audioManager.PlaySound("Landing");
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_LandingDust, 0.0f, dustYOffset);
    }

    void AE_Throw()
    {
        m_audioManager.PlaySound("Jump");
    }

    void AE_Parry()
    {
        m_audioManager.PlaySound("Parry");
        float xOffset = 0.1875f;
        float yOffset = 0.25f;
        m_player.SpawnDustEffect(m_ParryEffect, xOffset, yOffset);
        m_player.DisableMovement(0.5f);
    }

    void AE_ParryStance()
    {
        m_audioManager.PlaySound("DrawSword");
    }

    void AE_AttackAirSlam()
    {
        m_audioManager.PlaySound("DrawSword");
    }

    void AE_AttackAirLanding()
    {
        m_audioManager.PlaySound("AirSlamLanding");
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_AirSlamDust, 0.0f, dustYOffset);
        m_player.DisableMovement(0.5f);
    }

    void AE_Hurt()
    {
        m_audioManager.PlaySound("Hurt");
    }

    void AE_Whack()
    {
        m_audioManager.PlaySound("Whack");
    }

    void AE_Death()
    {
        m_audioManager.PlaySound("Death");
    }

    void AE_SwordAttack()
    {
        m_audioManager.PlaySound("PunchSwing");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = -.18f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = .18f;
        }
        activeHitbox = Instantiate(attack1HB, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + .25f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
            if (activeHitbox.transform.GetChild(0) != null)
            {
                activeHitbox.transform.GetChild(0).gameObject.layer = 19;
            }
        }
    }

    void AE_JabHitbox()
    {
        m_audioManager.PlaySound("SwordAttack");
    }

    void AE_SheathSword()
    {
        m_audioManager.PlaySound("SheathSword");
    }

    void AE_Dodge()
    {
        m_audioManager.PlaySound("Dodge");
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_DodgeDust, 0.0f, dustYOffset);
    }

    void AE_WallSlide()
    {
        //m_audioManager.GetComponent<AudioSource>().loop = true;
        if (!m_audioManager.IsPlaying("WallSlide"))
            m_audioManager.PlaySound("WallSlide");
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

    void AE_GolemFlick()
    {
        //m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_Golem.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0.3f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.3f;
        }
        activeHitbox = Instantiate(m_GolemFlick, new Vector3((m_Golem.transform.position.x + xDisplace), m_Golem.transform.position.y, m_Golem.transform.position.z),
            rotQuat, m_Golem.transform);
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    private IEnumerator Linger(float lingerDuration)
    {
        // TODO: add any logic we want here
        yield return new WaitForSeconds(lingerDuration);
        GameObject.Destroy(activeHitbox);

    }
}
