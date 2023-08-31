using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class CPUHotZone : MonoBehaviour
{
    private EnemyBehavior enemyParent;
    private CPUBehavior CPUParent;
    private MinionBehavior MinionParent;
    private GolemBehavior GolemParent;
    private bool inRange;
    private Animator anim;

    private void Awake()
    {
        enemyParent = GetComponentInParent<EnemyBehavior>();

        CPUParent = GetComponentInParent<CPUBehavior>();

        GolemParent = GetComponentInParent<GolemBehavior>();

        if (CPUParent == null)
        {
            MinionParent = GetComponentInParent<MinionBehavior>();
        }

        anim = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (CPUParent != null)
        {
            if (inRange && CPUParent.m_timeSinceAttack > 0.2f)
            {
                if (enemyParent != null)
                {
                    enemyParent.Flip();
                }
                if (CPUParent != null)
                {
                    CPUParent.Flip();
                }
            }
        }
        else if (MinionParent != null)
        {
            MinionParent.Flip();
        }
        else if (GolemParent != null)
        {
            GolemParent.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name.StartsWith("PrototypeHeroCPU") || collider.gameObject.name.StartsWith("RedSwordMinion") || collider.gameObject.name.Contains("Golem"))
        {
            //do nothing

        }
        else if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("PlayerMid"))
        {
            inRange = true;
            
            if (enemyParent != null)
            {
                CPUParent.inRange = true;
                enemyParent.target = collider.transform;
            }
            if (CPUParent != null)
            {
                CPUParent.inRange = true;
                CPUParent.target = collider.transform;
            }
            if (MinionParent != null)
            {
                MinionParent.inRange = true;
                MinionParent.target = collider.transform;
            }
            if (GolemParent != null)
            {
                GolemParent.inRange = true;
                GolemParent.target = collider.transform;
            }

        }
        else if (collider.gameObject.CompareTag("EyeTarget"))
        {
            inRange = true;

            if (enemyParent != null)
            {
                CPUParent.inRange = true;
                enemyParent.target = collider.transform;
            }
            if (CPUParent != null)
            {
                CPUParent.inRange = true;
                CPUParent.target = collider.transform;
            }
            if (MinionParent != null)
            {
                MinionParent.inRange = true;
                MinionParent.target = collider.transform;
            }

        }

    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && CPUParent != null)
        {
            inRange = false;
            //gameObject.SetActive(false);

            CPUParent.triggerArea.SetActive(true);
            CPUParent.inRange = false;
            CPUParent.SelectTarget();
        }
        else if (collider.gameObject.CompareTag("Player")  && MinionParent != null)
        {
            inRange = false;
            //gameObject.SetActive(false);

            MinionParent.triggerArea.SetActive(true);
            MinionParent.inRange = false;
            MinionParent.SelectTarget();
        }
        else if (collider.gameObject.CompareTag("PlayerMid") && GolemParent != null)
        {
            inRange = false;
            //gameObject.SetActive(false);

            GolemParent.triggerArea.SetActive(true);
            GolemParent.inRange = false;
            GolemParent.SelectTarget();
        }
        else if (collider.gameObject.CompareTag("EyeTarget") && MinionParent != null)
        {
            inRange = false;
            //gameObject.SetActive(false);

            MinionParent.triggerArea.SetActive(true);
            MinionParent.inRange = false;
            MinionParent.SelectTarget();
        }
    }
}
