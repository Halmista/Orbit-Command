using System.Collections.Generic;
using UnityEngine;

public class SatelliteSpawner : MonoBehaviour
{
    [Header("References")]
    public WireframeSphere wireframeSphere; 
    public GameObject satellitePrefab;      
    public GameObject laserPrefab;          
    public int satelliteCount = 10;

    void Start()
    {
        if (wireframeSphere == null || satellitePrefab == null || laserPrefab == null) return;

        // Make sure vertices exist
        wireframeSphere.CalculateVertices();
        List<Vector3> vertices = new List<Vector3>(wireframeSphere.worldVertices);

        if (vertices.Count == 0)
        {
            Debug.LogError("No vertices found! Make sure WireframeSphere has a mesh.");
            return;
        }

        Vector3 center = wireframeSphere.transform.position;
        Vector3 sphereUp = wireframeSphere.transform.up;
        float poleThreshold = 0.85f; // Exclude vertices close to poles

        // Filter out poles
        vertices.RemoveAll(v =>
        {
            Vector3 dir = (v - center).normalized;
            float verticalDot = Mathf.Abs(Vector3.Dot(dir, sphereUp));
            return verticalDot > poleThreshold;
        });

        if (vertices.Count == 0)
        {
            Debug.LogWarning("All vertices are at poles. Cannot spawn satellites.");
            return;
        }

        for (int i = 0; i < satelliteCount && vertices.Count > 0; i++)
        {
            int index = Random.Range(0, vertices.Count);
            Vector3 vertex = vertices[index];
            vertices.RemoveAt(index);

            // Spawn exactly on the surface
            Vector3 directionFromCenter = (vertex - center).normalized;
            Vector3 spawnPos = center + directionFromCenter * wireframeSphere.radius;

            GameObject sat = Instantiate(satellitePrefab, spawnPos, Quaternion.identity);

            // Face radially outward
            sat.transform.rotation = Quaternion.LookRotation(directionFromCenter);

            // Scale to be visible but not huge
            sat.transform.localScale = Vector3.one * 0.2f;

            // Add shooter and pass laser prefab
            SatelliteShooter shooter = sat.AddComponent<SatelliteShooter>();
            shooter.laserPrefab = laserPrefab;

            Debug.Log("Spawned satellite at " + spawnPos);
        }
    }
}
