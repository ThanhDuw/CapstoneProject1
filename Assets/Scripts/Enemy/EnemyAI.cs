using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float retreatRange = 5f;
    public float health = 100f;
    public float retreatThreshold = 30f; // Nếu máu thấp hơn thì chạy trốn

    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 originalPosition;
    private enum State { Idle, Chase, Attack, Retreat, Return }
    private State currentState;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = State.Idle;
        originalPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (health <= retreatThreshold && distance < retreatRange)
        {
            currentState = State.Retreat;
        }
        else if (distance <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (distance <= detectionRange)
        {
            currentState = State.Chase;
        }
        else
        {
            // Nếu đang ở xa vị trí gốc thì quay về
            float backDistance = Vector3.Distance(transform.position, originalPosition);
            if (backDistance > 0.5f)
                currentState = State.Return;
            else
                currentState = State.Idle;
        }

        UpdateState();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case State.Idle:
                agent.isStopped = true;
                animator.SetBool("isRunning", false);
                break;

            case State.Chase:
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
                break;

            case State.Attack:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                transform.LookAt(player);
                animator.applyRootMotion = false;  // nếu cần
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", true);
                break;

            case State.Retreat:
                agent.isStopped = false;
                Vector3 retreatDir = (transform.position - player.position).normalized;
                Vector3 retreatTarget = transform.position + retreatDir * 5f;
                agent.SetDestination(retreatTarget);
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
                break;

            case State.Return:
                agent.isStopped = false;
                agent.SetDestination(originalPosition);
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
                break;
        }
    }

    // Hàm để nhận sát thương
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        agent.isStopped = true;
        this.enabled = false;
    }
}
