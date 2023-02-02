using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform respawnPoint;
    public float respawnTimer = 3.0f;
    public float timeSinceDeath = 0.0f;
    public float playerHeight;

    private EnemyBehavior m_Enemy;

    private void Awake()
    {
        m_Enemy = GetComponent<EnemyBehavior>();
    }

    private void Update()
    {
        timeSinceDeath += Time.deltaTime;
        Animator enemyAnim = enemy.GetComponent<Animator>();

        if (timeSinceDeath >= respawnTimer && enemyAnim.GetBool("isDead") == true)
        {
            Respawn();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BlastZone"))
        {
            Die();
        }
    }

    public void Die()
    {

        timeSinceDeath = 0f;
        Animator playerAnim = enemy.GetComponent<Animator>();
        playerAnim.SetTrigger("Destroy");
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        playerAnim.SetBool("isDead", true);

        rb.bodyType = RigidbodyType2D.Static;
        m_Enemy.currentDamage = 0.0f;
    }

    public void Respawn()
    {
        
        enemy.transform.localScale = new Vector3(0.089023f, 0.089023f, 0.089023f);
        Animator enemyAnim = enemy.GetComponent<Animator>();
        enemyAnim.SetBool("isDead", false);
        //enemy.tag = "enemy";
        enemy.gameObject.layer = LayerMask.NameToLayer("EnemyForeground");
        var currRenderer = gameObject.GetComponent<SpriteRenderer>();
        currRenderer.sortingLayerName = "EnemyForeground";

        enemy.transform.position = respawnPoint.transform.position;

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        enemyAnim.SetTrigger("Respawn");

    }
}
