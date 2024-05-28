using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam; // The transform of the camera

    void LateUpdate()
    {
        // Make the canvas face the camera
        Vector3 direction = cam.position - transform.position;
        direction.x = direction.z = 0.0f; // Keep only the y-axis rotation
        transform.LookAt(cam.position - direction); // Correct the rotation
        transform.Rotate(0, 180, 0); // Optionally, adjust if facing the wrong way
    }
}
