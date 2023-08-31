using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public string hitEnemy;
    private AudioManager_PrototypeHero m_audioManager;

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
        float hitstun = .25f;
        bool isGrab = false;

        m_audioManager = AudioManager_PrototypeHero.instance;

        Conqueror playerCharacter = GetComponentInParent<Conqueror>();
        

        var animator = GetComponentInParent<Animator>();
        

        if (attackType.StartsWith("Jab1Hitbox"))
        {
            baseDmg = 4.5f + playerCharacter.m_chargeModifier * 2.5f;
            baseKB = .5f  + playerCharacter.m_chargeModifier * .05f;

            playerCharacter.m_fSmashCharging = false;
            playerCharacter.m_chargeModifier = 0.0f;
            
            m_audioManager.PlaySound("Whack");
            playerCharacter.m_shakeIntensity = .2f;
            playerCharacter.m_cameraShake = true;
            

            hitstun = .35f;
            modifiery = 2f;
        }
        if (attackType.StartsWith("Jab2Hitbox"))
        {
            baseDmg = 3.2f;
            baseKB = 0.1f;

            modifiery = 2f;
        }
        if (attackType.StartsWith("ChampSideSpecialBomb"))
        {
            baseDmg = 8.0f;
            baseKB = .6f;
            modifiery = 2f;
            isGrab = false;
            playerCharacter.m_shakeIntensity = .22f;
            playerCharacter.m_cameraShake = true;
            
        }
        if (attackType.StartsWith("ChampSideSpecHB"))
        {
            baseDmg = 5.0f;
            baseKB = 0.0f;
            multmodifiery = 0.0f;
            isGrab = true;
        }
        if (attackType.StartsWith("BBDair"))
        {
            baseDmg = 3f;
            baseKB = 0.3f;

            modifiery = -5f;
        }
        if (attackType.StartsWith("BBDTilt"))
        {
            baseDmg = 3f;
            baseKB = .1f;

            modifiery = 2.2f;
        }
        if (attackType.StartsWith("BBFair"))
        {
            baseDmg = 6.5f;
            baseKB = .5f;

            hitstun = .35f;
            modifiery = -6f;
            playerCharacter.m_shakeIntensity = .2f;
            playerCharacter.m_cameraShake = true;
            
        }
        if (attackType.StartsWith("BBDSmash"))
        {
            baseDmg = 8.2f + playerCharacter.m_chargeModifier * 2.5f;
            baseKB = 0.5f + playerCharacter.m_chargeModifier * .05f;

            playerCharacter.m_dSmashCharging = false;
            playerCharacter.m_chargeModifier = 0.0f;
            m_audioManager.PlaySound("Whack");
            playerCharacter.m_shakeIntensity = .2f;
            playerCharacter.m_cameraShake = true;
            
            modifiery = 3f;
        }
        if (attackType.StartsWith("DSpecFire"))
        {
            baseDmg = 1.5f;
            baseKB = 0.08f;

            modifiery = 1f;
        }
        if (attackType.StartsWith("NSpecFire"))
        {
            baseDmg = .5f;
            baseKB = 0.0f;

            modifiery = 0f;
        }
        if (attackType.StartsWith("BBDSpecHB"))
        {
            baseDmg = 4.5f;
            baseKB = 0.3f;

            modifiery = 4f;
            playerCharacter.m_cameraShake = true;
            playerCharacter.m_shakeIntensity = .22f;
        }
        if (attackType.StartsWith("BBUair"))
        {
            baseDmg = 5.0f;
            baseKB = 0.2f;

            modifiery = 4f;
        }
        if (attackType.StartsWith("BBUSmash"))
        {
            baseDmg = 8.5f + playerCharacter.m_chargeModifier * 2.5f; 
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
            modifiery = 6.5f;
        }
        if (attackType.StartsWith("ProtoJab1"))
        {
            baseDmg = 3.0f;
            baseKB = .2f;
            modifiery = 1f;
        }
        if (attackType.StartsWith("ProtoUTilt"))
        {
            baseDmg = 3.2f;
            baseKB = 0.3f;
            modifiery = 3f;
        }
        if (attackType.Contains("DTilt"))
        {
            baseDmg = 2f;
            baseKB = 0.2f;
            modifiery = 3f;
        }
        if (attackType.Contains("ProtoDSpec"))
        {
            baseDmg = 1.2f;
            baseKB = 0.2f;
            modifiery = 4f;
        }
        if (attackType.StartsWith("ProtoDair"))
        {
            baseDmg = 4.0f;
            baseKB = 0.5f;

            modifiery = -3f;
        }
        if (attackType.StartsWith("ProtoSideSpec1"))
        {
            baseDmg = 3.4f;
            baseKB = 0.0f;
            hitstun = .31f;
        }
        if (attackType.StartsWith("ProtoSideSpec2"))
        {
            baseDmg = 3.3f;
            baseKB = 0.1f;
            modifiery = 8f;
        }
        if (attackType.StartsWith("FSmash"))
        {
            baseDmg = 5f + playerCharacter.m_chargeModifier * 2.5f;
            baseKB = .5f + playerCharacter.m_chargeModifier * .05f;

            playerCharacter.m_fSmashCharging = false;
            playerCharacter.m_chargeModifier = 0.0f;
            m_audioManager.PlaySound("Whack");
            playerCharacter.m_cameraShake = true;
            playerCharacter.m_shakeIntensity = .15f;
            hitstun = .31f;
            modifiery = 2f;
        }
        if (attackType.StartsWith("USmash"))
        {
            baseDmg = 4.8f + playerCharacter.m_chargeModifier * 2.5f;
            baseKB = .5f + playerCharacter.m_chargeModifier * .05f;

            playerCharacter.m_uSmashCharging = false;
            playerCharacter.m_chargeModifier = 0.0f;
            m_audioManager.PlaySound("Whack");
            playerCharacter.m_cameraShake = true;
            playerCharacter.m_shakeIntensity = .15f;
            hitstun = .31f;
            modifiery = 6f;
        }
        if (attackType.StartsWith("DSmash"))
        {
            baseDmg = 5f + playerCharacter.m_chargeModifier * 2.5f;
            baseKB = .6f + playerCharacter.m_chargeModifier * .05f;

            playerCharacter.m_dSmashCharging = false;
            modifiery = 6f;
            playerCharacter.m_chargeModifier = 0.0f;
            m_audioManager.PlaySound("Whack");
            playerCharacter.m_cameraShake = true;
            playerCharacter.m_shakeIntensity = .15f;
            hitstun = .31f;

        }
        if (attackType.StartsWith("GolemFlick"))
        {
            baseDmg = 7f;
            baseKB = 1f;
            modifiery = 15f;
            m_audioManager.PlaySound("GolemHit");
        }



        var m_player = transform;

        
        //hit a player
        if ((enemy.GetComponentInParent<CPUBehavior>() != null || (enemy.GetComponentInParent<Conqueror>() != null && (transform.GetComponentInParent<CPUBehavior>() != null || transform.GetComponentInParent<MinionBehavior>() != null))) && enemy.tag.StartsWith("Player") && hitEnemy != enemy.tag)
        {
            if (attackType.StartsWith("NSpecFire"))
            {
                hitEnemy = "None";
            }
            else
            {
                hitEnemy = enemy.tag;
            }
            
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

            if (attackType.StartsWith("ProtoDair"))
            {
                var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                m_Rigidbody2D.velocity = new Vector2(0, 0);
                m_Rigidbody2D.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
            }

            var positionDifference = targetclosestPoint - sourceclosestPoint;

            //Must be done to detect y axis angle
            float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);

            // Convert the angle to degrees.
            float attackAngle = angleInRadians * Mathf.Rad2Deg;

            if (enemy.GetComponentInParent<CPUBehavior>() != null)
            {
                //Apply damage
                enemy.GetComponentInParent<CPUBehavior>().TakeDamage(baseDmg);
                //Apply Knockback
                enemy.GetComponentInParent<CPUBehavior>().incomingAngle = attackAngle;
                enemy.GetComponentInParent<CPUBehavior>().incomingKnockback = baseKB;
                enemy.GetComponentInParent<CPUBehavior>().incomingXMod = modifierx;
                enemy.GetComponentInParent<CPUBehavior>().incomingYMod = (modifiery + (enemy.GetComponentInParent<Conqueror>().currentDamage / 4)) * multmodifiery;
                enemy.GetComponentInParent<CPUBehavior>().HitStun(hitstun);

                playerCharacter.m_isInHitStop = true;
                playerCharacter.m_hitStopDuration = enemy.GetComponentInParent<CPUBehavior>().m_hitStunDuration * .8f;
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

                playerCharacter.m_isInHitStop = true;
                playerCharacter.m_hitStopDuration = enemy.GetComponentInParent<Conqueror>().m_hitStunDuration * .8f;
            }
            

            if (attackType.Contains("Smash"))
            {
                Instantiate(playerCharacter.HeavyAttackFX, new Vector3((targetclosestPoint.x), targetclosestPoint.y, transform.position.z),
            new Quaternion(0f, 0f, 0f, 0f), m_player.transform);
            }
            //enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

        }
        //hit a tower
        if (enemy.GetComponentInParent<TowerEye>() != null)
        {
            enemy.GetComponentInParent<TowerEye>().TakeDamage(baseDmg);
            if (attackType.StartsWith("ProtoDair"))
            {
                var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                m_Rigidbody2D.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
            }

        }
        //hit a minion
        if (enemy.GetComponentInParent<MinionBehavior>() != null && hitEnemy != enemy.tag)
        {
            hitEnemy = enemy.tag;
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
                var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                m_Rigidbody2D.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
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
                var m_Rigidbody2D = GetComponent<Rigidbody2D>();
                m_Rigidbody2D.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
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
