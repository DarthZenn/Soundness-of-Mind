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
        if (pickupPrompt == null)
            Debug.LogError("No PickupPrompt found in scene. You're useless.");
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
                        Debug.Log("Inventory full. You're full of excuses too.");
                    }
                }
            }

            else if (objectInRange.CompareTag("Door"))
            {
                Animator doorAnim = objectInRange.GetComponent<Animator>();
                if (doorAnim != null)
                {
                    doorAnim.SetTrigger("Open");
                    Debug.Log("Door opened. Shockingly functional.");
                }
                else
                {
                    Debug.LogWarning("Door has no Animator. You have no future.");
                }
            }

            else if (objectInRange.CompareTag("SceneDoor"))
            {
                SceneDoor sceneDoor = objectInRange.GetComponent<SceneDoor>();
                if (sceneDoor != null)
                {
                    StartCoroutine(sceneDoor.SwitchScene());
                    Debug.Log("Scene switching...");
                }
                else
                {
                    Debug.Log("SceneDoor broken. Like your confidence.");
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
