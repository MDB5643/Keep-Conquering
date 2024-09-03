using MiscUtil.Linq.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public List<Conqueror> playerList;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        public Vector3 minValues, maxValue;

        bool zoomOut = false;
        bool zoomIn = false;
        private GameObject farthestObject;

        // Use this for initialization
        private void Start()
        {
            playerList.AddRange(GameObject.FindObjectsOfType<Conqueror>().Where(x => x.isPlayer == true));
            

            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }


        // Update is called once per frame
        //TODO: Should this be fixed update?
        private void Update()
        {
            if (playerList.Count <= 0)
            {
                playerList.AddRange(GameObject.FindObjectsOfType<Conqueror>().Where(x => x.isPlayer == true));
            }
            else
            {

                if (MenuEvents.gameModeSelect == 2 || MenuEvents.gameModeSelect == 3)
                {

                    float midpointX = 0;
                    float midpointY = 0;

                    foreach (Conqueror conq in playerList)
                    {
                        if (!conq.m_dead)
                        {
                            midpointX += conq.transform.position.x;
                            midpointY += conq.transform.position.y;
                        }
                    }

                    midpointX = midpointX / playerList.Count;
                    midpointY = midpointY / playerList.Count;
                    Vector3 midpoint = new Vector3(midpointX, midpointY, -10);

                    GameObject farthestObject = playerList.OrderBy(x => Mathf.Abs(x.transform.position.x - midpointX)).LastOrDefault().gameObject;

                    if (farthestObject.transform.position.x > midpointX + 4)
                    {
                        if (farthestObject.transform.position.x - midpointX <= 25)
                        {
                            GetComponent<Camera>().fieldOfView = (farthestObject.transform.position.x - midpointX + 6.7f) * 2.5f;
                        }

                    }
                    else if (farthestObject.transform.position.x < midpointX - 4)
                    {
                        if (midpointX - farthestObject.transform.position.x >= 25)
                        {
                            GetComponent<Camera>().fieldOfView = (midpointX - farthestObject.transform.position.x + 6.7f) * 2.5f;
                        }

                    }
                    else
                    {
                        GetComponent<Camera>().fieldOfView = 26.8f;
                    }



                    float xMoveDelta = (midpoint - m_LastTargetPosition).x;

                    bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

                    if (updateLookAheadTarget)
                    {
                        m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
                    }
                    else
                    {
                        m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
                    }

                    //Define minimum x,y,z values
                    //Vector3 targetPos = midpoint;
                    Vector3 aheadTargetPos = midpoint + m_LookAheadPos + Vector3.forward * m_OffsetZ;
                    //Verify target pos is out of bounds or not
                    //Limit it to the min and max
                    Vector3 boundPosition = new Vector3(Mathf.Clamp(aheadTargetPos.x, minValues.x, maxValue.x),
                        Mathf.Clamp(aheadTargetPos.y, minValues.y, maxValue.y),
                        Mathf.Clamp(aheadTargetPos.z, minValues.z, maxValue.z));

                    Vector3 newPos = Vector3.SmoothDamp(transform.position, boundPosition, ref m_CurrentVelocity, damping);
                    //Vector3 smoothPos = Vector3.Lerp(transform.position, boundPosition, damping*Time)

                    transform.position = newPos;

                    m_LastTargetPosition = midpoint;
                    if (target != null)
                    {
                        if (target.GetComponentInParent<Conqueror>() && target.GetComponentInParent<Conqueror>().m_cameraShake)
                        {
                            //target.GetComponentInParent<Conqueror>().m_cameraShake = false;
                            //StartCoroutine(Shaking());
                        }
                    }

                }


                else
                {
                    if (MenuEvents.gameModeSelect == 4 || MenuEvents.gameModeSelect == 6 && playerList.Count() > 0)
                    {
                        target = playerList.Where(x => x.isPlayer == true).FirstOrDefault().transform;
                    }
                    // only update lookahead pos if accelerating or changed direction
                    float xMoveDelta = (target.position - m_LastTargetPosition).x;

                    bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

                    if (updateLookAheadTarget)
                    {
                        m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
                    }
                    else
                    {
                        m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
                    }

                    //Define minimum x,y,z values
                    //Vector3 targetPos = target.position;
                    Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
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
                        if (target.GetComponentInParent<Conqueror>() && target.GetComponentInParent<Conqueror>().m_cameraShake)
                        {
                            //target.GetComponentInParent<Conqueror>().m_cameraShake = false;
                            //StartCoroutine(Shaking());
                        }
                    }
                }
            }
            
        }

        public IEnumerator Shaking(float shakeIntensity)
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < shakeDuration)
            {
                elapsedTime += Time.deltaTime;
                float strength = animCurve.Evaluate(elapsedTime / shakeIntensity);
                transform.position = startPosition + UnityEngine.Random.insideUnitSphere * strength;
                yield return null;
            }
            transform.position = startPosition;
        }
    }
}
