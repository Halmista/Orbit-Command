using UnityEngine;

public class Meteor : MonoBehaviour
{
    Vector3 direction;
    float speed;
    Gameplay gameplay;
    float damage;
    Vector3 earthPos;
    float earthRadius;
    public float hp = 20f; // health of meteor
    private GameObject impactRing;
    public GameObject shockwavePrefab;

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

        float meteorRadius = 1f; // adjust to your meteor size
        if (Vector3.Distance(transform.position, earthPos) <= earthRadius + meteorRadius)
        {
            gameplay.TakeDamage(damage);
            KillMeteor(true);
        }
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0f)
        {
            KillMeteor(true);
          
        }
    }
    public void SetImpactRing(GameObject ring)
    {
        impactRing = ring;
    }

    public void KillMeteor(bool destroyedByPlayer)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.DecrementActiveMeteors();

            if (destroyedByPlayer)
                UIManager.Instance.IncrementDestroyedMeteors();
        }
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);

        if (impactRing != null)
            Destroy(impactRing);
    }
}