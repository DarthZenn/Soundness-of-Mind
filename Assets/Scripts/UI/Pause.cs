using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public AudioSource areaBGM;
    public AudioSource buttonSound;

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !GlobalControl.Instance.isInventoryOpen && !GlobalControl.Instance.isPrompt && !GlobalControl.Instance.isGameOver)
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
        buttonSound.Play();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GlobalControl.Instance.isPause = false;
        areaBGM.UnPause();
    }

    public void LoadGameButton()
    {
        buttonSound.Play();
    }

    public void SettingsButton()
    {
        buttonSound.Play();
    }

    public void MainMenuButton()
    {
        buttonSound.Play();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GlobalControl.Instance.playerHealth = 0;
        GlobalControl.Instance.currentAmmo = 0;
        GlobalControl.Instance.savedInventory.Clear();
        GlobalControl.Instance.currentWeapon = null;
        GlobalControl.Instance.destinationSpawnID = null;
        GlobalControl.Instance.isInventoryOpen = false;
        GlobalControl.Instance.isPause = false;
        GlobalControl.Instance.isPrompt = false;
        GlobalControl.Instance.isGameOver = false;
        SceneManager.LoadScene(0);
    }

    public void ExitButton()
    {
        buttonSound.Play();
        Application.Quit();
    }
}
