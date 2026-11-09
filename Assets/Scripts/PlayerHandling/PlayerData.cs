using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int Coins { get; private set; }
    public int HeartPoints { get; private set; }

    [SerializeField] private GameObject gridPlayerObject;
    [SerializeField] private Transform playerCardFolder;
    public List<BasicCardHandler> playerCards;

    private Vector2Int _currentPlayerPosition;



    private void Start()
    {
        
    }

    public void ChangeCoinAmount(int coinNumber)
    {
        Coins += coinNumber;
    }

    public void ChangeHeartPointsAmount(int heartPoints)
    {
        HeartPoints += heartPoints;
    }

    public void GetBasicCard(CardStats card)
    {
        GameObject newCard = Instantiate(card.card, playerCardFolder);

        if (newCard.TryGetComponent(out BasicCardHandler cardHandler))
        {
            playerCards.Add(cardHandler);
        }

        else
        {
            Destroy(newCard);
        }
    }

    public void GetMovementCard(MovementCardStats card)
    {
        GameObject newCard = Instantiate(card.card, playerCardFolder);

        if (newCard.TryGetComponent(out MovementCardHandling cardHandler))
        {
            playerCards.Add(cardHandler);
            cardHandler.SetMovement(card.movement);
        }

        else
        {
            Destroy(newCard);
        }
    }

    public void RemoveBasicCard(BasicCardHandler cardHandler)
    {
        playerCards.Remove(cardHandler);
    }

    public void SetPosition(Vector2Int gridPosition)
    {
        Vector3 position = GridHandler.Instance.ReturnPosition(gridPosition);

        _currentPlayerPosition = gridPosition;

        gridPlayerObject.transform.position = position;
    }

    public Vector2Int ReturnPlayerPosition()
    {
        return _currentPlayerPosition;
    }

    public Transform ReturnPlayerObject()
    {
        return gridPlayerObject.transform;
    }
}
