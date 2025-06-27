using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    public GameObject cameraOn;
    public GameObject cameraOff;
    public bool camOn = false;
    public int camNumber;

    // Start is called before the first frame update
    void Start()
    {
        camNumber = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            cameraOn.SetActive(true);
            cameraOff.SetActive(false);
        }
    }
}
