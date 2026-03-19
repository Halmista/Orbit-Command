using System.Collections.Generic;
using UnityEngine;

public class SatelliteSpawner : MonoBehaviour
{
    [Header("References")]
    public WireframeSphere wireframeSphere;
    public GameObject satellitePrefab;
    public GameObject laserPrefab;

    [Header("Starting Satellites")]
    public int satelliteCount = 10;

    public static SatelliteSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (wireframeSphere == null || satellitePrefab == null || laserPrefab == null)
            return;

        // Calculate vertices for the sphere
        wireframeSphere.CalculateVertices();

        // Spawn starting satellites
        SpawnSatellites(satelliteCount);

        // Spawn letters AFTER satellites exist
        wireframeSphere.SpawnLetters();
    }

    // MAIN SPAWN FUNCTION
    public void SpawnSatellites(int count)
    {
        List<Vector3> vertices = new List<Vector3>(wireframeSphere.worldVertices);

        Vector3 center = wireframeSphere.transform.position;
        Vector3 sphereUp = wireframeSphere.transform.up;
        float poleThreshold = 0.85f;

        // Remove vertices near poles
        vertices.RemoveAll(v =>
        {
            Vector3 dir = (v - center).normalized;
            float verticalDot = Mathf.Abs(Vector3.Dot(dir, sphereUp));
            return verticalDot > poleThreshold;
        });

        for (int i = 0; i < count && vertices.Count > 0; i++)
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

            //SatelliteManager.Instance.satellites.Add(shooter);
            //SatelliteManager.Instance.RegisterSatellite(shooter);


        }
    }

    // USED BY UPGRADES
    public void SpawnExtraSatellite()
    {
        SpawnSatellites(1);
    }
}