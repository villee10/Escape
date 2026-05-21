using UnityEngine;
using UnityEngine.AI;

public class SimpleNPC : MonoBehaviour
{
    public enum NPCBehavior
    {
        StandStill,
        Patrol
    }

    [Header("Behavior")]
    public NPCBehavior behavior = NPCBehavior.StandStill;
    public float movementSpeed = 2f;

    [Header("Patrol Points (Only if Patrol)")]
    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;

    [Header("References")]
    public Animator anim;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Stäng av rotation så din 2D-sprite inte snurrar i 3D
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.speed = movementSpeed;
        }

        if (anim == null) anim = GetComponentInChildren<Animator>();

        // Starta patrulleringen mot Punkt A om NPC:n ska gå
        targetPoint = pointA;
    }

    void Update()
    {
        // Om agenten inte är redo, gör inget
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            UpdateAnimations(Vector3.zero);
            return;
        }

        if (behavior == NPCBehavior.Patrol)
        {
            HandlePatrol();
        }
        else
        {
            // Om de ska stå still, se till att de inte har kvar gammal fart
            agent.ResetPath();
            UpdateAnimations(Vector3.zero);
        }
    }

    void HandlePatrol()
    {
        if (pointA == null || pointB == null) return;

        agent.SetDestination(targetPoint.position);

        // Skicka med agentens hastighet till animationssystemet
        UpdateAnimations(agent.velocity);

        // Byt punkt när NPC:n är tillräckligt nära
        if (!agent.pathPending && agent.remainingDistance < 0.6f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void UpdateAnimations(Vector3 velocity)
    {
        if (anim == null) return;

        if (velocity.magnitude > 0.1f)
        {
            Vector3 direction = velocity.normalized;
            anim.SetFloat("moveX", direction.x);
            anim.SetFloat("moveY", direction.z); // .z sköter upp/ner i din 2.5D-värld
        }

        anim.SetFloat("speed", velocity.magnitude);
    }
}