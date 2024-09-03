using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class TimeandSky : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 23.99f)]
    public float currentTime;
    [Tooltip("Amount time is increased by in seconds, every realtime seconds. E.g. value of 60 will increase time 1 minute every second")]
    public float timeRate;
    public bool pauseTime;
    private float currentTimeInSeconds;

    [Header("Sky Settings")]
    public bool spawnBackground;
    public enum backgroundResolution
    {
        res16Bit, res32Bit, res64Bit, res128Bit, res256Bit, res512Bit, res1024Bit, res2048Bit
    }
    [Range(0.5f, 2f)]
    public float camSizeMultiplier = 1f;
    public int backgroundOrderInLayer;
    public backgroundResolution myRes;
    [HideInInspector]
    public int resValue = 0;
    public DistinctTimeShade[] backgroundShades;

    [Header("Stars Settings")]
    public bool spawnStars;
    public Stars myStars;
    private Camera myCamera;
    private Texture2D texture;
    private SpriteRenderer myRenderer;
    private float backgroundWidth = 0f;
    private float backgroundHeight = 0f;
    void Start()
    {
        {
            myCamera = Camera.main;
            transform.parent = myCamera.transform;
            transform.localPosition = new Vector3(0, 0, 10);

            currentTimeInSeconds = currentTime * 3600f;

            if (texture == null)
            {
                InitializeTexture();
            }
        }
    }
    void Update()
    {
        if (!pauseTime)
        {
            TimeAdjustment();
            TimeGradientCheck();
        }
        else
        {
            currentTimeInSeconds = currentTime * 3600f;
        }
        UpdateStars();
    }

    public void InitializeTexture()
    {
        if (!spawnBackground)
        {
            return;
        }
        SetRes();
        currentTimeInSeconds = currentTime * 3600f;
        myRenderer = GetComponent<SpriteRenderer>();
        texture = new Texture2D(1, resValue, TextureFormat.ARGB32, false);
        myRenderer.material.mainTexture = texture;
        myRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, resValue), new Vector2(0.5f, 0.5f));
        myRenderer.sortingOrder = backgroundOrderInLayer;
        ResizeBackground();
        TimeGradientCheck();
    }


    public void SetRes()
    {
        switch (myRes)
        {
            case backgroundResolution.res16Bit:
                resValue = 16;
                break;
            case backgroundResolution.res32Bit:
                resValue = 32;
                break;
            case backgroundResolution.res64Bit:
                resValue = 64;
                break;
            case backgroundResolution.res128Bit:
                resValue = 128;
                break;
            case backgroundResolution.res256Bit:
                resValue = 256;
                break;
            case backgroundResolution.res512Bit:
                resValue = 512;
                break;
            case backgroundResolution.res1024Bit:
                resValue = 1024;
                break;
            case backgroundResolution.res2048Bit:
                resValue = 2048;
                break;
            default:
                resValue = 128;
                Debug.LogWarning("Unknown Resolution value. Defaulted to 128");
                break;
        }
    }
    private void ResizeBackground()
    {
        float camHeight = myCamera.orthographicSize / (resValue / 200F);
        float camWidth = camHeight * myCamera.aspect;
        transform.localScale = new Vector2(camWidth * resValue, camHeight) * camSizeMultiplier;
        backgroundHeight = camHeight * camSizeMultiplier * resValue / 200f;
        backgroundWidth = camWidth * camSizeMultiplier * resValue / 200f;
        foreach (Transform child in transform)
        {
            child.transform.position = new Vector2(child.transform.position.x / transform.localScale.x, child.transform.position.y / transform.localScale.y);
        }
    }
    private void TimeAdjustment()
    {
        currentTimeInSeconds += timeRate * Time.deltaTime;
        //86400 is the total seconds in a day - calculated as 24(hours) * 3600 (seconds in an hour)
        if (currentTimeInSeconds >= 86400)
        {
            currentTimeInSeconds -= 86400;
        }
        if (currentTimeInSeconds < 0)
        {
            currentTimeInSeconds += 86400;
        }
        currentTime = currentTimeInSeconds / 3600;
    }

    private void TimeGradientCheck()
    {
        if (texture == null && spawnBackground)
        {
            InitializeTexture();
        }
        if (!spawnBackground)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
            return;
        }
        DistinctTimeShade newTimeshade = backgroundShades[0];
        DistinctTimeShade oldTimeshade = backgroundShades[0];
        float timeOfNewGradient = 0f;
        float newTimeShadeDistance = 86401f;
        float oldTimeShadeDistance = 86401f;
        float distance = 1;
        foreach (DistinctTimeShade timeShade in backgroundShades)
        {
            float timeOfGradient = timeShade.hour * 3600 + timeShade.minute * 60;
            float difference = timeOfGradient - currentTimeInSeconds;

            if (timeOfGradient < currentTimeInSeconds)
            {
                difference += 86400;
            }
            if (Mathf.Abs(difference) < newTimeShadeDistance)
            {
                newTimeShadeDistance = Mathf.Abs(difference);
                newTimeshade = timeShade;
                timeOfNewGradient = timeOfGradient;
            }
            timeOfGradient = timeShade.hour * 3600 + timeShade.minute * 60;
            difference = timeOfGradient - currentTimeInSeconds;
            if (timeOfGradient > currentTimeInSeconds)
            {
                difference -= 86400;
            }
            if (Mathf.Abs(difference) < oldTimeShadeDistance)
            {
                oldTimeshade = timeShade;
                oldTimeShadeDistance = Mathf.Abs(difference);
            }
        }

        float newTimeShadePlusOldTimeShade = newTimeShadeDistance + oldTimeShadeDistance;
        if (timeRate >= 0)
        {
            if (newTimeShadePlusOldTimeShade == 0)
            {
                distance = 0f;
            }
            else
            {
                distance = oldTimeShadeDistance / (newTimeShadePlusOldTimeShade);
            }
            FormatTexture(oldTimeshade.myGradient, newTimeshade.myGradient, distance);
        }
        else
        {
            if (newTimeShadePlusOldTimeShade == 0)
            {
                distance = 0f;
            }
            else
            {
                distance = newTimeShadeDistance / (newTimeShadePlusOldTimeShade);
            }

            FormatTexture(newTimeshade.myGradient, oldTimeshade.myGradient, distance);
        }
    }
    private void FormatTexture(Gradient oldGradient, Gradient newGradient, float gradientMerge)
    {

        for (int y = 0; y < texture.height; y++)
        {
            float gradLevel = (float)y / resValue;
            Color myNewColor = Color.white;
            for (int x = 0; x < texture.width; x++)
            {
                if (newGradient == null)
                {
                    myNewColor = newGradient.Evaluate(gradLevel);
                    texture.SetPixel(x, y, myNewColor);
                }
                else
                {
                    myNewColor = Color.Lerp(oldGradient.Evaluate(gradLevel), newGradient.Evaluate(gradLevel), gradientMerge);
                }
                texture.SetPixel(x, y, myNewColor);

            }
        }
        texture.Apply();

    }
    private Color CalculateColorWithTime()
    {
        Color blank = new Color(1, 1, 1, 0);

        if (myStars.startTimeInSeconds > myStars.endTimeInSeconds)
        {
            if (currentTimeInSeconds >= myStars.startTimeInSeconds)
            {
                if (myStars.startTimeInSeconds + myStars.fadeTimeInSeconds < currentTimeInSeconds)
                {
                    return Color.white;
                }
                float difference = (currentTimeInSeconds - myStars.startTimeInSeconds) / myStars.fadeTimeInSeconds;
                return Color.Lerp(blank, Color.white, difference);
            }

            if (currentTimeInSeconds <= myStars.endTimeInSeconds)
            {
                if (myStars.endTimeInSeconds - myStars.fadeTimeInSeconds > currentTimeInSeconds)
                {
                    return Color.white;
                }

                float difference = (myStars.endTimeInSeconds - currentTimeInSeconds) / myStars.fadeTimeInSeconds;
                return Color.Lerp(blank, Color.white, difference);
            }
        }
        else
        {
            if (currentTimeInSeconds >= myStars.startTimeInSeconds && currentTimeInSeconds <= myStars.endTimeInSeconds)
            {
                if (currentTimeInSeconds <= myStars.startTimeInSeconds + myStars.fadeTimeInSeconds)
                {
                    float difference = (currentTimeInSeconds - myStars.startTimeInSeconds) / myStars.fadeTimeInSeconds;
                    return Color.Lerp(blank, Color.white, difference);
                }
                if (currentTimeInSeconds >= myStars.endTimeInSeconds - myStars.fadeTimeInSeconds)
                {
                    float difference = (myStars.endTimeInSeconds - currentTimeInSeconds) / myStars.fadeTimeInSeconds;
                    return Color.Lerp(blank, Color.white, difference);
                }
                return Color.white;
            }
        }

        return blank;

    }
    private void UpdateStars()
    {
        if (!spawnStars)
        {
            if (myStars.spawnedStars)
            {
                Destroy(myStars.spawnedStars);
            }
            return;
        }
        if (!myStars.spawnedStars)
        {
            myStars.spawnedStars = Instantiate(myStars.starsPrefab, transform.position, transform.rotation);
            var starRend = myStars.spawnedStars.GetComponent<Renderer>();
            myStars.myMaterial = starRend.material;
            starRend.sortingOrder = myStars.starsOrderInLayer;
            var shape = myStars.spawnedStars.GetComponent<ParticleSystem>().shape;
            shape.scale = new Vector2(backgroundWidth, backgroundHeight) * 2.5f;
            myStars.startTimeInSeconds = myStars.starHourStart * 3600 + myStars.starMinuteStart * 60;
            myStars.endTimeInSeconds = myStars.starHourFinish * 3600 + myStars.starMinuteFinish * 60;
            myStars.fadeTimeInSeconds = myStars.starFadeInAndOutInMinutes * 60;
            ParralaxObject myPO = myStars.spawnedStars.AddComponent<ParralaxObject>();
            myPO.parallaxAmount = myStars.parallaxScale;
            myPO.loop = true;
            myPO.playParticleSystem = true;
            myPO.totalXSize = backgroundWidth * 2.5f;

        }
        myStars.myMaterial.color = CalculateColorWithTime();
    }
    public void SetTime(bool isTimePaused, int changeTimeHour, int changeTimeMinutes, bool changeTime, float changeTimeRateInSeconds, bool changeTimeRate)
    {
        pauseTime = isTimePaused;
        if (changeTime)
        {
            while (changeTimeHour >= 24)
            {
                changeTimeHour -= 24;
            }
            while (changeTimeHour < 0)
            {
                changeTimeHour += 24;
            }
            while (changeTimeMinutes >= 60)
            {
                changeTimeHour -= 60;
            }
            while (changeTimeMinutes < 0)
            {
                changeTimeHour += 60;
            }
            currentTimeInSeconds = changeTimeHour * 3600 + changeTimeMinutes * 60;
        }
        if (changeTimeRate)
        {
            timeRate = changeTimeRateInSeconds;
        }
    }
}
