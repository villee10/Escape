using UnityEngine;
using TMPro; // Viktigt för att kunna ändra texten
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    
    [Header("Referenser")]
    public GameObject dialogueCanvas; // Dra in din Canvas här
    public TextMeshProUGUI dialogueText; // Dra in själva Text-objektet här
    public Camera mainCamera;
    public PlayerStateManager player; // Ditt player-script
    public GameObject guard; // Din gubbe/fiende

    [Header("Inställningar")]
    public float zoomFOV = 35f; // Hur nära kameran zoomar
    private float originalFOV;
    private bool hasTriggered = false;

    void Start()
    {
        originalFOV = mainCamera.fieldOfView;
        dialogueCanvas.SetActive(false); // Se till att den är avstängd från start
    }

    private void OnTriggerEnter(Collider other)
    {
        // Om spelaren går in i den osynliga boxen
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayDialogue());
        }
    }
    

    IEnumerator PlayDialogue()
    {
        // 1. Stoppa spelaren och zooma
        player.enabled = false;
        player.rb.linearVelocity = Vector3.zero;
        player.GetComponentInChildren<Animator>().SetFloat("Speed", 0);
        
        
        mainCamera.fieldOfView = zoomFOV;

        
        // 2. Visa dialogen steg för steg
        dialogueCanvas.SetActive(true);

        dialogueText.text = "Wait... How did you get out of your cage?";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "Where even am I? I... I don't even know how I got here..";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "You little rat... You think you've escaped?";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "GO ON THEN! RUN! I haven't had a good hunt in weeks!";
        yield return new WaitForSeconds(1.5f); 

        // 3. Starta jakten!
        dialogueCanvas.SetActive(false);
        mainCamera.fieldOfView = originalFOV; // Zooma ut snabbt
        player.enabled = true;

        // Här aktiverar vi gubben (GuardAI)
        guard.GetComponent<GuardAI>().StartChase(); 
        
        Debug.Log("JAKTEN HAR BÖRJAT!");
        Destroy(gameObject); // Ta bort triggern
    }
}