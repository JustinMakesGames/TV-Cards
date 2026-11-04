using System.Collections;
using UnityEngine;

public class StartHandler : MonoBehaviour
{
    private void Start()
    {
        HandleStart();
    }
    public void HandleStart()
    {
        HandleIntroductionDialogue();
    }

    private void HandleIntroductionDialogue()
    {
        StartCoroutine(DialogueSystem.Instance.HandleDialogue("Introduction"));

    }

    public void HandleExplanation()
    {
        StartCoroutine(ShowExplanation());
    }

    private IEnumerator ShowExplanation()
    {
        yield return StartCoroutine(DialogueSystem.Instance.HandleDialogue("Explanation"));
        StartCoroutine(HandlePlayerTurnOrder());

    }

    
    public void HandleNoExplanation()
    {
        StartCoroutine(ShowNoExplanation());
    }

    private IEnumerator ShowNoExplanation()
    {
        yield return StartCoroutine(DialogueSystem.Instance.HandleDialogue("NoExplanation"));
        StartCoroutine(HandlePlayerTurnOrder());

    }

    private IEnumerator HandlePlayerTurnOrder()
    {
        yield return null;
    }
}
