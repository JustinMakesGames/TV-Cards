using NUnit.Framework.Internal.Filters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MemoryCard
{
    public GameObject card;
    public Transform row;


    public MemoryCard(GameObject card, Transform row)
    {
        this.card = card;
        this.row = row;
    }
}
public class MemoryGameManager : MonoBehaviour, IMinigameStart
{

    public static MemoryGameManager Instance;
    [SerializeField] private List<Transform> players = new List<Transform>();
    [SerializeField] private Transform firstRow, secondRow, thirdRow;
    [SerializeField] private List<FruitObject> fruitObjects = new List<FruitObject>();

    public MemoryCard[,] cards = new MemoryCard[5, 3];

    public GameObject[,] cardObjects = new GameObject[5, 3];

    [SerializeField] private GameObject showCardObject;
    [SerializeField] private FruitObject chosenFruitObject;
    [SerializeField] private float selectionBorderSize = 8f;

    [Header("Shuffle")]
    [SerializeField] private float shuffleSpeed = 800f;
    [SerializeField] private float shuffleDuration = 2f;
    [SerializeField] private float returnSpeed = 1200f;


    public bool IsShuffling { get; private set; }
    public event System.Action ShuffleFinished;

    private Vector3[,] slotWorldPositions = new Vector3[5, 3];

    private Dictionary<Transform, FruitObject> _chosenPlayers = new Dictionary<Transform, FruitObject>();
    private List<Transform> _choiceOrder = new List<Transform>();

    [Header("Points")]
    [SerializeField] private int[] pointsPerPlace = { 5, 3, 2, 1 };

    [Header("Rounds")]
    [SerializeField] private int roundAmount = 5;
    [SerializeField] private float revealTime = 3f;
    [SerializeField] private float timeBetweenRounds = 1.5f;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text announcementText;

    public int CurrentRound { get; private set; }
    public event System.Action<List<Transform>> GameFinished;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    public Vector3 GetSlotPosition(int column, int row)
    {
        return slotWorldPositions[column, row];
    }

    public bool TryGetGridPosition(Transform card, out int column, out int row)
    {
        for (row = 0; row < cards.GetLength(1); row++)
        {
            for (column = 0; column < cards.GetLength(0); column++)
            {
                if (cards[column, row] != null && cards[column, row].card.transform == card)
                    return true;
            }
        }
        column = row = -1;
        return false;
    }

    public void StartMinigame()
    {
        CollectCards();
        StartCoroutine(GameRoutine());
    }

    private IEnumerator GameRoutine()
    {
        foreach (Transform player in players)
        {
            PlayerMemoryScoreAmount score = GetScore(player);
            if (score != null) score.ResetPoints();
        }

        if (announcementText != null) announcementText.gameObject.SetActive(false);

        for (int round = 1; round <= roundAmount; round++)
        {
            CurrentRound = round;
            if (roundText != null) roundText.text = $"Round {round}/{roundAmount}";

            ResetRound();
            AssignFruits();
            ShowCardToSearch();

            yield return ShowCards();

            ShuffleCards();
            yield return new WaitUntil(() => !IsShuffling);

            GivePlayersTurn();

            yield return new WaitUntil(() => _chosenPlayers.Count >= players.Count);

            yield return RevealCards();
            CheckIfRight();

            yield return new WaitForSeconds(timeBetweenRounds);
        }

        AnnounceWinner();
    }

    private void ResetRound()
    {
        _chosenPlayers.Clear();
        _choiceOrder.Clear();

        foreach (MemoryCard memoryCard in cards)
        {
            if (memoryCard == null) continue;
            Transform border = memoryCard.card.transform.Find("SelectionBorder");
            if (border != null) border.gameObject.SetActive(false);
        }

        UpdateCardObjects();
    }

    private IEnumerator RevealCards()
    {
        foreach (MemoryCard memoryCard in cards)
        {
            if (memoryCard == null) continue;
            Animator animator = memoryCard.card.GetComponentInChildren<Animator>();
            if (animator != null) animator.SetTrigger("RotateForward");
        }

        yield return new WaitForSeconds(revealTime);

        foreach (MemoryCard memoryCard in cards)
        {
            if (memoryCard == null) continue;
            Animator animator = memoryCard.card.GetComponentInChildren<Animator>();
            if (animator != null) animator.SetTrigger("RotateBackwards");
        }
    }

    private void AnnounceWinner()
    {
        int bestPoints = int.MinValue;
        List<Transform> winners = new List<Transform>();

        foreach (Transform player in players)
        {
            PlayerMemoryScoreAmount score = GetScore(player);
            int points = score != null ? score.points : 0;

            if (points > bestPoints)
            {
                bestPoints = points;
                winners.Clear();
            }

            if (points == bestPoints) winners.Add(player);
        }

        string message;
        if (winners.Count == 1)
            message = $"{GetPlayerName(winners[0])} wins with {bestPoints} points!";
        else
            message = $"It's a tie! {string.Join(" & ", winners.Select(GetPlayerName))} win with {bestPoints} points!";

        Debug.Log(message);

        if (announcementText != null)
        {
            announcementText.text = message;
            announcementText.color = winners.Count == 1 ? GetPlayerColor(winners[0]) : Color.white;
            announcementText.gameObject.SetActive(true);
        }

        GameFinished?.Invoke(winners);
    }

