using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class HotZoneCheck : MonoBehaviour
{
    private EnemyBehavior enemyParent;
    private CPUCharacter2D CPUParent;
    private bool inRange;
    private Animator anim;

    private void Awake()
    {
        enemyParent = GetComponentInParent<EnemyBehavior>();
        
        CPUParent = GetComponentInParent<CPUCharacter2D>();
        
        anim = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if(inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("skels_attack"))
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
        if (collider.gameObject.CompareTag("Player"))
        {
            inRange = true;
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
            gameObject.SetActive(false);
            
            if (enemyParent != null)
            {
                enemyParent.triggerArea.SetActive(true);
                enemyParent.inRange = false;
                enemyParent.SelectTarget();
            }
            if (CPUParent != null)
            {
                CPUParent.triggerArea.SetActive(true);
                CPUParent.inRange = false;
                CPUParent.SelectTarget();
            }

        }
    }
}
