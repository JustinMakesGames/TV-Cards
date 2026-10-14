using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ImageKind
{
    Neutral,
    Happy,
    Sad,
    Shocked,
    Angry
}
public class DialogueBox
{
    public ImageKind characterImage;
    public string characterName;
    public string dialogue;

    public DialogueBox(ImageKind characterImage, string characterName, string dialogue)
    {
        this.characterImage = characterImage;
        this.characterName = characterName;
        this.dialogue = dialogue;
    }
}
public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] private TMP_Text dialogueText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public IEnumerator HandleText(DialogueBox dialogueBox)
    {
        string text = dialogueBox.dialogue;
        string currentText = "";

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i];
            dialogueText.text = currentText;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
