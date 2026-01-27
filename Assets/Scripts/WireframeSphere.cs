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

    /*[Header("Vertex Labels")]
    public GameObject labelPrefab;
    public float labelSurfaceOffset = 0.02f;*/

    [HideInInspector]
    public List<Vector3> worldVertices = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> verticesFacingCamera = new List<Vector3>();

    private LineRenderer lineRenderer;
    //private Dictionary<Vector3, GameObject> activeLabels = new();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lastCameraForward = cam.transform.forward;


        DrawWireframe();
        CalculateVertices();
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
        Vector3 sphereCenter = transform.position;

        for (int i = 0; i <= latitudeLines; i++)
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


    

}
