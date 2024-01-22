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

    public void Hit(Transform enemy, string attackType)
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
        var animator = GetComponentInParent<Animator>();
        bool alreadyHit = false;
        alreadyHit = m_player.GetComponentInChildren<CollisionTracker>() && m_player.GetComponentInChildren<CollisionTracker>().hitEnemies.Contains(enemy.name);
        if (alreadyHit)
        {
            hitEnemy = "None";
        }
        if (m_player.GetComponentInChildren<CollisionTracker>())
        {
            m_player.GetComponentInChildren<CollisionTracker>().hitEnemies.Add(enemy.name);
        }

        if (m_player.GetComponent<ProjectileBehavior>())
        {
            ProjectileBehavior m_projectile = m_player.GetComponent<ProjectileBehavior>();
            if (enemy.GetComponentInParent<Conqueror>() != null && enemy.GetComponentInParent<Conqueror>().teamColor == m_projectile.GetComponentInParent<ProjectileBehavior>().teamColor)
            {

            }
            else if (enemy.GetComponentInParent<MinionBehavior>() != null && enemy.GetComponentInParent<MinionBehavior>().teamColor == m_projectile.GetComponentInParent<ProjectileBehavior>().teamColor)
            {

            }
            else if (enemy.GetComponentInParent<TowerEye>() != null && enemy.GetComponentInParent<TowerEye>().teamColor == m_projectile.GetComponentInParent<ProjectileBehavior>().teamColor)
            {

            }
            else
            {
                if (attackType.StartsWith("RRFSpec"))
                {
                    baseDmg = 10f;
                    baseKB = 0.2f;
                    modifiery = .5f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("MagicArrowDown"))
                {
                    baseDmg = 3.0f;
                    baseKB = 0.1f;
                    modifiery = .05f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("MagicArrowUp"))
                {
                    baseDmg = 3.0f;
                    baseKB = 0.1f;
                    modifiery = .1f;
                    m_audioManager.PlaySound("Hurt");
                }

                ProjectileHit(attackType, enemy, m_player, baseDmg, baseKB, modifierx, modifiery, multmodifiery, hitstun);
            }
        }

        else
        {

            if (enemy.GetComponentInParent<Conqueror>() != null && enemy.GetComponentInParent<Conqueror>().teamColor == m_player.GetComponentInParent<Conqueror>().teamColor)
            {

            }
            else if (enemy.GetComponentInParent<MinionBehavior>() != null && enemy.GetComponentInParent<MinionBehavior>().teamColor == m_player.GetComponentInParent<Conqueror>().teamColor)
            {

            }
            else if (enemy.GetComponentInParent<TowerEye>() != null && enemy.GetComponentInParent<TowerEye>().teamColor == m_player.GetComponentInParent<Conqueror>().teamColor)
            {

            }
            else
            {
                if (attackType.StartsWith("Jab1Hitbox"))
                {
                    baseDmg = 5.5f + playerCharacter.m_chargeModifier * 1.2f;
                    baseKB = .5f + playerCharacter.m_chargeModifier * .05f;

                    playerCharacter.m_fSmashCharging = false;
                    playerCharacter.m_chargeModifier = 0.0f;

                    m_audioManager.PlaySound("Whack");
                    playerCharacter.m_shakeIntensity = .2f;
                    playerCharacter.m_cameraShake = true;


                    hitstun = .3f;
                    modifiery = 2f;
                }
                if (attackType.StartsWith("Jab2Hitbox"))
                {
                    baseDmg = 4.2f;
                    baseKB = 0.1f;

                    modifiery = 2f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("BBDAHB"))
                {
                    baseDmg = 3.0f;
                    baseKB = .18f;
                    modifiery = .14f;
                    modifierx = .15f;
                    m_audioManager.PlaySound("BluntHit");
                }
                if (attackType.StartsWith("ChampSideSpecialBomb"))
                {
                    baseDmg = 9.5f;
                    baseKB = .6f;
                    modifiery = 2f;
                    isGrab = false;
                    playerCharacter.m_shakeIntensity = .2f;

                    playerCharacter.m_cameraShake = true;
                    if (enemy.GetComponentInParent<Conqueror>() != null)
                    {
                        enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                        enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                    }
                    m_audioManager.PlaySound("Hurt");

                }
                if (attackType.StartsWith("ChampSideSpecHB"))
                {
                    baseDmg = 3.0f;
                    baseKB = 0.0f;
                    multmodifiery = 0.0f;
                    isGrab = true;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("BBDair"))
                {
                    baseDmg = 3f;
                    baseKB = 0.3f;

                    modifiery = -5f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("BBDTilt"))
                {
                    baseDmg = 3f;
                    baseKB = .1f;

                    modifiery = 2.2f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("BBFairSpike"))
                {
                    baseDmg = 6.5f;
                    baseKB = .5f;

                    hitstun = .3f;
                    modifiery = 8f;
                    multmodifiery = -1;
                    playerCharacter.m_shakeIntensity = .22f;
                    playerCharacter.m_cameraShake = true;
                    m_audioManager.PlaySound("Hurt");
                }
                else if (attackType.StartsWith("BBFair"))
                {
                    baseDmg = 6.5f;
                    baseKB = .5f;

                    hitstun = .3f;
                    modifiery = 6f;
                    playerCharacter.m_shakeIntensity = .2f;
                    playerCharacter.m_cameraShake = true;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("BBDSmash"))
                {
                    baseDmg = 8.2f + playerCharacter.m_chargeModifier * 1.2f;
                    baseKB = 0.5f + playerCharacter.m_chargeModifier * .05f;

                    playerCharacter.m_dSmashCharging = false;
                    playerCharacter.m_chargeModifier = 0.0f;
                    m_audioManager.PlaySound("Whack");
                    playerCharacter.m_shakeIntensity = .2f;
                    playerCharacter.m_cameraShake = true;

                    modifiery = 3f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("DSpecFire"))
                {
                    baseDmg = 3f;
                    baseKB = 0.08f;

                    modifiery = 1f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("NSpecFire"))
                {
                    baseDmg = .5f;
                    baseKB = 0.0f;

                    m_audioManager.PlaySound("Hurt");
                    modifiery = 0f;
                }
                if (attackType.StartsWith("BBDSpecHB"))
                {
                    baseDmg = 4.5f;
                    baseKB = 0.3f;

                    modifiery = 4f;
                    playerCharacter.m_cameraShake = true;
                    playerCharacter.m_shakeIntensity = .22f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("BBUair"))
                {
                    baseDmg = 5.0f;
                    baseKB = 0.2f;

                    modifiery = 4f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("BBUSmash"))
                {
                    baseDmg = 8.5f + playerCharacter.m_chargeModifier * 1.2f;
                    baseKB = 0.5f + playerCharacter.m_chargeModifier * .05f;

                    playerCharacter.m_uSmashCharging = false;
                    playerCharacter.m_chargeModifier = 0.0f;
                    m_audioManager.PlaySound("Whack");
                    playerCharacter.m_cameraShake = true;
                    playerCharacter.m_shakeIntensity = .2f;
                    modifiery = 6.5f;

                }
                if (attackType.StartsWith("BBUTilt"))
                {
                    baseDmg = 3.5f;
                    baseKB = 0.2f;
                    m_audioManager.PlaySound("Hurt");
                    modifiery = 6.5f;
                }
                if (attackType.StartsWith("ProtoJab1"))
                {
                    baseDmg = 3.0f;
                    baseKB = .2f;
                    modifiery = 1f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("DPDash"))
                {
                    baseDmg = 3.3f;
                    baseKB = .23f;
                    modifiery = 1.5f;
                    m_audioManager.PlaySound("BluntHit");
                }
                if (attackType.StartsWith("ProtoUTilt"))
                {
                    baseDmg = 3.2f;
                    baseKB = 0.3f;
                    modifiery = 3f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.Contains("DTilt"))
                {
                    baseDmg = 2f;
                    baseKB = 0.2f;
                    modifiery = 3f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.Contains("ProtoDSpec"))
                {
                    baseDmg = 1.2f;
                    baseKB = 0.2f;
                    modifiery = 4f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("ProtoDair"))
                {
                    baseDmg = 4.0f;
                    baseKB = 0.5f;

                    modifiery = -3f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("ProtoSideSpec1"))
                {
                    baseDmg = 3.4f;
                    baseKB = 0.0f;
                    hitstun = .28f;
                    m_audioManager.PlaySound("Hurt");
                }
                if (attackType.StartsWith("ProtoSideSpec2"))
                {
                    baseDmg = 3.3f;
                    baseKB = 0.1f;
                    modifiery = 8f;
                    m_audioManager.PlaySound("Hurt");

                    if (m_player.GetComponent<PrototypeHero>().currentHookTarget == null)
                    {
                        GameObject.Instantiate(crosshair, enemy);
                        m_player.GetComponent<PrototypeHero>().currentHookTarget = enemy.gameObject;
                    }
                    
                }
                if (attackType.StartsWith("FSmash"))
                {
                    baseDmg = 5f + playerCharacter.m_chargeModifier * 1.1f;
                    baseKB = .7f + playerCharacter.m_chargeModifier * .05f;

                    playerCharacter.m_fSmashCharging = false;
                    playerCharacter.m_chargeModifier = 0.0f;
                    m_audioManager.PlaySound("ProxyHeavy");
                    playerCharacter.m_shakeIntensity = .2f;

                    playerCharacter.m_cameraShake = true;
                    if (enemy.GetComponentInParent<Conqueror>() != null)
                    {
                        enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                        enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                    }

                    //hitstun = .31f;
                    modifiery = 2f;
                }
                if (attackType.StartsWith("USmash"))
                {
                    baseDmg = 4.8f + playerCharacter.m_chargeModifier * 1.1f;
                    baseKB = .5f + playerCharacter.m_chargeModifier * .05f;

                    playerCharacter.m_uSmashCharging = false;
                    playerCharacter.m_chargeModifier = 0.0f;
                    m_audioManager.PlaySound("ProxyHeavy");
                    playerCharacter.m_cameraShake = true;
                    if (enemy.GetComponentInParent<Conqueror>() != null)
                    {
                        enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                        enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                    }
                    playerCharacter.m_shakeIntensity = .15f;
                    //hitstun = .31f;
                    modifiery = 6f;
                }
                if (attackType.StartsWith("DSmash"))
                {
                    baseDmg = 5f + playerCharacter.m_chargeModifier * 1.1f;
                    baseKB = .6f + playerCharacter.m_chargeModifier * .05f;

                    playerCharacter.m_dSmashCharging = false;
                    modifiery = 6f;
                    playerCharacter.m_chargeModifier = 0.0f;
                    m_audioManager.PlaySound("ProxyHeavy");
                    if (enemy.GetComponentInParent<Conqueror>() != null)
                    {
                        enemy.GetComponentInParent<Conqueror>().m_shakeIntensity = .2f;
                        enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                    }
                    enemy.GetComponentInParent<Conqueror>().m_cameraShake = true;
                    hitstun = .28f;

                }
                if (attackType.StartsWith("ChargeBall"))
                {
                    baseDmg = 3.0f;
                    baseKB = 0.1f;
                    modifiery = .1f;
                    m_audioManager.PlaySound("Hurt");
                }
                
                if (attackType.StartsWith("RRJabHB"))
                {
                    baseDmg = 3.2f;
                    baseKB = 0.14f;
                    modifiery = .2f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRUptilt"))
                {
                    baseDmg = 3.2f;
                    baseKB = 0.12f;
                    modifiery = .3f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRFSmash"))
                {
                    baseDmg = 6.5f + playerCharacter.m_chargeModifier * 1.1f;
                    baseKB = .4f + playerCharacter.m_chargeModifier * .05f;
                    modifiery = .15f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRForwardAir"))
                {
                    baseDmg = 3.1f;
                    baseKB = .16f;
                    modifiery = .2f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRNair"))
                {
                    baseDmg = 3.4f;
                    baseKB = .11f;
                    modifiery = .2f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRUpAir"))
                {
                    baseDmg = 3.4f;
                    baseKB = .11f;
                    modifiery = .2f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRDownAir"))
                {
                    baseDmg = 3.1f;
                    baseKB = .08f;
                    modifiery = -.5f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRDownSmash"))
                {
                    baseDmg = 6.3f + playerCharacter.m_chargeModifier * 1.1f;
                    baseKB = .35f + playerCharacter.m_chargeModifier * .05f;
                    modifiery = .05f;
                    modifierx = .1f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRDownTilt"))
                {
                    baseDmg = 2.0f;
                    baseKB = .1f;
                    modifiery = .15f;
                    modifierx = .1f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("RRDAHB"))
                {
                    baseDmg = 2.0f;
                    baseKB = .15f;
                    modifiery = .15f;
                    modifierx = .1f;
                    m_audioManager.PlaySound("MagicHit");
                }
                if (attackType.StartsWith("GolemFlick"))
                {
                    baseDmg = 7f;
                    baseKB = 1f;
                    modifiery = 15f;
                    m_audioManager.PlaySound("GolemHit");
                }
            }


            //golem attack
            if (m_player.GetComponentInParent<GolemBehavior>())
            {
                GolemBehavior golemBehavior = m_player.GetComponentInParent<GolemBehavior>();
                hitEnemy = enemy.name;

                var above = false;

                if (isGrab)
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

            //hit a playera
            else if (enemy.GetComponentInParent<Conqueror>() != null && enemy.GetComponentInParent<Conqueror>().teamColor != m_player.GetComponentInParent<Conqueror>().teamColor && enemy.tag.StartsWith("Player") && hitEnemy != enemy.name && !alreadyHit)
            {

                enemy.GetComponentInParent<Conqueror>().m_fallingdown = false;
                if (attackType.StartsWith("NSpecFire") || attackType.StartsWith("ChargeBall") || attackType.StartsWith("RR") || attackType.StartsWith("MagicArrow"))
                {
                    hitEnemy = "None";
                }
                else
                {
                    hitEnemy = enemy.name;
                }

                var above = false;
                
                if (isGrab)
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

                if (attackType.StartsWith("ProtoDair"))
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

                    if (!attackType.StartsWith("ProtoDair") && !attackType.StartsWith("ProtoDSpec"))
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
                    enemy.GetComponentInParent<Conqueror>().HitStun(hitstun);
                    if (!attackType.StartsWith("ProtoDair") && !attackType.StartsWith("ProtoDSpec"))
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
                if (attackType.StartsWith("ProtoDair"))
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

                if (isGrab)
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

                if (attackType.StartsWith("ProtoDair"))
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

                if (isGrab)
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

                if (attackType.StartsWith("ProtoDair"))
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
