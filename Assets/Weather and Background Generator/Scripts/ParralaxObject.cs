using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParralaxObject : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxAmount;
    public bool loop = false;
    public bool playParticleSystem = false;
    public float totalXSize;
    [HideInInspector]
    public bool transformIsNotCentre = false;
    public Component[] myComponentsToDeleteOnClones;
    private Camera myCamera;
    private float maxCamX;
    private float minCamX;
    private GameObject[] parallaxArray;
    private GameObject backgroundParent;
    private GameObject leftPanel;
    private GameObject rightPanel;
    void Start()
    {
        myCamera = Camera.main;
        if (loop)
        {
            LoopPrep();
        }
        if (playParticleSystem)
        {
            GetComponent<ParticleSystem>().Play();
        }
    }
    void Update()
    {
        UpdatePosition();
    }

    private void LoopPrep()
    {
        parallaxArray = new GameObject[3];
        parallaxArray[1] = transform.gameObject;
        CreateParallaxParent();
        CreateCopy(0);
        CreateCopy(2);
        leftPanel = EvaluatePanel(parallaxArray, false);
        rightPanel = EvaluatePanel(parallaxArray, true);
    }
    private void CreateParallaxParent()
    {
        backgroundParent = new GameObject(transform.name + " Holder");
        backgroundParent.transform.parent = transform.parent;
        transform.parent = backgroundParent.transform;
    }
    private void CreateCopy(int arrayId)
    {
        GameObject newMe = Instantiate(transform.gameObject, transform.position, transform.rotation, backgroundParent.transform);
        Component[] myComps = newMe.GetComponent<ParralaxObject>().myComponentsToDeleteOnClones;
        for (int i = 0; i < myComps.Length; i++)
        {
            Destroy(myComps[i]);
        }
        Destroy(newMe.GetComponent<ParralaxObject>());

        newMe.name = transform.name + " [" + arrayId + "]";
        float positionMultiplier = -1;
        if (arrayId > 1)
        {
            positionMultiplier = 1;
        }
        newMe.transform.position = new Vector2(transform.position.x + totalXSize * positionMultiplier, transform.position.y);
        if (playParticleSystem)
        {
            newMe.GetComponent<ParticleSystem>().Play();
        }
        parallaxArray[arrayId] = newMe;
    }
    private GameObject EvaluatePanel(GameObject[] obj, bool returnRightPanel)
    {
        float[] objX = new float[3];
        for (int i = 0; i < obj.Length; i++)
        {
            objX[i] = obj[i].transform.position.x;
        }
        float minMaxValue = objX[0];
        int returnId = 0;
        for (int i = 1; i < objX.Length; i++)
        {
            if (returnRightPanel)
            {
                if (objX[i] > minMaxValue)
                {
                    minMaxValue = objX[i];
                    returnId = i;
                }
            }
            else
            {
                if (objX[i] < minMaxValue)
                {
                    minMaxValue = objX[i];
                    returnId = i;
                }
            }
        }
        return obj[returnId];
    }
    private void CheckCameraSizeAndPos(GameObject leftPanel, GameObject rightPanel, float totXSize)
    {
        if (!loop)
        {
            return;
        }
        float camHeight = myCamera.orthographicSize * 2;
        float camHalfWidth = camHeight * myCamera.aspect / 2f;
        if (transformIsNotCentre)
        {
            maxCamX = rightPanel.transform.position.x + totXSize / 2f - camHalfWidth;
            minCamX = leftPanel.transform.position.x + totXSize / 2f + camHalfWidth;
        }
        else
        {
            maxCamX = rightPanel.transform.position.x - camHalfWidth;
            minCamX = leftPanel.transform.position.x + camHalfWidth;
        }
    }
    private void UpdatePosition()
    {
        CheckCameraSizeAndPos(leftPanel, rightPanel, totalXSize);

        if (myCamera.transform.position.x >= maxCamX && loop)
        {
            float newOffset = rightPanel.transform.position.x + totalXSize;
            leftPanel.transform.position = new Vector2(newOffset, transform.position.y);
            leftPanel = EvaluatePanel(parallaxArray, false);
            rightPanel = EvaluatePanel(parallaxArray, true);
        }

        if (myCamera.transform.position.x <= minCamX && loop)
        {
            float newOffset = leftPanel.transform.position.x - totalXSize;
            rightPanel.transform.position = new Vector2(newOffset, transform.position.y);
            leftPanel = EvaluatePanel(parallaxArray, false);
            rightPanel = EvaluatePanel(parallaxArray, true);
        }

        backgroundParent.transform.position = new Vector2(myCamera.transform.position.x * parallaxAmount, backgroundParent.transform.position.y);
    }
}
