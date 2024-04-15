using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public string hitEnemy;
    private AudioManager_PrototypeHero m_audioManager;
    public GameObject crosshair;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(Transform enemy, CollisionTracker collisionData)
    {
        float baseKB = 0.0f;
        float baseDmg = 0.0f;
        float modifierx = 0.0f;
        float modifiery = 1.0f;
        float multmodifiery = 1.0f;
        float multmodifierx = 1.0f;
        float hitstun = .15f;
        bool isGrab = false;

        var m_player = transform;
        m_audioManager = AudioManager_PrototypeHero.instance;
        Conqueror playerCharacter = GetComponentInParent<Conqueror>();
        string playerTeamColor = "";
        var animator = GetComponentInParent<Animator>();
        bool alreadyHit = false;
        if (collisionData)
        {
            alreadyHit = collisionData.hitEnemies.Contains(enemy.name);
            if (alreadyHit)
            {
                hitEnemy = "None";
            }
        }

        if (collisionData && (collisionData.gameObject.GetComponent<ProjectileBehavior>() || (collisionData.transform.parent && collisionData.gameObject.GetComponentInParent<ArrowBehavior>())))
        {
            if (m_player.GetComponent<ProjectileBehavior>())
            {
                ProjectileBehavior m_projectile = collisionData.gameObject.GetComponent<ProjectileBehavior>();
                playerTeamColor = m_projectile.teamColor;
            }
            else if (collisionData.gameObject.GetComponentInParent<ArrowBehavior>())
            {
                ArrowBehavior m_projectile = collisionData.gameObject.GetComponentInParent<ArrowBehavior>();
                playerTeamColor = m_projectile.teamColor;
            }
            if (enemy.GetComponentInParent<Conqueror>() != null && enemy.GetComponentInParent<Conqueror>().teamColor == playerTeamColor)
            {

            }
            else if (enemy.GetComponentInParent<MinionBehavior>() != null && enemy.GetComponentInParent<MinionBehavior>().teamColor == playerTeamColor)
            {

            }
            else if (enemy.GetComponentInParent<TowerEye>() != null && enemy.GetComponentInParent<TowerEye>().teamColor == playerTeamColor)
            {

            }
            else
            {
                collisionData.hitEnemies.Add(enemy.name);
                baseDmg = collisionData.baseDamage;
                baseKB = collisionData.baseKB;
                enemy.GetComponent<Conqueror>().m_shakeIntensity = collisionData.shakeIntensity;
                enemy.GetComponent<Conqueror>().m_cameraShake = collisionData.shakeScreen;

                m_audioManager.PlaySound(collisionData.soundName);

                hitstun = collisionData.hitStun;
                modifierx = collisionData.modifierX;
                modifiery = collisionData.modifierY;
            }
        }

        else if (m_player.GetComponentInChildren<CollisionTracker>())
        {
            collisionData.hitEnemies.Add(enemy.name);
            playerTeamColor = playerCharacter.teamColor;

            if (collisionData.smash)
            {
                baseDmg = collisionData.baseDamage + playerCharacter.m_chargeModifier * collisionData.damageModifier;
                baseKB = collisionData.baseKB + playerCharacter.m_chargeModifier * collisionData.KBModifier;

                playerCharacter.m_fSmashCharging = false;
                playerCharacter.m_chargeModifier = 0.0f;

            }
            else
            {
                baseDmg = collisionData.baseDamage;
                baseKB = collisionData.baseKB;
            }

            playerCharacter.m_shakeIntensity = collisionData.shakeIntensity;
            playerCharacter.m_cameraShake = collisionData.shakeScreen;

            m_audioManager.PlaySound(collisionData.soundName);

            hitstun = collisionData.hitStun;
            modifierx = collisionData.modifierX;
            modifiery = collisionData.modifierY;

        }

        if (collisionData)
        {
            
            if (collisionData.transform.name.StartsWith("ProtoSideSpec2HB") && m_player.GetComponent<PrototypeHero>().currentHookTarget == null)
            {
                GameObject.Instantiate(crosshair, enemy);
                m_player.GetComponent<PrototypeHero>().currentHookTarget = enemy.gameObject;
            }

            else if (collisionData.transform.name.StartsWith("ROUpSpec"))
            {
            
                m_player.GetComponentInParent<Conqueror>().m_groundSensor.Disable(0.25f);
            
                if (enemy.position.z == 20)
                {
                    enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("iFrameMid"));
                }
                else
                {
                    enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("iFrame"));
                }
            
                enemy.SetPositionAndRotation(new Vector3(m_player.transform.position.x, m_player.transform.position.y, m_player.transform.position.z), new Quaternion(0f, 0f, 0f, 0f));
            }

                //if (collisionData.transform.name.StartsWith("ROUpSpec2"))
                //{
                //    m_player.GetComponentInParent<Conqueror>().m_groundSensor.Disable(0.35f);
                //    m_player.GetComponentInParent<Conqueror>().m_body2d.velocity = new Vector2(0, 5);
                //    if (enemy.position.z == 20)
                //    {
                //        enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("PlayerMid"));
                //    }
                //    else
                //    {
                //        enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("Player"));
                //    }
                //
                //    enemy.SetPositionAndRotation(new Vector3(m_player.transform.position.x, m_player.transform.position.y - 1f, m_player.transform.position.z), new Quaternion(0f, 0f, 0f, 0f));
                //
                //    baseDmg = 5.5f;
                //    baseKB = .35f;
                //    multmodifiery = -1f;
                //    collisionData.grab = false;
                //    m_audioManager.PlaySound("Hurt");
                //}

                //else
                //{
                //    if (attackType.StartsWith("Jab1Hitbox"))
                //    {
                //        baseDmg = 5.5f + playerCharacter.m_chargeModifier * 1.2f;
                //        baseKB = .5f + playerCharacter.m_chargeModifier * .05f;
                //
                //        playerCharacter.m_fSmashCharging = false;
                //        playerCharacter.m_chargeModifier = 0.0f;
                //
                //        m_audioManager.PlaySound("Whack");
                //        playerCharacter.m_shakeIntensity = .2f;
                //        playerCharacter.m_cameraShake = true;
                //
                //
                //        hitstun = .3f;
                //        modifiery = 2f;
                //    }
                //    if (attackType.StartsWith("Jab2Hitbox"))
                //    {
                //        baseDmg = 4.2f;
                //        baseKB = 0.1f;
                //
                //        modifiery = 2f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBDAHB"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = .18f;
                //        modifiery = .14f;
                //        modifierx = .15f;
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("ChampSideSpecialBomb"))
                //    {
                //        baseDmg = 9.5f;
                //        baseKB = .6f;
                //        modifiery = 2f;
                //        collisionData.grab = false;
                //        playerCharacter.m_shakeIntensity = .2f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //        m_audioManager.PlaySound("Hurt");
                //
                //    }
                //    if (attackType.StartsWith("ChampSideSpecHB"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = 0.0f;
                //        multmodifiery = 0.0f;
                //        collisionData.grab = true;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBDair"))
                //    {
                //        baseDmg = 3f;
                //        baseKB = 0.3f;
                //
                //        modifiery = -5f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBDTilt"))
                //    {
                //        baseDmg = 3f;
                //        baseKB = .1f;
                //
                //        modifiery = 2.2f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBFairSpike"))
                //    {
                //        baseDmg = 6.5f;
                //        baseKB = .5f;
                //
                //        hitstun = .3f;
                //        modifiery = 8f;
                //        multmodifiery = -1;
                //        playerCharacter.m_shakeIntensity = .22f;
                //        playerCharacter.m_cameraShake = true;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    else if (attackType.StartsWith("BBFair"))
                //    {
                //        baseDmg = 6.5f;
                //        baseKB = .5f;
                //
                //        hitstun = .3f;
                //        modifiery = 6f;
                //        playerCharacter.m_shakeIntensity = .2f;
                //        playerCharacter.m_cameraShake = true;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBDSmash"))
                //    {
                //        baseDmg = 8.2f + playerCharacter.m_chargeModifier * 1.2f;
                //        baseKB = 0.5f + playerCharacter.m_chargeModifier * .05f;
                //
                //        playerCharacter.m_dSmashCharging = false;
                //        playerCharacter.m_chargeModifier = 0.0f;
                //        m_audioManager.PlaySound("Whack");
                //        playerCharacter.m_shakeIntensity = .2f;
                //        playerCharacter.m_cameraShake = true;
                //
                //        modifiery = 3f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBNSpec"))
                //    {
                //        baseDmg = 2.5f;
                //        baseKB = 0.08f;
                //
                //        modifiery = .1f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("NSpecFire"))
                //    {
                //        baseDmg = .5f;
                //        baseKB = 0.0f;
                //
                //        m_audioManager.PlaySound("Hurt");
                //        modifiery = 0f;
                //    }
                //    if (attackType.StartsWith("BBDSpecHB"))
                //    {
                //        baseDmg = 4.5f;
                //        baseKB = 0.3f;
                //
                //        modifiery = 4f;
                //        playerCharacter.m_cameraShake = true;
                //        playerCharacter.m_shakeIntensity = .22f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBUair"))
                //    {
                //        baseDmg = 5.0f;
                //        baseKB = 0.2f;
                //
                //        modifiery = 4f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBNair"))
                //    {
                //        baseDmg = 2.5f;
                //        baseKB = 0.1f;
                //
                //        modifiery = 1f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("BBUSmash"))
                //    {
                //        baseDmg = 8.5f + playerCharacter.m_chargeModifier * 1.2f;
                //        baseKB = 0.5f + playerCharacter.m_chargeModifier * .05f;
                //
                //        playerCharacter.m_uSmashCharging = false;
                //        playerCharacter.m_chargeModifier = 0.0f;
                //        m_audioManager.PlaySound("Whack");
                //        playerCharacter.m_cameraShake = true;
                //        playerCharacter.m_shakeIntensity = .2f;
                //        modifiery = 6.5f;
                //
                //    }
                //    if (attackType.StartsWith("BBUTilt"))
                //    {
                //        baseDmg = 3.5f;
                //        baseKB = 0.2f;
                //        m_audioManager.PlaySound("Hurt");
                //        modifiery = 6.5f;
                //    }
                //    if (attackType.StartsWith("ProtoJab1"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = .2f;
                //        modifiery = 1f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("DPDash"))
                //    {
                //        baseDmg = 3.3f;
                //        baseKB = .23f;
                //        modifiery = 1.5f;
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("ProtoUTilt"))
                //    {
                //        baseDmg = 3.2f;
                //        baseKB = 0.3f;
                //        modifiery = 3f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("UAirProto"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = 0.3f;
                //        modifiery = 3f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.Contains("DTilt"))
                //    {
                //        baseDmg = 2f;
                //        baseKB = 0.2f;
                //        modifiery = 3f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.Contains("ProtoDSpec"))
                //    {
                //        baseDmg = 1.2f;
                //        baseKB = 0.2f;
                //        modifiery = 4f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("ProtoDair"))
                //    {
                //        baseDmg = 4.0f;
                //        baseKB = 0.5f;
                //
                //        modifiery = -3f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("ProtoSideSpec1"))
                //    {
                //        baseDmg = 3.4f;
                //        baseKB = 0.0f;
                //        hitstun = .28f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    if (attackType.StartsWith("ProtoSideSpec2"))
                //    {
                //        baseDmg = 3.3f;
                //        baseKB = 0.1f;
                //        modifiery = 8f;
                //        m_audioManager.PlaySound("Hurt");
                //
                //        if (m_player.GetComponent<PrototypeHero>().currentHookTarget == null)
                //        {
                //            GameObject.Instantiate(crosshair, enemy);
                //            m_player.GetComponent<PrototypeHero>().currentHookTarget = enemy.gameObject;
                //        }
                //        
                //    }
                //    if (attackType.StartsWith("FSmash"))
                //    {
                //        baseDmg = 5f + playerCharacter.m_chargeModifier * 1.1f;
                //        baseKB = .7f + playerCharacter.m_chargeModifier * .05f;
                //
                //        playerCharacter.m_fSmashCharging = false;
                //        playerCharacter.m_chargeModifier = 0.0f;
                //        m_audioManager.PlaySound("ProxyHeavy");
                //        playerCharacter.m_shakeIntensity = .2f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        //hitstun = .31f;
                //        modifiery = 2f;
                //    }
                //    if (attackType.StartsWith("USmash"))
                //    {
                //        baseDmg = 4.8f + playerCharacter.m_chargeModifier * 1.1f;
                //        baseKB = .5f + playerCharacter.m_chargeModifier * .05f;
                //
                //        playerCharacter.m_uSmashCharging = false;
                //        playerCharacter.m_chargeModifier = 0.0f;
                //        m_audioManager.PlaySound("ProxyHeavy");
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //        playerCharacter.m_shakeIntensity = .15f;
                //        //hitstun = .31f;
                //        modifiery = 6f;
                //    }
                //    if (attackType.StartsWith("DSmash"))
                //    {
                //        baseDmg = 5f + playerCharacter.m_chargeModifier * 1.1f;
                //        baseKB = .6f + playerCharacter.m_chargeModifier * .05f;
                //
                //        playerCharacter.m_dSmashCharging = false;
                //        modifiery = 6f;
                //        playerCharacter.m_chargeModifier = 0.0f;
                //        m_audioManager.PlaySound("ProxyHeavy");
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //        enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        hitstun = .28f;
                //
                //    }
                //    if (attackType.StartsWith("ChargeBall"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = 0.1f;
                //        modifiery = .1f;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    
                //    if (attackType.StartsWith("RRJabHB"))
                //    {
                //        baseDmg = 3.2f;
                //        baseKB = 0.14f;
                //        modifiery = .2f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRUptilt"))
                //    {
                //        baseDmg = 3.2f;
                //        baseKB = 0.12f;
                //        modifiery = .3f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRFSmash"))
                //    {
                //        baseDmg = 6.5f + playerCharacter.m_chargeModifier * 1.1f;
                //        baseKB = .4f + playerCharacter.m_chargeModifier * .05f;
                //        modifiery = .15f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .15f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRForwardAir"))
                //    {
                //        baseDmg = 3.1f;
                //        baseKB = .16f;
                //        modifiery = .2f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRNair"))
                //    {
                //        baseDmg = 3.4f;
                //        baseKB = .11f;
                //        modifiery = .2f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRUpAir"))
                //    {
                //        baseDmg = 3.4f;
                //        baseKB = .11f;
                //        modifiery = .2f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRDownAir"))
                //    {
                //        baseDmg = 3.1f;
                //        baseKB = .08f;
                //        modifiery = -.5f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRDownSmash"))
                //    {
                //        baseDmg = 6.3f + playerCharacter.m_chargeModifier * 1.1f;
                //        baseKB = .35f + playerCharacter.m_chargeModifier * .05f;
                //        modifiery = .05f;
                //        modifierx = .1f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .15f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRDownTilt"))
                //    {
                //        baseDmg = 2.0f;
                //        baseKB = .1f;
                //        modifiery = .15f;
                //        modifierx = .1f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("RRDAHB"))
                //    {
                //        baseDmg = 2.0f;
                //        baseKB = .15f;
                //        modifiery = .15f;
                //        modifierx = .1f;
                //        m_audioManager.PlaySound("MagicHit");
                //    }
                //    if (attackType.StartsWith("ROJabHB"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = 0.1f;
                //        modifiery = .1f;
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("ROJabFlurry"))
                //    {
                //        baseDmg = 1.3f;
                //        baseKB = 0.09f;
                //        modifiery = .05f;
                //        m_audioManager.PlaySound("UnarmedLight");
                //    }
                //    if (attackType.StartsWith("ROUTilt"))
                //    {
                //        baseDmg = 4.5f;
                //        baseKB = 0.16f;
                //        modifiery = .31f;
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("ROFSmash"))
                //    {
                //        baseDmg = 7.1f + playerCharacter.m_chargeModifier * 1.12f;
                //        baseKB = .51f + playerCharacter.m_chargeModifier * .05f;
                //        modifiery = .12f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .18f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("UnarmedSmash");
                //    }
                //    if (attackType.StartsWith("RONSpec"))
                //    {
                //        baseDmg = 8.8f;
                //        baseKB = .77f;
                //        modifiery = .05f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .22f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("UnarmedSmash");
                //    }
                //    if (attackType.StartsWith("ROSideSpec"))
                //    {
                //        baseDmg = 5.0f;
                //        baseKB = .2f;
                //        modifiery = .05f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .14f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("UnarmedSmash");
                //    }
                //    if (attackType.StartsWith("ROFair"))
                //    {
                //        baseDmg = 4.5f;
                //        baseKB = .18f;
                //        modifiery = .21f;
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("RONair"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = .1f;
                //        modifiery = .1f;
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("ROUpair"))
                //    {
                //        baseDmg = 4.2f;
                //        baseKB = .15f;
                //        modifiery = .28f;
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("RODair"))
                //    {
                //        baseDmg = 5.8f;
                //        baseKB = .5f;
                //        modifiery = -.7f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .15f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("BluntHit");
                //    }
                //    if (attackType.StartsWith("RODSmash"))
                //    {
                //        baseDmg = 7.3f + playerCharacter.m_chargeModifier * 1.1f;
                //        baseKB = .35f + playerCharacter.m_chargeModifier * .05f;
                //        modifiery = .15f;
                //        modifierx = .15f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .18f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("UnarmedSmash");
                //    }
                //    if (attackType.StartsWith("ROUpSmash"))
                //    {
                //        baseDmg = 7.6f + playerCharacter.m_chargeModifier * 1.1f;
                //        baseKB = .35f + playerCharacter.m_chargeModifier * .05f;
                //        modifiery = .2f;
                //        modifierx = .05f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .18f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("UnarmedSmash");
                //    }
                //    if (attackType.StartsWith("RODSpecFall"))
                //    {
                //        baseDmg = 6.5f;
                //        baseKB = .25f;
                //        modifiery = -.15f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .1f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("UnarmedSmash");
                //    }
                //    if (attackType.StartsWith("RODSpec1HB"))
                //    {
                //        baseDmg = 5.0f;
                //        baseKB = .2f;
                //        modifiery = .15f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .18f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("UnarmedSmash");
                //    }
                //    if (attackType.StartsWith("RODownTilt"))
                //    {
                //        baseDmg = 3.0f;
                //        baseKB = .15f;
                //        modifiery = .1f;
                //        
                //        m_audioManager.PlaySound("UnarmedLight");
                //    }
                //    if (attackType.StartsWith("RODash"))
                //    {
                //        baseDmg = 5.2f;
                //        baseKB = .2f;
                //        modifiery = .15f;
                //        modifierx = .12f;
                //
                //        playerCharacter.m_cameraShake = true;
                //        if (enemy.GetComponentInParent<Conqueror>() != null)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .1f;
                //            enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                //        }
                //
                //        m_audioManager.PlaySound("BluntHit");
                //    }

                //    else if (attackType.StartsWith("ROUpSpec"))
                //    {
                //
                //        m_player.GetComponentInParent<Conqueror>().m_groundSensor.Disable(0.25f);
                //
                //        if (enemy.position.z == 20)
                //        {
                //            enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("iFrameMid"));
                //        }
                //        else
                //        {
                //            enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("iFrame"));
                //        }
                //
                //        enemy.SetPositionAndRotation(new Vector3(m_player.transform.position.x, m_player.transform.position.y, m_player.transform.position.z), new Quaternion(0f, 0f, 0f, 0f));
                //
                //        baseDmg = 2.0f;
                //        baseKB = 0.0f;
                //        multmodifiery = 0.0f;
                //        collisionData.grab = true;
                //        m_audioManager.PlaySound("Hurt");
                //    }
                //    
                //    if (attackType.StartsWith("GolemFlick"))
                //    {
                //        baseDmg = 7f;
                //        baseKB = 1f;
                //        modifiery = 15f;
                //        m_audioManager.PlaySound("GolemHit");
                //    }
                //}


                //golem attack
                if (m_player.GetComponentInParent<GolemBehavior>())
            {
                GolemBehavior golemBehavior = m_player.GetComponentInParent<GolemBehavior>();
                hitEnemy = enemy.name;

                var above = false;

                if (collisionData.grab)
                {
                    enemy.parent = m_player;
                    enemy.GetComponentInParent<Conqueror>().isGrappled = true;
                    
                }
                else
                {
                    enemy.parent = null;
                    if (enemy.GetComponentInParent<Conqueror>() != null)
                    {
                        enemy.GetComponentInParent<Conqueror>().isGrappled = false;
                    }

                }

                //Detect impact angle
                var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                var sourceclosestPoint = new Vector2(m_player.transform.position.x, m_player.transform.position.y);
                if (sourceclosestPoint.y > targetclosestPoint.y)
                {
                    above = true;
                }

                var positionDifference = targetclosestPoint - sourceclosestPoint;

                //Must be done to detect y axis angle
                float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

                // Convert the angle to degrees.
                float attackAngle = angleInRadians * Mathf.Rad2Deg;

                if (enemy.GetComponentInParent<Conqueror>() != null)
                {
                    //Apply damage
                    enemy.GetComponentInParent<Conqueror>().TakeDamage(baseDmg);
                    //Apply Knockback
                    enemy.GetComponentInParent<Conqueror>().incomingAngle = attackAngle;
                    enemy.GetComponentInParent<Conqueror>().incomingKnockback = baseKB;
                    enemy.GetComponentInParent<Conqueror>().incomingXMod = modifierx;
                    enemy.GetComponentInParent<Conqueror>().incomingYMod = (modifiery + (enemy.GetComponentInParent<Conqueror>().currentDamage / 4)) * multmodifiery;
                    enemy.GetComponentInParent<Conqueror>().HitStun(hitstun);

                    //golemBehavior.m_isInHitStop = true;
                    //golemBehavior.m_hitStopDuration = enemy.GetComponentInParent<Conqueror>().m_hitStunDuration * .8f;
                }


            }

            //hit a player
            else if (enemy.GetComponentInParent<Conqueror>() != null && enemy.GetComponentInParent<Conqueror>().teamColor != playerTeamColor && enemy.tag.StartsWith("Player") && !alreadyHit)
            {

                enemy.GetComponentInParent<Conqueror>().m_fallingdown = false;
                if (collisionData.continuous)
                {
                    hitEnemy = "None";
                }
                else
                {
                    hitEnemy = enemy.name;
                }

                var above = false;
                
                if (collisionData.grab)
                {
                    enemy.parent = m_player;
                    enemy.GetComponentInParent<Conqueror>().isGrappled = true;
                    m_player.GetComponentInParent<Conqueror>().grabbedPlayers.Add(enemy.GetComponentInParent<Conqueror>());
                }
                else
                {
                    enemy.parent = null;
                    if (enemy.GetComponentInParent<Conqueror>() != null)
                    {
                        enemy.GetComponentInParent<Conqueror>().isGrappled = false;
                        foreach(var c in enemy.GetComponentInParent<Conqueror>().grabbedPlayers)
                        {
                            c.transform.parent = null;
                            c.isGrappled = false;
                        }
                        foreach (var c in enemy.GetComponentInParent<Conqueror>().grabbedMinions)
                        {
                            c.transform.parent = null;
                            c.isGrappled = false;
                        }
                        enemy.GetComponentInParent<Conqueror>().grabbedPlayers.Clear();
                        enemy.GetComponentInParent<Conqueror>().grabbedMinions.Clear();
                    }

                }

                //Detect impact angle
                var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                var sourceclosestPoint = new Vector2(m_player.transform.position.x, m_player.transform.position.y);
                if (sourceclosestPoint.y > targetclosestPoint.y)
                {
                    above = true;
                }

                if (collisionData.groundPound)
                {
                    playerCharacter.m_groundSensor.Disable(0.1f);
                    var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                    m_Rigidbody2D.velocity = new Vector2(0, 0);
                    m_Rigidbody2D.AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
                }

                var positionDifference = targetclosestPoint - sourceclosestPoint;

                //Must be done to detect y axis angle
                float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

                // Convert the angle to degrees.
                float attackAngle = angleInRadians * Mathf.Rad2Deg;

                if (enemy.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<Conqueror>().m_facingDirection == 1 && Mathf.Abs(attackAngle) > 90 && Mathf.Abs(attackAngle) < 225)
                {
                    enemy.GetComponentInParent<Conqueror>().Block(baseDmg);
                    m_audioManager.StopSound("Hurt");
                    m_audioManager.StopSound("Whack");
                    m_audioManager.PlaySound("Block");
                }
                else if (enemy.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<Conqueror>().m_facingDirection == -1 && (Mathf.Abs(attackAngle) < 90 || Mathf.Abs(attackAngle) > 315))
                {
                    enemy.GetComponentInParent<Conqueror>().Block(baseDmg);
                    m_audioManager.StopSound("Hurt");
                    m_audioManager.StopSound("Whack");
                    m_audioManager.PlaySound("Block");
                }

                else if (enemy.GetComponentInParent<CPUBehavior>() != null)
                {
                    //Apply damage
                    enemy.GetComponentInParent<CPUBehavior>().TakeDamage(baseDmg);
                    //Apply Knockback
                    enemy.GetComponentInParent<CPUBehavior>().incomingAngle = attackAngle;
                    enemy.GetComponentInParent<CPUBehavior>().incomingKnockback = baseKB;
                    enemy.GetComponentInParent<CPUBehavior>().incomingXMod = modifierx;
                    enemy.GetComponentInParent<CPUBehavior>().incomingYMod = (modifiery + (enemy.GetComponentInParent<Conqueror>().currentDamage / 4)) * multmodifiery;
                    enemy.GetComponentInParent<CPUBehavior>().HitStun(hitstun);

                    if (!collisionData.groundPound && !collisionData.noHitStop)
                    {
                        playerCharacter.m_isInHitStop = true;
                        playerCharacter.m_hitStopDuration = enemy.GetComponentInParent<Conqueror>().m_hitStunDuration * .8f;
                    }
                }
                else if (enemy.GetComponentInParent<Conqueror>() != null)
                {
                    //Apply damage
                    enemy.GetComponentInParent<Conqueror>().TakeDamage(baseDmg);
                    //Apply Knockback
                    enemy.GetComponentInParent<Conqueror>().incomingAngle = attackAngle;
                    enemy.GetComponentInParent<Conqueror>().incomingKnockback = baseKB;
                    enemy.GetComponentInParent<Conqueror>().incomingXMod = modifierx;
                    enemy.GetComponentInParent<Conqueror>().incomingYMod = (modifiery + (enemy.GetComponentInParent<Conqueror>().currentDamage / 4)) * multmodifiery;
                    if (collisionData.grab)
                    {
                        enemy.GetComponentInParent<Conqueror>().incomingYMod = 0;
                        enemy.GetComponentInParent<Conqueror>().incomingKnockback = 0;
                    }
                    else
                    {
                        enemy.GetComponentInParent<Conqueror>().HitStun(hitstun);
                    }
                    
                    if (!collisionData.groundPound && !collisionData.noHitStop)
                    {
                        playerCharacter.m_isInHitStop = true;
                        playerCharacter.m_hitStopDuration = enemy.GetComponentInParent<Conqueror>().m_hitStunDuration * .8f;
                    }
                }



                //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

            }
            //hit a tower
            if (enemy.GetComponentInParent<TowerEye>() != null)
            {
                enemy.GetComponentInParent<TowerEye>().TakeDamage(baseDmg);
                if (collisionData.groundPound)
                {
                    playerCharacter.m_groundSensor.Disable(0.1f);
                    var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                    m_Rigidbody2D.velocity = new Vector2(0, 0);
                    m_Rigidbody2D.AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
                }

            }
            //hit a minion
            if (enemy.GetComponentInParent<MinionBehavior>() != null && hitEnemy != enemy.name && enemy.GetComponentInParent<MinionBehavior>().teamColor != m_player.GetComponentInParent<Conqueror>().teamColor)
            {
                hitEnemy = enemy.name;
                var above = false;

                if (collisionData.grab)
                {
                    enemy.parent = m_player;
                    enemy.GetComponentInParent<MinionBehavior>().isGrappled = true;
                }
                else
                {
                    enemy.parent = null;
                    enemy.GetComponentInParent<MinionBehavior>().isGrappled = false;
                }

                //Detect impact angle
                var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                var sourceclosestPoint = new Vector2(m_player.transform.position.x, m_player.transform.position.y);
                if (sourceclosestPoint.y > targetclosestPoint.y)
                {
                    above = true;
                }

                if (collisionData.groundPound)
                {
                    playerCharacter.m_groundSensor.Disable(0.1f);
                    var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                    m_Rigidbody2D.velocity = new Vector2(0, 0);
                    m_Rigidbody2D.AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
                }


                var positionDifference = targetclosestPoint - sourceclosestPoint;

                //Must be done to detect y axis angle
                float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

                // Convert the angle to degrees.
                float attackAngle = angleInRadians * Mathf.Rad2Deg;

                //Apply damage
                enemy.GetComponentInParent<MinionBehavior>().TakeDamage(baseDmg);
                //Apply Knockback          
                enemy.GetComponentInParent<MinionBehavior>().incomingAngle = attackAngle;
                enemy.GetComponentInParent<MinionBehavior>().incomingKnockback = baseKB;
                enemy.GetComponentInParent<MinionBehavior>().incomingXMod = modifierx;
                enemy.GetComponentInParent<MinionBehavior>().incomingYMod = modifiery;
                enemy.GetComponentInParent<MinionBehavior>().HitStun(hitstun);
                //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

            }
            //hit a golem
            if (enemy.GetComponentInParent<GolemBehavior>() != null && hitEnemy != enemy.tag && transform.GetComponentInParent<GolemBehavior>() == null)
            {
                hitEnemy = enemy.tag;
                var above = false;

                if (collisionData.grab)
                {
                    enemy.parent = m_player;
                    enemy.GetComponentInParent<GolemBehavior>().isGrappled = true;
                }
                else
                {
                    enemy.parent = null;
                    enemy.GetComponentInParent<GolemBehavior>().isGrappled = false;
                }

                //Detect impact angle
                var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                var sourceclosestPoint = new Vector2(m_player.transform.position.x, m_player.transform.position.y);
                if (sourceclosestPoint.y > targetclosestPoint.y)
                {
                    above = true;
                }

                if (collisionData.groundPound)
                {
                    playerCharacter.m_groundSensor.Disable(0.1f);
                    var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                    m_Rigidbody2D.velocity = new Vector2(0, 0);
                    m_Rigidbody2D.AddForce(new Vector2(0, 9), ForceMode2D.Impulse);
                }


                var positionDifference = targetclosestPoint - sourceclosestPoint;

                //Must be done to detect y axis angle
                float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

                // Convert the angle to degrees.
                float attackAngle = angleInRadians * Mathf.Rad2Deg;

                //Apply damage
                enemy.GetComponentInParent<GolemBehavior>().TakeDamage(baseDmg);
                //Apply Knockback          
                enemy.GetComponentInParent<GolemBehavior>().incomingAngle = attackAngle;
                enemy.GetComponentInParent<GolemBehavior>().incomingKnockback = baseKB;
                enemy.GetComponentInParent<GolemBehavior>().incomingXMod = modifierx;
                enemy.GetComponentInParent<GolemBehavior>().incomingYMod = modifiery;
                enemy.GetComponentInParent<GolemBehavior>().HitStun(hitstun);
                //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

            }
        }
        //This second collisionData check is intentional. Redundancy for potential outside sources of destroying
        if (collisionData)
        {
            if (collisionData.transform.gameObject.GetComponent<ProjectileBehavior>() || collisionData.transform.gameObject.GetComponent<ArrowBehavior>())
            {
                GameObject.Destroy(collisionData.transform.gameObject);
            }
            else if (collisionData.transform.parent && (collisionData.transform.parent.gameObject.GetComponent<ProjectileBehavior>() || collisionData.transform.parent.gameObject.GetComponent<ArrowBehavior>()))
            {
                GameObject.Destroy(collisionData.transform.parent.gameObject);
            }
        }
        
    }

    public void ProjectileHit(string attackType, Transform enemy, Transform m_projectile, float baseDmg, float baseKB, float modifierx, float modifiery, float multmodifiery, float hitstun)
    {

        //hit a player
        if (enemy.GetComponentInParent<Conqueror>() != null && enemy.GetComponentInParent<Conqueror>().teamColor != m_projectile.GetComponentInParent<ProjectileBehavior>().teamColor && enemy.tag.StartsWith("Player") && hitEnemy != enemy.name)
        {

            enemy.GetComponentInParent<Conqueror>().m_fallingdown = false;
            hitEnemy = "None";

            var above = false;

                enemy.parent = null;
                if (enemy.GetComponentInParent<Conqueror>() != null)
                {
                    enemy.GetComponentInParent<Conqueror>().isGrappled = false;
                    foreach (var c in enemy.GetComponentInParent<Conqueror>().grabbedPlayers)
                    {
                        c.transform.parent = null;
                        c.isGrappled = false;
                    }
                    foreach (var c in enemy.GetComponentInParent<Conqueror>().grabbedMinions)
                    {
                        c.transform.parent = null;
                        c.isGrappled = false;
                    }
                    enemy.GetComponentInParent<Conqueror>().grabbedPlayers.Clear();
                    enemy.GetComponentInParent<Conqueror>().grabbedMinions.Clear();
                }

            //Detect impact angle
            var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            var sourceclosestPoint = new Vector2(m_projectile.transform.position.x, m_projectile.transform.position.y);
            if (sourceclosestPoint.y > targetclosestPoint.y)
            {
                above = true;
            }

            var positionDifference = targetclosestPoint - sourceclosestPoint;

            //Must be done to detect y axis angle
            float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

            // Convert the angle to degrees.
            float attackAngle = angleInRadians * Mathf.Rad2Deg;

            if (enemy.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<Conqueror>().m_facingDirection == 1 && Mathf.Abs(attackAngle) > 90 && Mathf.Abs(attackAngle) < 225)
            {
                enemy.GetComponentInParent<Conqueror>().Block(baseDmg);
                m_audioManager.StopSound("Hurt");
                m_audioManager.StopSound("Whack");
                m_audioManager.PlaySound("Block");
            }
            else if (enemy.GetComponentInParent<Conqueror>().m_animator.GetBool("isParrying") && enemy.GetComponentInParent<Conqueror>().m_facingDirection == -1 && (Mathf.Abs(attackAngle) < 90 || Mathf.Abs(attackAngle) > 315))
            {
                enemy.GetComponentInParent<Conqueror>().Block(baseDmg);
                m_audioManager.StopSound("Hurt");
                m_audioManager.StopSound("Whack");
                m_audioManager.PlaySound("Block");
            }

            else if (enemy.GetComponentInParent<CPUBehavior>() != null)
            {
                //Apply damage
                enemy.GetComponentInParent<CPUBehavior>().TakeDamage(baseDmg);
                //Apply Knockback
                enemy.GetComponentInParent<CPUBehavior>().incomingAngle = attackAngle;
                enemy.GetComponentInParent<CPUBehavior>().incomingKnockback = baseKB;
                enemy.GetComponentInParent<CPUBehavior>().incomingXMod = modifierx;
                enemy.GetComponentInParent<CPUBehavior>().incomingYMod = (modifiery + (enemy.GetComponentInParent<Conqueror>().currentDamage / 4)) * multmodifiery;
                enemy.GetComponentInParent<CPUBehavior>().HitStun(hitstun);

            }
            else if (enemy.GetComponentInParent<Conqueror>() != null)
            {
                //Apply damage
                enemy.GetComponentInParent<Conqueror>().TakeDamage(baseDmg);
                //Apply Knockback
                enemy.GetComponentInParent<Conqueror>().incomingAngle = attackAngle;
                enemy.GetComponentInParent<Conqueror>().incomingKnockback = baseKB;
                enemy.GetComponentInParent<Conqueror>().incomingXMod = modifierx;
                enemy.GetComponentInParent<Conqueror>().incomingYMod = (modifiery + (enemy.GetComponentInParent<Conqueror>().currentDamage / 4)) * multmodifiery;
                enemy.GetComponentInParent<Conqueror>().HitStun(hitstun);

            }



            //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

        }
        //hit a tower
        if (enemy.GetComponentInParent<TowerEye>() != null)
        {
            enemy.GetComponentInParent<TowerEye>().TakeDamage(baseDmg);

        }
        //hit a minion
        if (enemy.GetComponentInParent<MinionBehavior>() != null && hitEnemy != enemy.name && enemy.GetComponentInParent<MinionBehavior>().teamColor != m_projectile.GetComponentInParent<ProjectileBehavior>().teamColor)
        {
            hitEnemy = enemy.name;
            var above = false;
                enemy.parent = null;
                enemy.GetComponentInParent<MinionBehavior>().isGrappled = false;

            //Detect impact angle
            var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            var sourceclosestPoint = new Vector2(m_projectile.transform.position.x, m_projectile.transform.position.y);
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
            enemy.GetComponentInParent<MinionBehavior>().TakeDamage(baseDmg);
            //Apply Knockback          
            enemy.GetComponentInParent<MinionBehavior>().incomingAngle = attackAngle;
            enemy.GetComponentInParent<MinionBehavior>().incomingKnockback = baseKB;
            enemy.GetComponentInParent<MinionBehavior>().incomingXMod = modifierx;
            enemy.GetComponentInParent<MinionBehavior>().incomingYMod = modifiery;
            enemy.GetComponentInParent<MinionBehavior>().HitStun(hitstun);
            //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

        }
        //hit a golem
        if (enemy.GetComponentInParent<GolemBehavior>() != null && hitEnemy != enemy.tag && transform.GetComponentInParent<GolemBehavior>() == null)
        {
            hitEnemy = "none";
            var above = false;

                enemy.parent = null;
                enemy.GetComponentInParent<GolemBehavior>().isGrappled = false;
            

            //Detect impact angle
            var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            var sourceclosestPoint = new Vector2(m_projectile.transform.position.x, m_projectile.transform.position.y);
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
            enemy.GetComponentInParent<GolemBehavior>().TakeDamage(baseDmg);
            //Apply Knockback          
            enemy.GetComponentInParent<GolemBehavior>().incomingAngle = attackAngle;
            enemy.GetComponentInParent<GolemBehavior>().incomingKnockback = baseKB;
            enemy.GetComponentInParent<GolemBehavior>().incomingXMod = modifierx;
            enemy.GetComponentInParent<GolemBehavior>().incomingYMod = modifiery;
            enemy.GetComponentInParent<GolemBehavior>().HitStun(hitstun);
            //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

        }


    }

}
