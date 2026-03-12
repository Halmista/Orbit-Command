using UnityEngine;

public class EarthAutoRotate : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float axialTilt = 23.4f;

    void Start()
    {
        // Apply Earth's axial tilt
        transform.rotation = Quaternion.Euler(axialTilt, 0f, 0f);
    }

    void Update()
    {
        // Rotate around the tilted axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}