using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventory;
    public AudioSource inventoryOpen;
    public AudioSource inventoryClose;
    public bool isOpen = false;
    public bool canClose = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Inventory") && isOpen == false && canClose == false)
        {
            isOpen = true;
            inventoryOpen.Play();
            StartCoroutine(InventoryControl());
        }

        if (Input.GetButton("Inventory") && isOpen == true && canClose == true)
        {
            Time.timeScale = 1;
            isOpen = false;
            inventoryClose.Play();
            StartCoroutine(InventoryControl());
        }
    }

    public void ExitButton()
    {
        Time.timeScale = 1;
        isOpen = false;
        inventoryClose.Play();
        StartCoroutine(InventoryControl());
    }

    IEnumerator InventoryControl()
    {
        if (isOpen == true)
        {
            inventory.SetActive(true);
        }
        else
        {
            inventory.SetActive(false);
        }

        yield return new WaitForSeconds(0.25f);

        if (isOpen == true)
        {
            Cursor.visible = true;
            Time.timeScale = 0;
            canClose = true;
        }
        else
        {
            canClose = false;
        }
    }
}
