using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private GameObject player;
    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator animator;

    public float sightRange;
    public float fieldOfView;

    [SerializeField] private float attackRange;
    [SerializeField] private float stopOffset;

    [SerializeField] private float attackCooldown;
    private float attackTimer;
    private bool isAttacking = false;

    [SerializeField] private int damage;

    private enum State { Idle, Chasing, Attacking }
    private State currentState = State.Idle;
    private PlayerStats playerStats;
    private bool isAttackLocked = false;

    [SerializeField] private AudioSource zombieAttackAudio;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attackTimer = 0f;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerTransform = player.transform;
                playerStats = player.GetComponent<PlayerStats>();
            }
            else
            {
                return;
            }
        }

        if (playerTransform == null) return;

        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Chasing:
                HandleChasing();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
        }

        if (!isAttackLocked)
        {
            CheckTransitions();
        }

        HandleFailsafe();
        //Debug.Log(agent.isStopped + " / " + currentState + " / " + isAttacking);
    }

    void HandleIdle()
    {
        agent.SetDestination(transform.position);
        animator.SetBool("isWalking", false);
    }

    void HandleChasing()
    {
        sightRange = 100;
        fieldOfView = 360;
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 offsetTarget = playerTransform.position - directionToPlayer * (attackRange - stopOffset);
        agent.SetDestination(offsetTarget);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);
    }

    void HandleAttacking()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        animator.SetBool("isWalking", false);

        if (!isAttacking && attackTimer <= 0f)
        {
            if (zombieAttackAudio != null)
            {
                zombieAttackAudio.Play();
            }
            isAttacking = true;
            isAttackLocked = true;
            attackTimer = attackCooldown;
            agent.isStopped = true;
            animator.SetTrigger("Attack");
        }
    }

    void CheckTransitions()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (distance <= attackRange && angle <= fieldOfView / 2f &&
            agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            currentState = State.Attacking;
        }
        else if (distance <= sightRange && angle <= fieldOfView / 2f)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    public void DealDamage()
    {
        if (playerStats != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                playerStats.TakeDamage(damage);
            }
            else
            {
                Debug.Log("Player was too far. Attack missed.");
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        isAttackLocked = false;
        agent.isStopped = false;
    }

    void HandleFailsafe()
    {
        if (currentState == State.Chasing && agent.isStopped == true && isAttacking == true)
        {
            Debug.LogWarning("Failsafe triggered: Zombie was stuck. Resuming movement.");
            agent.isStopped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 forward = transform.forward * sightRange;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);
    }
}
