using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth;
    private int currentHealth;

    [SerializeField] private VideoPlayer heartBeatMonitor;
    [SerializeField] private VideoClip healthNormal;
    [SerializeField] private VideoClip healthDangerous;
    [SerializeField] private VideoClip healthCritical;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player took damage! Current HP: " + currentHealth);

        if (currentHealth > 50)
        {
            heartBeatMonitor.clip = healthNormal;
        }
        else if (currentHealth < 49 && currentHealth > 20)
        {
            heartBeatMonitor.clip = healthDangerous;
        }
        else
        {
            heartBeatMonitor.clip = healthCritical;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead. GG no re.");
    }

    public int GetCurrentHealth() => currentHealth;

    public int GetMaxHealth() => maxHealth;

    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Player is already at full health. Healing ignored.");
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player healed. Current HP: " + currentHealth);
    }
}
