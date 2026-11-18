using System.Collections.Generic;
using UnityEngine;

// Shared board questions the CPU asks when it picks cards and plans its walk.
public static class CPUBoardHelper
{
    public static List<Vector2Int> ReturnOpponentPositions(Transform self)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        if (!GameManager.Instance || GameManager.Instance.ReturnPlayers() == null) return positions;

        foreach (Transform player in GameManager.Instance.ReturnPlayers())
        {
            if (player == null || player == self) continue;

            if (player.TryGetComponent(out PlayerData playerData))
            {
                positions.Add(playerData.ReturnPlayerPosition());
            }
        }

        return positions;
    }

    public static int ReturnDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Same shape as AttackCardHandler's range: a diamond around the attacker, without the attacker's own tile.
    public static bool CanHitFrom(Vector2Int from, int range, List<Vector2Int> opponents)
    {
        foreach (Vector2Int opponent in opponents)
        {
            int distance = ReturnDistance(from, opponent);
            if (distance > 0 && distance <= range) return true;
        }

        return false;
    }

    public static List<AttackCardHandler> ReturnAttackCards(PlayerData playerData)
    {
        List<AttackCardHandler> cards = new List<AttackCardHandler>();
        Transform folder = playerData.ReturnAttackCardFolder();
        if (!folder) return cards;

        foreach (Transform child in folder)
        {
            if (child.TryGetComponent(out AttackCardHandler card)) cards.Add(card);
        }

        return cards;
    }

    public static bool HasMovementCards(PlayerData playerData)
    {
        Transform folder = playerData.ReturnMovementCardFolder();
        if (!folder) return false;

        foreach (Transform child in folder)
        {
            if (child.TryGetComponent(out MovementCardHandling _)) return true;
        }

        return false;
    }
}
