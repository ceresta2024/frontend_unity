using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//camera clamp x2, x27, ,z4.5f, z24.5f

public class MS_CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public Camera mainCamera;
    public float margin = 0.1f; // Margin value for all sides

    void Update()
    {
        // Ensure playerTransform and mainCamera are assigned
        if (playerTransform == null || mainCamera == null)
        {
            Debug.LogWarning("Assign player transform and main camera in the inspector!");
            return;
        }

        // Convert player position to viewport space
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(playerTransform.position);

        // Add margins
        float leftMargin = margin;
        float rightMargin = 1 - margin;
        float bottomMargin = margin / 2;
        float topMargin = 1 - margin;

        // Check if player is outside camera view including margins
        if (viewportPos.x < leftMargin || viewportPos.x > rightMargin ||
            viewportPos.y < bottomMargin || viewportPos.y > topMargin ||
            viewportPos.z < 0)
        {
            //Debug.Log("Player is outside camera view!");

            if (playerTransform.position.y < 2)
            {
                if (playerTransform.position.x <= -12)
                    transform.DOMove(new Vector3(-13, -4.5f, transform.position.z), 0.5f);
                else if (playerTransform.position.x >= 8)
                    transform.DOMove(new Vector3(12, -4.5f, transform.position.z), 0.5f);
                else
                    transform.DOMove(new Vector3(playerTransform.position.x, -4.5f, transform.position.z), 0.5f);
            }
            else
            {
                if (playerTransform.position.x <= -12)
                    transform.DOMove(new Vector3(-13, 5.5f, transform.position.z), 0.5f);
                else if (playerTransform.position.x >= 8)
                    transform.DOMove(new Vector3(12, 5.5f, transform.position.z), 0.5f);
                else
                    transform.DOMove(new Vector3(playerTransform.position.x, 5.5f, transform.position.z), 0.5f);
            }
            // Implement your logic here, such as stopping player movement, showing a message, etc.
        }
    }
}
