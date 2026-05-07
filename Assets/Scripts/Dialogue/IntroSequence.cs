using UnityEngine;
using TMPro;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    [Header("Referenser")]
    public GameObject dialogueCanvas; 
    public TextMeshProUGUI dialogueText; 
    public Camera mainCamera;
    public PlayerStateManager player; 
    public GameObject guard; 

    [Header("Inställningar")]
    public float zoomFOV = 35f; 
    private float originalFOV;
    private bool hasTriggered = false;

    void Start()
    {
        if (mainCamera != null) originalFOV = mainCamera.fieldOfView;
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayDialogue());
        }
    }

    IEnumerator PlayDialogue()
    {
        // 1. FRYS SPELAREN VIA isInCutscene
        if (player != null)
        {
            player.isInCutscene = true; // Aktiverar pausen i PlayerStateManager
            player.SwitchState(player.IdleState); // Tvingar state-maskinen till Idle

            // Tvinga farten till noll fysiskt
            if (player.rb != null)
            {
                player.rb.linearVelocity = Vector3.zero;
                player.rb.angularVelocity = Vector3.zero;
            }

            // Tvinga animationen till Idle direkt
            if (player.anim != null)
            {
                player.anim.SetFloat("Speed", 0f);
                player.anim.SetFloat("inputX", 0f);
                player.anim.SetFloat("inputZ", 0f);
                
                // Spela Idle-animationen omedelbart för att avbryta Run-cykeln
                player.anim.Play("Idle", 0, 0f); 
            }
        }

        // 2. HANTERA KAMERAN
        if (mainCamera != null) 
        {
            mainCamera.fieldOfView = zoomFOV;
        }
        
        // 3. VISA DIALOGEN STEG FÖR STEG
        if (dialogueCanvas != null) 
        {
            dialogueCanvas.SetActive(true);
        }

        dialogueText.text = "Wait... How did you get out of your cage?";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "Where even am I? I... I don't even know how I got here..";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "You little rat... You think you've escaped?";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "GO ON THEN! RUN! I haven't had a good hunt in weeks!";
        yield return new WaitForSeconds(1.5f); 

        // 4. STARTA JAKTEN OCH ÅTERSTÄLL KONTROLLEN
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        
        if (mainCamera != null) 
        {
            mainCamera.fieldOfView = originalFOV; // Zooma ut
        }

        if (player != null)
        {
            player.isInCutscene = false; // Släpp kontrollen fri igen!
        }

        // 5. AKTIVERA GUBBEN (GuardAI)
        if (guard != null)
        {
            GuardAI guardScript = guard.GetComponent<GuardAI>();
            if (guardScript != null)
            {
                guardScript.StartChase();
            }
        }
        
        Debug.Log("JAKTEN HAR BÖRJAT!");
        Destroy(gameObject); // Ta bort triggern
    }
}