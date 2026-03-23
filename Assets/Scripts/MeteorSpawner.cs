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
    public float spawnRadius = 50f;
    public float meteorSpeed = 5f;
    public float spawnInterval = 1f;

    [Header("Dynamic Difficulty")]
    public float minSpawnInterval = 0.15f;
    public float intervalDecayRate = 0.015f;
    public int baseMaxMeteors = 20;
    public int meteorsPerMinute = 15;

    [Header("Meteor Settings")]
    public float meteorDamage = 10f;
    public float bigMeteorDamage = 30f;

    [Header("Boss Meteor")]
    public GameObject bigMeteorPrefab;
    public float bigMeteorHP = 50f;
    public float bigMeteorScale = 2f;
    public float bigMeteorSpeed = 2f;

    private int lastBigSpawnThreshold = 0;
    private float gameTimer = 0f;
    public Material lineMaterial;

    void Start()
    {
        StartCoroutine(SpawnMeteorRoutine());
    }

    void Update()
    {
        gameTimer += Time.deltaTime;
    }

    IEnumerator SpawnMeteorRoutine()
    {
        while (true)
        {
            if (Time.timeScale == 0f)
            {
                yield return null;
                continue;
            }
            if (gameplay != null && gameplay.currentEarthHP <= 0f)
            {
                Debug.Log("Earth destroyed. Clearing meteors.");

                Meteor[] existingMeteors = FindObjectsOfType<Meteor>();
                foreach (Meteor m in existingMeteors)
                    m.KillMeteor(true);

                yield break;
            }

            // Active meteor cap scaling with time
            //int maxMeteors = baseMaxMeteors + Mathf.FloorToInt((gameTimer / 60f) * meteorsPerMinute);
            //int maxMeteors = baseMaxMeteors + Mathf.FloorToInt(Mathf.Sqrt(gameTimer / 30f) * meteorsPerMinute);
            int scaledMax = baseMaxMeteors + Mathf.FloorToInt(Mathf.Sqrt(gameTimer / 20f) * meteorsPerMinute);

            // Clamp to 60 max
            int maxMeteors = Mathf.Clamp(scaledMax, baseMaxMeteors, 50);


            if (UIManager.Instance.ActiveMeteors < maxMeteors)
            {
                SpawnMeteor();
            }

            // Dynamic spawn interval scaling
            float difficultyFromTime = gameTimer * intervalDecayRate;

            int destroyed = UIManager.Instance != null
                ? UIManager.Instance.destroyedMeteors
                : 0;

            //float difficultyFromKills = destroyed * 0.002f;
            /*float timeFactor = Mathf.Sqrt(gameTimer) * intervalDecayRate;
            float killFactor = destroyed * 0.0015f;

            float dynamicInterval = Mathf.Clamp(spawnInterval - timeFactor - killFactor,
                minSpawnInterval,
                spawnInterval
            ); */

            float timeFactor = Mathf.Sqrt(gameTimer) * intervalDecayRate * 0.8f;
            float killFactor = destroyed * 0.0012f;

            // Slight easing so it doesn't get too crazy late-game
            float difficulty = timeFactor + killFactor;

            float dynamicInterval = Mathf.Lerp(spawnInterval, minSpawnInterval, difficulty);

            // Clamp safety
            dynamicInterval = Mathf.Clamp(dynamicInterval, minSpawnInterval, spawnInterval);

            if (gameTimer > 120f) // after 2 minutes
            {
                maxMeteors += 10;
                maxMeteors = Mathf.Min(maxMeteors, 50);

                dynamicInterval *= 0.85f; // faster spawns
            }

            //yield return new WaitForSeconds(dynamicInterval);
            yield return new WaitForSecondsRealtime(dynamicInterval);
        }
    }

    void SpawnMeteor()
    {
        if (gameplay != null && gameplay.currentEarthHP <= 0f)
            return;

        float lat = Random.Range(-60f, 60f) * Mathf.Deg2Rad;
        float lon = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(
            Mathf.Cos(lat) * Mathf.Cos(lon),
            Mathf.Sin(lat),
            Mathf.Cos(lat) * Mathf.Sin(lon)
        );

        Vector3 spawnPos = earthCenter.position + dir * spawnRadius;

        Vector3 randomOffset = Random.insideUnitSphere * 0.2f;
        Vector3 targetDir = ((earthCenter.position + randomOffset) - spawnPos).normalized;

        GameObject meteor;

        int destroyed = UIManager.Instance != null
            ? UIManager.Instance.destroyedMeteors
            : 0;

        bool spawnBig = false;

        int currentThreshold = destroyed / 25;

        if (currentThreshold > lastBigSpawnThreshold)
        {
            spawnBig = true;
            lastBigSpawnThreshold = currentThreshold;
        }

        if (spawnBig && bigMeteorPrefab != null)
        {
            meteor = Instantiate(bigMeteorPrefab, spawnPos, Quaternion.identity);
            meteor.transform.localScale *= bigMeteorScale;
        }
        else
        {
            meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.IncrementActiveMeteors();
            UIManager.Instance.LogMeteorSpawn(spawnPos);
        }

        Meteor mScript = meteor.GetComponent<Meteor>();

        if (spawnBig)
        {
            mScript.hp = bigMeteorHP;
            mScript.maxHP = bigMeteorHP;
        }

        float hpScale = 1f + gameTimer * 0.004f;

        if (!spawnBig)
        {
            mScript.maxHP *= hpScale;
            mScript.hp = mScript.maxHP;
        }

        float damageToUse = spawnBig ? bigMeteorDamage : meteorDamage;
        //float speedToUse = spawnBig ? bigMeteorSpeed : meteorSpeed;
        float speedScale = 1f + gameTimer * 0.0025f;

        float speedToUse = spawnBig ? bigMeteorSpeed * speedScale: meteorSpeed * speedScale;

        mScript.Initialize(targetDir, speedToUse, gameplay, damageToUse, earthCenter.position, gameplay.earthRadius);

        DrawMeteorPath(meteor, spawnPos, earthCenter.position);

        Vector3 toCenter = (earthCenter.position - spawnPos).normalized;
        Vector3 impactPoint = earthCenter.position - toCenter * gameplay.earthRadius;

        if (impactRingPrefab != null)
        {
            GameObject ringObj = Instantiate(impactRingPrefab, impactPoint, Quaternion.identity);

            Vector3 normal = (impactPoint - earthCenter.position).normalized;
            ringObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

            mScript.SetImpactRing(ringObj);

            RingPulse ringPulse = ringObj.GetComponent<RingPulse>();
            if (ringPulse != null)
            {
                float multiplier = spawnBig ? 2f : 1f;
                ringPulse.Initialize(meteor.transform, earthCenter, gameplay, multiplier);
            }
        }
    }

    void DrawMeteorPath(GameObject meteor, Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("MeteorPathLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        int segments = 20;
        lr.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = Vector3.Lerp(start, end, t);
            lr.SetPosition(i, pos);
        }

        lr.startWidth = 0.05f;
        lr.endWidth = 0.01f;

        if (lineMaterial != null)
        {
            lr.material = lineMaterial;
        }
        else
        {
            Debug.LogError("Line material not assigned!");
        }
        lr.material.mainTexture = GenerateDashedTexture();
        lr.textureMode = LineTextureMode.Tile;

        MeteorPathPulse pulse = lineObj.AddComponent<MeteorPathPulse>();
        pulse.Initialize(meteor);
    }

    Texture2D GenerateDashedTexture()
    {
        Texture2D tex = new Texture2D(128, 1);
        tex.wrapMode = TextureWrapMode.Repeat;

        for (int i = 0; i < 128; i++)
        {
            if (i % 32 < 16)
                tex.SetPixel(i, 0, Color.red);
            else
                tex.SetPixel(i, 0, Color.clear);
        }

        tex.Apply();
        return tex;
    }
}