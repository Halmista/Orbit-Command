using UnityEngine;

public class LetterInputController : MonoBehaviour
{
    public WireframeSphere wireframeSphere;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            string input = Input.inputString;
            if (string.IsNullOrEmpty(input)) return;

            char typed = char.ToLower(input[0]);

            Debug.Log("Typed: " + typed);

            if (wireframeSphere.letterToVertex.ContainsKey(typed))
            {
                Vector3 targetVertex = wireframeSphere.letterToVertex[typed];

                SatelliteShooter sat =
                    SatelliteManager.Instance.GetNearestIdleSatellite(targetVertex);

                if (sat != null)
                {
                    Vector3 satelliteStartPos = sat.transform.position;

                    // Get the letter object at that vertex
                    if (wireframeSphere.activeLabels.TryGetValue(targetVertex, out GameObject letterObj))
                    {
                        // Start swap coroutine
                        StartCoroutine(SwapPositions(sat, letterObj, typed, targetVertex, satelliteStartPos));
                    }
                }

                wireframeSphere.letterToVertex.Remove(typed);
            }

            /*if (wireframeSphere.letterToVertex.ContainsKey(typed))
            {
                Debug.Log("Letter found!");
                Vector3 targetVertex = wireframeSphere.letterToVertex[typed];

                SatelliteShooter sat =
                    SatelliteManager.Instance.GetNearestIdleSatellite(targetVertex);

                if (sat != null)
                {
                    sat.StartCoroutine(MoveSatellite(sat, targetVertex));
                }

                wireframeSphere.letterToVertex.Remove(typed);
            }*/
        }
    }

    /*System.Collections.IEnumerator MoveSatellite(SatelliteShooter sat, Vector3 target)
    {
        float speed = 5f;

        while (Vector3.Distance(sat.transform.position, target) > 0.1f)
        {
            sat.transform.position =
                Vector3.MoveTowards(sat.transform.position, target, speed * Time.deltaTime);

            yield return null;
        }
    }*/

    System.Collections.IEnumerator SwapPositions(
    SatelliteShooter sat,
    GameObject letterObj,
    char typedLetter,
    Vector3 originalVertex,
    Vector3 satStartPos)
    {
        float speed = 5f;

        while (Vector3.Distance(sat.transform.position, originalVertex) > 0.1f)
        {
            sat.transform.position =
                Vector3.MoveTowards(sat.transform.position, originalVertex, speed * Time.deltaTime);

            letterObj.transform.position =
                Vector3.MoveTowards(letterObj.transform.position, satStartPos, speed * Time.deltaTime);

            yield return null;
        }

        sat.transform.position = originalVertex;
        letterObj.transform.position = satStartPos;

        // 🔥 Update activeLabels
        wireframeSphere.activeLabels.Remove(originalVertex);
        wireframeSphere.activeLabels[satStartPos] = letterObj;

        // 🔥 Update letterToVertex
        wireframeSphere.letterToVertex[typedLetter] = satStartPos;
    }
}