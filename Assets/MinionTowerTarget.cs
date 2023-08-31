using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionTowerTarget : MonoBehaviour
{
    public GameObject TowerEye;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.GetComponentInParent<ChampJab1>() != null)
        //{
        //    collision.GetComponentInParent<ChampJab1>().Hit(transform);
        //}
        if (collision.transform.tag == "HotZone")
        {
            //do nothing
        }
        else if (collision.transform.tag == "AttackHitbox" && collision.GetComponentInParent<MinionBehavior>())
        {
            collision.GetComponentInParent<CombatManager>().Hit(TowerEye.transform, collision.transform.name);
        }
    }
}
