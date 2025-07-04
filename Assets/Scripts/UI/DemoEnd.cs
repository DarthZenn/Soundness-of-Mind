using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoEnd : MonoBehaviour
{
    public AudioSource buttonSound;
    public AudioSource areaBGM;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        areaBGM.Play();
    }

    void Update()
    {
        
    }

    public void MainMenuButton()
    {
        GlobalControl.Instance.playerHealth = 0;
        GlobalControl.Instance.currentAmmo = 0;
        GlobalControl.Instance.savedInventory.Clear();
        GlobalControl.Instance.currentWeapon = null;
        GlobalControl.Instance.destinationSpawnID = null;
        GlobalControl.Instance.isInventoryOpen = false;
        GlobalControl.Instance.isPause = false;
        GlobalControl.Instance.isPrompt = false;
        GlobalControl.Instance.isGameOver = false;
        if (buttonSound != null) 
        {
            buttonSound.Play();
        }
        SceneManager.LoadScene(0);
    }

    public void ExitButton()
    {
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        Application.Quit();
    }
}
