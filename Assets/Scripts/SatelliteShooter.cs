using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteShooter : MonoBehaviour
{
    public GameObject laserPrefab;

    [Header("Detection")]
    public float detectionRange = 10f; // radius around satellite to detect meteors
    public float detectionAngle = 70f; // degrees of cone outward

    [Header("Firing")]
    public float fireRate = 0.5f;
    public float laserSpeed = 25f;

    private WireframeSphere sphere;
    private float fireTimer;

    void Awake()
    {
        // Cache the sphere
        sphere = FindObjectOfType<WireframeSphere>();

        //Register satellite
        if (SatelliteManager.Instance != null)
           SatelliteManager.Instance.RegisterSatellite(this);
    }

    void Update()
    {
        if (laserPrefab == null || sphere == null) return;

        // Check for meteors in cone
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
            fireTimer = 0f; // reset timer if no target
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

            if (dist > detectionRange)
                continue;

            Vector3 dir = toMeteor.normalized;

            // Direction check (SOFT cone)
            float dot = Vector3.Dot(outward, dir);

            if (dot < 0.8f) // 👈 tweak this (higher = narrower vision)
                continue;

            // Earth blocking check
            Ray ray = new Ray(transform.position, dir);
            if (Physics.Raycast(ray, dist, LayerMask.GetMask("Earth")))
                continue;

            // Prioritize meteors closer to impact (not just distance)
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

        // Direction from satellite to target
        Vector3 dir = (target.transform.position - transform.position).normalized;

        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.LookRotation(dir));

        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb == null) rb = laser.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.velocity = dir * laserSpeed;

        // Ignore collision with sphere
        Collider laserCol = laser.GetComponent<Collider>();
        Collider sphereCol = sphere.GetComponent<Collider>();
        if (laserCol != null && sphereCol != null)
            Physics.IgnoreCollision(laserCol, sphereCol);

        // Destroy laser after 5 seconds
        Destroy(laser, 5f);

        // Add a script to deal damage when it hits
        laser.AddComponent<LaserHit>();
    }

    // Helper for LetterInputController
    public bool IsBusy()
    {
        return FindMeteorInCone() != null;
    }
}