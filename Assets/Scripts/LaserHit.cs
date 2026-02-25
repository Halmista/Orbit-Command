using UnityEngine;

public class LaserHit : MonoBehaviour
{
    public float damage = 10f;

    void OnTriggerEnter(Collider other)
    {
        Meteor meteor = other.GetComponent<Meteor>();
        if (meteor != null)
        {
            // Deal damage to meteor
            meteor.TakeDamage(damage);

            Destroy(gameObject); // destroy laser
        }
    }
}