using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStats : MonoBehaviour
{
    [Header("Zombie Stats")]
    public int maxHP = 100;
    private int currentHP;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<ZombieAI>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        Destroy(gameObject, 5f);
    }

    public bool IsDead() => isDead;
}
