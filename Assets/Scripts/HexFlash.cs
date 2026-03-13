using UnityEngine;
using System.Collections;

public class HexAppear : MonoBehaviour
{
    public float duration = 0.6f;
    public float delayMultiplier = 0.002f;

    void Start()
    {
        StartCoroutine(Appear());
    }

    IEnumerator Appear()
    {
        // delay based on position so hexes cascade
        float delay = transform.GetSiblingIndex() * delayMultiplier;
        yield return new WaitForSecondsRealtime(delay);

        float t = 0;

        while (t < duration)
        {
            float scale = Mathf.Lerp(0f, 1f, t / duration);
            transform.localScale = Vector3.one * scale;

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}