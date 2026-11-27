using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardPickingType
{
    Random,
    TargetPlayer
}

[RequireComponent(typeof(PlayerData))]
public class CPUCardHandling : MonoBehaviour
{
    private PlayerCardPickSystem _playerCardPickSystem;
    private PlayerData _playerData;

    [SerializeField] private CardPickingType cardPickingType = CardPickingType.TargetPlayer;

    [SerializeField] private int minimumCardMovement;
    [SerializeField] private int maximumCardMovement;
    [SerializeField] private float minimumThinkTime;
    [SerializeField] private float maximumThinkTime;

    [SerializeField] private float chosenCardHoverTime = 0.6f;

    private void Awake()
    {
        _playerCardPickSystem = GetComponent<PlayerCardPickSystem>();
        _playerData = GetComponent<PlayerData>();
    }

    public void StartCardPicking()
    {
        List<GameObject> cards = _playerCardPickSystem.ReturnCards();
        if (cards == null || cards.Count == 0) return;

        if (ContainsCard<TurnOrderCard>(cards))
        {
            StartCoroutine(PickTurnOrderCard());
            return;
        }

        if (cardPickingType == CardPickingType.Random)
        {
            StartCoroutine(CalculateRandomCardPicking());
            return;
        }

        GameObject chosenCard = ChooseCard(cards);

        if (chosenCard == null)
        {
            Debug.LogWarning($"{name} (CPU) found nothing to press.");
            return;
        }

        StartCoroutine(PickCard(chosenCard));
    }

    private GameObject ChooseCard(List<GameObject> cards)
    {
        if (ContainsCard<MovementCardHandling>(cards)) return ChooseMovementCard(cards);
        if (ContainsCard<AttackCardHandler>(cards)) return ChooseAttackCard(cards);

        return ChooseOption(cards);
    }

    private GameObject ChooseOption(List<GameObject> cards)
    {
        GameObject movementOption = ReturnOption(cards, _playerData.ReturnMovementCardFolder());
        if (movementOption && CPUBoardHelper.HasMovementCards(_playerData)) return movementOption;

        GameObject attackOption = ReturnOption(cards, _playerData.ReturnAttackCardFolder());
        if (attackOption && ReturnBestAttackCard() != null) return attackOption;

        foreach (GameObject card in cards)
        {
            if (card.TryGetComponent(out EndTurnCardHandler _)) return card;
        }

        return null;
    }

    private GameObject ChooseMovementCard(List<GameObject> cards)
    {
        if (!TryGetComponent(out CPUMovementHandler movementHandler))
        {
            movementHandler = gameObject.AddComponent<CPUMovementHandler>();
        }

        Vector2Int position = _playerData.ReturnPlayerPosition();
        List<GameObject> bestCards = new List<GameObject>();
        int bestScore = int.MaxValue;

        foreach (GameObject card in cards)
        {
            if (!card.TryGetComponent(out MovementCardHandling movementCard)) continue;

            movementHandler.PlanPath(position, movementCard.ReturnMovement(), out int score);

            if (score < bestScore)
            {
                bestScore = score;
                bestCards.Clear();
            }

            if (score == bestScore) bestCards.Add(card);
        }

        return bestCards.Count > 0 ? bestCards[Random.Range(0, bestCards.Count)] : null;
    }

    private GameObject ChooseAttackCard(List<GameObject> cards)
    {
        AttackCardHandler bestCard = ReturnBestAttackCard();
        if (bestCard != null && cards.Contains(bestCard.gameObject)) return bestCard.gameObject;

        foreach (GameObject card in cards)
        {
            if (card.TryGetComponent(out AttackCardHandler _)) return card;
        }

        return null;
    }

