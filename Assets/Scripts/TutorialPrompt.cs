using UnityEngine;
using TMPro; // Viktigt för att kunna ändra texten!

public class TutorialPrompt : MonoBehaviour 
{
    [Header("Inställningar")]
    [TextArea(3, 10)] 
    public string message; // Skriv t.ex. "Crouch with C" här i Inspectorn
    
    [Header("Referenser")]
    public GameObject uiContainer; // Dra in "crouch with C"-objektet här
    public TextMeshProUGUI textElement; // Dra in själva Text-komponenten här

    void Start()
    {
        // Denna rad gör att du slipper bocka ur objektet manuellt!
        if (uiContainer != null)
        {
            uiContainer.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Reagerar på din pappa (Player)
        if (other.CompareTag("Player")) 
        {
            Debug.Log("JAG SER SPELAREN!"); // Kolla i Console-fliken längst ner!

            if (textElement != null) 
            {
                textElement.text = message;
            }

            if (uiContainer != null) 
            {
                uiContainer.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (uiContainer != null) uiContainer.SetActive(false);
        }
    }
    
    
    
}