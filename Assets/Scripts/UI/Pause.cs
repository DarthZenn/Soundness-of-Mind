using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public AudioSource areaBGM;

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !GlobalControl.Instance.isInventoryOpen)
        {
            if (GlobalControl.Instance.isPause)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GlobalControl.Instance.isPause = true;
        areaBGM.Pause();
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GlobalControl.Instance.isPause = false;
        areaBGM.UnPause();
    }
}
