using UnityEngine;

public class Meteor : MonoBehaviour
{
    Vector3 direction;
    float speed;
    Gameplay gameplay;
    float damage;
    Vector3 earthPos;
    float earthRadius;

    public void Initialize(Vector3 dir, float spd, Gameplay game, float dmg, Vector3 ePos, float eRadius)
    {
        direction = dir;
        speed = spd;
        gameplay = game;
        damage = dmg;
        earthPos = ePos;
        earthRadius = eRadius;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Check if meteor reached Earth
        if (Vector3.Distance(transform.position, earthPos) <= earthRadius)
        {
            gameplay.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}