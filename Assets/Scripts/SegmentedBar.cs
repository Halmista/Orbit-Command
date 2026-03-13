using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SegmentedBar : MonoBehaviour
{
    public GameObject segmentPrefab;
    public Transform segmentContainer;

    public int totalSegments = 30;

    [Header("Normal Colors")]
    public Color startColor = Color.cyan;
    public Color endColor = Color.green;

    [Header("Warning Colors")]
    public Color warningColor = Color.yellow;
    public Color criticalColor = Color.red;

    [Header("Thresholds")]
    public float warningThreshold = 0.5f;
    public float criticalThreshold = 0.25f;

    [Header("Dim Settings")]
    public float inactiveAlpha = 0.2f;

    [Header("Pulse Settings")]
    public float pulseSpeed = 5f;

    private List<Image> segments = new();
    private List<Color> baseColors = new();

    private float currentPercent = 1f;

    void Start()
    {
        GenerateSegments();
    }

    void Update()
    {
        HandleCriticalPulse();
    }

    void GenerateSegments()
    {
        for (int i = 0; i < totalSegments; i++)
        {
            GameObject seg = Instantiate(segmentPrefab, segmentContainer);
            Image img = seg.GetComponent<Image>();

            float t = i / (float)totalSegments;
            Color col = Color.Lerp(startColor, endColor, t);

            img.color = col;

            segments.Add(img);
            baseColors.Add(col);
        }
    }

    public void SetPercent(float percent)
    {
        currentPercent = Mathf.Clamp01(percent);

        int active = Mathf.FloorToInt(currentPercent * totalSegments);

        for (int i = 0; i < segments.Count; i++)
        {
            Color c;

            if (currentPercent <= criticalThreshold)
                c = criticalColor;
            else if (currentPercent <= warningThreshold)
                c = warningColor;
            else
                c = baseColors[i];

            if (i < active)
                c.a = 1f;
            else
                c.a = inactiveAlpha;

            segments[i].color = c;
        }
    }

    void HandleCriticalPulse()
    {
        if (currentPercent > criticalThreshold) return;

        float pulse = Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));

        int active = Mathf.FloorToInt(currentPercent * totalSegments);

        for (int i = 0; i < active; i++)
        {
            Color c = Color.Lerp(criticalColor, Color.white, pulse);
            c.a = 1f;
            segments[i].color = c;
        }
    }
}