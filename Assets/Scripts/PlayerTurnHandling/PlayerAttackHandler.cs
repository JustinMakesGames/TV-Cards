using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    [SerializeField] private GameObject cardFolder;

    public void StartAttack(List<GameObject> gridTiles, int range, int damage)
    {
        foreach (GameObject tile in gridTiles)
        {
            if (tile.TryGetComponent(out GridTile gridTile))
            {
                Transform otherPlayer = gridTile.ReturnPlayer();
                if (!otherPlayer || otherPlayer == transform) continue;
                StealHearts(otherPlayer, damage);
            }
        }


        if (cardFolder) cardFolder.SetActive(false);

        if (TryGetComponent(out PlayerTurnHandling playerTurnHandling))
        {
            playerTurnHandling.PlayerChooseCard();
        }
    }

    private void StealHearts(Transform otherPlayer, int damage)
    {
        if (otherPlayer.TryGetComponent(out PlayerData otherPlayerData))
        {
            otherPlayerData.ChangeHeartPointsAmount(-damage);
        }

        if (TryGetComponent(out PlayerData playerData))
        {
            playerData.ChangeHeartPointsAmount(damage);
        }
    }
}
