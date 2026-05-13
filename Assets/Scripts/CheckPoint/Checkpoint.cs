using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("UI (Fyll i endast på första checkpointen)")]
    public GameObject hotbarToActivate; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Spara positionen (använder din befintliga Manager)
            CheckpointManager.SetCheckpoint(other.transform.position);
            Debug.Log("Checkpoint sparad!");

            // 2. Kolla om vi har lagt in en hotbar i just DENNA checkpoint
            if (hotbarToActivate != null)
            {
                hotbarToActivate.SetActive(true);
                Debug.Log("Hotbar aktiverad av denna checkpoint!");
            }
        }
    }
}