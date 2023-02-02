using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownAttackCollisions : MonoBehaviour
{
    PrototypeHero playerParent;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        playerParent = GetComponentInParent<PrototypeHero>();
        if (playerParent != null)
        {
            anim = playerParent.GetComponentInChildren<Animator>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //if (playerParent != null)
        //{
        //    if (collider.gameObject.CompareTag("Player") && anim.GetCurrentAnimatorStateInfo(0).IsName("AttackAirSlam"))
        //    {
        //        playerParent.Attack(playerParent.downAirDamage, playerParent.downAirKB, playerParent.downAirRange, 2, 3.5f, playerParent.downPoint);
        //        playerParent.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, 5), ForceMode2D.Impulse);
        //    }
        //}
    }
}
