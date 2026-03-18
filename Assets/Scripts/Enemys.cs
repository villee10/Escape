using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemys : MonoBehaviour
{
    public enum GuardState { Idle, Patrolling, Chasing }
    
    [Header("State")]
    public GuardState currentState = GuardState.Patrolling; // Börja med Patrolling för att han ska gå direkt

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
    public float viewDistance = 5f; // ÄNDRA DENNA: Sänkt från 20 till 5 för att han inte ska se dig direkt

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    
        // 1. DIN 2D-FIX: Hindrar gubben från att snurra i 3D
        agent.updateRotation = false; 
        agent.updateUpAxis = false;

        // 2. FIX FÖR HACKANDE (Stuttering):
        // Vi sätter dessa i kod så du slipper ändra i Inspectorn.
        // En hög acceleration (t.ex. 50) tar bort "hacket" så han når maxfart direkt.
        agent.acceleration = 60f; 
        // En hög angularSpeed här gör att han kan "svänga" sin väg internt utan att spriten roterar.
        agent.angularSpeed = 2000f; 
        // Hindrar honom från att vibrera när han kommer nära målet
        agent.stoppingDistance = 0.2f; 

        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    
        targetPoint = pointA;
    }

    void Update()
    {
        UpdateAnimations();

        // 1. Om han är Idle gör han inget förrän han ser dig
        if (currentState == GuardState.Idle) 
        {
            CheckForPlayer();
            return;
        }

        // 2. Jakt-logik (Han lämnar A-B punkterna här)
        if (currentState == GuardState.Chasing)
        {
            agent.SetDestination(player.position);
            agent.speed = chaseSpeed;
        }
        // 3. Patrull-logik
        else 
        {
            Patrol();
            CheckForPlayer();
        }

        // 4. Om han fångar dig
        if (currentState == GuardState.Chasing && Vector3.Distance(transform.position, player.position) < 1.2f) 
        {
            CaughtPlayer();
        }
    }

    void CheckForPlayer()
    {
        // Om avståndet är mindre än viewDistance byter han till Chasing
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            currentState = GuardState.Chasing;
            Debug.Log("DEBUG: Spelare sedd! Lämnar patrullstig.");
        }
    }

    void Patrol()
    {
        if (targetPoint == null) return;
        agent.speed = patrolSpeed;
        agent.SetDestination(targetPoint.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void UpdateAnimations()
    {
        if (anim == null) return;
        Vector3 velocity = agent.velocity;
        
        // Skickar värden till ditt Blend Tree
        anim.SetFloat("moveX", velocity.x);
        anim.SetFloat("moveY", velocity.z);
        anim.SetFloat("speed", velocity.magnitude);

        // Vänder spriten grafiskt
        if (velocity.x > 0.1f) transform.localScale = new Vector3(1, 1, 1);
        else if (velocity.x < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }

    void CaughtPlayer() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    
    // Ritar ut synfältet i Scene-fönstret så du ser hur långt 5 enheter faktiskt är
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}