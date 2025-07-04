using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private GameObject objectInRange;
    private PickupPrompt pickupPrompt;

    private void Start()
    {
        pickupPrompt = FindObjectOfType<PickupPrompt>();
    }

    private void Update()
    {
        if (objectInRange != null && Input.GetButtonDown("Interact"))
        {
            if (pickupPrompt != null && pickupPrompt.IsPromptActive()) return;

            if (objectInRange.CompareTag("Pickable"))
            {
                ItemPickup pickup = objectInRange.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    bool added = FindObjectOfType<InventoryManager>().AddItem(pickup.item);

                    if (added)
                    {
                        pickupPrompt.ShowPrompt(pickup.item.itemName, pickup.item.quantityPerPickup);
                        Destroy(objectInRange);
                    }
                    else
                    {
                        pickupPrompt.ShowPromptFull();
                    }
                }
            }

            else if (objectInRange.CompareTag("Door"))
            {
                Animator doorAnim = objectInRange.GetComponent<Animator>();
                if (doorAnim != null)
                {
                    doorAnim.SetTrigger("Open");
                }
            }

            else if (objectInRange.CompareTag("SceneDoor"))
            {
                SceneDoor sceneDoor = objectInRange.GetComponent<SceneDoor>();
                if (sceneDoor != null)
                {
                    StartCoroutine(sceneDoor.SwitchScene());
                }
            }

            else if (objectInRange.CompareTag("Saver"))
            {
                SaveController saveController = FindObjectOfType<SaveController>();
                saveController.ShowSavePrompt();
            }

            objectInRange = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable") || other.CompareTag("Door") ||
            other.CompareTag("SceneDoor") || other.CompareTag("Saver"))
        {
            objectInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectInRange)
        {
            objectInRange = null;
        }
    }
}
