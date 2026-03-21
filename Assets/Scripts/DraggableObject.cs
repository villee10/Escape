using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DraggableObject : MonoBehaviour, IInteractable
{
    public Rigidbody rb;
    public bool lockYAxis = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Den ska vara låst (Kinematic) från början så man inte kan gå in i den
        rb.isKinematic = true; 
    }

    public void Interact()
    {
        PlayerStateManager player = FindFirstObjectByType<PlayerStateManager>();
        if (player != null)
        {
            player.StartDragging(this);
        }
    }
}