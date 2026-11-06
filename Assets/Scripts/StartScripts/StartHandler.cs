using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHandler : MonoBehaviour
{
    public static StartHandler Instance;

    [SerializeField] private BoolEvent turnOrderHandlerEvent;
    [SerializeField] private BoolEvent destroyCardsEvent;
    private List<Transform> _sortedPlayers = new List<Transform>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

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
        HandlePlayerTurnOrder();

    }

    
    public void HandleNoExplanation()
    {
        StartCoroutine(ShowNoExplanation());
    }

    private IEnumerator ShowNoExplanation()
    {
        yield return StartCoroutine(DialogueSystem.Instance.HandleDialogue("NoExplanation"));
        HandlePlayerTurnOrder();

    }

    private void HandlePlayerTurnOrder()
    {
        turnOrderHandlerEvent?.Invoke(true);
    }

    public void SetPlayers(List<Transform> sortedPlayers)
    {
        _sortedPlayers = sortedPlayers;
        StartCoroutine(ShowPlayerOrder());

    }

    private IEnumerator ShowPlayerOrder()
    {

        yield return new WaitForSeconds(1);

        string[] placements = new string[4]
        {
            "first",
            "second",
            "third",
            "fourth"
        };

        for (int i = 0; i < _sortedPlayers.Count; i++)
        {
            print($"{_sortedPlayers[i].name} will start at {placements[i]}");
            yield return DialogueSystem.Instance.HandleSingularDialogue($"{_sortedPlayers[i].name} will start at {placements[i]}");
        }

        destroyCardsEvent?.Invoke(true);
    }

    

    

    
}
