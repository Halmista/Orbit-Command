using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class WireframeSphere : MonoBehaviour
{
    [Header("Sphere Settings")]
    public int latitudeLines = 12;
    public int longitudeLines = 20;
    public float radius = 1.2f;
    public Material lineMaterial;
    public Color lineColor = Color.red;
    public float lineWidth = 0.02f;
    private Vector3 lastCameraForward;

    [Header("Camera for visibility")]
    public Camera cam; 
    
    [Header("Vertex Letters")]
    public GameObject letterPrefab; 
    public int letterCount = 20;
    public float letterScale = 0.09f;
    public string availableLetters = "abcdefghijklmnopqrstuvwxyz";

    [HideInInspector]
    public List<Vector3> worldVertices = new List<Vector3>();
    public List<Vector3> verticesFacingCamera = new List<Vector3>();

    private LineRenderer lineRenderer;
    private Dictionary<Vector3, GameObject> activeLabels = new();
    //private Queue<char> letterQueue;
    public OrbitPivot orbitPivot;
    public Dictionary<char, Vector3> letterToVertex = new();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lastCameraForward = cam.transform.forward;
        //letterQueue = new Queue<char>(availableLetters);
        orbitPivot.OnRotationEnd += HandleCameraRotated;


        DrawWireframe();
        CalculateVertices();
        UpdateVerticesFacingCamera();
        UpdateLettersFacingCamera();
    }

    void Update()
    {
        if (cam == null) return;

        // Only update when camera direction changes
        if (Vector3.Angle(lastCameraForward, cam.transform.forward) > 1f)
        {
            lastCameraForward = cam.transform.forward;

            UpdateVerticesFacingCamera();
            
        }
    }

    void HandleCameraRotated()
    {
        UpdateVerticesFacingCamera();
        UpdateLettersFacingCamera();
    }

    void OnDestroy()
    {
        if (orbitPivot != null)
            orbitPivot.OnRotationEnd -= HandleCameraRotated;
    }

    void DrawWireframe()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i <= latitudeLines; i++)
        {
            float lat = Mathf.PI * i / latitudeLines - Mathf.PI / 2f;
            for (int j = 0; j <= longitudeLines; j++)
            {
                float lon = 2 * Mathf.PI * j / longitudeLines;
                positions.Add(new Vector3(
                    radius * Mathf.Cos(lat) * Mathf.Cos(lon),
                    radius * Mathf.Sin(lat),
                    radius * Mathf.Cos(lat) * Mathf.Sin(lon)
                ));
            }
        }

        for (int j = 0; j <= longitudeLines; j++)
        {
            float lon = 2 * Mathf.PI * j / longitudeLines;
            for (int i = 0; i <= latitudeLines; i++)
            {
                float lat = Mathf.PI * i / latitudeLines - Mathf.PI / 2f;
                positions.Add(new Vector3(
                    radius * Mathf.Cos(lat) * Mathf.Cos(lon),
                    radius * Mathf.Sin(lat),
                    radius * Mathf.Cos(lat) * Mathf.Sin(lon)
                ));
            }
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    public void CalculateVertices()
    {
        worldVertices.Clear();
        Vector3 sphereCenter = transform.position; //this gets the position of the game object and that becomes the center

        for (int i = 0; i <= latitudeLines; i++) //loops through the latitude lines from top to bottom
        {
            float lat = Mathf.PI * i / latitudeLines - Mathf.PI / 2f;
            for (int j = 0; j <= longitudeLines; j++)
            {
                float lon = 2 * Mathf.PI * j / longitudeLines;

                Vector3 localPos = new Vector3(
                    radius * Mathf.Cos(lat) * Mathf.Cos(lon),
                    radius * Mathf.Sin(lat),
                    radius * Mathf.Cos(lat) * Mathf.Sin(lon)
                );

                worldVertices.Add(sphereCenter + localPos);
            }
        }

        Debug.Log($"Wireframe vertices calculated: {worldVertices.Count}");
    }

    void UpdateVerticesFacingCamera()
    {
        verticesFacingCamera.Clear();

        // Direction from sphere center to camera
        Vector3 toCam = (cam.transform.position - transform.position);
        toCam.y = 0f; // ignore vertical difference
        toCam.Normalize();

        // Get camera horizontal angle around Y axis
        float camAngle = Mathf.Atan2(toCam.z, toCam.x) * Mathf.Rad2Deg;
        if (camAngle < 0) camAngle += 360f;

        float wedgeSize = 360f / 6f; // 8 watermelon slices
        float halfWedge = wedgeSize / 2f;

        foreach (var vertex in worldVertices)
        {
            Vector3 dir = (vertex - transform.position);
            dir.y = 0f; // project to horizontal plane
            dir.Normalize();

            float vertexAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (vertexAngle < 0) vertexAngle += 360f;

            float delta = Mathf.DeltaAngle(camAngle, vertexAngle);

            if (Mathf.Abs(delta) <= halfWedge)
            {
                verticesFacingCamera.Add(vertex);
            }
        }
    }
    void UpdateLettersFacingCamera()
{
    if (letterPrefab == null || cam == null) return;

    // Destroy existing letters
    foreach (var pair in activeLabels)
    {
        if (pair.Value != null)
            Destroy(pair.Value);
    }
    activeLabels.Clear();
    letterToVertex.Clear();

    if (verticesFacingCamera.Count == 0)
        return;

    Vector3 center = transform.position;
    Vector3 sphereUp = transform.up;

    float poleThreshold = 0.85f; // exclude poles
    verticesFacingCamera.RemoveAll(v =>
    {
        Vector3 dir = (v - center).normalized;
        float verticalDot = Mathf.Abs(Vector3.Dot(dir, sphereUp));
        return verticalDot > poleThreshold;
    });

    if (verticesFacingCamera.Count == 0)
        return;

    // Remove vertices already occupied by satellites
    List<Vector3> freeVertices = new List<Vector3>();
    foreach (var v in verticesFacingCamera)
    {
        bool occupied = false;
        foreach (var sat in SatelliteManager.Instance.satellites)
        {
            if (Vector3.Distance(sat.transform.position, v) < 0.1f) // small tolerance
            {
                occupied = true;
                break;
            }
        }

        if (!occupied)
            freeVertices.Add(v);
    }

    if (freeVertices.Count == 0)
        return;

    // Sort by camera alignment
    Vector3 camForward = cam.transform.forward;
    freeVertices.Sort((a, b) =>
    {
        float camScoreA = Vector3.Dot(camForward, (a - cam.transform.position).normalized);
        float camScoreB = Vector3.Dot(camForward, (b - cam.transform.position).normalized);
        return camScoreB.CompareTo(camScoreA);
    });

    // Shuffle letters
    List<char> available = new List<char>(availableLetters);
    for (int i = 0; i < available.Count; i++)
    {
        int randomIndex = Random.Range(i, available.Count);
        (available[i], available[randomIndex]) = (available[randomIndex], available[i]);
    }

    int lettersToSpawn = Mathf.Min(letterCount, freeVertices.Count, available.Count);

    List<Vector3> chosenVertices = new List<Vector3>();
    float minDistance = radius * 0.25f;

    int letterIndex = 0;
    for (int i = 0; i < freeVertices.Count && letterIndex < lettersToSpawn; i++)
    {
        Vector3 candidate = freeVertices[i];
        bool tooClose = false;

        foreach (var chosen in chosenVertices)
        {
            if (Vector3.Distance(candidate, chosen) < minDistance)
            {
                tooClose = true;
                break;
            }
        }
        if (tooClose) continue;

        chosenVertices.Add(candidate);

        GameObject labelObj = Instantiate(letterPrefab, candidate, Quaternion.identity, transform);
        labelObj.transform.localScale = Vector3.one * letterScale;

        labelObj.transform.LookAt(
            labelObj.transform.position + cam.transform.forward,
            cam.transform.up
        );

        TMP_Text tmp = labelObj.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            char letter = available[letterIndex];
            tmp.text = letter.ToString();
            letterToVertex[letter] = candidate;
        }

        activeLabels.Add(candidate, labelObj);
        letterIndex++;
    }
}

}
