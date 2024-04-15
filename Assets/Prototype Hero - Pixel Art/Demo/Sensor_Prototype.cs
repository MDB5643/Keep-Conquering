using UnityEngine;
using System.Collections;

public class Sensor_Prototype : MonoBehaviour {

    private int m_ColCount = 0;

    private float m_DisableTimer;

    private void OnEnable()
    {
        m_ColCount = 0;
    }

    public bool State()
    {
        if (m_DisableTimer > 0)
            return false;
        return m_ColCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("PortalForeground") && !other.gameObject.name.Contains("MinionJump")  && !other.gameObject.name.Contains("MinionBoard") && !other.gameObject.CompareTag("MinionBoardGondola") && !other.gameObject.CompareTag("HotZone") && !other.gameObject.name.Contains("BBDSpecHB") && !(transform.GetComponentInParent<Rigidbody2D>().velocity.y > 0 && other.gameObject.name.Contains("Platform")) && !other.gameObject.tag.Contains("AttackHitbox"))
            m_ColCount++;
            
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("PortalForeground") && !other.gameObject.name.Contains("MinionJump") && !other.gameObject.name.Contains("MinionBoard") && !other.gameObject.CompareTag("MinionBoardGondola") && !other.gameObject.CompareTag("HotZone") && !other.gameObject.name.Contains("BBDSpecHB") && !other.gameObject.tag.Contains("AttackHitbox"))
            if (m_ColCount > 0)
            {
                m_ColCount--;
            }
    }

    void Update()
    {
        m_DisableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}
