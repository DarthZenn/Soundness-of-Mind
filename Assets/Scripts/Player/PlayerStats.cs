using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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

    private PostProcessController pp;

    void Start()
    {
        if (GlobalControl.Instance != null && GlobalControl.Instance.playerHealth > 0)
        {
            currentHealth = GlobalControl.Instance.playerHealth;
            UpdateHealth();
        }
        else
        {
            currentHealth = maxHealth;
            UpdateHealth();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player took damage! Current HP: " + currentHealth);

        UpdateHealth();
    }

    public void UpdateHealth()
    {
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
        StartCoroutine(PlayDieAnim());
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

    IEnumerator PlayDieAnim()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }
        else
        {
            Debug.LogWarning("No Animator found on Player. Nice job, genius.");
        }

        yield return new WaitForSeconds(2.3f); // Adjust this to match your animation

        PlayerDie playerDie = FindObjectOfType<PlayerDie>();
        if (playerDie != null)
        {
            Time.timeScale = 0f;
            playerDie.HandleGameOver();
        }
    }
}
