using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GuardAI : MonoBehaviour
{
    public enum GuardState { Idle, Patrolling, Chasing }
    [Header("State")]
    public GuardState currentState = GuardState.Idle; 

    [Header("References")]
    public Animator anim; 
    public Transform player;
    private NavMeshAgent agent;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Patrol Points")]
    public Transform pointA; 
    public Transform pointB; 
    private Transform targetPoint;

    [Header("Vision")]
    public float viewDistance = 50f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; 
        agent.updateUpAxis = false;

        if (anim == null) anim = GetComponentInChildren<Animator>();
        
        if (player == null) 
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj != null) player = playerObj.transform;
        }
        
        targetPoint = pointA;
    }

    public void StartChase() 
    {
        currentState = GuardState.Chasing;
    }

    void Update()
    {
        if (player == null) return;
        UpdateAnimations();

        // --- VI HAR TAGIT BORT BRO-KOLLEN HÄRIFRÅN ---
        // Nu är det bara jakt och patrull kvar här.

        if (currentState == GuardState.Idle) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentState == GuardState.Chasing)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(player.position, out hit, 10.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            agent.speed = chaseSpeed;

            if (distanceToPlayer > viewDistance + 10f) 
            {
                currentState = GuardState.Patrolling;
            }
        }
        else 
        {
            Patrol();
            if (distanceToPlayer < viewDistance)
            {
                currentState = GuardState.Chasing;
            }
        }

        if (currentState == GuardState.Chasing && distanceToPlayer < 1.2f) 
        {
            CaughtPlayer();
        }
    }

    void Patrol()
    {
        if (pointA == null || pointB == null) return;
        agent.speed = patrolSpeed;
        agent.SetDestination(targetPoint.position);
        if (!agent.pathPending && agent.remainingDistance < 0.6f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void UpdateAnimations()
    {
        if (anim == null) return;
        Vector3 velocity = agent.velocity;
        anim.SetFloat("moveX", velocity.x);
        anim.SetFloat("moveY", velocity.z); 
        anim.SetFloat("speed", velocity.magnitude);
    }

    void CaughtPlayer() 
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}