    private AttackCardHandler ReturnBestAttackCard()
    {
        Vector2Int position = _playerData.ReturnPlayerPosition();
        List<Vector2Int> opponents = CPUBoardHelper.ReturnOpponentPositions(transform);

        List<AttackCardHandler> bestCards = new List<AttackCardHandler>();
        int bestDamage = int.MinValue;

        foreach (AttackCardHandler card in CPUBoardHelper.ReturnAttackCards(_playerData))
        {
            if (!CPUBoardHelper.CanHitFrom(position, card.ReturnRange(), opponents)) continue;

            if (card.ReturnDamage() > bestDamage)
            {
                bestDamage = card.ReturnDamage();
                bestCards.Clear();
            }

            if (card.ReturnDamage() == bestDamage) bestCards.Add(card);
        }

        return bestCards.Count > 0 ? bestCards[Random.Range(0, bestCards.Count)] : null;
    }

    private GameObject ReturnOption(List<GameObject> cards, Transform folder)
    {
        if (!folder) return null;

        foreach (GameObject card in cards)
        {
            if (card.TryGetComponent(out OptionsCardHandler option) && option.ReturnCardFolder() == folder)
            {
                return card;
            }
        }

        return null;
    }

    private bool ContainsCard<T>(List<GameObject> cards) where T : Component
    {
        foreach (GameObject card in cards)
        {
            if (card.TryGetComponent(out T _)) return true;
        }

        return false;
    }

    private IEnumerator CalculateRandomCardPicking()
    {
        int randomCardMovement = Random.Range(minimumCardMovement, maximumCardMovement + 1);

        for (int i = 0; i < randomCardMovement; i++)
        {
            yield return new WaitForSeconds(GetRandomThinkTime());
            int randomDirection = (Random.Range(0, 2) == 0 ? 1 : -1);

            _playerCardPickSystem.HandleMovement(randomDirection);
        }
        yield return new WaitForSeconds(GetRandomThinkTime());

        _playerCardPickSystem.SelectCard();
    }

    private IEnumerator PickTurnOrderCard()
    {
        GameObject chosenCard = null;

        while (true)
        {
            List<GameObject> cards = _playerCardPickSystem.ReturnCards();
            if (cards == null || cards.Count == 0) yield break;

            if (!cards.Contains(chosenCard))
            {
                chosenCard = cards[Random.Range(0, cards.Count)];
            }

            int targetIndex = cards.IndexOf(chosenCard);
            int currentIndex = _playerCardPickSystem.ReturnCurrentLocation();

            if (targetIndex == currentIndex)
            {
                yield return new WaitForSeconds(Mathf.Max(chosenCardHoverTime, GetRandomThinkTime()));

                if (_playerCardPickSystem.ReturnCards().Contains(chosenCard)
                    && _playerCardPickSystem.ReturnCurrentLocation() == _playerCardPickSystem.ReturnCards().IndexOf(chosenCard))
                {
                    _playerCardPickSystem.SelectCard();
                    yield break;
                }

                continue;
            }

            yield return new WaitForSeconds(GetRandomThinkTime());
            _playerCardPickSystem.HandleMovement(ReturnShortestDirection(currentIndex, targetIndex, cards.Count));
        }
    }

    private IEnumerator PickCard(GameObject chosenCard)
    {
        while (true)
        {
            List<GameObject> cards = _playerCardPickSystem.ReturnCards();
            int targetIndex = cards.IndexOf(chosenCard);
            int currentIndex = _playerCardPickSystem.ReturnCurrentLocation();

            if (targetIndex < 0 || targetIndex == currentIndex) break;

            yield return new WaitForSeconds(GetRandomThinkTime());
            _playerCardPickSystem.HandleMovement(ReturnShortestDirection(currentIndex, targetIndex, cards.Count));
        }

        yield return new WaitForSeconds(Mathf.Max(chosenCardHoverTime, GetRandomThinkTime()));

        _playerCardPickSystem.SelectCard();
    }

    private int ReturnShortestDirection(int from, int to, int count)
    {
        int forward = (to - from + count) % count;
        int backward = (from - to + count) % count;
        return forward <= backward ? 1 : -1;
    }

    private float GetRandomThinkTime()
    {
        return Random.Range(minimumThinkTime, maximumThinkTime);
    }
}
