using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtTarget : MonoBehaviour
{
    public Transform target;

    public float minVerticalAngle;
    public float maxVerticalAngle;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 euler = targetRotation.eulerAngles;

        if (euler.x > 180f) euler.x -= 360f;
        euler.x = Mathf.Clamp(euler.x, minVerticalAngle, maxVerticalAngle);
        euler.z = 0f;

        transform.rotation = Quaternion.Euler(euler);
    }
}