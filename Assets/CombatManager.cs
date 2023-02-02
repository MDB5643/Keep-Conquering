using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public string hitEnemy;

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
        float modifiery = 2.0f;
        bool isGrab = false;

        if (attackType.StartsWith("Jab1Hitbox"))
        {
            baseDmg = 4.5f;
            baseKB = 1.0f;
        }
        if (attackType.StartsWith("Jab2Hitbox"))
        {
            baseDmg = 3.2f;
            baseKB = 0.5f;
        }
        if (attackType.StartsWith("ChampSideSpecialBomb"))
        {
            baseDmg = 8.0f;
            baseKB = 2f;

            isGrab = false;
        }
        if (attackType.StartsWith("ChampSideSpecHB"))
        {
            baseDmg = 8.0f;
            baseKB = 0.0f;

            isGrab = true;
        }
        if (attackType.StartsWith("ProtoJab1"))
        {
            baseDmg = 3.0f;
            baseKB = .5f;
        }
        if (attackType.StartsWith("ProtoUTilt"))
        {
            baseDmg = 3.2f;
            baseKB = 0.6f;
        }
        if (attackType.StartsWith("ProtoDair"))
        {
            baseDmg = 4.0f;
            baseKB = 0.7f;
        }


        var m_player = transform;

        

        if (enemy.GetComponentInParent<CPUBehavior>() != null && enemy.tag.StartsWith("Player") && hitEnemy != enemy.tag)
        {
            hitEnemy = enemy.tag;
            var above = false;

            if (isGrab)
            {
                enemy.parent = m_player;
                enemy.GetComponentInParent<CPUBehavior>().isGrappled = true;
            }
            else
            {
                enemy.parent = null;
                enemy.GetComponentInParent<CPUBehavior>().isGrappled = false;
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

            //Apply damage
            enemy.GetComponentInParent<CPUBehavior>().TakeDamage(baseDmg);
            //Apply Knockback
            enemy.GetComponentInParent<CPUBehavior>().Knockback(baseKB, attackAngle, modifierx, modifiery, above);

        }
    }
}
