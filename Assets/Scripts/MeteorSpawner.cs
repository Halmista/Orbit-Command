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
    public float meteorSpeed = 20f;
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

    void SpawnMeteor()
    {
        // Safety check
        if (gameplay != null && gameplay.earthHP <= 0f)
            return;

        // Random point on sphere
        Vector3 spawnPos = earthCenter.position + Random.onUnitSphere * spawnRadius;

        // Direction toward Earth with small random offset for variation
        Vector3 randomOffset = Random.insideUnitSphere * 0.2f;
        Vector3 targetDir = ((earthCenter.position + randomOffset) - spawnPos).normalized;

        // Spawn meteor
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        // Set its movement
        Meteor mScript = meteor.AddComponent<Meteor>();
        mScript.Initialize(targetDir, meteorSpeed, gameplay, meteorDamage, earthCenter.position, gameplay.earthRadius);

        Debug.Log($"Spawned meteor at {spawnPos} towards {earthCenter.position}");
        Debug.DrawLine(spawnPos, earthCenter.position, Color.red, 5f);
    }
}
