using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    public int playerHealth;
    public int currentAmmo;

    public List<InventorySlotData> savedInventory = new List<InventorySlotData>();
    public GameObject currentWeapon;

    public bool isInventoryOpen = false;
    public bool isPause = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
