using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public AudioSource inventoryOpenSFX;
    public AudioSource inventoryCloseSFX;
    public AudioSource buttonSound;

    public InventorySlot[] slots;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public GameObject optionsPanel;

    private Transform handgunHolder;

    private bool isInventoryOpen = false;
    private InventorySlot selectedSlot;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            TankController tankController = player.GetComponent<TankController>();
            if (tankController != null)
            {
                handgunHolder = tankController.handgunHolder;
            }
            else
            {
                Debug.LogError("TankController component missing on player!");
            }
        }
        else
        {
            Debug.LogError("Player not found in scene!");
        }

        foreach (var slot in slots)
        {
            slot.SetInventoryManager(this);
        }

        if (GlobalControl.Instance != null)
        {
            if (GlobalControl.Instance.currentWeapon != null)
            {
                GameObject weapon = Instantiate(GlobalControl.Instance.currentWeapon, handgunHolder);
            }

            for (int i = 0; i < GlobalControl.Instance.savedInventory.Count && i < slots.Length; i++)
            {
                var data = GlobalControl.Instance.savedInventory[i];
                if (data.item != null)
                {
                    slots[i].SetItem(data.item, data.quantity);
                }
            }
        }

        ClearItemInfo();
        optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory") && !GlobalControl.Instance.isPause
            && !GlobalControl.Instance.isPrompt && !GlobalControl.Instance.isGameOver && !GlobalControl.Instance.isSaving)
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        GlobalControl.Instance.isInventoryOpen = isInventoryOpen;

        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inventoryOpenSFX.Play();

            ClearItemInfo();
            optionsPanel.SetActive(false);
        }
        else
        {
            isInventoryOpen = false;
            GlobalControl.Instance.isInventoryOpen = false;

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
        GlobalControl.Instance.isInventoryOpen = false;

        buttonSound.Play();
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
        int quantityToAdd = item.quantityPerPickup;

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty && slots[i].GetItem() == item && item.maxStack > 1)
            {
                int currentQty = slots[i].GetQuantity();
                int availableSpace = item.maxStack - currentQty;

                if (availableSpace > 0)
                {
                    int addAmount = Mathf.Min(quantityToAdd, availableSpace);
                    slots[i].AddQuantity(addAmount);
                    quantityToAdd -= addAmount;

                    if (quantityToAdd <= 0)
                        return true;
                }
            }
        }

        for (int i = 0; i < slots.Length && quantityToAdd > 0; i++)
        {
            if (slots[i].IsEmpty)
            {
                int addAmount = Mathf.Min(quantityToAdd, item.maxStack);
                slots[i].SetItem(item, addAmount);
                quantityToAdd -= addAmount;

                if (quantityToAdd <= 0)
                    return true;
            }
        }

        Debug.Log("Inventory Full! Could not add all of item: " + item.itemName);
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
        buttonSound.Play();

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
        buttonSound.Play();
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
                    selectedSlot.RemoveQuantity(1);
                    playerHealth.UpdateHealth();
                    Debug.Log("Used healing item: " + item.itemName);
                }
                else
                {
                    Debug.Log("Player is already at full health. Can't use " + item.itemName);
                }
            }
            return;
        }

        if (item.itemType == ItemType.Handgun)
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

    public void GetCurrentWeapon()
    {
        if (handgunHolder.childCount == 0)
        {
            Debug.LogWarning("No weapon equipped, so nothing to save, genius.");
            GlobalControl.Instance.currentWeapon = null;
            return;
        }

        Transform w = handgunHolder.GetChild(0);
        ItemPickup itemRef = w.GetComponent<ItemPickup>();

        GameObject currentWeaponPrefab = itemRef.item.worldPrefab;
        GlobalControl.Instance.currentWeapon = currentWeaponPrefab;
    }

    public void GetCurrentInventorySlotData()
    {
        GlobalControl.Instance.savedInventory.Clear();
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
            {
                InventorySlotData data = new InventorySlotData
                {
                    item = slot.GetItem(),
                    quantity = slot.GetQuantity()
                };
                GlobalControl.Instance.savedInventory.Add(data);
            }
        }
    }
}
