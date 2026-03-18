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

    [Header("Bridge / Limit")]
    public Transform bridgePoint; 
    public float stopAtBridgeDistance = 4.0f; 
    public Transform pointA; 
    public Transform pointB; 
    private Transform targetPoint;

    [Header("Vision")]
    public float viewDistance = 50f; // Du nämnde att du ville ha 50 här

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Om du kör 2D-look i 3D space, behåll dessa. 
        // Om det är ren 3D, kan du behöva sätta dem till true.
        agent.updateRotation = false; 
        agent.updateUpAxis = false;

        if (anim == null) anim = GetComponentInChildren<Animator>();
        
        // Hitta spelaren automatiskt om den inte är tilldelad
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

        // --- 1. KOLLA BRON (DÖDAR FIENDEN) ---
        if (bridgePoint != null)
        {
            float distToBridge = Vector3.Distance(transform.position, bridgePoint.position);
            if (distToBridge < stopAtBridgeDistance)
            {
                Debug.Log(gameObject.name + " nådde bron och togs bort.");
                Destroy(gameObject);
                return; 
            }
        }

        if (currentState == GuardState.Idle) return;

        // --- 2. LOGIK FÖR JAKT OCH PATRULL ---
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentState == GuardState.Chasing)
        {
            // Ökad sökradie (10.0f) ifall spelaren hoppar eller är på en plattform
            NavMeshHit hit;
            if (NavMesh.SamplePosition(player.position, out hit, 10.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            
            agent.speed = chaseSpeed;

            // Om spelaren kommer VÄLDIGT långt bort (t.ex. 10 meter utanför synfältet), sluta jaga
            if (distanceToPlayer > viewDistance + 10f) 
            {
                currentState = GuardState.Patrolling;
            }
        }
        else // Patrull-läge
        {
            Patrol();

            // Om spelaren kommer inom synhåll, börja jaga
            if (distanceToPlayer < viewDistance)
            {
                currentState = GuardState.Chasing;
            }
        }

        // --- 3. KOLLA OM SPELAREN ÄR FÅNGAD ---
        // Vi kollar bara detta om vi faktiskt jagar
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

        // Byt målpunkt när vi är nära
        if (!agent.pathPending && agent.remainingDistance < 0.6f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void UpdateAnimations()
    {
        if (anim == null) return;
        Vector3 velocity = agent.velocity;
        
        // I 2D/Top-down används ofta X och Y. I 3D är det X och Z.
        anim.SetFloat("moveX", velocity.x);
        anim.SetFloat("moveY", velocity.z); 
        anim.SetFloat("speed", velocity.magnitude);
    }

    void CaughtPlayer() 
    { 
        Debug.Log("Spelaren fångad!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}