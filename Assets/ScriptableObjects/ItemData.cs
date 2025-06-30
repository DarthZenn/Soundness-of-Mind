using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Handgun,
    Melee,
    Ammo,
    Consumable,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public int maxStack = 1;
    public ItemType itemType;
    public GameObject worldPrefab;
}
