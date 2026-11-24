using System.Collections.Generic;
using UnityEngine;

public struct MinigameResult
{
    public string playerName;
    public int placement; // 1 = first place

    public MinigameResult(string playerName, int placement)
    {
        this.playerName = playerName;
        this.placement = placement;
    }
}

/// <summary>
/// Shared state between the main game and a minigame scene that is loaded on top of it.
/// </summary>
public static class MinigameSession
{
    public static bool IsRunning { get; private set; }
    public static bool IsFinished { get; private set; }
    public static List<MinigameResult> Results { get; private set; } = new List<MinigameResult>();

    private static List<Transform> _mainGamePlayers = new List<Transform>();

    public static void Begin(List<Transform> mainGamePlayers)
    {
        _mainGamePlayers = new List<Transform>(mainGamePlayers);
        Results = new List<MinigameResult>();
        IsFinished = false;
        IsRunning = true;
    }

    public static void Finish(List<MinigameResult> results)
    {
        Results = results;
        IsFinished = true;

        foreach (MinigameResult result in results)
            Debug.Log($"Minigame result: {result.playerName} finished #{result.placement}");
    }

    public static void End()
    {
        IsRunning = false;
        IsFinished = false;
    }

    /// <summary>
    /// Copies settings from the main game players (like Player/CPU) onto the minigame players with the same name.
    /// </summary>
    public static void ApplyMainGamePlayerSettings(IEnumerable<Transform> minigamePlayers)
    {
        if (!IsRunning) return;

        foreach (Transform minigamePlayer in minigamePlayers)
        {
            Transform mainPlayer = _mainGamePlayers.Find(p => p != null && p.name == minigamePlayer.name);
            if (mainPlayer == null) continue;

            if (mainPlayer.TryGetComponent(out PlayerOrCPUChecker mainChecker) &&
                minigamePlayer.TryGetComponent(out PlayerOrCPUChecker minigameChecker))
            {
                minigameChecker.SetPlayer(mainChecker.PlayerOrCPU());
            }
        }
    }

    /// <summary>
    /// Builds placements from scores. Tied players share the same placement (e.g. 1, 1, 3, 4).
    /// </summary>
    public static List<MinigameResult> RankByScore(Dictionary<string, int> scores)
    {
        List<MinigameResult> results = new List<MinigameResult>();

        foreach (KeyValuePair<string, int> entry in scores)
        {
            int placement = 1;
            foreach (int otherScore in scores.Values)
                if (otherScore > entry.Value) placement++;

            results.Add(new MinigameResult(entry.Key, placement));
        }

        results.Sort((a, b) => a.placement.CompareTo(b.placement));
        return results;
    }
}
