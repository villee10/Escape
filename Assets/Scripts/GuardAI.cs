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
        agent.updateUpAxis = false;

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
                // tvingar parametrarna till 0 här inne också, 
                // så UpdateAnimations inte kan "ångra" det.
                Animator anim = GetComponentInChildren<Animator>();
                if (anim != null)
                {
                    anim.SetFloat("Speed", 0);
                    anim.SetFloat("moveX", 0);
                    anim.SetFloat("moveY", 0);
                }
                return; // VIKTIGT: Kör INTE UpdateAnimations() här
            }

            if (player == null) return;
            UpdateAnimations(); // Denna körs nu BARA när agenten är aktiv
    
            // ... resten av koden ...
        

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
    
        // velocity.normalized gör att värdena alltid håller sig mellan -1 och 1
        // Vi kollar om agenten faktiskt rör sig för att inte nollställa riktningen när han stannar
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            anim.SetFloat("moveX", direction.x);
            anim.SetFloat("moveY", direction.z); // .z är viktigt i 3D!
        }
    
        anim.SetFloat("speed", agent.velocity.magnitude);
    }

    void CaughtPlayer()
    {
        // Istället för att ladda om scenen, anropar vi en reset-funktion på spelaren
        CheckpointManager cpManager = player.GetComponent<CheckpointManager>();

        if (cpManager != null && CheckpointManager.hasReachedCheckpoint)
        {
            // Flytta spelaren direkt utan att ladda om scenen
            player.position = CheckpointManager.lastCheckPointPos;

            // Stoppa farten så han inte fortsätter springa in i vakten
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            Debug.Log("Spelaren fångad! Teleporterar till checkpoint.");
        }
        else
        {
            // Om ingen checkpoint finns, ladda om scenen som vanligt
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}