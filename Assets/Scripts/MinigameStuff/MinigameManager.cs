using System.Linq;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{

    private void Start()
    {
        PlayerOrCPUChecker[] minigamePlayers = FindObjectsByType<PlayerOrCPUChecker>(FindObjectsSortMode.None)
            .Where(checker => checker.gameObject.scene == gameObject.scene)
            .ToArray();

        MinigameSession.ApplyMainGamePlayerSettings(minigamePlayers.Select(checker => checker.transform));

        IMinigameStart[] minigameManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .Where(behaviour => behaviour.gameObject.scene == gameObject.scene)
            .OfType<IMinigameStart>()
            .ToArray();

        minigameManagers[0].StartMinigame();
    }
}
