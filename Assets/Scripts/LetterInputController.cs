using UnityEngine;
using TMPro;
using System.Collections;

public class LetterInputController : MonoBehaviour
{
    public WireframeSphere wireframeSphere;
    public float fadeDuration = 0.25f; // seconds for fade out/in

    void Update()
    {
        if (Input.anyKeyDown)
        {
            string input = Input.inputString;
            if (string.IsNullOrEmpty(input)) return;

            char typed = char.ToLower(input[0]);

            if (wireframeSphere.letterToVertex.ContainsKey(typed))
            {
                Vector3 targetVertex = wireframeSphere.letterToVertex[typed];

                SatelliteShooter sat = SatelliteManager.Instance.GetNearestIdleSatellite(targetVertex);

                if (sat != null && wireframeSphere.activeLabels.TryGetValue(targetVertex, out GameObject letterObj))
                {
                    Vector3 satStartPos = sat.transform.position;
                    StartCoroutine(FadeSwap(letterObj, sat, targetVertex, satStartPos, typed));
                }

                wireframeSphere.letterToVertex.Remove(typed);
            }
        }
    }

    private IEnumerator FadeSwap(GameObject letterObj, SatelliteShooter sat, Vector3 letterVertex, Vector3 satPos, char typedLetter)
    {
        TMP_Text tmp = letterObj.GetComponent<TMP_Text>();
        if (tmp == null)
            yield break;

        Color originalColor = tmp.color;

        // Fade out
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, timer / fadeDuration));
            yield return null;
        }
        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Instant teleport
        Vector3 letterOldPos = letterObj.transform.position;
        letterObj.transform.position = satPos;
        sat.transform.position = letterVertex;

        // Update dictionaries
        wireframeSphere.activeLabels.Remove(letterVertex);
        wireframeSphere.activeLabels[satPos] = letterObj;
        wireframeSphere.letterToVertex[typedLetter] = satPos;

        // Fade in
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0f, 1f, timer / fadeDuration));
            yield return null;
        }
        tmp.color = originalColor;
    }
}