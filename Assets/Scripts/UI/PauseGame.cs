using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public bool isPaused = false;
    public AudioSource areaBGM;
    public GameObject pause;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused == false)
            {
                Time.timeScale = 0;
                isPaused = true;
                areaBGM.Pause();
                pause.SetActive(true);
            }
            else
            {
                isPaused = false;
                areaBGM.UnPause();
                pause.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}
