using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class WeatherParticleEffect
{
    [Header("Prefab Particle Effect (OPTIONAL: See Tooltip)")]
    [Tooltip("Note: Using this field will disable new Particle Effect options. This is to replace creating a particle effect and allows you to use your prefabs")]
    public GameObject prefabParticleEffect;
    public enum spawnLocations
    {
        top,
        left,
        right,
        bottom,
        center,
        custom
    }
    public spawnLocations spawnLocation;
    [Tooltip("Coordinates (x, y) are based off (1, 1) is top right, and (-1, -1) is bottom left")]
    public Vector2 customSpawnLocation;
    [Header("New Particle Effect Options")]
    public string effectName;
    public float particleLifetime;
    public Color particleColor = Color.white;
    public float particleStartSpeed;
    public Vector2 particleMinMaxSize;
    public float gravityModifier;
    public float emissionRate;
    public ParticleSystemShapeType shapeType = ParticleSystemShapeType.SingleSidedEdge;
    [Range(0.1f, 3f)]
    public float shapeSizeMultiplier = 1f;
    public bool changeSizeWithSpeed;
    public Vector2 minMaxXSizeChangeWithSpeed;
    public Vector2 minMaxYSizeChangeWithSpeed;
    public bool swayParticle;
    [Range(0f, 10f)]
    public float xSwayAmount;
    [Range(0f, 10f)]
    public float ySwayAmount;
    public bool enableParticleCollisions;
    [Range(0f, 1f)]
    public float dampenOnCollision;
    [Range(0f, 1f)]
    public float bounceOnCollision;
    public float lifetimeLossOnCollision;
    public LayerMask collisionLayers;
    public float particleCollisionForce;
    public bool prewarmParticleEffect;
    [Header("Wind Options")]
    public bool effectedByWind;
    public float windEffectMultiplier;
    public bool rotatesWithWind;

    [Header("Splash Options")]
    public bool doesParticleSplash;
    public string splashName;
    public float splashStartSpeed;
    public float splashLife;
    public Color splashColor = Color.white;
    public float splashSize;
    public Vector2 splashOffset;
    public float splashGravityModifer;
    public int splashBurstAmount;
    public ParticleSystemShapeType splashShapeType;
    public float splashShapeSize;
    public bool splashCanCollide;
    [Range(0f, 1f)]
    public float splashDampenOnCollision;
    [Range(0f, 1f)]
    public float splashBounceOnCollision;
    [Header("Materials and Ordering")]
    public int spawnOrderInLayer = 0;
    public Material effectMaterial;
    public Material splashMaterial;
}
