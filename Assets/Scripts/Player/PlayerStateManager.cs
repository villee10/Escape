using UnityEngine;

public partial class PlayerStateManager : MonoBehaviour
{
    // --- STATES ---
    PlayerBaseState currentState;
    public PlayerIdleState IdleState = new PlayerIdleState();
    public PlayerMoveState MoveState = new PlayerMoveState();
    public PlayerJumpState JumpState = new PlayerJumpState();
    public PlayerCrouchState CrouchState = new PlayerCrouchState();
    
    [Header("Cutscene Mode")]
    public bool isInCutscene = false; // Styrs från dina Intro/Bridge skript
    
    [Header("Holding")]
    public Transform handTransform; 
    public GameObject currentlyHeldItem;
    public float handZOffset = 0.1f; // Hur mycket handen ska flyttas (0.1 brukar räcka)
    
    [Header("Dragging")]
    private DraggableObject currentDraggable; 
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
    public Transform characterTransform; 
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CapsuleCollider col;

    [HideInInspector] public float originalSpeed;
    [HideInInspector] public Vector3 originalColliderSize;
    [HideInInspector] public Vector3 originalColliderCenter;
    public float inputX;
    public float inputZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<CapsuleCollider>();

        if (rb != null) {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeRotation; 
        }

        originalSpeed = moveSpeed;

        if (col != null)
        {
            originalColliderSize = new Vector3(0, col.height, 0); 
            originalColliderCenter = col.center;
        }

        currentState = IdleState;
        currentState.EnterState(this);
    }

    void Update()
    {
        // 0. STOPP VID CUTSCENE: Vi returnerar så att ingen input eller state-logik körs.
        if (isInCutscene) return; 
    
        // 1. Hämta input
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        // --- NYTT: Justera djupet på handen baserat på riktning ---
        UpdateHandDepth(); 

        // 2. Mark-kontroll
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 3. Räkna ut rörelsemängd
        float moveInput = new Vector2(inputX, inputZ).magnitude;

        // 4. INTERAKTION
        if (Input.GetKeyDown(KeyCode.E)) CheckInteraction();
        if (Input.GetKeyUp(KeyCode.E)) StopDragging();

        // 5. ANIMATOR-SYNK
        if (anim != null) 
        {
            anim.SetBool("isCrouching", currentState == CrouchState);
        }

        // 6. STATE-BYTEN
        if (moveInput > 0.1f && currentState == IdleState)
        {
            SwitchState(MoveState);
        }

        if (Input.GetKey(KeyCode.C) && isGrounded && currentState != JumpState)
        {
            if (currentState != CrouchState) SwitchState(CrouchState);
        }

        if (Input.GetButtonDown("Jump") && isGrounded && currentState != CrouchState)
        {
            SwitchState(JumpState);
        }

        // 7. Kör logik för nuvarande state
        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        // 0. CUTSCENE-SPÄRR: Tvingar gubben att stå stilla fysiskt men behåller gravitationen.
        if (isInCutscene) 
        {
            if (rb != null)
            {
                // Vi nollar X och Z fart, men låter Y (gravitation) vara för att undvika att han svävar.
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
            return; 
        }

        // 1. Kör vanlig state-logik
        if (currentState != null)
        {
            currentState.FixedUpdateState(this);
        }

        // 2. Drag-logik
        if (currentDraggable != null)
        {
            Vector3 targetPos = transform.position + dragOffset;
            if (currentDraggable.lockYAxis) targetPos.y = initialObjectY;
            currentDraggable.rb.MovePosition(targetPos);
        }

        // 3. Broms vid stillastående
        if (isGrounded && inputX == 0 && inputZ == 0 && currentDraggable == null)
        {
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

        if (closestInteractable != null) closestInteractable.Interact();
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
        if (itemCol != null && this.col != null) Physics.IgnoreCollision(this.col, itemCol, false);
    }
    
    public void StartDragging(DraggableObject target)
    {
        currentDraggable = target;
        dragOffset = target.transform.position - transform.position;
        initialObjectY = target.transform.position.y;
        currentDraggable.rb.isKinematic = false; 
        moveSpeed = originalSpeed * 0.5f;
    }

    void StopDragging()
    {
        if (currentDraggable != null)
        {
            currentDraggable.rb.isKinematic = true; 
            currentDraggable = null;
            moveSpeed = originalSpeed; 
        }
    }
    
    
    void UpdateHandDepth()
    {
        if (handTransform == null) return;

        SpriteRenderer itemRenderer = handTransform.GetComponentInChildren<SpriteRenderer>();
        if (itemRenderer == null) return;

        // 1. Går han UPPÅT? (inputZ > 0.1) -> Hamna BAKOM
        // 2. Går han åt VÄNSTER? (inputX < -0.1) -> Hamna BAKOM
        if (inputZ > 0.1f || inputX < -0.1f)
        {
            itemRenderer.sortingOrder = -1; // Lägre än spelaren (0)
            handTransform.localPosition = new Vector3(handTransform.localPosition.x, handTransform.localPosition.y, 0.1f);
        }
        // 3. Går han neråt, höger eller står stilla? -> Hamna FRAMFÖR
        else
        {
            itemRenderer.sortingOrder = 1; // Högre än spelaren (0)
            handTransform.localPosition = new Vector3(handTransform.localPosition.x, handTransform.localPosition.y, -0.1f);
        }
    }
}