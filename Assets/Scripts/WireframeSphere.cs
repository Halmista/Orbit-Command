using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class WireframeSphere : MonoBehaviour
{
    [Header("Sphere Settings")]
    public int latitudeLines = 12;
    public int longitudeLines = 24;
    public float radius = 1.2f;
    public Material lineMaterial;
    public Color lineColor = Color.red;
    public float lineWidth = 0.02f;
    private Vector3 lastCameraForward;

    [Header("Camera for visibility")]
    public Camera cam; // assign your orbit camera
    [Header("Vertex Letters")]
    public GameObject letterPrefab;   // TMP prefab
    public int letterCount = 20;
    public float letterScale = 0.09f;
    public string availableLetters = "abcdefghijklmnopqrstuvwxyz";

    [HideInInspector]
    public List<Vector3> worldVertices = new List<Vector3>();
    public List<Vector3> verticesFacingCamera = new List<Vector3>();

    private LineRenderer lineRenderer;
    private Dictionary<Vector3, GameObject> activeLabels = new();
    private Queue<char> letterQueue;
    public OrbitPivot orbitPivot;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lastCameraForward = cam.transform.forward;
        letterQueue = new Queue<char>(availableLetters);
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

        Vector3 camDir = (cam.transform.position - transform.position).normalized;

        foreach (var vertex in worldVertices)
        {
            Vector3 normal = (vertex - transform.position).normalized;

            // Quarter sphere ≈ 45° cone
            if (Vector3.Dot(normal, camDir) > 0.7f)
            {
                verticesFacingCamera.Add(vertex);
            }
        }
    }
    void UpdateLettersFacingCamera()
    {
        if (letterPrefab == null || cam == null) return;

        // Spawn or update visible vertices
        foreach (var vertex in verticesFacingCamera)
        {
            if (activeLabels.ContainsKey(vertex))
            {
                // Just re-orient existing label
                GameObject label = activeLabels[vertex];
                label.transform.LookAt(
                    label.transform.position + cam.transform.forward,
                    cam.transform.up
                );
                continue;
            }

            if (letterQueue.Count == 0)
                return; // no more unique letters

            // Spawn new label
            GameObject labelObj = Instantiate(letterPrefab, vertex, Quaternion.identity, transform);
            labelObj.transform.localScale = Vector3.one * letterScale;

            labelObj.transform.LookAt(
                labelObj.transform.position + cam.transform.forward,
                cam.transform.up
            );

            TMP_Text tmp = labelObj.GetComponent<TMP_Text>();
            if (tmp != null)
                tmp.text = letterQueue.Dequeue().ToString(); // 🔥 unique letter

            activeLabels.Add(vertex, labelObj);
        }

        // Hide labels that are no longer facing the camera
        List<Vector3> toRemove = new();

        foreach (var pair in activeLabels)
        {
            if (!verticesFacingCamera.Contains(pair.Key))
            {
                pair.Value.SetActive(false);
                toRemove.Add(pair.Key);
            }
        }

        foreach (var key in toRemove)
            activeLabels.Remove(key);
    }





}
