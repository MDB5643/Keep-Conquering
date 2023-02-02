using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets._2D;

public class CPUDeath : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;
    public float respawnTimer = 3.0f;
    public float timeSinceDeath = 0.0f;
    public float playerHeight;

    private CPUCharacter2D m_Character;

    private void Awake()
    {
        m_Character = GetComponent<CPUCharacter2D>();
    }

    private void Update()
    {
        timeSinceDeath += Time.deltaTime;
        Animator playerAnim = player.GetComponent<Animator>();

        if (timeSinceDeath >= respawnTimer && playerAnim.GetBool("isDead") == true)
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
        m_Character.m_DamagePercentage = 0.0f;
        timeSinceDeath = 0f;
        Animator playerAnim = player.GetComponent<Animator>();

        playerAnim.ResetTrigger("Respawn");
        playerAnim.SetTrigger("Destroy");
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        playerAnim.SetBool("isDead", true);

        rb.bodyType = RigidbodyType2D.Static;

    }

    public void Respawn()
    {
        player.transform.localScale = new Vector3(2f, 2f, 1);
        Animator playerAnim = player.GetComponent<Animator>();
        playerAnim.SetBool("isDead", false);
        player.tag = "Player";
        player.gameObject.layer = LayerMask.NameToLayer("Player");
        var currRenderer = gameObject.GetComponent<SpriteRenderer>();
        currRenderer.sortingLayerName = "Player";

        player.transform.position = respawnPoint.transform.position;
        m_Character.m_FacingRight = true;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        playerAnim.ResetTrigger("Destroy");
        playerAnim.SetTrigger("Respawn");

    }
}
