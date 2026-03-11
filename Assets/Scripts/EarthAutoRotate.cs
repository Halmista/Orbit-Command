using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthAutoRotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 0.5f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
