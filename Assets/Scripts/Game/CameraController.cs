using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float damping;
    private Vector3 vel = Vector3.zero;

    void Start ()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = transform.position + offset;
        targetPosition.z = mainCamera.transform.position.z;

        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref vel, damping);
    }
}
