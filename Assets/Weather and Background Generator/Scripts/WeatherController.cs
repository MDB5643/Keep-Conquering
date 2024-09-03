using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [Header ("Required - Time Information")]
    public TimeandSky timeAndSkyScript;


    [Header("Wind Settings")]
    public bool isWindOn;
    public Vector2 windDirectionAndMagnitude;
    private ParticleSystemForceField myWindObject;

    [Header("Weather Elements")]
    public bool isLighteningOn;
    public Lightening myLightening;
    public WeatherParticleEffect[] weatherParticleEffects;
    private float backgroundWidth = 0f;
    private float backgroundHeight = 0f;
    private int resValue = 0;
    
    private Camera myCamera;

    private void Start()
    {
        myCamera = Camera.main;
    }
    private void Update()
    {
        CheckSize();
        WindAdjustment();
        UpdateLightening();

        UpdatePosition();
    }

private void CheckSize(){
    if (resValue != timeAndSkyScript.resValue){
        resValue = timeAndSkyScript.resValue;
        myCamera = Camera.main;
        float camSizeMultiplier = timeAndSkyScript.camSizeMultiplier;
        float camHeight = myCamera.orthographicSize / (resValue / 200F);
        float camWidth = camHeight * myCamera.aspect;
        backgroundHeight = camHeight * camSizeMultiplier * resValue / 200f;
        backgroundWidth = camWidth * camSizeMultiplier * resValue / 200f;       
    }
}
     private void SetWeatherLocation(GameObject myElementObj, WeatherParticleEffect.spawnLocations location, Vector2 customLocation)
    {
            CheckSize();
            Vector2 topLocation = new Vector2(0f, backgroundHeight);
            Vector2 leftLocation = new Vector2(-backgroundWidth, 0f);
            Vector2 rightLocation = new Vector2(backgroundWidth, 0f);
            Vector2 bottomLocation = new Vector2(0f, -backgroundHeight);
            Vector2 centerLocation = new Vector2(0f, 0f);
            Vector2 customLocationConverted = new Vector2(customLocation.x * backgroundWidth, customLocation.y * backgroundHeight);
            backgroundWidth = 0f;
        
        Vector2 newLocation = new Vector2();
        switch (location)
        {
            case WeatherParticleEffect.spawnLocations.top:
                newLocation = topLocation;
                break;
            case WeatherParticleEffect.spawnLocations.left:
                newLocation = leftLocation;
                break;
            case WeatherParticleEffect.spawnLocations.right:
                newLocation = rightLocation;
                break;
            case WeatherParticleEffect.spawnLocations.bottom:
                newLocation = bottomLocation;
                break;
            case WeatherParticleEffect.spawnLocations.center:
                newLocation = centerLocation;
                break;
            case WeatherParticleEffect.spawnLocations.custom:
                newLocation = customLocationConverted;
                break;
            default:
                Debug.LogWarning("Invalid location for weather element");
                break;
        }
        myElementObj.transform.localScale = new Vector3(1f, 1f, 1f);
        myElementObj.transform.localPosition = newLocation;
    }
    private void WindAdjustment()
    {
        if (!isWindOn && myWindObject)
        {
            myWindObject.directionX = 0f;
            myWindObject.directionY = 0f;
            foreach (ParticleSystem psChild in GetComponentsInChildren<ParticleSystem>())
            {
                if (psChild.externalForces.enabled)
                {
                    psChild.externalForces.RemoveInfluence(myWindObject);
                    WindRotateCalculate(psChild);
                }
            }
            Destroy(myWindObject.gameObject);
        }
        if (!isWindOn)
        {
            return;
        }
        if (!myWindObject)
        {
            myWindObject = new GameObject("Wind").AddComponent<ParticleSystemForceField>();
            myWindObject.transform.localScale = new Vector3(1, 1, 1);
            myWindObject.startRange = 0f;
            myWindObject.endRange = Mathf.Max(backgroundWidth, backgroundHeight) * 4f;
            myWindObject.gravity = 0f;
            myWindObject.transform.parent = transform;
            myWindObject.transform.localPosition = new Vector2(0, 0);
            foreach (ParticleSystem psChild in GetComponentsInChildren<ParticleSystem>())
            {
                if (psChild.externalForces.enabled)
                {
                    psChild.externalForces.AddInfluence(myWindObject);
                    WindRotateCalculate(psChild);
                }
            }
        }

        if (myWindObject.directionX.constantMax != windDirectionAndMagnitude.x || myWindObject.directionY.constantMax != windDirectionAndMagnitude.y)
        {
            myWindObject.directionX = windDirectionAndMagnitude.x;
            myWindObject.directionY = windDirectionAndMagnitude.y;
            foreach (ParticleSystem psChild in GetComponentsInChildren<ParticleSystem>())
            {
                if (psChild.externalForces.enabled)
                {
                    WindRotateCalculate(psChild);
                }
            }

        }
    }
    private void WindRotateCalculate(ParticleSystem myPS)
    {
        var main = myPS.main;
        var shape = myPS.shape;
        var externalForces = myPS.externalForces;

        float radianDirection = (shape.rotation.z + 90f) * Mathf.Deg2Rad;
        float xDir = Mathf.Cos(radianDirection);
        float yDir = Mathf.Sin(radianDirection);
        Vector2 startDirection = new Vector2(xDir, yDir) * main.startSpeed.constantMax;
        Vector2 gravityDirection = new Vector2(0f, main.gravityModifier.constantMax) * Physics.gravity.y;
        Vector2 windDirection = new Vector2(0f, 0f);
        try
        {
            windDirection = new Vector2(myWindObject.directionX.constantMax, myWindObject.directionY.constantMax) * externalForces.multiplier;
        }
        catch
        {
            Debug.Log("Wind enabled but no object found for rotation calculation");
        }
        Vector2 direction = startDirection + gravityDirection + windDirection;


        float newRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        newRotation *= -1f;
        newRotation -= 90f;
        main.startRotation = new ParticleSystem.MinMaxCurve(newRotation * Mathf.Deg2Rad);
    }

    private void UpdateLightening()
    {
        if (isLighteningOn)
        {
            if (!myLightening.currentLightening)
            {
                Vector2 spawnLoc = new Vector2(0f, backgroundHeight + 21);
                myLightening.currentLightening = Instantiate(myLightening.myLighteningPrefab, transform.position, transform.rotation, transform).GetComponent<ParticleSystem>();
                myLightening.currentLightening.transform.localPosition = spawnLoc;
                var shape = myLightening.currentLightening.shape;
                var lighteningCol = myLightening.currentLightening.GetComponentsInChildren<ParticleSystem>()[1].collision;
                Renderer[] lighteningRen = myLightening.currentLightening.GetComponentsInChildren<Renderer>();

                foreach (Renderer ren in lighteningRen)
                {
                    ren.sortingOrder = myLightening.lighteningOrderInLayer;
                }
                lighteningCol.collidesWith = myLightening.lighteningCollisionLayers;
                shape.radius = backgroundWidth * 1.1f;
            }
            var emission = myLightening.currentLightening.emission;
            if (emission.rateOverTime.constantMax != myLightening.lighteningFrequency)
            {
                emission.rateOverTime = myLightening.lighteningFrequency;
            }
        }
        else
        {
            if (myLightening.currentLightening)
            {
                Destroy(myLightening.currentLightening.gameObject);
            }
        }
    }
 
    private void UpdatePosition()
    {
        Vector2 newPos = Camera.main.transform.position;
        transform.position = newPos;
    }
    public void SpawnWeatherElement(WeatherParticleEffect we)
    {
        if (we.prefabParticleEffect != null)
        {
            Instantiate(we.prefabParticleEffect, transform.position, transform.rotation, transform);
            return;
        }

        GameObject weatherElement = new GameObject(we.effectName);
        weatherElement.transform.parent = transform;
        ParticleSystem myPS = weatherElement.AddComponent<ParticleSystem>();
        myPS.Stop();
        var main = myPS.main;
        var emmission = myPS.emission;
        var shape = myPS.shape;
        var sizeSpeed = myPS.sizeBySpeed;
        var velocityOverLifetime = myPS.velocityOverLifetime;
        var collision = myPS.collision;
        var subEmitter = myPS.subEmitters;
        var externalForces = myPS.externalForces;

        var psRend = myPS.GetComponent<Renderer>();
        main.startLifetime = we.particleLifetime;
        main.startColor = we.particleColor;
        main.startSpeed = we.particleStartSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(we.particleMinMaxSize.x, we.particleMinMaxSize.y);
        main.gravityModifier = we.gravityModifier;
        main.prewarm = we.prewarmParticleEffect;
        main.maxParticles = (int)(we.emissionRate * we.particleLifetime);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        emmission.rateOverTime = we.emissionRate;

        shape.shapeType = we.shapeType;
        if (backgroundWidth == 0f)
        {
            myCamera = Camera.main;
            float camHeight = myCamera.orthographicSize;
            float camWidth = camHeight * myCamera.aspect;
            shape.scale = new Vector3(camWidth * we.shapeSizeMultiplier, camHeight * we.shapeSizeMultiplier, 1f);
        }
        else
        {
            shape.scale = new Vector3(backgroundWidth * we.shapeSizeMultiplier, backgroundHeight * we.shapeSizeMultiplier, 1f);
        }
        if (we.changeSizeWithSpeed)
        {
            sizeSpeed.enabled = true;
            sizeSpeed.separateAxes = true;
            sizeSpeed.x = new ParticleSystem.MinMaxCurve(we.minMaxXSizeChangeWithSpeed.x, we.minMaxXSizeChangeWithSpeed.y);
            sizeSpeed.y = new ParticleSystem.MinMaxCurve(we.minMaxYSizeChangeWithSpeed.x, we.minMaxYSizeChangeWithSpeed.y);
        }

        if (we.swayParticle)
        {
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-we.xSwayAmount, we.xSwayAmount);
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-we.ySwayAmount, we.ySwayAmount);
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        }
        if (we.enableParticleCollisions)
        {
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision2D;
            collision.dampen = we.dampenOnCollision;
            collision.bounce = we.bounceOnCollision;
            collision.lifetimeLoss = we.lifetimeLossOnCollision;
            collision.collidesWith = we.collisionLayers;
            collision.colliderForce = we.particleCollisionForce;
        }

        if (we.effectedByWind)
        {
            externalForces.enabled = true;
            externalForces.influenceFilter = ParticleSystemGameObjectFilter.List;
            try
            {
                externalForces.AddInfluence(myWindObject);
            }
            catch
            {
                Debug.LogWarning("Element effected by wind, but no wind object defined");
            }
            externalForces.multiplier = we.windEffectMultiplier;

            if (we.rotatesWithWind)
            {
                WindRotateCalculate(myPS);
            }
        }

        if (we.doesParticleSplash)
        {
            subEmitter.enabled = true;
            GameObject subEmitterObj = new GameObject(we.splashName);
            subEmitterObj.transform.parent = weatherElement.transform;
            ParticleSystem mySubPS = subEmitterObj.AddComponent<ParticleSystem>();
            subEmitter.AddSubEmitter(mySubPS, ParticleSystemSubEmitterType.Collision, ParticleSystemSubEmitterProperties.InheritNothing, 1);

            var subMain = mySubPS.main;
            var subEmission = mySubPS.emission;
            var subShape = mySubPS.shape;
            var subCollision = mySubPS.collision;
            var subRend = mySubPS.GetComponent<Renderer>();

            subMain.loop = false;
            subMain.startSpeed = we.splashStartSpeed;
            subMain.startLifetime = we.splashLife;
            subMain.startSize = we.splashSize;
            subMain.startColor = we.splashColor;
            subMain.gravityModifier = we.splashGravityModifer;
            subMain.maxParticles = 300;
            subMain.simulationSpace = ParticleSystemSimulationSpace.World;

            subEmission.rateOverTime = 0f;
            subEmission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, we.splashBurstAmount) });

            subShape.shapeType = we.shapeType;
            subShape.scale = new Vector3(we.splashShapeSize, we.splashShapeSize, 1f);
            subShape.randomDirectionAmount = 1f;
            subShape.position = we.splashOffset;
            if (we.splashCanCollide)
            {
                subCollision.enabled = true;
                subCollision.type = ParticleSystemCollisionType.World;
                subCollision.mode = ParticleSystemCollisionMode.Collision2D;
                subCollision.collidesWith = we.collisionLayers;
                subCollision.colliderForce = 0f;
                subCollision.bounce = we.splashBounceOnCollision;
                subCollision.dampen = we.splashDampenOnCollision;
                subCollision.lifetimeLoss = 0f;
            }
            subRend.material = we.splashMaterial;
            subRend.sortingOrder = we.spawnOrderInLayer;

        }
        psRend.material = we.effectMaterial;
        psRend.sortingOrder = we.spawnOrderInLayer;
        SetWeatherLocation(weatherElement, we.spawnLocation, we.customSpawnLocation);
        myPS.Play();
    }

    public void ChangeWind(bool turnWindOn, float xVector, float yVector, bool useCurrentSettings)
    {
        isWindOn = turnWindOn;
        if (!useCurrentSettings)
        {
            windDirectionAndMagnitude = new Vector2(xVector, yVector);
        }
    }

    public void ChangeLightening(bool turnLighteningOn, float changeLighteningFrequency, bool useCurrentSettings)
    {
        isLighteningOn = turnLighteningOn;
        if (!useCurrentSettings)
        {
            myLightening.lighteningFrequency = changeLighteningFrequency;
        }
        Destroy(myLightening.currentLightening);
    }


}

