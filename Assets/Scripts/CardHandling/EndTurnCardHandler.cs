using UnityEngine;

public class EndTurnCardHandler : BasicCardHandler, ICardHandler
{

    [SerializeField] private TransformEvent movementCardEvent;
    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);
        transform.parent.gameObject.SetActive(false);

        movementCardEvent?.Invoke(player);
        GameManager.Instance.StartCoroutine(GameManager.Instance.StartPlayerTurn());
    }
}
