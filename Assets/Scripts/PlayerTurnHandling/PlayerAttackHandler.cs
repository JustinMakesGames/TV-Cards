using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    [SerializeField] private GameObject cardFolder;
    private int _damage;
    
    public void StartAttack(List<GameObject> gridTiles, int range, int damage)
    {
        _damage = damage;
        foreach (GameObject tile in gridTiles)
        {
            if (tile.TryGetComponent(out GridTile gridTile))
            {
                Transform otherPlayer = gridTile.ReturnPlayer();
                if (!otherPlayer) continue;
                StealHearts(otherPlayer);
            }
        }
    }

    private void StealHearts(Transform otherPlayer)
    {
        if (otherPlayer.TryGetComponent(out PlayerData otherPlayerData))
        {
            otherPlayerData.ChangeHeartPointsAmount(-_damage);

        }

        if (TryGetComponent(out PlayerData playerData))
        {
            playerData.ChangeHeartPointsAmount(_damage);
        }

        if (TryGetComponent(out PlayerTurnHandling playerTurnHandling))
        {
            cardFolder?.SetActive(false);
            playerTurnHandling.PlayerChooseCard();
        }
    }
}
