using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeHeroAnimEvents : MonoBehaviour
{
    // References to effect prefabs. These are set in the inspector
    [Header("Effects")]
    public GameObject           m_RunStopDust;
    public GameObject           m_JumpDust;
    public GameObject           m_LandingDust;
    public GameObject           m_DodgeDust;
    public GameObject           m_WallSlideDust;
    public GameObject           m_WallJumpDust;
    public GameObject           m_AirSlamDust;
    public GameObject           m_ParryEffect;

    public GameObject           m_HeavyHitFX;

    [Header("Objects")]
    private Conqueror       m_player;
    private AudioManager_PrototypeHero m_audioManager;

    public CombatManager c_Manager;
    [Header("Hitboxes")]
    public GameObject jab1Hitbox;
    public GameObject fSmashHitbox;
    public GameObject uSmashHitbox;
    public GameObject dSmashHitbox1;
    public GameObject dSmashHitbox2;
    public GameObject upTiltHitbox;
    public GameObject downSpecHitbox;
    public GameObject downTilt1Hitbox;
    public GameObject downTilt2Hitbox;
    public GameObject downTilt3Hitbox;
    public GameObject dairHitbox;
    public GameObject sideSpec1Hitbox;
    public GameObject sideSpec2Hitbox;
    public GameObject dashAttackHitbox;
    public GameObject upAirHitbox;

    public GameObject activeHitbox;
    private GameObject activeHitbox2;
    private GameObject activeHitbox3;
    [SerializeField]
    private float lingerDeltaTime;
    [SerializeField]
    private float lingerDurationSeconds;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GetComponentInParent<Conqueror>();
        m_audioManager = AudioManager_PrototypeHero.instance;
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

        if (m_player.transform.position.z > 10f)
        {
            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("PlayerMid"));
        }
        else
        {
            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("Player"));
        }
        
    }

    void AE_dTilt()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
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
        activeHitbox = Instantiate(downTilt1Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
            if (activeHitbox.transform.GetChild(0) != null)
            {
                activeHitbox.transform.GetChild(0).gameObject.layer = 19;
            }
        }
        StartCoroutine(Linger(lingerDeltaTime + .08f));
        c_Manager.hitEnemy = "None";
    }

    void AE_dTilt2()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
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
        
        activeHitbox2 = Instantiate(downTilt2Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox2.layer = 19;
            if (activeHitbox2.transform.GetChild(0) != null)
            {
                activeHitbox2.transform.GetChild(0).gameObject.layer = 19;
            }
        }
        StartCoroutine(Linger(lingerDeltaTime + .08f));
        c_Manager.hitEnemy = "None";
    }

    void AE_dTilt3()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
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
        activeHitbox3 = Instantiate(downTilt3Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox3.layer = 19;
            if (activeHitbox3.transform.GetChild(0) != null)
            {
                activeHitbox3.transform.GetChild(0).gameObject.layer = 19;
            }
        }
        StartCoroutine(Linger(lingerDeltaTime + .08f));
        c_Manager.hitEnemy = "None";
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
        m_player.m_animator.SetBool("inJumpSquat", false);
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

        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            //xDisplace = 3.0f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            //xDisplace = -3.0f;
        }
        m_player.m_inGroundSlam = true;
        m_player.GetComponent<PrototypeHero>().activeDairHitbox = Instantiate(dairHitbox, new Vector3((m_player.transform.position.x), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            m_player.GetComponent<PrototypeHero>().activeDairHitbox.layer = 19;
        }
        m_player.GetComponent<PrototypeHero>().activeDairHitbox.transform.SetParent(m_player.transform);
    }

    void AE_AttackAirLanding()
    {
        m_player.m_inGroundSlam = false;
        m_audioManager.PlaySound("AirSlamLanding");
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_AirSlamDust, 0.0f, dustYOffset);
        m_player.DisableMovement(0.5f);

        m_audioManager.PlaySound("SwordAttack");
        
        
        GameObject.Destroy(m_player.GetComponent<PrototypeHero>().activeDairHitbox);
        c_Manager.hitEnemy = "None";
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

    void AE_SwordAttack()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
        }
        activeHitbox = Instantiate(upTiltHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + 1.5f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        //StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_UpAir()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
        }
        activeHitbox = Instantiate(upAirHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        //StartCoroutine(Linger(lingerDeltaTime));
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

    void AE_FellDown()
    {
        m_player.m_fallingdown = false;
    }

    void AE_dashAttack()
    {
        m_audioManager.PlaySound("UnarmedAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = .35f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -.35f;
        }
        activeHitbox = Instantiate(dashAttackHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y + 1.85f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        //StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_JabHitbox()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0.3f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.3f;
        }
        activeHitbox = Instantiate(jab1Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
           
        }
        //StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_FSmashHitbox()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0.3f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.3f;
        }
        activeHitbox = Instantiate(fSmashHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
            if (activeHitbox.transform.GetChild(0) != null)
            {
                activeHitbox.transform.GetChild(0).gameObject.layer = 19;
            }
            
        }
        //StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_USmashHitbox()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0.3f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.3f;
        }
        activeHitbox = Instantiate(uSmashHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
            if (activeHitbox.transform.GetChild(0) != null)
            {
                activeHitbox.transform.GetChild(0).gameObject.layer = 19;
            }
        }
        //StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_dSmash()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = -0.2f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.0f;
        }
        activeHitbox = Instantiate(dSmashHitbox1, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
            if (activeHitbox.transform.GetChild(0) != null)
            {
                activeHitbox.transform.GetChild(0).gameObject.layer = 19;
            }
        }
        StartCoroutine(Linger(lingerDeltaTime + .08f));
        c_Manager.hitEnemy = "None";
    }

    void AE_dSmash2()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = -0.2f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.0f;
        }
        activeHitbox2 = Instantiate(dSmashHitbox2, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox2.layer = 19;
            if (activeHitbox2.transform.GetChild(0) != null)
            {
                activeHitbox2.transform.GetChild(0).gameObject.layer = 19;
            }
        }
        StartCoroutine(Linger(lingerDeltaTime + .08f));
        c_Manager.hitEnemy = "None";
    }

    void AE_Slide()
    {
        m_audioManager.PlaySound("SwordAttack");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = -2.2f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = 2.2f;
        }
        if (activeHitbox == null)
        {
            activeHitbox = Instantiate(downSpecHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
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
        else if (activeHitbox2 == null)
        {
            activeHitbox2 = Instantiate(downSpecHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox2.layer = 19;
                if (activeHitbox2.transform.GetChild(0) != null)
                {
                    activeHitbox2.transform.GetChild(0).gameObject.layer = 19;
                }
            }
        }
        else
        {
            activeHitbox3 = Instantiate(downSpecHitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y - .9f, m_player.transform.position.z),
            rotQuat, m_player.transform);
            if (m_player.transform.CompareTag("PlayerMid"))
            {
                activeHitbox3.layer = 19;
                if (activeHitbox3.transform.GetChild(0) != null)
                {
                    activeHitbox3.transform.GetChild(0).gameObject.layer = 19;
                }
            }
        }
        
        c_Manager.hitEnemy = "None";
    }

    void AE_SideSpec1Hitbox()
    {
        m_audioManager.PlaySound("ProxySlice");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
        if (m_player.m_facingDirection == 1)
        {
            rotQuat = new Quaternion(0f, 0f, 0f, 0f);
            xDisplace = 0.3f;
        }
        else
        {
            rotQuat = new Quaternion(0f, 180f, 0f, 0f);
            xDisplace = -0.3f;
        }
        activeHitbox = Instantiate(sideSpec1Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }
    void AE_SideSpec2Hitbox()
    {
        m_audioManager.PlaySound("ProxySlice");
        Quaternion rotQuat = new Quaternion();
        float xDisplace = 0.0f;
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
        activeHitbox = Instantiate(sideSpec2Hitbox, new Vector3((m_player.transform.position.x + xDisplace), m_player.transform.position.y, m_player.transform.position.z),
            rotQuat, m_player.transform);
        if (m_player.transform.CompareTag("PlayerMid"))
        {
            activeHitbox.layer = 19;
        }
        StartCoroutine(Linger(lingerDeltaTime));
        c_Manager.hitEnemy = "None";
    }

    void AE_Dair()
    {
        
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
        if (m_player.transform.position.z == 20)
        {
            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("iFrameMid"));
        }
        else
        {

            m_player.SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("iFrame"));
        }

    }

    void AE_AirDodge()
    {
        m_audioManager.PlaySound("Dodge");
        float dustYOffset = 0.078125f;
        m_player.SpawnDustEffect(m_DodgeDust, 0.0f, dustYOffset);
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
        if(!m_audioManager.IsPlaying("WallSlide")) 
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

    private IEnumerator Linger(float lingerDuration)
    {
        // TODO: add any logic we want here
        yield return new WaitForSeconds(lingerDuration);
        if (activeHitbox2 != null && activeHitbox == null)
        {
            GameObject.Destroy(activeHitbox2);
        }
        if (activeHitbox3 != null && activeHitbox2 == null)
        {
            GameObject.Destroy(activeHitbox3);
        }
        if (activeHitbox != null)
        {
            GameObject.Destroy(activeHitbox);
        }
        
    }

}
