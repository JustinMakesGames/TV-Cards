using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardPickingType
{
    Random,
    TargetPlayer
}

public class CPUCardHandling : MonoBehaviour
{
    private PlayerCardPickSystem _playerCardPickSystem;

    [SerializeField] private CardPickingType cardPickingType = CardPickingType.TargetPlayer;


    [SerializeField] private int minimumCardMovement;
    [SerializeField] private int maximumCardMovement;
    [SerializeField] private float minimumThinkTime;
    [SerializeField] private float maximumThinkTime;


    [SerializeField] private float matchingCardPreference = 2f;
    [SerializeField] private float chosenCardHoverTime = 0.6f;

    private void Awake()
    {
        _playerCardPickSystem = GetComponent<PlayerCardPickSystem>();
    }
    public void StartCardPicking()
    {
        // For now the CPU always opens the movement folder from the options menu.
        GameObject movementOption = ReturnMovementOption();
        if (movementOption != null)
        {
            StartCoroutine(PickCard(movementOption));
            return;
        }

        if (cardPickingType == CardPickingType.TargetPlayer && HasMovementCards())
        {
            // Add the targeting component if the scene doesn't have it yet.
            if (!TryGetComponent(out CPUTargetHandler targetHandler))
            {
                targetHandler = gameObject.AddComponent<CPUTargetHandler>();
            }

            StartCoroutine(CalculateTargetCardPicking(targetHandler));
            return;
        }

        StartCoroutine(CalculateRandomCardPicking());
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

    private IEnumerator CalculateTargetCardPicking(CPUTargetHandler targetHandler)
    {
        targetHandler.ChooseTarget();
        int distance = targetHandler.ReturnDistanceToTarget();

        GameObject chosenCard = ChooseMovementCard(distance);

        yield return PickCard(chosenCard);
    }

    // Walks the cursor over to the card one card at a time, like a player would, then presses it.
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

    private GameObject ChooseMovementCard(int distance)
    {
        List<GameObject> cards = _playerCardPickSystem.ReturnCards();
        List<float> weights = new List<float>();
        float totalWeight = 0f;

        foreach (GameObject card in cards)
        {
            float weight = 0f;

            if (card.TryGetComponent(out MovementCardHandling movementCard))
            {
                int difference = Mathf.Abs(movementCard.ReturnMovement() - distance);
                weight = 1f / Mathf.Pow(1f + difference, matchingCardPreference);
            }

            weights.Add(weight);
            totalWeight += weight;
        }

        float roll = Random.Range(0f, totalWeight);

        for (int i = 0; i < cards.Count; i++)
        {
            roll -= weights[i];
            if (roll <= 0f && weights[i] > 0f) return cards[i];
        }

        return cards[cards.Count - 1];
    }

    private GameObject ReturnMovementOption()
    {
        List<GameObject> cards = _playerCardPickSystem.ReturnCards();
        if (cards == null || !TryGetComponent(out PlayerData playerData)) return null;

        Transform movementFolder = playerData.ReturnMovementCardFolder();
        List<GameObject> movementOptions = new List<GameObject>();

        foreach (GameObject card in cards)
        {
            if (card.TryGetComponent(out OptionsCardHandler option) && option.ReturnCardFolder() == movementFolder)
            {
                movementOptions.Add(card);
            }
        }

        return movementOptions.Count > 0 ? movementOptions[Random.Range(0, movementOptions.Count)] : null;
    }

    private bool HasMovementCards()
    {
        List<GameObject> cards = _playerCardPickSystem.ReturnCards();
        if (cards == null) return false;

        foreach (GameObject card in cards)
        {
            if (card.TryGetComponent(out MovementCardHandling _)) return true;
        }

        return false;
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
