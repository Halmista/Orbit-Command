using UnityEngine;

public class RingPulse : MonoBehaviour
{
    private Transform meteor;
    private Transform earth;
    private Gameplay gameplay;
    private float maxDistance;

    public float minScale = 0.3f;
    public float maxScale = 1.5f;

    [Header("Danger Settings")]
    public float flashThreshold = 0.8f;     // when to start flashing (80% close)
    public float flashSpeed = 10f;

    [Header("Fade Settings")]
    public float fadeSpeed = 2f;            // speed of fade-out when Earth destroyed

    private Vector3 baseScale;
    private Renderer ringRenderer;
    private Color baseColor;
    private bool isFadingOut = false;

    public void Initialize(Transform meteorTransform, Transform earthTransform, Gameplay gameplayRef)
    {
        meteor = meteorTransform;
        earth = earthTransform;
        gameplay = gameplayRef;

        if (meteor == null || earth == null)
        {
            Debug.LogWarning("RingPulse: meteor or earth not assigned!");
            return;
        }

        maxDistance = Vector3.Distance(meteor.position, earth.position);
        baseScale = transform.localScale;

        ringRenderer = GetComponent<Renderer>();
        baseColor = Color.yellow;
        ringRenderer.material = new Material(ringRenderer.material); // instance material so fading doesn't affect others
    }

    void Update()
    {
        if (ringRenderer == null) return;

        // Destroy/fade if meteor or earth gone
        if (meteor == null || earth == null)
        {
            StartFadeOut();
        }

        // Fade out if Earth destroyed
        if (gameplay != null && gameplay.earthHP <= 0f)
        {
            StartFadeOut();
        }

        if (isFadingOut)
        {
            FadeOut();
            return; // skip normal updates while fading
        }

        // NORMAL SCALE + COLOR PULSE
        float currentDistance = Vector3.Distance(meteor.position, earth.position);
        float progress = Mathf.Clamp01(1f - (currentDistance / maxDistance));

        // SCALE (flat on ground)
        float scale = Mathf.Lerp(minScale, maxScale, progress);
        transform.localScale = new Vector3(baseScale.x * scale, baseScale.y, baseScale.z * scale);

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

    private void StartFadeOut()
    {
        isFadingOut = true;
    }

    private void FadeOut()
    {
        Color c = ringRenderer.material.color;
        c.a -= fadeSpeed * Time.deltaTime;
        if (c.a <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        ringRenderer.material.color = c;
    }
}