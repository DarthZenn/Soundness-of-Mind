using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] private GameObject gameOver;
    private bool isGameOver = false;
    private int playerHealth;
    public AudioSource buttonSound;
    public AudioSource dyingAmbience;
    public AudioSource areaBGM;

    public void HandleGameOver()
    {
        PlayerStats playerCurrentHealth = FindObjectOfType<PlayerStats>();
        playerHealth = playerCurrentHealth.GetCurrentHealth();

        if (playerHealth <= 0 && isGameOver == false)
        {
            isGameOver = true;
            gameOver.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            areaBGM.Stop();
            dyingAmbience.Play();
            GlobalControl.Instance.isGameOver = true;
        }
        else
        {
            return;
        }
    }

    public void LoadGameButton()
    {
        buttonSound.Play();
    }

    public void MainMenuButton()
    {
        isGameOver = false;
        GlobalControl.Instance.playerHealth = 0;
        GlobalControl.Instance.currentAmmo = 0;
        GlobalControl.Instance.savedInventory.Clear();
        GlobalControl.Instance.currentWeapon = null;
        GlobalControl.Instance.destinationSpawnID = null;
        GlobalControl.Instance.isInventoryOpen = false;
        GlobalControl.Instance.isPause = false;
        GlobalControl.Instance.isPrompt = false;
        GlobalControl.Instance.isGameOver = false;
        buttonSound.Play();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ExitButton()
    {
        buttonSound.Play();
        Application.Quit();
    }
}
