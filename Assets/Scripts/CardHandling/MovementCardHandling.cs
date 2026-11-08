using TMPro;
using UnityEngine;

public class MovementCardHandling : BasicCardHandler, ICardHandler
{
    [SerializeField] private TMP_Text movementText;

    private int _movement;

    public void SetMovement(int movement)
    {
        _movement = movement;
        movementText.text = movement.ToString();
    }

    public override void OnPressed(Transform player)
    {
        base.OnPressed(player);

        if (player.TryGetComponent(out PlayerMovementHandler playerMovementHandler))
        {
            playerMovementHandler.StartMovement(_movement);
        }

        transform.parent.gameObject?.SetActive(false);
    }
}
