using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            return;
        }

        if (GlobalControl.Instance.hasLoadedSave)
        {
            Instantiate(playerPrefab, GlobalControl.Instance.playerPosition, Quaternion.identity);
            GlobalControl.Instance.hasLoadedSave = false;
            return;
        }

        string spawnID = GlobalControl.Instance.destinationSpawnID;
        PlayerSpawnPoint[] spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();

        foreach (PlayerSpawnPoint spawn in spawnPoints)
        {
            if (spawn.spawnID == spawnID)
            {
                Instantiate(playerPrefab, spawn.transform.position, spawn.transform.rotation);
                return;
            }
        }
    }
}
