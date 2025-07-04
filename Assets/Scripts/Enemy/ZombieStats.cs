using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStats : MonoBehaviour
{
    [Header("Zombie Stats")]
    public int maxHP = 100;
    private int currentHP;

    private Animator animator;
    private NavMeshAgent agent;
    private bool isDead = false;
    private ZombieAI zombieAI;

    [SerializeField] private AudioSource zombieHurtAudio;
    [SerializeField] private AudioSource zombieFallAudio;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        zombieAI = GetComponent<ZombieAI>();
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
            if (zombieHurtAudio != null)
            {
                zombieHurtAudio.Play();
            }
            animator.SetTrigger("Hit");
            agent.isStopped = true;
            zombieAI.sightRange = 100;
            zombieAI.fieldOfView = 360;
        }
    }

    public void EndHitReaction()
    {
        agent.isStopped = false;
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<ZombieAI>().enabled = false;
        agent.enabled = false;

        Destroy(gameObject, 5f);
    }

    public void ZombieFall()
    {
        if (zombieFallAudio != null)
        {
            zombieFallAudio.Play();
        }
    }

    public bool IsDead() => isDead;
}
