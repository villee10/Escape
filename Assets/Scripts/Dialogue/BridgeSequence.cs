using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.AI;

public class BridgeSequence : MonoBehaviour
{
    [Header("Referenser")]
    public GameObject dialogueCanvas; 
    public TextMeshProUGUI dialogueText;
    public PlayerStateManager player; 
    public CanvasGroup blackScreen;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Vi kollar fortfarande efter gubben (forest_man)
        if (other.CompareTag("forest_man") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayBridgeDialogue(other.gameObject));
        }
    }

    IEnumerator PlayBridgeDialogue(GameObject guard)
    {
        // 1. STOPPA GUBBEN (Han funkar ju redan, så vi behåller detta)
        NavMeshAgent agent = guard.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // 2. STOPPA SPELAREN (Här lägger vi till "spiken")
        if (player != null)
        {
            player.enabled = false; 
            player.rb.linearVelocity = Vector3.zero;
            player.rb.angularVelocity = Vector3.zero;
            
            // DENNA RAD STOPPAR GLIDANDET:
            player.rb.isKinematic = true; 
            
            player.GetComponentInChildren<Animator>().SetFloat("Speed", 0);
        }

        // 3. DIALOG (Hårdkodad som du vill ha den)
        dialogueCanvas.SetActive(true);

        dialogueText.text = "You have no idea what awaits you...";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "This path leads only to darkness. Turn back!";
        yield return new WaitForSeconds(3f);

        dialogueCanvas.SetActive(false);

        // 4. BLACKOUT
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            blackScreen.alpha = t;
            yield return null;
        }

        // 5. TA BORT GUBBEN
        Destroy(guard);
        yield return new WaitForSeconds(1f);

        // 6. TONA TILLBAKA
        while (t > 0)
        {
            t -= Time.deltaTime;
            blackScreen.alpha = t;
            yield return null;
        }

        // 7. SLÄPP SPELAREN FRI (Viktigt: stäng av isKinematic igen!)
        if (player != null)
        {
            player.rb.isKinematic = false; // Nu kan du gå igen!
            player.enabled = true;
        }
        
        Destroy(gameObject);
    }
}