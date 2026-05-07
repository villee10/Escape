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
    public GameObject guard; 
    public Transform guardTargetPoint; 

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayBridgeCutscene());
        }
    }

    IEnumerator PlayBridgeCutscene()
    {
        // 1. FRYS SPELAREN
        player.isInCutscene = true;
        player.SwitchState(player.IdleState);
    
        if (player.anim != null)
        {
            player.anim.SetFloat("Speed", 0f);
            player.anim.Play("Idle", 0, 0f);
        }

        // 2. FIXA GUBBEN (GUARD)
        NavMeshAgent agent = guard.GetComponent<NavMeshAgent>();
        if (agent != null) {
            agent.isStopped = true;
            agent.enabled = false; 
        }
        guard.transform.position = guardTargetPoint.position;
        guard.transform.rotation = guardTargetPoint.rotation;

        // 3. DIALOG
        dialogueCanvas.SetActive(true);
        dialogueText.text = "You have no idea what awaits you...";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "This path leads only to darkness. Turn back!";
        yield return new WaitForSeconds(3f);

        // 4. BLINK-EFFEKT
        float t = 0;
        while (t < 1) {
            t += Time.deltaTime * 2f;
            blackScreen.alpha = t;
            yield return null;
        }

        dialogueCanvas.SetActive(false);
        if (guard != null) Destroy(guard); 
        yield return new WaitForSeconds(1f);

        while (t > 0) {
            t -= Time.deltaTime * 1.5f;
            blackScreen.alpha = t;
            yield return null;
        }

        // 5. SLÄPP KONTROLLEN
        player.isInCutscene = false;
        Destroy(gameObject);
    }
}