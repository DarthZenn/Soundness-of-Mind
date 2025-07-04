using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    public int sceneIndex;
    public Vector3 playerPosition;
    public int playerHealth;
    public int currentAmmo;
    public GameObject currentWeapon;
    public string destinationSpawnID;
    public List<InventorySlotData> savedInventory = new List<InventorySlotData>();

    public bool isInventoryOpen = false;
    public bool isPause = false;
    public bool isPrompt = false;
    public bool isGameOver = false;
    public bool isSaving = false;

    public bool hasLoadedSave = false;

    private string saveFilePath => Path.Combine(Application.persistentDataPath, "savegame.json");

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

    [System.Serializable]
    class SaveData
    {
        public int sceneIndex;
        public Vector3 playerPosition;
        public int playerHealth;
        public int currentAmmo;
        public string currentWeaponName;
        public string destinationSpawnID;
        public List<InventorySlotData> inventoryData;
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        Vector3 playerCurrentPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        playerPosition = playerCurrentPosition;
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        int playerCurrentHealth = playerStats.GetCurrentHealth();
        playerHealth = playerCurrentHealth;
        TankController tankController = FindObjectOfType<TankController>();
        int currentAmmoToSave = tankController.GetCurrentAmmo();
        currentAmmo = currentAmmoToSave;
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager.GetCurrentWeapon();
        inventoryManager.GetCurrentInventorySlotData();
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        data.sceneIndex = sceneIndex;
        data.playerPosition = playerPosition;
        data.playerHealth = playerHealth;
        data.currentAmmo = currentAmmo;
        data.destinationSpawnID = destinationSpawnID;
        data.currentWeaponName = currentWeapon != null ? currentWeapon.name : "";
        data.inventoryData = new List<InventorySlotData>(savedInventory);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            sceneIndex = data.sceneIndex;
            playerPosition = data.playerPosition;
            playerHealth = data.playerHealth;
            currentAmmo = data.currentAmmo;
            destinationSpawnID = data.destinationSpawnID;
            savedInventory = new List<InventorySlotData>(data.inventoryData);
            hasLoadedSave = true;

            if (!string.IsNullOrEmpty(data.currentWeaponName))
            {
                GameObject weaponPrefab = Resources.Load<GameObject>(data.currentWeaponName);
                currentWeapon = weaponPrefab;
            }

            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogWarning("No save file found.");
        }
    }
}
