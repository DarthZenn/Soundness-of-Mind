using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraManager : MonoBehaviour
{
    public static FixedCameraManager Instance;

    private List<Camera> cameras = new List<Camera>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Camera[] allCams = Camera.allCameras;
        foreach (Camera cam in allCams)
        {
            if (cam.CompareTag("MainCamera"))
            {
                cam.gameObject.SetActive(false);
                cameras.Add(cam);
            }
        }
    }

    public void SwitchToCamera(Camera cam)
    {
        foreach (Camera c in cameras)
        {
            c.gameObject.SetActive(false);
        }

        cam.gameObject.SetActive(true);

        CameraLookAtTarget lookScript = cam.GetComponent<CameraLookAtTarget>();
        if (lookScript != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lookScript.target = player.transform;
            }
        }
    }
}
