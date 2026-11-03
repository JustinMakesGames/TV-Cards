using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] private TMP_Text dialogueText;

    private bool _canContinueDialogue;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public IEnumerator HandleDialogue(string dialogueType)
    {

        DialogueBox dialogueBox = DialogueHolder.Instance.ReturnDialogueBox(dialogueType);
        if (dialogueBox == null) yield break;

        for (int i = 0; i < dialogueBox.dialogueLines.Length; i++)
        {
            _canContinueDialogue = false;
            yield return StartCoroutine(ShowDialogue(dialogueBox.dialogueLines[i]));
            PlayerDialogueHolder.SetPlayerDialogueOption(Players.RedPlayer);

            while (!_canContinueDialogue)
            {
                yield return null;
            } 

        }

    }

    public void ContinueDialogue()
    {
        _canContinueDialogue = true;
    }

    public IEnumerator ShowDialogue(string text)
    {
        string currentText = "";

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i];
            dialogueText.text = currentText;
            yield return new WaitForSeconds(.02f);
        }
    }

}
