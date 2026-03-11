using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlatMinimapUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform minimapPanel;
    public RectTransform letterContainer;
    public RectTransform satelliteContainer;
    public RectTransform meteorContainer;

    public WireframeSphere wireframeSphere;
    public Transform earthCenter;
    public List<Transform> satellites;

    [Header("Prefabs")]
    public GameObject letterPrefab;
    public GameObject satellitePrefab;
    public GameObject meteorRingPrefab;

    [Header("Settings")]
    public float sphereRadius = 1.2f;

    private Dictionary<char, GameObject> activeLetters = new();
    private Dictionary<Transform, GameObject> activeSatIcons = new();
    private List<GameObject> activeRings = new();

    void Start()
    {
        // Make sure vertices exist
        if (wireframeSphere != null)
        {
            wireframeSphere.CalculateVertices();
            wireframeSphere.UpdateVerticesFacingCamera();
            wireframeSphere.UpdateLettersFacingCamera();
        }

        // Spawn letters & satellites
        SpawnLetters();
        SpawnSatellites();
    }

    void Update()
    {
        // Keep letters & satellites updated
        UpdateLetters();
        UpdateSatellites();
        UpdateMeteorRings();
    }

    #region Letters
    void SpawnLetters()
    {
        ClearContainer(letterContainer);
        activeLetters.Clear();

        if (wireframeSphere == null || letterPrefab == null) return;

        wireframeSphere.UpdateVerticesFacingCamera();
        wireframeSphere.UpdateLettersFacingCamera();

        foreach (var kvp in wireframeSphere.letterToVertex)
        {
            Vector3 worldPos = kvp.Value;
            Vector2 pos = WorldToMinimap(worldPos);
            if (!IsInsideMinimap(pos)) continue;

            GameObject letterObj = Instantiate(letterPrefab, letterContainer);
            letterObj.GetComponent<RectTransform>().anchoredPosition = pos;
            TMP_Text tmp = letterObj.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = kvp.Key.ToString();

            activeLetters[kvp.Key] = letterObj;
        }
    }

    void UpdateLetters()
    {
        if (wireframeSphere == null) return;

        wireframeSphere.UpdateVerticesFacingCamera();
        wireframeSphere.UpdateLettersFacingCamera();

        foreach (var kvp in wireframeSphere.letterToVertex)
        {
            if (!activeLetters.ContainsKey(kvp.Key))
            {
                GameObject letterObj = Instantiate(letterPrefab, letterContainer);
                TMP_Text tmp = letterObj.GetComponent<TMP_Text>();
                if (tmp != null) tmp.text = kvp.Key.ToString();
                activeLetters[kvp.Key] = letterObj;
            }

            Vector2 pos = WorldToMinimap(kvp.Value);
            if (!IsInsideMinimap(pos)) continue;
            activeLetters[kvp.Key].GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
    #endregion

    #region Satellites
    void SpawnSatellites()
    {
        ClearContainer(satelliteContainer);
        activeSatIcons.Clear();

        if (satellites == null || satellitePrefab == null) return;

        foreach (var sat in satellites)
        {
            GameObject icon = Instantiate(satellitePrefab, satelliteContainer);
            activeSatIcons[sat] = icon;
        }
    }

    void UpdateSatellites()
    {
        foreach (var kvp in activeSatIcons)
        {
            if (kvp.Key == null || kvp.Value == null) continue;
            Vector2 pos = WorldToMinimap(kvp.Key.position);
            if (!IsInsideMinimap(pos)) continue;
            kvp.Value.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
    #endregion

    #region Meteor Rings
    public void SpawnMeteorRing(Vector3 meteorPos, float size = 1f)
    {
        if (meteorRingPrefab == null || meteorContainer == null) return;

        Vector2 pos = WorldToMinimap(meteorPos);
        if (!IsInsideMinimap(pos)) return;

        GameObject ring = Instantiate(meteorRingPrefab, meteorContainer);
        RectTransform rt = ring.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.localScale = Vector3.one * size;
        activeRings.Add(ring);
    }

    void UpdateMeteorRings()
    {
        activeRings.RemoveAll(r => r == null);
    }
    #endregion

    #region Utils
    Vector2 WorldToMinimap(Vector3 worldPos)
    {
        Vector3 offset = worldPos - earthCenter.position;

        // Only front-facing vertices
        Vector3 camForward = Camera.main.transform.forward;
        float facing = Vector3.Dot(camForward, offset.normalized);
        if (facing < 0.2f) return Vector2.positiveInfinity;

        Vector2 norm = new Vector2(offset.x / sphereRadius, offset.z / sphereRadius);
        Vector2 halfSize = minimapPanel.rect.size * 0.5f;
        return new Vector2(norm.x * halfSize.x, norm.y * halfSize.y);
    }

    bool IsInsideMinimap(Vector2 pos)
    {
        return pos != Vector2.positiveInfinity &&
               Mathf.Abs(pos.x) <= minimapPanel.rect.width * 0.5f &&
               Mathf.Abs(pos.y) <= minimapPanel.rect.height * 0.5f;
    }

    void ClearContainer(RectTransform container)
    {
        if (container == null) return;
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }
    #endregion
}