using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public int danceCount = 3;
    public float catchDistance = 1.2f;

    [SerializeField] private Animator animator;

    private NavMeshAgent navMeshAgent;
    private bool dancePicked;
    private bool caughtPlayer;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 0.5f;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (player == null || caughtPlayer)
        {
            navMeshAgent.isStopped = true;

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);

                if (!dancePicked)
                {
                    animator.SetInteger("DanceIndex", Random.Range(0, danceCount));
                    dancePicked = true;
                }
            }
            return;
        }

        navMeshAgent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= catchDistance)
        {
            caughtPlayer = true;
            player.GetComponent<PlayerController>().OnCaughtByEnemy();
            return;
        }

        if (animator != null)
        {
            float speed = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
            animator.SetFloat("Speed", speed);
        }
    }
}
