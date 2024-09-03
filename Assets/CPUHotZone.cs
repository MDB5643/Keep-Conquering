using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<GameObject> enemiesInBounds = new List<GameObject>();

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
            if (inRange && CPUParent.gameObject.GetComponentInParent<Conqueror>().m_timeSinceAttack > 0.2f)
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
        if (enemiesInBounds.Count <= 0)
        {
            inRange = false;
        }
        var deadTargets = enemiesInBounds.Where(x => (x.GetComponent<MinionBehavior>() && x.GetComponent<MinionBehavior>().m_dead) || (x.GetComponent<Conqueror>() && x.GetComponent<Conqueror>().m_dead)).ToList();
        
        foreach(GameObject go in deadTargets)
        {
            enemiesInBounds.Remove(go);
        }
        if (deadTargets.Count > 0)
        {
            if (enemiesInBounds.Count > 0)
            {
                if (CPUParent != null)
                {
                    CPUParent.target = enemiesInBounds[0].transform;
                }
                else if (MinionParent != null)
                {
                    MinionParent.target = enemiesInBounds[0].transform;
                }
                else if (GolemParent != null)
                {
                    GolemParent.target = enemiesInBounds[0].transform;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((MinionParent != null && collider.GetComponent<MinionBehavior>() != null && MinionParent.teamColor == collider.GetComponent<MinionBehavior>().teamColor) 
            || collider.gameObject.name.Contains("Golem") 
            || (collider.GetComponent<Conqueror>() != null && MinionParent != null && collider.GetComponent<Conqueror>().teamColor == MinionParent.teamColor) 
            || (CPUParent != null && collider.GetComponent<MinionBehavior>() != null && CPUParent.gameObject.GetComponentInParent<Conqueror>().teamColor == collider.GetComponent<MinionBehavior>().teamColor)
            || (CPUParent != null && collider.GetComponent<Conqueror>() != null && CPUParent.gameObject.GetComponentInParent<Conqueror>().teamColor == collider.GetComponent<Conqueror>().teamColor)
            || collider.name.Contains("Point")
            || collider.tag == "HotZone"
            || collider.tag == "EyeTarget")
        {
            //do nothing

        }
        else if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("PlayerMid") || collider.GetComponent<MinionBehavior>() != null)
        {
            inRange = true;
            if (enemyParent != null)
            {
                CPUParent.inRange = true;
                enemyParent.target = collider.transform;
            }
            if (CPUParent != null)
            {
                CPUParent.m_timeSinceDecision = 0.0f;
                CPUParent.inRange = true;
                if (enemiesInBounds.Count == 0)
                {
                    CPUParent.target = collider.transform;
                }
                enemiesInBounds.Add(collider.transform.gameObject);
            }
            if (MinionParent != null)
            {
                MinionParent.inRange = true;
                if (enemiesInBounds.Count == 0)
                {
                    MinionParent.target = collider.transform;
                }
                enemiesInBounds.Add(collider.transform.gameObject);
            }
            if (GolemParent != null)
            {
                GolemParent.inRange = true;
                if (enemiesInBounds.Count == 0)
                {
                    GolemParent.target = collider.transform;
                }
                enemiesInBounds.Add(collider.transform.gameObject);
            }

        }
        else if (collider.gameObject.CompareTag("EyeTarget"))
        {
            string enemyTeam = collider.GetComponentInParent<TowerEye>().teamColor;
            inRange = true;

            if (enemyParent != null)
            {
                CPUParent.inRange = true;
                enemyParent.target = collider.transform;
            }
            if (CPUParent != null)
            {
                if (CPUParent.gameObject.GetComponentInParent<Conqueror>().teamColor != enemyTeam)
                {
                    CPUParent.inRange = true;
                    CPUParent.target = collider.transform;
                    enemiesInBounds.Add(collider.transform.gameObject);
                }
                
            }
            if (MinionParent != null)
            {
                if (MinionParent.teamColor != enemyTeam)
                {
                    MinionParent.inRange = true;
                    MinionParent.target = collider.transform;
                    enemiesInBounds.Add(collider.transform.gameObject);
                }
            }

        }

    }


    private void OnTriggerExit2D(Collider2D collider)
    {
        if ((MinionParent != null && collider.GetComponent<MinionBehavior>() != null && MinionParent.teamColor == collider.GetComponent<MinionBehavior>().teamColor)
            || collider.gameObject.name.Contains("Golem")
            || (collider.GetComponent<Conqueror>() != null && MinionParent != null && collider.GetComponent<Conqueror>().teamColor == MinionParent.teamColor)
            || (CPUParent != null && collider.GetComponent<MinionBehavior>() != null && CPUParent.gameObject.GetComponentInParent<Conqueror>().teamColor == collider.GetComponent<MinionBehavior>().teamColor)
            || (CPUParent != null && collider.GetComponent<Conqueror>() != null && CPUParent.gameObject.GetComponentInParent<Conqueror>().teamColor == collider.GetComponent<Conqueror>().teamColor)
            || collider.name.Contains("Point"))
        {
            //do nothing

        }
        else
        {
            if (collider.gameObject.CompareTag("Player") && CPUParent != null)
            {

                enemiesInBounds.Remove(collider.transform.gameObject);
                CPUParent.triggerArea.SetActive(true);
                CPUParent.inRange = false;
                if (enemiesInBounds.Count <= 0 || enemiesInBounds[0] == null)
                {
                    CPUParent.SelectTarget();
                }
                else
                {
                    CPUParent.target = enemiesInBounds[0].transform;
                }
            }
            else if (collider.gameObject.CompareTag("Player") && MinionParent != null)
            {

                enemiesInBounds.Remove(collider.transform.gameObject);
                MinionParent.triggerArea.SetActive(true);
                MinionParent.inRange = false;
                if (enemiesInBounds.Count <= 0 || enemiesInBounds[0] == null)
                {
                    MinionParent.SelectTarget();
                }
                else
                {
                    MinionParent.target = enemiesInBounds[0].transform;
                }
            }

            else if (collider.gameObject.CompareTag("PlayerMid") && GolemParent != null)
            {
                enemiesInBounds.Remove(collider.transform.gameObject);
                GolemParent.triggerArea.SetActive(true);
                GolemParent.inRange = false;
                if (enemiesInBounds.Count <= 0 || enemiesInBounds[0] == null)
                {
                    GolemParent.SelectTarget();
                }
                else
                {
                    GolemParent.target = enemiesInBounds[0].transform;
                }
            }
            else if (collider.gameObject.CompareTag("EyeTarget") && MinionParent != null)
            {
                enemiesInBounds.Remove(collider.transform.gameObject);
                MinionParent.triggerArea.SetActive(true);
                MinionParent.inRange = false;
                if (enemiesInBounds.Count <= 0 || enemiesInBounds[0] == null)
                {
                    MinionParent.SelectTarget();
                }
                else
                {
                    MinionParent.target = enemiesInBounds[0].transform;
                }
            }
        }
    }
}
