using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    [Header("Inställningar")]
    public string keyName = "key"; // Kolla exakt vad nyckel-objektet heter i Hierarchy
    public string playerTag = "Player";

    [Header("Referenser")]
    public Animator hingeAnimator; // Dra in din "door_hinge" här

    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Kolla om det är spelaren
        if (other.CompareTag(playerTag) && !hasOpened)
        {
            // 2. Kolla om spelaren håller i något
            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            
            if (player != null && player.currentlyHeldItem != null)
            {
                // 3. Kolla om föremålet spelaren håller i är rätt nyckel
                if (player.currentlyHeldItem.name == keyName)
                {
                    OpenTheDoor(player);
                }
                else 
                {
                    Debug.Log("Du håller i fel sak! Du behöver: " + keyName);
                }
            }
            else
            {
                Debug.Log("Dörren är låst. Du behöver en nyckel!");
            }
        }
    }

    void OpenTheDoor(PlayerStateManager player)
    {
        hasOpened = true;

        // Starta animationen på gångjärnet
        if (hingeAnimator != null)
        {
            hingeAnimator.SetTrigger("Open"); // Se till att parametern i Animatorn heter "Open"
        }

        // Ta bort nyckeln från spelarens hand och förstör objektet
        GameObject keyToDestroy = player.currentlyHeldItem;
        player.currentlyHeldItem = null; // Rensa spelarens hand-referens
        Destroy(keyToDestroy);

        Debug.Log("Dörren öppnas! Nyckeln förbrukad.");
    }
}