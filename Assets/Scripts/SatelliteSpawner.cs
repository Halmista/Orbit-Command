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

        // Calculate vertices for the sphere
        wireframeSphere.CalculateVertices();

        // Spawn satellites
        SpawnSatellites();

        // Now spawn letters AFTER satellites exist
        wireframeSphere.SpawnLetters();
    }

    void SpawnSatellites()
    {
        List<Vector3> vertices = new List<Vector3>(wireframeSphere.worldVertices);

        Vector3 center = wireframeSphere.transform.position;
        Vector3 sphereUp = wireframeSphere.transform.up;
        float poleThreshold = 0.85f; // exclude poles

        vertices.RemoveAll(v =>
        {
            Vector3 dir = (v - center).normalized;
            float verticalDot = Mathf.Abs(Vector3.Dot(dir, sphereUp));
            return verticalDot > poleThreshold;
        });

        for (int i = 0; i < satelliteCount && vertices.Count > 0; i++)
        {
            int index = Random.Range(0, vertices.Count);
            Vector3 vertex = vertices[index];
            vertices.RemoveAt(index);

            Vector3 spawnPos = center + (vertex - center).normalized * wireframeSphere.radius;

            GameObject sat = Instantiate(satellitePrefab, spawnPos, Quaternion.identity);
            sat.transform.rotation = Quaternion.LookRotation((vertex - center).normalized);
            sat.transform.localScale = Vector3.one * 0.2f;

            SatelliteShooter shooter = sat.AddComponent<SatelliteShooter>();
            shooter.laserPrefab = laserPrefab;

            SatelliteManager.Instance.satellites.Add(shooter); // make sure manager knows about it
        }
    }
}
