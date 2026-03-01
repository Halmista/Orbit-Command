using UnityEngine;

public class MeteorPathPulse : MonoBehaviour
{
    LineRenderer lr;
    GameObject trackedMeteor;

    float pulseSpeed = 1f;
    float flowSpeed = 0.4f;
    float baseWidth;

    float lifetime = 10f;   // max lifetime
    float timer = 0f;

    public void Initialize(GameObject meteor)
    {
        trackedMeteor = meteor;
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        baseWidth = lr.startWidth;
    }

    void Update()
    {
        // 🔥 If meteor destroyed → destroy instantly
        if (trackedMeteor == null)
        {
            Destroy(gameObject);
            return;
        }

        // ⏳ Lifetime timer
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Pulse width
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
        lr.startWidth = baseWidth * (0.8f + pulse * 0.4f);
        lr.endWidth = lr.startWidth * 0.5f;

        // Flow animation
        lr.material.mainTextureOffset -=
            new Vector2(flowSpeed * Time.deltaTime, 0);
    }
}