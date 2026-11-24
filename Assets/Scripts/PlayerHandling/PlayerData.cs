using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string playerName;
    public Color playerColor = Color.white;
    public int Coins { get; private set; }
    public int HeartPoints { get; private set; }
    public Vector2Int currentPlayerPosition;

    [SerializeField] private GameObject gridPlayerObject;

    [SerializeField] private Transform movementCardFolder;
    [SerializeField] private Transform attackCardFolder;
    [SerializeField] private Transform eventCardFolder;

    [SerializeField] private TMP_Text heartPointsText;
    [SerializeField] private TMP_Text coinText;
 
    public List<BasicCardHandler> cardHandlers;
    public List<MovementCardHandling> movementCards;
    public List<AttackCardHandler> attackCards;




    private void Awake()
    {
        playerName = transform.name;
        DontDestroyOnLoad(gameObject);
    }
    public void ChangeCoinAmount(int coinNumber)
    {
        Coins += coinNumber;
        if (coinText) coinText.text = Coins.ToString();
    }

    public void ChangeHeartPointsAmount(int heartPoints)
    {
        HeartPoints += heartPoints;
        if (heartPointsText) heartPointsText.text = HeartPoints.ToString();
    }

    public void GetBasicCard(CardStats card)
    {
        GameObject newCard = Instantiate(card.card, eventCardFolder);

        if (newCard.TryGetComponent(out BasicCardHandler cardHandler))
        {
             cardHandlers.Add(cardHandler);
        }

        else
        {
            Destroy(newCard);
        }
    }

    public void GetMovementCard(MovementCardStats card)
    {
        GameObject newCard = Instantiate(card.card, movementCardFolder);

        if (newCard.TryGetComponent(out MovementCardHandling cardHandler))
        {
            movementCards.Add(cardHandler);
            cardHandler.SetMovement(card.movement);
        }

        else
        {
            Destroy(newCard);
        }
    }

    public void GetAttackCard(AttackCardStats card)
    {
        GameObject newCard = Instantiate(card.card, attackCardFolder);

        if (newCard.TryGetComponent(out AttackCardHandler cardHandler))
        {
            attackCards.Add(cardHandler);
            cardHandler.SetStats(transform, card.range, card.damage);
        }

        else
        {
            Destroy(newCard);
        }
    }

    public void RemoveBasicCard(BasicCardHandler cardHandler)
    {
        if (cardHandlers.Contains(cardHandler))
        {
            cardHandlers.Remove(cardHandler);
        }
    }

    public void RemoveMovementCard(MovementCardHandling movementCardHandling)
    {
        if (movementCards.Contains(movementCardHandling))
        {
            movementCards.Remove(movementCardHandling);
        }
    }

    public void RemoveAttackCard(AttackCardHandler attackCardHandler)
    {
        if (attackCards.Contains(attackCardHandler))
        {
            attackCards.Remove(attackCardHandler);
        }
    }

    public void SetPosition(Vector2Int gridPosition)
    {
        GameObject[,] grid = GridHandler.Instance.ReturnGrid();
        Vector3 position = GridHandler.Instance.ReturnPosition(gridPosition);

        if (grid[currentPlayerPosition.x, currentPlayerPosition.y].TryGetComponent(out GridTile oldTile) && oldTile.ReturnPlayer() == transform)
        {
            oldTile.DeletePlayuer();
        }

        currentPlayerPosition = gridPosition;

        gridPlayerObject.transform.position = position;

        if (grid[gridPosition.x, gridPosition.y].TryGetComponent(out GridTile gridTile))
        {
            gridTile.SetPlayer(transform);
        }
    }

   

    
    public Vector2Int ReturnPlayerPosition()
    {
        return currentPlayerPosition;
    }

    public Transform ReturnMovementCardFolder()
    {
        return movementCardFolder;
    }

    public Transform ReturnAttackCardFolder()
    {
        return attackCardFolder;
    }

    public Transform ReturnPlayerObject()
    {
        return gridPlayerObject.transform;
    }

    public void SavePlayerData()
    {
        PlayerSaveManager.SavePlayerData(this, playerName);
    }

    public void LoadPlayerData()
    {
        PlayerSaveData playerSaveData = PlayerSaveManager.LoadPlayerData(transform.name);

        SetPosition(new Vector2Int(playerSaveData.xPosition, playerSaveData.yPosition));


    }
}
