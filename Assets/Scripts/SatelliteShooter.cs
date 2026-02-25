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

    void Start()
    {
        // Cache the sphere
        sphere = FindObjectOfType<WireframeSphere>();

        // Register satellite
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
        float closestDist = Mathf.Infinity;
        Meteor closest = null;

        Vector3 outward = (transform.position - sphere.transform.position).normalized;

        foreach (var m in meteors)
        {
            Vector3 dirToMeteor = (m.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(outward, dirToMeteor);
            float dist = Vector3.Distance(transform.position, m.transform.position);

            if (angle <= detectionAngle / 2f && dist <= detectionRange)
            {
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = m;
                }
            }
        }

        return closest;
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