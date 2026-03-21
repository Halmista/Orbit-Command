using UnityEngine;

public class Meteor : MonoBehaviour
{
    Vector3 direction;
    float speed;
    Gameplay gameplay;
    float damage;
    Vector3 earthPos;
    float earthRadius;
    public float maxHP = 20f;
    public float hp; // health of meteor
    private GameObject impactRing;
    public GameObject shockwavePrefab;
    private Transform healthBarRoot;
    private Transform healthBarFill;


    public void Initialize(Vector3 dir, float spd, Gameplay game, float dmg, Vector3 ePos, float eRadius)
    {
        direction = dir;
        speed = spd;
        gameplay = game;
        damage = dmg;
        earthPos = ePos;
        earthRadius = eRadius;
    }
    void Start()
    {
        hp = maxHP;
        CreateHealthBar();
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
        if (healthBarRoot != null && Camera.main != null)
        {
            healthBarRoot.rotation =
                Quaternion.LookRotation(
                    healthBarRoot.position - Camera.main.transform.position
                );
        }
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        UpdateHealthBar();
        if (hp <= 0f)
        {
            KillMeteor(true);
          
        }
    }
    public void SetImpactRing(GameObject ring)
    {
        impactRing = ring;
    }

    void CreateHealthBar()
    {
        // Root object
        GameObject root = new GameObject("HealthBar");
        root.transform.SetParent(transform);
      

        Renderer r = GetComponentInChildren<Renderer>();

        if (r != null)
        {
            //float height = r.bounds.extents.y;
            float height = r.bounds.max.y - transform.position.y;
            root.transform.localPosition = Vector3.up * (height + 0.8f);
        }
        else
        {
            root.transform.localPosition = Vector3.up * 1f; // fallback
        }


        healthBarRoot = root.transform;

        // Background
        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bg.transform.SetParent(root.transform);
        bg.transform.localPosition = Vector3.zero;
        //bg.transform.localScale = new Vector3(1.2f, 0.2f, 1f);
        bg.transform.localScale = new Vector3(1f, 0.2f, 1f);

        Material bgMat = new Material(Shader.Find("Unlit/Color"));
        bgMat.color = new Color(0, 0, 0, 0.6f);
        bg.GetComponent<MeshRenderer>().material = bgMat;

        // Fill
        GameObject fill = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fill.transform.SetParent(root.transform);
        //fill.transform.localPosition = new Vector3(-0.1f, 0f, -0.01f);
        fill.transform.localScale = new Vector3(1f, 0.15f, 1f);
        fill.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        //fill.transform.localScale = new Vector3(1f, 0.15f, 1f);

        Material fillMat = new Material(Shader.Find("Unlit/Color"));
        fillMat.color = Color.green;
        fill.GetComponent<MeshRenderer>().material = fillMat;

        healthBarFill = fill.transform;

        // Remove colliders (not needed)
        Destroy(bg.GetComponent<Collider>());
        Destroy(fill.GetComponent<Collider>());
    }

    void UpdateHealthBar()
    {
        float percent = Mathf.Clamp01(hp / maxHP);

        healthBarFill.localScale = new Vector3(percent, 0.15f, 1f);
        healthBarFill.localPosition = new Vector3(-(1f - percent) * 0.5f, 0f, -0.01f);

        Color color = Color.Lerp(Color.red, Color.green, percent);
        healthBarFill.GetComponent<MeshRenderer>().material.color = color;
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

        if (XPManager.Instance != null)
        {
            float xpMultiplier = destroyedByPlayer ? 1f : 0.5f;

            float xp = Mathf.Clamp(maxHP * 0.5f, 3f, 25f) * xpMultiplier;

            XPManager.Instance.AddXP(xp);
        }

        Destroy(gameObject);
        

        if (impactRing != null)
            Destroy(impactRing);
    }
}