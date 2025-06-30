using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public AudioSource inventoryOpenSFX;
    public AudioSource inventoryCloseSFX;

    public InventorySlot[] slots;
    public TMPro.TextMeshProUGUI itemNameText;
    public TMPro.TextMeshProUGUI itemDescriptionText;
    public GameObject optionsPanel;

    public Transform handgunHolder;

    private bool isInventoryOpen = false;
    private InventorySlot selectedSlot;

    void Start()
    {
        foreach (var slot in slots)
        {
            slot.SetInventoryManager(this);
        }

        ClearItemInfo();
        optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory") && !GlobalControl.isPause)
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        GlobalControl.isInventoryOpen = isInventoryOpen;

        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inventoryOpenSFX.Play();
        }
        else
        {
            isInventoryOpen = false;
            GlobalControl.isInventoryOpen = false;

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inventoryCloseSFX.Play();

            if (selectedSlot != null)
            {
                selectedSlot.SetSelected(false);
                selectedSlot = null;
            }

            ClearItemInfo();
            optionsPanel.SetActive(false);
            selectedSlot = null;
        }
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        GlobalControl.isInventoryOpen = false;

        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventoryCloseSFX.Play();

        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
            selectedSlot = null;
        }

        ClearItemInfo();
        optionsPanel.SetActive(false);
    }


    public bool AddItem(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty && slots[i].GetItem() == item && item.maxStack > 1)
            {
                if (slots[i].GetQuantity() < item.maxStack)
                {
                    slots[i].AddQuantity(1);
                    return true;
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].SetItem(item);
                return true;
            }
        }

        Debug.Log("Inventory Full! Could not add item: " + item.itemName);
        return false;
    }


    public void DisplayItemInfo(ItemData item)
    {
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
    }

    public void ClearItemInfo()
    {
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void ShowOptionsPanel()
    {
        optionsPanel.SetActive(true);
    }

    public void HideOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }

    public void SetSelectedSlot(InventorySlot slot)
    {
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
        }

        selectedSlot = slot;

        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(true);
        }
    }


    public void UseSelectedItem()
    {
        if (selectedSlot == null || selectedSlot.IsEmpty) return;

        ItemData item = selectedSlot.GetItem();

        if (item.itemType == ItemType.HealingConsumable)
        {
            PlayerStats playerHealth = FindObjectOfType<PlayerStats>();
            if (playerHealth != null)
            {
                int current = playerHealth.GetCurrentHealth();
                int max = playerHealth.GetMaxHealth();

                if (current < max)
                {
                    playerHealth.Heal(item.healAmount);
                    selectedSlot.RemoveOne();
                    Debug.Log("Used healing item: " + item.itemName);
                }
                else
                {
                    Debug.Log("Player is already at full health. Can't use " + item.itemName);
                }
            }
            return;
        }

        if (item != null && item.worldPrefab != null)
        {
            Transform existingGun = handgunHolder.childCount > 0 ? handgunHolder.GetChild(0) : null;

            if (existingGun != null && existingGun.name.Contains(item.worldPrefab.name))
            {
                Destroy(existingGun.gameObject);
                Debug.Log("Unequipped: " + item.itemName);
            }
            else
            {
                foreach (Transform child in handgunHolder)
                {
                    Destroy(child.gameObject);
                }

                GameObject equippedGun = Instantiate(item.worldPrefab, handgunHolder);
                equippedGun.transform.localPosition = Vector3.zero;
                equippedGun.transform.localRotation = Quaternion.identity;

                Debug.Log("Equipped: " + item.itemName);
            }
        }
    }
}
