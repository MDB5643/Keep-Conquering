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
        MinionBehavior minion = GetComponentInParent<MinionBehavior>();
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
                if (enemy.GetComponentInParent<Conqueror>())
                {
                    enemy.GetComponent<Conqueror>().m_shakeIntensity = collisionData.shakeIntensity;
                    enemy.GetComponent<Conqueror>().m_cameraShake = collisionData.shakeScreen;
                }
                

                m_audioManager.PlaySound(collisionData.soundName);

                hitstun = collisionData.hitStun;
                modifierx = collisionData.modifierX;
                modifiery = collisionData.modifierY;
            }
        }
        else if (m_player.GetComponentInChildren<CollisionTracker>())
        {
            if (playerCharacter)
            {
                if (!alreadyHit)
                {
                    collisionData.hitEnemies.Add(enemy.name);
                }
                
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
            else if (minion)
            {
                if (!alreadyHit)
                {
                    collisionData.hitEnemies.Add(enemy.name);
                }
                playerTeamColor = minion.teamColor;
                baseDmg = collisionData.baseDamage;
                baseKB = collisionData.baseKB;

                m_audioManager.PlaySound(collisionData.soundName);

                hitstun = collisionData.hitStun;
                modifierx = collisionData.modifierX;
                modifiery = collisionData.modifierY;
            }
            

        }

        if (collisionData && !alreadyHit)
        {
            
            if (collisionData.transform.name.StartsWith("ProtoSideSpec2HB") && m_player.GetComponent<PrototypeHero>().currentHookTarget == null)
            {
                GameObject.Instantiate(crosshair, enemy);
                m_player.GetComponent<PrototypeHero>().currentHookTarget = enemy.gameObject;
            }

            else if (collisionData.transform.name.StartsWith("ROUpSpec"))
            {
                if (collisionData.transform.name.StartsWith("ROUpSpecHB2"))
                {
                    if (enemy.GetComponentInParent<Conqueror>())
                    {
                        enemy.GetComponentInParent<Conqueror>().transform.parent = null;
                    }
                    if (enemy.position.z == 20)
                    {
                        enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("PlayerMid"));
                    }
                    else
                    {
                        enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("Player"));
                    }
                    multmodifiery = collisionData.multModY;
                }
                else if (enemy.GetComponentInParent<Conqueror>())
                {
                    m_player.GetComponentInParent<Conqueror>().m_groundSensor.Disable(0.25f);

                    if (enemy.position.z == 20)
                    {
                        enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("Grabbed"));
                    }
                    else
                    {
                        enemy.GetComponentInParent<Conqueror>().SetLayerRecursively(enemy.gameObject, LayerMask.NameToLayer("Grabbed"));
                    }
                    if (m_player.position.z == 20)
                    {
                        m_player.GetComponentInParent<Conqueror>().SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("Grabbed"));
                    }
                    else
                    {
                        m_player.GetComponentInParent<Conqueror>().SetLayerRecursively(m_player.gameObject, LayerMask.NameToLayer("Grabbed"));
                    }

                    enemy.SetPositionAndRotation(new Vector3(m_player.transform.position.x, m_player.transform.position.y, m_player.transform.position.z), new Quaternion(0f, 0f, 0f, 0f));

                }
                
            }


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
                    enemy.GetComponentInParent<Conqueror>().TakeDamage(baseDmg, collisionData.noFlinch);
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

                
                else if (enemy.GetComponentInParent<Conqueror>() != null)
                {
                    //Apply damage
                    enemy.GetComponentInParent<Conqueror>().TakeDamage(baseDmg, collisionData.noFlinch);
                    //Apply Knockback
                    enemy.GetComponentInParent<Conqueror>().incomingAngle = attackAngle;
                    enemy.GetComponentInParent<Conqueror>().incomingKnockback = baseKB;
                    enemy.GetComponentInParent<Conqueror>().incomingXMod = modifierx;
                    enemy.GetComponentInParent<Conqueror>().incomingYMod = (modifiery + (enemy.GetComponentInParent<Conqueror>().currentDamage / 6.5f)) * multmodifiery;
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
                        if (playerCharacter)
                        {
                            playerCharacter.m_isInHitStop = true;
                            playerCharacter.m_hitStopDuration = enemy.GetComponentInParent<Conqueror>().m_hitStunDuration * .75f;
                        }
                        else if (minion)
                        {
                            enemy.GetComponentInParent<Conqueror>().incomingYMod = 0;
                            enemy.GetComponentInParent<Conqueror>().incomingKnockback = .0f;
                            minion.m_isInHitStop = true;
                            minion.m_hitStopDuration = enemy.GetComponentInParent<Conqueror>().m_hitStunDuration;
                        }
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
            if (enemy.GetComponentInParent<MinionBehavior>() != null && hitEnemy != enemy.name && 
                ((m_player.GetComponentInParent<Conqueror>() && (enemy.GetComponentInParent<MinionBehavior>().teamColor != m_player.GetComponentInParent<Conqueror>().teamColor)) || 
                (m_player.GetComponentInParent<MinionBehavior>() && (enemy.GetComponentInParent<MinionBehavior>().teamColor != m_player.GetComponentInParent<MinionBehavior>().teamColor))))
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

                var midPointX = (targetclosestPoint.x + sourceclosestPoint.x) / 2f;
                var midPointY = (targetclosestPoint.y + sourceclosestPoint.y) / 2f;

                Instantiate(enemy.GetComponentInParent<MinionBehavior>().LightAttackFX, new Vector3(midPointX, midPointY, transform.position.z),
                new Quaternion(0f, 0f, 0f, 0f), transform);

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
            if (collisionData.transform.name.StartsWith("ROUpSpecHB2"))
            {

            }
            else if (collisionData.transform.name.StartsWith("ROUpSpecHB"))
            {
                GameObject.Destroy(collisionData.transform.gameObject);
            }
            if (collisionData.transform.gameObject.GetComponent<ProjectileBehavior>())
            {
                if ((collisionData.transform.name.StartsWith("MagicArrow") && collisionData.transform.gameObject.GetComponent<ProjectileBehavior>().charged) || collisionData.transform.name.StartsWith("RRFSpecExplode"))
                {

                }
                else
                {
                    GameObject.Destroy(collisionData.transform.gameObject);
                }
                
            }
            else if (collisionData.transform.parent && (collisionData.transform.parent.gameObject.GetComponent<ProjectileBehavior>() || collisionData.transform.parent.gameObject.GetComponent<ArrowBehavior>()))
            {
                if ((collisionData.transform.name.StartsWith("MagicArrow") && collisionData.transform.gameObject.GetComponent<ProjectileBehavior>().charged) || collisionData.transform.name.StartsWith("RRFSpecExplode"))
                {

                }
                else
                {
                    GameObject.Destroy(collisionData.transform.gameObject);
                }
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

            else if (enemy.GetComponentInParent<Conqueror>() != null)
            {
                //Apply damage
                enemy.GetComponentInParent<Conqueror>().TakeDamage(baseDmg, false);
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
