using UnityEngine;
using TMPro;

public class TutorialPrompt : MonoBehaviour 
{
    [Header("Inställningar")]
    [TextArea(3, 10)] 
    public string message; 

    [Header("Referenser")]
    // Dra in samma objekt här som du använder i Bridge-scriptet!
    public GameObject dialogueCanvas; 
    public TextMeshProUGUI dialogueText; 

    private void Start()
    {
        // Se till att det är avstängt från början
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Här kollar vi efter båda taggarna för att vara säkra
        if (other.CompareTag("Player")) 
        {
            if (dialogueText != null && dialogueCanvas != null) 
            {
                dialogueText.text = message;
                dialogueCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (dialogueCanvas != null) 
            {
                dialogueCanvas.SetActive(false);
            }
        }
    }
}