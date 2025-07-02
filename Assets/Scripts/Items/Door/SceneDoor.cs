using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneDoor : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private GameObject doorTransitionScreen;
    [SerializeField] private VideoPlayer doorTransition;
    [SerializeField] private AudioSource doorCloseSound;

    private int playerCurrentHealth;
    private GameObject currentWeapon;
    private int currentAmmo;
    private InventoryManager inventoryManager;

    public IEnumerator SwitchScene()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        TankController tankController = FindObjectOfType<TankController>();

        playerCurrentHealth = playerStats.GetCurrentHealth();
        currentAmmo = tankController.GetCurrentAmmo();

        GlobalControl.Instance.playerHealth = playerCurrentHealth;
        GlobalControl.Instance.currentWeapon = currentWeapon;
        GlobalControl.Instance.currentAmmo = currentAmmo;

        inventoryManager.GetCurrentInventorySlotData();
        inventoryManager.GetCurrentWeapon();

        if (doorTransition != null)
        {
            Time.timeScale = 0;

            doorTransition.Prepare();

            while (!doorTransition.isPrepared)
            {
                yield return null;
            }

            doorTransition.Play();

            while (!doorTransition.isPlaying)
            {
                yield return null;
            }

            doorTransitionScreen.SetActive(true);

            while (doorTransition.isPlaying)
            {
                yield return null;
            }
        }

        if (doorCloseSound != null)
        {
            doorCloseSound.Play();
            Time.timeScale = 1;
            yield return new WaitForSeconds(doorCloseSound.clip.length);
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
