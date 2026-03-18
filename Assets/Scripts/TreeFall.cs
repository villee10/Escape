using UnityEngine;

public class TreeFall : MonoBehaviour
{
    // Vi byter från Rigidbody till Animator!
    public Animator treeAnimator; 
    public AudioSource fallSound; 

    private void OnTriggerEnter(Collider other)
    {
        // Kolla efter din pappa (Player)
        if (other.CompareTag("Player") && treeAnimator != null)
        {
            // Starta animationen genom att aktivera triggern "Fall"
            treeAnimator.SetTrigger("Fall"); 

            if (fallSound != null) fallSound.Play();

            // Ta bort triggern så den inte körs igen
            Destroy(gameObject); 
        }
    }
}