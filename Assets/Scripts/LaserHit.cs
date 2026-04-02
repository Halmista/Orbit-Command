using UnityEngine;

public class LaserHit : MonoBehaviour
{
    public float speed = 25f;
    public GameObject laserPrefab; // assign your laser prefab in inspector
    [HideInInspector] public int remainingBounces = 0;
    [HideInInspector] public Meteor lastMeteorHit;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Start()
    {
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        Meteor meteor = other.GetComponent<Meteor>();
        if (meteor == null) return;

        // Damage meteor
        meteor.TakeDamage(LaserStats.damage);

        // Bounce logic
        if (remainingBounces > 0)
        {
            Meteor nextMeteor = FindNearestMeteor(meteor.transform.position);
            if (nextMeteor != null)
            {
                SpawnBounce(nextMeteor, meteor);
            }
        }

        Destroy(gameObject); // destroy this laser regardless
    }

    void SpawnBounce(Meteor nextMeteor, Meteor justHit)
    {
        if (laserPrefab == null) return;

        GameObject newLaser = Instantiate(laserPrefab, justHit.transform.position,
            Quaternion.LookRotation(nextMeteor.transform.position - justHit.transform.position));

        LaserHit newHit = newLaser.GetComponent<LaserHit>();
        newHit.remainingBounces = remainingBounces - 1;
        newHit.lastMeteorHit = justHit; // prevent immediate back-bounce
        newHit.speed = speed;
        newHit.laserPrefab = laserPrefab;

        Rigidbody newRb = newLaser.GetComponent<Rigidbody>();
        if (newRb != null)
        {
            Vector3 dir = (nextMeteor.transform.position - justHit.transform.position).normalized;
            newRb.velocity = dir * speed;
        }

        Destroy(newLaser, 5f); // safety
    }

    Meteor FindNearestMeteor(Vector3 pos)
    {
        Meteor[] meteors = FindObjectsOfType<Meteor>();
        float closestDist = Mathf.Infinity;
        Meteor target = null;

        foreach (Meteor m in meteors)
        {
            if (m == lastMeteorHit) continue;

            float dist = Vector3.Distance(pos, m.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                target = m;
            }
        }

        return target;
    }
}