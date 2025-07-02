using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStats : MonoBehaviour
{
    [Header("Zombie Stats")]
    public int maxHP = 100;
    private int currentHP;
    private float defaultSpeed;

    private Animator animator;
    private NavMeshAgent agent;
    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        defaultSpeed = agent.speed;
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
            agent.speed = 0f;
        }
    }

    public void EndHitReaction()
    {
        agent.speed = defaultSpeed;
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<ZombieAI>().enabled = false;
        agent.enabled = false;

        Destroy(gameObject, 5f);
    }

    public bool IsDead() => isDead;
}
