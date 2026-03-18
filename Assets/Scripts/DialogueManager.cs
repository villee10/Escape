using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText; // Dra in din NPCName här
    public TextMeshProUGUI dialogueText;
    public GameObject responseContainer; // Dra in objektet som håller alla options
    
    public TextMeshProUGUI[] optionTexts; // En lista för alla 3 options

    private DialogueNode currentNode;

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        // Om det FINNS val, lyssna på 1, 2, 3
        if (currentNode && !string.IsNullOrEmpty(currentNode.option1))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectOption(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectOption(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectOption(3);
        }
        // Om det INTE finns val (Intro), tryck Space för nästa
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextNode();
        }
    }

    public void DisplayNode(DialogueNode node)
    {
        currentNode = node;
        dialoguePanel.SetActive(true);
        
        nameText.text = node.speakerName;
        dialogueText.text = node.npcDialogue;

        // Om options är tomma, göm hela svars-rutan
        if (string.IsNullOrEmpty(node.option1))
        {
            responseContainer.SetActive(false);
        }
        else
        {
            responseContainer.SetActive(true);
            optionTexts[0].text = "1. " + node.option1;
            optionTexts[1].text = "2. " + node.option2;
            optionTexts[2].text = "3. " + node.option3;
        }
    }

    void NextNode()
    {
        if (currentNode.nextNode)
            DisplayNode(currentNode.nextNode);
        else
            EndDialogue();
    }

    void SelectOption(int i) { EndDialogue(); } // Just nu stänger vi bara
    public void EndDialogue() { dialoguePanel.SetActive(false); }
}