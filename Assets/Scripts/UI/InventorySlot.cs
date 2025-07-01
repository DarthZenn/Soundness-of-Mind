using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;
    public Image buttonColor;
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.green;

    private ItemData currentItem;
    private int quantity = 0;
    private InventoryManager inventoryManager;

    public bool IsEmpty => currentItem == null;

    void Start()
    {
        UpdateUI();
    }

    public void SetInventoryManager(InventoryManager manager)
    {
        inventoryManager = manager;
    }

    public void SetItem(ItemData item, int count = 1)
    {
        currentItem = item;
        quantity = count;
        icon.sprite = item.icon;
        UpdateUI();
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
        }
        else
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        quantity = 0;
        icon.sprite = null;
        quantityText.text = "";
    }

    public void OnClicked()
    {
        if (IsEmpty)
        {
            inventoryManager.ClearItemInfo();
            inventoryManager.HideOptionsPanel();
            inventoryManager.SetSelectedSlot(null);
            return;
        }

        inventoryManager.SetSelectedSlot(this);
        inventoryManager.DisplayItemInfo(currentItem);
        inventoryManager.ShowOptionsPanel();
    }

    public void SetSelected(bool isSelected)
    {
        if (buttonColor != null)
            buttonColor.color = isSelected ? selectedColor : defaultColor;
    }

    public void RemoveQuantity(int amount = 1)
    {
        if (amount <= 0 || quantity <= 0) return;

        quantity -= amount;

        if (quantity <= 0)
        {
            ClearSlot();
            inventoryManager.ClearItemInfo();
            inventoryManager.HideOptionsPanel();
        }
        else
        {
            UpdateUI();
        }
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public ItemData GetItem() => currentItem;
}
