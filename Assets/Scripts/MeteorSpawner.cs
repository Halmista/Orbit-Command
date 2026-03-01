using System.Collections;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [Header("References")]
    public Gameplay gameplay;
    public Transform earthCenter;
    public GameObject meteorPrefab;

    [Header("Impact Indicator")]
    public GameObject impactRingPrefab;

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
            if (gameplay != null && gameplay.currentEarthHP <= 0f)
            {
                Debug.Log("Earth destroyed. Stopping meteor spawns and clearing meteors.");

                // Destroy all existing meteors
                Meteor[] existingMeteors = FindObjectsOfType<Meteor>();
                foreach (Meteor m in existingMeteors)
                    Destroy(m.gameObject);

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

        Destroy(lineObj, 10f);
    }

    void SpawnMeteor()
    {
        if (gameplay != null && gameplay.currentEarthHP <= 0f)
            return;

        // Random latitude and longitude
        float lat = Random.Range(-60f, 60f) * Mathf.Deg2Rad;
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

        Meteor mScript = meteor.GetComponent<Meteor>();
        mScript.Initialize(targetDir, meteorSpeed, gameplay, meteorDamage, earthCenter.position, gameplay.earthRadius);

        DrawMeteorPath(spawnPos, earthCenter.position);

        // Calculate impact point on Earth's surface
        Vector3 toCenter = (earthCenter.position - spawnPos).normalized;
        Vector3 impactPoint = earthCenter.position - toCenter * gameplay.earthRadius;

        // Spawn impact ring
        if (impactRingPrefab != null)
        {
            GameObject ringObj = Instantiate(impactRingPrefab, impactPoint, Quaternion.identity);

            // Align ring flat on Earth's surface
            Vector3 normal = (impactPoint - earthCenter.position).normalized;
            ringObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

            // Assign to meteor
            mScript.SetImpactRing(ringObj);

            // Initialize RingPulse
            RingPulse ringPulse = ringObj.GetComponent<RingPulse>();
            if (ringPulse != null)
            {
                ringPulse.Initialize(meteor.transform, earthCenter, gameplay);
            }
        }
    }
}