using UnityEngine;

public class CoinHandler : CollectibleHandler
{
    public override void GetCollectible(Transform player)
    {
        base.GetCollectible(player);

        if (player.TryGetComponent(out PlayerData playerData))
        {
            playerData.ChangeCoinAmount(3);
        }
    }
}
