using UnityEngine;

public class RingPulse : MonoBehaviour
{
    private Transform meteor;
    private Transform earth;
    private float maxDistance;

    public float minScale = 0.3f;
    public float maxScale = 1.5f;

    [Header("Danger Settings")]
    public float flashThreshold = 0.8f;     // when to start flashing (80% close)
    public float flashSpeed = 10f;

    private Vector3 baseScale;
    private Renderer ringRenderer;
    private Color baseColor;

    public void Initialize(Transform meteorTransform, Transform earthTransform)
    {
        meteor = meteorTransform;
        earth = earthTransform;

        maxDistance = Vector3.Distance(meteor.position, earth.position);
        baseScale = transform.localScale;

        ringRenderer = GetComponent<Renderer>();
        baseColor = Color.yellow;
    }

    void Update()
    {
        if (meteor == null || earth == null)
            return;

        float currentDistance = Vector3.Distance(meteor.position, earth.position);
        float progress = Mathf.Clamp01(1f - (currentDistance / maxDistance));

        // SCALE (flat on ground)
        float scale = Mathf.Lerp(minScale, maxScale, progress);

        transform.localScale = new Vector3(
            baseScale.x * scale,
            baseScale.y,
            baseScale.z * scale
        );

        // COLOR SHIFT (Yellow → Red)
        Color dangerColor = Color.Lerp(Color.yellow, Color.red, progress);

    
        // FLASH when near impact
        if (progress >= flashThreshold)
        {
            float flash = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            dangerColor = Color.Lerp(dangerColor, Color.white, flash);
        }

        ringRenderer.material.color = dangerColor;
    }
}