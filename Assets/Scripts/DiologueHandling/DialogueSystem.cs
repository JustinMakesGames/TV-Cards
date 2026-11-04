using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] private GameObject dialogueGameObject;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject optionsTab;

    private DialogueBox _currentDialogueBox;
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

        _currentDialogueBox = DialogueHolder.Instance.ReturnDialogueBox(dialogueType);
        if (_currentDialogueBox == null) yield break;

        dialogueGameObject?.SetActive(true);
        for (int i = 0; i < _currentDialogueBox.dialogueLines.Length; i++)
        {
            _canContinueDialogue = false;
            yield return StartCoroutine(ShowDialogue(_currentDialogueBox.dialogueLines[i]));
            PlayerHolder.SetPlayerDialogueOption(Players.RedPlayer);

            while (!_canContinueDialogue)
            {
                yield return null;
            } 

        }

        if (_currentDialogueBox.hasChoice)
        {
            HandleChoice();
        }

        else
        {
            dialogueGameObject?.SetActive(false);
        }

    }


    public IEnumerator HandleDialogue(string dialogueType, Players player)
    {

        _currentDialogueBox = DialogueHolder.Instance.ReturnDialogueBox(dialogueType);
        if (_currentDialogueBox == null) yield break;

        dialogueGameObject?.SetActive(true);
        for (int i = 0; i < _currentDialogueBox.dialogueLines.Length; i++)
        {
            _canContinueDialogue = false;
            yield return StartCoroutine(ShowDialogue(_currentDialogueBox.dialogueLines[i]));
            PlayerHolder.SetPlayerDialogueOption(player);

            while (!_canContinueDialogue)
            {
                yield return null;
            }

        }

        if (_currentDialogueBox.hasChoice)
        {
            HandleChoice();
        }

        else
        {
            dialogueGameObject?.SetActive(false);
        }

    }
    private void HandleChoice()
    {
        optionsTab.SetActive(true);

        GameObject[,] possibleSelectedGameObjects = new GameObject[1, optionsTab.transform.childCount];

        for (int i = 0; i < optionsTab.transform.childCount; i++)
        {
            possibleSelectedGameObjects[0, i] = optionsTab.transform.GetChild(i).gameObject;
        }

        PlayerHolder.SetPlayerUISelection(Players.RedPlayer, possibleSelectedGameObjects);
    }

    public void PlayerChoosesFirstOption()
    {
        dialogueGameObject?.SetActive(false);
        _currentDialogueBox.firstOption?.Invoke(true);
        optionsTab?.SetActive(false);
    }

    public void PlayersChoosesSecondOption()
    {
        dialogueGameObject?.SetActive(false);
        _currentDialogueBox.secondOption?.Invoke(true);
        optionsTab?.SetActive(false);
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
