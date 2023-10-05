using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = .3f;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public float shakeDuration = 0.2f;
        public AnimationCurve animCurve;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        public Vector3 minValues, maxValue;

        // Use this for initialization
        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }


        // Update is called once per frame
        //TODO: Should this be fixed update?
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            //Define minimum x,y,z values
            //Vector3 targetPos = target.position;
            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            //Verify target pos is out of bounds or not
            //Limit it to the min and max
            Vector3 boundPosition = new Vector3(Mathf.Clamp(aheadTargetPos.x, minValues.x, maxValue.x),
                Mathf.Clamp(aheadTargetPos.y, minValues.y, maxValue.y),
                Mathf.Clamp(aheadTargetPos.z, minValues.z, maxValue.z));

            Vector3 newPos = Vector3.SmoothDamp(transform.position, boundPosition, ref m_CurrentVelocity, damping);
            //Vector3 smoothPos = Vector3.Lerp(transform.position, boundPosition, damping*Time)

            transform.position = newPos;

            m_LastTargetPosition = target.position;
            if (target != null)
            {
                if (target.GetComponentInParent<Conqueror>().m_cameraShake)
                {
                    target.GetComponentInParent<Conqueror>().m_cameraShake = false;
                    StartCoroutine(Shaking());
                }
            }
            
        }

        IEnumerator Shaking()
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < shakeDuration)
            {
                elapsedTime += Time.deltaTime;
                float strength = animCurve.Evaluate(elapsedTime / target.GetComponentInParent<Conqueror>().m_shakeIntensity);
                transform.position = startPosition + UnityEngine.Random.insideUnitSphere * strength;
                yield return null;
            }
            transform.position = startPosition;
        }
    }
}
