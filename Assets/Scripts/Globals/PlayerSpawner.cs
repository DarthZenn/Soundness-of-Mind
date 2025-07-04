using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return;
        }

        string spawnID = GlobalControl.Instance.destinationSpawnID;
        PlayerSpawnPoint[] spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();

        foreach (PlayerSpawnPoint spawn in spawnPoints)
        {
            if (spawn.spawnID == spawnID)
            {
                Instantiate(playerPrefab, spawn.transform.position, spawn.transform.rotation);
                Debug.Log($"Player spawned at spawn point: {spawnID}");
                return;
            }
        }

        Debug.LogWarning("No matching spawn point found! Player not spawned.");
    }
}
