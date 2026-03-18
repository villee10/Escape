using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public string speakerName; // Nu lägger vi till namn så vi ser vilken vakt som pratar!
    [TextArea(3, 10)]
    public string npcDialogue;

    [Header("Options (Leave empty if just listening)")]
    public string option1;
    public string option2;
    public string option3;

    [Header("Next Part (If no options)")]
    public DialogueNode nextNode; // Dra nästa vakt-mening hit
}