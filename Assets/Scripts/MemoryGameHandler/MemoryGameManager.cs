using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryCard
{
    public Transform card;
    public Transform row;
    

    public MemoryCard(Transform card, Transform row)
    {
        this.card = card;
        this.row = row;
    }
}
public class MemoryGameManager : MonoBehaviour, IMinigameStart
{

    [SerializeField] private Transform firstRow, secondRow, thirdRow;

    public MemoryCard[,] cards = new MemoryCard[5, 3];

    public void StartMinigame()
    {
        CollectCards();
        ShuffleCards();
        StartCoroutine(ShowCards());
    }

    // Shuffles every card in the grid, moves each one into its new row/slot,
    // and updates MemoryCard.row to match.
    public void ShuffleCards()
    {
        int columns = cards.GetLength(0);
        int rowCount = cards.GetLength(1);
        Transform[] rows = { firstRow, secondRow, thirdRow };

        // Remember where each slot is, so cards land in the right spot
        // even if the rows don't use a layout group.
        Vector3[,] slotPositions = new Vector3[columns, rowCount];
        List<MemoryCard> allCards = new List<MemoryCard>();

        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (cards[c, r] == null) continue;
                slotPositions[c, r] = cards[c, r].card.localPosition;
                allCards.Add(cards[c, r]);
            }
        }

        // Fisher-Yates shuffle
        for (int i = allCards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (allCards[i], allCards[j]) = (allCards[j], allCards[i]);
        }

        // Put the shuffled cards back into the grid, slot by slot
        int index = 0;
        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (cards[c, r] == null) continue;

                MemoryCard memoryCard = allCards[index++];
                memoryCard.row = rows[r];
                memoryCard.card.SetParent(rows[r], false);
                memoryCard.card.SetSiblingIndex(c);
                memoryCard.card.localPosition = slotPositions[c, r];

                cards[c, r] = memoryCard;
            }
        }
    }

    private void CollectCards()
    {
        CollectCardsFromRow(firstRow, 0);
        CollectCardsFromRow(secondRow, 1);
        CollectCardsFromRow(thirdRow, 2);

    }

    private void CollectCardsFromRow(Transform row, int rowNumber)
    {
        for (int i = 0; i < row.childCount; i++)
        {
            cards[i, rowNumber] = new MemoryCard(row.GetChild(i), row);
        }
    }
    private IEnumerator ShowCards()
    {
        yield return null;
    }

    
    

    
}