    private string GetPlayerName(Transform player)
    {
        PlayerData playerData = player.GetComponentInParent<PlayerData>();
        if (playerData == null) playerData = player.GetComponentInChildren<PlayerData>();
        return playerData != null && !string.IsNullOrEmpty(playerData.playerName) ? playerData.playerName : player.name;
    }

    private PlayerMemoryScoreAmount GetScore(Transform player)
    {
        PlayerMemoryScoreAmount score = player.GetComponentInChildren<PlayerMemoryScoreAmount>(true);
        if (score == null) score = player.GetComponentInParent<PlayerMemoryScoreAmount>();
        return score;
    }

    public void ShuffleCards()
    {
        if (IsShuffling) return;
        StartCoroutine(ShuffleRoutine());
    }

    private IEnumerator ShuffleRoutine()
    {
        IsShuffling = true;

        int columns = cards.GetLength(0);
        int rowCount = cards.GetLength(1);
        Transform[] rows = { firstRow, secondRow, thirdRow };

        List<MemoryCard> allCards = new List<MemoryCard>();
        for (int r = 0; r < rowCount; r++)
            for (int c = 0; c < columns; c++)
                if (cards[c, r] != null) allCards.Add(cards[c, r]);

        foreach (MemoryCard memoryCard in allCards)
            GetMover(memoryCard).StartMoving(shuffleSpeed);

        yield return new WaitForSeconds(shuffleDuration);

        for (int i = allCards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (allCards[i], allCards[j]) = (allCards[j], allCards[i]);
        }

        int index = 0;
        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (cards[c, r] == null) continue;

                MemoryCard memoryCard = allCards[index++];
                memoryCard.row = rows[r];
                memoryCard.card.transform.SetParent(rows[r], true);
                cards[c, r] = memoryCard;

                GetMover(memoryCard).MoveToGridPosition(c, r, returnSpeed);
            }
        }

        UpdateCardObjects();

        yield return new WaitUntil(() => allCards.All(card => !GetMover(card).IsMoving));

        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (cards[c, r] == null) continue;
                cards[c, r].card.transform.position = slotWorldPositions[c, r];
                cards[c, r].card.transform.SetSiblingIndex(c);
            }
        }

        IsShuffling = false;
        ShuffleFinished?.Invoke();
    }

    public void UpdateCardObjects()
    {
        int columns = cards.GetLength(0);
        int rowCount = cards.GetLength(1);
        if (cardObjects == null || cardObjects.GetLength(0) != columns || cardObjects.GetLength(1) != rowCount)
            cardObjects = new GameObject[columns, rowCount];

        for (int r = 0; r < rowCount; r++)
            for (int c = 0; c < columns; c++)
                cardObjects[c, r] = cards[c, r] != null ? cards[c, r].card.gameObject : null;
    }

    private CardMover GetMover(MemoryCard memoryCard)
    {
        CardMover mover = memoryCard.card.GetComponent<CardMover>();
        if (mover == null) mover = memoryCard.card.gameObject.AddComponent<CardMover>();
        return mover;
    }

    private void CollectCards()
    {
        CollectCardsFromRow(firstRow, 0);
        CollectCardsFromRow(secondRow, 1);
        CollectCardsFromRow(thirdRow, 2);
        UpdateCardObjects();

    }

    private void AssignFruits()
    {
        List<CardHandler> handlers = new List<CardHandler>();
        foreach (MemoryCard memoryCard in cards)
        {
            if (memoryCard == null) continue;
            CardHandler handler = memoryCard.card.GetComponentInChildren<CardHandler>(true);
            if (handler != null) handlers.Add(handler);
            else Debug.LogWarning($"{memoryCard.card.name} has no CardHandler.", memoryCard.card);
        }

        if (fruitObjects.Count == 0 || handlers.Count == 0) return;

        int cardsPerFruit = handlers.Count / fruitObjects.Count;
        if (handlers.Count % fruitObjects.Count != 0)
            Debug.LogWarning($"{handlers.Count} cards can't be split evenly over {fruitObjects.Count} fruits; some fruits get one extra card.");

        List<FruitObject> pool = new List<FruitObject>();
        foreach (FruitObject fruit in fruitObjects)
            for (int i = 0; i < cardsPerFruit; i++)
                pool.Add(fruit);
        for (int i = 0; pool.Count < handlers.Count; i++)
            pool.Add(fruitObjects[i % fruitObjects.Count]);

        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        for (int i = 0; i < handlers.Count; i++)
            handlers[i].SetPlayerStats(pool[i]);
    }

    private void CollectCardsFromRow(Transform row, int rowNumber)
    {
        for (int i = 0; i < row.childCount; i++)
        {
            cards[i, rowNumber] = new MemoryCard(row.GetChild(i).gameObject, row);
            slotWorldPositions[i, rowNumber] = row.GetChild(i).position;
        }
    }
    private IEnumerator ShowCards()
    {
        for (int y = 0; y < cards.GetLength(1); y++)
        {
            for (int x = 0; x < cards.GetLength(0); x++)
            {
                StartCoroutine(ShowSingleCard(cards[x, y]));
            }
        }

        yield return new WaitForSeconds(5);
    }

    private IEnumerator ShowSingleCard(MemoryCard cardClass)
    {
        GameObject card = cardClass.card;

        Animator animator = card.GetComponentInChildren<Animator>();

        animator.SetTrigger("RotateForward");


        yield return new WaitForSeconds(3);

        animator.SetTrigger("RotateBackwards");

    }

    private void GivePlayersTurn()
    {
        foreach (Transform player in players)
        {
            if (player.TryGetComponent(out PlayerUISelectionHandler playerUISelectionHandler))
            {
                playerUISelectionHandler.SetPossibleGameObjects(cardObjects);

                if (IsCPU(player))
                {
                    if (!player.TryGetComponent(out CPUMemoryHandler cpuMemoryHandler))
                        cpuMemoryHandler = player.gameObject.AddComponent<CPUMemoryHandler>();

                    cpuMemoryHandler.StartChoosing(chosenFruitObject);
                }
            }
        }
    }

    private bool IsCPU(Transform player)
    {
        return player.TryGetComponent(out PlayerOrCPUChecker checker) && checker.PlayerOrCPU() == PlayerOrCPU.CPU;
    }

    public void HandlePlayerChoice(Transform player, FruitObject fruitObject, GameObject fruitCard)
    {
        if (_chosenPlayers.ContainsKey(player)) return;

        _chosenPlayers[player] = fruitObject;
        _choiceOrder.Add(player);

        ShowSelectionBorder(fruitCard, GetPlayerColor(player));
        RemoveCardFromOptions(fruitCard);

        foreach (Transform otherPlayer in players)
        {
            if (_chosenPlayers.ContainsKey(otherPlayer)) continue;

            if (otherPlayer.TryGetComponent(out PlayerUISelectionHandler playerUISelectionHandler))
            {
                playerUISelectionHandler.UpdatePossibleGameObjects(cardObjects);
            }
        }
    }

    private Color GetPlayerColor(Transform player)
    {
        PlayerData playerData = player.GetComponentInParent<PlayerData>();
        if (playerData == null) playerData = player.GetComponentInChildren<PlayerData>();
        return playerData != null ? playerData.playerColor : Color.white;
    }

    private void ShowSelectionBorder(GameObject fruitCard, Color color)
    {
        if (fruitCard == null) return;

        Transform existing = fruitCard.transform.Find("SelectionBorder");
        GameObject border = existing != null ? existing.gameObject : CreateSelectionBorder(fruitCard.transform);

        border.GetComponent<UnityEngine.UI.Image>().color = color;
        border.SetActive(true);
    }

    private GameObject CreateSelectionBorder(Transform card)
    {
        GameObject border = new GameObject("SelectionBorder", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.LayoutElement));
        border.transform.SetParent(card, false);
        border.transform.SetAsFirstSibling();

        RectTransform rect = (RectTransform)border.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(-selectionBorderSize, -selectionBorderSize);
        rect.offsetMax = new Vector2(selectionBorderSize, selectionBorderSize);

        border.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
        border.GetComponent<UnityEngine.UI.LayoutElement>().ignoreLayout = true;

        return border;
    }

    private void RemoveCardFromOptions(GameObject fruitCard)
    {
        for (int r = 0; r < cardObjects.GetLength(1); r++)
        {
            for (int c = 0; c < cardObjects.GetLength(0); c++)
            {
                if (cardObjects[c, r] == fruitCard)
                {
                    cardObjects[c, r] = null;
                    return;
                }
            }
        }
    }

    private void ShowCardToSearch()
    {
        if (showCardObject.TryGetComponent(out ShowCardHandler showCardHandler))
        {
            int randomFruitSelection = Random.Range(0, fruitObjects.Count);

            chosenFruitObject = fruitObjects[randomFruitSelection];
            showCardHandler.ShowCardToFind(chosenFruitObject);
        }
    }

    private void CheckIfRight()
    {
        int rank = 0;

        foreach (Transform player in _choiceOrder)
        {
            bool isRight = _chosenPlayers[player] == chosenFruitObject;
            int points = 0;

            if (isRight)
            {
                points = rank < pointsPerPlace.Length ? pointsPerPlace[rank] : 0;
                rank++;
            }

            PlayerMemoryScoreAmount score = GetScore(player);

            if (score != null) score.GivePoints(points);
            else Debug.LogWarning($"{player.name} has no PlayerMemoryScoreAmount.", player);
        }
    }
    

}
