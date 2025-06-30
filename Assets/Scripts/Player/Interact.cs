using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private GameObject itemInRange;

    private void Update()
    {
        if (itemInRange != null && Input.GetButtonDown("Interact"))
        {
            ItemPickup pickup = itemInRange.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                Debug.Log("Picked up: " + pickup.item.itemName);
                bool added = FindObjectOfType<InventoryManager>().AddItem(pickup.item);

                if (added)
                {
                    Destroy(itemInRange);
                }
                else
                {
                    Debug.Log("Could not pick up item. Inventory full!");
                }
            }

            itemInRange = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            itemInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == itemInRange)
        {
            itemInRange = null;
        }
    }
}
