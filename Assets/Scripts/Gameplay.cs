using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    [Header("Wireframe Sphere")]
    public WireframeSphere wireframeSphere; // assign your WireframeSphere

    [Header("Earth Settings")]
    public Transform earthCenter;  // center of the Earth
    public float earthRadius = 1f; // radius of the Earth
    public float earthHP = 100f;  // starting HP

   // [Header("Meteor Settings")]
    //public float meteorDamage = 10f; // damage per meteor

    void Update()
    {
        if (wireframeSphere == null)
        {
            Debug.LogWarning("WireframeSphere reference not set!");
            return;
        }

        // Access vertices currently facing the camera
        List<Vector3> visibleVerts = wireframeSphere.verticesFacingCamera;

        if (visibleVerts == null || visibleVerts.Count == 0)
        {
           // Debug.Log("No vertices currently facing the camera.");
        }
        else
        {
            //Debug.Log($"Vertices facing camera ({visibleVerts.Count}):");
            for (int i = 0; i < visibleVerts.Count; i++)
            {
                Vector3 v = visibleVerts[i];
                //Debug.Log($"Vertex {i}: {v}");
            }
        }
    }

   
    // Call this when a meteor hits Earth
    
    public void TakeDamage(float dmg)
    {
        earthHP -= dmg;
        if (earthHP < 0f) earthHP = 0f;

        Debug.Log($"Earth HP: {earthHP}");

        if (earthHP <= 0f)
        {
            Debug.Log("Earth destroyed! Game Over.");
            // TODO: implement game over logic
        }
    }

  
    
}