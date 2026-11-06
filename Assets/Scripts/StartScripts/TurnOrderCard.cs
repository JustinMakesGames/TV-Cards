using UnityEngine;

public class TurnOrderCard : BasicCardHandler, ICardHandler
{
    public int CardNumber { get; private set; }

    private TurnOrderHandler _turnOrderHandler;
    public void SetCardNumber(TurnOrderHandler turnOrderHandler, int cardNumber)
    {
        _turnOrderHandler = turnOrderHandler;
        CardNumber = cardNumber;
    }

    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);

        _turnOrderHandler.PlayerChoosesCard(player, gameObject, CardNumber);

    }

}
