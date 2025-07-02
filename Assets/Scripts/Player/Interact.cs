using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private GameObject objectInRange;

    private void Update()
    {
        if (objectInRange != null && Input.GetButtonDown("Interact"))
        {
            if (objectInRange.CompareTag("Pickable"))
            {
                ItemPickup pickup = objectInRange.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    Debug.Log("Picked up: " + pickup.item.itemName);
                    bool added = FindObjectOfType<InventoryManager>().AddItem(pickup.item);

                    if (added)
                    {
                        Destroy(objectInRange);
                    }
                    else
                    {
                        Debug.Log("Could not pick up item. Inventory full!");
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
                    Debug.LogWarning("Door object has no Animator, you brainless buffoon.");
                }
            }

            else if (objectInRange.CompareTag("SceneDoor"))
            {
                SceneDoor sceneDoor = objectInRange.GetComponent<SceneDoor>();
                if (sceneDoor != null)
                {
                    StartCoroutine(sceneDoor.SwitchScene());
                    Debug.Log("Switching Scene...");
                }
                else
                {
                    Debug.Log("U ain't going no where. Come over here and kiss me on my hot mouth.");
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
