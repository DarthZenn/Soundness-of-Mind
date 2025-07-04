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
            Debug.LogError("No pickupPrompt found in scene.");
    }

    private void Update()
    {
        if (objectInRange != null && Input.GetButtonDown("Interact") && !pickupPrompt.IsPromptActive())
        {
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
                        Debug.Log("Inventory full, nerd.");
                    }
                }
            }

            else if (objectInRange.CompareTag("Door"))
            {
                Animator doorAnim = objectInRange.GetComponent<Animator>();
                if (doorAnim != null)
                {
                    doorAnim.SetTrigger("Open");
                    Debug.Log("Door opening...");
                }
                else
                {
                    Debug.LogWarning("Door has no Animator. Just like your ideas have no depth.");
                }
            }

            else if (objectInRange.CompareTag("SceneDoor"))
            {
                SceneDoor sceneDoor = objectInRange.GetComponent<SceneDoor>();
                if (sceneDoor != null)
                {
                    StartCoroutine(sceneDoor.SwitchScene());
                    Debug.Log("Switching scene...");
                }
                else
                {
                    Debug.Log("SceneDoor missing. Like your common sense.");
                }
            }

            objectInRange = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable") || other.CompareTag("Door") || other.CompareTag("SceneDoor"))
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
