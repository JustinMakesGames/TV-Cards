using UnityEngine;

public class EndTurnCardHandler : BasicCardHandler, ICardHandler
{
    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);
        transform.parent.gameObject.SetActive(false);
        GameManager.Instance.StartCoroutine(GameManager.Instance.StartPlayerTurn());
    }
}
