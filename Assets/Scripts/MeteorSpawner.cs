using System.Collections;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [Header("References")]
    public Gameplay gameplay;
    public Transform earthCenter;
    public GameObject meteorPrefab;

    [Header("Spawn Settings")]
    public float spawnRadius = 50f;   // distance from Earth
    public float meteorSpeed = 5f;
    public float spawnInterval = 1f;  // seconds between meteors

    [Header("Meteor Settings")]
    public float meteorDamage = 10f;

    void Start()
    {
        StartCoroutine(SpawnMeteorRoutine());
    }

    IEnumerator SpawnMeteorRoutine()
    {
        while (true)
        {
            // Stop spawning if Earth is destroyed
            if (gameplay != null && gameplay.earthHP <= 0f)
            {
                Debug.Log("Earth destroyed. Stopping meteor spawns and clearing meteors.");

                // Destroy all existing meteors
                Meteor[] existingMeteors = FindObjectsOfType<Meteor>();
                foreach (Meteor m in existingMeteors)
                {
                    Destroy(m.gameObject);
                }

                yield break; // stop the coroutine
            }

            SpawnMeteor();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    void DrawMeteorPath(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("MeteorPathLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = Color.red;

        // Destroy line after 5 seconds
        Destroy(lineObj, 10f);
    }
    void SpawnMeteor()
    {
        if (gameplay != null && gameplay.earthHP <= 0f)
            return;

        // Define safe latitude range (in degrees)
        float minLatitude = -60f; // south pole limit
        float maxLatitude = 60f;  // north pole limit

        // Random latitude and longitude
        float lat = Random.Range(minLatitude, maxLatitude) * Mathf.Deg2Rad;
        float lon = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Convert spherical to Cartesian
        Vector3 dir = new Vector3(
            Mathf.Cos(lat) * Mathf.Cos(lon),
            Mathf.Sin(lat),
            Mathf.Cos(lat) * Mathf.Sin(lon)
        );

        Vector3 spawnPos = earthCenter.position + dir * spawnRadius;

        // Direction toward Earth with small random offset
        Vector3 randomOffset = Random.insideUnitSphere * 0.2f;
        Vector3 targetDir = ((earthCenter.position + randomOffset) - spawnPos).normalized;

        // Spawn meteor
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.IncrementActiveMeteors();
            UIManager.Instance.LogMeteorSpawn(spawnPos);
        }
        Meteor mScript = meteor.AddComponent<Meteor>();
        mScript.Initialize(targetDir, meteorSpeed, gameplay, meteorDamage, earthCenter.position, gameplay.earthRadius);

        DrawMeteorPath(spawnPos, earthCenter.position);
        Debug.Log($"Spawned meteor at {spawnPos} towards {earthCenter.position}");
        Debug.DrawLine(spawnPos, earthCenter.position, Color.red, 5f);
    }
}
