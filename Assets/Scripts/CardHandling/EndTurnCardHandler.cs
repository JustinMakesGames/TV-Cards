using UnityEngine;

public class EndTurnCardHandler : BasicCardHandler, ICardHandler
{
    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);
        GameManager.Instance.StartPlayerTurn();
    }
}
