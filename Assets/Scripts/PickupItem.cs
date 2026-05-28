using UnityEngine;
//gamla key.cs
public class PickupItem : MonoBehaviour, IInteractable
{
    // Vi sparar din exakta skala här
    private Vector3 myProperScale = new Vector3(1.5127f, 1.5127f, 1.5127f);

    public void Interact()
    {
        PlayerStateManager player = FindFirstObjectByType<PlayerStateManager>();

        if (player != null && player.currentlyHeldItem == null)
        {
            player.currentlyHeldItem = this.gameObject;

            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false; 

            transform.SetParent(player.handTransform);
            
            // --- FIXAR STORLEKEN OCH POSITIONEN ---
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = myProperScale; // Låser skalan till 1.5127 i handen

            Debug.Log("Jag håller nyckeln med rätt skala!");
        }
    }
}