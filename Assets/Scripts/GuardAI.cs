using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GuardAI : MonoBehaviour
{
    public enum GuardState
    {
        Idle,
        Patrolling,
        Chasing
    }

    [Header("State")] public GuardState currentState = GuardState.Idle;

    [Header("References")] public Animator anim;
    public Transform player;
    private NavMeshAgent agent;

    [Header("Movement Settings")] public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Patrol Points")] public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;

    [Header("Vision")] public float viewDistance = 50f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        
        // Denna är utkommenterad så han kan anpassa sin höjd efter terräng/trappor
        // agent.updateUpAxis = false;

        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        targetPoint = pointA;
    }

    public void StartChase()
    {
        currentState = GuardState.Chasing;
    }

    void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.SetFloat("speed", 0); 
                anim.SetFloat("moveX", 0);
                anim.SetFloat("moveY", 0);
            }
            return; 
        }

        if (player == null) return;
        UpdateAnimations(); 

        if (currentState == GuardState.Idle) return;

        // --- HÄR ÄR FIXEN FÖR HOVEDVÄRKEN (IGNORERA Y-AXELN) ---
        Vector3 guardPos2D = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPos2D = new Vector3(player.position.x, 0, player.position.z);
    
        // Räknar ut avståndet helt platt längs marken
        float distanceToPlayer = Vector3.Distance(guardPos2D, playerPos2D);
        // ------------------------------------------------------

        if (currentState == GuardState.Chasing)
        {
            // 1. Tvinga agenten att glömma sin gamla beräkning så han ALDRIG kan frysa fast
            agent.ResetPath(); 
    
            // 2. Sätt målet rakt på din spelares position direkt
            agent.SetDestination(player.position);
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
    
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            anim.SetFloat("moveX", direction.x);
            anim.SetFloat("moveY", direction.z); 
        }
    
        anim.SetFloat("speed", agent.velocity.magnitude);
    }

    void CaughtPlayer()
    {
        CheckpointManager cpManager = player.GetComponent<CheckpointManager>();

        if (cpManager != null && CheckpointManager.hasReachedCheckpoint)
        {
            player.position = CheckpointManager.lastCheckPointPos;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            Debug.Log("Spelaren fångad! Teleporterar till checkpoint.");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}