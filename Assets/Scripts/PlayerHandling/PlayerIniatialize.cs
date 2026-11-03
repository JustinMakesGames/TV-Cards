using UnityEngine;

public class PlayerIniatialize : MonoBehaviour
{
    [SerializeField] private Transform[] players;

    private void Start()
    {
        PlayerDialogueHolder.SetPlayers(players);
    }
}
