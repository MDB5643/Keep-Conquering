using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class CPUHotZone : MonoBehaviour
{
    private EnemyBehavior enemyParent;
    private CPUBehavior CPUParent;
    private bool inRange;
    private Animator anim;

    private void Awake()
    {
        enemyParent = GetComponentInParent<EnemyBehavior>();

        CPUParent = GetComponentInParent<CPUBehavior>();

        anim = GetComponentInParent<Animator>();
    }

    private void Update()
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name.StartsWith("CPU"))
        {
            //do nothing

        }
        else if (collider.gameObject.CompareTag("Player"))
        {
            inRange = true;
            CPUParent.inRange = true;
            if (enemyParent != null)
            {
                enemyParent.target = collider.transform;
            }
            if (CPUParent != null)
            {
                CPUParent.target = collider.transform;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            inRange = false;
            //gameObject.SetActive(false);

            CPUParent.triggerArea.SetActive(true);
            CPUParent.inRange = false;
            CPUParent.SelectTarget();
            

        }
    }
}
