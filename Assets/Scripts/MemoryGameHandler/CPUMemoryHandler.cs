using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerUISelectionHandler))]
public class CPUMemoryHandler : MonoBehaviour
{
    [SerializeField, Range(0.5f, 1f)] private float accuracy = 0.7f;
    [SerializeField] private int picksPerSchedule = 10;

    [SerializeField] private float minimumThinkTime = 0.25f;
    [SerializeField] private float maximumThinkTime = 0.6f;
    [SerializeField] private float chosenCardHoverTime = 0.6f;

    private readonly Queue<bool> _schedule = new Queue<bool>();
    private PlayerUISelectionHandler _selectionHandler;

    private void Awake()
    {
        _selectionHandler = GetComponent<PlayerUISelectionHandler>();
    }

    public void StartChoosing(FruitObject fruitToFind)
    {
        StopAllCoroutines();
        StartCoroutine(ChooseRoutine(fruitToFind));
    }

    private int CalculateCorrectPicks()
    {
        int total = Mathf.Max(1, picksPerSchedule);
        int correct = Mathf.RoundToInt(total * accuracy);

        int minimumCorrect = total / 2 + 1;
        return Mathf.Clamp(correct, Mathf.Min(minimumCorrect, total), total);
    }

    private void FillSchedule()
    {
        int total = Mathf.Max(1, picksPerSchedule);
        int correct = CalculateCorrectPicks();

        List<bool> picks = new List<bool>();
        for (int i = 0; i < total; i++) picks.Add(i < correct);

        for (int i = picks.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (picks[i], picks[j]) = (picks[j], picks[i]);
        }

        foreach (bool pick in picks) _schedule.Enqueue(pick);

        Debug.Log($"{name} (CPU) memory schedule: {correct} right, {total - correct} wrong.");
    }

    private bool NextPickIsCorrect()
    {
        if (_schedule.Count == 0) FillSchedule();
        return _schedule.Dequeue();
    }

    private IEnumerator ChooseRoutine(FruitObject fruitToFind)
    {
        yield return null;

        bool wantsCorrect = NextPickIsCorrect();
        Vector2Int? target = null;

        while (_selectionHandler.CanSelect)
        {
            GameObject[,] options = _selectionHandler.PossibleGameObjects;
            if (options == null) yield break;

            if (target == null || !IsStillAvailable(options, target.Value))
            {
                target = PickTarget(options, fruitToFind, wantsCorrect);
                if (target == null) yield break;
            }

            Vector2Int current = _selectionHandler.Coordinates;

            if (current == target.Value)
            {
                yield return new WaitForSeconds(Mathf.Max(chosenCardHoverTime, GetRandomThinkTime()));

                if (_selectionHandler.Coordinates == target.Value && IsStillAvailable(_selectionHandler.PossibleGameObjects, target.Value))
                {
                    _selectionHandler.Press();
                    yield break;
                }

                continue;
            }

            yield return new WaitForSeconds(GetRandomThinkTime());
            StepTowards(target.Value);
        }
    }

    private void StepTowards(Vector2Int target)
    {
        Vector2Int before = _selectionHandler.Coordinates;
        Vector2Int difference = target - before;

        if (difference.x != 0)
            _selectionHandler.Move(new Vector2Int((int)Mathf.Sign(difference.x), 0));

        if (_selectionHandler.Coordinates == before && difference.y != 0)
            _selectionHandler.Move(new Vector2Int(0, (int)Mathf.Sign(difference.y)));

        if (_selectionHandler.Coordinates == before)
            _selectionHandler.SelectCoordinates(target);
    }

    private Vector2Int? PickTarget(GameObject[,] options, FruitObject fruitToFind, bool wantsCorrect)
    {
        List<Vector2Int> rightCards = new List<Vector2Int>();
        List<Vector2Int> wrongCards = new List<Vector2Int>();

        for (int y = 0; y < options.GetLength(1); y++)
        {
            for (int x = 0; x < options.GetLength(0); x++)
            {
                GameObject card = options[x, y];
                if (card == null) continue;

                CardHandler handler = card.GetComponentInChildren<CardHandler>(true);
                bool isRight = handler != null && handler.Fruit == fruitToFind;
                (isRight ? rightCards : wrongCards).Add(new Vector2Int(x, y));
            }
        }

        List<Vector2Int> pool = wantsCorrect ? rightCards : wrongCards;
        if (pool.Count == 0) pool = wantsCorrect ? wrongCards : rightCards;
        if (pool.Count == 0) return null;

        return pool[Random.Range(0, pool.Count)];
    }

    private bool IsStillAvailable(GameObject[,] options, Vector2Int position)
    {
        return options != null
            && position.x < options.GetLength(0) && position.y < options.GetLength(1)
            && options[position.x, position.y] != null;
    }

    private float GetRandomThinkTime()
    {
        return Random.Range(minimumThinkTime, maximumThinkTime);
    }
}
