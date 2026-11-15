using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerData))]
public class CPUTargetHandler : MonoBehaviour
{
    private PlayerData _playerData;

    public PlayerData CurrentTarget { get; private set; }

    private void Awake()
    {
        _playerData = GetComponent<PlayerData>();
    }

    public PlayerData ChooseTarget()
    {
        List<PlayerData> closestPlayers = new List<PlayerData>();
        int closestDistance = int.MaxValue;

        foreach (Transform player in GameManager.Instance.ReturnPlayers())
        {
            if (player == null || player == transform) continue;
            if (!player.TryGetComponent(out PlayerData otherPlayerData)) continue;

            int distance = ReturnDistance(otherPlayerData.ReturnPlayerPosition());

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayers.Clear();
                closestPlayers.Add(otherPlayerData);
            }

            else if (distance == closestDistance)
            {
                closestPlayers.Add(otherPlayerData);
            }
        }

        CurrentTarget = closestPlayers.Count > 0 ? closestPlayers[Random.Range(0, closestPlayers.Count)] : null;
        return CurrentTarget;
    }

    public PlayerData ReturnTarget()
    {
        return CurrentTarget != null ? CurrentTarget : ChooseTarget();
    }

    public int ReturnDistanceToTarget()
    {
        PlayerData target = ReturnTarget();
        return target != null ? ReturnDistance(target.ReturnPlayerPosition()) : 0;
    }

    private int ReturnDistance(Vector2Int position)
    {
        Vector2Int difference = position - _playerData.ReturnPlayerPosition();
        return Mathf.Abs(difference.x) + Mathf.Abs(difference.y);
    }
}
