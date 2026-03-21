using UnityEngine;

public partial class PlayerStateManager : MonoBehaviour
{
    // --- STATES ---
    PlayerBaseState currentState;
    public PlayerIdleState IdleState = new PlayerIdleState();
    public PlayerMoveState MoveState = new PlayerMoveState();
    public PlayerJumpState JumpState = new PlayerJumpState();
    public PlayerCrouchState CrouchState = new PlayerCrouchState();
    
    [Header("Holding")]
    public Transform handTransform; 
    public GameObject currentlyHeldItem;
    
    [Header("Dragging")]
    private DraggableObject currentDraggable; // DraggableObject
    private Vector3 dragOffset;
    private float initialObjectY;
    
    [Header("Interaktion")]
    public float interactionDistance = 2.5f; 
    public LayerMask interactableMask;       
    
    [Header("Rörelseinställningar")]
    public float moveSpeed = 3f;
    public float jumpForce = 4f;
    public float fallMultiplier = 2.0f;    
    public float lowJumpMultiplier = 2.0f; 
    public float crouchSpeed = 2f;
    public float sprintSpeed = 5f;
    
    [Header("Mark-kontroll (Ground Check)")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask; 
    public bool isGrounded;

    [Header("Referenser")]
    public Transform characterTransform; // Se till att detta är din modell, INTE objektet med BoxCollider
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CapsuleCollider col;

    [HideInInspector] public float originalSpeed;
    [HideInInspector] public Vector3 originalColliderSize;
    [HideInInspector] public Vector3 originalColliderCenter;
    public float inputX;
    public  float inputZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<CapsuleCollider>();

        // 1. VIKTIGT: Sätt upp fysiken här i koden så den alltid är rätt
        if (rb != null) {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeRotation; 
        }

        originalSpeed = moveSpeed;

        // 2. Spara värden för Capsule Collider
        if (col != null)
        {
            originalColliderSize = new Vector3(0, col.height, 0); 
            originalColliderCenter = col.center;
        }

        // 3. DENNA RAD FATTADES (Starta motorn):
        currentState = IdleState;
        currentState.EnterState(this);
    }

    void Update()
    {
        // 1. Buffra input (Detta tar bort jitter/skak i rörelsen)
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        // 2. Mark-kontroll
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 3. INTERAKTION (E-knappen)
        if (Input.GetKeyDown(KeyCode.E)) CheckInteraction();
        if (Input.GetKeyUp(KeyCode.E)) StopDragging();

        // 4. ANIMATOR-SYNK (Gör att isCrouching blir true när vi är i CrouchState)
        if (anim != null) 
        {
            anim.SetBool("isCrouching", currentState == CrouchState);
        }

        // 5. STATE-BYTEN (Lägg tillbaka knappen för att faktiskt byta state!)
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && currentState != JumpState)
        {
            SwitchState(CrouchState); // Nu aktiveras logiken och animationen!
        }

        if (Input.GetButtonDown("Jump") && isGrounded && currentState != CrouchState)
        {
            SwitchState(JumpState);
        }

        // 6. Kör logiken för nuvarande tillstånd
        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        // 1. Kör logiken för nuvarande tillstånd (Move, Jump osv)
        if (currentState != null)
        {
            currentState.FixedUpdateState(this);
        }

        // 2. NYTT: Logik för att flytta objektet vi drar (Höboll, stol, etc.)
        if (currentDraggable != null)
        {
            // Beräkna målpositionen baserat på din nuvarande position + den sparade offsetten
            Vector3 targetPos = transform.position + dragOffset;
        
            // Om objektet ska vara klistrat mot marken (Y-axeln)
            if (currentDraggable.lockYAxis)
            {
                targetPos.y = initialObjectY;
            }

            // MovePosition är bäst för fysikobjekt – det gör att balen 
            // stannar om den krockar med en vägg istället för att gå igenom den.
            currentDraggable.rb.MovePosition(targetPos);
        }

        // 3. EXTRA KOD: "Bromsen"
        // VIKTIGT: Vi lägger till "&& currentDraggable == null" i kontrollen.
        // Om vi inte gör det kommer gubben att tvärstanna hela tiden 
        // medan han försöker dra ett tungt objekt.
        if (isGrounded && inputX == 0 && inputZ == 0 && currentDraggable == null)
        {
            // Vi behåller farten i Y (så vi fortfarande faller/landar rätt)
            // men vi sätter fart i X och Z till 0 så vi inte glider en millimeter.
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

   

    public void SwitchState(PlayerBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    // --- INTERAKTION OCH DROP LOGIK ---
    void CheckInteraction()
    {
        if (currentlyHeldItem != null) 
        {
            DropItem();
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance, interactableMask);
        float closestDistance = Mathf.Infinity;
        IInteractable closestInteractable = null;

        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    }

    public void DropItem()
    {
        if (currentlyHeldItem == null) return;

        currentlyHeldItem.transform.SetParent(null);
    
        Rigidbody itemRb = currentlyHeldItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            Vector3 dropDir = (characterTransform.forward + Vector3.down * 0.5f).normalized;
            itemRb.AddForce(dropDir * 2f, ForceMode.Impulse);
        }

        Collider itemCol = currentlyHeldItem.GetComponent<Collider>();
        if (itemCol != null) 
        {
            itemCol.enabled = true;
            Physics.IgnoreCollision(this.col, itemCol, true);
            StartCoroutine(ReEnableCollision(itemCol));
        }

        currentlyHeldItem = null;
    }

    System.Collections.IEnumerator ReEnableCollision(Collider itemCol)
    {
        yield return new WaitForSeconds(0.5f);
        if (itemCol != null && this.col != null)
        {
            Physics.IgnoreCollision(this.col, itemCol, false);
        }
    }
    
    public void StartDragging(DraggableObject target)
    {
        currentDraggable = target;
        
        // Sparar offsetten precis som innan
        dragOffset = target.transform.position - transform.position;
        initialObjectY = target.transform.position.y;

        // --- VIKTIGT: Gör den ICKE-kinematic när vi startar dragandet ---
        // Det betyder att fysikmotorn nu lyssnar på vår flytt-kommandon
        // och den känner av kollisioner (som bergsväggen).
        currentDraggable.rb.isKinematic = false; 

        // Sänk spelarens fart för viktkänsla
        moveSpeed = originalSpeed * 0.5f;
    }

    void StopDragging()
    {
        if (currentDraggable != null)
        {
            // --- VIKTIGT: Gör den Kinematic igen precis när vi släpper ---
            // Detta "låser" bollen på dess nuvarande position. 
            // Den står blickstilla, känner inga knuffar och känner inte tyngdlagen.
            // Perfekt för stapling!
            currentDraggable.rb.isKinematic = true; 

            currentDraggable = null;
            moveSpeed = originalSpeed; // Återställ spelarens fart
        }
    }
    
    
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}