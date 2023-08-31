using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    public int enemyPlayerCount = 0;
    public bool isInCountdown;
    public float countdownTimer = 5.0f;
    public float m_timeSinceCountStart = 5.0f;

    string teamColor = "";

    // Start is called before the first frame update
    void Start()
    {
        if (transform.name == "RedKeepChallengeTrigger")
        {
            teamColor = "Red";
        }
        else
        {
            teamColor = "Blue";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyPlayerCount > 0)
        {
            m_timeSinceCountStart += Time.deltaTime;
        }
        else
        {
            m_timeSinceCountStart = 0;
        }
        if (m_timeSinceCountStart > countdownTimer)
        {
            transform.GetComponentInParent<PlayerManager>().InitiateFinalChallenge(teamColor);
            isInCountdown = false;
            m_timeSinceCountStart = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("Hook1"))
        {
            if (collision.gameObject.GetComponent<Conqueror>() && transform.name == "RedKeepChallengeTrigger")
            {
                //add tags for red vs blue team and update later
                //parentPlatform.blueMinionCount++;
                enemyPlayerCount++;
            }
            if (collision.gameObject.GetComponent<CPUBehavior>())
            {
                enemyPlayerCount++;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("Hook1"))
        {
            if (collision.gameObject.GetComponent<Conqueror>() && transform.name == "RedKeepChallengeTrigger")
            {
                ////add tags for red vs blue team and update later
                enemyPlayerCount--;

            }
            if (collision.gameObject.GetComponent<CPUBehavior>())
            {
                if (enemyPlayerCount > 0)
                {
                    enemyPlayerCount--;
                }

            }
        }
    }
}
