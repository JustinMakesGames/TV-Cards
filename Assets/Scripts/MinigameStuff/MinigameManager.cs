using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{

    private void Start()
    {
        IMinigameStart[] minigameManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
    .OfType<IMinigameStart>()
    .ToArray();

        minigameManagers[0].StartMinigame();
    }
}
