using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteShooter : MonoBehaviour
{
    public GameObject laserPrefab; // assign your laser prefab here (with LaserHit attached)

    [Header("Detection")]
    public float detectionRange = 10f;
    public float detectionAngle = 70f;

    [Header("Firing")]
    public float fireRate = 0.5f;
    public float laserSpeed = 25f;

    private WireframeSphere sphere;
    private float fireTimer;

    void Awake()
    {
        sphere = FindObjectOfType<WireframeSphere>();

        if (SatelliteManager.Instance != null)
            SatelliteManager.Instance.RegisterSatellite(this);
    }

    void Update()
    {
        if (laserPrefab == null || sphere == null) return;

        Meteor targetMeteor = FindMeteorInCone();

        if (targetMeteor != null)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                FireLaser(targetMeteor);
                fireTimer = 0f;
            }
        }
        else
        {
            fireTimer = 0f;
        }
    }

    Meteor FindMeteorInCone()
    {
        Meteor[] meteors = FindObjectsOfType<Meteor>();
        float closestScore = Mathf.Infinity;
        Meteor best = null;

        Vector3 outward = (transform.position - sphere.transform.position).normalized;

        foreach (var m in meteors)
        {
            Vector3 toMeteor = m.transform.position - transform.position;
            float dist = toMeteor.magnitude;

            if (dist > detectionRange) continue;

            Vector3 dir = toMeteor.normalized;
            float dot = Vector3.Dot(outward, dir);

            if (dot < 0.8f) continue;

            Ray ray = new Ray(transform.position, dir);
            if (Physics.Raycast(ray, dist, LayerMask.GetMask("Earth"))) continue;

            float score = dist * (1.5f - dot);
            if (score < closestScore)
            {
                closestScore = score;
                best = m;
            }
        }

        return best;
    }

    void FireLaser(Meteor target)
    {
        if (laserPrefab == null || target == null) return;

        Vector3 dir = (target.transform.position - transform.position).normalized;

        // Instantiate laser
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.LookRotation(dir));

        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb == null) rb = laser.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.velocity = dir * laserSpeed;

        // Assign the prefab to LaserHit so bounces work
        LaserHit laserHit = laser.GetComponent<LaserHit>();
        if (laserHit != null)
        {
            laserHit.laserPrefab = laserPrefab; // ✅ important for bounce logic
            laserHit.remainingBounces = LaserStats.bounces; // use current upgrade bounces
        }

        // Ignore collision with sphere
        Collider laserCol = laser.GetComponent<Collider>();
        Collider sphereCol = sphere.GetComponent<Collider>();
        if (laserCol != null && sphereCol != null)
            Physics.IgnoreCollision(laserCol, sphereCol);

        // Destroy laser after 5 seconds if it never hits anything
        Destroy(laser, 5f);
    }

    public bool IsBusy()
    {
        return FindMeteorInCone() != null;
    }
}