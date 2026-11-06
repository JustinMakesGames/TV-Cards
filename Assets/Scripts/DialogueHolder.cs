using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class DialogueEntry
{
    public string name;
    public DialogueBox dialogueBox;
}

[System.Serializable]
public class DialogueBox
{
    public string characterName;
    public string[] dialogueLines;
    public bool hasChoice;
    public BoolEvent firstOption;
    public BoolEvent secondOption;

    public DialogueBox(string characterName, string[] dialogueLines)
    {
        this.characterName = characterName;
        this.dialogueLines = dialogueLines;
    }

    public DialogueBox(string characterName, string[] dialogueLines, bool hasChoice, BoolEvent firstOption, BoolEvent secondOption)
    {
        this.characterName = characterName;
        this.dialogueLines = dialogueLines;
        this.hasChoice = hasChoice;
        this.firstOption = firstOption;
        this.secondOption = secondOption;
    }
}

public class DialogueHolder : MonoBehaviour
{
    public static DialogueHolder Instance;

    [SerializeField] private List<DialogueEntry> entries;
    private Dictionary<string, DialogueBox> _dialogueBoxes = new Dictionary<string, DialogueBox>();
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        _dialogueBoxes = entries.ToDictionary(
            entry => entry.name,
            entry => entry.dialogueBox);
    }

    public DialogueBox ReturnDialogueBox(string key)
    {
        if (_dialogueBoxes.ContainsKey(key))
        {
            return _dialogueBoxes[key];

        }

        return null;
    }


}


