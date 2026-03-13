using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupHexReveal : MonoBehaviour
{
    public Transform hexContainer;
    public float delayBetweenHex = 0.01f;

    public float gridBuildTime = 1.5f; // wait for hex appear animation

    void Start()
    {
        Time.timeScale = 0f;
        StartCoroutine(RevealEarth());
    }

    IEnumerator RevealEarth()
    {
        // Wait until hex grid finishes forming
        yield return new WaitForSecondsRealtime(gridBuildTime);

        List<Transform> hexes = new List<Transform>();

        foreach (Transform hex in hexContainer)
            hexes.Add(hex);

        Vector3 center = hexContainer.GetComponent<RectTransform>().rect.center;

        hexes.Sort((a, b) =>
        {
            float da = Vector3.Distance(a.localPosition, center);
            float db = Vector3.Distance(b.localPosition, center);
            return da.CompareTo(db);
        });

        int hexesPerFrame = 2;

        for (int i = 0; i < hexes.Count; i += hexesPerFrame)
        {
            for (int j = 0; j < hexesPerFrame && i + j < hexes.Count; j++)
            {
                hexes[i + j].gameObject.SetActive(false);
            }

            yield return null;
        }

        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}