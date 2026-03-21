using UnityEngine;

public class LaserHit : MonoBehaviour
{
    public float speed = 25f; 
    int remainingBounces; 
    Meteor lastMeteorHit;

    void Start() 
    { 
        remainingBounces = LaserStats.bounces; 
    
    }
    void Update() 
    { 
        transform.position += transform.forward * speed * Time.deltaTime; 
    }

    void OnTriggerEnter(Collider other)
    {
        Meteor meteor = other.GetComponent<Meteor>();
        if (meteor == null) return;

        // Damage meteor
        meteor.TakeDamage(LaserStats.damage);
        lastMeteorHit = meteor;

        if (remainingBounces > 0)
        {
            Meteor nextMeteor = FindNearestMeteor(meteor.transform.position);

            if (nextMeteor != null)
            {
                // Spawn fresh laser from prefab
             
                    GameObject newLaser = Instantiate(gameObject, meteor.transform.position, Quaternion.LookRotation(nextMeteor.transform.position - meteor.transform.position)); 
                    LaserHit laser = newLaser.GetComponent<LaserHit>(); 
                    laser.remainingBounces = remainingBounces - 1; 
                    Destroy(gameObject); 
                    return;
                
            }
        }

        // No bounce or no next target
        Destroy(gameObject);
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