using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float expandSpeed = 8f;
    public float maxScale = 6f;
    public float fadeSpeed = 1f;

    private Vector3 startScale;
    private Renderer rend;
    private Color startColor;

    void Start()
    {
        startScale = transform.localScale;
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    void Update()
    {
        // Expand
        float scaleMultiplier = 1 + expandSpeed * Time.deltaTime;
        transform.localScale = new Vector3(
            transform.localScale.x * scaleMultiplier,
            transform.localScale.y,
            transform.localScale.z * scaleMultiplier
        );

        // Fade out
        Color c = rend.material.color;
        c.a -= fadeSpeed * Time.deltaTime;
        rend.material.color = c;

        // Destroy when faded or too big
        if (c.a <= 0f || transform.localScale.x >= maxScale)
        {
            Destroy(gameObject);
        }
    }
}