using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitPivot : MonoBehaviour
{
    public float rotationAmount = 45f; // degrees per tap
    public float rotationSpeed = 180f; // degrees per second for smooth rotation

    private float targetRotationY;
    private bool isRotating = false;

    void Start()
    {
        targetRotationY = transform.eulerAngles.y;
    }

    void Update()
    {
        // Input detection (tap)
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                targetRotationY += rotationAmount; // clockwise
                isRotating = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightShift))
            {
                targetRotationY -= rotationAmount; // counter-clockwise
                isRotating = true;
            }
        }

        // Smooth rotation
        if (isRotating)
        {
            float currentY = transform.eulerAngles.y;
            float newY = Mathf.MoveTowardsAngle(currentY, targetRotationY, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newY, transform.eulerAngles.z);

            if (Mathf.Approximately(newY, targetRotationY))
            {
                isRotating = false; // done rotating
            }
        }
    }
}
