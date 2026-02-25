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
                Debug.Log("Letter found!");
                Vector3 targetVertex = wireframeSphere.letterToVertex[typed];

                SatelliteShooter sat =
                    SatelliteManager.Instance.GetNearestIdleSatellite(targetVertex);

                if (sat != null)
                {
                    sat.StartCoroutine(MoveSatellite(sat, targetVertex));
                }

                wireframeSphere.letterToVertex.Remove(typed);
            }
        }
    }

    System.Collections.IEnumerator MoveSatellite(SatelliteShooter sat, Vector3 target)
    {
        float speed = 5f;

        while (Vector3.Distance(sat.transform.position, target) > 0.1f)
        {
            sat.transform.position =
                Vector3.MoveTowards(sat.transform.position, target, speed * Time.deltaTime);

            yield return null;
        }
    }
}