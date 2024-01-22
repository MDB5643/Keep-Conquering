using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampJab1 : MonoBehaviour
{
    private PolygonCollider2D pCollider;
    private float lingerTime = 0.0f;
    private float maxLingerTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lingerTime += Time.deltaTime;
        if (lingerTime >= maxLingerTime && gameObject != null)
        {
            GameObject.Destroy(gameObject);
        }
        pCollider = transform.GetComponent<PolygonCollider2D>();
    }

    //public void Hit(Transform enemy)
    //{
    //    var m_player = transform.GetComponentInParent<TheChampion>();
    //
    //    if (enemy.GetComponentInParent<CPUBehavior>() != null && enemy.tag.StartsWith("Player"))
    //    {
    //        var above = false;
    //
    //        //Detect impact angle
    //        var targetclosestPoint = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
    //        var sourceclosestPoint = new Vector2(m_player.transform.position.x, m_player.transform.position.y);
    //        if (sourceclosestPoint.y > targetclosestPoint.y)
    //        {
    //            above = true;
    //        }
    //
    //        var positionDifference = targetclosestPoint - sourceclosestPoint;
    //
    //        //Must be done to detect y axis angle
    //        float angleInRadians = Mathf.Atan2(positionDifference.y, positionDifference.x);
    //
    //        // Convert the angle to degrees.
    //        float attackAngle = angleInRadians * Mathf.Rad2Deg;
    //
    //        //Apply damage
    //        enemy.GetComponentInParent<CPUBehavior>().TakeDamage(4.5f);
    //        //Apply Knockback
    //        enemy.GetComponentInParent<CPUBehavior>().Knockback(1.0f, attackAngle, 0, 2, above);
    //
    //    }
    //}
}
