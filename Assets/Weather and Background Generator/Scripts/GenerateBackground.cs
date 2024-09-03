using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GenerateBackground : MonoBehaviour
{
    [Header("Repeat Build - Only for Debug/Troubleshooting")]
    public bool repeatBuild = false;
    [Header("Background Generation Settings")]
    public bool colourGradient;
    public Gradient myGrad;
    public int xSize = 20;
    public float xScale = 1f;
    public float yScale;
    [Range(0f, 0.999f)]
    public float noiseAmount;
    [Range(0f, 0.999f)]
    public float bumpiness;
    public float bumpSize;
    public float offsetX = 0f;
    public bool loop;
    public int degreesOfLoopSmooth;
    public bool randomiseCurve;
    public bool buildOverTime;
    [Range(0.01f, 0.1f)]
    public float buildDelay;

    [Header("Outline Settings")]
    public bool drawLine;
    public Color lineColor;
    public float lineWidth;
    public Material lineMaterial;

    [Header("Camera Position Settings")]
    public Camera myCamera;
    [Range(0f, 1f)]
    public float parallaxAmount = 1;
    public int orderInLayer;
    private bool loopInitial;
    private int ySize = 6;
    private int randOffset;
    private float minBGHeight;
    private float maxBGHeight;
    private Color[] colors;
    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;
    private GameObject line;
    private LineRenderer lineR;
    void Start()
    {
        Setup();
        if (buildOverTime)
        {
            StartCoroutine(CreateShape());
        }
        else
        {
            InstaBuild();
        }

    }
    void Update()
    {
        if ((!loopInitial && loop) || (!loop && loopInitial))
        {
            loop = false;
            loopInitial = false;
            Debug.LogWarning("Loop cannot be changed during runtime. Loop has been disabled - please check generated backgrounds");
        }
        if (repeatBuild == true)
        {
            if (buildOverTime)
            {
                Debug.LogWarning("Cannot have Repeat Build & Build Over Time enabled. Build Over Time has been disabled");
                buildOverTime = false;
            }
            CreateShapeImmediately();
            CalculateColours();

            UpdateMesh();
        }
    }
    private void Setup()
    {
        if (randomiseCurve)
        {
            randOffset = Random.Range(1, 99999);
        }
        else
        {
            randOffset = 0;
        }
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sortingOrder = orderInLayer;

        if (myCamera == null)
        {
            myCamera = Camera.main;
        }
        loopInitial = loop;
        if (loop)
        {
            ParralaxObject myPO = transform.gameObject.AddComponent<ParralaxObject>();
            myPO.parallaxAmount = parallaxAmount;
            myPO.loop = loop;
            myPO.totalXSize = xSize * xScale;
            myPO.myComponentsToDeleteOnClones = new Component[1];
            myPO.myComponentsToDeleteOnClones[0] = GetComponent<GenerateBackground>();
            myPO.transformIsNotCentre = true;
        }
    }
    public void InstaBuild()
    {
        if (mesh == null)
        {
            Setup();
        }
        CreateShapeImmediately();
        CalculateColours();
        UpdateMesh();
    }
    private void CreateShapeImmediately()
    {
        buildOverTime = false;
        StartCoroutine(CreateShape());
    }
    private IEnumerator CreateShape()
    {
        DesignShape();

        int vert = 0;
        int tris = 0;
        triangles = new int[xSize * ySize * 6];

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
                if (buildOverTime)
                {
                    CalculateColours();
                    UpdateMesh();
                    yield return new WaitForSecondsRealtime(buildDelay);
                }

            }
            vert++;
        }
        if (buildOverTime && drawLine)
        {
            Debug.LogWarning("Line Drawing unavailable with Build Over Time. Draw Line has been disabled");
            drawLine = false;
        }
        DrawLine();
    }

    private void DesignShape()
    {

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        float yMod = 0;
        degreesOfLoopSmooth = Mathf.Clamp(degreesOfLoopSmooth, 1, xSize / 2);
        int beginSmooth = xSize - degreesOfLoopSmooth;
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                yMod = CalculateYModPerlinNoise(x, y, beginSmooth);
                vertices[i] = new Vector3(x * xScale, y + yMod, 0);
                i++;
            }
        }
    }
    private float CalculateYModPerlinNoise(int x, int y, int beginSmooth)
    {
        float yMod = 0f;
        if (y != 0)
        {

            float xCoord = x + degreesOfLoopSmooth + offsetX + randOffset;
            float yCoord = y + randOffset;
            if (x >= beginSmooth && loop)
            {
                float yOld = Mathf.PerlinNoise(xCoord * noiseAmount, yCoord * noiseAmount) * yScale;
                float yOldBump = Mathf.PerlinNoise(xCoord * bumpiness, yCoord * bumpiness) * bumpSize;
                yOld += yOldBump;
                float yNew = Mathf.PerlinNoise((x - beginSmooth + offsetX + randOffset) * noiseAmount, yCoord * noiseAmount) * yScale;
                float yNewBump = Mathf.PerlinNoise((x - beginSmooth + offsetX + randOffset) * bumpiness, yCoord * bumpiness) * bumpSize;
                yNew += yNewBump;
                float changeDegree = x - beginSmooth;
                changeDegree /= degreesOfLoopSmooth;
                yMod = Mathf.Lerp(yOld, yNew, changeDegree);
            }
            else
            {
                yMod = Mathf.PerlinNoise(xCoord * noiseAmount, yCoord * noiseAmount) * yScale;
                float yBump = Mathf.PerlinNoise(xCoord * bumpiness, yCoord * bumpiness) * bumpSize;
                yMod += yBump;
            }
            CheckMaxMin(y + yMod);
        }
        else
        {
            minBGHeight = y;
        }
        return yMod;
    }
    private void CheckMaxMin(float checkValue)
    {
        if (checkValue > maxBGHeight)
        {
            maxBGHeight = checkValue;
        }
        if (checkValue < minBGHeight)
        {
            minBGHeight = checkValue;
        }
    }
    private void CalculateColours()
    {
        if (!colourGradient)
        {
            return;
        }
        colors = new Color[vertices.Length];

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minBGHeight, maxBGHeight, vertices[i].y);
                colors[i] = myGrad.Evaluate(height);
                i++;
            }
        }
    }
    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    private void DrawLine()
    {
        if (!drawLine)
        {
            return;
        }
        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int e = 0; e < 3; e++)
            {
                int vert1 = triangles[i + e];
                int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                if (edges.ContainsKey(edge))
                {
                    edges.Remove(edge);
                }
                else
                {
                    edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                }
            }
        }
        Dictionary<int, int> lookup = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> edge in edges.Values)
        {
            if (lookup.ContainsKey(edge.Key) == false)
            {
                lookup.Add(edge.Key, edge.Value);
            }
        }

        if (!line)
        {

            line = new GameObject("Outline");
            lineR = line.AddComponent<LineRenderer>();
        }
        lineR.transform.parent = transform;
        lineR.positionCount = 0;
        lineR.material = lineMaterial;
        lineR.startColor = lineR.endColor = lineColor;
        lineR.startWidth = lineR.endWidth = lineWidth;
        lineR.sortingOrder = orderInLayer-1;
        lineR.useWorldSpace = false;

        int startVert = 0;
        int nextVert = startVert;
        int highestVert = startVert;
        while (true)
        {
            if (vertices[nextVert].y == 0)
            {
                nextVert = lookup[nextVert];
                if (nextVert == startVert)
                {
                    break;
                }
            }
            else
            {
                lineR.positionCount++;
                lineR.SetPosition(lineR.positionCount - 1, vertices[nextVert] + transform.position);
                nextVert = lookup[nextVert];
                if (nextVert > highestVert)
                {
                    highestVert = nextVert;
                }
            }
        }

    }
}