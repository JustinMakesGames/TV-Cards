using UnityEngine;

public class HeartHandler : CollectibleHandler
{
    public override void GetCollectible(Transform player)
    {
        base.GetCollectible(player);

        if (player.TryGetComponent(out PlayerData playerData))
        {
            playerData.ChangeHeartPointsAmount(1);
        }
    }
}
