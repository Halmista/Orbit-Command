using System.Collections.Generic;
using UnityEngine;

public class SatelliteSpawner : MonoBehaviour
{
    [Header("References")]
    public WireframeSphere wireframeSphere; // your wireframe sphere
    public GameObject satellitePrefab;      // cylinder or cube
    public GameObject laserPrefab;          // laser prefab
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

        for (int i = 0; i < satelliteCount && vertices.Count > 0; i++)
        {
            int index = Random.Range(0, vertices.Count);
            Vector3 vertex = vertices[index];
            vertices.RemoveAt(index);

            // Spawn exactly on the surface
            Vector3 directionFromCenter = (vertex - wireframeSphere.transform.position).normalized;
            Vector3 spawnPos = wireframeSphere.transform.position + directionFromCenter * wireframeSphere.radius;

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
