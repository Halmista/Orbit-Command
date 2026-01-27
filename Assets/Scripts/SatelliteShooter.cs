using UnityEngine;

public class SatelliteShooter : MonoBehaviour
{
    public GameObject laserPrefab;      // automatically assigned
    public float laserSpeed = 20f;
    public float minFireInterval = 0.5f;
    public float maxFireInterval = 2f;

    void Start()
    {
        Invoke(nameof(FireLaser), Random.Range(minFireInterval, maxFireInterval));
    }

    void FireLaser()
    {
        if (laserPrefab != null)
        {
            // Outward direction with slight random spread
            Vector3 dir = transform.forward + Random.insideUnitSphere * 0.2f;
            dir.Normalize();

            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);

            Rigidbody rb = laser.GetComponent<Rigidbody>();
            if (rb == null) rb = laser.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = dir * laserSpeed;

            Destroy(laser, 10f); // cleanup
        }

        // Schedule next shot
        Invoke(nameof(FireLaser), Random.Range(minFireInterval, maxFireInterval));
    }
}
