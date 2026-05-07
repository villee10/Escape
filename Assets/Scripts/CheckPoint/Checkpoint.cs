using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Vi sparar spelarens position precis när han går in i boxen, 
            // istället för boxens mittpunkt. Det är säkrare!
            CheckpointManager.SetCheckpoint(other.transform.position);
            Debug.Log("Checkpoint sparad vid: " + other.transform.position);
        }
    }
}