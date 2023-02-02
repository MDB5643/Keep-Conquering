using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class TriggerAreaCheck : MonoBehaviour
{
    private EnemyBehavior enemyParent;
    private CPUCharacter2D CPUParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<EnemyBehavior>();
        CPUParent = GetComponentInParent<CPUCharacter2D>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            if (enemyParent != null)
            {
                enemyParent.target = collider.transform;
                enemyParent.inRange = true;
                enemyParent.hotZone.SetActive(true);
            }
            if (CPUParent != null)
            {
                CPUParent.target = collider.transform;
                CPUParent.inRange = true;
                CPUParent.hotZone.SetActive(true);
            }

        }
    }
}
