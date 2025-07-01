using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Handgun,
    Melee,
    HandgunAmmo,
    HealingConsumable,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public int maxStack = 1;
    public int quantityPerPickup = 1;
    public ItemType itemType;
    public GameObject worldPrefab;

    [Header("Consumable Settings")]
    public int healAmount = 0;

    [Header("Gun Settings")]
    public float fireRate = 0f;
    public int maxAmmo;
    public int gunDamage = 0;
}
