using System.Collections.Generic;
using UnityEngine;

public class CardRandomizeHandler : MonoBehaviour
{

    public static CardRandomizeHandler Instance;

    [SerializeField] private List<MovementCardStats> movementCards = new List<MovementCardStats>();
    [SerializeField] private List<AttackCardStats> attackCards = new List<AttackCardStats>();

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    public void GiveRandomMovementCard(Transform player)
    {
        if (player.TryGetComponent(out PlayerData playerData))
        {
            int randomCard = Random.Range(0, movementCards.Count);
            playerData.GetMovementCard(movementCards[randomCard]);
        }
    }

    public void GiveRandomAttackCard(Transform player)
    {

        if (player.TryGetComponent(out PlayerData playerData))
        {
            int randomCard = Random.Range(0, attackCards.Count);
            playerData.GetAttackCard(attackCards[randomCard]);
        }

    }
}
