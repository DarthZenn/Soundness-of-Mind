using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private float defaultSpeed;
    [SerializeField] private float sightRange;
    [SerializeField] private float fieldOfView;

    [SerializeField] private float attackRange;
    [SerializeField] private float stopOffset;

    [SerializeField] private float attackCooldown;
    private float attackTimer;
    private bool isAttacking = false;

    [SerializeField] private int damage;

    private enum State { Idle, Chasing, Attacking }
    private State currentState = State.Idle;
    private PlayerStats playerStats;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attackTimer = 0f;
        defaultSpeed = agent.speed;

        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        if (player == null) return;

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

        CheckTransitions();
    }

    void HandleIdle()
    {
        agent.SetDestination(transform.position);
        animator.SetBool("isWalking", false);
    }

    void HandleChasing()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 offsetTarget = player.position - directionToPlayer * (attackRange - stopOffset);
        agent.SetDestination(offsetTarget);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);
    }

    void HandleAttacking()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        animator.SetBool("isWalking", false);

        if (!isAttacking && attackTimer <= 0f)
        {
            agent.speed = 0;
            isAttacking = true;
            attackTimer = attackCooldown;
            animator.SetTrigger("Attack");
        }
    }

    void CheckTransitions()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (distance <= attackRange && angle <= fieldOfView / 2f)
        {
            currentState = State.Attacking;
        }
        else if (distance <= sightRange && angle <= fieldOfView / 2f)
        {
            sightRange = 100;
            fieldOfView = 360;
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idle;
        }

        if (currentState != State.Attacking)
        {
            isAttacking = false;

            if (agent.speed == 0)
            {
                agent.speed = defaultSpeed;
            }
        }
    }


    public void DealDamage()
    {
        if (playerStats != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
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
        agent.speed = 0.3f;
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
