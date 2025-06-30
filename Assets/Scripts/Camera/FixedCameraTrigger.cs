using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraTrigger : MonoBehaviour
{
    public Camera thisCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FixedCameraManager.Instance.SwitchToCamera(thisCamera);
        }
    }
}
