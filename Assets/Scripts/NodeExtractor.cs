using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class NodeExtractor : MonoBehaviour
{
    [Header("Camera & Earth Reference")]
    public Camera targetCamera;
    public Transform earthCenter;
    public float earthRadius = 1f; // approximate radius of your icosahedron

    [Header("Results")]
    public List<Vector3> allNodes = new();
    public List<Vector3> viewableNodes = new();

    Mesh mesh;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        CacheAllNodes();
    }

    void Update()
    {
        if (!targetCamera || !earthCenter) return;
        UpdateViewableNodes();
    }

    /// <summary> Cache all mesh vertices in world space </summary>
    void CacheAllNodes()
    {
        allNodes.Clear();
        foreach (var v in mesh.vertices)
        {
            allNodes.Add(transform.TransformPoint(v));
        }

        Debug.Log($"Total nodes: {allNodes.Count}");
    }

    /// <summary> Return only nodes facing the camera and not “behind” the Earth </summary>
    void UpdateViewableNodes()
    {
        viewableNodes.Clear();

        Vector3 camPos = targetCamera.transform.position;
        Vector3 camDir = (earthCenter.position - camPos).normalized;

        foreach (var node in allNodes)
        {
            Vector3 toNode = (node - earthCenter.position).normalized;

            // 1️⃣ Hemisphere check: node must face the camera roughly
            if (Vector3.Dot(toNode, camDir) <= 0f)
                continue;

            // 2️⃣ Screen viewport check
            Vector3 vp = targetCamera.WorldToViewportPoint(node);
            if (vp.z <= 0 || vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1)
                continue;

            // 3️⃣ Approximate occlusion by sphere math
            Vector3 camToNode = node - camPos;
            Vector3 camToCenter = earthCenter.position - camPos;

            float t = Vector3.Dot(camToNode.normalized, camToCenter);
            Vector3 closestPoint = camPos + camToNode.normalized * t;

            float distToCenter = Vector3.Distance(closestPoint, earthCenter.position);
            if (distToCenter < earthRadius) continue; // node is “behind” globe

            viewableNodes.Add(node);
        }

        Debug.Log($"Viewable nodes: {viewableNodes.Count}");
    }

}
